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

namespace Kuhela
{
    public partial class StressTestForm : Form
    {
        public StressTester StressTester { get; set; }
        public string FileName { get; set;  }
        public List<HttpRequestData> Requests { get; set; }

        public StressTestForm()
        {
            InitializeComponent();

            // Fix 
            //var tab = new TabPadding(this.TabsResult);
            //var tab2 = new TabPadding(this.TabSessions);
        }

        private void StressTestForm_Load(object sender, EventArgs e)
        {
            StressTester = new StressTester();
            StressTester.RequestProcessed += StressTester_RequestProcessed;
            StressTester.Progress += StressTester_Progress;

			if(!string.IsNullOrEmpty(App.Configuration.FileName))
			{			
				FileName = Path.GetFullPath(App.Configuration.FileName);
				if (!File.Exists(FileName))
					FileName = null;
			}

            if (!string.IsNullOrEmpty(FileName))
            {
                lblStatusFilename.Text = "Fiddler File: " + FileName;
                Requests = StressTester.ParseFiddlerSessions(FileName);
                RenderRequests(Requests);
            }

            tbtxtThreads.Text = App.Configuration.LastThreads.ToString();
            tbtxtTimeToRun.Text = App.Configuration.LastSecondsToRun.ToString();

            LoadOptions();

            OptionsPropertyGrid.SelectedObject = StressTester.Options;

            cmbListDisplayMode.SelectedItem = "Errors";
            TabsResult.SelectedTab = tabOptions;

            UpdateButtonStatus();
        }



        void StressTester_Progress(ProgressInfo obj)
        {
            BeginInvoke(new Action<ProgressInfo>(ShowProgress), obj);
        }

        void ShowProgress(ProgressInfo progress)
        {
            string text = progress.RequestsProcessed.ToString("n0") +
                          " requests, " + progress.RequestsFailed.ToString("n0") + " failed | " +
                          progress.SecondsProcessed + " of " +
                          progress.TotalSecondsToProcessed + " secs ";
            if (progress.SecondsProcessed > 0)
                text +=  "| " + progress.RequestsProcessed/progress.SecondsProcessed + " req/sec ";
                          
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
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    FileName = "1_Full.txt",
                    Title = "Open Fiddler Capture File"
                };
                var dr = fd.ShowDialog();
                if (dr != System.Windows.Forms.DialogResult.Cancel)
                {
                    FileName = Path.GetFullPath(fd.FileName);
                    ShowStatus("Fiddler File: " + FileName, 2);
                    App.Configuration.FileName = FileName;

                    Requests = StressTester.ParseFiddlerSessions(FileName);
                    RenderRequests(Requests);
                }
            }
            else if (sender == tbCapture || sender == btnCapture)
            {
                var fiddlerForm = new FiddlerCapture();
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
            }
            else if (sender == btnAbout)
            {
                var splashForm = new Splash();
                splashForm.Show();
            }
            else if (sender == tbExportXml || sender == btnExportXml)
                Export("xml");
            else if (sender == tbExportJson || sender == btnExportJson)
                Export("json");
            else if (sender == tbExportHtml || sender == btnExportHtml)
                Export("html");
            else if (sender == btnExit)
                Close();

            UpdateButtonStatus();
        }
        

        void  StartProcessing()
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

        void  StartProcessing_Internal()
        {
            int time = int.Parse(tbtxtTimeToRun.Text);
            int threads = int.Parse(tbtxtThreads.Text);
            App.Configuration.LastSecondsToRun = time;
            App.Configuration.LastThreads = threads;

            ShowStatus("Parsing Fiddler Sessions...");
            Requests = StressTester.ParseFiddlerSessions(FileName) as List<HttpRequestData>;

            if (Requests == null)
            {
                ShowStatus();
                MessageBox.Show(StressTester.ErrorMessage, "Stress Tester",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            ShowStatus("Checking sites...");
            var results = StressTester.CheckAllSites(Requests, threads, time);

            ShowStatus("Parsing requests...");
            BeginInvoke(new Action<List<HttpRequestData>>(ParseResults),StressTester.Results);

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
            if (results.Count == 0)
            {
                filtered = results;
            }
            else if (cmbListDisplayMode.SelectedItem == "Errors")
            {
                filtered = results.Where(rq => rq.StatusCode == null || !rq.StatusCode.StartsWith("2")).Take(1000).ToList();

                if (filtered.Count == 0)
                {
                    cmbListDisplayMode.SelectedItem = "Success";
                    filtered = results.Where(rq => rq.StatusCode!= null && rq.StatusCode.StartsWith("2")).Take(1000).ToList();
                }
            }
            else
            {
                filtered = results.Where(rq => rq.StatusCode != null && rq.StatusCode.StartsWith("2")).Take(1000).ToList();
            }

            ListResults.BeginUpdate();
            for (int i= 0; i < filtered.Count; i++)
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

            TabSessions.SelectedTab = tabRequests;
        }


        private void StressTestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveOptions();

        }

        public void ShowStatus(string text = null, int panelId = 1, int timeout = 0)
        {
            var action = new Action<string, int>(StatusTest_Internal);            
            
            if (timeout == 0)
                Invoke(action,text,panelId);            
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
            var config = App.Configuration;
            var options = StressTester.Options;
            
            config.ReplaceCookieValue = options.ReplaceCookieValue;
            config.MaxResponseSize = options.MaxResponseSize;
            config.DelayTimeMs = options.DelayTimeMs;
            config.RandomizeRequests = options.RandomizeRequests;
            config.RequestTimeoutMs = options.RequestTimeoutMs;
            config.FileName = FileName;

            // Save any changed configuration settings
            App.Configuration.Write();
        }

        void LoadOptions()
        {
            var config = App.Configuration;
            var options = StressTester.Options;

            options.ReplaceCookieValue = config.ReplaceCookieValue;
            options.MaxResponseSize = config.MaxResponseSize;
            options.DelayTimeMs = config.DelayTimeMs;
            options.RandomizeRequests = config.RandomizeRequests;
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

            btnExport.Enabled = StressTester.Results.Count > 0;
            tbExport.Enabled = btnExport.Enabled;
        }

        private void ListResults_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item.Tag == null)
                return;
            HttpRequestData req =  e.Item.Tag as HttpRequestData;
            if (e.Item.Tag == null)
                return;

            string html = StressTester.RequestDataToHtml(req, true);

            File.WriteAllText("_preview.html", html);
            string file = (Environment.CurrentDirectory + "/_preview.html").Replace("\\", "/");            
            PreViewBrowser.Url = new Uri( file);

            TabsResult.SelectedTab = tabPreview;
        }

        private void ListRequests_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item.Tag == null)
                return;

            HttpRequestData req = e.Item.Tag as HttpRequestData;
            if (e.Item.Tag == null)
                return;

            string html = StressTester.RequestDataToHtml(req, true);

            File.WriteAllText("_preview.html", html);
            string file = (Environment.CurrentDirectory + "/_preview.html").Replace("\\", "/");
            PreViewBrowser.Url = new Uri(file);

            TabsResult.SelectedTab = tabPreview;
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
                        sb.Append(StressTester.RequestDataToHtml(req));
                    }

                    File.WriteAllText(diag.FileName, sb.ToString());
                }
            }
        }

        private void tbNoProgressEvents_CheckedChanged(object sender, EventArgs e)
        {
            var button = sender as  ToolStripButton;
            StressTester.Options.NoProgressEvents = button.Checked;
        }


    }

}
