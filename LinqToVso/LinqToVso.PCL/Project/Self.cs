using Newtonsoft.Json;

namespace LinqToVso
{
    public class Self
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}