using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinqToVso.PCL.Hooks
{
    public class InputDescriptor
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("inputMode")]
        public string InputMode { get; set; }

        [JsonProperty("isConfidential")]
        public bool IsConfidential { get; set; }

        [JsonProperty("useInDefaultDescription")]
        public bool UseInDefaultDescription { get; set; }

        [JsonProperty("validation")]
        public Validation Validation { get; set; }

        [JsonProperty("values")]
        public InputDescriptorValue InputDescriptorValue { get; set; }

        [JsonProperty("hasDynamicValueInformation")]
        public bool HasDynamicValueInformation { get; set; }

        [JsonProperty("dependencyInputIds")]
        public List<string> DependencyInputIds { get; set; }
    }
}