using AndrealClient.AndreaMessage;
using AndrealClient.Core;
using AndrealClient.Data.Api;
using AndrealClient.Data.Json.Osu;
using AndrealClient.Data.Sqlite;
using AndrealClient.Model.Osu;
using AndrealClient.UI.ImageGenerator;

namespace AndrealClient.Executor;

[Serializable]
internal class OsuExecutor : ExecutorBase
{
    private static readonly string[] OsuModeString = { "Standard", "Taiko", "Catch", "Mania" };

    public OsuExecutor(MessageInfo info) : base(info) { }

    private static int GetOsuMode(string mode)
    {
        return mode switch
               {
                   "s"        => 0,
                   "std"      => 0,
                   "standard" => 0,
                   "t"        => 1,
                   "taiko"    => 1,
                   "c"        => 2,
                   "ctb"      => 2,
                   "cth"      => 2,
                   "catch"    => 2,
                   "m"        => 3,
                   "mania"    => 3,
                   _          => -2
               };
    }

    [CommandPrefix("/osu bind", "绑定osu")]
    private async Task<MessageChain> Bind()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        var osumode = CommandLength == 2
            ? GetOsuMode(Command[1])
            : -1;
        if (osumode == -2) return RobotReply.OsuModeConvertFailed;
        var user = User ?? new BotUserInfo();
        OsuUserinfo userinfo;

        var username = Command[0];

        if (osumode >= 0)
        {
            userinfo = await OsuApi.Userinfo(username, osumode);
            if (userinfo == null || userinfo.Username == "" || userinfo.UserId == "")
                return RobotReply.OsuUserBindFailed;
        }
        else
            for (osumode = 0; (userinfo = await OsuApi.Userinfo(username, osumode))?.PpRaw == null; ++osumode) { }

        user.QqId = Info.FromQq;
        user.OsuId = long.Parse(userinfo.UserId);
        user.OsuMode = osumode;
        BotUserInfo.Set(user);

        return userinfo.Username + RobotReply.BindSuccess;
    }

    [CommandPrefix("/osu config")]
    private MessageChain Config()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        if (User == null) return RobotReply.NotBind;
        if (User.OsuId == 0) return RobotReply.NotBindOsu;

        var osumode = GetOsuMode(Command[0]);
        if (osumode < 0) return RobotReply.OsuModeConvertFailed;

        User.OsuMode = osumode;

        BotUserInfo.Set(User);
        return $"Osu默认模式已经设置为{OsuModeString[osumode]}.";
    }

    [CommandPrefix("/osu mode ", "/osum ", "/om ")]
    private async Task<MessageChain> Record()
    {
        if (User == null) return RobotReply.NotBind;
        if (User.OsuId == 0) return RobotReply.NotBindOsu;

        var osuid = User.OsuId;
        var osumode = CommandLength == 1
            ? GetOsuMode(Command[0])
            : User.OsuMode;

        OsuRecentInfo recentInfo = null;
        OsuUserinfo userinfo = null;
        OsuBeatMapInfo beatMapInfo = null;

        await Task.WhenAll(Task.Run(() => recentInfo = OsuApi.RecentInfo(osuid, osumode).Result),
                           Task.Run(() => userinfo = OsuApi.Userinfo(osuid.ToString(), osumode).Result),
                           Task.Run(() => beatMapInfo = OsuApi.BeatMapInfo(recentInfo.BeatmapId).Result));

        if (recentInfo == null) return RobotReply.OsuRecentNotPlayed;
        var info = new QueryInfo(recentInfo, userinfo, beatMapInfo, osumode);
        return User.IsText == 1
            ? info.RecentMessage
            : new OsuImageGenerator(info).Generate();
    }
}
