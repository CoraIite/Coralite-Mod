using Coralite.Content.CoraliteNotes;
using Coralite.Core.Loaders;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    public class KKnowledgeSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            KeyKnowledgeLoader.SetUp();

            if (!Main.dedServ)
            {
                UILoader.GetUIState<CoraliteNoteUIState>().Init();
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            foreach (var knowledge in KeyKnowledgeLoader.knowledgesF)
            {
                knowledge.Value.SaveWorldData(tag);
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            foreach (var knowledge in KeyKnowledgeLoader.knowledgesF)
            {
                knowledge.Value.LoadWorldData(tag);
            }
        }
    }
}
