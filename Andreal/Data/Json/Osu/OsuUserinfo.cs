using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Osu;

[Serializable]
public class OsuUserinfo
{
    [JsonProperty("user_id")] public string UserId { get; set; }

    [JsonProperty("username")] public string Username { get; set; }

    [JsonProperty("pp_raw")] public double? PpRaw { get; set; } = 0;

    [JsonProperty("country")] public string Country { get; set; }

    [JsonProperty("pp_country_rank")] public string PpCountryRank { get; set; }
}
