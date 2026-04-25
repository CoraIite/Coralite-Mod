using Coralite.Core;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes
{
    [VaultLoaden(AssetDirectory.CoraliteNote)]
    public class CoraliteNoteSystem : ModSystem, ILocalizedModType
    {
        //public static bool[] CollectRewards = new bool[(int)RewardType.Count];

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

        /// <summary> 收集提示 </summary>
        public static LocalizedText HowToCollect;
        /// <summary>
        /// 点击快进
        /// </summary>
        public static LocalizedText ClickToClose;
        /// <summary>
        /// 未绑定按键
        /// </summary>
        public static LocalizedText ButtonNotCombine;

        public string LocalizationCategory => "Systems";

        public static ATex NoteConnectLine { get; set; }
        public static ATex ItemShowMarkTex { get; set; }
        public static ATex CoraliteNoteOpenAnmi { get; set; }
        public static ATex NewTextBarBack { get; set; }
        public static ATex Water1 { get; set; }
        public static ATex CoralBack { get; set; }

        //public enum RewardType
        //{
        //    FlyingShield,
        //    FlowerGun,
        //    LandOfTheLustrous,
        //    DashBow,
        //    Sword,

        //    Count,
        //}

        public override void Load()
        {
            if (Main.dedServ)
                return;

            CollectProgress = this.GetLocalization(nameof(CollectProgress));
            AllCollect = this.GetLocalization(nameof(AllCollect));

            CollectAllItems = this.GetLocalization(nameof(CollectAllItems));
            ClickGetReward = this.GetLocalization(nameof(ClickGetReward));
            RewardCollected = this.GetLocalization(nameof(RewardCollected));

            HowToCollect = this.GetLocalization(nameof(HowToCollect));
            ClickToClose = this.GetLocalization(nameof(ClickToClose));
            ButtonNotCombine = this.GetLocalization(nameof(ButtonNotCombine));
        }


        //public override void SaveWorldData(TagCompound tag)
        //{
        //    //tag.SaveBools(CollectRewards, nameof(CollectRewards));
        //    //FlyingShieldChapter.FlyingShieldCollect.Save(tag);
        //    FlowerGunChapter.FlowerGunCollect.Save(tag);
        //    LandOfTheLustrousChapter.LandOfTheLustrousCollect.Save(tag);
        //    DashBowChapter.DashBowCollect.Save(tag);
        //    SwordChapter.SwordCollect.Save(tag);
        //}

        //public override void OnWorldLoad()
        //{
        //    //Array.Fill(CollectRewards, false);
        //    //Array.Fill(FlyingShieldChapter.FlyingShieldCollect.Unlocks, false);
        //    Array.Fill(FlowerGunChapter.FlowerGunCollect.Unlocks, false);
        //    Array.Fill(LandOfTheLustrousChapter.LandOfTheLustrousCollect.Unlocks, false);
        //    Array.Fill(DashBowChapter.DashBowCollect.Unlocks, false);
        //    Array.Fill(SwordChapter.SwordCollect.Unlocks, false);
        //}

        //public override void LoadWorldData(TagCompound tag)
        //{
        //    //tag.LoadBools(CollectRewards, nameof(CollectRewards));
        //    FlowerGunChapter.FlowerGunCollect.Load(tag);
        //    LandOfTheLustrousChapter.LandOfTheLustrousCollect.Load(tag);
        //    DashBowChapter.DashBowCollect.Load(tag);
        //    SwordChapter.SwordCollect.Load(tag);
        //}
    }
}
