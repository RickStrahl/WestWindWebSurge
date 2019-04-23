using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebSurge.Extensibility
{
    public class AddinLoader
    {
        public static List<IWebSurgeExtensibility> LoadAddins()
        {
            var addins = new List<IWebSurgeExtensibility>();

            var path = Path.Combine(Environment.CurrentDirectory, "Addins");
            if (!Directory.Exists(path))
                return addins;

            var files = Directory.GetFiles(path, "*.dll");

            foreach (var file in files)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(file);
                }
                catch
                {
                    App.Log("Failed to load addin from " + assembly.FullName + ".");
                }

                if (assembly == null)
                    return addins;
                
                var addinTypes = assembly.GetTypes()
                                      .Where(typ => typeof (IWebSurgeExtensibility).IsAssignableFrom(typ));


                foreach (var type in addinTypes)
                {

                    IWebSurgeExtensibility addin = null;
                    try
                    {
                        addin = Activator.CreateInstance(type) as IWebSurgeExtensibility;
                    }
                    catch
                    {
                        App.Log("Failed to load addin: " + type.Name + " from " + assembly.FullName + ".");
                    }
                    if (addin != null)
                        addins.Add(addin);
                    else
                        App.Log("Failed to load addin: " + type.Name + " from " + assembly.FullName + ".");
                }

            }

            return addins;
        }
    }
}
