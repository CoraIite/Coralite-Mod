using Coralite.Core.Loaders;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public struct FairyAttempt
    {
        /// <summary>
        /// 墙壁类型
        /// </summary>
        public int wallType;

        /// <summary>
        /// 捕捉器弹幕
        /// </summary>
        public FairyCatcherProj catcherProj;

        /// <summary>
        /// 存储概率的字典<br></br>
        /// 负数代表具体仙灵的类型，正数代表稀有度<br></br>
        /// 默认UR 5，SR 30，RR 75，R 150，U 250，C 490，总和1000
        /// </summary>
        public static Dictionary<int, int> percentDict;
        public static WeightedRandom<int> fairyRand;

        /// <summary>
        /// 玩家
        /// </summary>
        public Player Player;
        /// <summary>
        /// 捕捉器物品，是玩家的手持物品
        /// </summary>
        public readonly Item Catcher => Player.HeldItem;

        /// <summary>
        /// 玩家目前所使用的鱼饵物品
        /// </summary>
        public Item baitItem;

        /// <summary>
        /// 随机生成仙灵点位的X坐标，是物块坐标
        /// </summary>
        public int X;
        /// <summary>
        /// 随机生成仙灵点位的Y坐标，是物块坐标
        /// </summary>
        public int Y;

        /// <summary>
        /// 捕捉环半径
        /// </summary>
        public float CircleRadius;

        /// <summary>
        /// 创建一个仙灵捕获尝试
        /// </summary>
        /// <param name="catcherProj"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="wallType"></param>
        /// <returns></returns>
        public static FairyAttempt CreateFairyAttempt(FairyCatcherProj catcherProj, int x, int y, int wallType)
        {
            percentDict ??= new Dictionary<int, int>();
            percentDict.Clear();

            //添加默认的生成
            AddNormalSpawnPool();

            return new FairyAttempt()
            {
                catcherProj = catcherProj,
                X = x,
                Y = y,
                wallType = wallType,
                Player = catcherProj.Owner,
                CircleRadius= catcherProj.Owner.GetModPlayer<FairyCatcherPlayer>().FairyCatcherRadius
            };
        }

        private static void AddNormalSpawnPool()
        {
            percentDict.Add((int)FairyRarity.UR, 5);
            percentDict.Add((int)FairyRarity.SR, 30);
            percentDict.Add((int)FairyRarity.RR, 75);
            percentDict.Add((int)FairyRarity.R, 150);
            percentDict.Add((int)FairyRarity.U, 250);
            percentDict.Add((int)FairyRarity.C, 490);
        }

        public bool SpawnFairy(out Fairy fairy)
        {
            fairy = null;
            int result = GetRandResult();

            //直接生成的仙灵
            if (result <= 0)
            {
                fairy = FairyLoader.GetFairy(-result);
                return true;
            }

            FairyRarity rarity = (FairyRarity)result;

            List<FairySpawnController> currentController = new();
            if (wallType != 0 && FairySystem.fairySpawnConditions.TryGetValue(0, out List<FairySpawnController> totalController))//不是无墙壁，添加所有的无墙壁仙灵
            {
                foreach (var controller in totalController)
                    if (rarity == FairyLoader.GetFairy(controller.fairyType).Rarity && controller.CheckCondition(this))
                        currentController.Add(controller);
            }

            if (FairySystem.fairySpawnConditions.TryGetValue(wallType, out List<FairySpawnController> totalController1)
                && totalController1 != null)
            {
                //将所有可生成的仙灵添加到列表中
                foreach (var controller in totalController1)
                    if (rarity == FairyLoader.GetFairy(controller.fairyType).Rarity && controller.CheckCondition(this))
                        currentController.Add(controller);
            }

            if (currentController.Count == 0)
                return false;

            fairy = Main.rand.NextFromList(currentController.ToArray()).SpawnFairy(this);
            return true;
        }

        /// <summary>
        /// 获得结果，0和负数表示仙灵ID，正数是仙灵稀有度
        /// </summary>
        /// <returns></returns>
        public static int GetRandResult()
        {
            fairyRand = new WeightedRandom<int>();
            fairyRand.Clear();

            foreach (var item in percentDict)
                fairyRand.Add(item.Key, item.Value);

            return fairyRand.Get();
        }
    }

    public enum FairyRarity
    {
        /// <summary> 常见的，淡黄色 </summary>
        C = 1,
        /// <summary> 不常见，青绿色 </summary>
        U = 10,
        /// <summary> 稀有，天蓝色 </summary>
        R = 20,
        /// <summary> 双倍稀有，蓝色 </summary>
        RR = 30,
        /// <summary> 史诗，超级稀有，红色 </summary>
        SR = 40,
        /// <summary> 神话，究极稀有，橙色 </summary>
        UR = 50,

        //-----------------------------------------------------------------------
        //                  以下为只有使用特殊鱼饵才能够出现的稀有度
        //-----------------------------------------------------------------------

        /// <summary> 三倍稀有，蓝紫色 </summary>
        RRR = 35,
        /// <summary> 传说，Hyper稀有，粉色 </summary>
        HR = 60,
        /// <summary> 异类稀有，粉色 </summary>
        AR = 25,
        /// <summary> 秘宝，金色 </summary>
        MR = 100,
    }
}
