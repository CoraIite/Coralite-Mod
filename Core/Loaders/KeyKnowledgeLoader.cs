using Coralite.Core.Systems.KeySystem;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class KeyKnowledgeLoader
    {
        internal static FrozenDictionary<int,KeyKnowledge> knowledgesF;
        internal static Dictionary<int,KeyKnowledge> knowledges = [];

        /// <summary>
        /// 根据ID获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static KeyKnowledge GetKeyKnowledge(int type)
        {
            if(knowledgesF.TryGetValue(type,out var knowledge))
                return knowledge;

            return null;
        }

        internal static void SetUp()
        {
            if (knowledges == null)
                return;

            knowledgesF = knowledges.ToFrozenDictionary();
            knowledges = null;

            foreach (var knowledge in knowledgesF)
                knowledge.Value.SetUp();
        }

        internal static void Unload()
        {
            foreach (var item in knowledgesF)
            {
                item.Value.Unload();
            }

            knowledgesF = null;
        }
    }
}
