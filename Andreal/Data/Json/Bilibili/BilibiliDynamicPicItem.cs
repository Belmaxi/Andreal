using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicPicItem
{
    [JsonProperty("description")] public string Description { get; set; }

    [JsonProperty("pictures")] public List<BilibiliDynamicPictures> Pictures { get; set; }

    [JsonProperty("pictures_count")] public sbyte PicturesCount { get; set; }
}
