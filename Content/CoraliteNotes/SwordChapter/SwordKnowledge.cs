using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.SwordChapter
{
    public class SwordKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<SwordPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Normal;

        public override UIPageGroup GetUIPageGroup() => new GroupSwordChapter();

        //public override void SetUp()
        //{
        //    Unlock = true;
        //}

        public override void LoadData(TagCompound tag)
        {
            Unlock = true;
        }

        public override void OnEnterWorld()
        {
            Unlock = true;
        }
    }
}
