using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Fiddler;

namespace WebSurge
{
    public partial class FiddlerCapture : Form
    {
        private const string Separator = "------------------------------------------------------------------";
        private UrlCaptureConfiguration CaptureConfiguration { get; set; }
        private StressTestForm MainForm;
        

    public FiddlerCapture(StressTestForm form)
    {
        InitializeComponent();
        CaptureConfiguration = App.Configuration.UrlCapture;
        MainForm = form;

        // Future use (Win7+): No dependencies on certmaker and bouncy castle libs
        // FiddlerApplication.Prefs.SetBoolPref("fiddler.certmaker.PreferCertEnroll", true);

        // IF PROBLEMS WITH SSL NOT WORKING
        // Delete the C:\Users\rstrahl\AppData\Roaming\Microsoft\Crypto\RSA folder

        if (!string.IsNullOrEmpty(App.Configuration.UrlCapture.Cert))
        {
            FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.key", App.Configuration.UrlCapture.Key);
            FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.cert", App.Configuration.UrlCapture.Cert);
        }
    }

        private void FiddlerCapture_Load(object sender, EventArgs e)
        {
            tbIgnoreResources.Checked = CaptureConfiguration.IgnoreResources;
            txtCaptureDomain.Text = CaptureConfiguration.CaptureDomain;

            UpdateButtonStatus();

            try
            {
                var processes = Process.GetProcesses().OrderBy(p => p.ProcessName);
                foreach (var process in processes)
                {
                    txtProcessId.Items.Add(process.ProcessName + "  - " + process.Id);
                }
            }
            catch { }

        }

        private void FiddlerApplication_AfterSessionComplete(Session sess)
        {
            // Ignore HTTPS connect requests
            if (sess.RequestMethod == "CONNECT")
                return;

            if (CaptureConfiguration.ProcessId > 0)
            {
                if (sess.LocalProcessID != 0 && sess.LocalProcessID != CaptureConfiguration.ProcessId)
                    return;
            }

            if (!string.IsNullOrEmpty(CaptureConfiguration.CaptureDomain))
            {
                if (sess.hostname.ToLower() != CaptureConfiguration.CaptureDomain.Trim().ToLower())
                    return;
            }

            if (CaptureConfiguration.IgnoreResources)
            {
                string url = sess.fullUrl.ToLower();

                var extensions = CaptureConfiguration.ExtensionFilterExclusions;
                foreach (var ext in extensions)
                {
                    if (url.Contains(ext))
                        return;
                }

                var filters = CaptureConfiguration.UrlFilterExclusions;
                foreach (var urlFilter in filters)
                {
                    if (url.Contains(urlFilter))
                        return;
                }
            }

            if (sess == null || sess.oRequest == null || sess.oRequest.headers == null)
                return;

            string headers = sess.oRequest.headers.ToString();

            string contentType =
                sess.oRequest.headers.Where(hd => hd.Name.ToLower() == "content-type")
                    .Select(hd => hd.Name)
                    .FirstOrDefault();

            string reqBody = null;
            if (sess.RequestBody.Length > 0)
            {

                if (sess.requestBodyBytes.Contains((byte) 0) || contentType.StartsWith("image/"))
                    reqBody = "b64_" + Convert.ToBase64String(sess.requestBodyBytes);
                else
                {                    
                    //reqBody = Encoding.Default.GetString(sess.ResponseBody);
                    reqBody = sess.GetRequestBodyAsString();                    
                }
            }
            
            // if you wanted to capture the response
            //string respHeaders = session.oResponse.headers.ToString();
            //var respBody = Encoding.UTF8.GetString(session.ResponseBody);
            
            // replace the HTTP line to inject full URL
            string firstLine = sess.RequestMethod + " " + sess.fullUrl + " " + sess.oRequest.headers.HTTPVersion;
            int at = headers.IndexOf("\r\n");
            if (at < 0)
                return;
            headers = firstLine + "\r\n" + headers.Substring(at + 1);

            string output = headers + "\r\n" +
                            (!string.IsNullOrEmpty(reqBody) ? reqBody + "\r\n" : string.Empty) +
                            Separator + "\r\n\r\n";

            // must marshal and synchronize to UI thread
            BeginInvoke(new Action<string>((text) =>
            {
                try
                {
                    txtCapture.AppendText(text);
                }
                catch (Exception e)
                {
                    App.Log(e);
                }

                UpdateButtonStatus();
            }), output);
        }

