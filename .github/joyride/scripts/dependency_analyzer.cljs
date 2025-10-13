(ns dependency-analyzer
  "Automated dependency component analysis for MTM WIP Application.
   Analyzes Services, Models, Converters, Behaviors, Extensions, and Core utilities.
   Extracts interface contracts, dependencies, DI registration, and usage patterns."
  (:require ["vscode" :as vscode]
            ["path" :as path]
            ["fs" :as fs]
            [promesa.core :as p]
            [clojure.string :as str]))

;; ============================================================================
;; Workspace Discovery
;; ============================================================================

(defn workspace-root
  "Get workspace root path."
  []
  (when-let [folders (.-workspaceFolders vscode/workspace)]
    (when (> (.-length folders) 0)
      (-> folders (aget 0) .-uri .-fsPath))))

(defn find-files-by-pattern
  "Find files matching glob pattern."
  [pattern]
  (p/let [uris (vscode/workspace.findFiles pattern)]
    (map #(.-fsPath %) (js->clj uris))))

;; ============================================================================
;; Component Discovery
;; ============================================================================

(defn discover-services
  "Find all service files in Services/ directory."
  []
  (find-files-by-pattern "Services/**/*.cs"))

(defn discover-models
  "Find all model files in Models/ directory."
  []
  (find-files-by-pattern "Models/**/*.cs"))

(defn discover-converters
  "Find all converter files in Converters/ directory."
  []
  (find-files-by-pattern "Converters/**/*.cs"))

(defn discover-behaviors
  "Find all behavior files in Behaviors/ directory."
  []
  (find-files-by-pattern "Behaviors/**/*.cs"))

(defn discover-extensions
  "Find all extension files in Extensions/ directory."
  []
  (find-files-by-pattern "Extensions/**/*.cs"))

(defn discover-core
  "Find all core utility files in Core/ directory."
  []
  (find-files-by-pattern "Core/**/*.cs"))

(defn discover-all-components
  "Discover all dependency components."
  []
  (p/let [services (discover-services)
          models (discover-models)
          converters (discover-converters)
          behaviors (discover-behaviors)
          extensions (discover-extensions)
          core (discover-core)]
    {:services services
     :models models
     :converters converters
     :behaviors behaviors
     :extensions extensions
     :core core
     :total (+ (count services) (count models) (count converters)
               (count behaviors) (count extensions) (count core))}))

;; ============================================================================
;; File Content Analysis
;; ============================================================================

(defn read-file-content
  "Read file content asynchronously."
  [file-path]
  (p/create
    (fn [resolve reject]
      (fs/readFile file-path "utf8"
        (fn [err data]
          (if err
            (reject err)
            (resolve data)))))))

(defn extract-namespace
  "Extract namespace from C# file."
  [content]
  (when-let [match (re-find #"namespace\s+([\w\.]+)" content)]
    (second match)))

(defn extract-class-name
  "Extract class name from C# file."
  [content]
  (when-let [match (re-find #"(?:public\s+)?(?:partial\s+)?(?:class|interface)\s+(\w+)" content)]
    (second match)))

(defn extract-interface-name
  "Extract implemented interface from C# class."
  [content]
  (when-let [match (re-find #"class\s+\w+\s*:\s*(\w+)" content)]
    (second match)))

(defn extract-constructor-dependencies
  "Extract constructor parameters as dependencies."
  [content class-name]
  (let [ctor-pattern (re-pattern (str "public\\s+" class-name "\\s*\\(([^)]*)\\)"))
        match (re-find ctor-pattern content)]
    (when match
      (let [params (second match)
            param-list (str/split params #",")]
        (map #(let [parts (str/split (str/trim %) #"\s+")]
                {:type (first parts)
                 :name (second parts)})
             (filter #(not (str/blank? %)) param-list))))))

(defn extract-public-methods
  "Extract public method signatures."
  [content]
  (let [method-pattern #"public\s+(?:async\s+)?(?:Task<?(\w*)>?|void|\w+)\s+(\w+)\s*\(([^)]*)\)"
        matches (re-seq method-pattern content)]
    (map (fn [[_ return-type method-name params]]
           {:return-type (or return-type "void")
            :method-name method-name
            :parameters params})
         matches)))

(defn extract-di-registration
  "Search for DI registration in ServiceCollectionExtensions."
  [class-name]
  (p/let [files (find-files-by-pattern "Extensions/ServiceCollectionExtensions.cs")]
    (if (empty? files)
      nil
      (p/let [content (read-file-content (first files))]
        (let [patterns [(re-pattern (str "AddSingleton<[^>]*" class-name))
                       (re-pattern (str "AddScoped<[^>]*" class-name))
                       (re-pattern (str "AddTransient<[^>]*" class-name))]
              matches (keep #(re-find % content) patterns)]
          (when (seq matches)
            (cond
              (str/includes? (first matches) "Singleton") :singleton
              (str/includes? (first matches) "Scoped") :scoped
              (str/includes? (first matches) "Transient") :transient
              :else :unknown)))))))

;; ============================================================================
;; Component Analysis
;; ============================================================================

(defn analyze-component
  "Deep analysis of a single component."
  [file-path component-type]
  (p/let [content (read-file-content file-path)
          namespace (extract-namespace content)
          class-name (extract-class-name content)
          interface-name (extract-interface-name content)
          dependencies (extract-constructor-dependencies content class-name)
          methods (extract-public-methods content)
          di-lifetime (when (= component-type :service)
                        (extract-di-registration class-name))]
    {:file-path file-path
     :component-type component-type
     :namespace namespace
     :class-name class-name
     :interface-name interface-name
     :dependencies dependencies
     :public-methods methods
     :di-lifetime di-lifetime
     :method-count (count methods)
     :dependency-count (count dependencies)}))

(defn batch-analyze-components
  "Analyze multiple components in batch."
  [file-paths component-type batch-size]
  (let [batches (partition-all batch-size file-paths)]
    (p/loop [remaining batches
             results []]
      (if (empty? remaining)
        results
        (p/let [batch (first remaining)
                batch-results (p/all (map #(analyze-component % component-type) batch))]
          (vscode/window.showInformationMessage
            (str "Analyzed " (count batch-results) " components..."))
          (p/recur (rest remaining)
                   (into results batch-results)))))))

(defn analyze-all-components
  "Analyze all discovered components."
  []
  (p/let [components (discover-all-components)
          _ (vscode/window.showInformationMessage
              (str "Discovered " (:total components) " components. Starting analysis..."))
          services-analyzed (batch-analyze-components (:services components) :service 10)
          models-analyzed (batch-analyze-components (:models components) :model 15)
          converters-analyzed (batch-analyze-components (:converters components) :converter 10)
          behaviors-analyzed (batch-analyze-components (:behaviors components) :behavior 10)
          extensions-analyzed (batch-analyze-components (:extensions components) :extension 10)
          core-analyzed (batch-analyze-components (:core components) :core 10)]
    {:services services-analyzed
     :models models-analyzed
     :converters converters-analyzed
     :behaviors behaviors-analyzed
     :extensions extensions-analyzed
     :core core-analyzed
     :total-analyzed (+ (count services-analyzed)
                        (count models-analyzed)
                        (count converters-analyzed)
                        (count behaviors-analyzed)
                        (count extensions-analyzed)
                        (count core-analyzed))}))

;; ============================================================================
;; Quick Analysis Functions
;; ============================================================================

(defn quick-component-summary
  "Quick summary analysis of a single component by name."
  [component-name]
  (p/let [all-files (p/all [(find-files-by-pattern (str "Services/**/" component-name ".cs"))
                            (find-files-by-pattern (str "Models/**/" component-name ".cs"))
                            (find-files-by-pattern (str "Converters/**/" component-name ".cs"))
                            (find-files-by-pattern (str "Behaviors/**/" component-name ".cs"))
                            (find-files-by-pattern (str "Extensions/**/" component-name ".cs"))
                            (find-files-by-pattern (str "Core/**/" component-name ".cs"))])
          found-files (filter seq (flatten all-files))]
    (if (empty? found-files)
      (do
        (vscode/window.showWarningMessage (str "Component not found: " component-name))
        nil)
      (p/let [file-path (first found-files)
              component-type (cond
                               (str/includes? file-path "Services") :service
                               (str/includes? file-path "Models") :model
                               (str/includes? file-path "Converters") :converter
                               (str/includes? file-path "Behaviors") :behavior
                               (str/includes? file-path "Extensions") :extension
                               (str/includes? file-path "Core") :core
                               :else :unknown)
              analysis (analyze-component file-path component-type)]
        (vscode/window.showInformationMessage
          (str "Component: " (:class-name analysis)
               " | Type: " (name component-type)
               " | Methods: " (:method-count analysis)
               " | Dependencies: " (:dependency-count analysis)
               (when (:di-lifetime analysis)
                 (str " | DI: " (name (:di-lifetime analysis))))))
        analysis))))

;; ============================================================================
;; Report Generation
;; ============================================================================

(defn format-component-for-inventory
  "Format component analysis for inventory table row."
  [component]
  (str "| " (:class-name component)
       " | " (name (:component-type component))
       " | `" (:file-path component) "`"
       " | " (or (:interface-name component) "N/A")
       " | " (:dependency-count component) " dependencies"
       " | " (if (:di-lifetime component) (name (:di-lifetime component)) "N/A")
       " | " (:method-count component) " methods"
       " | TODO: Document consumers"
       " | TODO: Add purpose summary"
       " | TODO: Add notes |"))

(defn generate-inventory-report
  "Generate dependency-inventory.md content."
  [analysis-results]
  (let [all-components (concat (:services analysis-results)
                               (:models analysis-results)
                               (:converters analysis-results)
                               (:behaviors analysis-results)
                               (:extensions analysis-results)
                               (:core analysis-results))]
    (str "# Dependency Inventory\n\n"
         "**Generated**: " (.toISOString (js/Date.)) "\n"
         "**Total Components**: " (:total-analyzed analysis-results) "\n\n"
         "## Component Summary\n\n"
         "- Services: " (count (:services analysis-results)) "\n"
         "- Models: " (count (:models analysis-results)) "\n"
         "- Converters: " (count (:converters analysis-results)) "\n"
         "- Behaviors: " (count (:behaviors analysis-results)) "\n"
         "- Extensions: " (count (:extensions analysis-results)) "\n"
         "- Core: " (count (:core analysis-results)) "\n\n"
         "## Component Inventory Table\n\n"
         "| Component Name | Type | File Path | Interface | Dependencies | DI Lifetime | Methods | Consumers | Purpose | Notes |\n"
         "|----------------|------|-----------|-----------|--------------|-------------|---------|-----------|---------|-------|\n"
         (str/join "\n" (map format-component-for-inventory all-components))
         "\n\n"
         "## Analysis Details\n\n"
         "**TODO**: Complete the following for each component:\n"
         "- Add purpose summary\n"
         "- Identify consumers (ViewModels, other services)\n"
         "- Document any quirks or edge cases\n"
         "- Verify DI lifetime is correct\n\n")))

(defn save-analysis-report
  "Analyze all components and save inventory report."
  []
  (p/let [root (workspace-root)
          _ (when (not root)
              (throw (js/Error. "No workspace folder open")))
          analysis (analyze-all-components)
          report-content (generate-inventory-report analysis)
          output-dir (path/join root "specs" "000-dependency-capture" "dependencies")
          output-file (path/join output-dir "dependency-inventory.md")]
    ;; Create directory if doesn't exist
    (p/create
      (fn [resolve reject]
        (fs/mkdir output-dir #js {:recursive true}
          (fn [err]
            (if err
              (reject err)
              (resolve nil))))))
    ;; Write report
    (p/create
      (fn [resolve reject]
        (fs/writeFile output-file report-content
          (fn [err]
            (if err
              (reject err)
              (do
                (vscode/window.showInformationMessage
                  (str "Analysis complete! Saved to: " output-file))
                (resolve output-file)))))))
    {:analysis analysis
     :report-path output-file}))

;; ============================================================================
;; Public API
;; ============================================================================

(comment
  "Usage examples:"
  
  ;; Discover all components
  (discover-all-components)
  
  ;; Quick summary of specific component
  (quick-component-summary "DatabaseService")
  
  ;; Analyze all and save report
  (save-analysis-report)
  
  ;; Analyze specific component
  (analyze-component "Services/Database.cs" :service))
