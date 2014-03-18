#West Wind Simple Stress Tester
**A simple stress testing utility using Fiddler Session Capture Files**

This small Windows application allows you to select a Fiddler Session file.
You can also create these Session files with plain text, using raw HTTP
requests along with a standard request separator. However, using Fiddler 
is the easiest way to capture requests.

You can pick up Fiddler from:
[Download Fiddler](http://www.telerik.com/download/fiddler) 

Here's what the tool looks like:

![Fiddler Stress Tester UI](https://bytebucket.org/rstrahl/west-wind-simple-stress-tester/raw/93d2006bfff9b5dd8f8aff65a77c78c89f5995d9/SimpleStressTester.png?token=6fc228988ed4d15e5d892f62f33983dab42d0191)

To use Fiddler for capturing Sessions for this tool:

* Open Fiddler
* Open your browsers and navigate through a Session
* Go back into Fiddler and stop capturing requests
* In Fiddler's List window select all requests you want to replay
* Right click | Save | Selected Session | As Text
* Save the file to disk

Then use the Simple Stress Tester to run a load test:

* Open the Fiddler Stress Tester tool
* Click Open and select the captured Session file you saved
* Set the Number of Threads to run to set the load for your tests
* Set the time to set to the number of seconds to run
* Optionally use the options on the top right to modify the test behavior
* Click the Start button to start the test

The test then runs through your captured requests by running as many 
thread as you specified simultaneously for each of the sessions you captured.
Each thread iterates through the full session and then starts over for the
specified amount of time.

The results are summarized at the top of the output window. 

The results view on the right lets you see each of the requests that was
sent to the server up to the first 1000 requests for each success and 
error requests. You can select an item and a preview pane displays the
request detail for that request, so you can review headers and content bodies
for both requests and responses. 

Note that compressed content is automatically decompressed and may not match
in length to the content length returned in the headers.


##Requirements
[Requires the .NET Framework 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=40779)

