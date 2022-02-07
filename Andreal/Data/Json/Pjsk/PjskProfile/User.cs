using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk.PjskProfile;

public class User
{
    [JsonProperty("userGamedata")] public UserGamedata UserGamedata { get; set; }
}
