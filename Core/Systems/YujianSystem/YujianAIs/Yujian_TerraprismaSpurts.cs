using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public class Yujian_TerraprismaSpurts : YujianAI
    {
        public Yujian_TerraprismaSpurts(int startTime)
        {
            StartTime = startTime;
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            Projectile.localAI[0] = Projectile.Center.X;
            Projectile.localAI[1] = Projectile.Center.Y;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            float factor = (StartTime - yujianProj.Timer) / StartTime;
            Projectile Projectile = yujianProj.Projectile;

            Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);

            Vector2 originCenter = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            originCenter += new Vector2(0f, Utils.GetLerpValue(0f, 0.4f, factor, clamped: true) * -100f);
            Vector2 v = targetCenter - originCenter;
            Vector2 vector6 = v.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(v.Length(), 60f, 150f);
            Vector2 value = targetCenter + vector6;

            float lerpValue3 = Utils.GetLerpValue(0.4f, 0.6f, factor, clamped: true);
            float lerpValue4 = Utils.GetLerpValue(0.6f, 1f, factor, clamped: true);
            float targetAngle = v.SafeNormalize(Vector2.Zero).ToRotation() + (float)Math.PI / 2f;

            Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle, (float)Math.PI / 5f);
            Projectile.Center = Vector2.Lerp(originCenter, targetCenter, lerpValue3);
            if (lerpValue4 > 0f)
                Projectile.Center = Vector2.Lerp(targetCenter, value, lerpValue4);
        }
    }
}
