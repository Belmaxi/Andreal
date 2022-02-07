using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicReplyMessage
{
    [JsonProperty("item")] public BilibiliDynamicReplyItem Item { get; set; }
}
