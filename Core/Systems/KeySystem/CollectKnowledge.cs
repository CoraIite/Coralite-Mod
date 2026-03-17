using Coralite.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    public abstract class CollectKnowledge : Knowledge
    {
        /// <summary>
        /// 是否领取奖励
        /// </summary>
        public bool RewardGetted { get; set; }
        public bool[] Collects { get; private set; }

        /// <summary>
        /// 数组长度
        /// </summary>
        /// <returns></returns>
        public abstract int GetCollectsCount();

        /// <summary>
        /// 奖励物品们
        /// </summary>
        public abstract IEnumerable<Item> GetRewardItemTypes();
        /// <summary>
        /// 主要奖励物品类型，用于UI中绘制
        /// </summary>
        public abstract int MainRewardItemType { get; }

        public override void Load()
        {
            Collects = new bool[GetCollectsCount()];
        }

        /// <summary>
        /// 获得收集品奖励
        /// </summary>
        /// <param name="rewardType"></param>
        /// <param name="itemType"></param>
        public void GetReward()
        {
            foreach (var item in GetRewardItemTypes())
                Main.LocalPlayer.QuickSpawnItem(new EntitySource_WorldEvent(), item);

            Helper.PlayPitched("UI/Success", 0.4f, 0);
        }

        public override void OnEnterWorld()
        {
            Array.Fill(Collects, false);
            RewardGetted = false;
        }

        public override void SaveData(TagCompound tag)
        {
            tag.SaveBools(Name + nameof(Collects), Collects);
            if (RewardGetted)
                tag.Add(Name + "RGetted", true);
        }

        public override void LoadData(TagCompound tag)
        {
            tag.LoadBools(Name + nameof(Collects), Collects);
            RewardGetted = tag.ContainsKey(Name + "RGetted");
        }
    }
}
