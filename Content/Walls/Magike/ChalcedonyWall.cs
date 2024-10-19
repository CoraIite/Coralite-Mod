using Coralite.Core;
using Terraria.ID;

namespace Coralite.Content.Walls.Magike
{
    public class ChalcedonyWall : ModWall
    {
        public override string Texture => AssetDirectory.MagikeWalls + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.GolfPaticle;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
