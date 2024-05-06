using Coralite.Content.ModPlayers;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.Items.FlyingShields
{
    public abstract class BaseGreatRiverSnailMeleeArmor : ModItem
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public static LocalizedText NotHasGreatRiverSnailSoul;
        public static LocalizedText GreatRiverSnailSoul2Part;
        public static LocalizedText GreatRiverSnailSoul3Part;
        public static LocalizedText NotHasConcertration;
        public static LocalizedText Concertration2Part;
        public static LocalizedText Concertration3Part;

        public override void Load()
        {
            NotHasGreatRiverSnailSoul = Language.GetOrRegister(this.GetLocalizationKey("NotHasGreatRiverSnailSoul"), () => "大田螺之魂：装备至少2件套后触发效果");
            GreatRiverSnailSoul2Part = Language.GetOrRegister(this.GetLocalizationKey("GreatRiverSnailSoul2Part"),
                                                () => "大田螺之魂：格挡伤害并用尖刺回击敌人\n[c/807b7d:(2)件套]\n[c/807b7d:格挡伤害]\n[c/807b7d:释放6个投射物攻击]\n[c/807b7d:造成] [c/9e82b5:30] [c/807b7d:伤害]\n[c/9e82b5:45] [c/807b7d:秒冷却时间]");
            GreatRiverSnailSoul3Part = Language.GetOrRegister(this.GetLocalizationKey("GreatRiverSnailSoul3Part"),
                                                () => "大田螺之魂：格挡伤害并用尖刺回击敌人\n[c/807b7d:(3)件套]\n[c/807b7d:格挡伤害]\n[c/807b7d:释放8个投射物攻击]\n[c/807b7d:造成] [c/9e82b5:45] [c/807b7d:伤害]\n[c/9e82b5:30] [c/807b7d:秒冷却时间]");
            NotHasConcertration = Language.GetOrRegister(this.GetLocalizationKey("NotHasConcertration"), () => "绝对专注：装备至少2件套后触发效果");
            Concertration2Part = Language.GetOrRegister(this.GetLocalizationKey("Concertration2Part"), () => "绝对专注：减少近战攻击冷却速度，增加攻速\n[c/807b7d:(2)件套]\n[c/807b7d:近战CD减少50%]\n[c/807b7d:近战攻速增加5%]");
            Concertration3Part = Language.GetOrRegister(this.GetLocalizationKey("Concertration3Part"), () => "绝对专注：减少近战攻击冷却速度，增加攻速\n[c/807b7d:(3)件套]\n[c/807b7d:近战CD减少66%]\n[c/807b7d:近战攻速增加12%]");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player p = Main.LocalPlayer;

            if (p.TryGetModPlayer(out CoralitePlayer cp))
            {
                string soulText = cp.medusaSoul switch
                {
                    0 or 1 => NotHasGreatRiverSnailSoul.Value,
                    2 => GreatRiverSnailSoul2Part.Value,
                    _ => GreatRiverSnailSoul3Part.Value,
                };

                tooltips.Add(new TooltipLine(Mod, "GreatRiverSnailSoul", soulText));

                string luckyStarText = cp.split switch
                {
                    0 or 1 => NotHasConcertration.Value,
                    2 => Concertration2Part.Value,
                    _ => Concertration3Part.Value,
                };

                tooltips.Add(new TooltipLine(Mod, "Concertration", luckyStarText));
            }
        }
    }
}
