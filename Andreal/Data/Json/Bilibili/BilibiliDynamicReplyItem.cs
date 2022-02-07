using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicReplyItem
{
    [JsonProperty("content")] public string Content { get; set; }
}
