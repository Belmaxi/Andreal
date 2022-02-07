using System.Net;
using AndrealClient.AndreaMessage;

namespace AndrealClient.RobotReply;

public class RobotReply
{
    public string NotBind { get; set; }
    public string NotBindArc { get; set; }
    public string NotBindOsu { get; set; }
    public string NotBindPjsk { get; set; }
    public string ParameterLengthError { get; set; }
    public string ParameterError { get; set; }
    public string ConfigNotFound { get; set; }
    public string NotPlayedTheSong { get; set; }
    public string OsuUserBindFailed { get; set; }
    public string PjskUserBindFailed { get; set; }
    public string OsuModeConvertFailed { get; set; }
    public string OsuRecentNotPlayed { get; set; }
    public string ArcUidNotFound { get; set; }
    public string TooManyArcUid { get; set; }
    public string NoSongFound { get; set; }
    public string NoBydChart { get; set; }
    public string NotPlayedTheChart { get; set; }
    public string UnBindSuccess { get; set; }
    public string GotShadowBanned { get; set; }
    public string APIQueryFailed { get; set; }
    public string TooManySongFound { get; set; }
    public string JrrpResult { get; set; }
    public string SendMessageFailed { get; set; }
    public string BindSuccess { get; set; }
    public string HelpMessage { get; set; }
    public string RandSongReply { get; set; }
   
    internal readonly Func<Exception, TextMessage>  ExceptionOccured = ex => ex switch
                                                                             {
                                                                                 AggregateException exception =>
                                                                                     $"发生了未预料的错误，请稍后重试。\n({exception.InnerException!.Message})",
                                                                                 _ => $"发生了未预料的错误，请稍后重试。\n({ex.Message})"
                                                                             };
}
