using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebSurge
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var limit = ServicePointManager.DefaultConnectionLimit;
            if (ServicePointManager.DefaultConnectionLimit < 10)
                ServicePointManager.DefaultConnectionLimit = 200;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new StressTestForm();
                  
            Thread newThread = new Thread(RunSplash);
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Name = "Splash";
            newThread.Start(mainForm);
            
            Application.Run(mainForm);
            Application.DoEvents();            
        }

        public static void RunSplash(object form = null)
        {
            // Force config to apply im
            var obj = App.AppDataPath;

            // Splash screen flag otherwise it just displays  and doesn't unload
            var splash = new Splash(true);
            splash.StressForm = form as StressTestForm;

            ((StressTestForm)form).Splash = splash;

            if (splash == null)
                splash = new Splash(true); // Splash screen flag otherwise it just displays  and doesn't unload

            splash.Visible = true;
            splash.TopMost = true;
            Application.Run(splash);
        }

    }
}
