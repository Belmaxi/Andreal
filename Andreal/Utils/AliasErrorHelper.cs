using AndrealClient.AndreaMessage;
using AndrealClient.Core;
using AndrealClient.Model.Arcaea;

namespace AndrealClient.Utils;

internal static class AliasErrorHelper
{
    internal static TextMessage? GetSongAliasErrorMessage(RobotReply info, int status, SongInfo[] ls)
    {
        return (status switch
                {
                    -1 => info.NoSongFound,
                    -2 => ls.Aggregate(info.TooManySongFound, (cur, i) => cur + "\n" + i.SongName),
                    _  => null
                })!;
    }
}
