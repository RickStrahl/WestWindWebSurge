using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fiddler;
using Westwind.Utilities;
using Westwind.Utilities.Configuration;

namespace Kuhela
{
    public partial class FiddlerCapture : Form
    {
        private const string Separator = "------------------------------------------------------------------";
        private UrlCaptureConfiguration CaptureConfiguration { get; set; }
        public FiddlerCapture()
        {
            InitializeComponent();
            CaptureConfiguration = App.CaptureConfiguration;
        }

        private void FiddlerCapture_Load(object sender, EventArgs e)
        {
            tbIgnoreResources.Checked = CaptureConfiguration.IgnoreResources; 
            UpdateButtonStatus();
         
        }

        private void FiddlerApplication_AfterSessionComplete(Session sess)
        {
            if (sess.RequestMethod == "CONNECT")
                return;

            if (CaptureConfiguration.ProcessId > 0)
            {
                if (sess.LocalProcessID != 0 && sess.LocalProcessID != CaptureConfiguration.ProcessId)
                    return;
            }

            if (CaptureConfiguration.IgnoreResources)
            {
                string url = sess.fullUrl.ToLower();

                var extensions = CaptureConfiguration.ExtensionFilterExclusions.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                foreach(var ext in extensions)
                {
                    if (url.Contains(ext))
                        return;
                }

                var filters = CaptureConfiguration.UrlFilterExclusions.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var urlFilter in filters)
                {
                    if (url.Contains(urlFilter))
                        return;
                }

            }

            BeginInvoke(new Action<Session>((session) =>
            {
                var reqText = Encoding.UTF8.GetString(session.RequestBody);
                //var respText = Encoding.UTF8.GetString(session.ResponseBody);

                string headers = session.oRequest.headers.ToString();
                
                // replace the HTTP line
                string firstLine = sess.RequestMethod + " " + sess.fullUrl + " " + sess.oRequest.headers.HTTPVersion;
                var lines = new List<string>(StringUtils.GetLines(headers));
                lines.RemoveAt(0);
                lines.Insert(0, firstLine);
                headers = string.Join("\r\n", lines);
                

                txtCapture.AppendText(headers + "\r\n"  +                                      
                                      (!string.IsNullOrEmpty(reqText) ? reqText + "\r\n" : string.Empty) +                                      
                                      Separator + "\r\n\r\n");

                UpdateButtonStatus();
            }),sess);

        }

        void Start()
        {
            if (tbIgnoreResources.Checked)
                CaptureConfiguration.IgnoreResources = true;
            else
                CaptureConfiguration.IgnoreResources = false;

            int procId = 0;
            if (!string.IsNullOrEmpty(tbtxtProcessId.Text))
            {
                if (!int.TryParse(tbtxtProcessId.Text, out procId))
                    procId = 0;
            }
            CaptureConfiguration.ProcessId = procId;
            
            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;            
            FiddlerApplication.Startup(8888,true,true,true);
        }

        void Stop()
        {
            FiddlerApplication.AfterSessionComplete -= FiddlerApplication_AfterSessionComplete;

            if (FiddlerApplication.IsStarted())
                FiddlerApplication.Shutdown();            
        }

        private void ButtonHandler(object sender, EventArgs e)
        {
            if (sender == tbCapture)
                Start();
            else if(sender == tbStop)
                Stop();
            else if (sender == tbSave)
            {
                var diag = new SaveFileDialog()
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "txt",
                    Filter = "Text files (*.txt)|*.txt|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Save Fiddler Capture File",
                    RestoreDirectory = true
                };
                var res = diag.ShowDialog();

                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);

                    File.WriteAllText(diag.FileName, txtCapture.Text);
                }
            }
            else if (sender == tbClear)
            {
                txtCapture.Text = string.Empty;
            }

            UpdateButtonStatus();
        }

     

        private void FiddlerCapture_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }


        public void UpdateButtonStatus()
        {
            tbCapture.Enabled = !FiddlerApplication.IsStarted();
            tbStop.Enabled = !tbCapture.Enabled;
            tbSave.Enabled = txtCapture.Text.Length > 0;
            tbClear.Enabled = tbSave.Enabled;

            CaptureConfiguration.IgnoreResources = tbIgnoreResources.Checked;
        }

    }

  
}
