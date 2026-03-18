using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.BookUI;
using Coralite.Core.Systems.KeySystem;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.CoraliteActivities
{
    public class CoraliteActivitiesKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<ActivityDescriptionPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Coral;

        public override UIPageGroup GetUIPageGroup() => new GroupCoraliteActivities();

        public override void LoadData(KnowledgePlayer player, TagCompound tag)
        {
            Unlock = true;
        }

        public override void OnEnterWorld()
        {
            Unlock = true;
        }
    }
}
