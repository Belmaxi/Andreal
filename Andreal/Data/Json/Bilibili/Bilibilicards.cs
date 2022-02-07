using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class Bilibilicards
{
    [JsonProperty("desc")] public BilibiliDesc Desc { get; set; }

    [JsonProperty("card")] public string Card { get; set; }
}
