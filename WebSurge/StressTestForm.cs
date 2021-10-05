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
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSurge.Core;
using WebSurge.Editor;
using WebSurge.Support;
using Westwind.Utilities;
using Timer = System.Threading.Timer;

namespace WebSurge
{
    public partial class StressTestForm : Form
    {
        public StressTester StressTester { get; set; }

        public HttpRequestData ActiveRequest { get; set;  }

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
                    !value.Contains("West Wind WebSurge ("))
                    text = $"{value} - {text}";

                base.Text = text;
            }
        }

        public List<HttpRequestData> Requests
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
            var form = new Splash(true);
            form.Show();
            Application.DoEvents();

            if (!string.IsNullOrEmpty(fileName))
                FileName = fileName;

            InitializeComponent();

            this.Text = string.Empty;  // force title update
        }

        #region load and unload

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

            StressTester.RequestWriter = new RequestWriter(StressTester);
            StressTester.InteractiveSessionCookieContainer = null;
                  
            // render an empty list
            RenderResultList(StressTester.RequestWriter.GetResults());

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

            if (!UnlockKey.IsRegistered())
            {
                var accessed = App.Configuration.WindowSettings.Accesses;
                var displayCount = 10;

                if (accessed > 150)
                    displayCount = 1;
                else if (accessed > 100)
                    displayCount = 2;
                else if (accessed > 70)
                    displayCount = 4;
                else if (accessed > 30)
                    displayCount = 7;

                if (accessed % displayCount == 0)
                {
                    regForm = new RegisterDialog();
                    regForm.StartPosition = FormStartPosition.Manual;
                    regForm.Left = Left + Width / 2 - regForm.Width / 2;
                    regForm.Top = Top + Height / 2 - regForm.Height / 2 + 40;
                    regForm.TopMost = true;

                    Hide();

                    regForm.ShowDialog();
                }
            }
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

                    if (SerializationUtils.SerializeObject(StressTester.RequestWriter.GetResults(), diag.FileName, false))
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
                        StressTester.RequestWriter,
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

            chkWrapHeaderText.Checked = App.Configuration.WrapHeaderText;
            txtRequestHeaders.WordWrap = App.Configuration.WrapHeaderText;            
            
            StringBuilder sb = new StringBuilder();
            foreach (var hd in request.Headers)
            {
                sb.AppendLine(hd.Name + ": " + hd.Value);
            }
            txtRequestHeaders.Text = sb.ToString();
            txtRequestContent.Text = request.RequestContent;
            chkNoRandomize.Checked = request.SortNoRandmomize;
        }

        HttpRequestData SaveRequest(HttpRequestData request)
        {                        
            if (request == null)
                request = new HttpRequestData();
            
            request.IsActive = chkIsActive.Checked;
            request.Name = txtName.Text;
            request.Url = txtRequestUrl.Text;
            request.HttpVerb = txtHttpMethod.Text;
            if (!string.IsNullOrEmpty(request.RequestContent) && request.HttpVerb == "GET")
            {
                request.HttpVerb = "POST";
                txtHttpMethod.Text = "POST";
            }
            request.RequestContent = txtRequestContent.Text;
            request.ParseHttpHeaders(txtRequestHeaders.Text);

            request.ResponseContent = null;
            request.ResponseHeaders = null;
      
            App.Configuration.WrapHeaderText = chkWrapHeaderText.Checked;
            request.SortNoRandmomize = chkNoRandomize.Checked;

            return request;
        }

        private void btn_PasteRawRequest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (ActiveRequest == null)
            {
                ShowStatus("There's no active request active to paste into. Please select New or select a request to edit first.", 6000);
                return;
            }

            var clipText = Clipboard.GetText();
            if (string.IsNullOrEmpty(clipText))
            {
                ShowStatus("No request data on the clipboard.",timeout:6000);
                return;
            }

            if (!(clipText.Contains("http") || clipText.Contains(": ") || clipText.Contains("HTTP/")))
            {
                if (MessageBox.Show("It looks like this is not an HTTP request.\r\nDo you want to try and parse it anyway?",
                        "Paste HTTP Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }

            var name = ActiveRequest.Name;

            var parser = new SessionParser();
            var request = parser.ParseRequest(clipText);

            if (!string.IsNullOrEmpty(name))
                request.Name = name;

            DataUtils.CopyObjectData(request, ActiveRequest, "Id");           
            LoadRequest(ActiveRequest);

            RequestData_Changed(null, null);
            RenderRequests(Requests);
        }

        public void RequestData_Changed(object sender, EventArgs e)
        {
            if (ActiveRequest == null)
            {
                ActiveRequest = new HttpRequestData();
                if (Requests.Count == 0)
                    Requests.Add(ActiveRequest);
            }


            FixClickFocus();
            
            string displayData = null;
            displayData = ActiveRequest.Url + "|" + ActiveRequest.Name;
            SaveRequest(ActiveRequest);

            Debug.WriteLine($"{displayData} - {ActiveRequest.Url}|{ActiveRequest.Name} - {ActiveRequest?.RequestContent?.Take(100)}");

            if (displayData != ActiveRequest.Url + "|" + ActiveRequest.Name)                
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
            string text;

            if (progress.IsWarmingUp)
                text = "Warming up:  " + progress.RequestsProcessed.ToString("n0") + " requests | " +
                          progress.SecondsProcessed + " of " + StressTester.Options.WarmupSeconds + " secs";
            else
            {

                text = progress.RequestsProcessed.ToString("n0") +
                          " requests, " + progress.RequestsFailed.ToString("n0") + " failed | " +
                          progress.SecondsProcessed + " of " +
                          progress.TotalSecondsToProcess + " secs ";
                if (progress.SecondsProcessed > 0)
                        text += "| " + (progress.RequestsProcessed / progress.SecondsProcessed).ToString("n0") +
                              " request/sec ";
            }

            if(progress.IsWarmingUp)
                txtProcessingTime.ForeColor = Color.SlateGray;
            else if (progress.RequestsFailed > 0)
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
            if (ActiveRequest == null)
                ActiveRequest = req;

            Cursor = Cursors.WaitCursor;

            StressTester.CancelThreads = false;

            AceInterop?.ShowProcessingHeader(true);

            var action = new Action<HttpRequestData>(rq =>
            {
                ShowStatus("Checking URL: " + req.Url);
           
                var result =  StressTester.CheckSite(req, StressTester.InteractiveSessionCookieContainer);
                string html = TemplateRenderer.RenderTemplate("Request.cshtml", result);
                
                Invoke(new Action<string>(htmlText =>
                {
                    ActiveRequest.ResponseContent = result.ResponseContent;
                    ActiveRequest.ResponseHeaders = result.ResponseHeaders;

                    ActiveRequest.StatusCode = result.StatusCode;
                    ActiveRequest.StatusDescription = result.StatusDescription;

                    HtmlPreview(html);
                    TabsResult.SelectedTab = tabPreview;       

                    ShowStatus("URL check complete.", 1, 5000);

                }), html);
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
            StressTester.RequestWriter.Clear();

            StressTester.CancelThreads = false;
            
            var t = new Thread(() =>
            {
                statusOutputBufferDelay = 1; 
                StressTester.RunSessions(Requests.Where( req=> req.IsActive).ToList(), true);
                statusOutputBufferDelay = 250;

                Application.DoEvents();

                ShowStatus("Running all requests completed.", 5);
                Application.DoEvents();
                
                BeginInvoke(new Action<List<HttpRequestData>>(ParseResults), StressTester.RequestWriter.GetResults());
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
                    ShellUtils.GoUrl("https://websurge.west-wind.com/pricing.aspx");

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

            var html = StressTester.ResultsParser.GetResultReportHtml(StressTester.RequestWriter,
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
                {
                    PreViewBrowser.Url = new Uri(file);
                }
            }
            else
                ShellUtils.GoUrl(file);
        }


        public void UpdateRequestHtmlPreview(HttpRequestData req = null)
        {
            if (req == null)
                req = ActiveRequest;

            string html = TemplateRenderer.RenderTemplate("Request.cshtml", req);
            HtmlPreview(html);
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

        public void CheckForNewVersion(bool force = false)
        {
            var updater = new ApplicationUpdater(typeof(Program));            
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


        /// <summary>
        /// Displays a status message.
        ///
        /// use - 1 to specify the default timeout
        /// </summary>
        /// <param name="text"></param>
        /// <param name="panelId"></param>
        /// <param name="timeout"></param>
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

        #region  Preview Browser and Ace Editor Interaction

        private AceInterop AceInterop;

        private void PreViewBrowser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (AceInterop == null)
                AceInterop = new AceInterop(this);

            AceInterop.InitializeInterop();
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

                    if (action == "test")
                    {
                        this.ButtonHandler(btnRunRequest, EventArgs.Empty);
                    }

                    if (action == "copyrequest")
                    {
                        var req = ActiveRequest;
                        req.Url = HttpRequestData.FixupUrl(ActiveRequest.Url, App.Configuration.StressTester.SiteBaseUrl);

                        var reqText = req.ToRequestHttpHeader();
                        if (!string.IsNullOrEmpty(req.StatusCode))
                        {
                            reqText +=
                                "\r\n------------------------------------------------------------------\r\n" +
                                req.ToResponseHttpHeader();
                        }

                        Clipboard.SetText(reqText);

                        ShowStatus("Request text copied to the clipboard.", timeout: 5000);
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

        private void BrowserContextMenu_Opening(object sender, CancelEventArgs e)
        {
            btnCopyResponseTraceToClipboard.Enabled = !string.IsNullOrEmpty(ActiveRequest?.StatusCode);
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
            else if (sender == btnClose || sender == tbCreateNewSession)
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
                ShellUtils.GoUrl(App.WebHomeUrl);
            else if (sender == btnGotoRegistration)
                ShellUtils.GoUrl(App.PurchaseUrl);
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
                    var form = new ChartFormZed(StressTester.RequestWriter, request.Url, ChartTypes.TimeTakenPerRequest);
                    form.ParentForm = this;
                    form.Show();
                }
            }
            if (sender == tbRequestsPerSecondChart || sender == tbRequestPerSecondChart ||
                sender == btnRequestsPerSecondChart)
            {
                if (StressTester.Results.Count > 0)
                {
                    var form = new ChartFormZed(StressTester.RequestWriter, null, ChartTypes.RequestsPerSecond);
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

            if (sender == tbNewRequest || sender == tbNewRequest2 ||
                sender == btnCreateNewSession || sender == tbCreateNewSession)
            {
                var req = new HttpRequestData()
                {
                    HttpVerb = "GET",
                    Url = "http://",
                    RequestContent = string.Empty
                };
                req.Headers.Add(new HttpRequestHeader
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
                TabsResult.SelectedTab.Focus();

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
                tabRequest.Focus();
                txtRequestUrl.Focus();

            }
            if (sender == btnSaveRequest)
            {
                RequestData_Changed(sender, e);
                
            }
            if (sender == btnRunRequest || sender == tbTestRequest2 || sender == tbTestRequest)
            {
                var req = txtRequestUrl.Tag as HttpRequestData;
                req = SaveRequest(req);

                if (req != null)
                {
                    ActiveRequest = req;
                    if (this.Requests == null || Requests.Count == 0)
                    {
                        Requests = new List<HttpRequestData>( new [] { req });
                        RenderRequests(Requests, 0);
                    }
                }

                TabsResult.SelectedTab = tabPreview;

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

                SaveRequest(ActiveRequest);

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
            else if (sender == btnPostmanExport)
            {
                var fileOnly = Path.GetFileNameWithoutExtension(FileName);
                
                SaveFileDialog sd = new SaveFileDialog
                {
                    Filter = "json files (*.json)|*.json|All files (*.*)|*.*",
                    FilterIndex = 1,
                    FileName = fileOnly + ".json",
                    CheckFileExists = false,
                    OverwritePrompt = false,
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    InitialDirectory = Path.GetDirectoryName(FileName),
                    RestoreDirectory = true
                };

                var result = sd.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var postman = new PostmanIntegration();
                    if (postman.Export(StringUtils.FromCamelCase(fileOnly), 
                            Requests, 
                            App.Configuration.StressTester,
                            sd.FileName) == null)
                        ShowStatus("Export failed", timeout: 5000);
                    else
                    {
                        ShowStatus("Export completed", timeout: 5000);
                        Westwind.Utilities.ShellUtils.OpenFileInExplorer(sd.FileName);
                    }
                }
            }
            else if (sender == btnPostmanImport)
            {
                string filename, filepath;

                if (!string.IsNullOrEmpty(FileName))
                {
                    filename = Path.GetFileName(FileName);
                    filepath = Path.GetDirectoryName(FileName);
                }
                else
                {
                    filename = null;
                    filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }


                var od = new OpenFileDialog
                {
                    Filter = "json files (*.json)|*.json|All files (*.*)|*.*",
                    FilterIndex = 1,
                    FileName = filename,
                    InitialDirectory = filepath,
                    RestoreDirectory = true,
                    CheckFileExists = true,
                    DefaultExt = "json",
                    Title = "Select Postman file to import"
                };
                var result = od.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var postman = new PostmanIntegration();
                    var requests = postman.ImportFromFile(od.FileName);
                    if (requests == null)
                        ShowStatus("Postman import failed.", timeout: 5000);
                    else
                    {
                        if (Requests != null && Requests.Count > 0)
                        {
                            if (MessageBox.Show(
                                "Do you want to append the imported requests to the current WebSurge session?",
                                "Import from PostMan",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button1) == DialogResult.No)
                            {
                                CloseSession();
                                Requests = requests.Requests;
                            }
                            else
                                Requests.AddRange(requests.Requests);
                        }
                        else 
                            Requests = requests.Requests;

                        RenderRequests(Requests);
                        ShowStatus("Postman Session Import completed.", timeout: 5000);
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
                    ShellUtils.GoUrl("https://support.west-wind.com?forum=West%20Wind%20WebSurge");
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
                ShellUtils.GoUrl("https://websurge.west-wind.com/docs/");            
            else if (sender == btnExit)
                Close();

            UpdateButtonStatus();
        }

        private void TabsResult_Click(object sender, EventArgs e)
        {
            var tab = TabsResult.SelectedTab;
            if (tab == tabPreview)
            {
                // explicitly force a refresh if manually updating
                UpdateRequestHtmlPreview();
            }

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
            //btnExport.Enabled = hasResults;
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
            if (req == null)
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

            req.RemoveResponseData();
            ActiveRequest = req;

            UpdateRequestHtmlPreview(req);

            LoadRequest(req);

            if (TabsResult.SelectedTab != tabPreview && TabsResult.SelectedTab != tabRequest)
                TabsResult.SelectedTab = tabPreview;            
        }


        /// <summary>
        /// Manual Selection assignment and double-click detection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListRequests_MouseDown(object sender, MouseEventArgs e)
        {

            // Handle double clicks here
            if (e.Clicks == 2)
            {
                if (ListRequests.SelectedItems.Count < 1) // should never fire
                    return;

                var listItem = ListRequests.SelectedItems[0];
                var request = listItem.Tag as HttpRequestData;

                if (request == null)
                    return;

                Thread.Sleep(80);
                Application.DoEvents();
                BeginInvoke((Action) (() =>  { TestSiteUrl(request); }));
                
                return;
            }

            // single click - force selection if not selected or single selection
            //if (ListRequests.SelectedItems.Count < 2)
            //{
            //    var lvi = GetItemFromPoint(ListRequests, Cursor.Position);
            //    if (lvi != null)
            //        lvi.Selected = true;
            //}
        }

        /// <summary>
        /// Gets a ListView Item from a location
        /// Call with: GetItemFromPoint(ListRequests, Cursor.Position)
        /// </summary>
        /// <param name="listView">The listview </param>
        /// <param name="mousePosition">Absolute (not control relative) position</param>
        /// <returns></returns>
        private ListViewItem GetItemFromPoint(ListView listView = null, Point? mousePosition = null)
        {
            if (listView == null)
                listView = ListRequests;
            if (!mousePosition.HasValue)
                mousePosition = Cursor.Position;

            // translate the mouse position from screen coordinates to 
            // client coordinates within the given ListView
            Point localPoint = listView.PointToClient(mousePosition.Value);
            return listView.GetItemAt(localPoint.X, localPoint.Y);
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

            var header= ActiveRequest.Headers.FirstOrDefault(h => h.Name.Equals("content-type", StringComparison.OrdinalIgnoreCase));
            string format = "text";
            if (tb == txtRequestHeaders)
                format = "http";
            else if (header != null && tb == txtRequestContent)
            {
                var ct = header.Value;
                if (ct == "text/xml")
                    format = "xml";
                if (ct == "application/json")
                    format = "json";
                if (ct.StartsWith("image/"))
                    format = "image";
            }

            var editForm = new EditForm(new EditorFormParameters
            {
                Content = tb.Text, 
                Syntax = format, 
                // not updated since Content is passed
                TextBoxToUpdate = tb
            });
            editForm.Text = $"Content Editor ({format})";
            editForm.ShowDialog();

            if (!editForm.Cancelled)
                tb.Text = editForm.EditorText;

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

      

        void updater_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ShowStatus("Downloading Update: " + (e.BytesReceived/1000).ToString("n0") + "kb  of  " +
                       (e.TotalBytesToReceive/1000).ToString("n0") + "kb");
        }

        #endregion

        private void chkWrapHeaderText_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null)
                return;

            txtRequestHeaders.WordWrap = checkBox.Checked;
            App.Configuration.WrapHeaderText = true;
        }



        private void ListRequests_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("WebSurge.HttpRequestData"))
                return;

            Point cp = ListRequests.PointToClient(new Point(e.X, e.Y));
            ListViewItem lvItem = ListRequests.GetItemAt(cp.X, cp.Y);
            if (lvItem == null) return;

            var targetRequest = lvItem.Tag as HttpRequestData;
            if (targetRequest == null) return;

          

            // assign current sort order
            for(var x=0; x < Requests.Count-1; x++)
                Requests[x].SortOrder = 100 + (Requests.Count - 1 - x) * 100;


            var itemCount = 0;
            HttpRequestData firstItem = null;
            // Get all selected items
            foreach (var item in ListRequests.SelectedItems)
            {
                lvItem = item as ListViewItem;
                var droppedRequest = lvItem.Tag as HttpRequestData;
                if (droppedRequest == null) continue;
                if (firstItem == null)
                    firstItem = droppedRequest;

                droppedRequest.SortOrder = targetRequest.SortOrder - 1 - itemCount;
                itemCount++;
            }

            Requests = Requests.OrderByDescending(req => req.SortOrder).ToList();
            int index = firstItem != null ? index = Requests.IndexOf(firstItem) : 0;

            // Rerender the list
            RenderRequests(Requests, index);
        }

        private void ListRequests_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var request = ActiveRequest;
            ListRequests.DoDragDrop(ActiveRequest, DragDropEffects.Move);
            Debug.WriteLine("StartDrag: " + ActiveRequest.Name ?? ActiveRequest.Url );
        }

        private void ListRequests_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data == null) return; 

            

            if (e.Data.GetDataPresent("WebSurge.HttpRequestData"))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }
    }

}
