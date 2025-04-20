using Terraria;

namespace Coralite.Content.DamageClasses
{
    public class RangedNoAttackSpeed : DamageClass
    {
        public static RangedNoAttackSpeed Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }

        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == Generic)
                return new StatInheritanceData(1, 1, 0, 1, 1);

            if (damageClass == Ranged)
                return new StatInheritanceData(1, 1, 0, 1, 1);

            return StatInheritanceData.None;
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
            return true;
        }

    }
}
