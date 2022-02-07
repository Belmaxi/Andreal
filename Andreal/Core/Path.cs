using AndrealClient.Data.Api;
using AndrealClient.Utils;

namespace AndrealClient.Core;

[Serializable]
internal class Path
{
    private const string ArcaeaBackgroundRoot = "./Andreal/Arcaea/Background/";
    private const string ArcaeaPackageSourseRoot = "./Andreal/Arcaea/PackageSourse/";
    private const string ArcaeaSourseRoot = "./Andreal/Arcaea/Sourse/";
    private const string TempImageRoot = "./Andreal/TempImage/";
    private const string AndreaOtherRoot = "./Andreal/Other/";
    private const string AndreaConfigRoot = "./Andreal/Config/";
    private const string OsuImageRoot = "./Andreal/Osu/";
    private const string PjskImageRoot = "./Andreal/Pjsk/";

    internal static readonly Path Database = new(AndreaOtherRoot + "Andreal.db");
    
    internal static readonly Path ApiConfig = new(AndreaConfigRoot + "apiconfig.json");
    
    internal static readonly Path Portconfig = new(AndreaConfigRoot + "portconfig");

    internal static readonly Path ArcaeaConstListBg = new(ArcaeaSourseRoot + "ConstList.jpg");

    internal static readonly Path ArcaeaDivider = new(ArcaeaSourseRoot + "Divider.png");

    internal static readonly Path ArcaeaGlass = new(ArcaeaSourseRoot + "Glass.png");

    internal static readonly Path ArcaeaBest30Bg = new(ArcaeaSourseRoot + "B30.png");

    internal static readonly Path ArcaeaBest5Bg = new(ArcaeaSourseRoot + "Floor.png");

    internal static readonly Path ArcaeaUnknownBg = new(ArcaeaSourseRoot + "Unknown.png");

    internal static readonly Path ExceptionReport = new(AndreaOtherRoot + "ExceptionReport.txt");

    internal static readonly Path ArcaeaBg1Mask = new(ArcaeaSourseRoot + "Mask.png");

    internal static readonly Path PartnerConfig = new(AndreaConfigRoot + "positioninfo.json");

    internal static readonly Path RobotReply = new(AndreaConfigRoot + "replytemplate.json");

    private readonly string _rawpath;

    private Path(string rawpath) { _rawpath = rawpath; }

    internal bool FileExists => File.Exists(this);

    internal static Path ArcaeaBg1(string sid, sbyte difficulty) =>
        new(ArcaeaBackgroundRoot + $"V1_{sid}{(difficulty == 3 ? "_3" : "")}.png");

    internal static Path ArcaeaBg2(string sid, sbyte difficulty) =>
        new(ArcaeaBackgroundRoot + $"V2_{sid}{(difficulty == 3 ? "_3" : "")}.png");

    internal static Path ArcaeaBg3(string sid, sbyte difficulty) =>
        new(ArcaeaBackgroundRoot + $"V3_{sid}{(difficulty == 3 ? "_3" : "")}.png");

    internal static Path ArcaeaBg3Mask(int side) => new(ArcaeaSourseRoot + $"RawV3Bg_{side}.png");

    internal static async Task<Path> ArcaeaSong(string sid, sbyte difficulty)
    {
        var song = sid switch
                   {
                       "stager"       => $"stager_{difficulty}",
                       "melodyoflove" => $"melodyoflove{(DateTime.Now.Hour is > 19 or < 6 ? "_night" : "")}",
                       _              => $"{sid}{(difficulty == 3 ? "_3" : "")}"
                   };

        var pth = new Path($"{ArcaeaPackageSourseRoot}Song/{song}.jpg");

        if (!pth.FileExists) await ArcaeaUnlimitedApi.SongAssets(song, pth);
        return pth;
    }

    internal static Path ArcaeaRating(short potential)
    {
        var img = potential switch
                  {
                      >= 1250 => "6",
                      >= 1200 => "5",
                      >= 1100 => "4",
                      >= 1000 => "3",
                      >= 700  => "2",
                      >= 350  => "1",
                      >= 0    => "0",
                      < 0     => "off"
                  };
        return new(ArcaeaSourseRoot + $"rating_{img}.png");
    }

    internal static async Task<Path> ArcaeaPartner(int partner, bool awakened)
    {
        var pth = new Path(ArcaeaPackageSourseRoot + $"Char/{partner}{(awakened ? "u" : "")}.png");

        if (!pth.FileExists) await ArcaeaUnlimitedApi.CharAssets(partner, awakened, pth);
        return pth;
    }

