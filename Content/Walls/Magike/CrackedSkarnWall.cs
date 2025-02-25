using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public class CrackedSkarnWall : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.BorealWood_Small;
            AddMapEntry(new Color(49, 61, 71));
            Main.wallHouse[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class CrackedSkarnWallUnsafe : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + "CrackedSkarnWall";

        public override void SetStaticDefaults()
        {
            DustType = DustID.BorealWood_Small;
            WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[Type] = true;
            WallID.Sets.AllowsPlantsToGrow[Type] = true;
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(49, 61, 71));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
