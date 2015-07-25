﻿using Newtonsoft.Json;

namespace LinqToVso.PCL.Team
{
    public class TeamMember
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonIgnore]
        public string ProjectId { get; set; }

        [JsonIgnore]
        public string TeamId { get; set; }
    }
}