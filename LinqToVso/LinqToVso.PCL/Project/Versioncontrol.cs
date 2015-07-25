using Newtonsoft.Json;

namespace LinqToVso
{
    public enum SourceControlType
    {
        Git,
        TfVc
    }
    public class Versioncontrol
    {
        [JsonProperty("sourceControlType")]
        public SourceControlType SourceControlType { get; set; }
    }
}