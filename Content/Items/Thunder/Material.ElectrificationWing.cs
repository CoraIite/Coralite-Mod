using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class ElectrificationWing : BaseMaterial
    {
        public ElectrificationWing() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 20, 0), ItemRarityID.Yellow, AssetDirectory.ThunderItems)
        {
        }
    }
}
