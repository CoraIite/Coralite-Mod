using Terraria.Localization;

namespace Coralite.Core.Systems.FlyingShieldSystem
{
    internal class FlyingShieldSystem :ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "Systems";

        public static LocalizedText FSDescriptionShort;
        public static LocalizedText FSDescriptionLong;

        public static LocalizedText ShieldPlusDescriptionShort;
        public static LocalizedText ShieldPlusDescriptionLong;

        public override void Load()
        {
            FSDescriptionShort = this.GetLocalization("FSDescriptionShort", () => "飞盾（按shift查看更多）");
            FSDescriptionLong = this.GetLocalization("FSDescriptionLong", 
                () => "左键以丢出盾牌，右键进行防御，成功防御时会根据盾牌属性以及饰品加成获得短暂伤害减免\n可以格挡敌怪或敌对弹幕");

            ShieldPlusDescriptionShort = this.GetLocalization("ShieldPlusDescriptionShort", () => "盾+（按shift查看更多）");
            ShieldPlusDescriptionLong = this.GetLocalization("ShieldPlusDescriptionLong", 
                () => "左键正常使用武器，右键进行防御，特殊攻击键丢出盾牌\n成功防御时会根据盾牌属性以及饰品加成获得短暂伤害减免\n可以格挡敌怪或敌对弹幕\n可以通过举盾来取消部分左键攻击的后摇");
        }

        public override void Unload()
        {
            FSDescriptionShort = null;
            FSDescriptionLong = null;
            ShieldPlusDescriptionShort = null;
            ShieldPlusDescriptionLong = null;
        }

    }
}
