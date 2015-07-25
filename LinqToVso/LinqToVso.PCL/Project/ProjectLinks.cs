﻿using Newtonsoft.Json;

namespace LinqToVso
{
    public class ProjectLinks
    {
        [JsonProperty("self")]
        public Self Self { get; set; }

        [JsonProperty("collection")]
        public Collection Collection { get; set; }

        [JsonProperty("web")]
        public Web Web { get; set; }
    }
}