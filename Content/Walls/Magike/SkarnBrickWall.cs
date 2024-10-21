using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public  class SkarnBrickWall:ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.BorealWood_Small;
            AddMapEntry(new Color(70, 100, 130));
            Main.wallHouse[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public  class SkarnBrickWallUnsafe:ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + "SkarnBrickWall";

        public override void SetStaticDefaults()
        {
            DustType = DustID.BorealWood_Small;
            AddMapEntry(new Color(70, 100, 130));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
