(ns dependency-progress-tracker
  "Progress tracking and completion reporting for dependency reverse-spec workflow.
   Tracks component analysis, spec generation, and TODO completion."
  (:require ["vscode" :as vscode]
            ["path" :as path]
            ["fs" :as fs]
            [promesa.core :as p]
            [clojure.string :as str]
            [dependency-analyzer :as da]))

;; Helper for formatting numbers (format not available in SCI)
(defn fmt-decimal
  "Format a number to 1 decimal place"
  [n]
  (let [rounded (* (js/Math.round (* n 10)) 0.1)]
    (str (.toFixed rounded 1))))

;; ============================================================================
;; Progress Data Collection
;; ============================================================================

(defn count-todos-in-file
  "Count TODO markers in a file."
  [file-path]
  (p/catch
    (p/let [content (da/read-file-content file-path)
            todos (re-seq #"TODO:" content)]
      (count todos))
    (fn [_] 0)))

(defn check-spec-exists
  "Check if spec file exists for a component."
  [component-name]
  (p/let [root (da/workspace-root)
          spec-path (path/join root "specs" "000-dependency-capture" "dependencies"
                              component-name "spec.md")]
    (p/create
      (fn [resolve reject]
        (fs/access spec-path fs/constants.F_OK
          (fn [err]
            (resolve (nil? err))))))))

(defn analyze-spec-completeness
  "Analyze completeness of a spec file."
  [spec-path]
  (p/let [exists (p/create
                   (fn [resolve reject]
                     (fs/access spec-path fs/constants.F_OK
                       (fn [err]
                         (resolve (nil? err))))))
          todo-count (if exists
                       (count-todos-in-file spec-path)
                       0)]
    {:exists exists
     :todo-count todo-count
     :completion-pct (if exists
                       (* 100 (/ (- 50 todo-count) 50.0)) ; Assume ~50 TODOs in template
                       0)}))

;; ============================================================================
;; Component Progress Analysis
;; ============================================================================

(defn analyze-component-progress
  "Analyze progress for a single component."
  [component-analysis]
  (p/let [component-name (:class-name component-analysis)
          root (da/workspace-root)
          spec-path (path/join root "specs" "000-dependency-capture" "dependencies"
                              component-name "spec.md")
          completeness (analyze-spec-completeness spec-path)]
    {:component-name component-name
     :component-type (:component-type component-analysis)
     :spec-exists (:exists completeness)
     :todo-count (:todo-count completeness)
     :completion-pct (:completion-pct completeness)}))

(defn analyze-all-progress
  "Analyze progress for all components."
  []
  (p/let [analysis (da/analyze-all-components)
          all-components (concat (:services analysis)
                                (:models analysis)
                                (:converters analysis)
                                (:behaviors analysis)
                                (:extensions analysis)
                                (:core analysis))
          progress-data (p/all (map analyze-component-progress all-components))]
    {:total-components (count all-components)
     :components-by-type {:services (count (:services analysis))
                          :models (count (:models analysis))
                          :converters (count (:converters analysis))
                          :behaviors (count (:behaviors analysis))
                          :extensions (count (:extensions analysis))
                          :core (count (:core analysis))}
     :components progress-data
     :specs-created (count (filter :spec-exists progress-data))
     :specs-missing (count (filter #(not (:spec-exists %)) progress-data))
     :avg-completion (if (empty? progress-data)
                       0
                       (/ (reduce + (map :completion-pct progress-data))
                          (count progress-data)))
     :total-todos (reduce + (map :todo-count progress-data))}))

;; ============================================================================
;; Progress Report Generation
;; ============================================================================

(defn format-progress-row
  "Format component progress as table row."
  [component-progress]
  (str "| " (:component-name component-progress)
       " | " (name (:component-type component-progress))
       " | " (if (:spec-exists component-progress) "✅" "❌")
       " | " (:todo-count component-progress)
       " | " (fmt-decimal (:completion-pct component-progress)) "%"
       " |"))

(defn generate-progress-report
  "Generate progress report markdown."
  [progress-data]
  (str "# Dependency Reverse-Spec Progress Report\n\n"
       "**Generated**: " (.toISOString (js/Date.)) "\n\n"
       "## Overall Progress\n\n"
       "- **Total Components**: " (:total-components progress-data) "\n"
       "- **Specs Created**: " (:specs-created progress-data) " / "
       (:total-components progress-data) "\n"
       "- **Specs Missing**: " (:specs-missing progress-data) "\n"
       "- **Average Completion**: " (fmt-decimal (:avg-completion progress-data)) "%\n"
       "- **Total TODOs Remaining**: " (:total-todos progress-data) "\n\n"
       "## Components by Type\n\n"
       "- Services: " (get-in progress-data [:components-by-type :services]) "\n"
       "- Models: " (get-in progress-data [:components-by-type :models]) "\n"
       "- Converters: " (get-in progress-data [:components-by-type :converters]) "\n"
       "- Behaviors: " (get-in progress-data [:components-by-type :behaviors]) "\n"
       "- Extensions: " (get-in progress-data [:components-by-type :extensions]) "\n"
       "- Core: " (get-in progress-data [:components-by-type :core]) "\n\n"
       "## Component Progress\n\n"
       "| Component | Type | Spec Exists | TODOs | Completion |\n"
       "|-----------|------|-------------|-------|------------|\n"
       (str/join "\n" (map format-progress-row (:components progress-data)))
       "\n\n"
       "## Next Actions\n\n"
       (if (> (:specs-missing progress-data) 0)
         (str "1. Generate missing specs (" (:specs-missing progress-data) " remaining)\n"
              "2. Complete TODO items in existing specs (" (:total-todos progress-data) " total)\n")
         (str "1. Complete TODO items in existing specs (" (:total-todos progress-data) " total)\n"
              "2. Review and validate all specs for accuracy\n"))
       "3. Generate global-architecture.md\n"
       "4. Generate dependency-audit-checklist.md\n\n"))

(defn save-progress-report
  "Analyze progress and save report."
  []
  (p/let [root (da/workspace-root)
          _ (when (not root)
              (throw (js/Error. "No workspace folder open")))
          progress (analyze-all-progress)
          report-content (generate-progress-report progress)
          output-dir (path/join root "specs" "000-dependency-capture" "dependencies")
          output-file (path/join output-dir "progress-report.md")]
    ;; Write report
    (p/create
      (fn [resolve reject]
        (fs/writeFile output-file report-content
          (fn [err]
            (if err
              (reject err)
              (do
                (vscode/window.showInformationMessage
                  (str "Progress report saved! Completion: "
                       (fmt-decimal (:avg-completion progress)) "%"))
                (resolve {:progress progress
                          :report-path output-file})))))))
    progress))

;; ============================================================================
;; Interactive Progress Functions
;; ============================================================================

(defn show-incomplete-components
  "Show quick-pick menu of incomplete components."
  []
  (p/let [progress (analyze-all-progress)
          incomplete (filter #(< (:completion-pct %) 100) (:components progress))
          items (map (fn [c]
                       {:label (:component-name c)
                        :description (str (name (:component-type c))
                                        " - " (:todo-count c) " TODOs - "
                                        (fmt-decimal (:completion-pct c)) "%")
                        :component c})
                    incomplete)
          choice (vscode/window.showQuickPick
                   (clj->js items)
                   #js {:placeHolder "Select incomplete component to work on"})]
    (when choice
      (vscode/window.showInformationMessage
        (str "Selected: " (.-label choice) " - " (.-description choice)))
      (js->clj choice :keywordize-keys true))))

(defn estimate-remaining-work
  "Estimate remaining work hours based on TODO count."
  []
  (p/let [progress (analyze-all-progress)
          total-todos (:total-todos progress)
          specs-missing (:specs-missing progress)
          ;; Estimate: 5 minutes per TODO, 15 minutes per missing spec
          todo-hours (/ (* total-todos 5) 60.0)
          spec-hours (/ (* specs-missing 15) 60.0)
          total-hours (+ todo-hours spec-hours)]
    (vscode/window.showInformationMessage
      (str "Estimated remaining work: " (fmt-decimal total-hours) " hours\n"
           "- TODOs: " (fmt-decimal todo-hours) " hours (" total-todos " items)\n"
           "- Missing specs: " (fmt-decimal spec-hours) " hours (" specs-missing " specs)"))
    {:todo-hours todo-hours
     :spec-hours spec-hours
     :total-hours total-hours}))

;; ============================================================================
;; Interactive Dashboard
;; ============================================================================

(defn show-progress-dashboard
  "Show interactive progress dashboard with action buttons."
  []
  (p/let [progress (analyze-all-progress)
          action (vscode/window.showQuickPick
                   #js ["View progress report"
                        "Show incomplete components"
                        "Estimate remaining work"
                        "Export progress to file"
                        "Refresh progress"]
                   #js {:placeHolder (str "Progress: "
                                         (fmt-decimal (:avg-completion progress)) "%"
                                         " - " (:total-todos progress) " TODOs remaining")})]
    (case action
      "View progress report"
      (save-progress-report)
      
      "Show incomplete components"
      (show-incomplete-components)
      
      "Estimate remaining work"
      (estimate-remaining-work)
      
      "Export progress to file"
      (save-progress-report)
      
      "Refresh progress"
      (show-progress-dashboard)
      
      nil)))

;; ============================================================================
;; Major Deliverables Tracking
;; ============================================================================

(defn check-deliverable-status
  "Check status of major deliverables."
  []
  (p/let [root (da/workspace-root)
          spec-md (path/join root "specs" "000-dependency-capture" "spec.md")
          inventory-md (path/join root "specs" "000-dependency-capture" "dependencies" "dependency-inventory.md")
          architecture-md (path/join root "specs" "000-dependency-capture" "dependencies" "global-architecture.md")
          audit-md (path/join root "specs" "000-dependency-capture" "dependencies" "dependency-audit-checklist.md")
          
          spec-exists (p/create (fn [resolve reject]
                                  (fs/access spec-md fs/constants.F_OK
                                    (fn [err] (resolve (nil? err))))))
          inventory-exists (p/create (fn [resolve reject]
                                       (fs/access inventory-md fs/constants.F_OK
                                         (fn [err] (resolve (nil? err))))))
          architecture-exists (p/create (fn [resolve reject]
                                          (fs/access architecture-md fs/constants.F_OK
                                            (fn [err] (resolve (nil? err))))))
          audit-exists (p/create (fn [resolve reject]
                                   (fs/access audit-md fs/constants.F_OK
                                     (fn [err] (resolve (nil? err))))))]
    {:spec-md spec-exists
     :inventory-md inventory-exists
     :architecture-md architecture-exists
     :audit-md audit-exists
     :all-complete (and spec-exists inventory-exists architecture-exists audit-exists)}))

;; ============================================================================
;; Public API
;; ============================================================================

(comment
  "Usage examples:"
  
  ;; Show interactive dashboard
  (show-progress-dashboard)
  
  ;; Save progress report
  (save-progress-report)
  
  ;; Show incomplete components
  (show-incomplete-components)
  
  ;; Estimate remaining work
  (estimate-remaining-work)
  
  ;; Check deliverables
  (check-deliverable-status))
