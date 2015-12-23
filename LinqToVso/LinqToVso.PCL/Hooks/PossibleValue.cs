using Newtonsoft.Json;

namespace LinqToVso
{
    public class PossibleValue
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("displayValue")]
        public string DisplayValue { get; set; }
    }
}