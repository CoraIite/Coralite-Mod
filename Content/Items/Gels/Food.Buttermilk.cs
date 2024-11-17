using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class Buttermilk : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.FoodParticleColors[Item.type] = [
                new Color(103, 152, 220),
                new Color(68, 87, 155),
                new Color(27, 111, 102)
            ];

            ItemID.Sets.IsFood[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 32, BuffID.WellFed3, 60 * 60 * 7, true);
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MilkCarton)
                .AddIngredient<Woodbine>()
                .AddIngredient(ItemID.Gel, 4)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
