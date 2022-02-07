using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class Bilibilidata
{
    [JsonProperty("cards")] public List<Bilibilicards> Cards { get; set; }
}
