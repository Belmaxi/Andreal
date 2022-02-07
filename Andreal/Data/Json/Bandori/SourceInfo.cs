using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bandori;

[Serializable]
public class SourceInfo
{
    [JsonProperty("name")] public string Name { get; set; }
}
