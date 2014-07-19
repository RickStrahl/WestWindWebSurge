using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.RazorHosting;

namespace WebSurge
{
    public class TemplateRenderer
    {

        private static Dictionary<string, string> compiledTemplates = new Dictionary<string, string>();
        private static RazorEngine<RazorTemplateBase> host = CreateHost();

        private static RazorEngine<RazorTemplateBase> CreateHost()
        {
            var host = new RazorEngine<RazorTemplateBase>();

            // add this assembly
            host.AddAssemblyFromType(typeof(ResultsParser));

            return host;
        }

        public static string RenderTemplate(string templateName, object model)
        {
            string compiledId = null;            

            if (!App.Configuration.StressTester.ReloadTemplates && 
                compiledTemplates.Keys.Contains(templateName))
                compiledId = compiledTemplates[templateName];
            else
            {
                compiledTemplates.Remove(templateName);

                string template = File.ReadAllText(App.UserDataPath + "Templates\\" + templateName);
                compiledId = host.CompileTemplate(template);

                if (compiledId == null)
                    return "<pre>" + host.ErrorMessage + "\r\n------\r\n" + host.LastGeneratedCode + "</pre>";

                compiledTemplates.Add(templateName, compiledId);
            }

            string result = host.RenderTemplateFromAssembly(compiledId, model);

            if (result == null)
                result = "<pre>" + host.ErrorMessage + "\r\n------\r\n" + host.LastGeneratedCode + "</pre>";

            return result;
        }
    }
}
