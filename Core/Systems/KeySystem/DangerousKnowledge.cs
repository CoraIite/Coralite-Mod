using Coralite.Content.UI.NewKnowledgeUnlock;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.KeySystem
{
    public abstract class DangerousKnowledge : Knowledge
    {
        public int ChallengeLevel
        {
            get => Main.LocalPlayer.GetModPlayer<KnowledgePlayer>().GetData<int>(ChallengeLevelName);
            set => Main.LocalPlayer.GetModPlayer<KnowledgePlayer>().SetData(ChallengeLevelName, value);
        }

        public string ChallengeLevelName => Name + "ChallengeLv";
        public string RewardsCollectName => Name + "RCollect";
        public abstract int MaxDangerousLevel { get; }

        /// <summary>
        /// 存储在世界里
        /// </summary>
        public bool[] DangerousTurnOn { get; private set; }
        public bool[] RewardsCollect
        {
            get => Main.LocalPlayer.GetModPlayer<KnowledgePlayer>().GetData<bool[]>(RewardsCollectName);
            set => Main.LocalPlayer.GetModPlayer<KnowledgePlayer>().SetData(RewardsCollectName, value);
        }
        public DangerousRewardInfo[] Rewards { get; private set; }
        public int[] DangerousLevels { get; private set; }
        public ATex[] Texes { get; private set; }
        public LocalizedText[] Texts { get; private set; }

        public struct DangerousRewardInfo(Item item, int level)
        {
            public Item item = item;
            public int level = level;
        }

        public void Initialize()
        {
            DangerousLevels = GetDangerousLevels();
            Rewards = GetRewards();

            int length = DangerousLevels.Length;
            Texes = new ATex[length];
            Texts = new LocalizedText[length];
            DangerousTurnOn = new bool[length];

            for (int i = 0; i < length; i++)
            {
                Texes[i] = ModContent.Request<Texture2D>(GetTexName(i));
                Texts[i] = this.GetLocalization(GetTextName(i));
            }
        }

        public override void OnPlayerInitialize(KnowledgePlayer player)
        {
            Initialize();
            player.AddData(ChallengeLevelName, 0);
            player.AddData(RewardsCollectName, new bool[Rewards.Length]);
        }

        public abstract int[] GetDangerousLevels();
        public abstract DangerousRewardInfo[] GetRewards();

        public virtual void SyncDangerousTrunOn() { }

        /// <summary>
        /// 获取当前的危险等级
        /// </summary>
        /// <returns></returns>
        public int GeCurrentDangerous()
        {
            int count = 0;
            for (int i = 0; i < DangerousLevels.Length; i++)
                if (DangerousTurnOn[i])
                    count += DangerousLevels[i];

            return count;
        }

        public void RecordChallenge()
        {
            int curr = GeCurrentDangerous();
            if (curr > ChallengeLevel)
                ChallengeLevel = curr;
        }

        public void RecordChallengeAndNewTip(Color color, LocalizedText name)
        {
            RecordChallenge();
            NewKnowledgeState.AddDangerousChallengeTip(this, color, name);
        }

        /// <summary>
        /// 传入索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract string GetTexName(int index);
        public abstract string GetTextName(int index);

        public override void OnEnterWorld()
        {
            Array.Fill(DangerousTurnOn, false);
        }

        public override void SaveData(KnowledgePlayer player, TagCompound tag)
        {
            tag.SaveBools(RewardsCollectName, player.GetData<bool[]>(RewardsCollectName));
            tag.Add(ChallengeLevelName, player.GetData<int>(ChallengeLevelName));
        }

        public override void LoadData(KnowledgePlayer player, TagCompound tag)
        {
            bool[] t = new bool[Rewards.Length];
            tag.LoadBools(RewardsCollectName, t);
            player.SetData(RewardsCollectName, t);

            if (tag.TryGet(ChallengeLevelName, out int value))
                player.SetData(ChallengeLevelName, value);
        }
    }
}
