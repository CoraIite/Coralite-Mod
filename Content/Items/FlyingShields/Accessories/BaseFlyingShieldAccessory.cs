using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public abstract class BaseFlyingShieldAccessory<TPage>(int rare, int value) : BaseAccessory(rare, value), IConsultableItem
        where TPage:UIPage
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlyingShieldKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<TPage>();
    }
}
