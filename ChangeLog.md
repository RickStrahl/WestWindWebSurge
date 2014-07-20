#West Wind WebSurge Change Log

###Version 0.61b
<i><small>July 21st, 2014</small></i>

* **Preview HTML, XML and JSON output**<br />
You can now preview HTML XML and JSON output using
formatted output for individual requests. Use the new
output-type hyperlink left of the Http Response header.

* **Redesigned Test Result and Preview Panes**<br/>
As a result of all of this view reshuffling the UI has been
updated to look a bit better integrated into the application.
Summaries are color coded for success and failure (green/red)
and headers and layout have been made more.

* **Template Rendering for output**<br />
All output is now rendered through Razor templates, 
which simplifies HTML layout and modifications.
 

###Version 0.60
<i><small>July 1st, 2014</small></i>

* **Add Support/Feedback Links to Help Menu**<br/>
There are now links to the message board in the help menu to
allow for feedback and support on our forums. We would like
to hear from you, so please let us know what we're doing right,
what you'd like to see improved or what's broken and not working
for you.

* **Integrated New Version Check**<br/>
WebSurge now automatically checks for a new version periodically
(configurable in config file). If a new version is available it
can be optionally downloaded and be launched for install optionally.

* **Configuration Refactoring**<br/>
Refactored the configuration settings and storage mechanism to
more logically reflect the structurue of settings. This minor
update should have little to no impact on existing applications,
but after updating check your settings on the options tab to 
make sure.

* **Open Settings and Templates Folder**<br/>
There's now a menu option that jumps to the settings and templates
folder to allow you to directly edit the WebSurgeConfiguration.json
file. You can also tweak the templates and CSS file used to render
results.


###Version 0.50

* **Add Session Management UI**<br/>
You can now use the new Session menu buttons to manage
creating, editing, deleting, capturing, saving and raw
file editing of Sessions.

* **Added new Request Edit Pane**<br/>
New request editing pane lets you create and edit URL
requests and headers as well as test them. Test button
runs requests and shows results on the output pane.
