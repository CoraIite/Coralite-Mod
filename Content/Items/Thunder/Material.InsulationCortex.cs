using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class InsulationCortex : BaseMaterial
    {
        public InsulationCortex() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 20, 0), ItemRarityID.Yellow, AssetDirectory.ThunderItems)
        {
        }
    }
}
