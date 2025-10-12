(ns reverse-spec-workflow
  "Automated workflow for reverse-engineering UI specifications with optimal model usage
  
  Orchestrates the complete reverse-spec process across multiple models:
  - Phase 1-2: GPT-5-Codex for code analysis and pattern recognition
  - Phase 3-4: Claude Sonnet 4.5 for documentation synthesis
  
  Includes human checkpoints between phases to review progress and approve model switches."
  (:require ["vscode" :as vscode]
            [promesa.core :as p]
            [joyride.core :as joyride]
            [model-switcher :as ms]))

;; Workflow configuration
(def workflow-config
  {:phases [{:id 1
             :name "Discovery & Inventory"
             :model :gpt-5-codex
             :tasks ["Search Views/ directory"
                     "Search Controls/ directory"
                     "Analyze MVVM patterns"
                     "Build ui-inventory.md"]
             :estimated-premium-requests 15
             :output "specs/000-ui-capture/ui/ui-inventory.md"}
            {:id 2
             :name "Per-View Analysis"
             :model :gpt-5-codex
             :tasks ["Generate specs for each view"
                     "Trace MVVM Toolkit bindings"
                     "Map navigation flows"
                     "Document composite shells"]
             :estimated-premium-requests 50
             :output "specs/000-ui-capture/ui/{ViewName}/spec.md (30+ files)"}
            {:id 3
             :name "Global Synthesis"
             :model :claude-sonnet-4-5
             :tasks ["Curate global-ux.md"
                     "Create resource appendices"
                     "Document theme patterns"
                     "Synthesize common conventions"]
             :estimated-premium-requests 10
             :output "specs/000-ui-capture/ui/global-ux.md"}
            {:id 4
             :name "Audit & Validation"
             :model :claude-sonnet-4-5
             :tasks ["Build ui-audit-checklist.md"
                     "Document gaps and ambiguities"
                     "Calculate completion metrics"
                     "Generate progress report"]
             :estimated-premium-requests 5
             :output "specs/000-ui-capture/ui/ui-audit-checklist.md"}]
   :total-estimated-premium-requests 80})

