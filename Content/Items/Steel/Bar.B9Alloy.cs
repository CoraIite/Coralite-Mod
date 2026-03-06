using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B9Alloy : BaseMaterial, IMagikeCraftable
    {
        public B9Alloy() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 20), ItemRarityID.Pink, AssetDirectory.SteelItems)
        {
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;

            //ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 5, ItemID.ChlorophyteBar, 2);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LeadBar, 4)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.LeadOre, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45), 2)
                .AddIngredient(ItemID.TitaniumOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.LeadOre, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45), 2)
                .AddIngredient(ItemID.AdamantiteOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.LeadBar, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45))
                .AddIngredient(ItemID.TitaniumOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.LeadBar, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45))
                .AddIngredient(ItemID.AdamantiteOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            //旧版合成方式
            //MagikeRecipe.CreateCraftRecipe<SteelBar, B9Alloy>(MagikeHelper.CalculateMagikeCost<BrilliantLevel>(4, 20), 2, 2)
            //                .AddIngredient<IcicleCrystal>()
            //                .AddIngredient(ItemID.HellstoneBar)
            //                .Register();
        }
    }
}
