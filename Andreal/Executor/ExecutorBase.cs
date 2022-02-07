using AndrealClient.Core;
using AndrealClient.Data.Sqlite;
using AndrealClient.RobotReply;
using Sora.Entities.Base;

namespace AndrealClient.Executor;

[Serializable]
internal abstract class ExecutorBase
{
    protected readonly MessageInfo Info;

    protected ExecutorBase(MessageInfo info) { Info = info; }

    protected SoraApi Api => Info.Api;
    protected bool IsGroup => Info.MessageType == MessageInfoType.Group;
    protected string[] Command => Info.CommandWithoutPrefix;
    protected int CommandLength => Info.CommandWithoutPrefix.Length;
    protected BotUserInfo? User => Info.UserInfo.Value;
    protected RobotReply.RobotReply RobotReply => Info.RobotReply;
}
