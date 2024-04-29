using Terraria;

namespace Coralite.Content.DamageClasses
{
    public class FairyDamage : DamageClass
    {
        public static FairyDamage Instance;

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
                return StatInheritanceData.Full;

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
