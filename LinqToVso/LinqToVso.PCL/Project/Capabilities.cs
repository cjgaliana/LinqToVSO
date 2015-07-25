using Newtonsoft.Json;

namespace LinqToVso
{
    public class Capabilities
    {
        [JsonProperty("versioncontrol")]
        public Versioncontrol Versioncontrol { get; set; }

        [JsonProperty("processTemplate")]
        public Processtemplate ProcessTemplate { get; set; }
    }
}