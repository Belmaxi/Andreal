using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AndrealClient.AndreaMessage;
using AndrealClient.Core;
using AndrealClient.Data.Api;
using AndrealClient.Data.Json.Arcaea.BotArcApi;
using AndrealClient.Data.Sqlite;
using AndrealClient.Model.Arcaea;
using AndrealClient.UI.ImageGenerator;
using AndrealClient.Utils;

namespace AndrealClient.Executor;

[Serializable]
internal class ArcExecutor : ExecutorBase
{
    public ArcExecutor(MessageInfo info) : base(info) { }

    private async Task<(RecordInfo?, PlayerInfo?, TextMessage?)> GetUserBest(SongInfo song, sbyte dif = 3)
    {
        var data = await ArcaeaUnlimitedApi.UserBest(User!.ArcId, song.SongId, dif);

        switch (data.Status)
        {
            case -14:
            case -15:
                return dif != 0
                    ? await GetUserBest(song, --dif)
                    : (null, null, RobotReply.NotPlayedTheSong);

            case 0: break;

            default: return (null, null, ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message));
        }

        var content = data.DeserializeContent<UserBestContent>();
        return (new(content.Record), new(content.AccountInfo, User), null);
    }

    [CommandPrefix("/arc download")]
    private static MessageChain Download() => "https://dl.arcaea.moe/";

    [CommandPrefix("/arc bind", "绑定arc")]
    private async Task<MessageChain> Bind()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;

        ResponseRoot data;
        if (long.TryParse(Command[0], out var uid))
        {
            if (uid < 3)
            {
                var unbinduser = User ?? new BotUserInfo
                                         {
                                             QqId = Info.FromQq,
                                             UiVersion = (BotUserInfo.ImgVersion)new Random().Next(3)
                                         };

                unbinduser.ArcId = 0;

                BotUserInfo.Set(unbinduser);
                return RobotReply.UnBindSuccess;
            }

            var info = await ArcaeaUnlimitedApi.UserInfo(uid);
            data = info.Status == 0
                ? info
                : await ArcaeaUnlimitedApi.UserInfo(Command[0]);
        }
        else
            data = await ArcaeaUnlimitedApi.UserInfo(Command[0]);

        if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);

        var content = data.DeserializeContent<UserInfoContent>();

        var user = User ?? new BotUserInfo
                           {
                               QqId = Info.FromQq, UiVersion = (BotUserInfo.ImgVersion)new Random().Next(3)
                           };

        user.ArcId = content.AccountInfo.Code;

        BotUserInfo.Set(user);

        return
            $"{content.AccountInfo.Name} ({(content.AccountInfo.Rating == -1 ? "--" : ((double)content.AccountInfo.Rating / 100).ToString("0.00"))}){RobotReply.BindSuccess}";
    }


    [CommandPrefix("/arc ycm")]
    private async Task<MessageChain> GetCars()
    {
        var response = await OtherApi.YcmApi("arc");
        if (response.Code == 404) return "myc";

        return response.Cars.Aggregate("现有车牌:",
                                       (curr, room) =>
                                           curr
                                           + $"\n\n{room.RoomId}   {room.AddTime.DateStringFromNow()}\n{Regex.Unescape(room.Description)}");
    }

    [CommandPrefix("/arc car")]
    private async Task<MessageChain> NewCar()
    {
        if (CommandLength < 2) return RobotReply.ParameterLengthError;
        if (Command[0].Length != 6) return RobotReply.ParameterError;
        var comment = string.Join("_", Command.Skip(1));

        if (comment.Length < 4) return "描述信息过短将被视作无意义车牌，请添加更多描述。";

        var response = await OtherApi.AddCarApi("arc", Command[0], comment, User.QqId);

        return (response.Code switch
                {
                    0    => $"车牌 {Command[0]} 创建成功.",
                    1001 => "车牌格式不正确，请重试.",
                    1004 => $"车牌 {Command[0]} 已被创建，不能再次创建.",
                    1005 => "创建车牌次数过多，请两分钟后重试.",
                    _    => null
                })!;
    }

    [CommandPrefix("/arc rand", "随机选曲", "选曲")]
    private MessageChain RandSong()
    {
        switch (CommandLength)
        {
            default: return RobotReply.ParameterLengthError;
            case 0:  return RobotReply.RandSongReply + SongInfo.RandomSong().NameWithPackage;
            case 1:
            {
                var (lower, upper) = Command[0].ConvertToArcaeaRange();
                if (lower < 0) return RobotReply.ParameterError;
                var info = SongInfo.RandomSong(lower, upper);
                return info == null
                    ? RobotReply.ParameterError
                    : RobotReply.RandSongReply + info.NameWithPackageAndConst;
            }
            case 2:
            {
                var (lower, _) = Command[0].ConvertToArcaeaRange();
                var (_, upper) = Command[1].ConvertToArcaeaRange();
                if (lower < 0 || upper < 0 || lower > upper) return RobotReply.ParameterError;
                var info = SongInfo.RandomSong(lower, upper);
                return info == null
                    ? RobotReply.ParameterError
                    : RobotReply.RandSongReply + info.NameWithPackageAndConst;
            }
        }
    }

    [CommandPrefix("/arc note", "计算note")]
    private async Task<MessageChain> CalcNote()
    {
        if (CommandLength < 3) return RobotReply.ParameterLengthError;
        if (!short.TryParse(Command[0], out var far) || !short.TryParse(Command[1], out var lost))
            return RobotReply.ParameterError;

        var (_, (status, result)) = await ArcaeaHelper.SongNameConverter(Command.Skip(2));
        if (status != 0) return AliasErrorHelper.GetSongAliasErrorMessage(RobotReply, status, result);
        var arcsong = result![0];

        var notes = arcsong.Note;

        if (arcsong.Const <= 0 || notes <= 0) return "此谱面的Note数量暂未被记录。";

        if (far + lost > notes) return RobotReply.ParameterError;

        var perNoteScore = 1e7 / notes;

        return
            $"{arcsong.NameWithPackageAndConst}\n(+{notes})\n在 {far} Far {lost} Lost 时\n理论最高分数约为 {1e7 + notes - far - lost - (int)(perNoteScore * (lost + (double)far / 2))}";
    }

    [CommandPrefix("/arc score", "计算分数")]
    private async Task<MessageChain> CalcScore()
    {
        if (CommandLength < 2) return RobotReply.ParameterLengthError;

        if (!double.TryParse(Command[0], out var ptt)) return RobotReply.ParameterError;

        var (_, (status, result)) = await ArcaeaHelper.SongNameConverter(Command.Skip(1));
        if (status != 0) return AliasErrorHelper.GetSongAliasErrorMessage(RobotReply, status, result);
        var arcsong = result![0];

        var defNum = arcsong.Const;
        var mptt = ptt - defNum;
        if (defNum == 0 || ptt < 0 || mptt > 2) return RobotReply.ParameterError;

        long score;
        if (Math.Abs(mptt - 2) < 1e-4) return "PureMemery.";
        if (mptt >= 1)
            score = (long)(9.8e6 + (mptt - 1) * 2e5);
        else
            score = (long)(9.5e6 + mptt * 3e5);

        return $"{arcsong.NameWithPackageAndConst}\n在Ptt为 {Command[0]} 时\n分数为 {score}";
    }

    [CommandPrefix("/arc ptt", "计算ptt")]
    private async Task<MessageChain> CalcPtt()
    {
        if (CommandLength < 2) return RobotReply.ParameterLengthError;

        if (!double.TryParse(Command[0], out var score) || score is < 0 or > 10009999) return RobotReply.ParameterError;

        var (_, (status, result)) = await ArcaeaHelper.SongNameConverter(Command.Skip(1));
        if (status != 0) return AliasErrorHelper.GetSongAliasErrorMessage(RobotReply, status, result);
        var arcsong = result[0];

        var defNum = arcsong.Const;
        var ptt = score switch
                  {
                      >= 1e7   => defNum + 2,
                      >= 9.8e6 => defNum + 1 + (score - 9.8e6) / 2e5,
                      _ => defNum + (score - 9.5e6) / 3e5 > 0
                          ? defNum + (score - 9.5e6) / 3e5
                          : 0
                  };
        return $"{arcsong.NameWithPackageAndConst}\n在分数为 {Command[0]} 时\nPtt为 {ptt:0.0000}";
    }

    [CommandPrefix("/arc const", "查定数")]
    private async Task<MessageChain> Const()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        var (_, (status, result)) = await ArcaeaHelper.SongNameConverter(Command);
        if (status != 0) return AliasErrorHelper.GetSongAliasErrorMessage(RobotReply, status, result);
        return await result[0].FullConstString();
    }

    [CommandPrefix("/arc connect")]
    private static MessageChain Connect()
    {
        var now = DateTime.UtcNow;
        const string p = "qwertyuiopasdfghjklzxcvbnm1234567890";
        string r = "",
               v = BitConverter
                   .ToString(MD5.Create()
                                .ComputeHash(Encoding.ASCII
                                                     .GetBytes($"{now.Year}ori{now.Month - 1}wol{now.Day}oihs{now.Day}otas")))
                   .ToLower().Replace("-", "");
        for (var i = 0; i < v.Length; r += p[v[i] % 36], ++i) { }

        return $"今天的连接代码是：{r[1]}{r[20]}{r[4]}{r[30]}{r[2]}{r[11]}{r[23]}";
    }

    [CommandPrefix("/arc b30", "/arc r10", "/b30", "查b30")]
    private async Task<MessageChain> Best30()
    {
        if (User == null) return RobotReply.NotBind;
        if (User.ArcId < 2) return RobotReply.NotBindArc;


        Info.SendMessage("Best30请求已收到，正在查询中，请稍后...");
        IBest30Data b30data;
        PlayerInfo playerInfo;

        if (Command.Length > 0 && Command[0] == "official" && ArcaeaLimitedApi.Available)
        {
            playerInfo = new((await ArcaeaLimitedApi.Userinfo(User.ArcId))!, User);
            b30data = new LimitedBest30Data((await ArcaeaLimitedApi.Userbest30(User.ArcId))!, playerInfo.Potential);
        }
        else
        {
            var data = await ArcaeaUnlimitedApi.UserBest30(User.ArcId);
            if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);
            var content = data.DeserializeContent<UserBest30Content>();
            b30data = new Best30Data(content);
            playerInfo = new(content.AccountInfo, User);
        }

        return await new ArcBest30ImageGenerator(b30data, playerInfo).Generate();
    }

    [CommandPrefix("/arc floor", "查地板")]
    private Task<MessageChain> Floor() => FloorOrCeil(true);

    [CommandPrefix("/arc ceil", "查天花板")]
    private Task<MessageChain> Ceil() => FloorOrCeil(false);

    private async Task<MessageChain> FloorOrCeil(bool isFloor)
    {
        if (User == null) return RobotReply.NotBind;
        if (User.ArcId < 2) return RobotReply.NotBindArc;

        var data = await ArcaeaUnlimitedApi.UserBest30(User.ArcId);
        if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);
        var content = data.DeserializeContent<UserBest30Content>();
        IBest30Data b30data = new Best30Data(content);

        return await new ArcRecord5ImageGenerator(b30data, isFloor).Generate();
    }

    private static double CalcSongConst(string scores, double rating)
    {
        var score = Convert.ToDouble(scores);
        return score switch
               {
                   >= 10000000 => rating - 2,
                   >= 9800000  => rating - 1 - (score - 9800000) / 200000,
                   _ => rating > 0
                       ? rating - (score - 9500000) / 300000
                       : 0.0
               };
    }

    private async Task<MessageChain> Recent()
    {
        var data = await ArcaeaUnlimitedApi.UserInfo(User!.ArcId);
        if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);
        var content = data.DeserializeContent<UserInfoContent>();

        RecordInfo recordInfo = new(content.RecentScore[0]);
        PlayerInfo playerInfo = new(content.AccountInfo, User);

        return await new RecordData(playerInfo, recordInfo, User).GetResult();
    }

    [CommandPrefix("/arc info", "查分 ", "查 ")]
    private async Task<MessageChain> Search()
    {
        if (User == null) return RobotReply.NotBind;
        if (User.ArcId < 2) return RobotReply.NotBindArc;

        if (CommandLength == 0) return await Recent();
        if (Command[0] == "b30") return await Best30();

        RecordInfo recordInfo;
        PlayerInfo playerInfo;

        var (cached, (status, result)) = await ArcaeaHelper.SongNameConverter(Command);
        if (status != 0) return AliasErrorHelper.GetSongAliasErrorMessage(RobotReply, status, result);
        var arcsong = result[0];

        if (cached)
        {
            var data = await ArcaeaUnlimitedApi.UserBest(User.ArcId, arcsong.SongId, arcsong.Difficulty);
            if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);

            var content = data.DeserializeContent<UserBestContent>();
            recordInfo = new(content.Record);
            playerInfo = new(content.AccountInfo, User);
        }
        else
        {
            TextMessage exceptionInformation;
            (recordInfo, playerInfo, exceptionInformation) = await GetUserBest(arcsong);
            if (recordInfo == null) return exceptionInformation;
        }

        return await new RecordData(playerInfo, recordInfo, User).GetResult();
    }

    [CommandPrefix("/arc level")]
    private async Task<MessageChain> Level()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;
        if (!double.TryParse(Command[0], out var num) || num > 12) return RobotReply.ParameterError;
        if (num < 8) return (TextMessage)"暂不支持查询8.0以下的定数。";
        var list = SongInfo.GetByConst(num).ToArray();
        if (list.Length == 0) return (TextMessage)"此定数在当前版本不存在对应谱面。";
        return await new ArcSongLevelListImageGenerator(list).Generate();
    }

    [CommandPrefix("/arc ui")]
    private MessageChain UiConfig()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;

        if (User == null) return RobotReply.NotBind;

        if (!int.TryParse(Command[0], out var version)) return RobotReply.ParameterError;

        string imgversion;
        (User.UiVersion, imgversion) = version switch
                                       {
                                           2 => (BotUserInfo.ImgVersion.ImgV2, "ImgV2"),
                                           3 => (BotUserInfo.ImgVersion.ImgV3, "ImgV3"),
                                           4 => (BotUserInfo.ImgVersion.ImgV4, "ImgV4"),
                                           _ => (BotUserInfo.ImgVersion.ImgV1, "ImgV1")
                                       };
        BotUserInfo.Set(User);
        return $"Arc查分显示样式已更改为{imgversion}。";
    }

    [CommandPrefix("/arc alias", "查别名")]
    private async Task<MessageChain> Alias()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        var data = await ArcaeaUnlimitedApi.SongAlias(Command[0]);
        
        if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);

        var alias = data.DeserializeContent<List<string>>();
        
        return alias.Count > 0
            ? $"在数据库中的别名列表：\n" + alias.Aggregate((i, j) => i + "\n" + j)
            : "该曲目暂无别名收录。";
    }
}
