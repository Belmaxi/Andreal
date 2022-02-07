using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicColumnMessage
{
    [JsonProperty("summary")] public string Summary { get; set; }

    [JsonProperty("title")] public string Title { get; set; }
}
