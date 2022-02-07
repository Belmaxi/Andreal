using AndrealClient.Data.Json.Arcaea.ArcaeaLimited;
using Newtonsoft.Json;
using ThesareaClient.Data.Json;

namespace AndrealClient.Data.Api;
internal static class ArcaeaLimitedApi
{
    private static HttpClient Client;

    internal static bool Available;
    
    internal static void Init(ThesareaConfig config)
    {
        Available = !string.IsNullOrEmpty(config.LimitedToken) && !string.IsNullOrEmpty(config.LimitedApi);
        
        Client = new();
        Client.BaseAddress = new(config.LimitedApi);
        Client.DefaultRequestHeaders.Authorization = new("Bearer", config.LimitedToken);
    }
    
    private static async Task<string> GetString(string url) => await Client.GetStringAsync(url);
    
    internal static async Task<UserinfoDataItem?> Userinfo(long uid) =>
        JsonConvert.DeserializeObject<UserinfoData>(await GetString($"user/{uid:D9}"))?.Data;
    
    internal static async Task<RecordDataItem?> RecordInfo(long uid, string sid, sbyte dif) =>
        JsonConvert
            .DeserializeObject<ScoreinfoData>(await GetString($"user/{uid:D9}/score?song_id={sid}&difficulty={dif}"))
            ?.Data;
    
    internal static async Task<Best30?> Userbest30(long uid) =>
        JsonConvert.DeserializeObject<Best30>( await GetString($"user/{uid:D9}/best"));
}