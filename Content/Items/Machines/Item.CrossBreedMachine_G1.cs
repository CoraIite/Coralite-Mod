using Coralite.Content.Tiles.Machines;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Machines
{
    public class CrossBreedMachine_G1Item : ModItem
    {
        public override string Texture => AssetDirectory.MachineItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("植物杂交机1型");

            Tooltip.SetDefault("放置下后右键打开植物杂交UI，放入植物后给予魔能，点击按钮进行杂交");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.placeStyle = 0;
            Item.maxStack = 99;

            Item.autoReuse = true;
            Item.useTurn = true;
            Item.consumable = true;

            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
            Item.createTile = ModContent.TileType<CrossBreedMachine_G1>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.ShadowScale, 10)
            .AddIngredient(ItemID.Wood, 20)
            .AddTile(TileID.Anvils)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.TissueSample, 10)
            .AddIngredient(ItemID.Wood, 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}
