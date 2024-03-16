using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowHammer : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("影子锤");

            // Tooltip.SetDefault("影子凝聚在这把锤子中");
        }

        public override void SetDefaults()
        {
            Item.height = Item.width = 40;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.damage = 6;
            Item.useTime = 12;
            Item.useAnimation = 18;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.hammer = 70;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ShadowCrystal>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
