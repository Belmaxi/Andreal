using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk.PjskProfile;

public class UserMusicResultsItem
{
    [JsonProperty("userId")] public long UserId { get; set; }

    [JsonProperty("musicId")] public int MusicId { get; set; }

    [JsonProperty("musicDifficulty")] public string MusicDifficulty { get; set; }

    [JsonProperty("playType")] public string PlayType { get; set; }

    [JsonProperty("playResult")] public string PlayResult { get; set; }

    [JsonProperty("highScore")] public int HighScore { get; set; }

    [JsonProperty("fullComboFlg")] public bool FullComboFlg { get; set; }

    [JsonProperty("fullPerfectFlg")] public bool FullPerfectFlg { get; set; }

    [JsonProperty("mvpCount")] public int MvpCount { get; set; }

    [JsonProperty("superStarCount")] public int SuperStarCount { get; set; }

    [JsonProperty("createdAt")] public long CreatedAt { get; set; }

    [JsonProperty("updatedAt")] public long UpdatedAt { get; set; }
}
