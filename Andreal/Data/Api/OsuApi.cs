using AndrealClient.Data.Json.Osu;
using Newtonsoft.Json;

namespace AndrealClient.Data.Api;

internal static class OsuApi
{
    private const string Api = "https://osu.ppy.sh/api/";

    [NonSerialized] private static readonly HttpClient Client;

    static OsuApi()
    {
        Client = new(); 
        Client.BaseAddress = new(Api);
    }
    
    private static async Task<string> GetString(string url) => await (await Client.GetAsync(url)).Content.ReadAsStringAsync();

    internal static async Task<OsuRecentInfo> RecentInfo(long uid, int osumode)
    {
        var recent
            = await GetString($"get_user_recent?k=4cc5802c9fdfaf8ae68f5e7ec6f3f4d8a70fa5f7&m={osumode}&limit=1&u={uid}");
        return recent == "[]"
            ? null
            : JsonConvert.DeserializeObject<List<OsuRecentInfo>>(recent)?[0];
    }

    internal static async Task<OsuUserinfo> Userinfo(string uid, int osumode)
    {
        var userinfo
            = await GetString($"get_user?k=4cc5802c9fdfaf8ae68f5e7ec6f3f4d8a70fa5f7&u={uid}&m={osumode}");
        return userinfo == "[]"
            ? null
            : JsonConvert.DeserializeObject<List<OsuUserinfo>>(userinfo)?[0];
    }

    internal static async Task<OsuBeatMapInfo> BeatMapInfo(string beatmapid) =>
        JsonConvert
            .DeserializeObject<
                List<OsuBeatMapInfo>>(await GetString($"get_beatmaps?k=4cc5802c9fdfaf8ae68f5e7ec6f3f4d8a70fa5f7&b={beatmapid}"))
            ?[0];
}
