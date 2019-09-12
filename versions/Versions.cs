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
                var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => (assembly.GetTypes()
                    .Select(type => type.IsDefined(typeof(PluginAttribute), false))
                    .FirstOrDefault()
                ));

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