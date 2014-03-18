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
                lblStatusFilename.Text = "Fiddler File: " + FileName;

            txtThreads.Text = App.Configuration.LastThreads.ToString();
            txtTimeToRun.Text = App.Configuration.LastSecondsToRun.ToString();

            LoadOptions();

            OptionsPropertyGrid.SelectedObject = StressTester.Options;

            cmbListDisplayMode.SelectedItem = "Errors";

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

        void StressTester_RequestProcessed(HttpRequestData req)
        {
            Invoke(new Action<HttpRequestData>(ShowRequestProcessed), req);
        }
        void ShowRequestProcessed(HttpRequestData req)
        {            
            string output = req.StatusCode + " -  " + req.HttpVerb + " " + req.Url + "(" + req.TimeTakenMs.ToString("n0") + "ms)\r\n";

            if (txtConsole.Text.Length > 50000)
            {
                lock (ConsoleLog)
                {
                    if (txtConsole.Text.Length <= 50000)
                    {
                        var txt = txtConsole.Text.Substring(10000);
                        txtConsole.Text = "";
                        output = txt.Substring(txt.IndexOf("\r\n") + 2) + output;
                    }
                }
            }

            txtConsole.AppendText(output);
            Application.DoEvents();
        }


        private void ButtonHandler_Click(object sender, EventArgs e)
        {

            if (sender == tbOpen || sender == txtProcessingTime)
            {
                var fd = new OpenFileDialog()
                {
                    DefaultExt = ".txt;.log",
                    CheckFileExists = true,
                    RestoreDirectory = true,
                    FileName = "1_Full.txt"
                };
                var dr = fd.ShowDialog();
                if (dr != System.Windows.Forms.DialogResult.Cancel)
                {
                    FileName = Path.GetFullPath(fd.FileName);
                    ShowStatus("Fiddler File: " + FileName, 2);
                    App.Configuration.FileName = FileName;
                }
            }
            else if (sender == tbFiddlerCapture)
            {
                var fiddlerForm = new FiddlerCapture();
                fiddlerForm.Show();
            }
            else if (sender == tbStart)
            {
                StartProcessing();
            }
            else if (sender == tbStop)
            {
                StressTester.CancelThreads = true;
                ShowStatus("Stopping request processing...");
            }
            else if (sender == tbEditFile)
            {
                if (!string.IsNullOrEmpty(FileName))
                    ShellUtils.GoUrl(FileName);
            }

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
            int time = int.Parse(txtTimeToRun.Text);
            int threads = int.Parse(txtThreads.Text);
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

            ShowStatus("Parsing results...");
            Invoke(new Action<List<HttpRequestData>>(ParseResults),results);

            ShowStatus("Done.");

            Invoke(new Action(UpdateButtonStatus));
        }

        private void ParseResults(List<HttpRequestData> results)
        {

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
            tbEditFile.Enabled = !string.IsNullOrEmpty(FileName);
            tbStart.Enabled = !StressTester.Running && !string.IsNullOrEmpty(FileName);
            tbStop.Enabled = StressTester.Running;
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

    }

}