(defn show-workflow-overview
  "Display complete workflow overview with model assignments and estimates"
  []
  (p/let [overview (str "🎯 Reverse-Spec Workflow Overview\n\n"
                       "Total Phases: " (count (:phases workflow-config)) "\n"
                       "Est. Premium Requests: " (:total-estimated-premium-requests workflow-config) "\n\n"
                       (clojure.string/join "\n\n"
                         (map (fn [{:keys [id name model estimated-premium-requests]}]
                                (str "Phase " id ": " name "\n"
                                     "Model: " (name model) "\n"
                                     "Est. Requests: " estimated-premium-requests))
                              (:phases workflow-config))))]
    (vscode/window.showInformationMessage overview #js {:modal true})))

(defn run-phase
  "Execute a single workflow phase with appropriate model
  
  Parameters:
  - phase: Phase configuration map
  
  Returns: Promise resolving to phase completion status"
  [phase]
  (p/let [{:keys [id name model tasks output estimated-premium-requests]} phase
          
          ;; Switch to optimal model for this phase
          _ (ms/switch-to-model model)
          
          ;; Show phase start notification
          _ (vscode/window.showInformationMessage
              (str "🚀 Starting Phase " id ": " name "\n\n"
                   "Model: " (name model) "\n"
                   "Tasks: " (count tasks) "\n"
                   "Est. Premium Requests: " estimated-premium-requests "\n\n"
                   "Execute the following prompt:\n"
                   "#file:speckit.reverseuispec.prompt.md\n\n"
                   "Focus on Phase " id " tasks only.")
              #js {:modal true})
          
          ;; Display task checklist
          task-list (clojure.string/join "\n" 
                      (map-indexed 
                        (fn [idx task] (str (inc idx) ". " task))
                        tasks))
          _ (vscode/window.showInformationMessage
              (str "📋 Phase " id " Tasks:\n\n" task-list)
              #js {:modal true})
          
          ;; Wait for user to complete phase
          completion (vscode/window.showInformationMessage
                       (str "Complete Phase " id " tasks in Copilot Chat.\n\n"
                            "Click 'Done' when phase is complete and output validated:\n"
                            output)
                       #js {:modal true}
                       "Done" "Skip Phase" "Stop Workflow")]
    
    (case completion
      "Done" {:status :completed :phase-id id}
      "Skip Phase" {:status :skipped :phase-id id}
      {:status :stopped :phase-id id})))

(defn create-checkpoint
  "Create checkpoint between phases for review and approval"
  [completed-phase next-phase]
  (p/let [summary (str "✅ Completed Phase " (:id completed-phase) ": " (:name completed-phase) "\n"
                      "Output: " (:output completed-phase) "\n\n"
                      "📊 Premium Requests Used: ~" (:estimated-premium-requests completed-phase) "\n\n"
                      "Next: Phase " (:id next-phase) ": " (:name next-phase) "\n"
                      "Model Switch: " (name (:model completed-phase)) " → " (name (:model next-phase)))
          continue? (ms/model-checkpoint 
                      (:model completed-phase)
                      (:model next-phase)
                      summary)]
    continue?))

(defn execute-workflow
  "Execute the complete reverse-spec workflow with model optimization
  
  Main workflow orchestrator that:
  1. Shows workflow overview
  2. Executes each phase with optimal model
  3. Creates checkpoints between phases for review
  4. Tracks progress and premium request usage
  
  Returns: Promise resolving to workflow completion summary"
  []
  (p/do
    ;; Show overview
    (show-workflow-overview)
    
    ;; Wait for user to confirm start
    (p/let [start? (vscode/window.showInformationMessage
                     "Start reverse-spec workflow?"
                     #js {:modal true}
                     "Start" "Cancel")]
      
      (if (not= start? "Start")
        (vscode/window.showWarningMessage "Workflow cancelled")
        
        ;; Execute phases sequentially with checkpoints
        (p/loop [phases (:phases workflow-config)
                 completed []]
          (if (empty? phases)
            ;; All phases complete
            (vscode/window.showInformationMessage
              (str "🎉 Reverse-Spec Workflow Complete!\n\n"
                   "Phases completed: " (count completed) "\n"
                   "Total premium requests: ~" (:total-estimated-premium-requests workflow-config) "\n\n"
                   "Deliverables:\n"
                   "- specs/000-ui-capture/spec.md\n"
                   "- specs/000-ui-capture/ui/ui-inventory.md\n"
                   "- specs/000-ui-capture/ui/{ViewName}/spec.md (30+ files)\n"
                   "- specs/000-ui-capture/ui/global-ux.md\n"
                   "- specs/000-ui-capture/ui/ui-audit-checklist.md")
              #js {:modal true})
            
            ;; Execute next phase
            (p/let [current-phase (first phases)
                    remaining-phases (rest phases)
                    result (run-phase current-phase)]
              
              (cond
                ;; Phase stopped - abort workflow
                (= (:status result) :stopped)
                (vscode/window.showWarningMessage 
                  (str "Workflow stopped at Phase " (:phase-id result)))
                
                ;; Phase skipped - continue to checkpoint
                (= (:status result) :skipped)
                (if (empty? remaining-phases)
                  (vscode/window.showInformationMessage "Workflow complete (with skipped phases)")
                  (p/let [next-phase (first remaining-phases)
                          continue? (create-checkpoint current-phase next-phase)]
                    (if continue?
                      (p/recur remaining-phases (conj completed current-phase))
                      (vscode/window.showWarningMessage "Workflow cancelled at checkpoint"))))
                
                ;; Phase completed successfully
                :else
                (if (empty? remaining-phases)
                  (vscode/window.showInformationMessage "Workflow complete!")
                  (p/let [next-phase (first remaining-phases)
                          continue? (create-checkpoint current-phase next-phase)]
                    (if continue?
                      (p/recur remaining-phases (conj completed current-phase))
                      (vscode/window.showWarningMessage "Workflow cancelled at checkpoint"))))))))))))

(defn quick-start-phase
  "Quick start a specific phase without full workflow
  
  Useful for resuming workflow or testing individual phases
  
  Parameters:
  - phase-id: Phase number (1-4)"
  [phase-id]
  (let [phase (first (filter #(= (:id %) phase-id) (:phases workflow-config)))]
    (if phase
      (run-phase phase)
      (vscode/window.showErrorMessage (str "Invalid phase ID: " phase-id)))))

(defn estimate-time
  "Estimate time required for complete workflow
  
  Based on:
  - Phase 1: 30 min (discovery)
  - Phase 2: 2-3 hours (30+ view specs)
  - Phase 3: 45 min (global synthesis)
  - Phase 4: 30 min (audit checklist)
  
  Total: 4-5 hours of active work"
  []
  (vscode/window.showInformationMessage
    (str "⏱️ Workflow Time Estimate\n\n"
         "Phase 1 (Discovery): 30 min\n"
         "Phase 2 (Per-View Specs): 2-3 hours\n"
         "Phase 3 (Global Synthesis): 45 min\n"
         "Phase 4 (Audit): 30 min\n\n"
         "Total Active Work: 4-5 hours\n"
         "Recommended: Split across 2-3 work sessions\n\n"
         "Timeline Target: 2 weeks (per spec.md)")
    #js {:modal true}))

(comment
  ;; Usage examples (evaluate in Joyride REPL)
  
  ;; Show workflow overview
  (show-workflow-overview)
  
  ;; Estimate time required
  (estimate-time)
  
  ;; Execute complete workflow
  (execute-workflow)
  
  ;; Quick start Phase 1 only
  (quick-start-phase 1)
  
  ;; Quick start Phase 3 (resume after Phase 2 complete)
  (quick-start-phase 3))

;; Export main function for VS Code command
(defn activate []
  (execute-workflow))
