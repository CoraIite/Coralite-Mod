using Terraria.Localization;

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
        public static LocalizedText TimelessLevel;

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
        public static LocalizedText Rarity_C;
        public static LocalizedText Rarity_U;
        public static LocalizedText Rarity_R;
        public static LocalizedText Rarity_RR;
        public static LocalizedText Rarity_SR;
        public static LocalizedText Rarity_UR;
        public static LocalizedText Rarity_RRR;
        public static LocalizedText Rarity_HR;
        public static LocalizedText Rarity_AR;
        public static LocalizedText Rarity_MR;
        public static LocalizedText Rarity_SP;

        //-----------------------------------
        //         生成条件的本地化
        //-----------------------------------
        public static LocalizedText ZoneForestDescription;

        //-----------------------------------
        //       仙灵物品其他文本的本地化
        //-----------------------------------
        public static LocalizedText CurrentLife;
        public static LocalizedText ResurrectionTime;


        public void LoadLocalization()
        {
            WeakLevel = this.GetLocalization("WeakLevel", () => "软弱：{0}（基础 {1}）");
            VeryCommonLevel = this.GetLocalization("VeryCommonLevel", () => "普通：{0}（基础 {1}）");
            CommonLevel = this.GetLocalization("CommonLevel", () => "普通：{0}（基础 {1}）");
            UncommonLevel = this.GetLocalization("UncommonLevel", () => "少见：{0}（基础 {1}）");
            RareLevel = this.GetLocalization("RareLevel", () => "稀有：{0}（基础 {1}）");
            SpecialLevel = this.GetLocalization("SpecialLevel", () => "独特：{0}（基础 {1}）");
            UniqueLevel = this.GetLocalization("UniqueLevel", () => "唯一：{0}（基础 {1}）");
            TimelessLevel = this.GetLocalization("TimelessLevel", () => "永恒：{0}（基础 {1}）");

            FairyLifeMax = this.GetLocalization("FairyLifeMax", () => "血量 ");
            FairyDamage = this.GetLocalization("FairyDamage", () => "伤害 ");
            FairyDefence = this.GetLocalization("FairyDefence", () => "防御 ");
            FairyScale = this.GetLocalization("FairyScale", () => "大小 ");

            Rarity_C= this.GetLocalization("RarityC", () => "常见");
            Rarity_U = this.GetLocalization("RarityU", () => "不常见");
            Rarity_R= this.GetLocalization("RarityR", () => "稀有");
            Rarity_RR= this.GetLocalization("RarityRR", () => "双倍稀有");
            Rarity_SR= this.GetLocalization("RaritySR", () => "史诗");
            Rarity_UR= this.GetLocalization("RarityUR", () => "神话");
            Rarity_RRR= this.GetLocalization("RarityRRR", () => "三倍稀有");
            Rarity_HR= this.GetLocalization("RarityHR", () => "传说");
            Rarity_AR= this.GetLocalization("RarityAR", () => "异类");
            Rarity_MR= this.GetLocalization("RarityMR", () => "秘宝");
            Rarity_SP= this.GetLocalization("RaritySP", () => "特殊");

            ZoneForestDescription = this.GetLocalization("ZoneForest", () => "在地表森林捕获");

            CurrentLife = this.GetLocalization("CurrentLife", () => "当前生命值：{0} / {1}");
            ResurrectionTime = this.GetLocalization("ResurrectionTime", () => "复活时间：{0}");
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
            TimelessLevel = null;

            FairyLifeMax = null;
            FairyDamage = null;
            FairyDefence=null;
            FairyScale = null;
            
            ZoneForestDescription = null;
        }

        public static string FormatIVDescription(LocalizedText pre, LocalizedText levelText, float @base, float bonused)
        {
            return pre.Value + levelText.Format( bonused,@base);
        }
    }
}
