using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public class MagicCrystalBrickWall : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink * 0.5f);
            Main.wallHouse[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
