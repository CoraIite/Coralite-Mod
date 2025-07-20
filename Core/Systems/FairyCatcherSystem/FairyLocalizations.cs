using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.Localization;

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
            ZoneHell,
            ZoneCrimson,
            ZoneCorrupt,



            CircleR_9,

            GemWall,

            Count,
        }

        public static LocalizedText FairySpawnCondition_Wall;

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

        public static LocalizedText SkillLVTips;
        public static LocalizedText SkillLVLimit;

        public void LoadLocalization()
        {
            FieldInfo[] infos = typeof(FairySystem).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
            foreach (var info in infos)
                if (info.FieldType==typeof(LocalizedText))
                    info.SetValue(null
                        , this.GetLocalization(info.Name.Replace("_", "")));

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

            SpawnDescriptions = new LocalizedText[(int)DescriptionID.Count];

            for (int i = 0; i < (int)DescriptionID.Count; i++)
                SpawnDescriptions[i] = this.GetLocalization(nameof(SpawnDescriptions) + "." + Enum.GetName((DescriptionID)i));
        }

        public static void UnloadLocalization()
        {
            FieldInfo[] infos = typeof(FairySystem).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
            foreach (var info in infos)
                if (info.FieldType == typeof(LocalizedText))
                    info.SetValue(null, null);

            RarityText = null;
            SpawnDescriptions = null;
        }

        public static string FormatIVDescription(LocalizedText pre, LocalizedText levelText, float value)
        {
            return pre.Format(levelText.Value, value);
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
