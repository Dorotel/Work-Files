```clojure
(ns progress-tracker
  "Progress tracking and reporting for reverse-spec workflow
  
  Monitors completion status of:
  - View analysis
  - Spec file generation
  - Manual review completion
  - Overall workflow progress"
  (:require ["vscode" :as vscode]
            ["path" :as path]
            ["fs" :as fs]
            [promesa.core :as p]
            [clojure.string :as str]
            [joyride.core :as joyride]
            [view-analyzer :as va]))

;; Helper for formatting numbers (format not available in SCI)
(defn fmt-decimal
  "Format a number to 1 decimal place"
  [n]
  (let [rounded (* (js/Math.round (* n 10)) 0.1)]
    (str (.toFixed rounded 1))))

(def workspace-root va/workspace-root)

(defn check-file-exists
  "Check if a file exists
  
  Parameters:
  - file-path: String path to file
  
  Returns: Promise resolving to boolean"
  [file-path]
  (p/promise
    (fn [resolve reject]
      (fs/access file-path fs/constants.F_OK
                (fn [err]
                  (resolve (nil? err)))))))

(defn count-todo-markers
  "Count TODO markers in a spec file
  
  Parameters:
  - file-path: String path to spec file
  
  Returns: Promise resolving to count of TODO markers"
  [file-path]
  (p/let [exists? (check-file-exists file-path)]
    (if-not exists?
      0
      (p/let [content (joyride.core/slurp file-path)
              todos (re-seq #"\*\*TODO\*\*" content)]
        (count todos)))))

(defn analyze-spec-completeness
  "Analyze completeness of a spec file
  
  Parameters:
  - view-name: String name of view
  
  Returns: Promise resolving to map with :exists?, :todo-count, :completion-pct"
  [view-name]
  (p/let [spec-path (str workspace-root "/specs/000-ui-capture/ui/" view-name "/spec.md")
          exists? (check-file-exists spec-path)
          todo-count (if exists? (count-todo-markers spec-path) 0)
          
          ;; Estimate: ~12 TODO sections in template
          total-sections 12
          completed-sections (- total-sections todo-count)
          completion-pct (if exists?
                          (int (* 100 (/ completed-sections total-sections)))
                          0)]
    {:view-name view-name
     :spec-path spec-path
     :exists? exists?
     :todo-count todo-count
     :completion-pct completion-pct
     :status (cond
              (not exists?) :not-started
              (> todo-count 8) :minimal
              (> todo-count 3) :partial
              (> todo-count 0) :nearly-complete
              :else :complete)}))

(defn check-all-deliverables
  "Check status of all major deliverables
  
  Returns: Promise resolving to map with deliverable statuses"
  []
  (p/let [spec-path (str workspace-root "/specs/000-ui-capture/spec.md")
          inventory-path (str workspace-root "/specs/000-ui-capture/ui/ui-inventory.md")
          global-ux-path (str workspace-root "/specs/000-ui-capture/ui/global-ux.md")
          checklist-path (str workspace-root "/specs/000-ui-capture/ui/ui-audit-checklist.md")
          analysis-path (str workspace-root "/specs/000-ui-capture/analysis-report.md")
          
          spec-exists? (check-file-exists spec-path)
          inventory-exists? (check-file-exists inventory-path)
          global-ux-exists? (check-file-exists global-ux-path)
          checklist-exists? (check-file-exists checklist-path)
          analysis-exists? (check-file-exists analysis-path)]
    {:feature-spec {:path spec-path :exists? spec-exists?}
     :ui-inventory {:path inventory-path :exists? inventory-exists?}
     :global-ux {:path global-ux-path :exists? global-ux-exists?}
     :audit-checklist {:path checklist-path :exists? checklist-exists?}
     :analysis-report {:path analysis-path :exists? analysis-exists?}}))

(defn generate-progress-report
  "Generate comprehensive progress report
  
  Returns: Promise resolving to map with complete progress data"
  []
  (p/let [analyses (va/analyze-all-views)
          view-names (map :view-name analyses)
          
          ;; Check per-view spec completion
          spec-completeness (p/all (map analyze-spec-completeness view-names))
          
          ;; Check major deliverables
          deliverables (check-all-deliverables)
          
          ;; Calculate statistics
          total-views (count view-names)
          specs-exist (count (filter :exists? spec-completeness))
          specs-complete (count (filter #(= :complete (:status %)) spec-completeness))
          specs-partial (count (filter #(#{:partial :nearly-complete} (:status %)) spec-completeness))
          specs-minimal (count (filter #(= :minimal (:status %)) spec-completeness))
          specs-missing (- total-views specs-exist)
          
          avg-completion (if (pos? specs-exist)
                          (int (/ (reduce + (map :completion-pct spec-completeness))
                                 specs-exist))
                          0)
          
          overall-pct (int (* 100 (/ (+ specs-complete (* 0.5 specs-partial) (* 0.2 specs-minimal))
                                    total-views)))]
    
    {:timestamp (.toISOString (js/Date.))
     :total-views total-views
     :specs-exist specs-exist
     :specs-complete specs-complete
     :specs-partial specs-partial
     :specs-minimal specs-minimal
     :specs-missing specs-missing
     :avg-completion avg-completion
     :overall-completion overall-pct
     :deliverables deliverables
     :per-view-status spec-completeness}))

(defn format-progress-report
  "Format progress report as markdown
  
  Parameters:
  - progress: Progress report map
  
  Returns: String with formatted markdown report"
  [progress]
  (let [{:keys [timestamp total-views specs-exist specs-complete specs-partial 
                specs-minimal specs-missing avg-completion overall-completion
                deliverables per-view-status]} progress]
    (str "# Reverse-Spec Progress Report\n\n"
         "**Generated**: " timestamp "\n\n"
         "## Overall Progress: " overall-completion "%\n\n"
         "### Per-View Specs\n\n"
         "- **Total Views**: " total-views "\n"
         "- **Specs Generated**: " specs-exist " (" (int (* 100 (/ specs-exist total-views))) "%)\n"
         "- **Complete**: " specs-complete " (" (int (* 100 (/ specs-complete total-views))) "%)\n"
         "- **Partial**: " specs-partial " (" (int (* 100 (/ specs-partial total-views))) "%)\n"
         "- **Minimal**: " specs-minimal " (" (int (* 100 (/ specs-minimal total-views))) "%)\n"
         "- **Missing**: " specs-missing " (" (int (* 100 (/ specs-missing total-views))) "%)\n"
         "- **Avg Completion**: " avg-completion "%\n\n"
         "### Major Deliverables\n\n"
         (str/join "\n"
           (map (fn [[name {:keys [exists? path]}]]
                  (str "- " (if exists? "✅" "❌") " **" (str/replace (str name) #":" "") "**"))
                deliverables))
         "\n\n### Per-View Status\n\n"
         "| View | Status | Completion | TODOs |\n"
         "|------|--------|------------|-------|\n"
         (str/join "\n"
           (map (fn [{:keys [view-name status completion-pct todo-count exists?]}]
                  (str "| " view-name 
                       " | " (cond
                              (= status :complete) "✅ Complete"
                              (= status :nearly-complete) "🟡 Nearly Complete"
                              (= status :partial) "🟠 Partial"
                              (= status :minimal) "🔴 Minimal"
                              :else "❌ Not Started")
                       " | " completion-pct "%"
                       " | " (if exists? todo-count "N/A")
                       " |"))
                per-view-status))
         "\n\n### Phase Completion Estimate\n\n"
         (cond
           (< overall-completion 25)
           "**Phase 1**: Discovery & Inventory (In Progress)\n"
           
           (< overall-completion 75)
           "**Phase 2**: Per-View Analysis (In Progress)\n"
           
           (< overall-completion 95)
           "**Phase 3**: Global Synthesis (Ready to Start)\n"
           
           :else
           "**Phase 4**: Audit & Validation (Ready to Start)\n")
         "\n### Next Actions\n\n"
         (cond
           (pos? specs-missing)
           (str "1. Generate " specs-missing " missing spec files\n"
                "2. Run: `(sg/generate-all-specs)`\n")
           
           (> (+ specs-minimal specs-partial) (* 0.5 total-views))
           "1. Complete TODO sections in partial specs\n2. Focus on views with highest TODO counts\n"
           
           (and (get-in deliverables [:feature-spec :exists?])
                (not (get-in deliverables [:global-ux :exists?])))
           "1. Generate global-ux.md\n2. Switch to Claude Sonnet 4.5 for Phase 3\n"
           
           :else
           "1. Final audit with ui-audit-checklist.md\n2. Workflow nearly complete!\n")
         "\n---\n\n"
         "*Run `(pt/show-progress-dashboard)` for interactive dashboard*\n"
         "*Run `(pt/save-progress-report)` to save this report*\n")))

(defn save-progress-report
  "Save progress report to file
  
  Returns: Promise resolving to file URI"
  []
  (p/let [progress (generate-progress-report)
          report (format-progress-report progress)
          output-path (str workspace-root "/specs/000-ui-capture/progress-report.md")
          uri (vscode/Uri.file output-path)]
    (p/promise
      (fn [resolve reject]
        (fs/writeFile output-path report
                     (fn [err]
                       (if err 
                         (reject err)
                         (do
                           (vscode/window.showInformationMessage 
                             (str "✓ Progress report saved: " output-path))
                           (resolve uri)))))))))

(defn show-progress-dashboard
  "Display interactive progress dashboard
  
  Returns: Promise resolving to user action"
  []
  (p/let [progress (generate-progress-report)
          {:keys [overall-completion specs-exist specs-missing total-views
                  deliverables]} progress
          
          message (str "📊 Reverse-Spec Progress Dashboard\n\n"
                      "Overall: " overall-completion "%\n"
                      "Per-View Specs: " specs-exist "/" total-views " generated\n\n"
                      "Deliverables:\n"
                      (str/join "\n"
                        (map (fn [[name {:keys [exists?]}]]
                               (str (if exists? "✅" "❌") " " 
                                    (str/replace (str name) #":" "")))
                             deliverables))
                      "\n\nWhat would you like to do?")
          
          action (vscode/window.showInformationMessage
                   message
                   #js {:modal true}
                   "Save Report"
                   "Generate Missing Specs"
                   "View Detailed Report"
                   "Close")]
    
    (case action
      "Save Report" 
      (save-progress-report)
      
      "Generate Missing Specs"
      (do
        (require '[spec-generator :as sg])
        ((resolve 'spec-generator/generate-all-specs)))
      
      "View Detailed Report"
      (p/let [report (format-progress-report progress)
              doc (vscode/workspace.openTextDocument 
                    (vscode/Uri.parse "untitled:Progress-Report.md"))]
        (p/let [editor (vscode/window.showTextDocument doc)]
          (p/let [edit (vscode/WorkspaceEdit.)]
            (.insert edit (.-uri doc) (vscode/Position. 0 0) report)
            (vscode/workspace.applyEdit edit))))
      
      "Close"
      nil)))

(defn show-incomplete-views
  "Display list of views with incomplete specs
  
  Returns: Promise resolving to user selection of view to edit"
  []
  (p/let [progress (generate-progress-report)
          {:keys [per-view-status]} progress
          incomplete (filter #(not= :complete (:status %)) per-view-status)
          
          ;; Sort by completion percentage (least complete first)
          sorted (sort-by :completion-pct incomplete)
          
          items (mapv (fn [{:keys [view-name completion-pct todo-count status]}]
                       #js {:label view-name
                            :description (str completion-pct "% - " todo-count " TODOs")
                            :detail (str "Status: " (name status))})
                     sorted)]
    
    (if (empty? items)
      (vscode/window.showInformationMessage "🎉 All specs are complete!")
      (p/let [selection (vscode/window.showQuickPick 
                         (clj->js items)
                         #js {:placeHolder "Select view to edit spec"
                              :title (str (count incomplete) " incomplete specs")})]
        (when selection
          (let [view-name (.-label selection)
                spec-path (str workspace-root "/specs/000-ui-capture/ui/" view-name "/spec.md")
                uri (vscode/Uri.file spec-path)]
            (p/let [doc (vscode/workspace.openTextDocument uri)]
              (vscode/window.showTextDocument doc))))))))

(defn estimate-remaining-work
  "Estimate remaining work based on current progress
  
  Returns: Promise resolving to work estimate map"
  []
  (p/let [progress (generate-progress-report)
          {:keys [overall-completion specs-missing specs-partial specs-minimal
                  deliverables]} progress
          
          ;; Time estimates per task type (minutes)
          time-per-missing-spec 15
          time-per-partial-spec 10
          time-per-minimal-spec 5
          time-per-deliverable 30
          
          missing-deliverables (count (filter #(not (:exists? (second %))) deliverables))
          
          spec-work-min (+ (* specs-missing time-per-missing-spec)
                          (* specs-partial time-per-partial-spec)
                          (* specs-minimal time-per-minimal-spec))
          deliverable-work-min (* missing-deliverables time-per-deliverable)
          total-min (+ spec-work-min deliverable-work-min)
          total-hours (/ total-min 60.0)
          
          estimate {:remaining-hours (fmt-decimal total-hours)
                   :remaining-minutes total-min
                   :spec-work-hours (fmt-decimal (/ spec-work-min 60.0))
                   :deliverable-work-hours (fmt-decimal (/ deliverable-work-min 60.0))
                   :specs-remaining (+ specs-missing specs-partial specs-minimal)
                   :deliverables-remaining missing-deliverables}]
    
    (vscode/window.showInformationMessage
      (str "⏱️ Remaining Work Estimate\n\n"
           "Total: ~" (:remaining-hours estimate) " hours\n\n"
           "Breakdown:\n"
           "- Per-view specs: ~" (:spec-work-hours estimate) " hours (" (:specs-remaining estimate) " specs)\n"
           "- Major deliverables: ~" (:deliverable-work-hours estimate) " hours (" (:deliverables-remaining estimate) " files)\n\n"
           "At 50% completion: ~" (fmt-decimal (/ total-hours 2)) " hours remain\n"
           "Timeline: " (cond 
                         (< total-hours 2) "Can finish today"
                         (< total-hours 8) "1 work day"
                         (< total-hours 16) "2 work days"
                         :else "3+ work days"))
      #js {:modal true})
    
    estimate))

(comment
  ;; Usage examples (evaluate in Joyride REPL)
  
  ;; Show interactive dashboard
  (show-progress-dashboard)
  
  ;; Save progress report to file
  (save-progress-report)
  
  ;; View incomplete specs
  (show-incomplete-views)
  
  ;; Estimate remaining work
  (estimate-remaining-work)
  
  ;; Check specific view
  (p/let [status (analyze-spec-completeness "MainWindow")]
    (vscode/window.showInformationMessage
      (str "MainWindow: " (:completion-pct status) "% complete")))
  
  ;; Generate and view progress
  (p/let [progress (generate-progress-report)
          report (format-progress-report progress)]
    (println report)))

```
