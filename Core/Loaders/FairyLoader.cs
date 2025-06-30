using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    public class FairyLoader
    {
        internal static IList<Fairy> fairys;
        internal static int FairyCount { get; private set; } = 0;

        internal static IList<FairyCircleCore> cores;
        internal static int FairyCircleCoreCount { get; private set; } = 0;

        internal static IList<FairySkill> skills;
        internal static int FairySkillCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取仙灵
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Fairy GetFairy(int type)
                 => type < FairyCount ? fairys[type] : null;

        /// <summary>
        /// 根据类型获取仙灵核心
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FairyCircleCore GetFairyCircleCore(int type)
                 => type < FairyCircleCoreCount ? cores[type] : null;

        /// <summary>
        /// 根据类型获取仙灵技能
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FairySkill GetFairySkill(int type)
                 => type < FairySkillCount ? skills[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveFairyID() => FairyCount++;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveFairyCoreID() => FairyCircleCoreCount++;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveFairySkillID() => FairySkillCount++;

        internal static void Unload()
        {
            foreach (var item in fairys)
                item.Unload();

            fairys.Clear();
            fairys = null;
            FairyCount = 0;

            foreach (var item in cores)
                item.Unload();

            cores.Clear();
            cores = null;
            FairyCircleCoreCount = 0;

            foreach (var item in skills)
                item.Unload();

            skills.Clear();
            skills = null;
            FairySkillCount = 0;
        }
    }
}
