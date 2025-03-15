using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public class ChalcedonyWall : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.GolfPaticle;
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(150, 150, 120));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class ChalcedonyWallUnsafe : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + "ChalcedonyWall";

        public override void SetStaticDefaults()
        {
            WallID.Sets.AllowsPlantsToGrow[Type] = true;
            DustType = DustID.GolfPaticle;
            AddMapEntry(new Color(150, 150, 120));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
