using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.FairyCatcherSystem;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Prefixes.FairyWeaponPrefixes
{
    public abstract class BaseFairyPrefix(float damageMult =1f, float knockbackMult = 1f, int critBonus = 0, float catchPowerMult = 1f
            , float useSpeedMult = 1f, float shootSpeedMult = 1f, float valueMult = 1f) : ModPrefix
    {
        internal float damageMult = damageMult;
        internal float knockbackMult = knockbackMult;
        internal int critBonus = critBonus;
        internal float catchPowerMult = catchPowerMult;
        internal float useSpeedMult = useSpeedMult;
        internal float shootSpeedMult = shootSpeedMult;
        internal float valueMult = valueMult;

        public override PrefixCategory Category => PrefixCategory.Custom;

        public override void Apply(Item item)
        {
            if (item.TryGetGlobalItem(out FairyGlobalItem fgi))
                fgi.CatchPowerMult = catchPowerMult;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = this.valueMult;
        }

        public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
        {
            if (catchPowerMult == 1)
                return null;

            bool isbad = catchPowerMult < 1;
            string sign = isbad ? "" : "+";
            string modify = string.Concat(sign, ((int)((catchPowerMult - 1) * 100)).ToString(), "%");

            return new List<TooltipLine>()
            {
                new TooltipLine(Mod, "CatchPowerMult", FairySystem.CatchPowerMult.Format(modify))
                {
                    IsModifier = true,
                    IsModifierBad = isbad
                }
            };
        }

        public override bool CanRoll(Item item)
        {
            return item.DamageType == FairyDamage.Instance && RollChance(item) > 0;
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.damageMult;
            knockbackMult = this.knockbackMult;
            critBonus = this.critBonus;
            useTimeMult = useSpeedMult;
            shootSpeedMult = this.shootSpeedMult;
        }
    }
}
