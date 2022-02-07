using Newtonsoft.Json;

namespace AndrealClient.Data.Json;

public class ThesareaConfig
{
    [JsonProperty("limitedapiurl")] public string LimitedApi { get; set; }
    [JsonProperty("limitedtoken")] public string LimitedToken { get; set; }
    [JsonProperty("unlimitedapiurl")]  public string UnlimitedApi { get; set; }
    [JsonProperty("unlimitedtoken")] public string UnlimitedToken { get; set; }
}
