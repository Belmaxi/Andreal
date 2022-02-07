using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Arcaea.ArcaeaLimited;

[Serializable]
public class ScoreinfoData
{
    [JsonProperty("data")] public RecordDataItem Data { get; set; }
}
