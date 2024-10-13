using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class RoyalKonjac : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.FoodParticleColors[Item.type] = [
                new Color(251, 192, 224),
                new Color(229, 30, 202),
                new Color(0, 138, 122)
            ];

            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(28, 22, BuffID.WellFed3, 60 * 60 * 7);
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Princesstrawberry>()
                .AddIngredient<EmperorGel>(2)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
