using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Arcaea.ArcaeaLimited;

[Serializable]
public class UserinfoDataItem
{
    [JsonProperty("display_name")] public string DisplayName { get; set; }
    [JsonProperty("potential")] public short? Potential { get; set; } = -1;
    [JsonProperty("partner")] public Partner Partner { get; set; }
    [JsonProperty("last_played_song")] public RecordDataItem LastPlayedSong { get; set; }
}
