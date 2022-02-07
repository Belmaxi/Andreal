using AndrealClient.AndreaMessage;
using AndrealClient.Data.Json.Arcaea.BotArcApi;
using Newtonsoft.Json;
using ThesareaClient.Data.Json;

namespace AndrealClient.Data.Api;

internal static class ArcaeaUnlimitedApi
{
    private static HttpClient Client;

    internal static void Init(ThesareaConfig config)
    {
        Client = new();
        Client.BaseAddress = new(config.UnlimitedApi);
        Client.DefaultRequestHeaders.Add("User-Agent", config.UnlimitedToken);
        Client.Timeout = TimeSpan.FromMinutes(10);
    }

    private static async Task<string> GetString(string url) =>
        await (await Client.GetAsync(url)).Content.ReadAsStringAsync();

    private static async Task GetStream(string url, Core.Path filename)
    {
        await using var fileStream
            = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        await (await Client.GetAsync(url)).EnsureSuccessStatusCode().Content.CopyToAsync(fileStream)
                                          .ContinueWith((_) => fileStream.Close());
    }

    internal static async Task<ResponseRoot> UserInfo(long ucode) =>
        JsonConvert.DeserializeObject<ResponseRoot>(await GetString($"user/info?usercode={ucode:D9}"))!;

    internal static async Task<ResponseRoot> UserInfo(string uname) =>
        JsonConvert.DeserializeObject<ResponseRoot>(await GetString($"user/info?user={uname}"))!;

    internal static async Task<ResponseRoot> UserBest(long ucode, string song, object dif) =>
        JsonConvert.DeserializeObject<ResponseRoot>(await
                                                        GetString($"user/best?usercode={ucode:D9}&songid={song}&difficulty={dif}"))
        !;

    internal static async Task<ResponseRoot> UserBest30(long ucode) =>
        JsonConvert.DeserializeObject<ResponseRoot>(await GetString($"user/best30?usercode={ucode:D9}"))!;

    internal static async Task<ResponseRoot> SongByAlias(string alias) =>
        JsonConvert.DeserializeObject<ResponseRoot>(await GetString($"song/info?songname={alias}"))!;

    internal static async Task<ResponseRoot> SongAlias(string alias) =>
        JsonConvert.DeserializeObject<ResponseRoot>(await GetString($"song/alias?songname={alias}"))!;

    internal static async Task<ResponseRoot> SongList() =>
        JsonConvert.DeserializeObject<ResponseRoot>(await GetString("song/list"))!;

    internal static async Task SongAssets(string filename, Core.Path pth) =>
        await GetStream($"assets/song?file={filename}", pth);

    internal static async Task CharAssets(int partner, bool awakened, Core.Path pth) =>
        await GetStream($"assets/char?partner={partner}&awakened={(awakened ? "true" : "false")}", pth);

    internal static async Task IconAssets(int partner, bool awakened, Core.Path pth) =>
        await GetStream($"assets/icon?partner={partner}&awakened={(awakened ? "true" : "false")}", pth);

    internal static TextMessage GetErrorMessage(RobotReply.RobotReply info, int status, string message)
    {
        return status switch
               {
                   -1 or -2 or -3 => info.ArcUidNotFound,
                   -4             => info.TooManyArcUid,
                   -14            => info.NoBydChart,
                   -15            => info.NotPlayedTheChart,
                   -16            => info.GotShadowBanned,
                   _              => $"{info.APIQueryFailed}({status}: {message})"
               };
    }
}
