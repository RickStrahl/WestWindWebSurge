using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSurge.Core;

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

            if (propertyName.Equals(ReflectionExtensions.GetPropertyName((DistributionGraphSettings p) => p.BinSizeMilliseconds) , StringComparison.InvariantCultureIgnoreCase) )
            {
               if(_newSettings.BinSizeMilliseconds<=0 || _newSettings.BinSizeMilliseconds>int.MaxValue)
                {
                    errorFound = true;
                    errorMessage = string.Format("'{0}' must have a value between 1 and {1}"
                        , e.ChangedItem.PropertyDescriptor.DisplayName
                        , int.MaxValue.ToString());
                    //errorMessage = string.Concat("'", e.ChangedItem.PropertyDescriptor.DisplayName, "' must have a value between 1 and ", int.MaxValue.ToString());
                    _newSettings.BinSizeMilliseconds = Convert.ToInt32(e.OldValue);
                }
            }
            else if (propertyName.Equals(ReflectionExtensions.GetPropertyName((DistributionGraphSettings p) => p.MinX), 
                StringComparison.InvariantCultureIgnoreCase))
            {
                if (_newSettings.MinX <= 0 || _newSettings.MinX > int.MaxValue || _newSettings.MinX >= _newSettings.MaxX)
                {
                    errorFound = true;
                    string maxXDisplayName = ReflectionExtensions.GetPropertyDisplayName<DistributionGraphSettings>(i => i.MaxX);
                    errorMessage = string.Format("'{0}' must have a value between 1 and {1} and must be smaller than the value of '{2}'"
                        , e.ChangedItem.PropertyDescriptor.DisplayName
                        , int.MaxValue.ToString()
                        , maxXDisplayName);
                    //errorMessage = string.Concat("'", e.ChangedItem.PropertyDescriptor.DisplayName, "' must have a value between 1 and ",
                    //    int.MaxValue.ToString(), " and must be smaller than the value of ", maxXDisplayName);
                    _newSettings.MinX = Convert.ToInt32(e.OldValue);
                }
            }
            else if (propertyName.Equals(ReflectionExtensions.GetPropertyName((DistributionGraphSettings p) => p.MaxX), 
                StringComparison.InvariantCultureIgnoreCase))
            {
                if (_newSettings.MaxX <= 0 || _newSettings.MaxX > int.MaxValue || _newSettings.MinX >= _newSettings.MaxX)
                {
                    errorFound = true;
                    string minXDisplayName = ReflectionExtensions.GetPropertyDisplayName<DistributionGraphSettings>(i => i.MinX);
                    errorMessage = string.Format("'{0}' must have a value between 1 and {1} and must be greater than the value of '{2}'"
                        , e.ChangedItem.PropertyDescriptor.DisplayName
                        , int.MaxValue.ToString()
                        , minXDisplayName);
                    //errorMessage = string.Concat("'", e.ChangedItem.PropertyDescriptor.DisplayName, "' must have a value between 1 and ",
                    //    int.MaxValue.ToString(), " and must be greater than the value of '", minXDisplayName, "'");
                    _newSettings.MaxX = Convert.ToInt32(e.OldValue);
                }
            }
            else if (propertyName.Equals(ReflectionExtensions.GetPropertyName((DistributionGraphSettings p) => p.SmoothTension), 
                StringComparison.InvariantCultureIgnoreCase))
            {
                if (_newSettings.SmoothTension < 0 || _newSettings.SmoothTension > 1)
                {
                    errorFound = true;
                    errorMessage = string.Format("'{0}' must have a value between 0 and 1, or else it will display some really weird results.." 
                        , e.ChangedItem.PropertyDescriptor.DisplayName);
                    //errorMessage = string.Concat("'", e.ChangedItem.PropertyDescriptor.DisplayName, "' must have a value between 0 and 1",
                    //    "or else it will display some really weird results..");
                    _newSettings.SmoothTension = Convert.ToInt32(e.OldValue);
                }
            }

            if (errorFound == true && errorMessage != string.Empty)
            {
                MessageBox.Show(errorMessage, "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
    }
}
