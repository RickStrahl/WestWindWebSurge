# West Wind WebSurge Change Log

### Version 0.72b
<i><small>not released yet</small></i>
* **Rework the Request Viewer HTML View**
The HTTP Request viewer for completed requests now seperates out
Response headers and Response content. New option buttons allow
you view html, json and xml in syntax formatted views as well as
shell out to see in the native viewer/editor.

* **Add resizable Headers and Content Panel Splitter**
You can now resize the content and header panels when editing request input.
This allows you to more easily edit this content. Last sizes of the panels are 
remembered between loads of the application.

* **Add double-click popup editor for Headers and Content**
You can now also double click on either the header or content panel to get 
a dedicated editing window for the header or content text. If you're dealing
with large or unformatted content having a maximized view can make editing 
a bit easier.

### Version 0.70b
<i><small>October 8th, 2014</small></i>

* **Store Configuration Settings with saved Session Files**<br />
Configuration options are now stored as part of a Saved Session file and stored 
in the same text file that contains the raw HTTP header request info. This
allows restoring the same configuration settings and storing it with the 
Session file.

* **Session Management Enhancements**<br/>
Tweaked the Session management features to make it easier to created edit and save URLs for testing manually. Hotkeys for most operations (especially Alt-T for testing selected URL) to improve URL testing and creation workflow.

* **Add ReplaceQueryStringValuePairs Option**<br/>
Added a new override option that can replace and add querystring values to all
requests in a session. You can simple set key value pairs and those key value
pairs are replaced or added on each request. Useful for adding ids or tokens
required for user tracking.

### Version 0.69b
<i><small>September 4th, 2014</small></i>

* **Add Export of Summary Results**<br />
You can now export summary results to JSON so you can archive or otherwise
store the summarized results from a test session. Use the Export button
in the UI form, or the new --json option on the WebSurgeCli.exe command 
line to capture the summary results as JSON.  

* **Add Copy Request Option**<br />
You can now copy an existing request using a new Copy from Request 
shortcut menu option on the request tab. Facilitates creating similar
requests more quickly.

* **Request Test, Edit, Preview shortcuts**<br/>
There are now shortcut keys to most request related operations. 
This makes it quicker to test and edit individual requests very
quickly.


### Version 0.68b
<i><small>August 15th, 2014</small></i>

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

### Version 0.67b
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


### Version 0.61b
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
