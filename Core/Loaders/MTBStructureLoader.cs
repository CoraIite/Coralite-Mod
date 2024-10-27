using Coralite.Core.Systems.MTBStructure;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class MTBStructureLoader
    {
        internal static IList<MultiblockStructure> structures;
        internal static int StructureCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MultiblockStructure GetMTBStructure(int type)
                 => type < StructureCount ? structures[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveMTBStructureID() => StructureCount++;

        internal static void Unload()
        {
            foreach (var item in structures)
            {
                item.Unload();
            }

            structures.Clear();
            structures = null;
            StructureCount = 0;
        }
    }
}
