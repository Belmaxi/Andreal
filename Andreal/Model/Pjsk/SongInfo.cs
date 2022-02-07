using System.Collections.Concurrent;
using AndrealClient.AndreaMessage;
using AndrealClient.Core;
using AndrealClient.Data.Json.Pjsk;
using AndrealClient.Utils;
using Path = AndrealClient.Core.Path;

namespace AndrealClient.Model.Pjsk;

[Serializable]
internal class SongInfo : SongInfos.ISongInfo
{
    [NonSerialized] private static Lazy<ConcurrentDictionary<int, SongInfo>> _songList = new();

    private SongInfo(PjskMusicMetadata data)
    {
        Data = data;
        Levels = data.Level.Split('|').Select(int.Parse).ToList();
    }

    private PjskMusicMetadata Data { get; }

    internal int SongId => Data.SongId;

    internal string SongName => Data.Songname;

    internal List<int> Levels { get; }

    string SongInfos.ISongInfo.SongName => SongName;

    internal static bool CheckSongId(int songId) => _songList.Value.ContainsKey(songId);

    internal static bool CheckFull(int songId) =>
        _songList.Value.TryGetValue(songId, out var info) && info.Data.FeverScore != "NA";

    internal string GetSongName(byte length) =>
        Data.Songname.Length < length + 3
            ? Data.Songname
            : $"{Data.Songname.Substring(0, length)}...";

    private ImageMessage SongImage()
    {
        var pth = Path.PjskSong(SongId);
        if (!pth.FileExists)
            WebHelper.DownloadImage($"https://assets.pjsek.ai/file/pjsekai-assets/startapp/music/jacket/jacket_s_{SongId:000}/jacket_s_{SongId:000}.png",
                                    pth);
        return ImageMessage.FromPath(pth);
    }


    internal string NameWithLevel(sbyte dif) => $"{SongName}  [{DifficultyInfo.GetByIndex(dif).LongStr} {Levels[dif]}]";

    internal static int Count() => _songList.Value.Count;

    internal static SongInfo? GetBySid(int? sid) =>
        sid == null
            ? null
            : _songList.Value.FirstOrDefault(i => sid.Value == i.Key).Value;


    public bool Equals(SongInfo? other)
    {
        if (ReferenceEquals(other, null)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SongId == other.SongId;
    }

    private static IEnumerable<(SongInfo, sbyte)> GetByLevelRange(int lowerlimit, int upperlimit)
    {
        return _songList.Value.Values.SelectMany(c => c.Levels, (c, i) => new { c, i })
                        .Where(t => t.i >= lowerlimit && t.i <= upperlimit)
                        .Select(t => (t.c, (sbyte)t.c.Levels.IndexOf(t.i)));
    }

    private static IEnumerable<(SongInfo, sbyte)> GetByLevel(int limit)
    {
        return _songList.Value.Values.SelectMany(c => c.Levels, (c, i) => new { c, i }).Where(t => t.i == limit)
                        .Select(t => (t.c, (sbyte)t.c.Levels.IndexOf(t.i)));
    }

    internal static (SongInfo?, sbyte) RandomSong(int lowerlimit, int upperlimit)
    {
        var ls = GetByLevelRange(lowerlimit, upperlimit).ToArray();
        return ls.Length > 0
            ? ls.GetRandomItem()
            : (null, -1);
    }

    internal static (SongInfo?, sbyte) RandomSong(int limit)
    {
        var ls = GetByLevel(limit).ToArray();
        return ls.Length > 0
            ? ls.GetRandomItem()
            : (null, -1);
    }

    internal static void Insert(PjskMusics item, List<PjskMusicMetas> musicMetas)
    {
        var obj = new PjskMusicMetadata
                  {
                      SongId = item.Id,
                      Songname = item.Title,
                      Categories = string.Join(" | ", item.Categories),
                      Lyricist = item.Lyricist,
                      Composer = item.Composer,
                      MusicTime = musicMetas.First().MusicTime,
                      EventRate = musicMetas.First().EventRate,
                      Level = string.Join(" | ", musicMetas.Select(i => i.Level)),
                      Note = string.Join(" | ", musicMetas.Select(i => i.Combo)),
                      BaseScore = string.Join(" | ", musicMetas.Select(i => i.BaseScore.ToString("0.000"))),
                      FeverScore = string.Join(" | ", musicMetas.Select(i => i.FeverScore.ToString("0.000"))),
                      AssetbundleName = item.AssetbundleName,
                      PublishedAt = item.PublishedAt
                  };

        if (!CheckSongId(item.Id))
        {
            _songList.Value.TryAdd(item.Id, new(obj));
            SqliteHelper.Insert(obj);
        }
        else
        {
            _songList.Value[item.Id] = new(obj);
            SqliteHelper.Update(obj);
        }
    }

    internal static void Insert(PjskMusics item, List<PjskMusicDifficulties> musicMetas)
    {
        var obj = new PjskMusicMetadata
                  {
                      SongId = item.Id,
                      Songname = item.Title,
                      Categories = string.Join(" | ", item.Categories),
                      Lyricist = item.Lyricist,
                      Composer = item.Composer,
                      MusicTime = 0,
                      EventRate = 0,
                      Level = string.Join(" | ", musicMetas.Select(i => i.PlayLevel)),
                      Note = string.Join(" | ", musicMetas.Select(i => i.NoteCount)),
                      BaseScore = "NA",
                      FeverScore = "NA",
                      AssetbundleName = item.AssetbundleName,
                      PublishedAt = item.PublishedAt
                  };

        if (!CheckSongId(item.Id))
        {
            _songList.Value.TryAdd(item.Id, new(obj));
            SqliteHelper.Insert(obj);
        }
    }

    internal static SongInfo RandomSong() => _songList.Value.Values.GetRandomItem();
}
