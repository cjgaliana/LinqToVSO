using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinqToVso.PCL.Hooks
{
    public class Hook
    {
        [JsonIgnore]
        public HookType Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("informationUrl")]
        public string InformationUrl { get; set; }

        [JsonProperty("authenticationType")]
        public string AuthenticationType { get; set; }

        [JsonProperty("inputDescriptors")]
        public List<InputDescriptor> InputDescriptors { get; set; }

        [JsonProperty("actions")]
        public List<HookAction> Actions { get; set; }
    }
}