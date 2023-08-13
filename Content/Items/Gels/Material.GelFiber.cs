using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class GelFiber : BaseMaterial
    {
        public GelFiber() : base(9999, 0, ItemRarityID.White, AssetDirectory.GelItems) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.consumable = true;
            Item.createTile = ModContent.TileType<GelFiberTile>();
        }
    }

    public class GelFiberTile : ModTile
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            //Main.tileBlockLight[Type] = true;
           
            DustType = DustID.t_Slime;

            AddMapEntry(new Microsoft.Xna.Framework.Color(0, 138, 122));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }
    }
}
