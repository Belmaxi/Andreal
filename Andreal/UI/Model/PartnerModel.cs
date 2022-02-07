using System.Drawing;
using AndrealClient.Data.Json.Arcaea.PartnerPosInfoBase;
using AndrealClient.Data.Sqlite;
using Path = AndrealClient.Core.Path;

namespace AndrealClient.UI.Model;

#pragma warning disable CA1416

internal class PartnerModel : IGraphicsModel
{
    private readonly ImageModel _imageModel;

    internal PartnerModel(int partner, bool awakened, BotUserInfo.ImgVersion imgVersion)
    {
        var location = PartnerPosInfoBase.Get($"{partner}{(awakened ? "u" : "")}", imgVersion)!;
        _imageModel = new(Path.ArcaeaPartner(partner, awakened).Result, location.PositionX, location.PositionY,
                          location.Size, location.Size);
    }

    void IGraphicsModel.Draw(Graphics g) { (_imageModel as IGraphicsModel).Draw(g); }
}
