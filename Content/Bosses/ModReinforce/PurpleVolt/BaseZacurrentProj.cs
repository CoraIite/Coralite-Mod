using Coralite.Content.Items.Thunder;
using InnoVault.GameContent.BaseEntity;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    /// <summary>
    /// ai2控制点距离，localai1控制闪电宽度，localai2控制闪电透明度
    /// </summary>
    public abstract class BaseZacurrentProj : BaseHeldProj
    {
        public ref float PointDistance => ref Projectile.ai[2];
        public override bool CanFire => true;
        public ref float ThunderWidth => ref Projectile.localAI[1];
        public ref float ThunderAlpha => ref Projectile.localAI[2];

        public virtual float ThunderWidthFunc_Sin(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth;
        }

        public virtual Color ThunderColorFunc_Purple(float factor)
        {
            return ZacurrentDragon.ZacurrentPurple;
        }

        public virtual Color ThunderColorFunc2_Pink(float factor)
        {
            return ZacurrentDragon.ZacurrentPink;
        }

        public virtual float GetAlpha(float factor)
        {
            return ThunderAlpha;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<ThunderElectrified>(), Main.rand.Next(2, 4) * 60);
        }
    }
}
