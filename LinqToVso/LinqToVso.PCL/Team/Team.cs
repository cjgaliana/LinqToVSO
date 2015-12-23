// Camilo Galiana
// LinqToVso.PCL
// Team.cs
// 19 / 07 / 2015

using Newtonsoft.Json;

namespace LinqToVso
{
    public class Team
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("identityUrl")]
        public string IdentityUrl { get; set; }

        [JsonIgnore]
        public string ProjectId { get; set; }
    }
}