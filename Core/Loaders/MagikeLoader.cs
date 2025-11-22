using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.MagikeSystem;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class MagikeLoader
    {
        internal static IList<MagikeLevel> levels;
        internal static ushort LevelCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取等级
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MagikeLevel GetLevel(ushort type)
                 => type < LevelCount ? levels[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static ushort ReserveMagikeLevelID() => LevelCount++;

        internal static void Unload()
        {
            foreach (var item in levels)
                item.Unload();

            levels.Clear();
            levels = null;
            LevelCount = 0;
        }
    }
}
