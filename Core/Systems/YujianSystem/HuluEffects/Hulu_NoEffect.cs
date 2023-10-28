using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Core.Systems.YujianSystem.HuluEffects
{
    public class Hulu_NoEffect : IHuluEffect
    {
        public void AIEffect(Projectile projectile) { }
        public void HitEffect(Projectile projectile, NPC target, int damage, float knockback, bool crit) { }
        public void PreDrawEffect(Projectile projectile, ref Color lightColor) { }
        public void PostDrawEffect(Projectile projectile, Color lightColor) { }
    }
}
