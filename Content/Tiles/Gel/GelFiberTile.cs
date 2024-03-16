using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Gel
{
    public class GelFiberTile : ModTile
    {
        public override string Texture => AssetDirectory.GelTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = DustID.t_Slime;

            AddMapEntry(new Microsoft.Xna.Framework.Color(0, 138, 122));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }
    }
}
