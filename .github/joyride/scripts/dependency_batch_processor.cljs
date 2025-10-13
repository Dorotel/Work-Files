(ns dependency-batch-processor
  "Efficient batch processing for large dependency component collections.
   Handles 50+ components with adaptive batch sizing, error recovery, and progress reporting."
  (:require ["vscode" :as vscode]
            [promesa.core :as p]
            [clojure.string :as str]
            [dependency-analyzer :as da]
            [dependency-spec-generator :as dsg]))

;; ============================================================================
;; Batch Configuration
;; ============================================================================

(def default-batch-config
  {:batch-size 15           ; Components per batch
   :pause-between-batches 100 ; ms
   :max-concurrent 5        ; Max parallel operations
   :retry-attempts 3
   :retry-delay 2000})      ; ms

(defn calculate-adaptive-batch-size
  "Calculate optimal batch size based on total component count."
  [total-count]
  (cond
    (< total-count 20) 5
    (< total-count 50) 10
    (< total-count 100) 15
    :else 20))

;; ============================================================================
;; Error Handling & Retry
;; ============================================================================

(defn retry-operation
  "Retry operation with exponential backoff."
  [operation max-attempts delay]
  (p/loop [attempt 1]
    (p/catch
      (operation)
      (fn [error]
        (if (>= attempt max-attempts)
          (do
            (vscode/window.showErrorMessage
              (str "Operation failed after " max-attempts " attempts: " (.-message error)))
            (throw error))
          (p/do
            (vscode/window.showWarningMessage
              (str "Attempt " attempt " failed, retrying..."))
            (p/delay (* delay attempt))
            (p/recur (inc attempt))))))))

;; ============================================================================
;; Batch Analysis
;; ============================================================================

(defn batch-analyze-components
  "Analyze components in batches with progress reporting."
  ([components component-type]
   (batch-analyze-components components component-type (:batch-size default-batch-config)))
  ([components component-type batch-size]
   (let [total (count components)
         batches (partition-all batch-size components)
         total-batches (count batches)]
     (vscode/window.showInformationMessage
       (str "Starting batch analysis: " total " components in " total-batches " batches"))
     (p/loop [remaining batches
              batch-num 1
              results []
              errors []]
       (if (empty? remaining)
         (do
           (vscode/window.showInformationMessage
             (str "Analysis complete! Analyzed: " (count results)
                  " | Errors: " (count errors)))
           {:results results
            :errors errors
            :success-rate (/ (count results) total)})
         (p/let [batch (first remaining)
                 batch-results (p/all
                                 (map #(p/catch
                                         (da/analyze-component % component-type)
                                         (fn [err]
                                           {:error true
                                            :file %
                                            :message (.-message err)}))
                                      batch))
                 successful (filter #(not (:error %)) batch-results)
                 failed (filter :error batch-results)]
           (vscode/window.showInformationMessage
             (str "Batch " batch-num "/" total-batches ": "
                  (count successful) " success, "
                  (count failed) " failed"))
           (p/delay (:pause-between-batches default-batch-config))
           (p/recur (rest remaining)
                    (inc batch-num)
                    (into results successful)
                    (into errors failed))))))))

;; ============================================================================
;; Batch Spec Generation
;; ============================================================================

(defn batch-generate-specs-with-errors
  "Generate specs in batches with detailed error tracking."
  ([analyses]
   (batch-generate-specs-with-errors analyses (:batch-size default-batch-config)))
  ([analyses batch-size]
   (let [total (count analyses)
         batches (partition-all batch-size analyses)
         total-batches (count batches)]
     (vscode/window.showInformationMessage
       (str "Starting batch spec generation: " total " components in " total-batches " batches"))
     (p/loop [remaining batches
              batch-num 1
              results []
              errors []]
       (if (empty? remaining)
         (do
           (vscode/window.showInformationMessage
             (str "Spec generation complete! Generated: " (count results)
                  " | Errors: " (count errors)))
           {:results results
            :errors errors
            :success-rate (/ (count results) total)})
         (p/let [batch (first remaining)
                 batch-results (p/all
                                 (map #(p/catch
                                         (dsg/generate-spec-file %
                                           (str (da/workspace-root)
                                                "/specs/000-dependency-capture/dependencies"))
                                         (fn [err]
                                           {:error true
                                            :component (:class-name %)
                                            :message (.-message err)}))
                                      batch))
                 successful (filter #(not (:error %)) batch-results)
                 failed (filter :error batch-results)]
           (vscode/window.showInformationMessage
             (str "Batch " batch-num "/" total-batches ": "
                  (count successful) " success, "
                  (count failed) " failed"))
           (p/delay (:pause-between-batches default-batch-config))
           (p/recur (rest remaining)
                    (inc batch-num)
                    (into results successful)
                    (into errors failed))))))))

;; ============================================================================
;; Parallel Processing with Concurrency Limits
;; ============================================================================

(defn parallel-analyze-batch
  "Analyze batch with concurrency limit."
  [components component-type max-concurrent]
  (let [semaphore (atom 0)]
    (p/all
      (map (fn [component]
             (p/loop []
               (if (< @semaphore max-concurrent)
                 (do
                   (swap! semaphore inc)
                   (p/-> (da/analyze-component component component-type)
                         (p/finally
                           (fn []
                             (swap! semaphore dec)))))
                 (p/do
                   (p/delay 50)
                   (p/recur)))))
           components))))

