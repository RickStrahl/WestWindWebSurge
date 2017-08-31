<%@ Page Language="C#" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Import Namespace="System.Net" %>

<!DOCTYPE html>
<html>
<head>
    <title>West Wind WebSurge Load Tester</title>    
    <meta name="keywords" content="Load,Testing,Web,Server,ASP.NET,Capture,Playback,Threads" />
    <meta name="description" content="West Wind Web Surge - Easy Windows based load and URL testing for Web sites and services." />

    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Css/application.min.css" rel="stylesheet" />
    <link href="//netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.css" rel="stylesheet" />
    
    <link rel="shortcut icon" href="favicon.ico" />
    <link rel="icon" type="image/png" href="favicon.png" />
    <meta content="images/websurgelogo128.png" itemprop="image">
</head>
<body>
    <div class="banner">
        <span class="banner-title" style="cursor: pointer;" onclick="window.location = './';" >
            <img src="Images/websurgeicon.png" 
                style="height: 28px;" 
                alt="West Wind Globalization" /> 
            <span>West Wind WebSurge</span>
        </span>        
        <div id="TopMenu" class="right">
            <a href="./" class="active">Home</a>            
            <a href="download.aspx">Download</a>                                    
            <a href="https://youtu.be/O5J8mDfVZH8">Video</a>
            <a href="features.aspx">Features</a>            
        </div>            
    </div>


    <div id="MainContainer" class="background">
        <div id="ContentContainer" class="content">
        

            <header style="background-color: #535353; color: whitesmoke;padding: 8px;">
                <div class="row">
                    <div class="col-sm-5">
                        <img id="HeroImage" src="images/WebSurgeLogo.png" alt="WebSurge"/>
                    </div>
                    <div class="col-sm-7">
                        <h1 style="font-weight: bold">West Wind WebSurge</h1>
                        <h4 style="color: #ffd281">Web Application Load Testing made easy</h4>
                        <div style="padding: 5px 20px;font-size: 1.3em">
                            <div><i class="fa fa-check" style="color: lightgreen"></i> Capture HTTP Requests</div>    
                            <div><i class="fa fa-check" style="color: lightgreen"></i> Test HTTP Requests</div>                            
                            <div><i class="fa fa-check" style="color: lightgreen"></i> Play them back under Load</div>
                            <div><i class="fa fa-check" style="color: lightgreen"></i> Summarize Results</div>
                            <div><i class="fa fa-check" style="color: lightgreen"></i> Done!</div>                           
                        </div>
                         <div style="margin-top: 10px;">
                                <a href="download.aspx">
                                    <img src="/images/download.gif" class="boxshadow roundbox" alt="download WebSurge"
                                        style="height: 32px">
                                </a>
                                <div style="display: inline-block; background: #ffffcc; padding: 1px; border: 1px solid silver;
                                    border-radius: 4px;"
                                    title="Install with the Chocolatey NuGet Package installer">
                                    <a href="http://chocolatey.org/packages/WestwindWebSurge">
                                        <img src="images/chocolatey.png" style="height: 27px;" alt="Chocolatey" />
                                    </a>
                                </div>
                                <div style="display: inline-block; background: #ffffcc; padding: 2px 10px; border: 1px solid silver;
                                    border-radius: 4px;"
                                    title="Open Source Code on GitHub">
                                    <a href="http://github.com/rickstrahl/WestwindWebSurge">                                        
                                        <img src="Images/Octocat.png" style="height: 24px" />
                                        <img src="Images/github.png" style="height: 24px"/>
                                    </a>
                                </div>
                                <br />
                                <small style="font-size: 8pt;"><i>version <%= Version %> - <%= ReleaseDate%></i></small>


                                
                            </div>
                    </div>
                </div>                
            </header>
            
         
            

            
               
       

               

            <div class="content" style="padding: 10px 40px 0px;">
                 <div class="right" style="margin-right: 15px; margin-top: -5px; margin-right: -20px;">
                                     <div style="font-size: xx-small">created by:</div>
                                     <a href="http://west-wind.com">
                                         <img src="/Images/wwToolbarLogo.png" style="float: right; height: 30px" alt="West Wind Technologies" />
                                     </a>
                                 </div>


                <h2>Load Testing Web Sites shouldn't be difficult</h2>
                <p>
                    We believe that testing HTTP requests and load testing a site should be easy. It's something that
                    should take a few minutes to set up and then run on a regular basis during the development process,
                    so that you can monitor performance of your sites while you are building them.
                </p>
                <p>
                    We built West Wind WebSurge with developers and testers in mind to make it easy to create 
                    HTTP requests or entire sessions, and then easily play back either individual URLs
                    for API or response testing, or for full-on stress testing under heavy load. 
                    It's easy to share sessions that are plain text files and can be stored on disk 
                    with projects, shared folders or cloud drive storage or via source control for all 
                    users access.
                </p>
                    
                         
                <div class="content" style="padding: 0 0 20px">
                          <%-- <h2>Ready to get started?</h2>
                <p>
                    Here are a few screen shots and a video to give you an idea what to expect when 
                    you run WebSurge. The user interface is
                    simple and easy to use, and that's the way it should be
                    to make it quick and easy to test your Web sites.
                </p>--%>
                     <div id="Carousel" class="carousel slide" data-ride="carousel">
                      <!-- Indicators -->
                      <ol class="carousel-indicators">
                        <li data-target="#Carousel" data-slide-to="0" class="active"></li>
                        <li data-target="#Carousel" data-slide-to="1"></li>
                        <li data-target="#Carousel" data-slide-to="2"></li>
                        <li data-target="#Carousel" data-slide-to="3"></li>
                        <li data-target="#Carousel" data-slide-to="4"></li>
                        <li data-target="#Carousel" data-slide-to="5"></li>
                        <li data-target="#Carousel" data-slide-to="5"></li>
                      </ol> 

                      <!-- Wrapper for slides -->
                      <div class="carousel-inner">
                        <div class="item active">
                          <iframe width="860" height="575" 
                              src="//www.youtube.com/embed/O5J8mDfVZH8?rel=0" 
                              frameborder="0"></iframe>
                          <div class="carousel-caption">
                          </div>
                        </div>
           
                       <div class="item">
                          <img src="Images/WebSurge_RequestDisplay.png"  />
                          <div class="carousel-caption">
                          </div>
                        </div>
                        <div class="item">
                          <img src="Images/WebSurge_RequestsView.png"  />
                          <div class="carousel-caption">
                          </div>
                        </div>
                        <div class="item">
                          <img src="Images/WebSurge_ResultsView.png"  />
                          <div class="carousel-caption">                             
                          </div>
                        </div>
                        <div class="item">
                          <img src="Images/WebSurge_Capture.png"  />
                          <div class="carousel-caption">                            
                          </div>
                        </div>
                        <div class="item">                                                    
                          <img src="Images/WebSurge_Charts.png"  />                          
                          <div class="carousel-caption">                            
                          </div>
                        </div>  
                        <div class="item">                                                    
                          <img src="Images/console.png"  />                          
                          <div class="carousel-caption">
                              Command Line Support
                          </div>
                        </div>  
                      </div>

                      <!-- Controls -->
                      <a class="left carousel-control" href="#Carousel" data-slide="prev">
                        <span class="glyphicon glyphicon-chevron-left"></span>
                      </a>
                      <a class="right carousel-control" href="#Carousel" data-slide="next">
                        <span class="glyphicon glyphicon-chevron-right"></span>
                      </a>
                    </div>
            </div>

                <article class="content" style="padding: 10px 0;">
                <div class="row">
                    <div class="col-sm-6">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h5 class="panel-title">Features</h5>
                            </div>
                            <div id="FeatureList" class="panel-body">
                                <div><i class="fa fa-check-circle"></i> Easy to capture, easy to run tests</div>                               
                                <div><i class="fa fa-check-circle"></i> Built-in capture tool to capture requests</div>
                                <div><i class="fa fa-check-circle"></i> Capture Web browsers or Windows apps</div>                               
                                <div><i class="fa fa-check-circle"></i> Filter captures by domain or process Id</div>
                                <div><i class="fa fa-check-circle"></i> Import sessions from Fiddler</div>
                                <div><i class="fa fa-check-circle"></i> Manually create and edit sessions</div>
                                <div><i class="fa fa-check-circle"></i> Complete HTTP and SSL Support</div>   
                                <div><i class="fa fa-check-circle"></i> Automatic Cookie Tracking</div>
                                <div><i class="fa fa-check-circle"></i> Test HTML, AJAX, REST and SOAP Services</div>    
                                <div><i class="fa fa-check-circle"></i> Test individual URLs</div>                                                                                                
                                <div><i class="fa fa-check-circle"></i> Test with unlimited load</div>                                                                
                                <div><i class="fa fa-check-circle"></i> Easy to read summarized results</div>   
                                <div><i class="fa fa-check-circle"></i> Charts to visualize results</div>                              
                                <div><i class="fa fa-check-circle"></i> Export results to Xml,Json,Html</div>                                                                
                                <div><i class="fa fa-check-circle"></i> Test locally and within your Firewall/VPN</div>
                                <div><i class="fa fa-check-circle"></i> Command Line Interface</div>            
                                <div><i class="fa fa-check-circle"></i> Great for HTTP Request testing and managing</div>            
                                <div><i class="fa fa-arrow-circle-o-right"></i> <a href="features.aspx">more features...</a></div>
                            </div>
                        </div>
                    </div>

                    <div class="col-sm-6">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h5 class="panel-title">Get it</h5>
                            </div>
                            <div class="panel-body">
                                <p>
                                    You can simply download and install WebSurge from
                                    our Web site. Unzip the distribution file and run
                                    the installer.
                                </p>
                                <div style="margin: 10px;">
                                    <a href="download.aspx" style="display: block;margin-bottom: 15px;">
                                        <img src="/images/download.gif" class="boxshadow roundbox">
                                    </a>                                    
                                </div>
                                
                                <p>                                
                                   Alternately you can also install West Wind WebSurge 
                                    using the Chocolatey NuGet installer:
                                </p>
                                <div style="margin: 10px;">                                    
                                    <a href="http://chocolatey.org/packages/WestwindWebSurge" style="display:block" >
                                        <img src="images/chocolatey.png" style="width: 170px;" alt="Chocolatey" />                                        
                                    </a>  
                                    
                                    <pre style="font-size: 10pt; font-family: Consolas, monospace;color: whitesmoke;background: #535353">c:\> choco install WestwindWebSurge</pre>                                                          
                                </div>    
                                
                                <p><strong>West Wind WebSurge is open source</strong>, but licensed software. Source code is available on GitHub. </p>

                                <div style="margin: 10px 10px;">                                    
                                        <a href="http://github.com/rickstrahl/WestwindWebSurge" style="display:block" >
                                        <img src="images/octocat.png" style="height: 27px;" alt="GitHub" />
                                        <img src="images/github.png" style="width: 110px;padding: 3px 10px 3px 2px;" 
                                             alt="GitHub" />
                                    </a>                                      
                                </div>    
                                                                                       
                                <b>
                                    Requirements
                                </b>        
                                <p>
                                    <ul>
                                        <li>Windows Vista or newer, 2008 or newer</li>
                                        <li>.NET Framework 4.5</li>
                                    </ul>
                                </p>
                            </div>
                        </div>
                    </div>

                

                
               <%-- <p>
                    WebSurge 
                    is a Windows application that makes it drop dead easy to enter or capture HTTP requests, and 
                    then test them interactively either individually for request inspection and 
                    development testing, or play them back under heavy load. 
                    The process is quick and easy and you can be up and running in minutes.</p>
                <p>
                    Capture HTTP requests using our built-in HTTP capture tool 
                    that can capture HTTP requests from browser or live running applications on your 
                    system. Or you can&nbsp; manually create sessions using an easy to use request UI to enter URLs and
                    headers, or just use plain text files with raw HTTP headers using a format 
                    compatible with exports from Telerik's popular 
                    <a href="http://telerik.com/fiddler">Fiddler</a> tool.
                    You can test individual URLs quickly through the UI or fire up a load test 
                    with a large number of simultaneous sessions to stress your Web Site. Sessions 
                    are saved as plain request files and can be easily reloaded, shared, stored with 
                    relevant applications and checked into source control alongside applications.</p>
                <p>
                    WebSurge provides full access to the HTTP protocol - you can capture and play 
                    back plain HTML requests, AJAX requests, REST or SOAP Services, SSL - even API 
                    requests from your Windows applications. And unlike online testing services 
                    you can run WebSurge locally on one or more machines inside of 
                    your Firewall or VPN. </p>
                <p>
                    Once a session has been captured, specify the number of threads
                    and length of time to run and go! While
                    running, an ongoing tally of requests and stats is provided. 
                    When complete, results are summarized for you in
                    a simple, easy to understand report. You can also review a few
                    charts and export the captured results for later analysis.
                </p>
                <p>
                    It's that easy! You can be up and running in minutes and get useful results
                    as soon as your test run is complete. Because the capture and test process 
                    is so straight forward, it promotes frequent testing of your Web applications 
                    for better performance, stability and reliability.
                </p>              
                   --%>                              
            </div>                
            


            </article>
        </div>
        <div class="clearfix"></div>
    </div>
    </div>

    <nav class="banner" style="font-size: 8pt;padding: 10px; height: 80px; border-top: solid black 4px;border-bottom: none;">
        <div class="right">
            created by:<br/>
            <a href="http://west-wind.com/" style="padding: 0;">
                <img src="/Images/wwToolbarLogo.png" style="width: 150px;" />
            </a>
        </div>
        &copy; West Wind Technologies, 2000-<%= DateTime.Now.Year %>
    </nav>
    
    <script src="//code.jquery.com/jquery-1.11.0.min.js"></script>
    <script src="Css/js/bootstrap.min.js"></script>
    <script src="Css/js/touchswipe.js"></script>  
    <script>
        $(document).ready(function() {

            $("#Carousel").carousel({
                interval: false
            });
           
            //Enable swiping...
            $(".carousel-inner").swipe({
                //Generic swipe handler for all directions
                swipeRight: function (event, direction, distance, duration, fingerCount) {
                    $(this).parent().carousel('prev');
                },
                swipeLeft: function () {
                    $(this).parent().carousel('next');
                },
                //Default is 75px, set to 0 for demo so any distance triggers swipe
                threshold: 0
            });
        });
    </script> 

    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-9492219-10', 'auto');
        ga('send', 'pageview');

    </script>
</body>

</html>
<script runat="server">
    // This code dynamically updates version and date info
    // whenver the application is restarted
    static string Version
    {
        get
        {
            
            if (_version != null)
                return _version;

            // set default dates for fallback here
            _version = "0.90";
            ReleaseDate = "July 7th, 2015";

            try
            {
                WebClient client = new WebClient();
                string xml = client.DownloadString("http://west-wind.com/files/websurge_version.xml");
                Regex regex = new Regex(@"<Version>(.*)<\/Version>");
                MatchCollection matches = regex.Matches(xml);
                if (matches != null && matches.Count > 0)
                {
                    _version = matches[0].Value;
                }

                regex = new Regex(@"<ReleaseDate>(.*)<\/ReleaseDate>");
                matches = regex.Matches(xml);
                if (matches != null && matches.Count > 0)
                {
                    ReleaseDate = matches[0].Value;
                }                
            }
            catch(Exception ex)
            {
                HttpContext.Current.Response.Write(ex.Message);
            }

            return _version;
        }
    }
    private static string _version;
    public static string ReleaseDate;
</script>
