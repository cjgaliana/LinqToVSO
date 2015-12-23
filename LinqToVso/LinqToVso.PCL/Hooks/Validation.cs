using Newtonsoft.Json;

namespace LinqToVso
{
    public class Validation
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonProperty("isRequired")]
        public bool IsRequired { get; set; }

        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        [JsonProperty("maxLength")]
        public int MaxLength { get; set; }

        [JsonProperty("minLength")]
        public int MinLength { get; set; }

        [JsonProperty("patternMismatchErrorMessage")]
        public string PatternMismatchErrorMessage { get; set; }
    }
}