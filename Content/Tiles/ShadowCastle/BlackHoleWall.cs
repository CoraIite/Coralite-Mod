using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class BlackHoleWall : ModWall
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override void SetStaticDefaults()
        {
            WallID.Sets.CannotBeReplacedByWallSpread[Type] = true;

            CoraliteSets.Walls.ShadowCastle[Type] = true;
            DustType = DustID.Granite;
            AddMapEntry(new Microsoft.Xna.Framework.Color(50, 0, 3));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

    }
}
