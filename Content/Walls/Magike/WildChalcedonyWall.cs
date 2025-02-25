using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public class WildChalcedonyWall : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.GolfPaticle;
            AddMapEntry(new Color(150, 150, 120));
            Main.wallHouse[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class WildChalcedonyWallUnsafe : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + "WildChalcedonyWall";

        public override void SetStaticDefaults()
        {
            DustType = DustID.GolfPaticle;
            AddMapEntry(new Color(150, 150, 120));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
