using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public abstract class BaseGemItem(int value, int rare, string texturePath, bool pathHasName = false) : BaseMaterial(Item.CommonMaxStack, value, rare, texturePath, pathHasName),IConsultableItem
    {
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<LandOfTheLustrousKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<LandOfTheLustrousPage2>();

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = CoraliteSoundID.Ding_Item4;
        }
    }
}
