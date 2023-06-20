<img src="WebSurgeLogo.png" align="right" height="130" />

# West Wind WebSurge Change Log 

[![Download WebSurge](https://img.shields.io/badge/Download-Installer-blue.svg)](https://websurge.west-wind.com/download)
[![Download WebSurge from Chocolatey](https://img.shields.io/chocolatey/dt/westwindwebsurge.svg)](https://chocolatey.org/packages/WestwindWebSurge)
[![Web Site](https://img.shields.io/badge/West_Wind_WebSurge-WebSite-blue.svg)](https://websurge.west-wind.com)

### Version 2.3 
<small>not released yet</small>

 * **Saved Request Viewer**  
 Added a separate saved request viewer that you can use to temporarily save individual requests for later review or comparison separately from the main request viewer window. 
 
 * **Improve Slow Request Processing Display**  
 Update the display when running slow requests with a bit of delay so you only see it if requests are slow. Requests that take multiple seconds now also show the number of cumulative seconds requests has been running.
  
 * **Allow Import of Visual Studio\VsCode .rest and .http Files**  
 You can now import all Visual Studio (or VS Code) `.http` files, or a folder of multiple `.http` files. Both individual requests and request lists in a single `.http` file are imported into the current project.
 
 * **Support for Session Variables**  
 You can now create a list of replaceable session variables that can be injected into content, using replacement syntax that is compatible with Visual Studio's `.http` files. Using `@varname=value` declarations (in Session Configuration) and then embedding with `{{ varname }}` into Url, Headers and Content.

 * **Paste CURL Commands into Request Editor**  
 You can now use the Paste button to paste CURL commands directly into the request editor. Note not all CURL features are supported but many of the most common ones work.
 
 * **Improved Live Request Editing**  
 The request editor is now more selective about what can be edited in the Previewer for Url, Request Headers and Request Content. If doing a plain (non-executed) Preview of a request, everything is always editable. On executed HTTP Requests requests are editable only when requests do not contain variables or other embedded content that might affect the initial content. Ideally all editing should only 
 
 
 * **Show Audio Files as Audio Player**   
 If you download MP3 files as audio they will now display as a music player that you can use to play the audio file in the Response Content area. This is similar to images which are displayed as images in the previewer.0
 
 * **Preview Zoom Sticky**  
Previously you could zoom into the Preview View of the Http Request and Result pane, but the zoomed view would only be active for the current request. As soon as you re-ran the preview would resize to the original un-zoomed setting. This update keeps the zoom fixed for the duration of the application once the previewer has been activated.

* **Improve Default Request Display in the Request List**  
Requests in the Request List now show only the last segment of the URL as list item title rather than the full URL specified for the request. This often reduces the need to explicitly provide a display title for requests.

* **Fix: Bearer token replacement only occurs if `Authorization: Bearer` is present**  
Fixed behavior of authorization override for bearer token to only replace Authorization headers when a `Authorization Bearer` header is already present in the request headers. This is to ensure that other authentication like Basic for auth requests are still honored and not replaced by the bearer token replacement.

 * **Fix OpenApi/Swagger Import Dialog**  
Fix issue with the import dialog that would not pre-validate JSON spec content correctly, incorrectly flagging it as invalid. Fixed. 
 
### Version 2.2
<small>January 24th, 2023</small>

* **Add support for HTTP 2.0**  
You can now run requests in HTTP/2 mode. Http Mode is specified as part of the session and is displayed in the Request Preview as `HTTP/2.0`. HTTP 2.0 falls back to 1.1 if the server doesn't support it and that is reflected in the response from the server which then shows `HTTP/1.1`.    
*note: the HTTP/2 header display uses the HTTP/1.1 syntax as 2.0 syntax is much less readable/editable*.

* **Edit Url and HttpVerb in Preview Display**  
You can now edit the URL in the Request Previewer along with the headers and request body. You can still navigate the URL as well, but it now uses Ctrl-Click.

* **Specify Item to open from CommandLine**  
You can now specify which item to open by using the following syntax: `WebSurge startupFilename:startupItem`. `startupItem` can be a URL, name or request ID from Session file.

* **Open Request in new Window**  
You can now open any request in a new WebSurge instance to allow opening and running multiple requests simultaneously. Very useful for reference.

* **Copy and Paste Requests**  
You can now copy a request using the copy button and paste it into a new request. Request data is copied to the clipboard in plain text, HTTP header syntax, and can also be used for easily pasting into blog posts documentation.

* **New Recent SiteBaseUrl List to allow easy switching between sites**  
The Session configuration now includes a dropdown for the SiteBaseUrl that keeps track of the last 5 unique base Urls used for the session. This makes it easy to switch between different sites for testing (ie. local dev, staging, live site etc.) without having to duplicate session files. The list is stored with the `.websurge` Session file so the list can also be checked in as part of a project and shared across developers in dev environments.

* **Print Request on Menu and Button Bar in Previewer**  
You can now more easily print to a printer or save the current request to PDF. This operation takes the Request output from the previewer and sends it to the Print output. Print Request is now available on the Button Bar at the top, in the Context Menu for request content and body, and in the main body of the document. `Ctrl-p` now also works more reliably both in the editor and Previewer.

* **Add Basic Authentication Username and Password Dialog**  
You can now specify Basic Authentication for individual requests via the Context Menu and the *Basic Authentication* option. You can enter and update and clear basic authentication `Authorization` headers via this dialog that handles encoding of the userid:password combination.

* **Bearer Token Context Menu Options**  
The request menu now has a number of Bearer token defaults that display including *Empty Bearer Token* (adds an Authorization: Bearer with not token header), as well as several WebSurge specific settings that can capture a token and either save it and manually assign it or set the global Authorization override in the settings automatically.

* **Add Request Name to the Tab Bar**  
If the request has a Name associated with it, that name is now displayed on the Tab bar above the request. This makes it easier to see what's active even when a request has already run and only shows the URL.

* **Preview Request Buttons stay Visible on Scrolling**  
The Request buttons in the HTML Previewer (Run/Print/Copy/Paste) now are pinned to the top of the preview window and stay visible even if you scroll down the request display. This makes it more consistent to re-run the current request.

* **Update to .NET 7.0**  
The application now runs under .NET 7.0 which brings additional performance improvements for load testing and general UI operation.

* **Improved UI Responsiveness for the Previewer and Live Request Editor**  
There have been many improvements to integration with the new features of the WebView component used for the previewer, request capture and documentation window. The new async related improvements improve stability and responsiveness of any of the HTML based interfaces.

* **Support for OpenAPI 3.0 Imports**  
There's now support for importing OpenAPI 3.0 service descriptions as WebSurge Sessions. There are also improved error messages when errors occur.

* **Fix: First time error page on Request Preview**  
Fix issue where the request viewer briefly showed a *Page Unavailable* page when first displaying request information. 

* **Fix: Multipart Form Variables Encoding**  
Fix issue with CRLF breaks in the form variable output - in some cases only LF was generated which breaks some servers that parse by explicitly looking for CRLF line breaks. 

* **Fix: Addin OnBeforeRequestSent() Timing**  
Fix issue where headers were sent before this addin was fired resulting in headers not getting applied. Fixed [#101](https://github.com/RickStrahl/WestWindWebSurge/issues/101)

### Version 2.1

<small>March 31st, 2022</small>

* **Interactive Http Forms Editor**  
Added an interactive forms editor for UrlEncoded and multi-part forms. The editor allows for interactively editing form variables and values, and for multipart forms allows for loading file content and encoding it. This complements the existing edit view, and urlencoded interactive editing in the previewer. 

* **Support for Custom Users**  
Added support for users that can be assigned to a session. Users define session specific information that can vary between users - for example separate login credentials or variables to log in with for different users, or different values to fill into various controls. User can add global Cookies and Authorization headers, or URL specific headers and form variables. 

* **Separate Http Context for each executing Session**  
When running load tests, each **Thread** or Session of requests, gets it's own dedicated Http call context that is reused for all requests on that thread. This allows separate threads to have separate context so each context can maintain its own authorization, cookie and other Http state.

* **Better handling of Binary Data in Multi-Part forms**  
Multipart forms now display binary content as `b64_<long base64 string>` in the Request editor. This format is recognized by the request runner and it appropriately expands the binary data to send the request. You can create the binary data interactively in the **Http Forms Editor** (`ctrl-t` in the Request Content Entry textbox).

* **Request Grouping**  
You can now optionally assign requests to a group and drag and drop between groups. This makes it a little easier to organize requests in long request lists that don't need a fixed sequence.

* **Clickable File Link in StatusBar**  
The open file in the editor is now a clickable link in the right corner of the statusbar, so you can easily grab the path and jump to the file location. The tooltip also now displays the folder information.

* **When importing Multi-Part forms via Request Capture Parse Form Vars**  
Form variables are now parsed when importing multi-part forms via the Request Capture Form. Previously data was captured as a single `b64_` binary blob. You can now view and edit captured the data in the Forms editor.  
**Note**: Binary data still can't be captured with the Request Capture tool due to a bug in the WebView control that doesn't return this data.


### Version 2.0.1

<small>February 2nd, 2022 • [Initial Release Post](https://weblog.west-wind.com/posts/2022/Feb/01/West-Wind-WebSurge-20-is-here) &bull; [Release Video](https://www.youtube.com/watch?v=jgEYn-ldr30) </small>

* **[Complete UI Overhaul](https://websurge.west-wind.com/docs/_67i10usf7.htm)**  
Rebuilt user interface of the application entirely using modern, responsive and more flexible UI functionality. The new Request Previewer provides more request and response information and allows for two-way editing of request data in addition to the official request editor. There are dark and light application themes and the entire UI is now responsive and reflects changes made immediately. The new look is more modern, but also offers much better interactivity for a much improved workflow.

* **Updated to run on .NET 6.0**  
Updated application to run on .NET 6.0 to take advantage of new performance features. Chief functionality updates include a completely re-written HTTP request engine for more efficient request processing which much lighter processor usage. This translates into higher request load and less request overhead both in terms of memory use and load on the system while testing.

* **Improved Performance and lower CPU Usage**  
The application is overall faster, taking advantage of many of the performance enhancements in the .NET platform, as well as using the new interface which is much more fluent and responsive to user input. A key update is the request processing engine which has been completely re-written for improved Http request throughput and much lower CPU usage while running load tests.

* **More Interactivity for Http Request Views**  
The live Request Preview is fully editable now. You can edit both the request Preview as well as the Http Result Preview which also contains the result Http headers and content. Both are now editable and can update the Http Request. This is in addition to the traditional form based request editor view. The interactive editing makes it much quicker and easier to tweak Http request headers and content interactively as you run requests without constant context switches.

* **[New Test Results View](https://websurge.west-wind.com/docs/_44b0366fb.htm)**  
The Test Results View is now browser based and uses in browser technology for faster rendering. The request per second graph is now embedded into the result view. We've also added 99th, 95th, 1st and 5th percentiles to the Results View and cleaned up the display to show data at a glance and to be more visually appealing.

* **[Completely overhauled User Management System](https://websurge.west-wind.com/docs/_6820px62u.htm)**  
Each executing session thread is now mapped to a User. Users contain a unique HTTP instance with its own connection state for things like cookies, caching and and authorization state. In addition, you can now capture and inject JSON tokens and Http Form variables (capture in one request and inject into another) via Http Headers and override global settings like Cookies and Authorization for a given user context. 

* **[New User Management UI Form](https://websurge.west-wind.com/docs/_6910v2k3q.htm)**  
You can now interactively edit user information with the new User Management Form. This interface lets you easily create users add Urls to override behavior and more.

* **[Updated Addin Interfaces](https://websurge.west-wind.com/docs/_5gq0iwk9n.htm)**  
The .NET based Addin engine has been rewritten and simplified with a simple .NET Assembly based deployment model. This model is meant for extending WebSurge functionality for test processing with intercept points for `OnBeforeRequestSent()`, `OnAfterRequestSent()` and `OnLoadTestStarted()` and `OnLoadTestCompleted()`. These simple interfaces allow modification of requests while tests or individual requests are running. Using these simple intercept methods allow you to intercept each request in a test and dynamically modify requests to inject dynamic data into the Http request data. It also allows for custom logging and interfacing tests with external tools that can receive live test data as tests run.


* **You can now Print and Save the Results Page**  
The results page now sports the regular browser Print dialog that allows you to print or save the results page, with full fidelity to the Printer or PDF File. Accessible via context menu on the results page or via **Results -> Print**.

* **Updated CLI Test UI**  
We're now using much richer display output to show progress information during tests and to display the result view. The new view uses tables and colors to make results easier to view at a glance. For raw data result capture `--json` option is also available.

* **Image and Html Previews**  
Image results now display the image, rather than the binary data. HTML result displays HTML as unformatted (default) view and the rendered HTML in formatted view.

* **[Import Requests from OpenAPI/Swagger](https://websurge.west-wind.com/docs/_67l10xhlr.htm)**  
You can now import a session of request from OpenAPI/Swagger using either a Url, raw Json, or a local file. 

* **Import and Export Postman Collections**  
You can now use the **Session->Import and Export->Import from Postman** to import Postman collections into WebSurge and **Session->Import and Export->Export to Postman** to export WebSurge sessions into Postman Collections.

* **Import Fiddler Traces**  
You can now use Fiddler to capture requests and export them via **Save Sessions->Save As->Text** in Fiddler to export selected Http traces and then import them into WebSurge via **Session->Import and Export->Export from Fiddler**.


### Version 1.16
<i><small>March 30th, 2020</small></i>

* **Allow editing Request Data in Previewer**  
You can now interactively edit the request data in the preview window using rich JSON/XML and HTTP header editing to edit the content for POST content and the HTTP Headers for each request. Editing in the preview is quicker and provides a richer editing experience than using the request entry form. Requests still have to be created in the request editor, but once created updating and customization of requests can be done in the previewer.

* **Add Syntax Highlighting to pop-out Request Editor**  
Added syntax highlighting to the popout editor available when double clicking on the request editor. The editor pops up with the appropriate syntax highlighting for HTTP headers or Request content in xml, json format. The editor is a full editor interface with better support for text spacing and re-alignment that makes it easier to edit text.

* **Add Request Header Context Menu to add common Headers**
You can now use the context menu to quickly pop in common headers like Content Type, Accept, and Accept-Encoding headers into the Request header both in the Request Editor and Previewer. Context menu also shows additional actions for running, active/inactive etc.

* **Update Report Summary Display Page**  
Created a new more visual display for the Report Summary section. Shout out the request per seconds and failures at the top and group test stats vs. result stats using colored visual bars.

* **Add Total Bytes Sent and Total Bytes Posted to Summaries**  
Added Total bytes send and posted to the WebSurge summary result screens of both the GUI and Console apps.



### Version 1.14
<i><small>April 22rd, 2019</small></i>

* **Add New Session Button and Menu Item** 
Added options to more obviously create a new session, rather than just closing. Create New Session closes the old session and creates a new and highlights a new request entry.

* **Fix New Request Focus Issues**  
New requests now properly focus the new request tab and URL controls to avoid having to click or tab to the right field. Small thing but saves a few key strokes/mouse clicks.

* **Fix: Warmup Request Count Interference**  
Fix warmup request count that was interfering with partial seconds adding to the total request count. Simplified request code to separate warm up and proper test operation.


### Version 1.13
<i><small>March 13th, 2019</small></i>

* **Fix: Import WebSurge/Fiddler Session Files with LF only Formatting**  
Fixed session import which didn't work with files using only LF instead of CRLF for line feeds. Refactored session importer completely to be more resilient various format issues.

* **Fix: Splash Screen Startup Issues**  
Fix issue with the splash screen in some environments and especially on Remote instances where threading issues were causing WebSurge to quit on startup. Fixed by moving the splash screen inline and removing the fade out behavior.

* **Fix: Version Update Checks only for Minor Version Changes**  
WebSurge no longer considers a build change when prompting for new version updates. Updates now only show on shutdown when the minor (or major) version changes.

### Version 1.12
<i><small>March 7, 2019</small></i>

* **Add SortNoRandomize Option to Entry**  
You can now specify that a URL should not randomize when all requests are randomized. This allows you to randomize all requests **except** those that are marked so they always run at the beginning. Useful for ensuring that login URLs are always fired before other requests.

* **Add Paste Raw HTTP Request to Request Entry Form**  
You can now paste a raw HTTP request into an the Request entry form. You can now capture a raw request trace from tools like fiddler or a documentation form and simply paste it as is using a new link button on the request form. The request is automatically parsed and then displayed without having to separate out headers, verb and URL.


### Version 1.10
<i><small>July 6th, 2018</small></i>

* **Word Wrap Checkbox on Headers Field on Request Form**  
There's now a button on the Headers field to make it easier to view and edit header data. The default not wrapping to show 'raw' data which usually makes it easier to see the overall header content. Wrap mode makes it easier to view and edit long fields like cookies.

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
