# West Wind WebSurge Change Log

### Version 1.08.5
<i><small>not released yet</small></i>

* **Fix: DataBinding Bug for Request Data Editing**  
Fixed issue where request data was not properly saved after running a test as the active request was overwritten by newly created instance.

* **Fix: Cookie TrackSessions Tweaks**   
Change `TrackSessions` behavior so if no Cookies are yet assigned the original cookies from the request are sent. Cookie Container is also cleared now when you turn `TrackSessions` to `false`.


### Version 1.08
<i><small>August 12th, 2017</small></i>

* **Cookie Tracking in Interactive Session**  
If `TrackSessionCookies=true` in the configuration, the interactive test session now tracks cookies created by a test and passes forward that same cookie on subsequent requests. This means you can run an authenticate link to get a cookie and then run subsequent requests that use that same cookie. Tracked cookies override both explicit cookies in the request header, as well as `ReplaceCookieValue` overrides.

### Version 1.07
<i><small>July 14th, 2017</small></i>

* **Automatic Cookie Tracking via Configuration Option**  
Cookies are now automatically tracked per session (group of URLs) when used in a test run. Each session runs on an isolated thread and cookies are reset as each session is restarted in a test run or individual test. Use the `TrackSessionCookies` configuration flag to enable or disable Cookie tracking. The default is `true` - set to `false` if you don't use Cookies that carry forward in your tests to improve test throughput.

* **UrlEncoded Form Request Data Formatting**  
UrlEncoded form data with content type of `application/x-www-form-urlencoded` automatically displays formatted key value pairs rather than the raw UrlEncoded data. Raw View shows the original URL encoded data. This makes it easier to understand form encoded data.

* **Experimental: User Management**  
You can now set up multiple users that are assigned to sessions by mapping login URLs and login form data to specific users. This feature is experimental and doesn't support UI at the moment and requires configuration via JSON text for a WebSurge configuration file. [More info](https://websurge.west-wind.com/docs/_4ym15ftd2.htm).

* **Add --output Command Line Switch to WebSurgeCli**  
The `--output` flag sends the final Web Surge report output or `--json` response from the WebSurgeCli to the specified file.

* **Group Test Results by Verb + Url + Name**  
Previously test results were grouped by Verb + URL only which caused problems for some scenarios like Web Services that are always using the same Url. Now the name is used in combination and combined with the Url and displayed on the test results page.

* **Fix: JSON Result Formatting**  
Fix but with various values in the JSON result displaying as Milliseconds when they should display as seconds. 

### Version 1.02
<i><small>February 30th, 2017</small></i>

* **Add additional XML preview formats**  
Added additional XML preview formats to shows as formatted XML.

* **Fix Request Editing Focus Issues**   
Changed the request editing to not require explicit **Save** operations. New entries are now immediately added to the Request list and displayed in the list and any updates are immediately reflected in the request - no explicit save operation required.


### Version 1.01
<i><small>Oct. 10th, 2016</small></i>

* **Code Signed Exe's and Installer**  
The WebSurge installer file and main binary EXEs are now code-signed for verification purposes.

* **Byte Size Display on Response Pane**  
The result Response pane now shows the byte size of the actual request. The actual size may differ from the Content-Length if the content is encoded, or if the content is chunked and there is no Content-Length sent from the server.

* **File Exports are not auto-opened**  
Due to the large file size of exported reports exported test results are no longer opened, but simply shown in Explorer and highlighted. You can choose to open the files from Explorer.

### Version 1.0
<i><small>Sept. 2nd, 2015</small></i>

* **Change licensing and remove limits on free version**  
Remove limitations on the free version and change licensing to
make free for personal use and paid for commercial use. 

