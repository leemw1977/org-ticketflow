;;; Load and configure org-ticketflow
(setq debug-on-error t)

;; Optionally: autoload the package
(when (featurep 'org-ticketflow)
  (unload-feature 'org-ticketflow t))

(require 'org-ticketflow)

;; Set the base URL for your Jira instance
(setq org-ticketflow-base-url "https://topcashback.atlassian.net")


;; Optional: bind a key to test connection quickly
(global-set-key (kbd "C-c t") #'org-ticketflow-test-connection)
