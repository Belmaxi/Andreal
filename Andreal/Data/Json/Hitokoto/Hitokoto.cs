using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Hitokoto;

[Serializable]
public class Hitokoto
{
    [JsonProperty("hitokoto")] public string Content { get; set; }

    [JsonProperty("from")] public string From { get; set; }
}
