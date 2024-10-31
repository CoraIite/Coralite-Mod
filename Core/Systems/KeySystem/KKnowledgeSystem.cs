using Coralite.Core.Loaders;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    public class KKnowledgeSystem: ModSystem
    {
        public override void PostSetupContent()
        {
            KeyKnowledgeLoader.SetUp();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            foreach (var knowledge in KeyKnowledgeLoader.knowledges)
            {
                knowledge.SaveWorldData(tag);
            }
        }

        public override void LoadWorldData(TagCompound tag)
        {
            foreach (var knowledge in KeyKnowledgeLoader.knowledges)
            {
                knowledge.LoadWorldData(tag);
            }
        }
    }
}
