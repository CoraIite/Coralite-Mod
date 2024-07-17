using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class LegendaryCard : BaseMaterial
    {
        public LegendaryCard() : base(Item.CommonMaxStack, Item.sellPrice(0, 1), ItemRarityID.Yellow, AssetDirectory.Materials)
        {
        }
    }
}
