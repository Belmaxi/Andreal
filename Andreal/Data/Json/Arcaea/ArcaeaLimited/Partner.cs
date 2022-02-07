using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Arcaea.ArcaeaLimited;

[Serializable]
public class Partner
{
    [JsonProperty("partner_id")] public int PartnerId { get; set; }
    [JsonProperty("is_awakened")] public bool IsAwakened { get; set; }
}
