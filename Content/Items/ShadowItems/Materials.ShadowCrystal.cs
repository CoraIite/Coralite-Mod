using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.ShadowItems
{
    public class ShadowCrystal : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("影水晶");

            Tooltip.SetDefault("影子能量的结晶");

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; 
            ItemID.Sets.ItemIconPulse[Item.type] = true; 
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0,0,18,0);
            Item.maxStack = 9999;
            Item.height = Item.width = 40;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Amethyst, 2);
            recipe.AddIngredient(ItemID.Obsidian, 1);
            recipe.AddIngredient(ModContent.ItemType<ShadowEnergy>(), 3);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
}
