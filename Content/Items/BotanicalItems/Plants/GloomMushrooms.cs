using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.BotanicalItems.Plants
{
    public class GloomMushrooms : BasePlantMaterial
    {
        public GloomMushrooms() : base("吸光蘑菇", "它周围一片黑暗", 999, Item.sellPrice(0, 0, 3, 0), ItemRarityID.Green) { }
    }
}
