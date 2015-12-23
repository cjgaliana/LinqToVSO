using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinqToVso
{
    public class SupportedEvent
    {
        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("supportedResourceVersions")]
        public List<string> SupportedResourceVersions { get; set; }

        [JsonProperty("inputDescriptors")]
        public List<InputDescriptor> InputDescriptors { get; set; }
    }
}