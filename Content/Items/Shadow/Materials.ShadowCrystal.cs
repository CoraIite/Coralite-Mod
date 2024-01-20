using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowCrystal : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 3));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 0, 18, 0);
            Item.maxStack = 9999;
            Item.height = Item.width = 40;
            //Item.useStyle = ItemUseStyleID.Swing;
            //Item.useTime = Item.useAnimation = 50;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Amethyst, 2);
            recipe.AddIngredient(ModContent.ItemType<ShadowEnergy>(), 3);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
}
