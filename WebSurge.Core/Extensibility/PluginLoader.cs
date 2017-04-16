using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebSurge.Extensibility
{
    public class PluginLoader
    {
        public static List<IWebSurgeExtensibility> LoadPlugIns()
        {
            var plugins = new List<IWebSurgeExtensibility>();

            var path = Path.Combine(Environment.CurrentDirectory, "plugins");
            if (!Directory.Exists(path))
                return plugins;

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
                    App.Log("Failed to load plugin from " + assembly.FullName + ".");
                }

                if (assembly == null)
                    return plugins;
                
                var pluginTypes = assembly.GetTypes()
                                      .Where(typ => typeof (IWebSurgeExtensibility).IsAssignableFrom(typ));


                foreach (var type in pluginTypes)
                {

                    IWebSurgeExtensibility plugin = null;
                    try
                    {
                        plugin = Activator.CreateInstance(type) as IWebSurgeExtensibility;
                    }
                    catch
                    {
                        App.Log("Failed to load plugin: " + type.Name + " from " + assembly.FullName + ".");
                    }
                    if (plugin != null)
                        plugins.Add(plugin);
                    else
                        App.Log("Failed to load plugin: " + type.Name + " from " + assembly.FullName + ".");
                }

            }

            return plugins;
        }
    }
}
