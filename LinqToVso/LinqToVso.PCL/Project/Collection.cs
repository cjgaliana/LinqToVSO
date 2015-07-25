using Newtonsoft.Json;

namespace LinqToVso
{
    public class Collection
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}