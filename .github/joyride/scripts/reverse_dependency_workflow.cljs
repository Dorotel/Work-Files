(ns reverse-dependency-workflow
  "Orchestrates the complete dependency reverse-spec workflow with model switching.
   4-Phase workflow: Discovery → Per-Component Analysis → Global Synthesis → Audit"
  (:require ["vscode" :as vscode]
            [promesa.core :as p]
            [clojure.string :as str]
            [model-switcher :as ms]
            [dependency-analyzer :as da]
            [dependency-spec-generator :as dsg]
            [dependency-batch-processor :as dbp]
            [dependency-progress-tracker :as dpt]))

;; Helper for formatting numbers (format not available in SCI)
(defn fmt-decimal
  "Format a number to 1 decimal place"
  [n]
  (let [rounded (* (js/Math.round (* n 10)) 0.1)]
    (str (.toFixed rounded 1))))

;; ============================================================================
;; Workflow Configuration
;; ============================================================================

(def workflow-phases
  {:phase-1 {:name "Discovery & Inventory"
             :model :gpt-5-codex
             :estimated-requests 15
             :estimated-hours 1
             :description "Discover all services, models, converters, behaviors, extensions, and core utilities. Build dependency-inventory.md."}
   
   :phase-2 {:name "Per-Component Analysis"
             :model :gpt-5-codex
             :estimated-requests 55
             :estimated-hours 3
             :description "Generate detailed spec.md for each component using Dependency Spec Template. NO code examples - focus on contracts and architecture."}
   
   :phase-3 {:name "Global Architecture Synthesis"
             :model :claude-sonnet-4-5
             :estimated-requests 10
             :estimated-hours 1
             :description "Synthesize global-architecture.md covering DI patterns, service layer architecture, configuration management, and common patterns."}
   
   :phase-4 {:name "Audit & Gap Analysis"
             :model :claude-sonnet-4-5
             :estimated-requests 5
             :estimated-hours 0.5
             :description "Generate dependency-audit-checklist.md with completion status, gaps, and architectural concerns."}})

;; ============================================================================
;; Phase Execution
;; ============================================================================

(defn execute-phase-1
  "Phase 1: Discovery & Inventory"
  []
  (vscode/window.showInformationMessage
    "🔍 Phase 1: Discovery & Inventory - Starting...")
  (p/let [analysis (da/analyze-all-components)
          report (da/save-analysis-report)]
    (vscode/window.showInformationMessage
      (str "✅ Phase 1 Complete! Discovered " (:total-analyzed analysis) " components."))
    {:phase :phase-1
     :analysis analysis
     :report-path (:report-path report)}))

(defn execute-phase-2
  "Phase 2: Per-Component Analysis with batch processing"
  []
  (vscode/window.showInformationMessage
    "📝 Phase 2: Per-Component Analysis - Starting batch generation...")
  (p/let [results (dbp/smart-batch-processor :generate-specs 15)
          progress (dpt/analyze-all-progress)]
    (vscode/window.showInformationMessage
      (str "✅ Phase 2 Complete! Generated " (:specs-created progress) " specs. "
           "Completion: " (fmt-decimal (:avg-completion progress)) "%"))
    {:phase :phase-2
     :results results
     :progress progress}))

(defn execute-phase-3
  "Phase 3: Global Architecture Synthesis"
  []
  (vscode/window.showInformationMessage
    "🏗️ Phase 3: Global Architecture Synthesis - Manual completion required")
  (vscode/window.showInformationMessage
    (str "📋 Action Required:\n"
         "1. Review all component specs for accuracy\n"
         "2. Create global-architecture.md covering:\n"
         "   - DI patterns and conventions\n"
         "   - Service layer architecture\n"
         "   - Configuration management\n"
         "   - Common patterns (error handling, logging, async)\n"
         "   - Data flow patterns\n"
         "3. Use Claude Sonnet 4.5 for sophisticated synthesis"))
  {:phase :phase-3
   :status :manual-completion-required})

