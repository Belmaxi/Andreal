using AndrealClient.Data.Json.Pjsk;
using AndrealClient.Data.Json.Pjsk.PjskProfile;
using Newtonsoft.Json;

namespace AndrealClient.Data.Api;

internal static class PjskApi
{
    [NonSerialized] private static readonly HttpClient Client;

    static PjskApi()
    {
        Client = new(); 
    }
    
    private static async Task<string> GetString(string url) => await (await Client.GetAsync(url)).Content.ReadAsStringAsync();


    internal static async Task<PjskProfiles> PjskProfile(long userId) =>
        JsonConvert.DeserializeObject<PjskProfiles>(await GetString($"https://api.pjsekai.moe/api/user/{userId}/profile"));

    internal static async Task<List<PjskMusics>> PjskMusics() =>
        JsonConvert
            .DeserializeObject<
                List<PjskMusics>>(await GetString("https://sekai-world.github.io/sekai-master-db-diff/musics.json"));

    internal static async Task<List<PjskMusicDifficulties>> PjskMusicDifficulties() =>
        JsonConvert
            .DeserializeObject<
                List<PjskMusicDifficulties>>(await GetString("https://sekai-world.github.io/sekai-master-db-diff/musicDifficulties.json"));

    internal static async Task<List<PjskMusicMetas>> PjskMusicMetas() =>
        JsonConvert
            .DeserializeObject<
                List<PjskMusicMetas>>(await GetString("https://minio.dnaroma.eu/sekai-best-assets/music_metas.json"));

    internal static async Task<PjskRankings> PjskUserRanking(long userId, int eventId) =>
        JsonConvert
            .DeserializeObject<
                PjskEventUserRanking>(await GetString($"https://api.pjsekai.moe/api/user/%7Buser_id%7D/event/{eventId}/ranking?targetUserId={userId}"))
            ?.Rankings?.FirstOrDefault();

    internal static async Task<PjskRankings> PjskEventRanking(long targetRank, int eventId) =>
        JsonConvert
            .DeserializeObject<
                PjskEventUserRanking>(await GetString($"https://api.pjsekai.moe/api/user/%7Buser_id%7D/event/{eventId}/ranking?targetRank={targetRank}"))
            ?.Rankings?.FirstOrDefault();

    internal static async Task<PjskCurrentEventItem> PjskCurrentEvent() =>
        JsonConvert.DeserializeObject<PjskCurrentEvent>(await GetString("https://strapi.sekai.best/sekai-current-event"))
                   ?.EventJson;

    internal static async Task<Dictionary<int, int>> PjskCurrentEventPredict() =>
        JsonConvert.DeserializeObject<PjskCurrentEventPredict>(await GetString("https://api.sekai.best/event/pred"))?.Data
                   .Where(pair => int.TryParse(pair.Key, out _))
                   .ToDictionary(pair => int.Parse(pair.Key), pair => (int)pair.Value);
}
