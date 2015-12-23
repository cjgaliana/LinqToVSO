using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinqToVso
{
    public class InputDescriptorValue
    {
        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("possibleValues")]
        public List<PossibleValue> PossibleValues { get; set; }

        [JsonProperty("isLimitedToPossibleValues")]
        public bool IsLimitedToPossibleValues { get; set; }

        [JsonProperty("isReadOnly")]
        public bool IsReadOnly { get; set; }
    }
}