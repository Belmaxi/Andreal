using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk;

public class PjskRankings
{
    [JsonProperty("userId")] public long UserId { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("score")] public int Score { get; set; }

    [JsonProperty("rank")] public int Rank { get; set; }
}

public class PjskEventUserRanking
{
    [JsonProperty("rankings")] public List<PjskRankings> Rankings { get; set; }
}
