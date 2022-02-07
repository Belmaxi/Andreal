using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicTextMessage
{
    [JsonProperty("item")] public BilibiliDynamicTextItem Item { get; set; }
}
