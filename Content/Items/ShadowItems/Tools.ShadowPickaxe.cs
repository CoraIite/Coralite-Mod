using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.ShadowItems
{
    public class ShadowPickaxe : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("影子镐");

            Tooltip.SetDefault("影子凝聚在这把稿子中");
        }

        public override void SetDefaults()
        {
            Item.height = Item.width = 40;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.damage = 6;
            Item.useTime = 16;
            Item.useAnimation = 20;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.pick = 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
           .AddIngredient(ModContent.ItemType<ShadowCrystal>(), 8)
           .AddTile(TileID.Anvils)
           .Register();
        }
    }
}
