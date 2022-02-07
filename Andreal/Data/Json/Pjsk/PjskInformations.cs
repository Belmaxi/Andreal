using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk;

public class PjskInformations
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("informationTag")] public string InformationTag { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
}
