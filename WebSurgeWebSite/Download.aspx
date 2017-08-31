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

        
        th {
             background: #535353;
             color: whitesmoke;
             text-align: center;
         }
        td a {
            font-weight: bold;
        }
        .price {
            text-align: right;
            font-weight: bold;
        }
        .centered {
            text-align: center;                
            background: #b4fae1;    
        }
        .centered .fa {
            color: #02b402; 

        }
        .centered.not-available 
        {
            background: white;
        }
        #FeatureTable td {
            min-width: 75px;
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
                                <a href="#Licensing">purchase one of our reasonably priced licenses</a>. Thanks for playing fair.                                
                                
                              
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
            
            
        <h3 id="Licensing">Licensing and Pricing of West Wind WebSurge</h3>
        <p>
            West Wind WebSurge is open source with source code <a href="https://github.com/rickstrahl/WestwindWebSurge">available on GitHub</a>, 
            but it is a licensed product.                                                 
            A <b>free</b> version is available that can be used without explicit registration 
            or usage limits for <b>personal use</b> and checking out of WebSurge's features.
        </p>
                
        <p>
            For use in a commercial or institutional environment, please purchase a Professional or 
            Organizational license. Each licensed user requires a separate license, but a 
            single user may use multiple copies of West Wind WebSurge on multiple machines, 
            given that only one copy at a time is in use. An organizational license is 
            available to allow any number of users running unlimited numbers of 
            West Wind WebSurge within a single organization. Any purchased license is 
            valid for the duration of the major release that it was purchased for (ie. 1.00-1.99). 
        </p>
            
        <p>
            To allow you to check out WebSurge completely, we provide a fully functional, 
            unlimited version of West Wind WebSurge for download, so we rely on the 
            honor system from users for supporting this product.
        </p>
        <p>
            <b>Thanks for playing fair!</b>
        </p>
        <%--            <p>
                For more licensing information please check out the 
                <a href="http://websurge.west-wind.com/docs/?page=_433179ec8.htm">
                    West Wind WebSurge Licensing
                </a> topic in the documentation.                
            </p>--%>
                                 
        <table class="table table-bordered table-striped" style="width: 80%">
            <thead>
            <tr>
                <th>Product</th>
                <th>Price</th>
            </tr>
            </thead>
            <tr>
                <td>
                    <a href="download.aspx">WebSurge Free</a><br/>
                    <small>(for personal use or checking out WebSurge features)</small>
                </td>
                <td class="price">
                    <span style="font-weight: bold; color: maroon">FREE</span>
                </td>
            </tr>
            <tr>
                <td>
                    <a href="http://store.west-wind.com/product/websurge" class="pull-right btn btn-sm btn-primary" style="width: 70px;">
                        <i class="fa fa-credit-card"></i>
                        Buy
                    </a>
                    <a href="http://store.west-wind.com/product/websurge">WebSurge Professional</a><br/>
                    <small>(required for commercial use, suggested for continued personal use)</small>
                        
                </td>
                <td class="price">
                    $199.00
                </td>
            </tr>
            <tr>
                <td>
                    <a href="http://store.west-wind.com/product/websurge" class="pull-right btn btn-sm btn-primary" style="width: 70px;">
                        <i class="fa fa-credit-card"></i>
                        Buy
                    </a>

                    <a href="http://store.west-wind.com/product/websurge_org">WebSurge Organizational License</a><br/>
                    <small>(unlimited users for a single organization)</small>
                </td>
                <td class="price">
                    $899.00
                </td>
            </tr>
        </table>
            
        <p>
            Product purchases can be made through our secure online store. For more information on other order arrangements please visit our order policies page.

            <ul>
                <li><a href="http://store.west-wind.com/products?search=Web+Surge">Purchase in our online Store</a></li>
            </ul>
    
        </p>
            
        <h3>Contribute - Get a Free License</h3>
        <p>
            Contributors that provide valuable feedback with quality bug reports or enhancement requests,
            or help out with code via Pull Requests, or support WebSurge in a significant way are
            all eligible for a free license.
        </p>
        <p>
            Microsoft employees and Microsoft MVPs as well as employees of any company offering free tools to
            the Micorosoft MVP or Insiders programs also qualify for a free license.
        </p>
        <p>
            Contact Rick for more info or - just as likely - I'll be contacting you.
        </p>

        <h3>Feature Matrix</h3>
        <a name="FeatureMatrix"></a>
        <p>
            Not sure which version to get? Here's a feature matrix that can help you decide.
        </p>
        <table id="FeatureTable" class="table table-bordered" style="width: 80%">
            <thead>
            <tr>
                <th>Feature</th>
                <th>Free</th>
                <th>Professional</th>
                <th>Org</th>
            </tr>
            </thead>
            <tbody>
            <tr>
                <td>
                    Max number of 
                    URLs in session
                </td>
                <td class="centered">
                    unlimited   
                </td>
                <td class="centered">
                    unlimited
                </td>
                <td class="centered">unlimited
                </td>
            </tr>
            <tr>
                <td>
                    Max number of 
                    simultaneous sessions
                </td>
                <td class="centered">
                    unlimited   
                </td>
                <td class="centered">
                    unlimited
                </td>
                <td class="centered">unlimited
                </td>
            </tr>
                       
            <tr>
                <td>
                    Built-in HTTP Capture Tool</td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
            </tr>
            <tr>
                <td>
                    Capture any Windows HTTP traffic from Web Browsers or Apps</td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
            </tr>
            <tr>
                <td>
                    Filter captures by domain, process or user defined exclusions</td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
            </tr>
            <tr>
                <td>
                    Support for advanced HTTP features and SSL</td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>    
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
            </tr>
            <tr>
                <td>
                    Support for <a href="http://telerik.com/fiddler">Telerik&#39;s Fiddler Exports</a>
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>    
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
            </tr>
            <tr>
                <td>
                    Export results to JSON, XML, HTML
                    and raw HTTP Headers
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>    
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
            </tr>
            <tr>
                <td>
                    Test locally within your Firewall or VPN
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>    
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
            </tr>
            <tr>
                <td>
                    Multiple Users within an Organization
                </td>
                <td class="centered not-available">
                    &nbsp;
                </td>
                <td class="centered not-available">
                    &nbsp;
                </td>
                <td class="centered">
                    <i class="fa fa-check fa-2x"></i>  
                </td>
            </tr>
            </tbody>
        </table>
            


        </div>
            
            

            
            

        <div class="clearfix"></div>            


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