using Coralite.Core.Systems.KeySystem;
using Coralite.Core.Systems.MTBStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Core.Loaders
{
    public class KeyKnowledgeLoader
    {
        internal static IList<KeyKnowledge> knowledges;
        internal static int knowledgeCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static KeyKnowledge GetMTBStructure(int type)
                 => type < knowledgeCount ? knowledges[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveMTBStructureID() => knowledgeCount++;

        internal static void Unload()
        {
            foreach (var item in knowledges)
            {
                item.Unload();
            }

            knowledges.Clear();
            knowledges = null;
            knowledgeCount = 0;
        }

    }
}
