using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Arcaea.Songlist;

#pragma warning disable CS8618

[Serializable]
public class TitleLocalized
{
    [JsonProperty("en")] public string En { get; set; }

    [JsonProperty("ja")] public string Ja { get; set; }
}
