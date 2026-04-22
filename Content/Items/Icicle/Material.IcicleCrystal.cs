using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleCrystal : BaseMaterial,IConsultableItem
    {
        public IcicleCrystal() : base(9999, Item.sellPrice(0, 0, 50, 0), ItemRarityID.Orange, AssetDirectory.IcicleItems) { }

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<IceDragon1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<IciclePage1>();

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.IceMachine);
            recipe.AddIngredient<IcicleCrystal>(2);
            recipe.AddTile(TileID.Anvils);
            recipe.DisableDecraft();
            recipe.Register();

            recipe = CreateRecipe();
            recipe.ReplaceResult(ItemID.IceRod);
            recipe.AddIngredient<IcicleCrystal>(2);
            recipe.AddTile(TileID.IceMachine);
            recipe.DisableDecraft();
            recipe.Register();
        }
    }
}
