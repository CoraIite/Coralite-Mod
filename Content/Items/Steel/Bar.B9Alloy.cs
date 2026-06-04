using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.SteelChapter;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.KeySystem;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    public class B9Alloy() : BaseBarItem<B9AlloyBarTile>(Item.sellPrice(0, 0, 20), ItemRarityID.Pink, AssetDirectory.SteelItems), IMagikeCraftable, IConsultableItem
    {
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LeadBar, 4)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.LeadOre, ModContent.ItemType<B9Alloy>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45), 2)
                .AddIngredient(ItemID.TitaniumOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.LeadOre, ModContent.ItemType<B9Alloy>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45), 2)
                .AddIngredient(ItemID.AdamantiteOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.LeadBar, ModContent.ItemType<B9Alloy>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45))
                .AddIngredient(ItemID.TitaniumOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.LeadBar, ModContent.ItemType<B9Alloy>()
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

    public class B9AlloyBarTile() : BaseBarTile(AssetDirectory.SteelItems)
    {
        public override int GetDustType() => DustID.SilverCoin;
        public override Color GetMapColor() => new Color(164, 181, 186);
    }
}
