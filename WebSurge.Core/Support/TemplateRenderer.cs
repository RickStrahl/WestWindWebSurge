using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.RazorHosting;
using Westwind.Utilities.Configuration;

namespace WebSurge
{
    public class TemplateRenderer
    {

        private static Dictionary<string, string> compiledTemplates = new Dictionary<string, string>();
        private static RazorEngine<RazorTemplateBase> host = null; //CreateHost();
        private static RazorFolderHostContainer<RazorTemplateFolderHost> hostContainer = CreateHostContainer();    

        private static RazorEngine<RazorTemplateBase> CreateHost()
        {
            var host = new RazorEngine<RazorTemplateBase>();
                               
            // add this assembly
            host.AddAssemblyFromType(typeof(ResultsParser));
            host.AddAssemblyFromType(typeof (AppConfiguration));

            return host;
        }

        private static RazorFolderHostContainer<RazorTemplateFolderHost> CreateHostContainer()
        {
            var host = new RazorFolderHostContainer<RazorTemplateFolderHost>();
            host.UseAppDomain = false;
            host.TemplatePath = App.UserDataPath + "Html\\";
            
            // add this assembly
            host.AddAssemblyFromType(typeof(ResultsParser));
            host.AddAssemblyFromType(typeof(AppConfiguration)); 

            host.Start();

            return host;
        }
        
        public static string RenderTemplate(string templateName, object model)
        {            
            string result = hostContainer.RenderTemplate(templateName, model);
            if (result == null)
                result = "<pre>" + hostContainer.ErrorMessage + "\r\n------\r\n" + hostContainer.Engine.LastGeneratedCode + "</pre>";

            return result;
        }

        //public static string RenderTemplateOld(string templateName, object model)
        //{
        //    string compiledId = null;            

        //    if (!App.Configuration.StressTester.ReloadTemplates && 
        //        compiledTemplates.Keys.Contains(templateName))
        //        compiledId = compiledTemplates[templateName];
        //    else
        //    {
        //        compiledTemplates.Remove(templateName);

        //        string template = File.ReadAllText(App.UserDataPath + "Html\\" + templateName);
        //        compiledId = host.CompileTemplate(template);

        //        if (compiledId == null)
        //            return "<pre>" + host.ErrorMessage + "\r\n------\r\n" + host.LastGeneratedCode + "</pre>";

        //        compiledTemplates.Add(templateName, compiledId);
        //    }

        //    string result = host.RenderTemplateFromAssembly(compiledId, model);

        //    if (result == null)
        //        result = "<pre>" + host.ErrorMessage + "\r\n------\r\n" + host.LastGeneratedCode + "</pre>";

        //    return result;
        //}
    }
}
