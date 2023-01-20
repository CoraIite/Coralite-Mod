using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.BotanicalItems.Plants
{
    public class Watermelon : BasePlantMaterial
    {
        public Watermelon() : base("西瓜", "用切瓜小刀丢向它来切成西瓜片", 999, Item.sellPrice(0, 0, 1, 0), ItemRarityID.Blue) { }
    }
}
