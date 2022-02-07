using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Bilibili;

[Serializable]
public class BilibiliDynamicPictures
{
    [JsonProperty("img_src")] public string ImgSrc { get; set; }
}
