﻿using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Content.CoraliteNotes
{
    public class CoraliteNoteSystem : ModSystem, ILocalizedModType
    {
        public static bool[] CollectRewards = new bool[(int)RewardType.Count];

        /// <summary> 收集进度 </summary>
        public static LocalizedText CollectProgress;
        /// <summary> 全收集！ </summary>
        public static LocalizedText AllCollect;

        /// <summary> 收集所有物品吧！ </summary>
        public static LocalizedText CollectAllItems;
        /// <summary> 点击获得奖励 </summary>
        public static LocalizedText ClickGetReward;
        /// <summary> 奖励已收集 </summary>
        public static LocalizedText RewardCollected;

        public string LocalizationCategory => "Systems";

        public enum RewardType
        {
            FlyingShield,

            Count,
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            CollectProgress = this.GetLocalization(nameof(CollectProgress));
            AllCollect = this.GetLocalization(nameof(AllCollect));
            CollectAllItems = this.GetLocalization(nameof(CollectAllItems));
            ClickGetReward = this.GetLocalization(nameof(ClickGetReward));
            RewardCollected = this.GetLocalization(nameof(RewardCollected));
        }

        /// <summary>
        /// 获得收集品奖励
        /// </summary>
        /// <param name="rewardType"></param>
        /// <param name="itemType"></param>
        public static void GetReward(RewardType rewardType, int itemType)
        {
            CollectRewards[(int)rewardType] = true;
            Main.LocalPlayer.QuickSpawnItem(new EntitySource_WorldEvent(), itemType);

            Helper.PlayPitched("UI/Success", 0.4f, 0);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.SaveBools(CollectRewards, nameof(CollectRewards));
            FlyingShieldChapter.FlyingShieldCollect.Save(tag);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            tag.LoadBools(CollectRewards, nameof(CollectRewards));
            FlyingShieldChapter.FlyingShieldCollect.Load(tag);
        }
    }
}
