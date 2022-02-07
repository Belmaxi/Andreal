using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Osu;

[Serializable]
public class OsuBeatMapInfo
{
    [JsonProperty("beatmapset_id")] public string BeatmapsetId { get; set; }

    [JsonProperty("diff_size")] public string DiffSize { get; set; }

    [JsonProperty("diff_overall")] public string DiffOverall { get; set; }

    [JsonProperty("diff_approach")] public string DiffApproach { get; set; }

    [JsonProperty("diff_drain")] public string DiffDrain { get; set; }

    [JsonProperty("artist")] public string Artist { get; set; }

    [JsonProperty("artist_unicode")] public string ArtistUnicode { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("title_unicode")] public string TitleUnicode { get; set; }

    [JsonProperty("difficultyrating")] public double Difficultyrating { get; set; }
}
