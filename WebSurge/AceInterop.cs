using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Westwind.Utilities;

namespace WebSurge
{
    [ComVisible(true)]
    public class AceInterop
    {
        //private StressTestForm WebSurgeForm;

        public AceInterop(StressTestForm webSurgeForm)
        {
            Program.WebSurgeForm = webSurgeForm;
            InitializeInterop();

            
        }


        /// <summary>
        ///  Detect when the previewer loses focus and allow updating the editor
        /// </summary>
        /// <param name="text"></param>
        /// <param name="id"></param>
        public void updatefromeditor(string text, string id)
        {
            var form = Program.WebSurgeForm;

            if (id == "RequestBodyFormatted")
            {
                form.txtRequestContent.Text = text;
                form.RequestData_Changed(null, null);
            }
            else if (id == "RequestHeaders")
            {
                form.txtRequestHeaders.Text = text;
                form.RequestData_Changed(null, null);
            }
        }

        public void navigatebrowser(string href)
        {
            ShellUtils.GoUrl(href);
        }

        public void ShowProcessingHeader(bool show)
        {
            try
            {
                Program.WebSurgeForm.PreViewBrowser.Document.InvokeScript("showProcessingHeader", new object[] { show });
            }
            catch
            {

            }
        }

        public void InitializeInterop()
        {
            try
            {
               Program.WebSurgeForm.PreViewBrowser.Document.InvokeScript("initializeInterop", new object[] {this});
            }
            catch 
            {

            }

        }

    }
}
