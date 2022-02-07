using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk.PjskProfile;

public class UserChallengeLiveSoloResultsItem
{
    [JsonProperty("characterId")] public int CharacterId { get; set; }

    [JsonProperty("highScore")] public int HighScore { get; set; }
}
