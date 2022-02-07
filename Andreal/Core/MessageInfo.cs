using System.Reflection;
using AndrealClient.AndreaMessage;
using AndrealClient.Data.Sqlite;
using AndrealClient.Executor;
using AndrealClient.RobotReply;
using AndrealClient.Utils;
using Newtonsoft.Json;
using Sora.Entities;
using Sora.Entities.Base;
using Sora.Entities.Segment;
using Sora.Enumeration.EventParamsType;

namespace AndrealClient.Core;

[Serializable]
internal class MessageInfo
{
    internal SoraApi Api { get; set; }
    internal long FromGroup { get; private set; }
    internal long FromQq { get; private set; }
    internal string[] CommandWithoutPrefix { get; private set; }
    internal MessageInfoType MessageType { get; private set; }
    private MessageChain ReplyMessages { get; set; }
    private long MessageID { get; set; }

    internal Lazy<BotUserInfo?> UserInfo => new(() => BotUserInfo.Get(FromQq));

    internal RobotReply.RobotReply RobotReply => _reply.Value;

    private static Lazy<RobotReply.RobotReply> _reply
        = new(() => JsonConvert.DeserializeObject<RobotReply.RobotReply>(File.ReadAllText(Path.RobotReply))!);

    
    private static MessageBody FromMessageChain(MessageChain messages)
    {
        var body = new MessageBody();

        foreach (var msg in messages.ToArray())
        {
            switch (msg)
            {
                case null: continue;
                    
                case ReplyMessage reply:
                    body.Add(SoraSegment.Reply(reply.MessageId));
                    break;

                case ImageMessage img:
                    body.Add(SoraSegment.Image(img.ToString()));
                    break;

                default:
                    if (!string.IsNullOrEmpty(msg.ToString())) body.Add(msg.ToString());
                    break;
            }
        }
        return body;
    }
    
    private object SendPrivateMessage(MessageChain messages) =>
        MessageType == MessageInfoType.Friend
            ? Api.SendPrivateMessage( FromQq, FromMessageChain(messages))
            : Api.SendTemporaryMessage( FromQq, FromGroup, FromMessageChain(messages));

    private object SendGroupMessage(MessageChain messages)
    {
        if (MessageType == MessageInfoType.Friend) return -1;
        return Api.SendGroupMessage( FromGroup, FromMessageChain(messages));
    }

    internal async Task<bool> PermissionCheck() =>
        (await Api.GetGroupMemberInfo(FromGroup, FromQq)).memberInfo.Role > MemberRoleType.Member;

    internal void SendMessage(MessageChain? message)
    {
        if (message is null) return;
        _ = FromGroup != 0 && MessageType == MessageInfoType.Group
            ? SendGroupMessage(message.Prepend(new ReplyMessage((int)MessageID)))
            : SendPrivateMessage(message);
    }

    internal void SendMessageOnly(MessageChain? message)
    {
        if (message is null) return;
        _ = FromGroup != 0 && MessageType == MessageInfoType.Group
            ? SendGroupMessage(message)
            : SendPrivateMessage(message);
    }

    private void SendMessage()
    {
        try
        {
            SendMessage(ReplyMessages);
            ++BotStatementHelper.ProcessCount;
        }
        catch (Exception e)
        {
            SendMessage(RobotReply.SendMessageFailed);
            Reporter.ExceptionReport(e, FromQq);
        }
    }

    public static void Process(SoraApi api, int messageType, long fromGroup, long fromQq,
                               string message, long messageid)
    {
        Task.Run(() =>
                 {
                     if ((MessageInfoType)messageType != MessageInfoType.Group)
                         ++BotStatementHelper.PrivateMessageCount;
                     else
                         ++BotStatementHelper.GroupMessageCount;
                     var rMsg = Replace(message);

                     foreach (var pair in MethodPrefixs)
                     {
                         var match
                             = pair.Value.FirstOrDefault(j => rMsg.StartsWith(j, StringComparison.OrdinalIgnoreCase));
                         if (match != default)
                         {
                             var (executor, method) = pair.Key;
                             var info = new MessageInfo
                                        {
                                            Api = api,
                                            MessageType = (MessageInfoType)messageType,
                                            CommandWithoutPrefix
                                                = rMsg.Substring(match.Length)
                                                      .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                                       FromQq = fromQq,
                                            FromGroup = fromGroup,
                                            MessageID = messageid
                                        };

                             try
                             {
                                 var result = method.Invoke(Activator.CreateInstance(executor, info), null);
                                 info.ReplyMessages = result as MessageChain ?? (result as Task<MessageChain>)?.Result;
                             }
                             catch (TargetInvocationException e)
                             {
                                 info.ReplyMessages = e.InnerException switch
                                                      {
                                                          HttpRequestException exception => 
                                                          $"{info.RobotReply.APIQueryFailed}({exception.Message})",
                                                          _ => info.RobotReply.ExceptionOccured(e.InnerException)
                                                      };
                                 Reporter.ExceptionReport(e.InnerException, info.FromQq);
                             }
                             catch (AggregateException e)
                             {
                                 info.ReplyMessages = e.InnerException switch
                                                      {
                                                          JsonReaderException =>   $"{info.RobotReply.APIQueryFailed}",
                                                          HttpRequestException exception =>   $"{info.RobotReply.APIQueryFailed}({exception.Message})",
                                                          _ => info.RobotReply.ExceptionOccured(e.InnerException)
                                                      };
                                 Reporter.ExceptionReport(e.InnerException, info.FromQq);
                             }
                             catch (Exception e)
                             {
                                 info.ReplyMessages = info.RobotReply.ExceptionOccured(e);
                                 Reporter.ExceptionReport(e, info.FromQq);
                             }
                             finally
                             {
                                 info.SendMessage();
                             }

                             return;
                         }
                     }
                 });
    }

    private static string Replace(string rawMessage)
    {
        rawMessage = rawMessage.Trim();
        switch (rawMessage)
        {
            case "查分":
            case "查最近":
            case "查":
            case "/a":
            case "/arc":
                return "/arc info";
            case "/o":
            case "/osu":
                return "/osu mode";
        }

        foreach (var (key, value) in
                 AbbreviationPairs.Where(i => rawMessage.StartsWith(i.Key, StringComparison.OrdinalIgnoreCase)))
            rawMessage = value + rawMessage.Substring(key.Length);

        return string.Join(" ",
                           rawMessage.Split(new char[] { '\n', '\t', '\r' }, StringSplitOptions.RemoveEmptyEntries));
    }

    private static readonly Dictionary<(Type, MethodInfo), string[]> MethodPrefixs = GetMethodPrefixs();

    private static Dictionary<(Type, MethodInfo), string[]> GetMethodPrefixs()
    {
        var ls = new Dictionary<(Type, MethodInfo), string[]>();

        foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes
                                     .Where(i => i.BaseType == typeof(ExecutorBase)))
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic
                                                   | BindingFlags.Public))
            {
                var prefixs = (method.GetCustomAttribute(typeof(CommandPrefixAttribute)) as CommandPrefixAttribute)
                    ?.Prefixs;
                if (prefixs != null) ls.Add((type, method), prefixs);
            }

        return ls;
    }

    private static readonly Dictionary<string, string> AbbreviationPairs = new()
                                                                           {
                                                                               { "/a ", "/arc " },
                                                                               { "/o ", "/osu " },
                                                                               { "/p ", "/pjsk " },
                                                                               { "/ar ", "/arc room " }
                                                                           };
}
