using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.GlistentChapter;
using Coralite.Content.Tiles.Glistent;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Glistent
{
    public class LeafStone : BasePlaceableItem, IConsultableItem
    {
        public LeafStone() : base(Item.sellPrice(0, 0, 0, 20), ItemRarityID.Green
            , ModContent.TileType<LeafStoneTile>(), AssetDirectory.GlistentItems)
        { }

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<GlistentKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<GlistentChapterPage1>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 12)
                .AddTile(TileID.Furnaces)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Vine)
                .AddTile(TileID.Furnaces)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.VineRope, 8)
                .AddTile(TileID.Furnaces)
                .Register();
        }

        public override void UpdateInventory(Player player)
        {
            KnowledgeSystem.CheckForUnlock<GlistentKnowledge>(player.MountedCenter, Coralite.GlistentGreen);
        }

        //public void AddMagikeRemodelRecipe()
        //{
        //    MagikeSystem.AddRemodelRecipe<LeafStone>(0f, ItemID.Wood, 20, selfStack: 15);
        //}
    }
}
