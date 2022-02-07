using System.Text.RegularExpressions;
using AndrealClient.AndreaMessage;
using AndrealClient.Core;
using AndrealClient.Data.Api;
using AndrealClient.Data.Json.Pjsk;
using AndrealClient.Data.Json.Pjsk.PjskProfile;
using AndrealClient.Data.Sqlite;
using AndrealClient.UI.ImageGenerator;
using AndrealClient.Utils;

namespace AndrealClient.Executor;

[Serializable]
internal class PjskExecutor : ExecutorBase
{
    public PjskExecutor(MessageInfo info) : base(info) { }

    [CommandPrefix("/pjsk bind", "绑定pjsk")]
    private async Task<MessageChain> Bind()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;
        if (!long.TryParse(Command[0], out var userId)) return RobotReply.ParameterError;

        var pjskProfile = await PjskApi.PjskProfile(userId);

        if (pjskProfile is null || pjskProfile.User is null) return RobotReply.PjskUserBindFailed;

        var user = User ?? new BotUserInfo { QqId = Info.FromQq };

        user.PjskId = pjskProfile.User.UserGamedata.UserId;

        BotUserInfo.Set(user);

        return $"{pjskProfile.User.UserGamedata.Name} ({pjskProfile.User.UserGamedata.Rank}) " + RobotReply.BindSuccess;
    }

    [CommandPrefix("/pjsk rand")]
    private MessageChain RandSong()
    {
        switch (CommandLength)
        {
            default: return RobotReply.ParameterLengthError;
            case 0:  return RobotReply.RandSongReply + Model.Pjsk.SongInfo.RandomSong().SongName;
            case 1:
            {
                if (!int.TryParse(Command[0], out var limit) || limit < 5 || limit > 40)
                    return RobotReply.ParameterError;

                var (info, dif) = Model.Pjsk.SongInfo.RandomSong(limit);

                return info == null
                    ? RobotReply.ParameterError
                    : RobotReply.RandSongReply + info.NameWithLevel(dif);
            }
            case 2:
            {
                if (!int.TryParse(Command[0], out var lower) || lower < 5 || lower > 40)
                    return RobotReply.ParameterError;

                if (!int.TryParse(Command[1], out var upper) || lower > upper || upper > 40)
                    return RobotReply.ParameterError;

                var (info, dif) = Model.Pjsk.SongInfo.RandomSong(lower, upper);
                return info == null
                    ? RobotReply.ParameterError
                    : RobotReply.RandSongReply + info.NameWithLevel(dif);
            }
        }
    }

    [CommandPrefix("/pjsk user")]
    private async Task<MessageChain> Profile()
    {
        if (User == null) return RobotReply.NotBind;
        if (User.PjskId == 0) return RobotReply.NotBindPjsk;

        // 2020/09/16 10:00:00 (utc+9) so 9am in utc+8
        // and add (Id/1000/1024/4096) seconds
        // thanks to nilcric.

        var registerDate
            = new DateTime(2020, 9, 16, 9, 0, 0, DateTimeKind.Utc).AddSeconds((double)User.PjskId / 0xFA000000);

        PjskCurrentEventItem currentEvent = null;
        PjskProfiles pjskProfile = null;

        await Task.WhenAll(Task.Run(() => pjskProfile = PjskApi.PjskProfile(User.PjskId).Result),
                           Task.Run(() => currentEvent = PjskApi.PjskCurrentEvent().Result));

        var eventRankings = await PjskApi.PjskUserRanking(User.PjskId, currentEvent.EventId);
        var userGamedata = pjskProfile.User.UserGamedata;

        return
            $"{userGamedata.Name}  ({userGamedata.Rank})\n\n注册时间 : {registerDate:yyyy/MM/dd hh:mm:ss}\n\n{Capture()}\n\n"
            + $"本期活动：{currentEvent.Name} \n" + (eventRankings == null
                ? ""
                : $"#{eventRankings.Rank}  ({eventRankings.Score}P)\n")
            + $"\n详细信息请使用浏览器查看：\nhttps://profile.pjsekai.moe/#/user/{User.PjskId}";

        string Capture()
        {
            int maFc = 0, maAp = 0, exFc = 0, exAp = 0;
            var count = Model.Pjsk.SongInfo.Count();
            foreach (var item in pjskProfile.UserMusics.SelectMany(i => i.UserMusicDifficultyStatuses)
                                            .Where(i => i.MusicDifficulty is "master" or "expert"))
            {
                if (!item.UserMusicResults.Any()) continue;

                var ap = false;
                var fc = false;

                foreach (var resultsItem in item.UserMusicResults)
                {
                    ap = ap || resultsItem.FullPerfectFlg;
                    fc = fc || resultsItem.FullComboFlg;
                }

                if (ap)
                {
                    if (item.MusicDifficulty == "master") maAp++;
                    if (item.MusicDifficulty == "expert") exAp++;
                }

                if (fc)
                {
                    if (item.MusicDifficulty == "master") maFc++;
                    if (item.MusicDifficulty == "expert") exFc++;
                }
            }

            return $"进度 (AP | FC | All)\nExpert : {exAp} | {exFc} | {count}\nMaster : {maAp} | {maFc} | {count}";
        }
    }

    [CommandPrefix("/pjsk event")]
    private async Task<MessageChain> Event()
    {
        PjskCurrentEventItem currentEvent = null;
        Dictionary<int, int> predict = null;

        await Task.WhenAll(Task.Run(async () => currentEvent = await PjskApi.PjskCurrentEvent()),
                           Task.Run(async () => predict = await PjskApi.PjskCurrentEventPredict()));

        var eventRankings = await PjskApi.PjskUserRanking(User.PjskId, currentEvent.EventId);
        var generator = new PjskCurrentEventImageGenerator(currentEvent, eventRankings, predict);
        return User.IsText == 1
            ? generator.TextMessage
            : await generator.Generate();
    }

    [CommandPrefix("/pjsk ycm")]
    private async Task<MessageChain> GetCars()
    {
        var response = await OtherApi.YcmApi("pjsk");
        if (response.Code == 404) return "myc";

        return response.Cars.Aggregate("现有车牌:",
                                       (curr, room) =>
                                           curr
                                           + $"\n\n{room.RoomId}   {room.AddTime.DateStringFromNow()}\n{Regex.Unescape(room.Description)}");
    }

    [CommandPrefix("/pjsk car")]
    private async Task<MessageChain> NewCar()
    {
        if (CommandLength < 2) return RobotReply.ParameterLengthError;
        if (Command[0].Length != 5! || int.TryParse(Command[0], out var id)) return RobotReply.ParameterError;
        if (id is 11451 or 14514) return "恶臭车牌(";
        var comment = string.Join("_", Command.Skip(1));

        if (!comment.Contains("大分") && !comment.Contains("自由") && !comment.Contains("q4") && !comment.Contains("q3")
            && !comment.Contains("q2") && !comment.Contains("q1") && !comment.Contains("m") && !comment.Contains("18w")
            && !comment.Contains("15w") && !comment.Contains("12w") && comment.Length < 4)
            return "描述信息过短将被视作无意义车牌，请添加更多描述。";

        var response = await OtherApi.AddCarApi("pjsk", Command[0], comment, User.QqId);

        return (response.Code switch
                {
                    0    => $"车牌 {Command[0]} 创建成功.",
                    1001 => "车牌格式不正确，请重试.",
                    1004 => $"车牌 {Command[0]} 已被创建，不能再次创建.",
                    1005 => "创建车牌次数过多，请两分钟后重试.",
                    _    => null
                })!;
    }
}
