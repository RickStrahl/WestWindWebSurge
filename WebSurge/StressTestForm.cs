using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Westwind.Utilities;

namespace WebSurge
{
    public partial class StressTestForm : Form
    {
        StressTester StressTester { get; set; }

        private string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value; 
                lblStatusFilename.Text = "Session File: " + value;                
            }
        }
        private string _FileName;

        List<HttpRequestData> Requests { get; set; }
        FileSystemWatcher Watcher { get; set; }
        public Splash Splash { get; set; }

        public StressTestForm()
        {
            InitializeComponent();
        }

        public void OpenFile(string fileName = null)
        {
            if (fileName == null)
                fileName = FileName;
            else
                FileName = fileName;

            Requests = StressTester.ParseFiddlerSessions(FileName);
            RenderRequests(Requests);

            App.Configuration.LastFileName = FileName;

            AttachWatcher(fileName);
        }


        private void StressTestForm_Load(object sender, EventArgs e)
        {
            Watcher = new FileSystemWatcher();

            App.Configuration.WindowSettings.Load(this);

            StressTester = new StressTester();
            StressTester.RequestProcessed += StressTester_RequestProcessed;
            StressTester.Progress += StressTester_Progress;

            var config = App.Configuration.StressTester;

            if (!string.IsNullOrEmpty(App.Configuration.LastFileName))
            {
                FileName = Path.GetFullPath(App.Configuration.LastFileName);
                if (!File.Exists(FileName))
                    FileName = null;
            }

            if (!string.IsNullOrEmpty(FileName))
                OpenFile(FileName);

            tbtxtThreads.Text = config.LastThreads.ToString();
            tbtxtTimeToRun.Text = config.LastSecondsToRun.ToString();

            LoadOptions();

            OptionsPropertyGrid.SelectedObject = StressTester.Options;

            cmbListDisplayMode.SelectedItem = "Errors";
            TabsResult.SelectedTab = tabOptions;

            AddRecentFiles();

            UpdateButtonStatus();
        }

        void StressTester_Progress(ProgressInfo obj)
        {
            try
            {
                BeginInvoke(new Action<ProgressInfo>(ShowProgress), obj);
            }
            catch { }
        }

        void ShowProgress(ProgressInfo progress)
        {
            string text = progress.RequestsProcessed.ToString("n0") +
                          " requests, " + progress.RequestsFailed.ToString("n0") + " failed | " +
                          progress.SecondsProcessed + " of " +
                          progress.TotalSecondsToProcessed + " secs ";
            if (progress.SecondsProcessed > 0)
                text += "| " + progress.RequestsProcessed / progress.SecondsProcessed + " request/sec ";

            ShowStatus(text, 3);
        }


        private static object ConsoleLog = new object();
        private StringBuilder statusOutput = new StringBuilder();
        private DateTime lastUpdate = DateTime.UtcNow;

        void StressTester_RequestProcessed(HttpRequestData req)
        {
            string output = req.StatusCode + " -  " + req.HttpVerb + " " + req.Url + "(" + req.TimeTakenMs.ToString("n0") + "ms)";
            lock (ConsoleLog)
            {
                statusOutput.AppendLine(output);
                if (lastUpdate.AddMilliseconds(300) < DateTime.UtcNow)
                {
                    lastUpdate = DateTime.UtcNow;
                    Invoke(new Action<string>(ShowRequestProcessed), statusOutput.ToString());
                    statusOutput.Clear();
                }
            }
        }
        void ShowRequestProcessed(string output)
        {
            txtConsole.AppendText(output);
            if (txtConsole.Text.Length > 50000)
            {
                var txt = txtConsole.Text.Substring(5000);
                txtConsole.Text = "";
                txtConsole.Text = txt.Substring(txt.IndexOf("\r\n") + 2) + output;
            }
        }


        private void ButtonHandler_Click(object sender, EventArgs e)
        {

            if (sender == tbOpen || sender == btnOpen || sender == txtProcessingTime)
            {
                var fd = new OpenFileDialog()
                {
                    DefaultExt = ".txt;.log",
                    Filter = "Text files (*.txt)|*.txt|Log Files (*.log)|*.log|All Files (*.*)|*.*",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    FileName = "1_Full.txt",
                    Title = "Open Fiddler Capture File"
                };
                var dr = fd.ShowDialog();
                if (dr != System.Windows.Forms.DialogResult.Cancel)
                {
                    FileName = Path.GetFullPath(fd.FileName);
                    OpenFile(FileName);                    
                }
            }
            else if (sender == btnClose)
            {
                Requests = new List<HttpRequestData>();                
                RenderRequests(Requests);
                TabSessions.SelectedTab = tabSession; 
                
                RenderResults(Requests);
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
                    ShellUtils.GoUrl(FileName);
                AttachWatcher(FileName);
            }
            else if (sender == btnAbout)
            {
                var splashForm = new Splash();
                splashForm.Show();
            }
            else if (sender == btnGotoWebSite)
                ShellUtils.GoUrl("http://west-wind.com/WebSurge");
            else if (sender == btnGotoRegistration)
                ShellUtils.GoUrl("http://store.west-wind.com/product/websurge");
            else if (sender == btnRegistration)
            {
                var regForm = new UnlockKeyForm("Web Surge");
                regForm.Show();
            }
            else if (sender == tbExportXml || sender == btnExportXml)
                Export("xml");
            else if (sender == tbExportJson || sender == btnExportJson)
                Export("json");
            else if (sender == tbExportHtml || sender == btnExportHtml)
                Export("html");

            if (sender == tbTimeTakenPerUrl || sender == tbTimeTakenPerUrlChart || sender == btnTimeTakenPerUrlChart)
            {
                if (ListResults.SelectedItems.Count > 0)
                {
                    var listItem = ListResults.SelectedItems[0];
                    var request = listItem.Tag as HttpRequestData;
                    var form = new ChartFormZed(StressTester.Results, request.Url, ChartTypes.TimeTakenPerRequest);
                    form.Show();
                }
            }
            if (sender == tbRequestsPerSecondChart || sender == tbRequestPerSecondChart || sender == btnRequestsPerSecondChart)
            {
                if (StressTester.Results.Count() > 0)
                {
                    var form = new ChartFormZed(StressTester.Results, null, ChartTypes.RequestsPerSecond);
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
                        ListRequests.Items.Remove(listItem);
                    }
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
            if (sender == tbAddRequest || sender == tbAddRequest2)
            {
                txtRequestUrl.Tag = null; 
                
                txtHttpMethod.Text = "GET";
                txtRequestUrl.Text = "http://";                
                txtRequestHeaders.Text = "Accept-Encoding: gzip,deflate";
                txtRequestContent.Text = string.Empty;
                TabsResult.SelectedTab = tabRequest;                
            }
            if (sender == btnSaveRequest)
            {
                var req = txtRequestUrl.Tag as HttpRequestData;
                bool isNew = req == null;
                req = SaveRequest(req);

                if (isNew)
                    Requests.Add(req);

                RenderRequests(Requests);
            }
            if (sender == btnRunRequest)
            {
                var req = txtRequestUrl.Tag as HttpRequestData;
                req = SaveRequest(req);

                var reqResult = StressTester.CheckSite(req);

                string html = reqResult.ToHtml(true);
                HtmlPreview(html);
                TabsResult.SelectedTab = tabPreview;
            }

        
            if (sender == tbSaveAllRequests || sender == tbSaveAllRequests2)
            {
                var parser = new FiddlerSessionParser();

                var path = App.AppDataPath;
                if (!string.IsNullOrEmpty(FileName))
                    path = Path.GetDirectoryName(FileName);

                var file = string.Empty;
                if (!string.IsNullOrEmpty(FileName))
                    file = Path.GetFileName(FileName);

                SaveFileDialog sd = new SaveFileDialog
                {
                    Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                    FilterIndex = 1,
                    FileName = file,
                    CheckFileExists = false,
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    InitialDirectory = path,
                    RestoreDirectory = true
                };

                var result = sd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    FileName = sd.FileName;
                    parser.Save(Requests, sd.FileName);
                }
            }

            else if (sender == btnExit)
                Close();

            UpdateButtonStatus();
        }


        /// <summary>
        /// Translates context menu clicks to ButtonHandler clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuItemClickedToButtonHandler_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            ButtonHandler_Click(e.ClickedItem, e);
        }


        void StartProcessing()
        {
            txtConsole.Text = "";
            ListResults.BeginUpdate();
            ListResults.Items.Clear();
            ListResults.EndUpdate();

            TabsResult.SelectedTab = tabOutput;

            Application.DoEvents();

            StressTester.Running = true;
            Thread td = new Thread(StartProcessing_Internal);
            td.Start();
        }

        void StartProcessing_Internal()
        {
            var config = App.Configuration.StressTester;

            //int time = StringUtils.ParseInt(tbtTxtTimeToRun.Text,2)
            int time = int.Parse(tbtxtTimeToRun.Text);
            int threads = int.Parse(tbtxtThreads.Text);
            config.LastSecondsToRun = time;
            config.LastThreads = threads;

            //ShowStatus("Parsing Fiddler Sessions...");
            //Requests = StressTester.ParseFiddlerSessions(FileName) as List<HttpRequestData>;

            if (Requests == null)
            {
                ShowStatus();
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

            Invoke(new Action(UpdateButtonStatus));
        }

        private void ParseResults(List<HttpRequestData> results)
        {

            TabSessions.SelectedTab = tabResults;

            txtConsole.Text = StressTester.ParseResults() +
                "\r\n-------------\r\n" +
                txtConsole.Text;

            Application.DoEvents();

            RenderResults(results);
        }


        void RenderResults(List<HttpRequestData> results)
        {
            ListResults.BeginUpdate();
            ListResults.Items.Clear();
            ListResults.EndUpdate();

            // clear out progress status
            ShowStatus(null, 3);

            Application.DoEvents();

            List<HttpRequestData> filtered = null;
            int count = 0;
            filtered = results;

            if (cmbListDisplayMode.SelectedItem == "Errors")
            {
                filtered = results.Where(rq => rq.StatusCode == null || !rq.StatusCode.StartsWith("2")).Take(1000).ToList();
            }
            else if (cmbListDisplayMode.SelectedItem == "Success")
            {
                filtered = results.Where(rq => rq.StatusCode != null && rq.StatusCode.StartsWith("2")).Take(1000).ToList();
            }

            if (filtered.Count == 0)
            {
                cmbListDisplayMode.SelectedItem = "All";
                filtered = results;
            }

            ListResults.BeginUpdate();
            for (int i = 0; i < filtered.Count; i++)
            {
                var request = filtered[i];

                var item = ListResults.Items.Add(new ListViewItem()
                {
                    Text = request.StatusCode,
                    Tag = request,
                });
                if (!request.IsError)
                    item.ImageKey = "ok";
                else
                    item.ImageKey = "error";

                item.SubItems.Add(request.HttpVerb + " " + request.Url);
                item.SubItems.Add(request.ErrorMessage);
                var resp = request.LastResponse ?? string.Empty;
                if (resp.Length > 1001)
                    resp.Substring(0, 1000);
                item.ToolTipText = resp;
            }

            ListResults.EndUpdate();
        }

        void RenderRequests(List<HttpRequestData> requests)
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

                var item = ListRequests.Items.Add(new ListViewItem()
                {
                    Text = request.HttpVerb,
                    Tag = request
                });
                

                if (!string.IsNullOrEmpty(request.RequestContent))
                    item.ImageKey = "upload";
                else
                    item.ImageKey = "download";

                item.SubItems.Add(request.Url);
                //item.ToolTipText = request.Headers;
            }

            ListRequests.EndUpdate();

            TabSessions.SelectedTab = tabSession;
        }


        private void StressTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveOptions();

        }

        public void ShowStatus(string text = null, int panelId = 1, int timeout = 0)
        {
            var action = new Action<string, int>(StatusTest_Internal);

            if (timeout == 0)
            {
                try
                {
                    Invoke(action, text, panelId);
                }
                catch { }
            }
        }

        public void StatusTest_Internal(string text, int panelId = 1)
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
            var config = App.Configuration.StressTester;
            var options = StressTester.Options;

            config.ReplaceCookieValue = options.ReplaceCookieValue;
            config.ReplaceDomain = options.ReplaceDomain;
            config.MaxResponseSize = options.MaxResponseSize;
            config.DelayTimeMs = options.DelayTimeMs;
            config.RandomizeRequests = options.RandomizeRequests;
            config.RequestTimeoutMs = options.RequestTimeoutMs;
            App.Configuration.LastFileName = FileName;

            App.Configuration.WindowSettings.Save(this);

            // Save any changed configuration settings
            App.Configuration.Write();
        }

        void LoadOptions()
        {
            var config = App.Configuration.StressTester;
            var options = StressTester.Options;

            options.ReplaceCookieValue = config.ReplaceCookieValue;
            options.MaxResponseSize = config.MaxResponseSize;
            options.DelayTimeMs = config.DelayTimeMs;
            options.RandomizeRequests = config.RandomizeRequests;
        }

        void LoadRequest(HttpRequestData request)
        {            
            txtHttpMethod.Text = request.HttpVerb;
            txtRequestUrl.Text = request.Url;
            txtRequestUrl.Tag = request;
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

            request.Url = txtRequestUrl.Text;
            request.HttpVerb = txtHttpMethod.Text;
            request.RequestContent = txtRequestContent.Text;
            request.ParseHttpHeaders(txtRequestHeaders.Text);



            return request;
        }

        private void cmbListDisplayMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderResults(StressTester.Results);
        }


        public void UpdateButtonStatus()
        {
            tbOpen.Enabled = !StressTester.Running;
            btnOpen.Enabled = tbOpen.Enabled;

            tbEditFile.Enabled = !string.IsNullOrEmpty(FileName);
            btnEditFile.Enabled = tbEditFile.Enabled;

            tbStart.Enabled = !StressTester.Running && !string.IsNullOrEmpty(FileName);
            btnStart.Enabled = tbStart.Enabled;

            tbStop.Enabled = StressTester.Running;
            btnStop.Enabled = tbStop.Enabled;

            var hasResults = StressTester.Results.Count > 0;
            btnExport.Enabled = hasResults;
            tbExport.Enabled = hasResults;            

            tbCharts.Enabled = hasResults;
            btnCharts.Enabled = hasResults;

            var isResultSelected = ListResults.SelectedItems.Count > 0;
            tbTimeTakenPerUrl.Enabled = isResultSelected;
            tbTimeTakenPerUrlChart.Enabled = isResultSelected;

            var isRequestSelected = ListRequests.SelectedItems.Count > 0;
            tbEditRequest.Enabled = isRequestSelected;
            tbEditRequest2.Enabled = isRequestSelected;
            tbDeleteRequest.Enabled = isRequestSelected;
            tbDeleteRequest2.Enabled = isRequestSelected;

        }

        private void ListResults_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            
            if (e.Item.Tag == null)
                return;

            UpdateButtonStatus();

            HttpRequestData req = e.Item.Tag as HttpRequestData;
            if (e.Item.Tag == null)
                return;

            string html = req.ToHtml(true);
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

            string html = req.ToHtml(true);
            HtmlPreview(html);

            LoadRequest(req);

            if (TabsResult.SelectedTab != tabPreview && TabsResult.SelectedTab != tabRequest)
                TabsResult.SelectedTab = tabPreview;            
        }


        void HtmlPreview(string html)
        {

            string outputPath = App.AppDataPath + "_preview.html";

            File.WriteAllText(outputPath, html);
            string file = (outputPath).Replace("\\", "/");
            PreViewBrowser.Url = new Uri(file);
        }

        void Export(string mode)
        {
            if (mode == "xml")
            {
                var diag = new SaveFileDialog()
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "xml",
                    Filter = "Xml files (*.xml)|*.xml|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Export Results as XML",
                    RestoreDirectory = true,
                    FileName = "KuhelaResults.xml"
                };
                var res = diag.ShowDialog();

                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);

                    if (SerializationUtils.SerializeObject(StressTester.Results, diag.FileName, false))
                        ShellUtils.GoUrl(diag.FileName);
                }
            }
            else if (mode == "json")
            {
                var diag = new SaveFileDialog()
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "JSON",
                    Filter = "JSON files (*.json)|*.json|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Export Results as JSON",
                    RestoreDirectory = true,
                    FileName = "KuhelaResults.json"
                };
                var res = diag.ShowDialog();

                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);
                    string json = JsonSerializationUtils.Serialize(StressTester.Results, false, true);

                    if (json != null)
                    {
                        File.WriteAllText(diag.FileName, json);
                        ShellUtils.GoUrl(diag.FileName);
                    }
                }
            }
            else if (mode == "html")
            {
                var diag = new SaveFileDialog()
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "html",
                    Filter = "Html files (*.html)|*.html|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Export Results as HTML",
                    RestoreDirectory = true,
                    FileName = "KuhelaResults.html"
                };
                var res = diag.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);

                    StringBuilder sb = new StringBuilder();
                    foreach (var req in StressTester.Results)
                    {
                        sb.Append(req.ToHtml(true));
                    }

                    File.WriteAllText(diag.FileName, sb.ToString());
                }
            }
        }

        private void tbNoProgressEvents_CheckedChanged(object sender, EventArgs e)
        {
            var button = sender as ToolStripButton;
            StressTester.Options.NoProgressEvents = button.Checked;
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
            var watcher = sender as FileSystemWatcher;
            Invoke(new Action(() =>
            {
                Thread.Sleep(1);
                OpenFile(FileName);
            }));
        }

        private void StressTestForm_Shown(object sender, EventArgs e)
        {
            if (Splash != null)
            {
                new System.Threading.Timer(p => Splash.Invoke(new Action(() =>
                {
                    if (Splash != null)
                        Splash.Close();
                })),
                    null, 1000, Timeout.Infinite);
            }
            Application.DoEvents();
        }

        private void PreViewBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();

            if (string.IsNullOrEmpty(url) || url.StartsWith("file://"))  
                return;

            // use OS browser rather
            ShellUtils.GoUrl(url.ToString());
            e.Cancel = true;
        }

        private void PreViewBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            string url = PreViewBrowser.StatusText;
            if (string.IsNullOrEmpty(url) || url.StartsWith("file://"))
                return;

            // use OS browser rather
            ShellUtils.GoUrl(url.ToString());
            e.Cancel = true;

            e.Cancel = true;
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

        
        void AddRecentFiles(object sender = null, CancelEventArgs e = null)
        {
            RecentFilesContextMenu.Items.Clear();
            int x = 0;
            foreach (var s in App.Configuration.RecentFiles)
            {
                if (!File.Exists(s))
                    continue;

                var btn = new ToolStripMenuItem
                {
                    Text = s,
                    Name = "RecentFile_" + x
                };

                btn.Click +=
                    (snd, args) =>
                    {
                        var bt = snd as ToolStripMenuItem;
                        OpenFile(bt.Text);
                    };

                RecentFilesContextMenu.Items.Add(btn);
                x++;
            }
        }


    }

}
