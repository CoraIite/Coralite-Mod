using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public class HardBasaltWallUnsafe : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + "HardBasaltWall";
        public override void SetStaticDefaults()
        {
            Main.wallLargeFrames[Type] = 2;
            DustType = DustID.CorruptionThorns;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class HardBasaltWall : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            Main.wallLargeFrames[Type] = 2;
            DustType = DustID.CorruptionThorns;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
