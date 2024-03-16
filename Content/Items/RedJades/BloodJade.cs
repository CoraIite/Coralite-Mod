using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class BloodJade : BaseMaterial
    {
        public BloodJade() : base(9999, Item.sellPrice(0, 0, 5, 50), ItemRarityID.LightRed, AssetDirectory.RedJadeItems) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.consumable = true;
            Item.createTile = ModContent.TileType<BloodJadeTile>();
        }
    }
}
