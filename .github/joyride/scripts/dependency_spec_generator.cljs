(ns dependency-spec-generator
  "Automated spec.md generation for dependency components.
   Generates specifications following the Dependency Spec Template.
   NO code examples - focuses on contracts, responsibilities, and architecture."
  (:require ["vscode" :as vscode]
            ["path" :as path]
            ["fs" :as fs]
            [promesa.core :as p]
            [clojure.string :as str]
            [dependency-analyzer :as da]))

;; ============================================================================
;; Template Generation
;; ============================================================================

(def dependency-spec-template
  "---
Title: {ComponentName} – Dependency Specification
Source:
- File: {FilePath}
- Interface: {InterfaceName}
- Registration: {DIRegistration}
- Type: {ComponentType}

1. Purpose
   - TODO: What problem this component solves and for whom
   - TODO: Business domain context

2. Interface Contract (for services/components with interfaces)
   - Interface name: {InterfaceName}
   - Public methods:
{PublicMethods}
   - Public properties:
     - TODO: Document public properties
   - Events (if any):
     - TODO: Document events

3. Dependencies
   - Constructor parameters:
{Dependencies}
   - Configuration dependencies:
     - TODO: Document configuration dependencies
   - External dependencies:
     - TODO: Document external dependencies (database, file system, APIs)

4. DI Registration
   - Lifetime: {DILifetime}
   - Registration location: ServiceCollectionExtensions.ConfigureServices()
   - Rationale for lifetime choice: TODO: Explain why this lifetime

5. Responsibilities & Business Logic
   - Primary responsibilities: TODO: What this component does
   - Business rules enforced: TODO: Validation, authorization, workflow rules
   - Orchestration patterns: TODO: How it coordinates with other services
   - Error handling approach: TODO: How errors are handled and propagated
   - Async patterns: TODO: Which operations are async and why

6. Data Flow & Transformations
   - Input: TODO: What data comes in, from where
   - Processing: TODO: High-level description of transformations
   - Output: TODO: What data goes out, to where
   - Models/DTOs used: TODO: List of models this component works with

7. Consumers & Usage
   - Direct consumers:
     - TODO: Identify ViewModels, services that use this component
   - Usage patterns: TODO: Common usage scenarios
   - Initialization requirements: TODO: Any setup needed before use

8. Configuration
   - Configuration sections used: TODO: appsettings.json paths
   - Configuration binding: TODO: How configuration is loaded
   - Environment-specific settings: TODO: Development/Production differences

9. Error Handling & Logging
   - Error handling strategy: TODO: Exceptions, Result<T> pattern, etc.
   - Logging approach: TODO: What gets logged at what levels
   - Error propagation: TODO: How errors flow to consumers

10. Performance Considerations
   - Async operations: TODO: Which methods are async
   - Caching (if applicable): TODO: What is cached and why
   - Resource management: TODO: IDisposable, connection pooling, etc.
   - Performance patterns: TODO: Batch processing, lazy loading, etc.

11. Acceptance Criteria
   - [ ] Interface contract documented with all public methods
   - [ ] All dependencies identified and documented
   - [ ] DI registration location and lifetime documented
   - [ ] All consumers identified
   - [ ] Business logic responsibilities clearly described
   - [ ] Data flow and transformations mapped
   - [ ] Error handling patterns documented
   - [ ] Configuration dependencies listed

Open Questions
- TODO: Any uncertainties needing clarification
---")

;; ============================================================================
;; Template Population
;; ============================================================================

(defn format-public-methods
  "Format public methods for template."
  [methods]
  (if (empty? methods)
    "     - TODO: Document public methods"
    (str/join "\n"
      (map (fn [m]
             (str "     - " (:method-name m)
                  "(" (:parameters m) ") : "
                  (:return-type m)
                  " - TODO: Describe responsibility"))
           methods))))

(defn format-dependencies
  "Format constructor dependencies for template."
  [dependencies]
  (if (empty? dependencies)
    "     - No constructor dependencies"
    (str/join "\n"
      (map (fn [d]
             (str "     - " (:type d) " " (:name d)
                  " - TODO: Explain why this dependency is needed"))
           dependencies))))

(defn populate-template
  "Populate template with analysis data."
  [component-analysis]
  (-> dependency-spec-template
      (str/replace "{ComponentName}" (or (:class-name component-analysis) "Unknown"))
      (str/replace "{FilePath}" (or (:file-path component-analysis) "Unknown"))
      (str/replace "{InterfaceName}" (or (:interface-name component-analysis) "N/A"))
      (str/replace "{DIRegistration}" "TODO: Find registration location")
      (str/replace "{ComponentType}" (name (:component-type component-analysis)))
      (str/replace "{PublicMethods}" (format-public-methods (:public-methods component-analysis)))
      (str/replace "{Dependencies}" (format-dependencies (:dependencies component-analysis)))
      (str/replace "{DILifetime}" (if (:di-lifetime component-analysis)
                                     (name (:di-lifetime component-analysis))
                                     "TODO: Determine lifetime"))))

;; ============================================================================
;; Spec File Generation
;; ============================================================================

(defn generate-spec-file
  "Generate spec.md file for a component."
  [component-analysis output-dir]
  (p/let [component-name (:class-name component-analysis)
          spec-content (populate-template component-analysis)
          component-dir (path/join output-dir component-name)
          spec-file (path/join component-dir "spec.md")]
    ;; Create component directory
    (p/create
      (fn [resolve reject]
        (fs/mkdir component-dir #js {:recursive true}
          (fn [err]
            (if err
              (reject err)
              (resolve nil))))))
    ;; Write spec file
    (p/create
      (fn [resolve reject]
        (fs/writeFile spec-file spec-content
          (fn [err]
            (if err
              (reject err)
              (resolve spec-file))))))))

(defn generate-specs-for-components
  "Generate spec files for multiple components."
  [component-analyses]
  (p/let [root (da/workspace-root)
          _ (when (not root)
              (throw (js/Error. "No workspace folder open")))
          output-dir (path/join root "specs" "000-dependency-capture" "dependencies")
          spec-files (p/all (map #(generate-spec-file % output-dir) component-analyses))]
    (vscode/window.showInformationMessage
      (str "Generated " (count spec-files) " spec files!"))
    spec-files))

(defn batch-generate-specs
  "Generate specs in batches with progress reporting."
  [component-analyses batch-size]
  (let [batches (partition-all batch-size component-analyses)
        total-batches (count batches)]
    (p/loop [remaining batches
             batch-num 1
             results []]
      (if (empty? remaining)
        (do
          (vscode/window.showInformationMessage
            (str "Completed! Generated " (count results) " spec files."))
          results)
        (p/let [batch (first remaining)
                batch-results (generate-specs-for-components batch)]
          (vscode/window.showInformationMessage
            (str "Batch " batch-num "/" total-batches " complete ("
                 (* batch-num batch-size) "/" (count component-analyses) " components)"))
          (p/recur (rest remaining)
                   (inc batch-num)
                   (into results batch-results)))))))

(defn generate-all-specs
  "Analyze all components and generate all spec files."
  []
  (p/let [analysis (da/analyze-all-components)
          all-components (concat (:services analysis)
                                (:models analysis)
                                (:converters analysis)
                                (:behaviors analysis)
                                (:extensions analysis)
                                (:core analysis))]
    (batch-generate-specs all-components 15)))

;; ============================================================================
;; Regeneration Functions
;; ============================================================================

(defn regenerate-spec
  "Regenerate spec file for a specific component by name."
  [component-name]
  (p/let [analysis (da/quick-component-summary component-name)]
    (if analysis
      (p/let [root (da/workspace-root)
              output-dir (path/join root "specs" "000-dependency-capture" "dependencies")
              spec-file (generate-spec-file analysis output-dir)]
        (vscode/window.showInformationMessage
          (str "Regenerated spec for " component-name))
        spec-file)
      (vscode/window.showWarningMessage
        (str "Component not found: " component-name)))))

;; ============================================================================
;; Interactive Generation
;; ============================================================================

(defn interactive-spec-generation
  "Interactive menu for spec generation."
  []
  (p/let [choice (vscode/window.showQuickPick
                   #js ["Generate all specs"
                        "Generate service specs only"
                        "Generate model specs only"
                        "Generate converter specs only"
                        "Regenerate single spec"]
                   #js {:placeHolder "Select spec generation option"})]
    (case choice
      "Generate all specs"
      (generate-all-specs)
      
      "Generate service specs only"
      (p/let [analysis (da/analyze-all-components)]
        (batch-generate-specs (:services analysis) 10))
      
      "Generate model specs only"
      (p/let [analysis (da/analyze-all-components)]
        (batch-generate-specs (:models analysis) 15))
      
      "Generate converter specs only"
      (p/let [analysis (da/analyze-all-components)]
        (batch-generate-specs (:converters analysis) 10))
      
      "Regenerate single spec"
      (p/let [component-name (vscode/window.showInputBox
                               #js {:prompt "Enter component name (e.g., DatabaseService)"})]
        (when component-name
          (regenerate-spec component-name)))
      
      nil)))

;; ============================================================================
;; Public API
;; ============================================================================

(comment
  "Usage examples:"
  
  ;; Generate all specs at once
  (generate-all-specs)
  
  ;; Generate specs for specific components
  (p/let [analysis (da/analyze-all-components)]
    (generate-specs-for-components (:services analysis)))
  
  ;; Regenerate single spec
  (regenerate-spec "DatabaseService")
  
  ;; Interactive menu
  (interactive-spec-generation))
