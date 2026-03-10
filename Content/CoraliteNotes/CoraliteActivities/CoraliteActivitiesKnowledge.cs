using Coralite.Core.Systems.KeySystem;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.CoraliteActivities
{
    public class CoraliteActivitiesKnowledge : Knowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<ActivityDescriptionPage>();

        public override void SetUp()
        {
            Unlock = true;
        }

        public override void LoadData(TagCompound tag)
        {
            Unlock = true;
        }
    }
}
