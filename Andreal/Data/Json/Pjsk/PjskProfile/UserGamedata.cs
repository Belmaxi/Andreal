using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk.PjskProfile;

public class UserGamedata
{
    [JsonProperty("userId")] public long UserId { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("deck")] public int Deck { get; set; }

    [JsonProperty("rank")] public int Rank { get; set; }
}
