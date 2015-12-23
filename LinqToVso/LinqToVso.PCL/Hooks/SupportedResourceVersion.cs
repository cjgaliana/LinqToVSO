using Newtonsoft.Json;
using System.Collections.Generic;

namespace LinqToVso
{
    public class SupportedResourceVersion
    {
        [JsonProperty("workitemcommented")]
        public List<string> Workitemcommented { get; set; }
    }
}