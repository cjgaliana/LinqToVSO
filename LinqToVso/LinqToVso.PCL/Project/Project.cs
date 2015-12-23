using Newtonsoft.Json;

namespace LinqToVso
{
    public class Project
    {
        [JsonIgnore]
        public const string CapabilitiesKey = "Capabilities";

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("state")]
        public ProjectState State { get; set; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }

        [JsonProperty("_links")]
        public ProjectLinks ProjectLinks { get; set; }

        [JsonProperty("defaultTeam")]
        public Team DefaultTeam { get; set; }
    }
}