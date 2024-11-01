using Coralite.Core.Systems.KeySystem;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class KeyKnowledgeLoader
    {
        internal static List<KeyKnowledge> knowledges = [];

        internal static void SetUp()
        {
            knowledges.Sort((k1, k2) => k1.Type.CompareTo(k2.Type));

            foreach (var knowledge in knowledges)
            {
                knowledge.SetUp();
            }
        }

        internal static void Unload()
        {
            foreach (var item in knowledges)
            {
                item.Unload();
            }

            knowledges.Clear();
            knowledges = null;
        }
    }
}
