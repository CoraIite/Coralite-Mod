using Coralite.Content.Tiles.Plants;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items
{
    public class C : BaseSeed
    {
        public override string Texture => "Coralite/Content/Items/C";
        public C() : base("C", "”C“", 999, Item.sellPrice(0, 0, 0, 1), ItemRarityID.White, 15, 15, 0, 0, ModContent.TileType<AgropyronFrozen>()) { }
    }
}
