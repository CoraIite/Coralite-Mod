using Coralite.Core.Systems.MTBStructure;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class MultiblockLoader
    {
        internal static IList<Multiblock> structures;
        internal static int StructureCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Multiblock GetMTBStructure(int type)
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
