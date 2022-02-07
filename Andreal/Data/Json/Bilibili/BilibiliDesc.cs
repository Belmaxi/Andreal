using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDesc
{
    [JsonProperty("type")] public sbyte Type { get; set; }

    [JsonProperty("dynamic_id_str")] public string DynamicId { get; set; }

    [JsonProperty("bvid")] public string Bvid { get; set; }

    [JsonProperty("rid_str")] public string Rid { get; set; }
}
