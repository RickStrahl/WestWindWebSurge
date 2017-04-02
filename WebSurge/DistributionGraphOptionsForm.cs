using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebSurge
{
    public partial class DistributionGraphOptionsForm : Form
    {
        ChartFormZed ChartForm;

        public DistributionGraphOptionsForm(ChartFormZed chartForm)
        {
            InitializeComponent();
            ChartForm = chartForm;
        }
    }
}
