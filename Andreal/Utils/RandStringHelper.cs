using System.Text;

namespace AndrealClient.Utils;

internal static class RandStringHelper
{
    private static readonly Random Random = new();

    private static readonly string Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

    public static string GetRandString(int length = 10)
    {
        var res = new StringBuilder();
        for (var i = 0; i < length; i++) res.Append(Chars[Random.Next(36)]);
        return res.ToString();
    }
}