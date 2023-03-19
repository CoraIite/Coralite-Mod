using Coralite.Content.DamageClasses;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Prefixes.YujianWeaponPrefixes
{
    public abstract class BaseYujianPrefix:ModPrefix
    {
        internal float damageMult = 1f;
        internal float knockbackMult = 1f;
        internal int critBonus;

        public override PrefixCategory Category => PrefixCategory.Custom;

        public BaseYujianPrefix(float damageMult = 1f, float knockbackMult=1f, int critBonus = 0)
        {
            this.damageMult = damageMult;
            this.knockbackMult = knockbackMult;
            this.critBonus = critBonus;
        }

        public override void Apply(Item item)
        {

        }

        public override void ModifyValue(ref float valueMult)
        {

        }

        public override bool CanRoll(Item item)
        {
            return item.CountsAsClass<YujianDamage>() && item.IsCandidateForReforge &&
                GetType() != typeof(BaseYujianPrefix);
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            damageMult = this.damageMult;
            knockbackMult = this.knockbackMult;
            critBonus = this.critBonus;
        }

    }
}
