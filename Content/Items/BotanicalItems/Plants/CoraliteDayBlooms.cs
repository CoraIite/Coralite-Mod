using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.BotanicalItems.Plants
{
    public class CoraliteDayBlooms : BasePlantMaterial
    {
        public CoraliteDayBlooms() : base("太阳花", "", 9999, Item.sellPrice(0, 0, 0, 20), ItemRarityID.White) { }
    }
}
