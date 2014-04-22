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
        public Splash()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ShellUtils.GoUrl("http://west-wind.com/kuhela");
        }

        private void Splash_Load(object sender, EventArgs e)
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            this.lblVersionText.Text = "Build " + v.Major + "." + v.Minor;

            var reg = UnlockKey.RegType;
            if (reg == RegTypes.Free)
                lblRegisterType.Text = "Free Version";
            else if (UnlockKey.RegType == RegTypes.Professional)
                lblRegisterType.Text = "Professional Version";
        }

        private void PictureLogo_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Splash_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void lblRegisterType_Click(object sender, EventArgs e)
        {
            ShellUtils.GoUrl("http://west-wind.com/websurge/pricing.aspx");
        }
    }
}
