using System.Text.RegularExpressions;

namespace AndrealClient.Utils;

internal static class StringHelper
{
    private static readonly Regex Reg = new(@"\s|\(|\)|（|）", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    internal static bool Contains(this string raw, string seed) =>
        Reg.Replace(raw, "").IndexOf(Reg.Replace(seed, ""), StringComparison.OrdinalIgnoreCase) >= 0;

    internal static bool Equals(this string raw, string seed) =>
        string.Equals(Reg.Replace(raw, ""), Reg.Replace(seed, ""), StringComparison.OrdinalIgnoreCase);
}
