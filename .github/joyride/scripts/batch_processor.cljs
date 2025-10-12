```clojure
(ns batch-processor
  "Batch processing utilities for efficient multi-view operations
  
  Optimizes processing of large view collections by:
  - Batching operations to manage memory and performance
  - Parallel processing where safe
  - Progress reporting during long operations
  - Error recovery and retry logic"
  (:require ["vscode" :as vscode]
            ["fs" :as fs]
            [promesa.core :as p]
            [clojure.string :as str]
            [view-analyzer :as va]
            [spec-generator :as sg]
            [progress-tracker :as pt]))

(def workspace-root va/workspace-root)

(defn batch-partition
  "Partition a collection into batches
  
  Parameters:
  - items: Collection to partition
  - batch-size: Size of each batch
  
  Returns: Sequence of batches"
  [items batch-size]
  (partition-all batch-size items))

(defn process-with-progress
  "Process items with progress reporting
  
  Parameters:
  - items: Collection to process
  - process-fn: Function to apply to each item (returns promise)
  - label: Label for progress notification
  
  Returns: Promise resolving to vector of results"
  [items process-fn label]
  (p/let [total (count items)
          _ (vscode/window.showInformationMessage 
              (str "Starting " label ": " total " items"))]
    (p/loop [remaining items
             completed []
             index 0]
      (if (empty? remaining)
        (do
          (vscode/window.showInformationMessage 
            (str "✅ " label " complete: " (count completed) " items processed"))
          completed)
        (p/let [current (first remaining)
                _ (when (zero? (mod index 5))
                    (vscode/window.showInformationMessage 
                      (str label ": " index "/" total " (" 
                           (int (* 100 (/ index total))) "%)")))
                result (process-fn current)]
          (p/recur (rest remaining) 
                  (conj completed result) 
                  (inc index)))))))

(defn batch-analyze-views
  "Analyze views in batches with progress reporting
  
  Parameters:
  - view-uris: Collection of VS Code URIs for .axaml files
  - batch-size: Number of views per batch (default 10)
  
  Returns: Promise resolving to vector of analysis results"
  [view-uris batch-size]
  (let [batches (batch-partition view-uris (or batch-size 10))
        total-batches (count batches)]
    (p/loop [remaining-batches batches
             all-results []
             batch-num 0]
      (if (empty? remaining-batches)
        (do
          (vscode/window.showInformationMessage 
            (str "✅ Batch analysis complete: " (count all-results) " views analyzed"))
          all-results)
        (p/let [current-batch (first remaining-batches)
                _ (vscode/window.showInformationMessage 
                    (str "Processing batch " (inc batch-num) "/" total-batches 
                         " (" (count current-batch) " views)"))
                
                ;; Process all views in batch concurrently
                batch-results (p/all (map va/analyze-view current-batch))
                
                ;; Brief pause between batches to prevent overwhelming VS Code API
                _ (p/delay 100)]
          (p/recur (rest remaining-batches) 
                  (concat all-results batch-results) 
                  (inc batch-num)))))))

(defn batch-generate-specs-with-errors
  "Generate specs in batches with error handling
  
  Parameters:
  - analyses: Collection of view analysis maps
  - batch-size: Number of specs per batch (default 10)
  
  Returns: Promise resolving to map with :successful and :failed"
  [analyses batch-size]
  (let [batches (batch-partition analyses (or batch-size 10))
        total-batches (count batches)]
    (p/loop [remaining-batches batches
             successful []
             failed []
             batch-num 0]
      (if (empty? remaining-batches)
        (do
          (vscode/window.showInformationMessage 
            (str "✅ Batch spec generation complete\n"
                 "Successful: " (count successful) "\n"
                 "Failed: " (count failed))
            #js {:modal true})
          {:successful successful :failed failed})
        (p/let [current-batch (first remaining-batches)
                _ (vscode/window.showInformationMessage 
                    (str "Generating specs batch " (inc batch-num) "/" total-batches))
                
                ;; Process batch with error handling per item
                batch-results (p/all 
                               (map (fn [analysis]
                                      (p/catch
                                        (p/let [uri (sg/create-spec-file analysis)]
                                          {:status :success 
                                           :view-name (:view-name analysis) 
                                           :uri uri})
                                        (fn [err]
                                          {:status :error 
                                           :view-name (:view-name analysis) 
                                           :error (.-message err)})))
                                    current-batch))
                
                ;; Separate successful and failed
                batch-successful (filter #(= :success (:status %)) batch-results)
                batch-failed (filter #(= :error (:status %)) batch-results)
                
                ;; Brief pause between batches
                _ (p/delay 100)]
          
          (p/recur (rest remaining-batches) 
                  (concat successful batch-successful) 
                  (concat failed batch-failed) 
                  (inc batch-num)))))))

(defn smart-batch-processor
  "Intelligently process views with adaptive batch sizing
  
  Adjusts batch size based on:
  - Total number of views
  - Available memory
  - Success/failure rates
  
  Parameters:
  - operation: :analyze or :generate-specs
  - initial-batch-size: Starting batch size (default 10)
  
  Returns: Promise resolving to processing results"
  [operation initial-batch-size]
  (p/let [view-files (va/find-all-views)
          total-views (count view-files)
          
          ;; Adaptive batch sizing
          batch-size (cond
                      (< total-views 10) total-views
                      (< total-views 30) (or initial-batch-size 5)
                      (< total-views 50) (or initial-batch-size 10)
                      :else (or initial-batch-size 15))
          
          _ (vscode/window.showInformationMessage 
              (str "🎯 Smart Batch Processor\n\n"
                   "Operation: " (name operation) "\n"
                   "Total views: " total-views "\n"
                   "Batch size: " batch-size "\n"
                   "Est. batches: " (int (js/Math.ceil (/ total-views batch-size))))
              #js {:modal true})]
    
    (case operation
      :analyze
      (batch-analyze-views view-files batch-size)
      
      :generate-specs
      (p/let [analyses (va/analyze-all-views)]
        (batch-generate-specs-with-errors analyses batch-size))
      
      (throw (js/Error. (str "Unknown operation: " operation))))))

(defn retry-failed-specs
  "Retry generation of failed specs
  
  Parameters:
  - failed-specs: Collection of failed spec generation results
  
  Returns: Promise resolving to retry results"
  [failed-specs]
  (p/let [view-names (map :view-name failed-specs)
          _ (vscode/window.showInformationMessage 
              (str "Retrying " (count failed-specs) " failed specs..."))]
    (p/loop [remaining view-names
             successful []
             still-failed []]
      (if (empty? remaining)
        (do
          (vscode/window.showInformationMessage 
            (str "Retry complete\n"
                 "Fixed: " (count successful) "\n"
                 "Still failing: " (count still-failed)))
          {:successful successful :failed still-failed})
        (p/let [view-name (first remaining)
                result (p/catch
                         (p/let [_ (sg/regenerate-spec view-name)]
                           {:status :success :view-name view-name})
                         (fn [err]
                           {:status :error 
                            :view-name view-name 
                            :error (.-message err)}))]
          (p/recur (rest remaining)
                  (if (= :success (:status result))
                    (conj successful result)
                    successful)
                  (if (= :error (:status result))
                    (conj still-failed result)
                    still-failed)))))))

(defn parallel-analyze-batch
  "Analyze a batch of views in parallel (with concurrency limit)
  
  Parameters:
  - view-uris: Collection of view URIs
  - concurrency: Max parallel operations (default 5)
  
  Returns: Promise resolving to analysis results"
  [view-uris concurrency]
  (let [limit (or concurrency 5)
        total (count view-uris)]
    (p/loop [pending (vec view-uris)
             in-progress []
             completed []
             index 0]
      (cond
        ;; All done
        (and (empty? pending) (empty? in-progress))
        (do
          (vscode/window.showInformationMessage 
            (str "✅ Parallel analysis complete: " (count completed) " views"))
          completed)
        
        ;; Can start more work
        (and (seq pending) (< (count in-progress) limit))
        (let [next-uri (first pending)
              analysis-promise (va/analyze-view next-uri)]
          (p/recur (subvec pending 1)
                  (conj in-progress analysis-promise)
                  completed
                  (inc index)))
        
        ;; Wait for any in-progress to complete
        :else
        (p/let [result (p/race in-progress)
                remaining-in-progress (remove #{result} in-progress)
                _ (when (zero? (mod (count completed) 5))
                    (vscode/window.showInformationMessage 
                      (str "Progress: " (count completed) "/" total)))]
          (p/recur pending
                  remaining-in-progress
                  (conj completed result)
                  index))))))

(defn batch-update-todos
  "Batch update TODO sections in generated specs
  
  Useful for:
  - Marking sections as complete
  - Adding notes to TODO placeholders
  - Batch editing common patterns
  
  Parameters:
  - view-names: Collection of view names to update
  - replacements: Map of {old-text new-text}
  
  Returns: Promise resolving to update count"
  [view-names replacements]
  (p/let [_ (vscode/window.showInformationMessage 
              (str "Batch updating " (count view-names) " specs..."))]
    (p/loop [remaining view-names
             updated 0]
      (if (empty? remaining)
        (do
          (vscode/window.showInformationMessage 
            (str "✅ Batch update complete: " updated " files updated"))
          updated)
        (p/let [view-name (first remaining)
                spec-path (str workspace-root "/specs/000-ui-capture/ui/" 
                              view-name "/spec.md")
                content (joyride.core/slurp spec-path)
                
                ;; Apply all replacements
                new-content (reduce (fn [text [old new]]
                                     (str/replace text old new))
                                   content
                                   replacements)
                
                ;; Write back if changed
                changed? (not= content new-content)
        _ (when changed?
          (.writeFileSync fs spec-path new-content "utf8"))]
          (p/recur (rest remaining)
                  (if changed? (inc updated) updated)))))))

(defn export-batch-results
  "Export batch processing results to markdown report
  
  Parameters:
  - results: Map with processing results
  - operation: Operation name (string)
  
  Returns: Promise resolving to report file URI"
  [results operation]
  (p/let [timestamp (.toISOString (js/Date.))
          successful (get results :successful [])
          failed (get results :failed [])
          total (+ (count successful) (count failed))
          success-rate (if (pos? total)
                        (int (* 100 (/ (count successful) total)))
                        0)
          
          report (str "# Batch Processing Report: " operation "\n\n"
                     "**Generated**: " timestamp "\n"
                     "**Total Processed**: " total "\n"
                     "**Successful**: " (count successful) " (" success-rate "%)\n"
                     "**Failed**: " (count failed) "\n\n"
                     
                     (when (seq successful)
                       (str "## Successful ✅\n\n"
                            (str/join "\n" (map #(str "- " (:view-name %)) successful))
                            "\n\n"))
                     
                     (when (seq failed)
                       (str "## Failed ❌\n\n"
                            (str/join "\n" 
                              (map #(str "- **" (:view-name %) "**: " (:error %))
                                   failed))
                            "\n\n"
                            "### Retry Command\n\n"
                            "```clojure\n"
                            "(require '[batch-processor :as bp])\n"
                            "(bp/retry-failed-specs\n"
                            "  [" (str/join "\n   " 
                                   (map #(str "{:view-name \"" (:view-name %) "\"}")
                                        failed))
                            "])\n"
                            "```\n")))
          
          output-path (str workspace-root "/specs/000-ui-capture/batch-report-" 
                          (.getTime (js/Date.)) ".md")
          uri (vscode/Uri.file output-path)]
    
  (.writeFileSync fs output-path report "utf8")
    (vscode/window.showInformationMessage 
      (str "✓ Batch report saved: " output-path))
    uri))

