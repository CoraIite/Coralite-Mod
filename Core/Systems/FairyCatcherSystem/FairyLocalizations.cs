using System.Collections.Generic;
using Terraria.Localization;
using FairyRarity = Coralite.Core.Systems.FairyCatcherSystem.FairyAttempt.Rarity;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial class FairySystem
    {
        //-----------------------------------
        //         个体值评级的本地化
        //-----------------------------------
        public static LocalizedText WeakLevel;
        public static LocalizedText VeryCommonLevel;
        public static LocalizedText CommonLevel;
        public static LocalizedText UncommonLevel;
        public static LocalizedText RareLevel;
        public static LocalizedText SpecialLevel;
        public static LocalizedText UniqueLevel;
        public static LocalizedText EternalLevel;

        public static LocalizedText NotHaveLevel;

        //-----------------------------------
        //         个体描述的本地化
        //-----------------------------------
        public static LocalizedText FairyLifeMax;
        public static LocalizedText FairyDamage;
        public static LocalizedText FairyDefence;
        public static LocalizedText FairyScale;

        //-----------------------------------
        //         仙灵稀有度的本地化
        //-----------------------------------
        public static Dictionary<FairyRarity, LocalizedText> RarityText;
        public static LocalizedText Rarity_SP;

        //-----------------------------------
        //         生成条件的本地化
        //-----------------------------------
        public static LocalizedText[] SpawnDescriptions { get; private set; }

        public class DescriptionID
        {
            public const int ZoneForest = 0;
            public const int ZoneRockLayer = 1;
            public const int ZoneBeach = 2;

            public const int DescriptionCount = 3;
        }

        //-----------------------------------
        //       仙灵物品其他文本的本地化
        //-----------------------------------
        public static LocalizedText CurrentLife;
        public static LocalizedText ResurrectionTime;
        public static LocalizedText BottleCapacity;
        public static LocalizedText SeeMore;
        public static LocalizedText CatchPowerMult;
        public static LocalizedText UncaughtMouseText;
        public static LocalizedText SelectButtonMouseText;
        public static LocalizedText SortButtonMouseText;

        public static LocalizedText SortByTypeText;
        public static LocalizedText SortByRarityText;
        public static LocalizedText SortByCaughtText;
        public static LocalizedText FairyTradeCondition;

        public void LoadLocalization()
        {
            WeakLevel = this.GetLocalization("WeakLevel", () => "软弱：{0}（基础 {1}）");
            VeryCommonLevel = this.GetLocalization("VeryCommonLevel", () => "普通：{0}（基础 {1}）");
            CommonLevel = this.GetLocalization("CommonLevel", () => "普通：{0}（基础 {1}）");
            UncommonLevel = this.GetLocalization("UncommonLevel", () => "少见：{0}（基础 {1}）");
            RareLevel = this.GetLocalization("RareLevel", () => "稀有：{0}（基础 {1}）");
            SpecialLevel = this.GetLocalization("SpecialLevel", () => "独特：{0}（基础 {1}）");
            UniqueLevel = this.GetLocalization("UniqueLevel", () => "唯一：{0}（基础 {1}）");
            EternalLevel = this.GetLocalization("EternalLevel", () => "永恒：{0}（基础 {1}）");

            NotHaveLevel = this.GetLocalization("NotHaveLevel", () => "无：{0}（基础 {1}）");

            FairyLifeMax = this.GetLocalization("FairyLifeMax", () => "血量 ");
            FairyDamage = this.GetLocalization("FairyDamage", () => "伤害 ");
            FairyDefence = this.GetLocalization("FairyDefence", () => "防御 ");
            FairyScale = this.GetLocalization("FairyScale", () => "大小 ");

            RarityText = new Dictionary<FairyRarity, LocalizedText>
            {
                { FairyRarity.C, this.GetLocalization("RarityC", () => "常见") },
                { FairyRarity.U, this.GetLocalization("RarityU", () => "不常见") },
                { FairyRarity.R, this.GetLocalization("RarityR", () => "稀有") },
                { FairyRarity.RR, this.GetLocalization("RarityRR", () => "双倍稀有") },
                { FairyRarity.SR, this.GetLocalization("RaritySR", () => "史诗") },
                { FairyRarity.UR, this.GetLocalization("RarityUR", () => "神话") },
                { FairyRarity.RRR, this.GetLocalization("RarityRRR", () => "三倍稀有") },
                { FairyRarity.HR, this.GetLocalization("RarityHR", () => "传说") },
                { FairyRarity.AR, this.GetLocalization("RarityAR", () => "异类") },
                { FairyRarity.MR, this.GetLocalization("RarityMR", () => "秘宝") },
            };

            Rarity_SP = this.GetLocalization("RaritySP", () => "特殊");

            SpawnDescriptions = new LocalizedText[DescriptionID.DescriptionCount];

            SpawnDescriptions[DescriptionID.ZoneForest] = this.GetLocalization(nameof(DescriptionID.ZoneForest)
                , () => "在地表森林出现");
            SpawnDescriptions[DescriptionID.ZoneRockLayer] = this.GetLocalization(nameof(DescriptionID.ZoneRockLayer)
                , () => "在洞穴中出现");
            SpawnDescriptions[DescriptionID.ZoneBeach] = this.GetLocalization(nameof(DescriptionID.ZoneBeach)
                , () => "在海边出现");

            CurrentLife = this.GetLocalization("CurrentLife", () => "当前生命值：{0} / {1}");
            ResurrectionTime = this.GetLocalization("ResurrectionTime", () => "复活时间：{0}");
            BottleCapacity = this.GetLocalization("BottleCapacity", () => "容量 {0} / {1}");
            SeeMore = this.GetLocalization("SeeMore", () => "按上键以查看雷达图（游戏暂停时无法使用）");
            CatchPowerMult = this.GetLocalization("CatchPowerMult", () => "{0} 捕捉力");
            UncaughtMouseText = this.GetLocalization("UncaughtMouseText", () => "???\n点击以查看捕获条件");
            SelectButtonMouseText = this.GetLocalization("SelectButtonMouseText", () => "左键单击以筛选指定稀有度的仙灵\n右键恢复为显示全部");
            SortButtonMouseText = this.GetLocalization("SortButtonMouseText", () => "左键单击以按指定规则排序\n右键恢复为默认按内部ID排序");

            SortByTypeText = this.GetLocalization("SortByTypeText", () => "按内部ID排序");
            SortByRarityText = this.GetLocalization("SortByRarityText", () => "按稀有度排序");
            SortByCaughtText = this.GetLocalization("SortByCaughtText", () => "按是否捕获排序");
            FairyTradeCondition = this.GetLocalization("FairyTradeCondition", () => "使用妖精传送门进行交易");
        }

        public static void UnloadLocalization()
        {
            WeakLevel = null;
            VeryCommonLevel = null;
            CommonLevel = null;
            UncommonLevel = null;
            RareLevel = null;
            SpecialLevel = null;
            UniqueLevel = null;
            EternalLevel = null;

            NotHaveLevel = null;

            FairyLifeMax = null;
            FairyDamage = null;
            FairyDefence = null;
            FairyScale = null;

            RarityText = null;

            Rarity_SP = null;

            SpawnDescriptions = null;

            CurrentLife = null;
            ResurrectionTime = null;
            BottleCapacity = null;
            SeeMore = null;
            CatchPowerMult = null;
            UncaughtMouseText = null;
            SelectButtonMouseText = null;
            SortButtonMouseText = null;

            SortByTypeText = null;
            SortByRarityText = null;
            SortByCaughtText = null;
            FairyTradeCondition = null;
        }

        public static string FormatIVDescription(LocalizedText pre, LocalizedText levelText, float @base, float bonused)
        {
            return pre.Value + levelText.Format(bonused, @base);
        }
    }
}
