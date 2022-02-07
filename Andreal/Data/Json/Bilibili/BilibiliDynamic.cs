using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamic
{
    [JsonProperty("data")] public Bilibilidata Data { get; set; }
}
