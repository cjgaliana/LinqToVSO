using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinqToVso.PCL.Hooks
{
    public class HookAction
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("consumerId")]
        public string ConsumerId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("supportedEventTypes")]
        public List<string> SupportedEventTypes { get; set; }

        [JsonProperty("supportedResourceVersions")]
        public SupportedResourceVersion SupportedResourceVersions { get; set; }

        [JsonProperty("inputDescriptors")]
        public List<InputDescriptor> InputDescriptors { get; set; }
    }
}