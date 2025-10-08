using Coralite.Core.Loaders;
using System;
using System.Collections.Generic;
using Terraria.ID;
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
            ///// <summary>
            ///// 宝石晶格墙
            ///// </summary>
            //Gemspark,
            /// <summary>
            /// 宝石石墙，是会自然生成的宝石洞的墙壁
            /// </summary>
            Gem,
            /// <summary>
            /// 泥土墙壁
            /// </summary>
            Dirt,
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
        public FairySpawnController AddCondition(LocalizedText description, Func<FairyAttempt, bool> predicate)
        {
            Conditions ??= new List<FairySpawnCondition>();
            Conditions.Add(new FairySpawnCondition(() => description.Value, predicate));

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
        /// 加入特殊条件，尽量使用现成的
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public FairySpawnController AddConditions(params FairySpawnCondition[] conditions)
        {
            Conditions ??= new List<FairySpawnCondition>();
            foreach (var condition in conditions)
                Conditions.Add(condition);

            return this;
        }

        /// <summary>
        /// 注册到指定的墙壁
        /// </summary>
        /// <param name="wallType"></param>
        public void RegisterToWall(int wallType = 0)
        {
            RegisterToWallInner(wallType);

            FairySpawnController condition = CloneSelf();
            if (FairySystem.GetWallTypeToItemType.TryGetValue(wallType, out int itemType))
                condition.AddCondition(new FairySpawn_Wall(itemType));

            FairySystem.fairySpawnConditions_InEncyclopedia[fairyType] = condition;
        }

        private void RegisterToWallInner(int wallType = 0)
        {
            if (FairySystem.fairySpawnConditions == null)
                return;

            if (!FairySystem.fairySpawnConditions.ContainsKey(wallType))
                FairySystem.fairySpawnConditions.Add(wallType, new List<FairySpawnController>());

            FairySystem.fairySpawnConditions[wallType].Add(this);
        }

        /// <summary>
        /// 手动注册到仙灵图鉴中，需要传入特殊出现条件（99%的情况是墙壁组）
        /// </summary>
        /// <param name="specialCondition"></param>
        public void RegisterToEncyclopedia(FairySpawnCondition specialCondition)
        {
            FairySpawnController condition = CloneSelf();
            condition.AddCondition(specialCondition);

            FairySystem.fairySpawnConditions_InEncyclopedia[fairyType] = condition;
        }

        /// <summary>
        /// 克隆一份自己的副本
        /// </summary>
        /// <param name="fairyType"></param>
        /// <returns></returns>
        public FairySpawnController CloneSelf()
        {
            FairySpawnController condition = Create(fairyType);
            if (Conditions != null && Conditions.Count > 0)
                condition.Conditions = [.. Conditions];

            return condition;
        }


        public void RegisterToWallGroup(WallGroupType group)
        {
            switch (group)
            {
                case WallGroupType.Gem:
                    RegisterToWallInner(WallID.AmethystUnsafe);
                    RegisterToWallInner(WallID.TopazUnsafe);
                    RegisterToWallInner(WallID.SapphireUnsafe);
                    RegisterToWallInner(WallID.EmeraldUnsafe);
                    RegisterToWallInner(WallID.RubyUnsafe);
                    RegisterToWallInner(WallID.DiamondUnsafe);

                    RegisterToWallInner(WallID.AmethystEcho);
                    RegisterToWallInner(WallID.TopazEcho);
                    RegisterToWallInner(WallID.SapphireEcho);
                    RegisterToWallInner(WallID.EmeraldEcho);
                    RegisterToWallInner(WallID.RubyEcho);
                    RegisterToWallInner(WallID.DiamondEcho);

                    RegisterToWallInner(WallID.AmberGemspark);
                    RegisterToWallInner(WallID.AmethystGemspark);
                    RegisterToWallInner(WallID.DiamondGemspark);
                    RegisterToWallInner(WallID.EmeraldGemspark);
                    RegisterToWallInner(WallID.AmberGemsparkOff);
                    RegisterToWallInner(WallID.AmethystGemsparkOff);
                    RegisterToWallInner(WallID.DiamondGemsparkOff);
                    RegisterToWallInner(WallID.EmeraldGemsparkOff);
                    RegisterToWallInner(WallID.RubyGemsparkOff);
                    RegisterToWallInner(WallID.SapphireGemsparkOff);
                    RegisterToWallInner(WallID.TopazGemsparkOff);
                    RegisterToWallInner(WallID.RubyGemspark);
                    RegisterToWallInner(WallID.SapphireGemspark);
                    RegisterToWallInner(WallID.TopazGemspark);

                    RegisterToEncyclopedia(FairySpawnCondition.GemWall);
                    break;
                case WallGroupType.Dirt:
                    RegisterToWallInner(WallID.Dirt);
                    RegisterToWallInner(WallID.Dirt1Echo);
                    RegisterToWallInner(WallID.Dirt2Echo);
                    RegisterToWallInner(WallID.Dirt3Echo);
                    RegisterToWallInner(WallID.Dirt4Echo);

                    RegisterToWallInner(WallID.DirtUnsafe);
                    RegisterToWallInner(WallID.DirtUnsafe1);
                    RegisterToWallInner(WallID.DirtUnsafe2);
                    RegisterToWallInner(WallID.DirtUnsafe3);
                    RegisterToWallInner(WallID.DirtUnsafe4);

                    RegisterToEncyclopedia(FairySpawnCondition.DirtWall);
                    break;
                default:
                    break;
            }
        }
    }
}
