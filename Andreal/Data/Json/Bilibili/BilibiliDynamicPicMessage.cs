using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicPicMessage
{
    [JsonProperty("item")] public BilibiliDynamicPicItem Item { get; set; }
}