;; ============================================================================
;; Smart Batch Processor
;; ============================================================================

(defn smart-batch-processor
  "Intelligent batch processor with adaptive sizing and error recovery."
  [operation batch-size]
  (p/let [components (da/discover-all-components)
          total (:total components)
          adaptive-size (if batch-size
                          batch-size
                          (calculate-adaptive-batch-size total))]
    (vscode/window.showInformationMessage
      (str "Smart batch processor: " total " components | Batch size: " adaptive-size))
    
    (case operation
      :analyze
      (p/let [services (batch-analyze-components (:services components) :service adaptive-size)
              models (batch-analyze-components (:models components) :model adaptive-size)
              converters (batch-analyze-components (:converters components) :converter adaptive-size)
              behaviors (batch-analyze-components (:behaviors components) :behavior adaptive-size)
              extensions (batch-analyze-components (:extensions components) :extension adaptive-size)
              core-utils (batch-analyze-components (:core components) :core adaptive-size)]
        {:services services
         :models models
         :converters converters
         :behaviors behaviors
         :extensions extensions
         :core core-utils})
      
      :generate-specs
      (p/let [analysis (da/analyze-all-components)
              all-analyses (concat (:services analysis)
                                  (:models analysis)
                                  (:converters analysis)
                                  (:behaviors analysis)
                                  (:extensions analysis)
                                  (:core analysis))]
        (batch-generate-specs-with-errors all-analyses adaptive-size))
      
      (vscode/window.showErrorMessage
        (str "Unknown operation: " operation)))))

;; ============================================================================
;; Retry Failed Operations
;; ============================================================================

(defn retry-failed-specs
  "Retry generating specs that failed in previous batch run."
  [failed-components]
  (vscode/window.showInformationMessage
    (str "Retrying " (count failed-components) " failed specs..."))
  (p/loop [remaining failed-components
           results []
           still-failed []]
    (if (empty? remaining)
      (do
        (vscode/window.showInformationMessage
          (str "Retry complete! Success: " (count results)
               " | Still failed: " (count still-failed)))
        {:results results
         :errors still-failed})
      (p/let [component (first remaining)
              result (p/catch
                       (retry-operation
                         #(dsg/regenerate-spec (:component component))
                         (:retry-attempts default-batch-config)
                         (:retry-delay default-batch-config))
                       (fn [err]
                         {:error true
                          :component (:component component)
                          :message (.-message err)}))]
        (if (:error result)
          (p/recur (rest remaining) results (conj still-failed result))
          (p/recur (rest remaining) (conj results result) still-failed))))))

;; ============================================================================
;; Batch Progress Export
;; ============================================================================

(defn export-batch-results
  "Export batch results to JSON file for analysis."
  [results output-path]
  (let [report {:timestamp (.toISOString (js/Date.))
                :total-processed (+ (count (:results results))
                                   (count (:errors results)))
                :successful (count (:results results))
                :failed (count (:errors results))
                :success-rate (:success-rate results)
                :errors (:errors results)}]
    (p/create
      (fn [resolve reject]
        (require '["fs" :as fs])
        (fs/writeFile output-path
                      (js/JSON.stringify (clj->js report) nil 2)
                      (fn [err]
                        (if err
                          (reject err)
                          (do
                            (vscode/window.showInformationMessage
                              (str "Batch results exported to: " output-path))
                            (resolve output-path)))))))))

;; ============================================================================
;; Interactive Batch Menu
;; ============================================================================

(defn interactive-batch-menu
  "Interactive menu for batch operations."
  []
  (p/let [choice (vscode/window.showQuickPick
                   #js ["Smart analyze (adaptive batch sizing)"
                        "Smart generate specs (adaptive batch sizing)"
                        "Custom batch analyze (specify size)"
                        "Custom batch generate (specify size)"
                        "Retry failed operations"]
                   #js {:placeHolder "Select batch operation"})]
    (case choice
      "Smart analyze (adaptive batch sizing)"
      (smart-batch-processor :analyze nil)
      
      "Smart generate specs (adaptive batch sizing)"
      (smart-batch-processor :generate-specs nil)
      
      "Custom batch analyze (specify size)"
      (p/let [size (vscode/window.showInputBox
                     #js {:prompt "Enter batch size (5-20):"
                          :value "10"})]
        (when size
          (smart-batch-processor :analyze (js/parseInt size))))
      
      "Custom batch generate (specify size)"
      (p/let [size (vscode/window.showInputBox
                     #js {:prompt "Enter batch size (5-20):"
                          :value "15"})]
        (when size
          (smart-batch-processor :generate-specs (js/parseInt size))))
      
      "Retry failed operations"
      (vscode/window.showWarningMessage
        "TODO: Load previous batch results and retry failed items")
      
      nil)))

;; ============================================================================
;; Public API
;; ============================================================================

(comment
  "Usage examples:"
  
  ;; Smart adaptive batch processing
  (smart-batch-processor :analyze nil)
  (smart-batch-processor :generate-specs nil)
  
  ;; Custom batch sizes
  (smart-batch-processor :analyze 10)
  (smart-batch-processor :generate-specs 15)
  
  ;; Interactive menu
  (interactive-batch-menu)
  
  ;; Export results
  (p/let [results (smart-batch-processor :generate-specs 15)]
    (export-batch-results results "batch-results.json")))
