using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk;

public class PjskCurrentEventPredict
{
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("data")] public Dictionary<string, long> Data { get; set; }
}
