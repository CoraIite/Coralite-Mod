using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Walls
{
    public class B9AlloyWall : ModWall
    {
        public override string Texture => AssetDirectory.Walls + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.Titanium;
            Main.wallHouse[Type] = true;
            AddMapEntry(Color.DarkGray);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
