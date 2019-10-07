using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

using plugin;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

namespace versions
{
    [PluginAttribute(PluginName = "Versions")]
    public class IVersions : IInputPlugin
    {
        public event EventHandler<MessageEventArgs> MessageEvent;

        public string Execute(JObject settings)
        {
            var versions = new Models.VersionsMessage
            {
                HostName = System.Environment.MachineName,
                Agent = GetAgentVersion(),
                AutoUpdater = GetAutoUpdaterVersion(),
                Plugin = GetPluginVersion(),
                Plugins = GetPluginsVersions()
            };

            return JsonConvert.SerializeObject(versions);
        }

        private string GetAgentVersion()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(@"winagent.exe");

            return versionInfo.FileVersion;
        }

        private string GetAutoUpdaterVersion()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(@"winagent-updater.exe");

            return versionInfo.FileVersion;
        }

        private string GetPluginVersion()
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(@"plugin.dll");

            return versionInfo.FileVersion;
        }

        private JObject GetPluginsVersions()
        {
            var plugins = new JObject();
            try
            {
                // Get the assemblies which have at least(t>0) one type that defines PluginAttribute
                IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => (assembly.GetTypes()
                    .Where(type => type.IsDefined(typeof(PluginAttribute), false) == true)).Count() > 0
                );


                foreach (Assembly plugin in assemblies)
                {
                    plugins[plugin.GetName().Name] = plugin.GetName().Version.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
            
            return plugins;
        }

    }
}