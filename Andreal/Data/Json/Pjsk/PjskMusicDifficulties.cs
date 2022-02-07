using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk;

public class PjskMusicDifficulties
{
    [JsonProperty("musicId")] public int MusicId { get; set; }
    [JsonProperty("musicDifficulty")] public string MusicDifficulty { get; set; }
    [JsonProperty("playLevel")] public int PlayLevel { get; set; }
    [JsonProperty("noteCount")] public int NoteCount { get; set; }
}
