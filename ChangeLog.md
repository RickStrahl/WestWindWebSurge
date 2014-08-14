#West Wind WebSurge Change Log

###Version 0.69b
<i><small>not released yet</small></i>

* **Make URL Test Asynchronous**<br/>
When you click the Test button on a request URL the URL is
now executed asynchronously and no longer locks up the UI.
Statusbar shows checking and completion notice and Preview
form pops up when done just as before. You can also use a
Alt-T shortcut to test the current request quickly.

* **Support for Razor Templates Intellisense**<br />
Added bin folder to the Razor templates folder so that Intellisense
now works for the preview and result templates.

* **Fix Content Type Bug on Requests**<br/>
Content type would not properly display in the preview form. 
Reworked import and capture and display mechanisms to only 
display Content type from headers - not from ContentType property.

* **Additional Menu Options to Save Sessions**<br/>
You can now use Ctrl-S to save an open Sessions without having to
reconfirm the file name to make it quicker to save Session changes.
Save options added to File menu.

* **Save Session no longer prompts for File Overwrites**<br/>
When saving you are now no longer prompted whether you want to overwrite
the file if it already exists. Also added short cut for Save operation.

###Version 0.67b
<i><small>August 7th, 2014</small></i>

* **Add Authorization Header Override to Options**<br />
You can now override the Authorization header for individual
requests by setting an override in the Options. The override 
from options is applied only to existing requests that already
include an Authorization header.

* **Optimize UI Status Updates**<br/>
Fixed several issues in the logic for the UI update routines
that caused slow operation for 'console view' updates to the
textbox. Fixed sizing bug that leaked memory. Performance 
of console view is now a lot closer to summary only view,
but still slower as should be expected for the UI overhead.

* **Add Raw HTTP Export of Results**<br />
You can now export results in raw format that simply exports the
raw results from a test run as HTTP headers and content. The format
is plain text and can be re-imported later (pending feature), so
you can review a previously run test.

* **Updated Results View**<br/>
The results view now shows 404 and 401 responses with different
icons rather than the error icon. Result pane also shows a bit
more data, and shows total for the list displayed (All, Success, Errors)

* **Fix crash bug with short tests**<br />
Fixed bug that would crash WebSurge when short tests were run,
due to no results view. Results view now returns empty.

* **Fix Warmup Seconds parsing**<br/>
Warmup seconds weren't processed properly in previous builds as
threads would start running before the flag was set. Warmup seconds
now properly initialize immediately and are properly removed before
summarizing results.

* **Add Installer Build Batch File**<br/>
Added a batch file to pull in all distribution files and
run the InstallMate installer from the command line to copy
files, and compile final distribution EXE in one step.
WebSurge in one step.



###Version 0.65b
<i><small>August 1st, 2014</small></i>

* **Add SSL Cert Installation for HTTP Capture**<br />
Added support to install the required Fiddler SSL capture certificate 
directly from within the application. As before WebSurge works
with existing Fiddler certificate installations, but if you don't
have Fiddler installed you can now enable the interception 
certificate directly from within WebSurge's capture screen.

* **Additional Error Logging**<br />
The application error handler now logs a number of 
additional errors including hard crash errors. Slight
error log format updates to make errors easier to read.


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
