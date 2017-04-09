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
        ChartFormZed _chartForm;
        DistributionGraphSettings _newSettings;

        public DistributionGraphOptionsForm(ChartFormZed chartForm, DistributionGraphSettings settings)
        {
            InitializeComponent();
            _chartForm = chartForm;
            _newSettings = settings;
            pgProperties.SelectedObject = settings;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            _chartForm.RenderResponseTimeDistribution(_newSettings);
        }

        private void pgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            string test = string.Empty;

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(_newSettings);


            //if (e.ChangedItem.PropertyDescriptor == properties.Find( System.Reflection.GetMemberInfo(_newSettings.MinX) ., false);
        }
    }
}
