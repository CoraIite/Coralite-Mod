using Coralite.Content.CoraliteNotes.GlistentChapter;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Glistent
{
    public class GlistentBar() : BaseBarItem<GlistentBarTile>(Item.sellPrice(0, 0, 5,50), ItemRarityID.Green, AssetDirectory.GlistentItems)
    {
        public override void UpdateInventory(Player player)
        {
            KnowledgeSystem.CheckForUnlock<GlistentKnowledge>(player.MountedCenter, Coralite.GlistentGreen);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient<LeafStone>(3)
                .AddIngredient(ItemID.CrimtaneBar, 3)
                .AddIngredient(ItemID.Diamond)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe(3)
                .AddIngredient<LeafStone>(3)
                .AddIngredient(ItemID.DemoniteBar, 3)
                .AddIngredient(ItemID.Diamond)
                .AddTile<MagicCraftStation>()
                .Register();

            Recipe r = CreateRecipe();
            r.ReplaceResult(ItemID.LivingLoom);
            r.AddIngredient<LeafStone>()
                .AddRecipeGroup(RecipeGroupID.Wood, 12)
                .AddIngredient(ItemID.Acorn)
                .AddTile<MagicCraftStation>()
                .Register();
        }

        //public void AddMagikePolymerizeRecipe()
        //{
        //    PolymerizeRecipe.CreateRecipe<GlistentBar>(50)
        //        .SetMainItem<LeafStone>()
        //        .AddIngredient(ItemID.CrimtaneBar, 2)
        //        .AddIngredient(ItemID.Diamond)
        //        .Register();

        //    PolymerizeRecipe.CreateRecipe<GlistentBar>(50)
        //        .SetMainItem<LeafStone>()
        //        .AddIngredient(ItemID.DemoniteBar, 2)
        //        .AddIngredient(ItemID.Diamond)
        //        .Register();
        //}
    }

    public class GlistentBarTile() : BaseBarTile(AssetDirectory.GlistentItems)
    {
        public override int GetDustType() => DustID.GreenTorch;
        public override Color GetMapColor() => new Color(127, 218, 153);
    }

}
