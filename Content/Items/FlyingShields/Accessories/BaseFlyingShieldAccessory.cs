using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlyingShieldChapter;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public abstract class BaseFlyingShieldAccessory(int rare, int value) : BaseAccessory(rare, value), IConsultableItem
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlyingShieldKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<FlyingShieldAccessoryPage1>();
    }
}
