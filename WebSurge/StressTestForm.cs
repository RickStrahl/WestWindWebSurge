using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSurge.Core;
using Westwind.Utilities;
using Timer = System.Threading.Timer;

namespace WebSurge
{
    public partial class StressTestForm : Form
    {
        StressTester StressTester { get; set; }

        HttpRequestData ActiveRequest { get; set;  }

        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value; 
                if (lblStatusFilename != null)
                    lblStatusFilename.Text = "Session File: " + value;         
                
                // update caption
                if (string.IsNullOrEmpty(_FileName))
                    Text = Path.GetFileName(_FileName);
                else
                    Text = null;

            }
        }
        private string _FileName;

        public override string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                string text;


                if (UnlockKey.Unlocked)
                    text = "West Wind WebSurge (Professional)";
                else
                    text = "West Wind Web Surge (Free Version)";

          
                if (!string.IsNullOrEmpty(value) && 
                    !value.Contains("Professional") && 
                    !value.Contains("Free Version"))
                    text += " - " + value;

                base.Text = text;
            }
        }

        private List<HttpRequestData> Requests
        {
            get
            {
                if (_Requests == null)
                    _Requests = new List<HttpRequestData>();

                return _Requests;
            }
            set { _Requests = value; }
        }
        List<HttpRequestData> _Requests;


   


        FileSystemWatcher Watcher { get; set; }
        public Splash Splash { get; set; }

        public StressTestForm(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
                FileName = fileName;

            InitializeComponent();            
        }

        public void OpenFile(string fileName = null)
        {
            if (fileName == null)
                fileName = FileName;
            else
                FileName = fileName;

            Requests.Clear();

            Requests = StressTester.ParseSessionFile(FileName);
            if (Requests == null)
                Requests = new List<HttpRequestData>();            

            RenderRequests(Requests);
           
            App.Configuration.StressTester = StressTester.Options;
            OptionsPropertyGrid.SelectedObject = App.Configuration.StressTester;
            tbtxtThreads.Text = StressTester.Options.LastThreads.ToString();
            tbtxtTimeToRun.Text = StressTester.Options.LastSecondsToRun.ToString();
            
            App.Configuration.LastFileName = FileName;

            Text = Path.GetFileName(FileName);

            AttachWatcher(fileName);
            UpdateButtonStatus();
        }

        #region load and unload

        private void StressTestForm_Load(object sender, EventArgs e)
        {
            Watcher = new FileSystemWatcher();            

            // resize the window with configured values
            App.Configuration.WindowSettings.Load(this);            

            StressTester = new StressTester();
            StressTester.RequestProcessed += StressTester_RequestProcessed;
            StressTester.Progress += StressTester_Progress;

            if (string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(App.Configuration.LastFileName))
            {
                FileName = Path.GetFullPath(App.Configuration.LastFileName);
                if (!File.Exists(FileName))
                    FileName = null;
            }

            if (!string.IsNullOrEmpty(FileName))
                OpenFile(FileName);
            else
                Requests = new List<HttpRequestData>();

            LoadOptions();

            OptionsPropertyGrid.SelectedObject = StressTester.Options;

            cmbListDisplayMode.SelectedItem = "Errors";
            TabsResult.SelectedTab = tabOptions;

            AddRecentFiles();

            if (!UnlockKey.IsRegistered())
            {           
                var t = new System.Windows.Forms.Timer();
                t.Interval = 15*60*1000; // 15 mins
                t.Tick += t_Tick;
                t.Start();
            }

            UpdateButtonStatus();
        }

        private void CloseSession()
        {
            Requests = new List<HttpRequestData>();
            FileName = null;
            RenderRequests(Requests);
            TabSessions.SelectedTab = tabSession;

            string oldTheme = StressTester.Options.FormattedPreviewTheme;

            // clear out options so we don't 'inherit' weird
            // settings from the last session
            StressTester.Options = new StressTesterConfiguration()
            {
                FormattedPreviewTheme = oldTheme
            };
            OptionsPropertyGrid.SelectedObject = StressTester.Options;

            StressTester.Results = new List<HttpRequestData>();
            StressTester.InteractiveSessionCookieContainer = null;
                  
            // render an empty list
            RenderResultList(StressTester.Results);

            var request = new HttpRequestData();
            LoadRequest(request);

            // open the options tab
            TabsResult.SelectedTab = tabRequest;

            try
            {
                // force to clear memory
                GC.Collect();
            }
            catch
            {
            }
        }

        void ShowStatus_Internal(string text, int panelId = 1)
        {
            if (panelId == 1)
                lblStatusText.Text = text;
            else if (panelId == 2)
                lblStatusFilename.Text = text;
            else if (panelId == 3)
                txtProcessingTime.Text = text;
            Application.DoEvents();
        }

        void SaveOptions()
        {            
            App.Configuration.LastFileName = FileName;
            App.Configuration.WindowSettings.Save(this);            

            // Save any changed configuration settings
            App.Configuration.Write();
        }

        void LoadOptions()
        {
            // Window settings were loaded on form load
            StressTester.Options = App.Configuration.StressTester;            

            // manually assign threads and time
            tbtxtTimeToRun.Text = StressTester.Options.LastSecondsToRun.ToString();
            tbtxtThreads.Text = StressTester.Options.LastThreads.ToString();            
        }

        private void StressTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Hide();
            Application.DoEvents();

            App.Configuration.WindowSettings.Accesses++;
            if (!UnlockKey.Unlocked)
            {
                var displayCount = 5;
                if (App.Configuration.WindowSettings.Accesses > 250)
                    displayCount = 1;
                else if (App.Configuration.WindowSettings.Accesses > 100)
                    displayCount = 2;
                else if (App.Configuration.WindowSettings.Accesses > 50)
                    displayCount = 3;

                if (App.Configuration.WindowSettings.Accesses % displayCount == 0)
                {
                    var form = new RegisterDialog();
                    form.ShowDialog();
                }
            }
            
            if (StressTester != null)
                StressTester.CancelThreads = true;
            
            SaveOptions();
        }


        private void StressTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckForNewVersion(false);
        }

        void Export(string mode)
        {
            string filePrefix = "WebSurge";
            if (!string.IsNullOrEmpty(FileName))
            {
                string tFileName = Path.GetFileNameWithoutExtension(FileName);
                if (!string.IsNullOrEmpty(tFileName))
                    filePrefix = tFileName;
            }
            string fileName = filePrefix + "_Result_" + DateTime.Now.ToString("yyyy-MM-dd");

            if (mode == "xml")
            {
                var diag = new SaveFileDialog
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "xml",
                    Filter = "Xml files (*.xml)|*.xml|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Export Results as XML",
                    RestoreDirectory = true,
                    FileName = fileName + ".xml"
                };
                var res = diag.ShowDialog();

                if (res == DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);

                    if (SerializationUtils.SerializeObject(StressTester.Results, diag.FileName, false))
                       App.OpenFileInExplorer(diag.FileName);
                    
                }
            }
            else if (mode == "json")
            {
                var diag = new SaveFileDialog
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "JSON",
                    Filter = "JSON files (*.json)|*.json|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Export Results as JSON",
                    RestoreDirectory = true,
                    FileName = fileName + ".json"
                };
                var res = diag.ShowDialog();

                if (res == DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);
                    string json = JsonSerializationUtils.Serialize(StressTester.Results, false, true);

                    if (json != null)
                    {
                        File.WriteAllText(diag.FileName, json);
                        App.OpenFileInExplorer(diag.FileName);
                    }
                }
            }
            else if (mode == "raw")
            {
                var diag = new SaveFileDialog
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "websurge",
                    Filter = "websurge files (*.websurge)|*.websurge|txt files (*.txt)|*.txt|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Export Results",
                    RestoreDirectory = true,
                    FileName = fileName + ".websurge"
                };
                var res = diag.ShowDialog();
                if (res == DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);

                    var parser = new SessionParser();
                    if (parser.Save(StressTester.Results, diag.FileName))
                        App.OpenFileInExplorer(diag.FileName);
                }
            }
            else if (mode == "results")
            {
                var diag = new SaveFileDialog
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "txt",
                    Filter = "txt files (*.json)|*.json|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Export Summary Results",                    
                    RestoreDirectory = true,
                    FileName = filePrefix + "_Result_Summary_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json"
                };
                if (!string.IsNullOrEmpty(FileName))
                {
                    var file = Path.GetFileNameWithoutExtension(FileName);
                    var dt = DateTime.Now;                    
                }

                var res = diag.ShowDialog();
                if (res == DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);

                    var result = StressTester.ResultsParser.GetResultReport(
                        StressTester.Results,
                        StressTester.TimeTakenForLastRunMs, 
                        StressTester.ThreadsUsed);
                    string json = JsonSerializationUtils.Serialize(result, false, true);
                    File.WriteAllText(diag.FileName, json);

                    App.OpenFileInExplorer(diag.FileName);
                }
            }
        }

        void AttachWatcher(string fileName)
        {
            Watcher.EnableRaisingEvents = false;
            Watcher.Changed -= watcher_Changed;

            Watcher.Filter = Path.GetFileName(fileName);
            // monitor size change to avoid last write dupe events
            Watcher.NotifyFilter = NotifyFilters.LastWrite;  // Note: Fires more than once!!!
            Watcher.Path = Path.GetDirectoryName(fileName);
            Watcher.Changed += watcher_Changed;
            Watcher.EnableRaisingEvents = true;
        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // this gets called more than once per update typically            
            BeginInvoke(new Action(() =>
            {
                Thread.Sleep(1);

                try
                {
                    // reload the session list
                    OpenFile(FileName);
                }               
                catch { }
            }));
        }

        #endregion

        #region Editing Requests

        void LoadRequest(HttpRequestData request)
        {
            if (request == null)
                request = new HttpRequestData();

            txtName.Text = request.Name;
            txtHttpMethod.Text = request.HttpVerb;
            txtRequestUrl.Text = request.Url;
            txtRequestUrl.Tag = request;
            chkIsActive.Checked = request.IsActive;

            StringBuilder sb = new StringBuilder();
            foreach (var hd in request.Headers)
            {
                sb.AppendLine(hd.Name + ": " + hd.Value);
            }
            txtRequestHeaders.Text = sb.ToString();
            txtRequestContent.Text = request.RequestContent;            
        }

        HttpRequestData SaveRequest(HttpRequestData request)
        {                        
            if (request == null)
                request = new HttpRequestData();
            
            request.IsActive = chkIsActive.Checked;
            request.Name = txtName.Text;
            request.Url = txtRequestUrl.Text;
            request.HttpVerb = txtHttpMethod.Text;
            request.RequestContent = txtRequestContent.Text;
            request.ParseHttpHeaders(txtRequestHeaders.Text);            

            return request;
        }

        private void RequestData_Changed(object sender, EventArgs e)
        {
            if (ActiveRequest == null)
                return;
            
            FixClickFocus();
            
            string displayData = null;
            displayData = ActiveRequest.Url + "|" + ActiveRequest.Name;
            SaveRequest(ActiveRequest);

            Debug.WriteLine($"{displayData} - {ActiveRequest.Url}|{ActiveRequest.Name} - {ActiveRequest?.RequestContent?.Take(100)}");

            if (displayData == null  ||
                displayData != ActiveRequest.Url + "|" + ActiveRequest.Name)                
                RenderRequests(Requests);
        }
        #endregion

        #region Progress

        void StressTester_Progress(ProgressInfo obj)
        {
            try
            {
                BeginInvoke(new Action<ProgressInfo>(ShowProgress), obj);
            }
            catch (Exception ex)
            {                
                Trace.WriteLine("StressTester_Progress Exception: " + ex.Message);
            }            
        }

        void ShowProgress(ProgressInfo progress)
        {
            string text = progress.RequestsProcessed.ToString("n0") +
                          " requests, " + progress.RequestsFailed.ToString("n0") + " failed | " +
                          progress.SecondsProcessed + " of " +
                          progress.TotalSecondsToProcessed + " secs ";
            if (progress.SecondsProcessed > 0)
                text += "| " + (progress.RequestsProcessed / progress.SecondsProcessed).ToString("n0") + " request/sec ";
            
            if (progress.RequestsFailed > 0)
                txtProcessingTime.ForeColor = Color.Red;
            else
                txtProcessingTime.ForeColor = Color.DarkGreen;

            ShowStatus(text, 3);
        }


        private static object ConsoleLog = new object();
        private StringBuilder statusOutput = new StringBuilder();
        private DateTime lastUpdate = DateTime.UtcNow;
        private int statusOutputBufferDelay = 225;

        private void StressTester_RequestProcessed(HttpRequestData req)
        {
            string currentLine = req.StatusCode + " -  " + req.HttpVerb + " " + req.Url + "(" +
                                 req.TimeTakenMs.ToString("n0") + "ms)";

            string textToWrite = null;

            lock (ConsoleLog)
            {
                statusOutput.AppendLine(currentLine);
                if (lastUpdate.AddMilliseconds(statusOutputBufferDelay) < DateTime.UtcNow)
                {
                    lastUpdate = DateTime.UtcNow;
                    textToWrite = statusOutput.ToString();
                    statusOutput.Length = 0;
                }
            }

            if (textToWrite != null)
            {
                try
                {
                    BeginInvoke(new Action<string>(WriteConsoleOutput), textToWrite);
                }
                catch
                {
                }
            }
        }

        private void tbNoProgressEvents_CheckedChanged(object sender, EventArgs e)
        {
            var button = sender as ToolStripButton;
            StressTester.Options.NoProgressEvents = button.Checked;
        }

        #endregion

        #region Request Processing
        void WriteConsoleOutput(string output)
        {
            var len = txtConsole.Text.Length;     
            
            // truncate output
            if (len > 30000)
            {                
                var txt = txtConsole.Text;                
                txt = txt.Substring(txt.Length - 4000);
                
                // find line break and append output
                txt = txt.Substring(txt.IndexOf("\r\n") + 2) + output + "\r\n";                
                txtConsole.Text = txt;                           
            }
            else
            {
                txtConsole.AppendText(output);
            }            
        }


        private void TestSiteUrl(HttpRequestData req)
        {            
            Cursor = Cursors.WaitCursor;

            StressTester.CancelThreads = false;
            
            var action = new Action<HttpRequestData>(rq =>
            {
                ShowStatus("Checking URL: " + rq.Url);
           
                var result = StressTester.CheckSite(rq, StressTester.InteractiveSessionCookieContainer);
                string html = TemplateRenderer.RenderTemplate("Request.cshtml", result);                

                Invoke(new Action<string>(htmlText =>
                {
                    HtmlPreview(html);
                    TabsResult.SelectedTab = tabPreview;                    
                    ShowStatus("URL check complete.", 1, 5000);

                }),html);
                
            });
            action.BeginInvoke(req,null,null);
           
            Cursor = Cursors.Default;
        }

        private void TestAllSiteUrls()
        {
            ShowStatus("Running all requests...");
            ListResults.BeginUpdate();
            ListResults.Items.Clear();
            ListResults.EndUpdate();

            TestResultBrowser.Visible = false;
            txtConsole.Visible = true;
            txtConsole.Text = "Processing " + Requests.Count + " requests...\r\n";

            TabsResult.SelectedTab = tabOutput;
            Application.DoEvents();            
            StressTester.Results.Clear();

            StressTester.CancelThreads = false;
            
            var t = new Thread(() =>
            {
                statusOutputBufferDelay = 1; 
                StressTester.RunSessions(Requests.Where( req=> req.IsActive).ToList(), true);
                statusOutputBufferDelay = 250;

                Application.DoEvents();

                ShowStatus("Running all requests completed.", 5);
                Application.DoEvents();
                
                BeginInvoke(new Action<List<HttpRequestData>>(ParseResults), StressTester.Results);
            });
            t.Start();


        }


        void StartProcessing()
        {
            txtConsole.Text = "";
            Application.DoEvents();

            ListResults.BeginUpdate();
            ListResults.Items.Clear();
            ListResults.EndUpdate();

            TabsResult.SelectedTab = tabOutput;
                        
            TestResultBrowser.Visible = false;
            txtConsole.Visible = true;

            Application.DoEvents();
            
            StressTester.Running = true;            
            Thread td = new Thread(StartProcessing_Internal);
            td.Start();
        }

        void StartProcessing_Internal()
        {
            var config = App.Configuration.StressTester;

            int time = StringUtils.ParseInt(tbtxtTimeToRun.Text,20);            
            int threads = StringUtils.ParseInt(tbtxtThreads.Text,5);

            config.LastSecondsToRun = time;
            config.LastThreads = threads;

            //ShowStatus("Parsing Fiddler Sessions...");
            //Requests = StressTester.ParseSessionFile(FileName) as List<HttpRequestData>;

            if (Requests == null)
            {
                ShowStatus();
                UpdateButtonStatus();
                MessageBox.Show(StressTester.ErrorMessage, App.Configuration.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);                
                return;
            }

            ShowStatus("Checking sites...");
            var results = StressTester.CheckAllSites(Requests, threads, time);

            if (results == null)
            {
                ShowStatus("Aborted.", timeout: 5000);
                Invoke(new Action(UpdateButtonStatus));

                MessageBox.Show(StressTester.ErrorMessage, App.Configuration.AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                if (StressTester.ErrorMessage.Contains("The free version"))
                    ShellUtils.GoUrl("http://west-wind.com/websurge/pricing.aspx");

                return;
            }

            ShowStatus("Parsing requests...");
            BeginInvoke(new Action<List<HttpRequestData>>(ParseResults), StressTester.Results);

            ShowStatus("Done.");

            try
            {
                Invoke(new Action(UpdateButtonStatus));
            }
            catch { }           
        }

        private void ParseResults(List<HttpRequestData> results)
        {
            TabSessions.SelectedTab = tabResults;

            var html = StressTester.ResultsParser.GetResultReportHtml(StressTester.Results,
                StressTester.TimeTakenForLastRunMs,StressTester.ThreadsUsed);

            HtmlPreview(html, false,"html\\_results.html");

            Application.DoEvents();

            RenderResultList(results);
        }


        void RenderResultList(List<HttpRequestData> results)
        {
            ListResults.BeginUpdate();
            ListResults.Items.Clear();
            ListResults.EndUpdate();

            // clear out progress status
            ShowStatus(null, 3);

            Application.DoEvents();

            IEnumerable<HttpRequestData> filtered = null;
            
            filtered = results;
            int filterCount = 3000;                        

            if (cmbListDisplayMode.SelectedItem as string == "Errors")
                filtered =
                    results.Where(rq => rq.StatusCode == null || !rq.StatusCode.StartsWith("2"))                        
                        .ToList();
            else if (cmbListDisplayMode.SelectedItem as string == "Success")
                filtered =
                    results.Where(rq => rq.StatusCode != null && rq.StatusCode.StartsWith("2"))                        
                        .ToList();
            else if (cmbListDisplayMode.SelectedItem as string  == "All")
                filtered = results;

            int fullCount = filtered.Count();

            if (fullCount == 0)
            {
                cmbListDisplayMode.SelectedItem = "All";
                filtered = results;
                fullCount = results.Count;
            }

            filtered = filtered.Take(filterCount).ToList();
            filterCount = filtered.Count();

            ListResults.BeginUpdate();
            foreach(var request in filtered)
            {                
                var item = ListResults.Items.Add(new ListViewItem
                {
                    Text = request.StatusCode,
                    Tag = request
                });
                if (request.StatusCode == null) 
                    item.ImageKey = "error";
                else if (!request.IsError)
                    item.ImageKey = "ok";
                else if (request.StatusCode == "404")
                    item.ImageKey = "notfound";
                else if (request.StatusCode == "401" || request.StatusCode == "403")
                    item.ImageKey = "accessdenied";
                else
                    item.ImageKey = "error";

                item.SubItems.Add(request.HttpVerb + " " + request.Url);
                item.SubItems.Add(request.ErrorMessage);                
            }

            ListResults.EndUpdate();
            lblRequestCount.Text = filterCount.ToString("n0") + " of " +  fullCount.ToString("n0") + " shown";

        }

        void RenderRequests(List<HttpRequestData> requests, int selectedIndex = -1)
        {
            ListRequests.BeginUpdate();
            ListRequests.Items.Clear();
            ListRequests.EndUpdate();

            if (requests == null)
                return;

            Application.DoEvents();

            var filtered = requests;

            ListRequests.BeginUpdate();

            for (int i = 0; i < filtered.Count; i++)
            {
                var request = filtered[i];

                var item = ListRequests.Items.Add(new ListViewItem
                {
                    Text = request.HttpVerb,
                    Tag = request
                });

                if (!request.IsActive)
                    item.Font = new Font(item.Font,FontStyle.Italic);

                if (!string.IsNullOrEmpty(request.RequestContent))
                    item.ImageKey = "upload";
                else
                    item.ImageKey = "download";

                item.SubItems.Add(string.IsNullOrEmpty(request.Name) ? request.Url : request.Name);
                //item.ToolTipText = request.Headers;
            }

            ListRequests.EndUpdate();

            TabSessions.SelectedTab = tabSession;

            if (selectedIndex > -1 && selectedIndex < ListRequests.Items.Count)
                ListRequests.Items[selectedIndex].Selected = true;
        }


        void HtmlPreview(string html, bool showInBrowser = false, string fileName = "html\\_preview.html")
        {
            fileName = fileName.ToLower();

            string outputPath = App.UserDataPath + fileName;
            File.WriteAllText(outputPath, html);
            string file = (outputPath).Replace("\\", "/");

            if (!showInBrowser)
            {
                // _results.html is rendered into the output tab
                if (fileName == "html\\_results.html")
                {                    
                    TestResultBrowser.Url = new Uri(file);
                    TestResultBrowser.Visible = true;                    
                    txtConsole.Visible = false;
                }
                else
                    PreViewBrowser.Url = new Uri(file);
            }
            else
                ShellUtils.GoUrl(file);
        }


        private void StressTestForm_Shown(object sender, EventArgs e)
        {
            if (Splash != null)
            {
                new Timer(p => Splash.Invoke(new Action(() =>
                {
                    if (Splash != null)
                    {
                        Splash.Close();                        
                    }
                })),
                null, 1000, 
                Timeout.Infinite);                
            }
            Application.DoEvents();
        }

        private void PreViewBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();

            //if (string.IsNullOrEmpty(url) || url.StartsWith("file://") || url.StartsWith("javascript:") ||
            //    url.StartsWith("about:blank")) 
            //    return;

            if (url.StartsWith("app://"))
            {
                var tokens = url.Replace("app://", "").Split('/');
                if (tokens != null && tokens.Length > 0)
                {
                    e.Cancel = true;
                    string action = tokens[0];
                    
                    // app://preview/id
                    if (action == "preview")
                    {
                        string outputType = ActiveRequest.GetTypeOfContent();
                        if (outputType == null)
                            return;

                        
                        if (outputType == "html")
                            HtmlPreview(ActiveRequest.ResponseContent.Replace(@"""//",@"=""http://").Replace("'//","http://"),true);
                        else if (outputType == "json")                        
                            HtmlPreview(JValue.Parse(ActiveRequest.ResponseContent).ToString(Formatting.Indented) ,true,"html\\_preview.json");
                        else if (outputType == "xml")                                                 
                            HtmlPreview(ActiveRequest.ResponseContent, true, "html\\_preview.xml");
                    }                    
                }                
            }
        }

        private void PreViewBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            string url = PreViewBrowser.StatusText;
            if (string.IsNullOrEmpty(url) || url.StartsWith("file://"))
                return;

            // use OS browser rather
            ShellUtils.GoUrl(url);
            e.Cancel = true;
        }


        void AddRecentFiles(object sender = null, CancelEventArgs e = null)
        {
            RecentFilesContextMenu.Items.Clear();

            if (App.Configuration.RecentFiles == null)
                return;

            int x = 0;
            foreach (var s in App.Configuration.RecentFiles)
            {
                x++;

                if (!File.Exists(s))
                    continue;

                if (!string.IsNullOrEmpty(FileName) &&
                    s.ToLower() == FileName.ToLower())
                    continue;
                
                var btn = new ToolStripMenuItem
                {
                    Text = s,
                    Name = "RecentFile_" + (x -1),
                    ImageKey = "websurge"
                };

                btn.Click +=
                    (snd, args) =>
                    {
                        var bt = snd as ToolStripMenuItem;
                        OpenFile(bt.Text);                        
                    };

                RecentFilesContextMenu.ImageList = Images;
                RecentFilesContextMenu.Items.Add(btn);   
            }
        }

        public void CheckForNewVersion(bool force = false)
        {
            var updater = new ApplicationUpdater(typeof(Program));
            //updater.LastCheck = DateTime.UtcNow.AddDays(-50);
            if (updater.NewVersionAvailable(!force))
            {
                if (MessageBox.Show(updater.VersionInfo.Detail + "\r\n" +
                    "Do you want to download and install this version?",
                    updater.VersionInfo.Title,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    ShellUtils.GoUrl(App.InstallerDownloadPage);
                    //updater.DownloadProgressChanged += updater_DownloadProgressChanged;
                    //ShowStatus("Downloading Update - Version " + updater.VersionInfo.Version);
                    //updater.Download();
                    //updater.ExecuteDownloadedFile();
                    //ShowStatus("Download completed.");
                    Application.Exit();
                }
            }
            App.Configuration.CheckForUpdates.LastUpdateCheck = DateTime.UtcNow.Date;            
        }


        private RegisterDialog regForm;

        public void ShowStatus(string text = null, int panelId = 1, int timeout = 0)
        {

            var action = new Action<string, int>(ShowStatus_Internal);
            try
            {
                Invoke(action, text, panelId);
            }
            catch { }


            if (timeout > 0)
            {
                new Timer(id =>
                {
                    try
                    {
                        Invoke(action, "Ready", (int) id);
                    }
                    catch { }
                }, panelId, timeout ,0);                                
            }

                
        }
        #endregion

        #region Event Handling
        private void ButtonHandler(object sender, EventArgs e)
        {
			// force active control to refresh the binding source
            FixClickFocus();

            if (sender == tbOpen || sender == btnOpen ||
                sender == tbOpenFromDropbox || sender == tbOpenFromOneDrive ||                
                sender == txtProcessingTime)
            {
                string path = null;
                if (sender == tbOpenFromDropbox)
                    path = CloudFolders.DropboxDirectory;
                else if (sender == tbOpenFromOneDrive)
                    path = CloudFolders.OneDriveDirectory;

                var fd = new OpenFileDialog
                {
                    DefaultExt = ".websurge;.txt;.log",
                    Filter = "WebSurge files (*.websurge)|*.websurge|Text files (*.txt)|*.txt|Log Files (*.log)|*.log|All Files (*.*)|*.*",
                    CheckFileExists = true,
                    RestoreDirectory = true,                    
                    FileName = "",
                    Title = "Open WebSurge Request File"
                };
                if (!string.IsNullOrEmpty(path))
                    fd.InitialDirectory = path;

                var dr = fd.ShowDialog();
                if (dr != DialogResult.Cancel)
                {
                    FileName = Path.GetFullPath(fd.FileName);
                    OpenFile(FileName);
                }
            }
            else if (sender == btnClose)
            {
                CloseSession();
            }
            else if (sender == tbCapture || sender == btnCapture)
            {
                var fiddlerForm = new FiddlerCapture(this);
                fiddlerForm.Owner = this;
                fiddlerForm.Show();
            }
            else if (sender == tbStart || sender == btnStart)
            {
                StartProcessing();
            }
            else if (sender == tbStop || sender == btnStop)
            {
                StressTester.CancelThreads = true;
                ShowStatus("Stopping request processing...");
            }
            else if (sender == tbEditFile || sender == btnEditFile)
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    Process.Start(new ProcessStartInfo("notepad.exe", FileName) {UseShellExecute = true});
                    AttachWatcher(FileName);                 
                }
            }
            else if (sender == btnAbout)
            {
                var splashForm = new Splash();
                splashForm.StartPosition = FormStartPosition.Manual;
                splashForm.Left = Left + Width / 2 - splashForm.Width / 2;
                splashForm.Top = Top + Height / 2 - splashForm.Height / 2;
                splashForm.Show();
            }
            else if (sender == btnGotoWebSite)
                ShellUtils.GoUrl("http://websurge.west-wind.com");
            else if (sender == btnGotoRegistration)
                ShellUtils.GoUrl("http://store.west-wind.com/product/websurge");
            else if (sender == btnRegistration)
            {
                var regForm = new UnlockKeyForm("Web Surge");
                regForm.Left = Left + Width / 2 - regForm.Width / 2;
                regForm.Top = Top + Height / 2 - regForm.Height / 2 + 40;
                regForm.ShowDialog();
                UpdateButtonStatus();
            }
            else if (sender == tbExportXml || sender == btnExportXml)
                Export("xml");
            else if (sender == tbExportJson || sender == btnExportJson)
                Export("json");
            else if (sender == tbExportRaw || sender == btnExportHtml)
                Export("raw");
            else if (sender == btnExportResultSummary)
                Export("results");


            if (sender == tbTimeTakenPerUrl || sender == tbTimeTakenPerUrlChart || sender == btnTimeTakenPerUrlChart)
            {
                if (ListResults.SelectedItems.Count > 0)
                {
                    var listItem = ListResults.SelectedItems[0];
                    var request = listItem.Tag as HttpRequestData;
                    var form = new ChartFormZed(StressTester.Results, request.Url, ChartTypes.TimeTakenPerRequest);
                    form.ParentForm = this;
                    form.Show();
                }
            }
            if (sender == tbRequestsPerSecondChart || sender == tbRequestPerSecondChart ||
                sender == btnRequestsPerSecondChart)
            {
                if (StressTester.Results.Count > 0)
                {
                    var form = new ChartFormZed(StressTester.Results, null, ChartTypes.RequestsPerSecond);
                    form.ParentForm = this;
                    form.Show();
                }
            }
            if (sender == tbDeleteRequest || sender == tbDeleteRequest2)
            {
                if (ListRequests.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem listItem in ListRequests.SelectedItems)
                    {
                        var request = listItem.Tag as HttpRequestData;
                        Requests.Remove(request);
                        var index = listItem.Index;                        
                        ListRequests.Items.Remove(listItem);
                        if (index < ListRequests.Items.Count)
                            ListRequests.Items[index].Selected = true;
                    }
                }
            }
            if (sender == tbToggleActive)
            {
                if (ListRequests.SelectedItems.Count > 0)
                {
                    int index = 0;
                    foreach (ListViewItem listItem in ListRequests.SelectedItems)
                    {
                        index = listItem.Index;
                        var request = listItem.Tag as HttpRequestData;
                        request.IsActive = !request.IsActive;
                    }
                    RenderRequests(Requests,index);
                }
            }
            if (sender == tbMoveUp || sender == tbMoveDown)
            {
                if (ListRequests.SelectedItems.Count > 0)
                {
                    // assign current sort order
                    for(var x=0; x < Requests.Count-1; x++)
                        Requests[x].SortOrder = Requests.Count - 1 - x;

                    int newVal = 0;
                    int index = -1;
                    foreach (ListViewItem listItem in ListRequests.SelectedItems)
                    {
                        var request = listItem.Tag as HttpRequestData;
                        HttpRequestData swap;

                        if (sender == tbMoveUp)
                        {
                            newVal = request.SortOrder + 1;
                            if (newVal > Requests.Count - 1)
                                newVal = request.SortOrder;                            
                            index = Requests.Count - 1 - newVal;
                        }
                        if (sender == tbMoveDown)
                        {
                            newVal = request.SortOrder - 1 ;
                            if (newVal < 0)
                                newVal = 0;
                            index = Requests.Count - 1 - newVal;
                        }

                        swap = Requests.FirstOrDefault(req => req.SortOrder == newVal);
                        if (swap != null)
                            swap.SortOrder = request.SortOrder;
                        request.SortOrder = newVal;                    
                    }
                    Requests = Requests.OrderByDescending(req => req.SortOrder).ToList();
                    RenderRequests(Requests,index);
                }
            }
            if (sender == tbEditRequest || sender == tbEditRequest2)
            {
                if (ListRequests.SelectedItems.Count > 0)
                {
                    var listItem = ListRequests.SelectedItems[0];
                    var request = listItem.Tag as HttpRequestData;
                    LoadRequest(request);
                    TabsResult.SelectedTab = tabRequest;
                }
            }
            if (sender == tbNewRequest || sender == tbNewRequest2)
            {
                var req = new HttpRequestData()
                {
                    HttpVerb = "GET",
                    Url = "http://",
                    RequestContent = string.Empty
                };
                req.Headers.Add( new HttpRequestHeader
                {
                    Name = "Accept-Encoding",
                    Value = "gzip,deflate"
                });
                                
                txtRequestUrl.Tag = req;
                ActiveRequest = req;
                LoadRequest(req);

                Requests.Add(req);
                RenderRequests(Requests);

                TabsResult.SelectedTab = tabRequest;
                chkIsActive.Checked = true;
                txtRequestUrl.Focus();
            }
            if (sender == tbCopyFromRequest)
            {
                var req = txtRequestUrl.Tag as HttpRequestData;
                if (req == null)
                    return;

                var newRequest = HttpRequestData.Copy(req);
                if (!newRequest.Url.EndsWith("_COPIED"))
                    newRequest.Url += "_COPIED";

                ActiveRequest = newRequest;
                LoadRequest(newRequest);

                Requests.Add(newRequest);
                RenderRequests(Requests);
                
                txtRequestUrl.Tag = newRequest; // it's a new request

                TabsResult.SelectedTab = tabRequest;
                txtRequestUrl.Focus();

            }
            if (sender == btnSaveRequest)
            {
                RequestData_Changed(sender, e);
                //var req = txtRequestUrl.Tag as HttpRequestData;
                //bool isNew = req == null;
                //req = SaveRequest(req);                

                //if (isNew)
                //{
                //    Requests.Add(req);
                //    txtRequestUrl.Tag = req;
                //}

                //RenderRequests(Requests);
            }
            if (sender == btnRunRequest || sender == tbTestRequest2 || sender == tbTestRequest)
            {
                var req = txtRequestUrl.Tag as HttpRequestData;
                req = SaveRequest(req);

                TestSiteUrl(req);
            } 
            if (sender == tbTestAll)
            {
                TestAllSiteUrls();
            }


            if (sender == btnOpenInDefaultBrowser)
            {
                var context = ((ToolStripItem) sender).GetCurrentParent() as ContextMenuStrip;
                if (context == null)
                    return;

                var browser = context.SourceControl as WebBrowser;
                ShellUtils.GoUrl(browser.Url.ToString());
            }
            if (sender == btnOpenUrlInDefaultBrowser)
            {
                var context = ((ToolStripItem)sender).GetCurrentParent() as ContextMenuStrip;
                if (context == null)
                    return;

                ShellUtils.GoUrl(ActiveRequest.Url);
            }
            if (sender == btnCopyRequestTraceToClipboard)
            {
                var context = ((ToolStripItem)sender).GetCurrentParent() as ContextMenuStrip;
                if (context == null || ActiveRequest == null)
                    return;
                
                var reqText = ActiveRequest.ToRequestHttpHeader(true);
                Clipboard.SetText(reqText);

                this.ShowStatus("Request data copied to the Clipboard...", 1, 7000);
            }
            if (sender == btnCopyResponseTraceToClipboard)
            {
                var context = ((ToolStripItem)sender).GetCurrentParent() as ContextMenuStrip;
                if (context == null || ActiveRequest == null)
                    return;

                var reqText = ActiveRequest.ToResponseHttpHeader();
                Clipboard.SetText(reqText);

                this.ShowStatus("Response data copied to the Clipboard...", 1, 7000);
            }


            if (sender == tbSaveAllRequests || sender == tbSaveAllRequests2 ||
                sender == btnSaveAllRequests || sender == btnSaveAllRequestsAs ||
                sender == tbSaveToDropbox || sender == tbSaveToOneDrive)
            {	            
                var parser = new SessionParser();

                var path = App.UserDataPath;

                
                if (sender == tbSaveToDropbox)
                    path = CloudFolders.DropboxDirectory;
                else if(sender == tbSaveToOneDrive)
                    path = CloudFolders.OneDriveDirectory;
                else if (!string.IsNullOrEmpty(FileName))
                    path = Path.GetDirectoryName(FileName);


                StressTester.Options.LastSecondsToRun = StringUtils.ParseInt(tbtxtTimeToRun.Text, 20);
                StressTester.Options.LastThreads = StringUtils.ParseInt(tbtxtThreads.Text, 5);
                
                var file = string.Empty;
                if (!string.IsNullOrEmpty(FileName))
                    file = Path.GetFileName(FileName);

                if (sender != btnSaveAllRequestsAs && 
                    sender != tbSaveToDropbox && sender != tbSaveToOneDrive &&
                    File.Exists(FileName))
                {
                    parser.Save(Requests, FileName, StressTester.Options);
                    ShowStatus("Session saved.", 1, 4000);
                }
                else
                {
                    SaveFileDialog sd = new SaveFileDialog
                    {
                        Filter = "websurge files (*.websurge)|*.websurge|txt files (*.txt)|*.txt|All files (*.*)|*.*",
                        FilterIndex = 1,
                        FileName = file,
                        CheckFileExists = false,
                        OverwritePrompt = false,
                        AutoUpgradeEnabled = true,
                        CheckPathExists = true,
                        InitialDirectory = path,
                        RestoreDirectory = true
                    };

                    var result = sd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        FileName = sd.FileName;
                        parser.Save(Requests, sd.FileName, StressTester.Options);
                    }
                }

            }
            else if (sender == btnFeedback)
            {
                string msg = 
                    @"We really appreciate your feedback, good and bad!

Please use our message board to post any feedback about
what doesn't work quite the way you'd like it to, or 
to request functionality that's not there at the moment.
Comments about what you like or how WebSurge is helping
you get your job done are also very welcome. 

Your feedback is important and helps us improve WebSurge
so please don't be shy. 

It takes a only a few seconds to create an account 
to post a message. We want to hear from you and we
reply to all messages promptly with frank discussions.

Thank you!";
                var res = MessageBox.Show(msg, App.Configuration.AppName + " Bug Report",MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
                if (res == DialogResult.OK)
                    ShellUtils.GoUrl("http://west-wind.com/wwThreads/default.asp?forum=West%20Wind%20WebSurge");
            }
            else if (sender == btnBugReport)
            {
                string msg =
                    @"Please use our issue tracker to report bugs or enhancement
requests on the GitHub repository.

When describing your issue, please provide as much detail 
as possible, and if possible provide steps to reproduce 
the behavior, so we can replicate and fix the issue as 
quickly as possible.

You can also look at the WebSurge Error Log by using the
Help | Show Error Log menu option. You can copy and paste 
the relevant error section into the issue text.

If you're not sure whether you have discovered a bug or
need clarification of functionality, please use the 
Feedback and Suggestions link from the Help menu.

We want to hear from you, and we respond promptly to
any reported issues.";

                var res =  MessageBox.Show(msg, App.Configuration.AppName + " Feedback", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
                if (res == DialogResult.OK)
                    ShellUtils.GoUrl("https://github.com/RickStrahl/WestWindWebSurge/issues");
            }                
            else if (sender == btnShowErrorLog)
            {
                ShellUtils.GoUrl(App.UserDataPath + "WebSurgeErrors.log");
            }
            else if (sender == btnGotoSettingsFolder)
            {
                ShellUtils.GoUrl(App.UserDataPath);
            }
            else if (sender == btnCheckForNewVersion)
                CheckForNewVersion(true);
            else if (sender == btnHelp)
                System.Windows.Forms.Help.ShowHelp(this,"websurge.chm",HelpNavigator.TableOfContents);
            else if(sender == btnHelpIndex)
                System.Windows.Forms.Help.ShowHelp(this, "websurge.chm", HelpNavigator.KeywordIndex);
            else if (sender == btnExit)
                Close();

            UpdateButtonStatus();
        }

        public void UpdateButtonStatus()
        {
            tbOpen.Enabled = !StressTester.Running;
            btnOpen.Enabled = tbOpen.Enabled;

            tbEditFile.Enabled = !string.IsNullOrEmpty(FileName);
            btnEditFile.Enabled = tbEditFile.Enabled;

            tbStart.Enabled = !StressTester.Running && Requests.Count > 0;
            btnStart.Enabled = tbStart.Enabled;
            

            tbStop.Enabled = StressTester.Running;
            btnStop.Enabled = tbStop.Enabled;

            var hasResults = StressTester.Results.Count > 0;
            btnExport.Enabled = hasResults;
            tbExport.Enabled = hasResults;            

            tbCharts.Enabled = hasResults;
            btnCharts.Enabled = hasResults;

            // Result Selected
            var isResultSelected = ListResults.SelectedItems.Count > 0;
            tbTimeTakenPerUrl.Enabled = isResultSelected;
            tbTimeTakenPerUrlChart.Enabled = isResultSelected;

            // All Requests
            tbSaveAllRequests.Enabled = Requests.Count > 0;
            tbSaveAllRequests2.Enabled = tbSaveAllRequests.Enabled;
            tbTestAll.Enabled = tbSaveAllRequests.Enabled;
            tbSaveToDropbox.Enabled = tbSaveAllRequests.Enabled && CloudFolders.IsDropbox;
            tbOpenFromDropbox.Enabled = tbSaveToDropbox.Enabled;
            tbSaveToOneDrive.Enabled = tbSaveAllRequests.Enabled && CloudFolders.IsOneDrive;
            tbOpenFromOneDrive.Enabled = tbSaveToOneDrive.Enabled;

            tbNoProgressEvents.Checked = StressTester.Options.NoProgressEvents;

            // Request Selected
            var isRequestSelected = ListRequests.SelectedItems.Count > 0;
            tbEditRequest.Enabled = isRequestSelected;
            tbEditRequest2.Enabled = isRequestSelected;
            tbDeleteRequest.Enabled = isRequestSelected;
            tbDeleteRequest2.Enabled = isRequestSelected;
            tbTestRequest.Enabled = isRequestSelected;
            tbTestRequest2.Enabled = isRequestSelected;

            btnShowErrorLog.Enabled = File.Exists(App.LogFile) &&
                                      new FileInfo(App.LogFile).Length > 0;

            
        }

        private void ListResults_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {            
            if (e.Item.Tag == null)
                return;

            UpdateButtonStatus();

            HttpRequestData req = e.Item.Tag as HttpRequestData;
            if (e.Item.Tag == null)
                return;

            ActiveRequest = req;

            string html = TemplateRenderer.RenderTemplate("Request.cshtml", req);            
            HtmlPreview(html);

            TabsResult.SelectedTab = tabPreview;
        }

        private void ListRequests_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item.Tag == null)
                return;

            UpdateButtonStatus();

            HttpRequestData req = e.Item.Tag as HttpRequestData;
            if (e.Item.Tag == null)
                return;

            ActiveRequest = req;

            string html = TemplateRenderer.RenderTemplate("Request.cshtml", req);

            HtmlPreview(html);

            LoadRequest(req);

            if (TabsResult.SelectedTab != tabPreview && TabsResult.SelectedTab != tabRequest)
                TabsResult.SelectedTab = tabPreview;            
        }

        private void ListRequests_DoubleClick(object sender, EventArgs e)
        {
            if (ListRequests.SelectedItems.Count < 1)
                return;

            var listItem = ListRequests.SelectedItems[0];
            var request = listItem.Tag as HttpRequestData;

            if (request == null)
                return;

            TestSiteUrl(request);
        }

        /// <summary>
        /// Translates context menu clicks to ButtonHandler clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuItemClickedToButtonHandler_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            ButtonHandler(e.ClickedItem, e);
        }

        private void cmbListDisplayMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderResultList(StressTester.Results);
        }

        private void ListRequests_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (ListRequests.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem listItem in ListRequests.SelectedItems)
                    {
                        var request = listItem.Tag as HttpRequestData;
                        Requests.Remove(request);
                        ListRequests.Items.Remove(listItem);
                    }
                }                
            }
        }

        private void TextBoxEditor_DoubleClick(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null)
                return;

            var editForm = new EditForm(tb);
            editForm.ShowDialog();
        }

        private void HeadersContentSplitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            App.Configuration.WindowSettings.HeadersContentSplit = HeadersContentSplitter.SplitterDistance;
        }

        private void FixClickFocus()
        {
            if (ActiveControl != null)
            {
                // *** Force focus to 'save' content
                Control ctrl = ActiveControl;
                Application.DoEvents();
                label2.Focus();
                Application.DoEvents();
                ctrl.Focus();
            }
        }

        void t_Tick(object sender, EventArgs e)
        {
            regForm = new RegisterDialog();
            regForm.StartPosition = FormStartPosition.Manual;
            regForm.Left = Left + Width/2 - regForm.Width/2 ;
            regForm.Top = Top + Height/2 - regForm.Height/2 + 40;
            regForm.TopMost = true;
            regForm.Show();
        }

        void updater_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ShowStatus("Downloading Update: " + (e.BytesReceived/1000).ToString("n0") + "kb  of  " +
                       (e.TotalBytesToReceive/1000).ToString("n0") + "kb");
        }

        private void BrowserContextMenu_Opening(object sender, CancelEventArgs e)
        {
            btnCopyResponseTraceToClipboard.Enabled = !string.IsNullOrEmpty(ActiveRequest?.StatusCode);
        }

        #endregion
    }

}
