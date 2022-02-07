using System.Collections.Concurrent;
using AndrealClient.Core;
using AndrealClient.Utils;
using SQLite;

namespace AndrealClient.Data.Sqlite;

[Serializable]
[Table("BotUserInfo")]
[CreateTableSql("CREATE TABLE IF NOT EXISTS BotUserInfo(" + "QQId INTEGER PRIMARY KEY NOT NULL, "
                                                          + "ArcId INTEGER DEFAULT(-1), "
                                                          + "OsuId INTEGER DEFAULT(-1), "
                                                          + "PjskId INTEGER DEFAULT(0), "
                                                          + "OsuMode INTEGER DEFAULT(0),"
                                                          + "IsHide INTEGER DEFAULT(0), "
                                                          + "IsText INTEGER DEFAULT(0), "
                                                          + "ImgVer INTEGER DEFAULT(0));")]
internal class BotUserInfo
{
    private static Lazy<ConcurrentDictionary<long, BotUserInfo>> _list
        = new(() => new(SqliteHelper.SelectAll<BotUserInfo>().ToDictionary(i => i.QqId)));

    [PrimaryKey] [Column("QQId")] public long QqId { get; set; }
    [Column("ArcId")] public int ArcId { get; set; }
    [Column("OsuId")] public long OsuId { get; set; }
    [Column("PjskId")] public long PjskId { get; set; }
    [Column("OsuMode")] public int OsuMode { get; set; }
    [Column("IsHide")] public int IsHide { get; set; }
    [Column("IsText")] public int IsText { get; set; }
    [Column("ImgVer")] public ImgVersion UiVersion { get; set; }
    internal static int Count => _list.Value.Count;

    internal static void Init()
    {
        _list = new(() => new(SqliteHelper.SelectAll<BotUserInfo>().ToDictionary(i => i.QqId)));
    }

    internal static void Set(BotUserInfo user)
    {
        if (_list.Value.ContainsKey(user.QqId))
        {
            _list.Value[user.QqId] = user;
            SqliteHelper.Update(user);
        }
        else
        {
            _list.Value.TryAdd(user.QqId, user);
            SqliteHelper.Insert(user);
        }
    }

    internal static BotUserInfo? Get(long qqid) =>
        _list.Value.TryGetValue(qqid, out var user)
            ? user
            : null;

    internal enum ImgVersion
    {
        ImgV1 = 0,
        ImgV2 = 1,
        ImgV3 = 2,
        ImgV4 = 3
    }
}
