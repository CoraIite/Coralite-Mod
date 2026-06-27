using Coralite.Content.Items.Thunder;
using InnoVault.GameContent.BaseEntity;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public abstract class BaseThunderProj : BaseHeldProj
    {
        public ref float PointDistance => ref Projectile.ai[2];
        public override bool CanFire => false;
        public ref float ThunderWidth => ref Projectile.localAI[1];
        public ref float ThunderAlpha => ref Projectile.localAI[2];

        public virtual float ThunderWidthFunc_Sin(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth;
        }

        public virtual Color ThunderColorFunc_Yellow(float factor)
        {
            return ThunderveinDragon.ThunderveinYellow;
        }

        public virtual Color ThunderColorFunc2_Orange(float factor)
        {
            return ThunderveinDragon.ThunderveinOrange;
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
