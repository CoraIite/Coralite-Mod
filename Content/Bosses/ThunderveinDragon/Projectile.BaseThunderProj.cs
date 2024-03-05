using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// ai2控制点距离，localai1控制闪电宽度，localai2控制闪电透明度
    /// </summary>
    public abstract class BaseThunderProj: ModProjectile
    {
        public ref float PointDistance => ref Projectile.ai[2];

        public ref float ThunderWidth => ref Projectile.localAI[1];
        public ref float ThunderAlpha => ref Projectile.localAI[2];

        public virtual float ThunderWidthFunc_Sin(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth;
        }

        public virtual Color ThunderColorFunc_Yellow(float factor)
        {
            return ThunderveinDragon.ThunderveinYellowAlpha * ThunderAlpha;
        }

        public virtual Color ThunderColorFunc2_Orange(float factor)
        {
            return ThunderveinDragon.ThunderveinOrangeAlpha * ThunderAlpha;
        }

    }
}
