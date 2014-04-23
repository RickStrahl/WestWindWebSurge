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
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ShellUtils.GoUrl("http://west-wind.com/kuhela");
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            if (Startup)
                StartupTimer.Enabled = true;

            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            lblVersionText.Text = "Build " + v.Major + "." + v.Minor;

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


        private void lblRegisterType_Click(object sender, EventArgs e)
        {
            ShellUtils.GoUrl("http://west-wind.com/websurge/pricing.aspx");
        }

        private void StartupTimer_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void Splash_KeyPress(object sender, KeyPressEventArgs e)
        {
            Close();
        }

        public void Close()
        {
            FadeOut();            
        }

        bool InFadeOut;
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
    }
}
