using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace WebSurge
{
    public class UrlCaptureConfiguration
    {

        [XmlIgnore]
        [JsonIgnore]
        public int ProcessId { get; set; }

        public bool IgnoreResources { get; set; }
        public string CaptureDomain { get; set; }
        public List<string> UrlFilterExclusions { get; set; }
        public List<string> ExtensionFilterExclusions { get; set; }

        public UrlCaptureConfiguration()
        {
            UrlFilterExclusions = new List<string>();
            ExtensionFilterExclusions = new List<string>();
        }
    }
}