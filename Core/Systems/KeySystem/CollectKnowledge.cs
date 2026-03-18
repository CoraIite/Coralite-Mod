using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    public abstract class CollectKnowledge : Knowledge
    {
        /// <summary>
        /// 是否领取奖励，注意这个只能在游戏内使用！
        /// </summary>
        public bool RewardGetted
        {
            get => Main.LocalPlayer.GetModPlayer<KnowledgePlayer>().GetData<bool>(RewardGettedName);
            set => Main.LocalPlayer.GetModPlayer<KnowledgePlayer>().SetData(RewardGettedName, value);
        }
        /// <summary>
        /// 收集进度，注意这个只能在游戏内使用！
        /// </summary>
        public bool[] Collects => Main.LocalPlayer.GetModPlayer<KnowledgePlayer>().GetData<bool[]>(CollectName);

        public string CollectName => Name + nameof(Collects);
        public string RewardGettedName => Name + "RGetted";

        /// <summary>
        /// 数组长度
        /// </summary>
        /// <returns></returns>
        public abstract int GetCollectsCount();

        /// <summary>
        /// 生成的奖励物品，默认生成主要物品
        /// </summary>
        public virtual IEnumerable<Item> GetRewardItemTypes()
        {
            yield return new Item(MainRewardItemType);
        }

        /// <summary>
        /// 主要奖励物品类型，用于UI中绘制
        /// </summary>
        public abstract int MainRewardItemType { get; }

        public override void OnPlayerInitialize(KnowledgePlayer player)
        {
            player.AddData(CollectName, new bool[GetCollectsCount()]);
            player.AddData(RewardGettedName, false);
        }

        /// <summary>
        /// 获得收集品奖励
        /// </summary>
        public void GetReward()
        {
            foreach (var item in GetRewardItemTypes())
                Main.LocalPlayer.QuickSpawnItem(new EntitySource_WorldEvent(), item);

            Helper.PlayPitched("UI/Success", 0.4f, 0);
            RewardGetted = true;
        }

        public override void SaveData(KnowledgePlayer player, TagCompound tag)
        {
            tag.SaveBools(CollectName, player.GetData<bool[]>(CollectName));
            if (player.GetData<bool>(RewardGettedName))
                tag.Add(RewardGettedName, true);
        }

        public override void LoadData(KnowledgePlayer player, TagCompound tag)
        {
            tag.LoadBools(CollectName, player.GetData<bool[]>(CollectName));
            player.SetData(RewardGettedName, tag.ContainsKey(RewardGettedName));
        }
    }
}
