using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Core.Systems.YujianSystem.HuluEffects
{
    public class Hulu_NoEffect : IHuluEffect
    {
        public void AIEffect(BaseYujianProj yujianProj) { }
        public void HitEffect(NPC target, int damage, float knockback, bool crit) { }
        public bool PreDrawEffect(ref Color lightColor) => true;
        public void PostDrawEffect(Color lightColor) { }
    }
}
