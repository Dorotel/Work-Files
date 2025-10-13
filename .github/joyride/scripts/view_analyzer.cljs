```clojure
(ns view-analyzer
  "Automated analysis of Avalonia AXAML views and ViewModels
  
  Extracts structured data from view files to accelerate spec generation:
  - AXAML layout hierarchy
  - Binding expressions with MVVM Toolkit traceability
  - Command definitions and CanExecute logic
  - Navigation triggers
  - Resource dependencies"
  (:require ["vscode" :as vscode]
            ["path" :as path]
            ["fs" :as fs]
            [promesa.core :as p]
            [clojure.string :as str]
            [joyride.core :as joyride]))

(def workspace-root
  "Get workspace root path"
  (.. vscode/workspace -workspaceFolders (at 0) -uri -fsPath))

(defn find-all-views
  "Discover all .axaml view files in the project
  
  Returns: Promise resolving to vector of file URIs"
  []
  (p/let [views-files (vscode/workspace.findFiles "Views/**/*.axaml")
          controls-files (vscode/workspace.findFiles "Controls/**/*.axaml")
          overlays-files (vscode/workspace.findFiles "Views/*/*.axaml")
          root-files (vscode/workspace.findFiles "*.axaml")]
    (let [to-vec (fn [items]
                   (if items
                     (js->clj items :keywordize-keys false)
                     []))
          all-files (remove nil?
                            (concat (to-vec views-files)
                                    (to-vec controls-files)
                                    (to-vec overlays-files)
                                    (to-vec root-files)))]
      (vec (distinct all-files)))))

(defn read-file-content
  "Read content of a file from URI
  
  Parameters:
  - file-uri: VS Code URI object
  
  Returns: Promise resolving to file content string"
  [file-uri]
  (p/let [doc (vscode/workspace.openTextDocument file-uri)]
    (.getText doc)))

(defn extract-view-name
  "Extract view name from file path
  
  Parameters:
  - file-path: String path to .axaml file
  
  Returns: View name (e.g., 'MainWindow' from 'Views/MainWindow.axaml')"
  [file-path]
  (-> file-path
      (str/replace #".*[/\\]" "")  ; Get filename
      (str/replace #"\.axaml$" ""))) ; Remove extension

(defn find-viewmodel-file
  "Find corresponding ViewModel file for a view
  
  Parameters:
  - view-name: String name of view (e.g., 'MainWindow')
  
  Returns: Promise resolving to ViewModel file URI or nil"
  [view-name]
  (p/let [vm-pattern (str workspace-root "/ViewModels/**/" view-name "ViewModel.cs")
          files (vscode/workspace.findFiles vm-pattern)]
    (first files)))

(defn extract-bindings
  "Extract binding expressions from AXAML content
  
  Parameters:
  - axaml-content: String content of .axaml file
  
  Returns: Vector of maps with :control, :property, :binding-path, :mode"
  [axaml-content]
  (let [binding-pattern #"\{Binding\s+([^,}]+)(?:,\s*Mode=(\w+))?\}"
        lines (str/split-lines axaml-content)
        bindings (atom [])]
    (doseq [[idx line] (map-indexed vector lines)]
      (when-let [matches (re-seq binding-pattern line)]
        (doseq [[full-match path mode] matches]
          (swap! bindings conj
                 {:line (inc idx)
                  :binding-path path
                  :mode (or mode "OneWay")
                  :context line}))))
    @bindings))

(defn extract-commands
  "Extract command bindings from AXAML content
  
  Parameters:
  - axaml-content: String content of .axaml file
  
  Returns: Vector of maps with :control-name, :command-path"
  [axaml-content]
  (let [command-pattern #"Command=\"\{Binding\s+([^}]+)\}\""
        lines (str/split-lines axaml-content)
        commands (atom [])]
    (doseq [[idx line] (map-indexed vector lines)]
      (when-let [matches (re-seq command-pattern line)]
        (doseq [[full-match command-path] matches]
          (swap! commands conj
                 {:line (inc idx)
                  :command-path command-path
                  :context line}))))
    @commands))

(defn extract-datacontext
  "Extract DataContext declaration from AXAML content
  
  Parameters:
  - axaml-content: String content of .axaml file
  
  Returns: String with ViewModel type or nil"
  [axaml-content]
  (when-let [match (re-find #"x:DataType=\"([^\"]+)\"" axaml-content)]
    (second match)))

(defn extract-layout-structure
  "Extract high-level layout structure from AXAML
  
  Parameters:
  - axaml-content: String content of .axaml file
  
  Returns: Map with :root-container and :major-regions"
  [axaml-content]
  (let [root-pattern #"<(Grid|StackPanel|DockPanel|Border|ScrollViewer|ContentControl)"
        region-names (re-seq #"x:Name=\"([^\"]+)\"" axaml-content)
        root-match (re-find root-pattern axaml-content)]
    {:root-container (when root-match (second root-match))
     :major-regions (mapv second region-names)}))

(defn extract-resource-references
  "Extract ResourceDictionary and StaticResource references
  
  Parameters:
  - axaml-content: String content of .axaml file
  
  Returns: Map with :resource-dictionaries and :static-resources"
  [axaml-content]
  {:resource-dictionaries (mapv second (re-seq #"<ResourceDictionary Source=\"([^\"]+)\"" axaml-content))
   :static-resources (mapv second (re-seq #"\{StaticResource\s+([^}]+)\}" axaml-content))
   :dynamic-resources (mapv second (re-seq #"\{DynamicResource\s+([^}]+)\}" axaml-content))})

(defn analyze-viewmodel
  "Analyze ViewModel C# file for MVVM Toolkit patterns
  
  Parameters:
  - vm-content: String content of ViewModel.cs file
  
  Returns: Map with :observable-properties and :relay-commands"
  [vm-content]
  (let [prop-pattern #"\[ObservableProperty\][^\n]*\n\s*private\s+(\w+)\s+_(\w+)"
        cmd-pattern #"\[RelayCommand(?:\([^\)]*\))?\][^\n]*\n\s*(?:private|public)\s+(?:async\s+)?(?:Task|void)\s+(\w+)\("
        properties (mapv (fn [[_ type name]] {:type type :field (str "_" name) :property (str/capitalize name)})
                        (re-seq prop-pattern vm-content))
        commands (mapv (fn [[_ method]] {:method method :command (str method "Command")})
                      (re-seq cmd-pattern vm-content))]
    {:observable-properties properties
     :relay-commands commands
     :has-mvvm-toolkit? (or (seq properties) (seq commands))}))

(defn analyze-view
  "Complete analysis of a view file and its ViewModel
  
  Parameters:
  - view-uri: VS Code URI for .axaml file
  
  Returns: Promise resolving to comprehensive analysis map"
  [view-uri]
  (p/let [view-path (.-fsPath view-uri)
          view-name (extract-view-name view-path)
          axaml-content (read-file-content view-uri)
          vm-uri (find-viewmodel-file view-name)
          vm-content (when vm-uri (read-file-content vm-uri))
          
          ;; Extract AXAML data
          bindings (extract-bindings axaml-content)
          commands (extract-commands axaml-content)
          datacontext (extract-datacontext axaml-content)
          layout (extract-layout-structure axaml-content)
          resources (extract-resource-references axaml-content)
          
          ;; Extract ViewModel data
          vm-analysis (when vm-content (analyze-viewmodel vm-content))]
    
    {:view-name view-name
     :view-path view-path
     :viewmodel-path (when vm-uri (.-fsPath vm-uri))
     :datacontext datacontext
     :layout layout
     :bindings bindings
     :commands commands
     :resources resources
     :viewmodel vm-analysis
     :analysis-timestamp (.toISOString (js/Date.))}))

(defn analyze-all-views
  "Analyze all views in the project with progress reporting
  
  Returns: Promise resolving to vector of analysis maps"
  []
  (p/let [view-files (find-all-views)
          total (count view-files)]
    (vscode/window.withProgress
      #js {:title (str "Analyzing " total " views")
           :location (.-Notification vscode/ProgressLocation)
           :cancellable true}
      (fn [progress token]
        (let [increment (if (pos? total) (/ 100 total) 0)]
          (p/loop [remaining view-files
                   index 0
                   results []]
            (cond
              (.-isCancellationRequested token)
              (p/rejected (js/Error. "View analysis cancelled"))

              (empty? remaining)
              (p/resolved results)

              :else
              (let [view-uri (first remaining)
                    view-name (extract-view-name (.-fsPath view-uri))]
                (.report progress
                         #js {:message (str "Analyzing " view-name)
                              :increment increment})
                (p/let [analysis (analyze-view view-uri)]
                  (p/recur (rest remaining)
                           (inc index)
                           (conj results analysis)))))))))))

(defn generate-analysis-report
  "Generate markdown report from view analyses
  
  Parameters:
  - analyses: Vector of view analysis maps
  
  Returns: String with markdown report"
  [analyses]
  (str "# View Analysis Report\n\n"
       "**Generated**: " (.toISOString (js/Date.)) "\n"
       "**Total Views**: " (count analyses) "\n\n"
       "## Summary\n\n"
       "| View | ViewModel | Bindings | Commands | Resources |\n"
       "|------|-----------|----------|----------|----------|\n"
       (str/join "\n"
         (map (fn [{:keys [view-name viewmodel-path bindings commands resources]}]
                (str "| " view-name 
                     " | " (if viewmodel-path "✓" "✗")
                     " | " (count bindings)
                     " | " (count commands)
                     " | " (+ (count (:resource-dictionaries resources))
                             (count (:static-resources resources))
                             (count (:dynamic-resources resources)))
                     " |"))
              analyses))
       "\n\n## MVVM Toolkit Usage\n\n"
       (let [with-mvvm (filter #(get-in % [:viewmodel :has-mvvm-toolkit?]) analyses)]
         (str "Views using MVVM Toolkit: " (count with-mvvm) " / " (count analyses) "\n"
              "Coverage: " (int (* 100 (/ (count with-mvvm) (count analyses)))) "%\n"))
       "\n\n## Detailed Analysis\n\n"
       (str/join "\n\n"
         (map (fn [{:keys [view-name bindings commands viewmodel]}]
                (str "### " view-name "\n\n"
                     "**Bindings**: " (count bindings) "\n"
                     (when (seq bindings)
                       (str "- " (str/join "\n- " (map :binding-path (take 5 bindings)))
                            (when (> (count bindings) 5) "\n- ...")))
                     "\n\n**Commands**: " (count commands) "\n"
                     (when (seq commands)
                       (str "- " (str/join "\n- " (map :command-path (take 5 commands)))
                            (when (> (count commands) 5) "\n- ...")))
                     (when viewmodel
                       (str "\n\n**MVVM Toolkit**:\n"
                            "- Observable Properties: " (count (:observable-properties viewmodel)) "\n"
                            "- Relay Commands: " (count (:relay-commands viewmodel))))))
              analyses))))

(defn save-analysis-report
  "Save analysis report to file
  
  Returns: Promise resolving to file URI or nil if cancelled"
  []
  (-> (p/let [analyses (analyze-all-views)
              report (generate-analysis-report analyses)
              output-path (str workspace-root "/specs/000-ui-capture/analysis-report.md")
              uri (vscode/Uri.file output-path)
              _ (joyride.core/slurp output-path) ;; Ensure directory exists
              doc (vscode/workspace.openTextDocument uri)]
        (p/let [editor (vscode/window.showTextDocument doc)
                edit (vscode/WorkspaceEdit.)]
          (.replace edit uri 
                   (vscode/Range. (vscode/Position. 0 0) 
                                 (vscode/Position. 999999 0))
                   report)
          (vscode/workspace.applyEdit edit)
          (vscode/window.showInformationMessage 
            (str "✓ Analysis report saved: " output-path))
          uri))
      (p/catch (fn [err]
                 (let [message (.-message err)]
                   (if (= message "View analysis cancelled")
                     (vscode/window.showWarningMessage "Analysis cancelled by user")
                     (vscode/window.showErrorMessage (str "Analysis failed: " message))))
                 nil))))

(defn quick-view-summary
  "Display quick summary of a single view
  
  Parameters:
  - view-name: String name of view to analyze
  
  Returns: Promise resolving to analysis displayed in info message"
  [view-name]
  (p/let [pattern (str workspace-root "/Views/**/" view-name ".axaml")
          files (vscode/workspace.findFiles pattern)
          view-uri (first files)]
    (if view-uri
      (p/let [analysis (analyze-view view-uri)]
        (vscode/window.showInformationMessage
          (str "📊 " view-name " Analysis\n\n"
               "ViewModel: " (or (:datacontext analysis) "Not specified") "\n"
               "Layout: " (get-in analysis [:layout :root-container]) "\n"
               "Bindings: " (count (:bindings analysis)) "\n"
               "Commands: " (count (:commands analysis)) "\n"
               "Resources: " (count (get-in analysis [:resources :resource-dictionaries])) " dictionaries\n"
               (when (get-in analysis [:viewmodel :has-mvvm-toolkit?])
                 "✓ Uses MVVM Community Toolkit"))
          #js {:modal true}))
      (vscode/window.showErrorMessage (str "View not found: " view-name)))))

(comment
  ;; Usage examples (evaluate in Joyride REPL)
  
  ;; Analyze all views and generate report
  (save-analysis-report)
  
  ;; Quick summary of a single view
  (quick-view-summary "MainWindow")
  (quick-view-summary "InventoryTabView")
  
  ;; Get raw analysis data
  (p/let [analyses (analyze-all-views)]
    (def all-analyses analyses)
    (vscode/window.showInformationMessage 
      (str "Analyzed " (count analyses) " views")))
  
  ;; Filter views without ViewModels
  (p/let [analyses (analyze-all-views)
          without-vm (filter #(nil? (:viewmodel-path %)) analyses)]
    (vscode/window.showWarningMessage
      (str "Views without ViewModel: " (str/join ", " (map :view-name without-vm))))))

```
