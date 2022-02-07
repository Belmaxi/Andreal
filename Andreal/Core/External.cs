using AndrealClient.Utils;
using Sora.Entities.Base;

namespace AndrealClient.Core;

[Serializable]
public static class External
{
    private static volatile bool _cacheCompleted;
    
    public static void ExceptionReport(Exception ex) { Reporter.ExceptionReport(ex); }

    public static void Process(SoraApi api,int type,  long fromGroup, long fromQq, string message,
                               long messageid = 0)
    {
        MessageInfo.Process(api,type, fromGroup, fromQq, message, messageid);
    }

    public static void Initialize()
    {
        if (_cacheCompleted) return;
        SystemHelper.Init();
        _cacheCompleted = true;
    }

    public static void Deinitialize() { _cacheCompleted = false; }
}
