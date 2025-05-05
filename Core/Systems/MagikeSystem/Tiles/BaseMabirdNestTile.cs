using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseMabirdNestTile(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false)
        : BaseMagikeTile(width, height, mapColor, dustType, minPick, topSoild)
    {
        public override string Texture => AssetDirectory.ItemTransmitTiles + Name;

        public override CoraliteSetsSystem.MagikeTileType PlaceType => CoraliteSetsSystem.MagikeTileType.FourWayNoFilp;

        public override void QuickLoadAsset(MALevel level) { }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) { }
    }
}
