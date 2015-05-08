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
            var path = Path.Combine(Environment.CurrentDirectory, "plugins");
            var files = Directory.GetFiles(path, "*.dll");

            var plugins = new List<IWebSurgeExtensibility>();

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFile(file);

                var pluginTypes = assembly.GetTypes()
                                      .Where(typ => typeof (IWebSurgeExtensibility).IsAssignableFrom(typ));
                foreach (var type in pluginTypes)
                {
                    var plugin = Activator.CreateInstance(type) as IWebSurgeExtensibility;
                    if (plugin != null)
                        plugins.Add(plugin);
                    else
                        App.Log("Failed to load plugin: " + type.Name + " from " + assembly.FullName);
                }

            }

            return plugins;
        }
    }
}