(defn execute-phase-4
  "Phase 4: Audit & Gap Analysis"
  []
  (vscode/window.showInformationMessage
    "✅ Phase 4: Audit & Gap Analysis - Manual completion required")
  (vscode/window.showInformationMessage
    (str "📋 Action Required:\n"
         "1. Review dependency-inventory.md for completeness\n"
         "2. Create dependency-audit-checklist.md with:\n"
         "   - Static sections (services, models, DI registration, etc.)\n"
         "   - Dynamic sections populated from findings\n"
         "   - Gaps and ambiguities\n"
         "   - Progress metrics\n"
         "3. Use Claude Sonnet 4.5 for comprehensive audit"))
  {:phase :phase-4
   :status :manual-completion-required})

;; ============================================================================
;; Workflow Orchestration
;; ============================================================================

(defn run-phase
  "Execute a single workflow phase with model switching."
  [phase-key]
  (let [phase (get workflow-phases phase-key)]
    (vscode/window.showInformationMessage
      (str "🚀 Starting " (:name phase) "\n"
           "Estimated: " (:estimated-hours phase) " hours, "
           (:estimated-requests phase) " requests"))
    (p/let [_ (ms/switch-to-model (:model phase))]
      (case phase-key
        :phase-1 (execute-phase-1)
        :phase-2 (execute-phase-2)
        :phase-3 (execute-phase-3)
        :phase-4 (execute-phase-4)))))

(defn create-checkpoint
  "Create workflow checkpoint for resume capability."
  [phase-key results]
  (vscode/window.showInformationMessage
    (str "💾 Checkpoint: " (name phase-key) " complete"))
  {:phase phase-key
   :timestamp (.toISOString (js/Date.))
   :results results})

(defn workflow-checkpoint-review
  "Human checkpoint between phases."
  [phase-key results]
  (p/let [choice (vscode/window.showQuickPick
                   #js ["Continue to next phase"
                        "Review results"
                        "Stop workflow"]
                   #js {:placeHolder (str "Phase " (name phase-key)
                                         " complete. Continue?")})]
    (case choice
      "Continue to next phase" :continue
      "Review results" (do
                        (dpt/show-progress-dashboard)
                        :review)
      "Stop workflow" :stop
      :stop)))

(defn execute-workflow
  "Execute complete dependency reverse-spec workflow."
  []
  (vscode/window.showInformationMessage
    "🎬 Starting Dependency Reverse-Spec Workflow\n\n"
    "Total Estimated: 5.5 hours, 85 premium requests\n\n"
    "Phases:\n"
    "1. Discovery & Inventory (1h, 15 req) - GPT-5-Codex\n"
    "2. Per-Component Analysis (3h, 55 req) - GPT-5-Codex\n"
    "3. Global Synthesis (1h, 10 req) - Claude Sonnet 4.5\n"
    "4. Audit & Gaps (0.5h, 5 req) - Claude Sonnet 4.5")
  
  (p/let [;; Phase 1: Discovery
          _ (vscode/window.showInformationMessage "=== PHASE 1: DISCOVERY ===")
          phase1-result (run-phase :phase-1)
          checkpoint1 (create-checkpoint :phase-1 phase1-result)
          continue1? (workflow-checkpoint-review :phase-1 phase1-result)]
    
    (if (= continue1? :stop)
      (vscode/window.showWarningMessage "Workflow stopped after Phase 1")
      
      (p/let [;; Phase 2: Per-Component Analysis
              _ (vscode/window.showInformationMessage "=== PHASE 2: COMPONENT ANALYSIS ===")
              phase2-result (run-phase :phase-2)
              checkpoint2 (create-checkpoint :phase-2 phase2-result)
              continue2? (workflow-checkpoint-review :phase-2 phase2-result)]
        
        (if (= continue2? :stop)
          (vscode/window.showWarningMessage "Workflow stopped after Phase 2")
          
          (p/let [;; Checkpoint: Switch to Claude Sonnet 4.5
                  _ (vscode/window.showInformationMessage
                      "🔄 Model Switch Checkpoint\n\n"
                      "Phase 2 complete. Switching to Claude Sonnet 4.5 for synthesis phases.")
                  switch-approval (vscode/window.showQuickPick
                                    #js ["Approve model switch"
                                         "Cancel workflow"]
                                    #js {:placeHolder "Switch to Claude Sonnet 4.5?"})
                  
                  _ (when (= switch-approval "Cancel workflow")
                      (throw (js/Error. "Workflow cancelled by user")))]
            
            (p/let [;; Phase 3: Global Synthesis
                    _ (vscode/window.showInformationMessage "=== PHASE 3: GLOBAL SYNTHESIS ===")
                    phase3-result (run-phase :phase-3)
                    checkpoint3 (create-checkpoint :phase-3 phase3-result)
                    
                    ;; Phase 4: Audit
                    _ (vscode/window.showInformationMessage "=== PHASE 4: AUDIT & GAPS ===")
                    phase4-result (run-phase :phase-4)
                    checkpoint4 (create-checkpoint :phase-4 phase4-result)]
              
              (vscode/window.showInformationMessage
                "🎉 Workflow Complete!\n\n"
                "Generated:\n"
                "✅ dependency-inventory.md\n"
                "✅ Per-component spec.md files\n"
                "📋 TODO: global-architecture.md\n"
                "📋 TODO: dependency-audit-checklist.md")
              
              {:phase-1 checkpoint1
               :phase-2 checkpoint2
               :phase-3 checkpoint3
               :phase-4 checkpoint4
               :status :complete})))))))

