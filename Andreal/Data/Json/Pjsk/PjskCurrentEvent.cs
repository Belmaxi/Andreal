using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk;

public class PjskCurrentEventItem
{
    [JsonProperty("id")] public int EventId { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("assetbundleName")] public string AssetbundleName { get; set; }
}

public class PjskCurrentEvent
{
    [JsonProperty("eventJson")] public PjskCurrentEventItem EventJson { get; set; }
}