        void Start()
        {
            if (tbIgnoreResources.Checked)
                CaptureConfiguration.IgnoreResources = true;
            else
                CaptureConfiguration.IgnoreResources = false;

            string strProcId = txtProcessId.Text;
            if (strProcId.Contains('-'))
                strProcId = strProcId.Substring(strProcId.IndexOf('-') + 1).Trim();

            strProcId = strProcId.Trim();

            int procId = 0;
            if (!string.IsNullOrEmpty(strProcId))
            {
                if (!int.TryParse(strProcId, out procId))
                    procId = 0;
            }
            CaptureConfiguration.ProcessId = procId;
            CaptureConfiguration.CaptureDomain = txtCaptureDomain.Text;

            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;
            //FiddlerApplication.Startup( App.Configuration.UrlCapture.ProxyPort, true, true, true);

            const FiddlerCoreStartupFlags flags =
                FiddlerCoreStartupFlags.AllowRemoteClients |
                FiddlerCoreStartupFlags.CaptureLocalhostTraffic |
                FiddlerCoreStartupFlags.DecryptSSL |
                FiddlerCoreStartupFlags.MonitorAllConnections |
                FiddlerCoreStartupFlags.RegisterAsSystemProxy;

            FiddlerApplication.Startup(App.Configuration.UrlCapture.ProxyPort, flags);



        }


        void Stop()
        {
            FiddlerApplication.AfterSessionComplete -= FiddlerApplication_AfterSessionComplete;

            if (FiddlerApplication.IsStarted())
                FiddlerApplication.Shutdown();
        }



        public static bool InstallCertificate()
        {
            if (!CertMaker.rootCertExists())
            {
                if (!CertMaker.createRootCert())
                    return false;

                if (!CertMaker.trustRootCert())
                    return false;

                App.Configuration.UrlCapture.Cert = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.cert", null);
                App.Configuration.UrlCapture.Key = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.key", null);
            }

            return true;
        }

        public static bool UninstallCertificate()
        {
            if (CertMaker.rootCertExists())
            {
                if (!CertMaker.removeFiddlerGeneratedCerts(true))
                    return false;
            }
            App.Configuration.UrlCapture.Cert = null;
            App.Configuration.UrlCapture.Key = null;
            return true;
        }

        private void ButtonHandler(object sender, EventArgs e)
        {
            if (sender == tbCapture)
                Start();
            else if (sender == tbStop)
                Stop();
            else if (sender == tbSave)
            {
                var diag = new SaveFileDialog()
                {
                    AutoUpgradeEnabled = true,
                    CheckPathExists = true,
                    DefaultExt = "txt",
                    Filter = "Text files (*.websurge)|*.websurge|All Files (*.*)|*.*",
                    OverwritePrompt = false,
                    Title = "Save Fiddler Capture File",
                    RestoreDirectory = true
                };
                var res = diag.ShowDialog();

                if (res == DialogResult.OK)
                {
                    if (File.Exists(diag.FileName))
                        File.Delete(diag.FileName);

                    File.WriteAllText(diag.FileName, txtCapture.Text);

                    MainForm.OpenFile(diag.FileName);
                }
            }
            else if (sender == tbClear)
            {
                txtCapture.Text = string.Empty;
            }
            else if (sender == btnInstallSslCert)
            {
                Cursor = Cursors.WaitCursor;
                InstallCertificate();
                Cursor = Cursors.Default;
            }
            else if (sender == btnUninstallSslCert)
                UninstallCertificate();

            UpdateButtonStatus();
        }

        private void FiddlerCapture_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            MainForm = null;
        }


        public void UpdateButtonStatus()
        {
            tbCapture.Enabled = !FiddlerApplication.IsStarted();
            tbStop.Enabled = !tbCapture.Enabled;
            tbSave.Enabled = txtCapture.Text.Length > 0;
            tbClear.Enabled = tbSave.Enabled;

            btnInstallSslCert.Enabled = !CertMaker.rootCertExists();
            btnUninstallSslCert.Enabled = !btnInstallSslCert.Enabled;

            CaptureConfiguration.IgnoreResources = tbIgnoreResources.Checked;
        }

    }


}
