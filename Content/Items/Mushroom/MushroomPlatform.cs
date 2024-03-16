using Coralite.Core;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.Mushroom
{
    public class MushroomPlatform : ModItem
    {
        public override string Texture => AssetDirectory.MushroomItems + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;
        }

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 10;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Mushroom.MushroomPlatform>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(2)
            .AddIngredient(ItemID.Mushroom)
            .Register();

            Recipe recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.Mushroom);
            recipe.AddIngredient<MushroomPlatform>(2);
            recipe.Register();
        }
    }
}