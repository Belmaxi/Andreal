using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk.PjskProfile;

public class UserChallengeLiveSoloStagesItem
{
    [JsonProperty("challengeLiveStageType")]
    public string ChallengeLiveStageType { get; set; }

    [JsonProperty("characterId")] public int CharacterId { get; set; }

    [JsonProperty("challengeLiveStageId")] public int ChallengeLiveStageId { get; set; }

    [JsonProperty("rank")] public int Rank { get; set; }
}
