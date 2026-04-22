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
    public class SteelBar() : BaseBarItem<SteelBarTile>(Item.sellPrice(0, 0, 20), ItemRarityID.LightRed, AssetDirectory.SteelItems), IMagikeCraftable, IConsultableItem
    {
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<SteelKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<SteelPage1>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 4)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.IronOre, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45), 2)
                .AddIngredient(ItemID.TitaniumOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.IronOre, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45), 2)
                .AddIngredient(ItemID.AdamantiteOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.IronBar, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45))
                .AddIngredient(ItemID.TitaniumOre)
                .AddIngredient(ItemID.Coal)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.IronOre, ModContent.ItemType<SteelBar>()
                , MagikeHelper.CalculateMagikeCost<BrilliantLevel>(3, 45))
                .AddIngredient(ItemID.AdamantiteOre)
                .AddIngredient(ItemID.Coal)
                .Register();
        }

        //public void AddMagikePolymerizeRecipe()
        //{
        //    PolymerizeRecipe.CreateRecipe<SteelBar>(300)
        //        .SetMainItem(ItemID.IronBar, 12)
        //        .AddIngredient(ItemID.Coal, 6)
        //        .AddIngredient<HeatanInABottle>()
        //        .AddCondition(HardModeCondition.Instance)
        //        .Register();
        //}
    }

    public class SteelBarTile() : BaseBarTile(AssetDirectory.SteelItems)
    {
        public override int GetDustType() => DustID.PlatinumCoin;
        public override Color GetMapColor() => new Color(150, 150, 150);
    }
}
