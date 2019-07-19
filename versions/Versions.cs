using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

using plugin;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace versions
{
    [PluginAttribute(PluginName = "Versions")]
    public class IHeartbeat : IInputPlugin
    {
        public string Execute(JObject settings)
        {
            var versions = new Models.VersionsMessage
            {
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
                foreach (String plugin in Directory.GetFiles("plugins"))
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(String.Format(plugin));
                    plugins[Path.GetFileNameWithoutExtension(plugin)] = versionInfo.FileVersion;
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