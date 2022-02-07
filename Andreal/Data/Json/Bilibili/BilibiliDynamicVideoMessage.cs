using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicVideoMessage
{
    [JsonProperty("desc")] public string Desc { get; set; }

    [JsonProperty("pic")] public string Pic { get; set; }

    [JsonProperty("title")] public string Title { get; set; }
}
