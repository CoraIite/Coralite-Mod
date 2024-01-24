using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public class Yujian_Spurts : YujianAI
    {
        private readonly float spurtsSpeed;
        private readonly float turnSpeed;
        private readonly float roughlyVelocity;
        private bool canSpurts = false;

        public int SpurtsLength { get; init; }

        public Yujian_Spurts(int startTime, float spurtsSpeed, int spurtsLength, float turnSpeed, float roughlyVelocity)
        {
            StartTime = startTime;
            this.SpurtsLength = spurtsLength;
            this.spurtsSpeed = spurtsSpeed;
            this.turnSpeed = turnSpeed;
            this.roughlyVelocity = roughlyVelocity;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            //先尝试靠近目标，再向目标方向突刺
            Projectile Projectile = yujianProj.Projectile;

            if (canSpurts)
            {
                if (yujianProj.Timer < StartTime * 0.2f)
                {
                    Projectile.velocity *= 0.96f;
                    canDamage = false;
                }

                return;
            }

            //尝试靠近敌人
            Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
            Vector2 targetDirection = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
            float distance = Vector2.Distance(targetCenter, Projectile.Center);
            float targetAngle = targetDirection.ToRotation();

            if (distance > SpurtsLength * 2)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) + targetDirection * turnSpeed) / 21f;
            else if (distance < SpurtsLength * 1.8f)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) - targetDirection * turnSpeed) / 21f;

            //控制旋转，接近时逐渐转到对应的角度
            float factor = 0.008f * turnSpeed + 0.01f * roughlyVelocity;
            Projectile.rotation = Projectile.rotation.AngleLerp(targetAngle + 1.57f, factor);

            if (distance < SpurtsLength * 2 && distance > SpurtsLength * 1.8f)
            {
                canSpurts = true;
                canDamage = true;
                Projectile.velocity = targetDirection * spurtsSpeed;
                Projectile.rotation = targetAngle + 1.57f;
                yujianProj.InitTrailCaches();
            }
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            Projectile.velocity = new Vector2(-Projectile.velocity.Y, Projectile.velocity.X) * 2f;
            canSpurts = false;
            canDamage = false;
        }

        protected override bool UpdateTime(BaseYujianProj yujianProj)
        {
            return canSpurts;
        }

        public override void DrawAdditive(SpriteBatch spriteBatch, BaseYujianProj yujianProj)
        {
            if (!canSpurts)
                return;

            Projectile Projectile = yujianProj.Projectile;

            //绘制影子拖尾
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle source = mainTex.Frame();
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 2);
            float scale = 1.6f + yujianProj.trailCacheLength * 0.015f;

            for (int i = yujianProj.trailCacheLength - 1; i > 0; i -= 4)
            {
                Color shadowColor = Color.Lerp(yujianProj.color1, yujianProj.color2, (float)i / yujianProj.trailCacheLength);
                int a = 20 + i * 6;
                if (a > 255)
                    a = 255;
                shadowColor.A = (byte)a;
                spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, source, shadowColor, Projectile.oldRot[i], origin, scale - i * 0.015f, SpriteEffects.None, 0f);
            }
        }
    }
}
