using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk.PjskProfile;

public class UserCharactersItem
{
    [JsonProperty("characterId")] public int CharacterId { get; set; }

    [JsonProperty("characterRank")] public int CharacterRank { get; set; }
}
