using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicTextItem
{
    [JsonProperty("content")] public string Content { get; set; }
}
