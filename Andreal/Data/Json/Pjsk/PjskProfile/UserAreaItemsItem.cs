using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk.PjskProfile;

public class UserAreaItemsItem
{
    [JsonProperty("areaItemId")] public int AreaItemId { get; set; }

    [JsonProperty("level")] public int Level { get; set; }
}
