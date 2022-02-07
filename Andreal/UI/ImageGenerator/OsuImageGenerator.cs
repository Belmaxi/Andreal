using AndrealClient.AndreaMessage;
using AndrealClient.Model.Osu;
using AndrealClient.UI.Model;
using AndrealClient.Utils;
using Path = AndrealClient.Core.Path;

namespace AndrealClient.UI.ImageGenerator;

#pragma warning disable CA1416

internal class OsuImageGenerator
{
    private readonly QueryInfo _info;

    internal OsuImageGenerator(QueryInfo info) { _info = info; }

    private Image Flag()
    {
        var pth = Path.OsuCountryFlag(_info.Country);
        if (!pth.FileExists) WebHelper.DownloadImage($"https://assets.ppy.sh/old-flags/{_info.Country}.png", pth);
        return new(pth);
    }

    internal ImageMessage Generate()
    {
        using var flag = Flag();
        using BackGround bg = new(Path.OsuBg(_info.Mode));
        var song = WebHelper.GetImage($"https://assets.ppy.sh/beatmaps/{_info.BeatmapSetId}/covers/cover.jpg");

        bg.Draw(new ImageModel(song, 0, 0), new ImageModel(flag, 40, 637),
                new TextWithStrokeModel($"★ × {Convert.ToDouble(_info.Stars):0.00}", Font.Torus32, Color.White, 480,
                                        207),
                new TextWithStrokeModel($"CS   {_info.CS}", Font.Torus28, Color.White, 715, 40),
                new TextWithStrokeModel($"AR   {_info.AR}", Font.Torus28, Color.White, 715, 70),
                new TextWithStrokeModel($"OD  {_info.OD}", Font.Torus28, Color.White, 715, 100),
                new TextWithStrokeModel($"HP   {_info.HP}", Font.Torus28, Color.White, 715, 130),
                new TextWithStrokeModel(_info.BeatmapId, Font.Torus24, Color.White, 25, 125),
                new TextWithStrokeModel(_info.Title, Font.Torus48, Color.AntiqueWhite, 20, 145),
                new TextWithStrokeModel(_info.Artist, Font.Torus24, Color.White, 25, 204),
                new TextOnlyModel(_info.Score, Font.Torus52, Color.White, 260, 280),
                new TextOnlyModel(_info.Username, Font.Torus40, Color.White, 125, 628),
                new TextOnlyModel(_info.PP + "  #" + _info.PPrank, Font.Torus32, Color.White, 195, 695),
                new TextOnlyModel(_info.Acc(), Font.Torus48, Color.OsuGray, 635, 290),
                new TextOnlyModel(_info.Combo + "x", Font.Torus48, Color.OsuGray, 570, 540),
                new TextOnlyModel(_info.Date, Font.Torus24, Color.OsuGray, 195, 766));
        if (_info.Rank != "F") bg.Draw(new ImageModel(Path.OsuRank(_info.Rank), 40, 290));

        DrawMods(bg);
        DrawScore(bg);
        return bg;
    }

    private void DrawMods(BackGround bg)
    {
        int[,] position = { { 600, 405 }, { 693, 405 }, { 786, 405 }, { 600, 475 }, { 693, 475 }, { 786, 475 } };
        var p = 0;
        for (var i = _info.Mods.Length - 1; i >= 0; --i)
        {
            var mod = _info.Mods.Length - i - 1;
            if (p == 6 || _info.Mods[i] != '1' || mod == 23 || mod == 29) continue;
            switch (mod)
            {
                case 6 when _info.Mods.Length > 9 && _info.Mods[^10] == '1':
                case 5 when _info.Mods.Length > 14 && _info.Mods[^15] == '1':
                    continue;
                default:
                    bg.Draw(new ImageModel(Path.OsuMod(mod), position[p, 0], position[p, 1]));
                    ++p;
                    break;
            }
        }
    }

    private void DrawScore(BackGround bg)
    {
        var score = _info.Mode switch
                    {
                        0 => new string[] { _info.C300, _info.Geki, _info.C100, _info.Katu, _info.C50, _info.Miss },
                        1 => new string?[] { _info.C300, null, _info.C100, null, _info.Miss },
                        2 => new string[] { _info.C300, _info.C50, _info.C100, _info.Miss },
                        3 => new string[] { _info.C300, _info.Geki, _info.Katu, _info.C100, _info.C50, _info.Miss },
                        _ => null
                    };

        int[,] position = { { 125, 390 }, { 385, 390 }, { 125, 470 }, { 385, 470 }, { 125, 550 }, { 385, 550 } };

        for (var i = 0; i < score!.Length; ++i)
            if (score[i] is not null)
                bg.Draw(new TextOnlyModel(score[i]!, Font.Torus40, Color.White, position[i, 0], position[i, 1]));
    }
}
