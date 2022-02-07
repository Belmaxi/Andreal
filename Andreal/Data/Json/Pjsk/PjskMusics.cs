using Newtonsoft.Json;

namespace AndrealClient.Data.Json.Pjsk;

public class PjskMusics
{
    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("categories")] public List<string> Categories { get; set; }

    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("lyricist")] public string Lyricist { get; set; }

    [JsonProperty("composer")] public string Composer { get; set; }

    [JsonProperty("arranger")] public string Arranger { get; set; }

    [JsonProperty("assetbundleName")] public string AssetbundleName { get; set; }

    [JsonProperty("publishedAt")] public long PublishedAt { get; set; }
}