    internal static async Task<Path> ArcaeaPartnerIcon(int partner, bool awakened)
    {
        var pth = new Path(ArcaeaPackageSourseRoot + $"Icon/{partner}{(awakened ? "u" : "")}.png");
        if (!pth.FileExists) await ArcaeaUnlimitedApi.IconAssets(partner, awakened, pth);
        return pth;
    }

    internal static Path ArcaeaCleartypeV3(sbyte cleartype) => new(ArcaeaSourseRoot + $"clear_{cleartype}.png");

    internal static Path ArcaeaCleartypeV1(sbyte cleartype) => new(ArcaeaSourseRoot + $"end_{cleartype}.png");

    internal static Path ArcaeaCleartypeV4(sbyte cleartype) => new(ArcaeaSourseRoot + $"clear_badge_{cleartype}.png");

    internal static Path ArcaeaDifficultyForV1(sbyte difficulty) => new(ArcaeaSourseRoot + $"con_{difficulty}.png");

    internal static Path RandImageFileName() => new(TempImageRoot + $"{RandStringHelper.GetRandString()}.jpg");

    internal static Path OsuCountryFlag(string country) => new(OsuImageRoot + $"Flags/{country}.png");

    internal static Path OsuBg(int mode) => new(OsuImageRoot + $"Background_{mode}.png");

    internal static Path OsuRank(string rank) => new(OsuImageRoot + $"Rank/{rank}.png");

    internal static Path OsuMod(int mod) => new(OsuImageRoot + $"Mods/mod_{mod}.png");

    internal static Path PjskSong(int sid) => new(PjskImageRoot + $"Song/{sid}.png");

    internal static Path PjskChart(int sid, string dif) => new(PjskImageRoot + $"Chart/{sid}_{dif}.png");

    public static Path PjskEvent(string currentEventAssetbundleName) =>
        new(PjskImageRoot + $"EventBg/{currentEventAssetbundleName}.png");
   
    public static Path ArcaeaBg4(sbyte difficulty, string package)
    {
        var backgroundImgV4 = difficulty == 3
            ? "Beyond"
            : package switch
              {
                  "base"                 => "Arcaea",
                  "core" or "yugamu"     => "Lost World",
                  "rei" or "prelude"     => "Spire of Convergence",
                  "mirai" or "nijuusei"  => "Outer Reaches",
                  "shiawase" or "zettai" => "Dormant Echoes",
                  "vs" or "extend"       => "Boundless Divide",
                  _                      => "Event"
              };
        return new(ArcaeaSourseRoot + $@"V4Bg-{backgroundImgV4}.png");
    }

    static Path()
    {
        if (!Directory.Exists(ArcaeaBackgroundRoot)) Directory.CreateDirectory(ArcaeaBackgroundRoot);
        if (!Directory.Exists(ArcaeaSourseRoot)) Directory.CreateDirectory(ArcaeaSourseRoot);
        if (!Directory.Exists(TempImageRoot)) Directory.CreateDirectory(TempImageRoot);
        if (!Directory.Exists(AndreaOtherRoot)) Directory.CreateDirectory(AndreaOtherRoot);
        if (!Directory.Exists(AndreaConfigRoot)) Directory.CreateDirectory(AndreaConfigRoot);
        if (!Directory.Exists(OsuImageRoot)) Directory.CreateDirectory(OsuImageRoot);

        if (!Directory.Exists(PjskImageRoot))
        {
            Directory.CreateDirectory(PjskImageRoot);
            Directory.CreateDirectory(PjskImageRoot + "Song/");
            Directory.CreateDirectory(PjskImageRoot + "Chart/");
            Directory.CreateDirectory(PjskImageRoot + "EventBg/");
        }

        if (!Directory.Exists(ArcaeaPackageSourseRoot))
        {
            Directory.CreateDirectory(ArcaeaPackageSourseRoot);
            Directory.CreateDirectory(ArcaeaPackageSourseRoot + "Song/");
            Directory.CreateDirectory(ArcaeaPackageSourseRoot + "Char/");
            Directory.CreateDirectory(ArcaeaPackageSourseRoot + "Icon/");
        }
    }

    public static implicit operator string(Path path) => AppContext.BaseDirectory + path._rawpath;
}
