using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Thunder
{
    public class ThunderStoneTile : ModTile
    {
        public override string Texture => AssetDirectory.ThunderTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = DustID.YellowTorch;

            AddMapEntry(Coralite.Instance.ThunderveinYellow);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }
    }
}
