# West Wind WebSurge ToDo List
This document describes various fixes, ideas for new features and various 
work items. 

### Priority List

* **Swagger Import Fixes**  
	* Handle Multiple Verbs for Same URL (iterate on Verb)
	* Parse POSt Parameters better (see other Petstore Swagger)

* **Review and Update Documentation**  
Documentation is for v1, so all screenshots need to be updated and behavior changes. It'll require a complete revamping of the docs.

* **User Handling**  
Per request user handling is currently disabled and needs to be hooked back up. Review and update logic and look into providing UI for configuring user data beyond the current Configuration file settings.

* **Check Content Compression Options with HttpClient**  
Need to verify that we can automatically decompress Gzip/deflate etc. content when retrieved and whether this works with HttpClient automatically.

* **Check out `Spectre.Console` for WebSurgeCli**  
We need better Console operation and result output generation and Spectre.Console looks like it might provide the features needed including: test progress, result display (table mode), layout and colors for results.


### Core Features

* **Save and Load From Gist (or other global store)**  
Allow saving or loading from Gist.

* **Support for User Sessions (works but needs review)**  
We need the ability to explicitly load a session with specific user credentials. This can currently be done with creating a custom plug-in but it'd be nice to have a generic way to configure a user 'session' based on ids. Not sure how to approach this as each implentation tends to be different.

### Miscellaneous Fixes required

* **Add per request username and password option**
In addition to the global username and password option we also should 
support username and password per request. This is already supported,
but there's no UI and no way to store this info in a request (use
custom headers like the Active flag).

### Features
  
* **Sync multiple WebSurge Instances to increase load**<br/>
Allow multiple instances of WebSurge on multiple machines run the same
test and synchronize their results. One machine acts as a master the
others as slaves controlled by master.

* **Get IIS Server Performance Stats**  
Already started collecting performance data and implementing IIS module
to retrieve perf data to client. Have to create background thread that
periodically picks up this performance data and either graphs or displays
the progress.

* **Export Urls to Common Test Formats**   
WCAT, CURL Batch file, Something Fiddler can import. Postman?

* **Import Urls from Other Formats**  
First and foremost a PostMan import might bring more users.





