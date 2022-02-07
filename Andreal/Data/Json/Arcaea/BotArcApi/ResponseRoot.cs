using AndrealClient.Data.Json.Arcaea.Songlist;
using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Arcaea.BotArcApi;

public class ResponseRoot
{
    [JsonProperty("status")] public int Status { get; set; }

    [JsonProperty("message")] public string Message { get; set; }

    [JsonProperty("content")] public dynamic Content { get; set; }

    internal T DeserializeContent<T>() => JsonConvert.DeserializeObject<T>(Content.ToString());
}

public class AccountInfo
{
    [JsonProperty("code")] public int Code { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("user_id")] public int UserID { get; set; }
    [JsonProperty("is_mutual")] public bool IsMutual { get; set; }

    [JsonProperty("is_char_uncapped_override")]
    public bool IsCharUncappedOverride { get; set; }

    [JsonProperty("is_char_uncapped")] public bool IsCharUncapped { get; set; }
    [JsonProperty("is_skill_sealed")] public bool IsSkillSealed { get; set; }
    [JsonProperty("rating")] public short Rating { get; set; }
    [JsonProperty("character")] public int Character { get; set; }
}

public class ArcSongdata
{
    [JsonProperty("song_id")] public string SongId { get; set; }

    [JsonProperty("difficulty")] public sbyte Difficulty { get; set; }

    [JsonProperty("score")] public string Score { get; set; }

    [JsonProperty("shiny_perfect_count")] public string MaxPure { get; set; }

    [JsonProperty("perfect_count")] public string Pure { get; set; }

    [JsonProperty("near_count")] public string Far { get; set; }

    [JsonProperty("miss_count")] public string Lost { get; set; }

    [JsonProperty("time_played")] public long TimePlayed { get; set; }

    [JsonProperty("clear_type")] public sbyte ClearType { get; set; }

    [JsonProperty("rating")] public double Rating { get; set; }
}

public class UserInfoContent
{
    [JsonProperty("account_info")] public AccountInfo AccountInfo { get; set; }
    [JsonProperty("recent_score")] public List<ArcSongdata> RecentScore { get; set; }
}

public class UserBest30Content
{
    [JsonProperty("best30_avg")] public double Best30Avg { get; set; }
    [JsonProperty("recent10_avg")] public double Recent10Avg { get; set; }
    [JsonProperty("account_info")] public AccountInfo AccountInfo { get; set; }
    [JsonProperty("best30_list")] public List<ArcSongdata> Best30List { get; set; }
}

public class UserBestContent
{
    [JsonProperty("account_info")] public AccountInfo AccountInfo { get; set; }
    [JsonProperty("record")] public ArcSongdata Record { get; set; }
}

public class SongListContent
{
    [JsonProperty("songs")] public List<SongsItem> Songs { get; set; }
}

public class TooManySongsContent
{
    [JsonProperty("songs")] public List<string> Songs { get; set; }
}