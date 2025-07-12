# Welcome to org-ticketflow
This package has been designed to link up with your ticket systems.

The first system will be Jira because that is what my company is using, but I could see this being used for GitHub too.

It aims to provide that bridge between the ticketing system and Org mode in Emacs, so you can remain in your familiar environment but still keep your stakeholders happy.


You can execute something like this in the Scratch buffer if you want to download and test locally:

```
(unload-feature 'org-ticketflow t)
(load-file "./org-ticketflow.el")
(require 'org-ticketflow)
(setq org-ticketflow-base-url "https://example.atlassian.net")
```



## 🔒 Security Notice
org-ticketflow runs a local HTTP service on 127.0.0.1  TODO: and uses a simple token-based authentication mechanism.

This is suitable for single-user desktop environments where Emacs and the service run under the same user account.

If you are running in a shared or untrusted environment, be aware that:

Loopback traffic can be intercepted by privileged processes.

TODO: The authentication token is stored temporarily in memory and may be visible to other users on the same machine.

You use this software at your own risk under the terms of the GPLv3 license. Do not expose the service on external ports or use it for production-level secrets without additional hardening.