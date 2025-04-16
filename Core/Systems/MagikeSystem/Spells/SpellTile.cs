using Coralite.Core.Systems.MagikeSystem.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    public abstract class SpellTile(int width, int height, Color mapColor, int dustType, int minPick = 0) : BaseMagikeTile(width, height, mapColor, dustType, minPick)
    {
        public override string Texture => AssetDirectory.MagikeSpellTiles + Name;

        public override CoraliteSets.MagikeTileType PlaceType => CoraliteSets.MagikeTileType.None;

        public override void Load()
        {
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
        }
    }
}
