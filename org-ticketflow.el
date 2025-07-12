;;; org-ticketflow.el --- Two-way sync between Org-mode and your ticketing system -*- lexical-binding: t -*-

;; Author: Lee Williams
;; Version: 0.1.0
;; Package-Requires: ((emacs "27.1"))
;; Homepage: https://github.com/leemw1977/org-ticketflow
;; Keywords: org, jira, tools, tickets
;; License: GPL-3.0-or-later

;;; Commentary:

;; org-ticketflow provides a two-way bridge between Org-mode and a ticketing
;; system via a platform-specific CLI. This version supports JIRA.

;;; Code:

(require 'auth-source)
(require 'json)
(require 'subr-x)

(defgroup org-ticketflow nil
  "Two-way Org-mode sync with Jira or similar systems."
  :group 'applications)

(defcustom org-ticketflow-cli-path nil
  "Path to the OrgTicketflow CLI binary. If nil, a default is selected by OS."
  :type 'string
  :group 'org-ticketflow)

(defcustom org-ticketflow-base-url nil
  "Base URL of the JIRA instance (e.g. https://example.atlassian.net)."
  :type 'string
  :group 'org-ticketflow)


(defconst org-ticketflow--base-directory
  (file-name-directory
   (or load-file-name
       (when (boundp 'bytecomp-filename) bytecomp-filename)
       buffer-file-name
       (locate-library "org-ticketflow")
       default-directory))
  "Base directory where the org-ticketflow package is located.")


(defun org-ticketflow--resolve-cli-path ()
  "Return the path to the CLI binary based on OS."
  (let ((relative
         (cond
          ((eq system-type 'darwin)    "bin/macos/OrgTicketflowCLI")
          ((eq system-type 'gnu/linux) "bin/linux/OrgTicketflowCLI")
          ((eq system-type 'windows-nt) "bin/windows/OrgTicketflowCLI.exe")
          (t (error "Unsupported OS")))))
    (expand-file-name relative org-ticketflow--base-directory)))

(defun org-ticketflow--get-cli-binary ()
  "Return the CLI binary path, using override or default."
  (or org-ticketflow-cli-path
      (org-ticketflow--resolve-cli-path)))


(defun org-ticketflow--get-credentials ()
  "Retrieve Jira auth info from auth-source based on `org-ticketflow-base-url`."
  (unless org-ticketflow-base-url
    (user-error "Please set `org-ticketflow-base-url`"))

  (let* ((host (url-host (url-generic-parse-url org-ticketflow-base-url)))
         (entry (car (auth-source-search :host host :require '(:user :secret)))))
    (unless entry
      (user-error "No matching auth-source credentials found for host %s" host))
    (let ((user (plist-get entry :user))
          (secret (let ((sec (plist-get entry :secret)))
                    (if (functionp sec) (funcall sec) sec))))
      (list :username user :token secret))))

(defun org-ticketflow-test-connection ()
  "Call the CLI to test connection to Jira."
  (interactive)
  (let* ((creds (org-ticketflow--get-credentials))
         (bin (org-ticketflow--get-cli-binary))
         (payload (json-encode (append creds
                                       `(:baseUrl ,org-ticketflow-base-url))))
         (output-buffer (generate-new-buffer "*org-ticketflow-output*"))
         (error-buffer (generate-new-buffer "*org-ticketflow-error*")))
    (if (not (file-executable-p bin))
        (message "❌ CLI tool not found or not executable: %s" bin)
      (let ((proc (make-process
                   :name "org-ticketflow-test"
                   :buffer output-buffer
                   :stderr error-buffer
                   :command (list bin "test-connection")
                   :coding 'utf-8
                   :noquery t
                   :connection-type 'pipe)))
        (process-send-string proc (concat payload "\n"))
        (process-send-eof proc)
        (set-process-sentinel
         proc
         (lambda (proc event)
           (unwind-protect
               (cond
                ((eq (process-exit-status proc) 0)
                 (with-current-buffer output-buffer
                   (goto-char (point-min))
                   (let ((json (json-parse-buffer :object-type 'alist)))
                     (message "✅ Authenticated as: %s"
                              (alist-get 'displayName json)))))
                (t
                 (with-current-buffer error-buffer
                   (message "❌ Error: %s" (string-trim (buffer-string))))))

             (kill-buffer output-buffer)
             (kill-buffer error-buffer))))))))

(provide 'org-ticketflow)

;;; org-ticketflow.el ends here