* **[Plugin Support for Extensibility](https://github.com/RickStrahl/WestWindWebSurge/wiki/Creating-a-WebSurge-Plugin)**  
Initial plugin interface that allow hooking into request processing as requests are fired against the server. You can hook `OnBeforeRequestSent` and `OnAfterRequestSent` methods that allow you to manipulate inbound requests and capture result requests as they are completed for custom logging or live analytics.

* **First Byte Times Added to Results**  
Result objects now contain a value to show milliseconds to
first byte for more information on each request.

* **Add Name to the Request Entry**  
Added a name field to make it easier to differentiate similar URLs when
testing APIs. In the request list the name displays if it's provided,
otherwise the URL displays.

* **Add Sorting to the Request List**
You can now sort the request list by using either a menu option or shortcut keys to move items up and down in the list. Useful to properly group requests together.

* **Add Test All Button to Session List**<br/>
Option allows you to run all selected tests in a session at once
and see a summary result page, as well as a result list where you
can check the output from requests and responses

* **DoubleClick on Session Request to Test Item**<br/>
You can now DoubleClick on a request in the Session list to fire
that request and see the request and response displayed. Facilitates
mouse only navigation and is in addition to using the toolbar button 
and the Alt-T shortcut.

* **Add .websurge Windows Extension for Request Files**<br/>
When you save request files the default extension is now .websurge,
which is now a registered extension in Windows so you can just 
open a request file by double clicking. This also makes it easier
to separate request files from saved result traces or .txt export
stored in the AppData folder. Also added support for Most Recently
Used (MRU) list so most recent files always show up on taskbar and
desktop icons.

* **Share Session files on DropBox or OneDrive**  
You can now open and save session files from DropBox and OneDrive using the Cloud Drives option from the **File** menu. This makes it easy to share your Session files across machines more easily and share them with others on your team.

* **Add requirement for Remote Server Safeguard files**  
WebSurge now requires that any remote server tested has a `websurge-allowed.txt` file in the root folder, or a `#Allow: WebSurge` comment entry in `robots.txt`. Localhost URLs don't require these files and neither do single URL tests. 

* **Change Update Download Location**<br/>
Change the download location to the system Downloads folder to 
avoid potential issues with Anti-virus software flagging the
downloaded file as malicious.

* **Fix Content Encoding for Request Data**  
Fixed content encoding to detect request encoding format properly
when posting data to the server. Content is still captured and displayed
as Unicoded (decoded) text for ease of editing, but content sent
to server is encoded in the encoding specified in the request.
If no content type encoding is specified UTF-8 is assumed.

* **Improve throughput for High Volume Testing**  
Tweaked the per request yielding between requests to allow for faster request processing on very high volume sessions. Performance should improve 2-3 fold in some scenarios. Note that optimal thread configuration might involve **less** threads. In tests we found to maximize load best performance for very high load scenarios is often achieved with 1 thread per CPU Core.

### Version 0.75b
<i><small>January 20th, 2015</small></i>

* **Syntax Coloring for Request and Response Content**<br />
Both request and response content now allow for a 'Formatted'
view that displays common display formats in prettified form.
HTML, JSON, and XML are syntax colored. A 'Raw' view still
exists to show the original content as well as a 'preview'
mode that displays the content in your preferred browser
which is useful for HTML content.

* **JSON Content can be Prettified**<br />
JOSN content in either the request or response content is 
'Formatted' request/response content viewer to make it easier to
read JSON data. Raw view still shows the original raw response.

* **Show actual Response Headers for results**<br />
You can now see the **actual** request headers instead of the headers 
that were entered for the request. You'll see things like calculated 
content-length, auth headers etc. that are injected by the HTTP client
when the request runs.

* **Add support for Username and Password in Options**<br/>
You can now specify a username and password for authentication for NTLM 
and Basic Authentication that is applied to all requests in a test. You 
can also use 'AutoLogin' to login in to NTLM sites using your current
Windows Credentials of the logged in user.

* **Encryption of Authentication Password**<br />
The password is now saved with sessions and is encrypted for the 
local machine both in saved session files as well as the global 
configuration files.

* **Selectively enable and disable Requests from Test**<br />
You can now use the Active flag on individual requests to enable or
deactivate request to run. This option facilitates quickly enabling/disabling 
requests.  Good for disabling all but one, or quickly excluding 
a failing URL from test runs.

* **Collapsible Panels for PreView Sections**<br />
The Preview's Request and Repsonse header and context sections are now
collapsible to allow easier navigation of long requests, especially
those that have long request content. Plus/minus buttons let you
collapse any panel to just the header to quickly hide large content.

* **Miscellaneous Fixes**</br>
Add Content-Length: 0 to no body requests that POST or PUT requests
(even though that's a bad idea - apparently this happens frequently).
Show actual request headers sent rather than just the headers entered
for requests. Fix 64 memory issue - memory usage is no longer limited
to 1.5gig for large tests.

### Version 0.72
<i><small>Novemember 24th, 2014</small></i>

* **Rework the Request Viewer HTML View**
The HTTP Request viewer for completed requests now seperates out
Response headers and Response content. New option buttons allow
you view html, json and xml in syntax formatted views as well as
shell out to see in the native viewer/editor.

* **Add resizable Headers and Content Panel Splitter**<br />
You can now resize the content and header panels when editing request input.
This allows you to more easily edit this content. Last sizes of the panels are 
remembered between loads of the application.

* **Add double-click popup editor for Headers and Content**<br />
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
 

### Version 0.60
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
