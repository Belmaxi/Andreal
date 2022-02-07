using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bandori;

[Serializable]
public class BandoriRoomInfo
{
    [JsonProperty("response")] public List<ResponseItem> Response { get; set; }
}
