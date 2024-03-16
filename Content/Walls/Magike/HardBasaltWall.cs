using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public class HardBasaltWall : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;
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
}
