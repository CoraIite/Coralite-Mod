using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class ZapCrystal : BaseMaterial
    {
        public ZapCrystal() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 50, 0), ItemRarityID.Yellow, AssetDirectory.ThunderItems)
        {
        }
    }
}
