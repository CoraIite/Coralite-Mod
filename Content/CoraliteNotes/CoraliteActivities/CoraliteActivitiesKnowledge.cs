using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.UI.UILib;
using Coralite.Core.Systems.KeySystem;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes.CoraliteActivities
{
    public class CoraliteActivitiesKnowledge : Knowledge
    {
        public override string Texture => "Coralite/icon_small";
        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<ActivityDescriptionPage>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Coral;

        public override UIPage[] GetUIPages()
        {
            return [
                    new ActivityDescriptionPage(),
                    new StructrueActivityP1(),
                    new StructrueActivityP2(),
                ];
        }

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
