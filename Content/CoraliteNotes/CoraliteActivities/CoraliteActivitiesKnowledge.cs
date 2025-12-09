using Coralite.Core.Systems.KeySystem;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.CoraliteActivities
{
    public class CoraliteActivitiesKnowledge : KeyKnowledge
    {
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<ActivityDescriptionPage>();

        public override void SetUp()
        {
            Unlock = true;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            Unlock = true;
        }
    }
}
