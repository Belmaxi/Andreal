// ReSharper disable InconsistentNaming

using System.Text.RegularExpressions;
using AndrealClient.AndreaMessage;
using AndrealClient.Data.Json.Osu;

namespace AndrealClient.Model.Osu;

[Serializable]
internal class QueryInfo
{
    internal readonly int Mode;

    internal QueryInfo(OsuRecentInfo jsonData1, OsuUserinfo jsonData2, OsuBeatMapInfo jsonData3, int osuMode)
    {
        Rank = jsonData1.Rank;
        BeatmapId = jsonData1.BeatmapId;
        BeatmapSetId = jsonData3.BeatmapsetId;
        Date = DateTime.Parse(jsonData1.Date).AddHours(8).ToString("yyyy/MM/dd HH:mm:ss");
        Mods = Convert.ToString(jsonData1.EnabledMods, 2).ToCharArray();
        C100 = jsonData1.Count100;
        C300 = jsonData1.Count300;
        C50 = jsonData1.Count50;
        Geki = jsonData1.Countgeki;
        Katu = jsonData1.Countkatu;
        Miss = jsonData1.Countmiss;
        Combo = jsonData1.Maxcombo;
        Score = jsonData1.Score;
        PPrank = jsonData2.PpCountryRank;
        PP = jsonData2.PpRaw.ToString();
        Username = jsonData2.Username;
        Country = jsonData2.Country;
        Artist = jsonData3.ArtistUnicode == null
            ? jsonData3.Artist
            : Regex.Unescape(jsonData3.ArtistUnicode);
        Title = jsonData3.TitleUnicode == null
            ? jsonData3.Title
            : Regex.Unescape(jsonData3.TitleUnicode);
        Stars = jsonData3.Difficultyrating.ToString("0.00");
        AR = jsonData3.DiffApproach;
        HP = jsonData3.DiffDrain;
        OD = jsonData3.DiffOverall;
        CS = jsonData3.DiffSize;
        Mode = osuMode;
    }

    //23,29 null
    private static string[] Mods_String { get; } =
        {
            "NoFail", "Easy", "TouchDevice", "Hidden", "HardRock", "SuddenDeath", "DoubleTime", "Relax", "HalfTime",
            "Nightcore", "Flashlight", "Autoplay", "SpunOut", "Autopilot", "Perfect", "Key4", "Key5", "Key6", "Key7",
            "Key8", "FadeIn", "Random", "Cinema", "Target", "Key9", "KeyCoop", "Key1", "Key3", "Key2", "ScoreV2",
            "Mirror"
        };

    internal char[] Mods { get; }
    internal string C100 { get; }
    internal string C300 { get; }
    internal string C50 { get; }
    internal string Geki { get; }
    internal string Katu { get; }
    internal string Miss { get; }
    internal string Date { get; }
    internal string Combo { get; }
    internal string Score { get; }
    internal string PPrank { get; }
    internal string Username { get; }
    internal string Artist { get; }
    internal string Title { get; }
    internal string PP { get; }
    internal string Stars { get; }
    internal string AR { get; }
    internal string HP { get; }
    internal string OD { get; }
    internal string CS { get; }
    internal string Rank { get; }
    internal string BeatmapId { get; }
    internal string BeatmapSetId { get; }
    internal string Country { get; }

    internal TextMessage RecentMessage =>
        $"{Username}(#{PPrank}) 的最近记录\n曲名：{Title}\nAR：{AR}  OD：{OD}  CS：{CS}  HP：{HP}\nStars：{Stars}    Acc.：{Acc()}\nMode： {ModsString()}\nResults：\n{Result()}\n分数：{Score}  Combo：{Combo}\n时间：{Date}";

    private string Result()
    {
        return Mode switch
               {
                   0 => "300(激: " + C300 + "(" + Geki + "   50:  " + C50 + "\n100(喝: " + C100 + "(" + Katu + "   Miss: "
                        + Miss,
                   1 => "良: " + C300 + "   可: " + C100 + "   不可 :" + Miss,
                   2 => "Fruit: " + C300 + "   Drop: " + C100 + "\nDroplet: " + C50 + "   Miss: " + Miss,
                   3 => "300M: " + Geki + "   300: " + C300 + "   200: " + Katu + "\n100: " + C100 + "   50: " + C50
                        + "   Miss: " + Miss,
                   _ => ""
               };
    }

    internal string Acc()
    {
        double c300 = Convert.ToDouble(C300), c100 = Convert.ToDouble(C100), c50 = Convert.ToDouble(C50),
               miss = Convert.ToDouble(Miss), c200 = Convert.ToDouble(Katu), c300M = Convert.ToDouble(Geki);
        return Mode switch
               {
                   0 => ((c300 + c100 / 3 + c50 / 6) / (c300 + c100 + c50 + miss) * 100).ToString("0.00") + "%",
                   1 => ((c300 + c100 / 2) / (c300 + c100 + miss) * 100).ToString("0.00") + "%",
                   2 => ((c300 + c100 + c50) / (c300 + c100 + c50 + miss + c200) * 100).ToString("0.00") + "%",
                   3 => ((c300 + c300M + c200 * 2 / 3 + c100 / 3 + c50 / 6) / (c300 + c300M + c200 + c100 + c50 + miss)
                         * 100).ToString("0.00") + "%",
                   _ => ""
               };
    }

    private string ModsString()
    {
        var mods = "";
        for (var i = Mods.Length - 1; i >= 0; --i)
        {
            var mod = Mods.Length - i - 1;
            switch (mod)
            {
                case 6 when Mods.Length > 9 && Mods[Mods.Length - 10] == '1':
                case 5 when Mods.Length > 14 && Mods[Mods.Length - 15] == '1':
                    continue;
                default:
                    mods += Mods[i] == '1'
                        ? Mods_String[mod] + ","
                        : "";
                    break;
            }
        }

        return mods == ""
            ? "None"
            : mods.Substring(0, mods.Length - 1);
    }
}
