using Newtonsoft.Json.Linq;
namespace versions.Models
{
    class VersionsMessage
    {
        public string Agent { set; get; }
        public string AutoUpdater { set; get; }
        public string Plugin { set; get; }
        public JObject Plugins { set; get; }
    }
}
