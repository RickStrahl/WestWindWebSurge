using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
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
            string propertyName = e.ChangedItem.PropertyDescriptor.Name;
            bool errorFound = false;
            string errorMesage = string.Empty;

            if (propertyName.Equals(GetPropertyName((DistributionGraphSettings p) => p.BinSizeMilliseconds) , StringComparison.InvariantCultureIgnoreCase) )
            {
               if(_newSettings.BinSizeMilliseconds<=0 || _newSettings.BinSizeMilliseconds>int.MaxValue)
                {
                    string test = string.Empty;
                }

            }


            
        }

        //based on https://handcraftsman.wordpress.com/2008/11/11/how-to-get-c-property-names-without-magic-strings/
        private string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            MemberExpression body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }

        


    }
}
