using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class Woodbine : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            ItemID.Sets.FoodParticleColors[Item.type] = [
                new Color(103, 152, 220),
                new Color(68, 87, 155),
                new Color(27, 111, 102)
            ];
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 22, BuffID.WellFed, 60 * 60 * 5);
            Item.value = Item.buyPrice(0, 3);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
