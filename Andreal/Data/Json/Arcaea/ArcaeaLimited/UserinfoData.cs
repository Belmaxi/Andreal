using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Arcaea.ArcaeaLimited;

[Serializable]
public class UserinfoData
{
    [JsonProperty("data")] public UserinfoDataItem Data { get; set; }
}
