using AndrealClient.AndreaMessage;
using AndrealClient.Core;
using AndrealClient.Data.Api;
using AndrealClient.Data.Sqlite;
using AndrealClient.Utils;

namespace AndrealClient.Executor;

[Serializable]
internal class OtherExecutor : ExecutorBase
{
    public OtherExecutor(MessageInfo info) : base(info) { }

    [CommandPrefix("/help", "/arc help")]
    private MessageChain? HelpMessage()
    {
        if (CommandLength > 0) return null;
        return RobotReply.HelpMessage;
    }

    [CommandPrefix("/yiyan")]
    private async Task<MessageChain> Hitokoto() => await OtherApi.HitokotoApi();

    [CommandPrefix("/jrrp")]
    private async Task<MessageChain> Jrrp() => RobotReply.JrrpResult.Replace("$jrrp$",await OtherApi.JrrpApi(Info.FromQq));

    [CommandPrefix("/dismiss")]
    private async Task<MessageChain?> Dismiss()
    {
        if (IsGroup && await Info.PermissionCheck()) await Api.LeaveGroup(Info.FromGroup);

        return null;
    }
    
    [CommandPrefix("/geterr")]
    private MessageChain ExceptionReport() =>
        LastExceptionHelper.GetDetails();

    [CommandPrefix("/config")]
    private MessageChain Config()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;
        if (User == null) return RobotReply.NotBind;

        switch (Command[0])
        {
            case "hide":
                User.IsHide ^= 1;
                BotUserInfo.Set(User);
                return $"私密模式已{(User.IsHide == 1 ? "开启" : "关闭")}。";

            case "text":
            case "txt":
                User.IsText = 1;
                BotUserInfo.Set(User);
                return "默认显示方式已更改为文字。";

            case "pic":
            case "img":
                User.IsText = 0;
                BotUserInfo.Set(User);
                return "默认显示方式已更改为图片。";

            default: return RobotReply.ConfigNotFound;
        }
    }
    
    [CommandPrefix("/ycm")]
    private async Task<MessageChain> GetBandoriRoom() => await OtherApi.BandoriYcmApi();

    [CommandPrefix("/state")]
    private MessageChain Statement() => BotStatementHelper.Statement;
}
