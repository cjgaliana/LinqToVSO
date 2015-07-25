using Newtonsoft.Json;

namespace LinqToVso
{
    public class Web
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}