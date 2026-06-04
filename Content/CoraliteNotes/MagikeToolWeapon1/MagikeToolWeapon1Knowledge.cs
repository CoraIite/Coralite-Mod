using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;

namespace Coralite.Content.CoraliteNotes.MagikeToolWeapon1
{
    public class MagikeToolWeapon1Knowledge : Knowledge
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + nameof(CrystalSword);
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeToolWeaponPage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Rune;

        public override UIPage[] GetUIPages()
        {
            return [
                new MagikeToolWeaponPage1(),
                ];
        }
    }
}
