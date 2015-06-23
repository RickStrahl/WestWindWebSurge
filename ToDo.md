# West Wind WebSurge ToDo List

This document describes various fixes, ideas for new features and various 
work items. 


### Miscellaneous Fixes required
* **Add per request username and password option**<br/>
In addition to the global username and password option we also should 
support username and password per request. This is already supported,
but there's no UI and no way to store this info in a request (use
custom headers like the Active flag).

<s>* **Hide and encrypt Password for Authentication Options**<br/>
Currently the password in the options is entered and shown in plain text.
When the request project is saved the password is saved along with the 
request data also in plain text. Data can be entered as plain text, but 
then needs to be displayed as ****. When written to file the value needs
to be encrypted.</s>


### Features
* **Sync multiple WebSurge Instances to increase load**<br/>
Allow multiple instances of WebSurge on multiple machines run the same
test and synchronize their results. One machine acts as a master the
others as slaves controlled by master.

* **Get IIS Server Performance Stats**<br/>
Already started collecting performance data and implementing IIS module
to retrieve perf data to client. Have to create background thread that
periodically picks up this performance data and either graphs or displays
the progress.

### Artwork
* **Web Surge Logo Cleanup**<br/>
Logo needs to be smoothed out. Would like to add a byte cloud into the
black background of the warning logo - fade white code/text to black 
gradient. Any graphic artists that want to help out?
