using Coralite.Core.Systems.FairyCatcherSystem;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class FairyLoader
    {
        internal static IList<Fairy> fairys;
        internal static int FairyCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取仙灵
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Fairy GetFairy(int type)
                 => type < FairyCount ? fairys[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveParticleID() => FairyCount++;

        internal static void Unload()
        {
            foreach (var item in fairys)
            {
                item.Unload();
            }

            fairys.Clear();
            fairys = null;
            FairyCount = 0;
        }
    }
}
