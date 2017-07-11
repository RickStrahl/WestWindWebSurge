using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Westwind.Utilities;

namespace WebSurge
{
    public partial class Splash : Form
    {
        bool Startup = false;
        public StressTestForm StressForm = null;

        public Splash(bool startup = false)
        {
            InitializeComponent();
            Startup = startup;

            Height = 305;            

            if (!startup && !UnlockKey.Unlocked)
                Height = lnkRegister.Top + lnkRegister.Height + 20;
            if (startup)
                lblClickClose.Visible = false;

            FormClosed += Splash_FormClosed;
        }

        private void Splash_FormClosed(object sender, FormClosedEventArgs e)
        {

            if (StressForm != null && StressForm.Visible)
                StressForm.Invoke(new Action(() =>
                {
                    if (StressForm != null)
                    {
                        StressForm.TopMost = true;
                        Application.DoEvents();
                        StressForm.TopMost = false;
                    }
                }));

            StressForm = null;            
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            if (Startup)
                StartupTimer.Enabled = true;

            string bitNess = Environment.Is64BitProcess ? "64 bit" : "32 bit";
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            lblVersionText.Text = "Build " + v.Major + "." + v.Minor + " " + bitNess ;

            var reg = UnlockKey.RegType;
            if (reg == RegTypes.Free)
                lblRegisterType.Text = "Free";
            else if (UnlockKey.RegType == RegTypes.Professional)
                lblRegisterType.Text = "Professional";

            Top -= 50;
            TopMost = true;
        }

        private void PictureLogo_Click(object sender, EventArgs e)
        {
            Close();
        }


  
        private void StartupTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void Splash_KeyPress(object sender, KeyPressEventArgs e)
        {
            Close();
        }

        public new void Close()
        {
            TopMost = false;
            FadeOut();            
        }

        bool InFadeOut = false;
        void FadeOut()
        {
            if (InFadeOut)
                return;

            if (SystemInformation.TerminalServerSession)
            {
                if (Startup)
                    Application.ExitThread();

                StressForm = null;
                base.Close();
                return;
            }

            int duration = 500;//in milliseconds
            int steps = 50;
            Timer timer = new Timer();
            timer.Interval = duration / steps;
            timer.Enabled = true;

            int currentStep = steps;
            timer.Tick += (arg1, arg2) =>
            {
                Opacity = ((double)currentStep) / steps;
                currentStep--;

                if (currentStep <= 0)
                {
                    timer.Stop();
                    timer.Dispose();                   

                    if (Startup)
                        Application.ExitThread();

                    Visible = false;

                    base.Close();

                }
            };

            timer.Start();
        }

        private void lblRegisterType_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {            
            ShellUtils.GoUrl(App.WebHomeUrl + "/pricing.aspx");
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShellUtils.GoUrl("https://store.west-wind.com/product/websurge");
            Close();
        }

        private void lnkRegister_Click(object sender, EventArgs e)
        {
            lnkRegister_LinkClicked(sender, null);
        }

        private void lblClickClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
