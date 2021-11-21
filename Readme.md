# West Wind WebSurge
[![Download WebSurge](https://img.shields.io/badge/WebSurge-Installer-blue.svg)](https://websurge.west-wind.com/download.aspx)
[![Download WebSurge from Chocolatey](https://img.shields.io/chocolatey/dt/westwindwebsurge.svg)](https://chocolatey.org/packages/WestwindWebSurge)
[![Web Site](https://img.shields.io/badge/WebSurge-WebSite-blue.svg)](https://websurge.west-wind.com)

#### Http Request and Load Testing for Windows

<img src="WebSurgeLogo.png" height="240"  /> 

Like what you see? Don't forget to :star: Star the repo (top right).

### What is WebSurge?
West Wind WebSurge is an Http Request and Load Testing tool for Windows.

WebSurge makes it easy to create or capture and then play back Http requests either individually for interactive request testing and debugging, or for Http Load Tests that can play back a session of requests under heavy load.

The goal of the tool is to make it quick and drop dead simple to create and set up requests, and then test and load test them easily, so that you can integrate the functionality without friction into your development process and you can test applications often.

WebSurge also is a more interactive alternative to Postman for Http request testing.

> Note this repository no longer holds source code for West Wind WebSurge, due to rampant license abuse. Version 2.0 has moved to a private repository. If you still would like to contribute to this project you can request private access.
>  
> This repository's primary purpose is to provide a feedback mechanism via Issues.

### Screenshots
Here are a some screen shots that demonstrate the core features of WebSurge:

**Request View**  
The request view shows the raw Http request with headers and content. You can select, manage and run requests from this view. Most of the headers and request content of existing requests is also editable from here.

![](Assets/ScreenShots/RequestScreen.png)

**Edit Request View**  
To create new requests or edit more detailed editing of requests, the edit view is used:

![](Assets/ScreenShots/EditRequest.png)

**Running a Load Test**  
To run a load test you press **Start**, set the Threads and Duration and off you go. Requests run in the background with a running tally showing in the HUD display on the bottom. You can continue to use the UI while tests are running.

![](Assets/ScreenShots/ActiveLoadSession.png)

**Test Result View**  
The test result view shows a summary of the test that was just run. It'll provide request per second, failed requests, total requests, duration and a few other top level statistics. It also shows similiar information for each individual Url of the session. 

![](Assets/ScreenShots/SessionResults.png)

**Command Line Interface**  
The load testing interface is also available via a Command Line tool (`websurgeCli`) that allows you to run load tests from the Windows Terminal and capture test results via Console output.

![](Assets/ScreenShots/WebSurgeCli.png)


#### Goal of WebSurge
The goal of this tool is to make it drop dead easy to capture or create HTTP request content and test and play it back easily under load. It provides simple results that are easy to understand and act on. It's so easy that you can *frequently* stress test Web applications for load  characteristics during development, testing and in production.

* Easy capture or manual entry of URLs
* Interactive recording of screen sessions
* Quickly access and run your test
* Test and preview individual URLs 
* Immediate process feedback

#### Interactive Http Request Testing
WebSurge is great for load testing, but it's also an excellent URL test tool for developers who need to test API, SOAP or any other type of HTTP application service. 

It makes for a compelling alternative to the popular Postman tool and you can import sessions from Postman as well as from Fiddler traces.

You can easily capture sessions and easily recall individual URLs for testing of endpoints. The formatted output views make it easy to visualize the resulting contact with syntax highlighted markup/text as well as a preview for HTML content. We've optimized the UI flow to make request access and request execution and review as easy as possible. 

Sessions are stored in plain files and can easily be shared in source control, the file system, or via cloud storage solutions like DropBox or OneDrive. And because it's just text it's easy to modify Sessions in raw form or generate them via code.

#### Use Cases

* Load testing Web applications and services
* Easy Http Request testing and debugging for REST and Data services
* URL management for saving, sharing, restoring

### Requirements

* Windows 7+, Windows 2012+
* .NET 6.0
* Microsoft WebView2 


### More info

* [WebSurge Web Sitee](https://websurge.west-wind.com/)
* [WebSurge Download](https://websurge.west-wind.com/download)
* [WebSurge Chocolatey Package](https://chocolatey.org/packages/WestwindWebSurge)
* [Licensing](http://west-wind.com/websurge/purchase#license)

### Developer Info

* [Change Log](ChangeLog.md)
* [To Do List](ToDo.md)
