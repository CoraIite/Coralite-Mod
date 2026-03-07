using Coralite.Core.Systems.KeySystem;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.SwordChapter
{
    public class SwordKnowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<SwordPage>();

        public override void SetUp()
        {
            Unlock = true;
        }

        public override void LoadExtraData(TagCompound tag)
        {
            base.LoadExtraData(tag);
            Unlock = true;
        }
    }
}
