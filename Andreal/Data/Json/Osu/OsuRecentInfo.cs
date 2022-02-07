using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Osu;

[Serializable]
public class OsuRecentInfo
{
    [JsonProperty("beatmap_id")] public string BeatmapId { get; set; }

    [JsonProperty("score")] public string Score { get; set; }

    [JsonProperty("maxcombo")] public string Maxcombo { get; set; }

    [JsonProperty("count50")] public string Count50 { get; set; }

    [JsonProperty("count100")] public string Count100 { get; set; }

    [JsonProperty("count300")] public string Count300 { get; set; }

    [JsonProperty("countmiss")] public string Countmiss { get; set; }

    [JsonProperty("countkatu")] public string Countkatu { get; set; }

    [JsonProperty("countgeki")] public string Countgeki { get; set; }

    [JsonProperty("enabled_mods")] public int EnabledMods { get; set; }

    [JsonProperty("date")] public string Date { get; set; }

    [JsonProperty("rank")] public string Rank { get; set; }
}
