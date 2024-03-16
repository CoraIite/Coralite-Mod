using Terraria;

namespace Coralite.Core.Systems.YujianSystem.HuluEffects
{
    public interface IHuluEffect
    {
        void AIEffect(Projectile projectile);
        void HitEffect(Projectile projectile, NPC target, int damage, float knockback, bool crit);
        void PreDrawEffect(Projectile projectile, ref Color lightColor);
        void PostDrawEffect(Projectile projectile, Color lightColor);
    }
}
