using Terraria;

namespace Coralite.Content.GlobalProjectiles
{
    public partial class CoraliteGlobalProjectile
    {
        public bool EdibleDamage;

        public static void SetEdibleDamage(Projectile i)
        {
            i.GetGlobalProjectile<CoraliteGlobalProjectile>().EdibleDamage = true;
        }

        public static bool IsEdibleDamage(Projectile i)
        {
            return i.GetGlobalProjectile<CoraliteGlobalProjectile>().EdibleDamage;
        }
    }
}