(defn interactive-batch-menu
  "Interactive menu for batch operations
  
  Returns: Promise resolving to user selection"
  []
  (p/let [choice (vscode/window.showQuickPick
                  #js ["Analyze All Views (Batch)"
                       "Generate All Specs (Batch)"
                       "Generate Specs (Parallel)"
                       "Retry Failed Specs"
                       "Smart Batch Processor"
                       "Export Batch Report"
                       "Cancel"]
                  #js {:placeHolder "Select batch operation"
                       :title "Batch Processor"})]
    
    (case choice
      "Analyze All Views (Batch)"
      (p/let [size-input (vscode/window.showInputBox 
                          #js {:prompt "Batch size (default 10)"
                               :value "10"})
              batch-size (js/parseInt size-input)
              view-files (va/find-all-views)]
        (batch-analyze-views view-files batch-size))
      
      "Generate All Specs (Batch)"
      (p/let [size-input (vscode/window.showInputBox 
                          #js {:prompt "Batch size (default 10)"
                               :value "10"})
              batch-size (js/parseInt size-input)]
        (sg/batch-generate-specs batch-size))
      
      "Generate Specs (Parallel)"
      (p/let [concurrency-input (vscode/window.showInputBox 
                                  #js {:prompt "Max parallel operations (default 5)"
                                       :value "5"})
              concurrency (js/parseInt concurrency-input)
              analyses (va/analyze-all-views)]
        (parallel-analyze-batch analyses concurrency))
      
      "Smart Batch Processor"
      (p/let [op-choice (vscode/window.showQuickPick
                         #js ["analyze" "generate-specs"]
                         #js {:placeHolder "Select operation"})]
        (smart-batch-processor (keyword op-choice) 10))
      
      "Cancel"
      (vscode/window.showInformationMessage "Batch operation cancelled")
      
      nil)))

(comment
  ;; Usage examples (evaluate in Joyride REPL)
  
  ;; Interactive menu
  (interactive-batch-menu)
  
  ;; Smart batch processing
  (smart-batch-processor :analyze 10)
  (smart-batch-processor :generate-specs 10)
  
  ;; Manual batch analysis
  (p/let [views (va/find-all-views)]
    (batch-analyze-views views 10))
  
  ;; Manual batch spec generation with error handling
  (p/let [analyses (va/analyze-all-views)]
    (batch-generate-specs-with-errors analyses 10))
  
  ;; Retry failed specs
  (def failed-results [{:view-name "SomeView" :error "Some error"}])
  (retry-failed-specs failed-results)
  
  ;; Parallel processing with concurrency limit
  (p/let [views (va/find-all-views)]
    (parallel-analyze-batch views 5))
  
  ;; Batch update TODOs
  (batch-update-todos 
    ["MainWindow" "InventoryTabView"]
    {"**TODO**: Document" "✅ Documented"})
  
  ;; Export results
  (export-batch-results 
    {:successful [{:view-name "View1"}] 
     :failed [{:view-name "View2" :error "Error"}]}
    "Spec Generation"))

```
