using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Arcaea.ArcUpdate;

public class ArcUpdateValue
{
    [JsonProperty("url")]  public string Url { get; set; }
    [JsonProperty("version")]  public string Version { get; set; }
}

public class  ArcUpdateRoot
{
    [JsonProperty("success")] public bool Success { get; set; }
    [JsonProperty("code")]  public string Code { get; set; }
    [JsonProperty("value")] public ArcUpdateValue Value { get; set; }
}

