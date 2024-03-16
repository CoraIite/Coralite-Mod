using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class ShadowGlassWall : ModWall
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.wallLight[Type] = true;
            WallID.Sets.CannotBeReplacedByWallSpread[Type] = true;
            DustType = DustID.Shadowflame;
            AddMapEntry(new Microsoft.Xna.Framework.Color(68, 18, 57));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

    }
}
