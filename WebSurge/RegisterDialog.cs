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
    public partial class RegisterDialog : Form
    {
        bool Startup = false;
        public StressTestForm StressForm = null;

        public RegisterDialog(bool startup = false)
        {
            InitializeComponent();
            Startup = startup;
        }


        private void Splash_Load(object sender, EventArgs e)
        {
            var accessCount = App.Configuration.WindowSettings.Accesses;

            if (accessCount > 200)
                txtUsed.ForeColor= Color.Red;
            else if (accessCount > 120)
                txtUsed.ForeColor = Color.Firebrick;
            else if (accessCount > 50)
                txtUsed.ForeColor = Color.Orange;
            else if (accessCount > 10)
                txtUsed.ForeColor = Color.Green;

            txtUsed.Text = $"{accessCount} times.";

            Top -= 50;
            TopMost = true;
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
                    base.Close();

                }
            };

            timer.Start();
        }


        private void lblClickClose_Click(object sender, EventArgs e)
        {
            Close();
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
    }
}
