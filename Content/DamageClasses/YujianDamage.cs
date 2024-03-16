using Terraria;

namespace Coralite.Content.DamageClasses
{
    public class YujianDamage : DamageClass
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("念力伤害");
        }

        //会受到一半的魔法和召唤伤害加成
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
                return StatInheritanceData.Full;

            if (damageClass == Summon)
                return new StatInheritanceData(
                    damageInheritance: 0.5f,
                    critChanceInheritance: 0.5f,
                    armorPenInheritance: 0f,
                    attackSpeedInheritance: 0.5f,
                    knockbackInheritance: 0f
                    );

            if (damageClass == Magic)
                return new StatInheritanceData(
                    damageInheritance: 0.5f,
                    critChanceInheritance: 0.5f,
                    armorPenInheritance: 0f,
                    attackSpeedInheritance: 0.5f,
                    knockbackInheritance: 0f
                    );

            return new StatInheritanceData(
                damageInheritance: 0f,
                critChanceInheritance: 0f,
                attackSpeedInheritance: 0f,
                armorPenInheritance: 0f,
                knockbackInheritance: 0f
                );
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return false;
        }

        public override void SetDefaultStats(Player player)
        {

        }

        public override bool UseStandardCritCalcs => true;

        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            if (lineName == "Speed")
                return false;

            return true;
        }
    }
}
