using Newtonsoft.Json;

namespace LinqToVso
{
    public class Processtemplate
    {
        [JsonProperty("templateName")]
        public string TemplateName { get; set; }
    }
}