using Coralite.Core;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class ShadowBrickWall : ModWall
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override void SetStaticDefaults()
        {
            WallID.Sets.CannotBeReplacedByWallSpread[Type] = true;
            DustType = DustID.Shadowflame;
            AddMapEntry(new Microsoft.Xna.Framework.Color(48, 18, 37));

        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

    }
}
