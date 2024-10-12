using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class Princesstrawberry : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            ItemID.Sets.FoodParticleColors[Item.type] = [
                new Color(251, 192, 224),
                new Color(229, 30, 202),
                new Color(0, 138, 122)
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
