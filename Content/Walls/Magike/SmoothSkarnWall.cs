using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public class SmoothSkarnWall : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.BorealWood_Small;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class SmoothSkarnWallUnsafe : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + "SmoothSkarnWall";

        public override void SetStaticDefaults()
        {
            DustType = DustID.BorealWood_Small;
            WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
