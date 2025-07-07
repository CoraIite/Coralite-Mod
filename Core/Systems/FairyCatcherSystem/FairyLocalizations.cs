using System;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial class FairySystem
    {
        //-----------------------------------
        //         个体值评级的本地化
        //-----------------------------------
        public static LocalizedText WeakLevel;
        public static LocalizedText WeakCommonLevel;
        public static LocalizedText CommonLevel;
        public static LocalizedText UncommonLevel;
        public static LocalizedText RareLevel;
        public static LocalizedText EpicLevel;
        public static LocalizedText LegendaryLevel;
        public static LocalizedText EternalLevel;

        public static LocalizedText OverLevel;

        //-----------------------------------
        //         个体描述的本地化
        //-----------------------------------
        public static LocalizedText FairyLifeMax;
        public static LocalizedText FairyDamage;
        public static LocalizedText FairyDefence;
        public static LocalizedText FairySpeed;
        public static LocalizedText FairySkillLevel;
        public static LocalizedText FairyStamina;
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

        public enum DescriptionID
        {
            ZoneForest = 0,
            ZoneRockLayer,
            ZoneBeach,
            ZoneDesert,



            CircleR_9,

            Count,
        }

        //-----------------------------------
        //       仙灵物品其他文本的本地化
        //-----------------------------------
        public static LocalizedText CurrentLife;
        public static LocalizedText ResurrectionTime;
        public static LocalizedText BottleFightCapacity;
        public static LocalizedText BottleContainCapacity;
        public static LocalizedText SeeMore;
        public static LocalizedText CatchPowerMult;
        public static LocalizedText UncaughtMouseText;
        public static LocalizedText SelectButtonMouseText;
        public static LocalizedText SortButtonMouseText;

        public static LocalizedText SortByTypeText;
        public static LocalizedText SortByRarityText;
        public static LocalizedText SortByCaughtText;
        public static LocalizedText FairyTradeCondition;

        public static LocalizedText CircleRadiusBonus;
        public static LocalizedText FightFairy;
        public static LocalizedText ContainsFairy;
        public static LocalizedText PutFairyBottleIn;

        public static LocalizedText HowToSort;
        public static LocalizedText SortByRarity;
        public static LocalizedText SortByLifeMax;
        public static LocalizedText SortByDamage;
        public static LocalizedText SortByDefence;
        public static LocalizedText SortBySpeed;
        public static LocalizedText SortBySkillLevel;
        public static LocalizedText SortByStamina;
        public static LocalizedText BottleHeal;

        public void LoadLocalization()
        {
            WeakLevel = this.GetLocalization("WeakLevel");
            WeakCommonLevel = this.GetLocalization("WeakCommonLevel");
            CommonLevel = this.GetLocalization("CommonLevel");
            UncommonLevel = this.GetLocalization("UncommonLevel");
            RareLevel = this.GetLocalization("RareLevel");
            EpicLevel = this.GetLocalization("EpicLevel");
            LegendaryLevel = this.GetLocalization("LegendaryLevel");
            EternalLevel = this.GetLocalization("EternalLevel");

            OverLevel = this.GetLocalization("OverLevel");

            FairyLifeMax = this.GetLocalization("FairyLifeMax");
            FairyDamage = this.GetLocalization("FairyDamage");
            FairyDefence = this.GetLocalization("FairyDefence");
            FairySpeed = this.GetLocalization("FairySpeed");
            FairySkillLevel = this.GetLocalization("FairySkillLevel");
            FairyStamina = this.GetLocalization("FairyStamina");
            FairyScale = this.GetLocalization("FairyScale");

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

            SpawnDescriptions = new LocalizedText[(int)DescriptionID.Count];

            for (int i = 0; i < (int)DescriptionID.Count; i++)
                SpawnDescriptions[i] = this.GetLocalization(nameof(SpawnDescriptions) + "." + Enum.GetName((DescriptionID)i));

            CurrentLife = this.GetLocalization("CurrentLife", () => "当前生命值：{0} / {1}");
            ResurrectionTime = this.GetLocalization("ResurrectionTime", () => "复活时间：{0}");
            BottleFightCapacity = this.GetLocalization("BottleCapacity", () => "出战仙灵 {0} / {1}");
            BottleContainCapacity = this.GetLocalization("BottleCapacity", () => "容纳仙灵 {0} / {1}");
            SeeMore = this.GetLocalization("SeeMore", () => "按上键以查看雷达图（游戏暂停时无法使用）");
            CatchPowerMult = this.GetLocalization("CatchPowerMult", () => "{0} 捕捉力");
            UncaughtMouseText = this.GetLocalization("UncaughtMouseText", () => "???\n点击以查看捕获条件");
            SelectButtonMouseText = this.GetLocalization("SelectButtonMouseText", () => "左键单击以筛选指定稀有度的仙灵\n右键恢复为显示全部");
            SortButtonMouseText = this.GetLocalization("SortButtonMouseText", () => "左键单击以按指定规则排序\n右键恢复为默认按内部ID排序");

            SortByTypeText = this.GetLocalization("SortByTypeText", () => "按内部ID排序");
            SortByRarityText = this.GetLocalization("SortByRarityText", () => "按稀有度排序");
            SortByCaughtText = this.GetLocalization("SortByCaughtText", () => "按是否捕获排序");
            FairyTradeCondition = this.GetLocalization("FairyTradeCondition", () => "使用妖精传送门进行交易");
            CircleRadiusBonus = this.GetLocalization("CircleRadiusBonus");

            FightFairy = this.GetLocalization("FightFairy");
            ContainsFairy = this.GetLocalization("ContainsFairy");
            PutFairyBottleIn = this.GetLocalization("PutFairyBottleIn");

            HowToSort = this.GetLocalization("HowToSort");
            SortByRarity = this.GetLocalization("SortByRarity");
            SortByLifeMax = this.GetLocalization("SortByLifeMax");
            SortByDamage = this.GetLocalization("SortByDamage");
            SortByDefence = this.GetLocalization("SortByDefence");
            SortBySpeed = this.GetLocalization("SortBySpeed");
            SortBySkillLevel = this.GetLocalization("SortBySkillLevel");
            SortByStamina = this.GetLocalization("SortByStamina");
            BottleHeal = this.GetLocalization("BottleHeal");
        }

        public static void UnloadLocalization()
        {
            WeakLevel = null;
            WeakCommonLevel = null;
            CommonLevel = null;
            UncommonLevel = null;
            RareLevel = null;
            EpicLevel = null;
            LegendaryLevel = null;
            EternalLevel = null;

            OverLevel = null;

            FairyLifeMax = null;
            FairyDamage = null;
            FairyDefence = null;
            FairyScale = null;

            RarityText = null;

            Rarity_SP = null;

            SpawnDescriptions = null;

            CurrentLife = null;
            ResurrectionTime = null;
            BottleFightCapacity = null;
            SeeMore = null;
            CatchPowerMult = null;
            UncaughtMouseText = null;
            SelectButtonMouseText = null;
            SortButtonMouseText = null;

            SortByTypeText = null;
            SortByRarityText = null;
            SortByCaughtText = null;
            FairyTradeCondition = null;
            CircleRadiusBonus = null;
        }

        public static string FormatIVDescription(LocalizedText pre, LocalizedText levelText, float value)
        {
            return pre.Value + levelText.Format(value);
        }

        public static LocalizedText GetSpawnCondition(DescriptionID descriptionID)
            => SpawnDescriptions[(int)descriptionID];

        public static string GetIVLevel(int level,float value)
        {
            return level switch
            {
                FairyIVLevelID.Weak => WeakLevel.Format(value),
                FairyIVLevelID.WeakCommon => WeakCommonLevel.Format(value),
                FairyIVLevelID.Common => CommonLevel.Format(value),
                FairyIVLevelID.Uncommon => UncommonLevel.Format(value),
                FairyIVLevelID.Rare => RareLevel.Format(value),
                FairyIVLevelID.Epic => EpicLevel.Format(value),
                FairyIVLevelID.Legendary => LegendaryLevel.Format(value),
                FairyIVLevelID.Eternal => EternalLevel.Format(value),
                >= 100 => OverLevel.Format(value),
                _ => EternalLevel.Format(value),
            };
        }
    }
}
