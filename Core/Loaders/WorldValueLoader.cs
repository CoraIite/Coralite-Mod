using Coralite.Core.Systems.WorldValueSystem;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    internal class WorldValueLoader
    {
        internal static IList<WorldFlag> flags;
        internal static int FlagCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static WorldFlag GetFlag(int type)
                 => type < FlagCount ? flags[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveFlagsID() => FlagCount++;

        internal static void Unload()
        {
            foreach (var item in flags)
            {
                item.Unload();
            }

            flags.Clear();
            flags = null;
            FlagCount = 0;
        }

    }
}
