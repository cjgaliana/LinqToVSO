using Newtonsoft.Json;
using System;

namespace LinqToVso.PCL.Subscriptions
{
    public class Subscription
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("resourceVersion")]
        public object ResourceVersion { get; set; }

        [JsonProperty("eventDescription")]
        public string EventDescription { get; set; }

        [JsonProperty("consumerId")]
        public string ConsumerId { get; set; }

        [JsonProperty("consumerActionId")]
        public string ConsumerActionId { get; set; }

        [JsonProperty("actionDescription")]
        public string ActionDescription { get; set; }

        [JsonProperty("createdBy")]
        public Createdby CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("modifiedBy")]
        public Modifiedby ModifiedBy { get; set; }

        [JsonProperty("modifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [JsonProperty("publisherInputs")]
        public Publisherinputs PublisherInputs { get; set; }

        [JsonProperty("consumerInputs")]
        public Consumerinputs ConsumerInputs { get; set; }
    }

    public class Createdby
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Modifiedby
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class Publisherinputs
    {
        [JsonProperty("buildStatus")]
        public string BuildStatus { get; set; }

        [JsonProperty("definitionName")]
        public string DefinitionName { get; set; }

        [JsonProperty("hostId")]
        public string HostId { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("tfsSubscriptionId")]
        public string TfsSubscriptionId { get; set; }
    }

    public class Consumerinputs
    {
        [JsonProperty("feedId")]
        public string FeedId { get; set; }

        [JsonProperty("packageSourceId")]
        public string PackageSourceId { get; set; }
    }
}