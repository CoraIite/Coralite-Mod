using Coralite.Content.Tiles.Gel;
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
            Item.DefaultToPlaceableTile(ModContent.TileType<GelFiberTile>());
        }
    }
}