;; ============================================================================
;; Quick Start Functions
;; ============================================================================

(defn quick-start-phase
  "Quick start a specific phase without full workflow."
  [phase-key]
  (p/let [phase (get workflow-phases phase-key)
          _ (vscode/window.showInformationMessage
              (str "Quick start: " (:name phase)))
          _ (ms/switch-to-model (:model phase))
          result (case phase-key
                   :phase-1 (execute-phase-1)
                   :phase-2 (execute-phase-2)
                   :phase-3 (execute-phase-3)
                   :phase-4 (execute-phase-4))]
    (vscode/window.showInformationMessage
      (str "✅ " (:name phase) " complete!"))
    result))

(defn resume-workflow-from
  "Resume workflow from a specific phase."
  [phase-key]
  (vscode/window.showInformationMessage
    (str "Resuming workflow from " (name phase-key)))
  (case phase-key
    :phase-1 (execute-workflow)
    :phase-2 (p/let [result (run-phase :phase-2)]
               (quick-start-phase :phase-3))
    :phase-3 (p/let [result (run-phase :phase-3)]
               (quick-start-phase :phase-4))
    :phase-4 (run-phase :phase-4)))

;; ============================================================================
;; Interactive Workflow Menu
;; ============================================================================

(defn interactive-workflow-menu
  "Interactive menu for workflow execution."
  []
  (p/let [choice (vscode/window.showQuickPick
                   #js ["Execute complete workflow"
                        "Quick start Phase 1 (Discovery)"
                        "Quick start Phase 2 (Component Analysis)"
                        "Quick start Phase 3 (Global Synthesis)"
                        "Quick start Phase 4 (Audit)"
                        "View workflow progress"]
                   #js {:placeHolder "Select dependency reverse-spec workflow action"})]
    (case choice
      "Execute complete workflow"
      (execute-workflow)
      
      "Quick start Phase 1 (Discovery)"
      (quick-start-phase :phase-1)
      
      "Quick start Phase 2 (Component Analysis)"
      (quick-start-phase :phase-2)
      
      "Quick start Phase 3 (Global Synthesis)"
      (quick-start-phase :phase-3)
      
      "Quick start Phase 4 (Audit)"
      (quick-start-phase :phase-4)
      
      "View workflow progress"
      (dpt/show-progress-dashboard)
      
      nil)))

;; ============================================================================
;; Public API
;; ============================================================================

(comment
  "Usage examples:"
  
  ;; Complete workflow
  (execute-workflow)
  
  ;; Quick start specific phase
  (quick-start-phase :phase-1)
  (quick-start-phase :phase-2)
  
  ;; Resume from checkpoint
  (resume-workflow-from :phase-2)
  
  ;; Interactive menu
  (interactive-workflow-menu))
