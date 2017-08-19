<%@ Page Language="C#" %>
<%@ Import Namespace="System.Net" %>

<%
    //WestWindSiteUtils.LogInfo("/WebMonitor/default.aspx");	
%>
<!DOCTYPE html>
<html>
<head>
    <title>Download - West Wind WebSurge</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="description" content="Download West Wind Web Surge for quick and easy URL and Load Testing of Web Applications on Windows. Capture URL request sessions, and play them back under load." />
    <meta name="keywords" content="Load,Testing,Web,Server,ASP.NET,Capture,Playback,Threads" />

    <link href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Css/application.min.css" rel="stylesheet" />
    <link href="//netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.css" rel="stylesheet" />
    
    <style>
        dt { float: left;width: 90px; font-weight: normal}
        dd { font-size: 100% }
        .btn-info {
            background-color: #4d94d0;            
        }
        .panel {
            min-height: 110px;
        }
    </style>
</head>
<body>
    <div class="banner">        
          
         <div class="banner-title" style="cursor: pointer;" onclick="window.location = './';" >
            <img src="Images/websurgeicon.png" 
                style="height: 28px;"
                alt="West Wind Globalization" /> 
            <span>West Wind WebSurge</span>
        </div>        
        <div id="TopMenu" class="right">
            <a href="./" >Home</a>            
            <a href="features.aspx" >Features</a>    
            <a  class="active">Download</a>                                    
            <a href="pricing.aspx" >License</a>
        </div>       
    </div>
    

    <div id="MainContainer" class="background">
        <div id="ContentContainer" class="content" style="padding: 10px 45px;">
            
            <h2>Download West Wind WebSurge</h2>
            
            <p>
                You can download the free version of West Wind WebSurge. Capture and test individual URLs or create sessions
                for full on load testing. The free version lets you get started easily and 
                quickly. Get testing!
            </p>

            <div class="well well-lg">
                <div class="row">
                    <div class=" col-sm-5">
                        <img src="Images/WebSurgeLogo.png"  style="width: 195px; margin-bottom: 4px; display: block"/>
                        
                        <p><b>West Wind Web Surge</b> &nbsp; <i class="fa fa-windows" style="color: steelblue"></i></p>
                        <dl>                            
                            <dt>Version:</dt>
                            <dd>v<%= Version %></dd>
                                
                            <dt>Released:</dt>
                            <dd><%= ReleaseDate %></dd>

                            <dt>File size:</dt>
                            <dd>2.3 mb</dd>
                       </dl> 
                        
                        
                         
                    </div>
                    <div class=" col-sm-7">
                        <a class="btn btn-lg btn-info" href="http://west-wind.com/files/WebSurgeSetup.exe">
                            <i class="fa fa-download fa-" style="font-size: 150%;"></i> &nbsp; 
                            Download Web Surge now
                        </a>
                        <div class="small" style="margin-top: 10px">
                            Download <a href="http://west-wind.com/files/WebSurgeSetup.zip">Zip file</a>
                        </div>
                        <div style="margin-top: 15px;">                            
                            <div class="fa fa-info-circle" style="font-size: 260%; color: #535353; float: left;"></div>
                            <div style="margin-left: 50px;">
                                
                            <small>
                                
                                West Wind WebSurge is free for personal use, for use on open source projects and for checking out features. This
                                download provides a fully functional, non-limited version that includes all features. For 
                                commercial or institutional use, please 
                                <a href="pricing.aspx">purchase one of our reasonably priced licenses</a>. Thanks for playing fair.                                
                                
                              
                            </small>
                            </div>
                        </div>
                        
                        <a href="http://chocolatey.org/packages/WestwindWebSurge" style="display:block" >
                                        <img src="images/chocolatey.png" style="width: 170px; margin-top: 10px;" alt="Chocolatey" />                                        
                                    </a>  
                          <p>
                              You can also install directly from Chocolatey's package store:
<%--                                    If you have <a href="https://chocolatey.org/">Chocolatey</a> installed you can also install West Wind WebSurge 
                                    directly from the <a href="https://chocolatey.org/packages/WestwindWebSurge">package repository</a>:--%>
                                    
                                    <pre style="font-size: 10pt; font-family: Consolas, monospace;color: whitesmoke;background: #535353">c:\> choco install WestwindWebSurge</pre>                                                          
                                </p>
                    </div>
                </div>                
            </div>
            
            
            <div class="row">
                <div class="col-sm-6">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h5 class="panel-title">System Requirements</h5>
                        </div>
                        <div id="FeatureList" class="panel-body">
                            <div><i class="fa fa-check-circle"></i>Microsoft Windows Vista or 2008 and newer</div>
                            <div><i class="fa fa-check-circle"></i>32 or 64 bit</div>
                            <div><i class="fa fa-check-circle"></i>Microsoft .NET 4.5 Runtime (<a href="http://smallestdotnet.com/">check</a>)</div>
                        </div>
                    </div>
                </div>
                
                <div class="col-sm-6">
                    <div class="panel panel-default" >
                        <div class="panel-heading">
                            <h5 class="panel-title">Instructions</h5>
                        </div>

                        <div id="FeatureList" class="panel-body">
                            <div><i class="fa fa-check-circle"></i>Download the zip file</div>
                            <div><i class="fa fa-check-circle"></i>Run the contained WebSurgeSetup.exe</div>
                            <div><i class="fa fa-check-circle"></i>Follow the installation instructions</div>
                        </div>
                    </div>
                </div>
        </div>
        <div class="clearfix"></div>            
    </div>
        <div style="height: 50px;"></div>
   </div>

    <nav class="banner" style="font-size: 8pt; padding: 10px; height: 80px; border-top: solid black 4px;
                                                                                                                                                                                                                                                                                                     border-bottom: none;">
        <div class="right">
            created by:<br />
            <a href="http://west-wind.com/" style="padding: 0;">
                <img src="/Images/wwToolbarLogo.png" style="width: 150px;" />
            </a>
        </div>
        &copy; West Wind Technologies, 2000-<%= DateTime.Now.Year %>
    </nav> 
    
    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-9492219-10', 'west-wind.com');
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
            _version = "0.79";
            ReleaseDate = "April 4th, 2015";

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
            catch
            {
            }

            return _version;
        }
    }
    private static string _version;
    public static string ReleaseDate;
</script>