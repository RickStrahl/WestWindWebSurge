using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace WebSurge
{
    public class UrlCaptureConfiguration
    {

        [XmlIgnore]
        [JsonIgnore]
        [Browsable(false)]
        public int ProcessId { get; set; }

        public int ProxyPort { get; set; }
        public bool IgnoreResources { get; set; }
        public string CaptureDomain { get; set; }
        public List<string> UrlFilterExclusions { get; set; }
        public List<string> ExtensionFilterExclusions { get; set; }
                
        [Browsable(false)]
        public string Cert { get; set; }

        [Browsable(false)]
        public string Key { get; set; }
        

        public UrlCaptureConfiguration()
        {
            ProxyPort = 8888;
            UrlFilterExclusions = new List<string>();
            ExtensionFilterExclusions = new List<string>();
        }
    }
}