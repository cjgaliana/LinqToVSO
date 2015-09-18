using LinqToVso.PCL.Team;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LinqToVso.PCL.TeamRoom
{
    public class TeamRoom
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("lastActivity")]
        public DateTime LastActivity { get; set; }

        [JsonProperty("createdBy")]
        public TeamMember CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("hasAdminPermissions")]
        public bool HasAdminPermissions { get; set; }

        [JsonProperty("hasReadWritePermissions")]
        public bool HasReadWritePermissions { get; set; }

        [JsonIgnore]
        public IList<TeamMember> Members { get; set; }
    }
}