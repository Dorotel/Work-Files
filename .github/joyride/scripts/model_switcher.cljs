(ns model-switcher
  "Automated model switching for GitHub Copilot workflows
  
  Provides utilities to programmatically switch between different AI models
  during multi-phase workflows to optimize for model strengths and minimize
  premium request usage."
  (:require ["vscode" :as vscode]
            [promesa.core :as p]))

;; Available Copilot models with their optimal use cases
(def models
  {:gpt-5-codex {:family "gpt-5-codex"
                 :vendor "copilot"
                 :description "Code analysis, MVVM patterns, bindings tracing"
                 :use-case "Phase 1-2: Code discovery and analysis"}
   :claude-sonnet-4-5 {:family "claude-sonnet-4-5"
                       :vendor "copilot"
                       :description "Documentation generation, synthesis, narrative"
                       :use-case "Phase 3-4: Global UX and synthesis"}
   :claude-sonnet-4 {:family "claude-sonnet-4"
                     :vendor "copilot"
                     :description "Balanced coding workflows"
                     :use-case "Backup for Phase 3-4"}
   :gpt-5-mini {:family "gpt-5-mini"
                :vendor "copilot"
                :description "Fast, general-purpose with reasoning"
                :use-case "Quick tasks or fallback"}})

(defn get-model-info
  "Get information about a specific model by keyword"
  [model-key]
  (get models model-key))

(defn list-available-models
  "Display all available models and their use cases"
  []
  (doseq [[key {:keys [family description use-case]}] models]
    (vscode/window.showInformationMessage
      (str "Model: " (name key) "\n"
           "Family: " family "\n"
           "Best for: " description "\n"
           "Use case: " use-case))))

(defn switch-to-model
  "Switch Copilot to specified model
  
  Parameters:
  - model-key: Keyword identifying the model (:gpt-5-codex, :claude-sonnet-4-5, etc.)
  
  Returns: Promise resolving to the selected model or error"
  [model-key]
  (p/let [model-config (get models model-key)
          _ (when-not model-config
              (throw (js/Error. (str "Unknown model key: " model-key))))
          {:keys [family vendor]} model-config
          models-result (vscode/lm.selectChatModels 
                          #js {:vendor vendor :family family})
          selected-model (first models-result)]
    (if selected-model
      (do
        (vscode/window.showInformationMessage 
          (str "✓ Switched to " family " - " (:description model-config)))
        selected-model)
      (throw (js/Error. (str "Model " family " not available. Check Copilot access."))))))

(defn get-current-model
  "Get information about currently active Copilot model
  
  Note: This queries available models but doesn't guarantee which is active in chat"
  []
  (p/let [all-models (vscode/lm.selectChatModels #js {:vendor "copilot"})]
    (if (pos? (count all-models))
      (vscode/window.showInformationMessage 
        (str "Available Copilot models: " (count all-models)))
      (vscode/window.showWarningMessage 
        "No Copilot models available. Check your connection."))))

(defn model-checkpoint
  "Create a checkpoint before model switch with human confirmation
  
  Parameters:
  - current-model: Keyword of current model
  - next-model: Keyword of next model  
  - phase-summary: String describing completed work
  
  Returns: Promise resolving to user's decision (continue or stop)"
  [current-model next-model phase-summary]
  (p/let [current-info (get models current-model)
          next-info (get models next-model)
          message (str "📋 Model Switch Checkpoint\n\n"
                      "Completed with: " (:family current-info) "\n"
                      "Summary: " phase-summary "\n\n"
                      "Next model: " (:family next-info) "\n"
                      "Optimized for: " (:description next-info) "\n\n"
                      "Continue to next phase?")
          choice (vscode/window.showInformationMessage 
                   message
                   #js {:modal true}
                   "Continue" "Stop")]
    (= choice "Continue")))

(defn estimate-premium-requests
  "Estimate premium requests for a phase based on task count
  
  Different models have different multipliers:
  - GPT-5-Codex: Higher multiplier but better for code analysis
  - Claude Sonnet 4.5: Agent mode, good for orchestration
  
  Parameters:
  - model-key: Model to estimate for
  - estimated-tasks: Number of major tasks (views to analyze, specs to generate)
  
  Returns: Estimated premium requests"
  [model-key estimated-tasks]
  (let [model-multipliers {:gpt-5-codex 1.5
                           :claude-sonnet-4-5 1.2
                           :claude-sonnet-4 1.0
                           :gpt-5-mini 0.8}
        multiplier (get model-multipliers model-key 1.0)
        base-requests-per-task 3] ; Rough estimate
    (* estimated-tasks base-requests-per-task multiplier)))

(defn optimize-for-batch
  "Suggest model optimization strategy for batch work
  
  Parameters:
  - task-count: Number of similar tasks to perform
  - task-type: :code-analysis or :documentation
  
  Returns: Map with recommended model and batching strategy"
  [task-count task-type]
  (let [recommendation
        (cond
          (and (= task-type :code-analysis) (> task-count 20))
          {:model :gpt-5-codex
           :strategy "Batch analyze 5-10 views at once for efficiency"
           :reason "GPT-5-Codex excels at code patterns, batch to minimize context switching"}
          
          (and (= task-type :code-analysis) (<= task-count 20))
          {:model :gpt-5-codex
           :strategy "Analyze views individually for accuracy"
           :reason "Small enough set for detailed individual analysis"}
          
          (and (= task-type :documentation) (> task-count 10))
          {:model :claude-sonnet-4-5
           :strategy "Use agent mode for orchestration across all docs"
           :reason "Claude Sonnet 4.5 agent mode can handle complex multi-document synthesis"}
          
          :else
          {:model :gpt-5-mini
           :strategy "Process tasks individually"
           :reason "General-purpose model suitable for mixed or small workloads"})]
    (vscode/window.showInformationMessage
      (str "💡 Optimization Recommendation\n\n"
           "Model: " (name (:model recommendation)) "\n"
           "Strategy: " (:strategy recommendation) "\n"
           "Reason: " (:reason recommendation)))
    recommendation))

(comment
  ;; Usage examples (evaluate in Joyride REPL)
  
  ;; List all available models
  (list-available-models)
  
  ;; Switch to GPT-5-Codex for code analysis
  (switch-to-model :gpt-5-codex)
  
  ;; Switch to Claude Sonnet 4.5 for documentation
  (switch-to-model :claude-sonnet-4-5)
  
  ;; Check current model availability
  (get-current-model)
  
  ;; Create checkpoint before switching
  (model-checkpoint 
    :gpt-5-codex 
    :claude-sonnet-4-5
    "Completed ui-inventory.md with 32 views analyzed")
  
  ;; Estimate premium requests for 30 views
  (estimate-premium-requests :gpt-5-codex 30)
  
  ;; Get optimization recommendation
  (optimize-for-batch 32 :code-analysis))
