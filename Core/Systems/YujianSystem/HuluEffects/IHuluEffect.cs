using Microsoft.Xna.Framework;
using Terraria;

namespace Coralite.Core.Systems.YujianSystem.HuluEffects
{
    public interface IHuluEffect
    {
        void AIEffect(BaseYujianProj yujianProj);
        void HitEffect(NPC target, int damage, float knockback, bool crit);
        bool PreDrawEffect(ref Color lightColor);
        void PostDrawEffect(Color lightColor);
    }
}
