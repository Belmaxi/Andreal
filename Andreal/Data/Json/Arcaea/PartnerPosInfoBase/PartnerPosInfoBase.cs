﻿using System.Collections.Concurrent;
using AndrealClient.Data.Sqlite;
using Newtonsoft.Json;
using Path = AndrealClient.Core.Path;

namespace AndrealClient.Data.Json.Arcaea.PartnerPosInfoBase;

public class PosInfoItem
{
    public string Partner { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public int Size { get; set; }
}

internal static class PartnerPosInfoBase
{
    private static readonly Lazy<ConcurrentDictionary<string, List<PosInfoItem>>> Locations
        = new(() =>
                  new(JsonConvert
                          .DeserializeObject<
                              Dictionary<string, List<PosInfoItem>>>(File.ReadAllText(Path.PartnerConfig))!));

    private static readonly Lazy<ConcurrentDictionary<string, Dictionary<string, PosInfoItem>>> Dict
        = new(() => new(Init()));

    private static Dictionary<string, Dictionary<string, PosInfoItem>> Init()
    {
        var ls = new Dictionary<string, Dictionary<string, PosInfoItem>>();
        foreach (var (key, value) in Locations.Value) ls.Add(key, value.ToDictionary(i => i.Partner));
        return ls;
    }

    private static readonly PosInfoItem ImgV1 = new() { PositionX = 770, PositionY = 58, Size = 950 };
    private static readonly PosInfoItem ImgV2 = new() { PositionX = 850, PositionY = 0, Size = 1400 };
    private static readonly PosInfoItem ImgV4 = new() { PositionX = 550, PositionY = 50, Size = 1500 };

    internal static PosInfoItem? Get(string partner, BotUserInfo.ImgVersion imgVersion)
    {
        return imgVersion switch
               {
                   BotUserInfo.ImgVersion.ImgV1 => Dict.Value["1"].TryGetValue(partner, out var result)
                       ? result
                       : ImgV1,
                   BotUserInfo.ImgVersion.ImgV2 => Dict.Value["2"].TryGetValue(partner, out var result)
                       ? result
                       : ImgV2,
                   BotUserInfo.ImgVersion.ImgV4 => Dict.Value["4"].TryGetValue(partner, out var result)
                       ? result
                       : ImgV4,
                   _ => null
               };
    }
}
