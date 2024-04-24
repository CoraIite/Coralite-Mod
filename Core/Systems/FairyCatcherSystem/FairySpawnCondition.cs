using Coralite.Core.Loaders;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial record FairySpawnController(int fairyType)
    {
        public int fairyType = fairyType;

        public List<FairySpawnCondition> Conditions;

        public Fairy SpawnFairy(FairyAttempt attempt)
        {
            Fairy fairy = FairyLoader.GetFairy(fairyType).NewInstance();
            fairy.Spawn(attempt);
            return fairy;
        }

        public enum WallGroupType
        {

        }

        /// <summary>
        /// 检测所有的Condition，如果有一个返回false那么就直接返回
        /// </summary>
        /// <returns></returns>
        public bool CheckCondition(FairyAttempt attempt)
        {
            if (Conditions != null)
            {
                foreach (var condition in Conditions)
                    if (!condition.Predicate(attempt))
                        return false;
            }

            return true;
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="fairyType"></param>
        /// <returns></returns>
        public static FairySpawnController Create(int fairyType)
        {
            return new FairySpawnController(fairyType);
        }

        ///// <summary>
        ///// 加条件
        ///// </summary>
        ///// <param name="conditions"></param>
        ///// <returns></returns>
        //public FairySpawnCondition AddCondition(params Condition[] conditions)
        //{
        //    this.conditions ??= new List<Condition>();
        //    this.conditions.AddRange(conditions);

        //    return this;
        //}

        ///// <summary>
        ///// 加条件
        ///// </summary>
        ///// <param name="conditions"></param>
        ///// <returns></returns>
        //public FairySpawnCondition AddCondition(Condition condition)
        //{
        //    conditions ??= new List<Condition>();
        //    conditions.Add(condition);

        //    return this;
        //}

        /// <summary>
        /// 加入特殊条件，可以自定义
        /// </summary>
        /// <param name="description"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public FairySpawnController AddCondition(LocalizedText description, Func<FairyAttempt,bool> predicate)
        {
            Conditions ??= new List<FairySpawnCondition>();
            Conditions.Add(new FairySpawnCondition(description, predicate));

            return this;
        }

        /// <summary>
        /// 加入特殊条件，尽量使用现成的
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public FairySpawnController AddCondition(FairySpawnCondition condition)
        {
            Conditions ??= new List<FairySpawnCondition>();
            Conditions.Add(condition);

            return this;
        }

        /// <summary>
        /// 注册到指定的墙壁
        /// </summary>
        /// <param name="wallType"></param>
        public void RegisterToWall(int wallType = 0)
        {
            if (FairySystem.fairySpawnConditions == null)
                return;

            if (!FairySystem.fairySpawnConditions.ContainsKey(wallType))
                FairySystem.fairySpawnConditions.Add(wallType, new List<FairySpawnController>());
            
            FairySystem.fairySpawnConditions[wallType].Add(this);
            FairySystem.fairySpawnConditions_InEncyclopedia[fairyType] = this;
        }

        public void RegisterToWallGroup(WallGroupType group)
        {

        }
    }
}
