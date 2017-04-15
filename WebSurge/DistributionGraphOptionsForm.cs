using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebSurge
{
    public partial class DistributionGraphOptionsForm : Form
    {
        IDistributionGraphContainer _chartForm;
        DistributionGraphSettings _newSettings;

        public DistributionGraphOptionsForm(IDistributionGraphContainer chartForm, DistributionGraphSettings settings)
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
            string errorMessage = string.Empty;

            if (propertyName.Equals(GetPropertyName((DistributionGraphSettings p) => p.BinSizeMilliseconds) , StringComparison.InvariantCultureIgnoreCase) )
            {
               if(_newSettings.BinSizeMilliseconds<=0 || _newSettings.BinSizeMilliseconds>int.MaxValue)
                {
                    errorFound = true;
                    errorMessage = string.Concat("'", e.ChangedItem.PropertyDescriptor.DisplayName, "' must have a value between 1 and ", int.MaxValue.ToString());
                    _newSettings.BinSizeMilliseconds = Convert.ToInt32(e.OldValue);
                }
            }
            else if (propertyName.Equals(GetPropertyName((DistributionGraphSettings p) => p.MinX), StringComparison.InvariantCultureIgnoreCase))
            {
                if (_newSettings.MinX <= 0 || _newSettings.MinX > int.MaxValue || _newSettings.MinX >= _newSettings.MaxX)
                {
                    errorFound = true;
                    string maxXDisplayName = "test";//from i in e.ChangedItem.  where i.
                    errorMessage = string.Concat("'", e.ChangedItem.PropertyDescriptor.DisplayName, "' must have a value between 1 and ",
                        int.MaxValue.ToString(), " and must be smaller than the value of ", maxXDisplayName);
                    _newSettings.MinX = Convert.ToInt32(e.OldValue);
                }
            }


            if (errorFound == true && errorMessage != string.Empty)
            {
                MessageBox.Show(errorMessage, "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
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
