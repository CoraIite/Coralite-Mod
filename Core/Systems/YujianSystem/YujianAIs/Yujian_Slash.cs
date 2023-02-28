using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public class Yujian_Slash : YujianAI
    {
        private readonly float turnSpeed;
        private readonly float roughlyVelocity;
        private readonly float halfShortAxis;
        private readonly float halfLongAxis;
        private readonly float startElliptical;
        private readonly ISmoother smoother;
        private float targetRotation;
        private bool canSlash = false;

        private Trail trail;

        public int SlashWidth { get; init; }
        public int SlashTime { get; init; }
        public float StartAngle { get; private set; }
        public float TotalAngle { get; init; }

        public Yujian_Slash(int startTime, int slashWidth, int slashTime, float startAngle, float totalAngle, float turnSpeed, float roughlyVelocity, float halfShortAxis, float halfLongAxis, ISmoother smoother)
        {
            if (startTime < slashTime)
                throw new Exception("总时间不能小于斩击时间");
            StartTime = startTime;
            SlashWidth = slashWidth;
            SlashTime = slashTime;
            StartAngle = startAngle;
            TotalAngle = totalAngle;
            this.turnSpeed = turnSpeed;
            this.roughlyVelocity = roughlyVelocity;
            this.halfShortAxis = halfShortAxis;
            this.halfLongAxis = halfLongAxis;
            this.smoother = smoother;
            startElliptical = Helper.EllipticalEase(StartAngle, halfShortAxis, halfLongAxis);
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            //先尝试接近NPC，距离小于一定值后开始斩击
            if (canSlash)
            {
                //斩击AI
                int time = StartTime - (int)yujianProj.Timer;
                if (time < 3)
                    yujianProj.InitTrailCache();

                if (time < SlashTime)
                {
                    float factor2 = smoother.Smoother(time, SlashTime);
                    float changeAngle = StartAngle + (-Math.Sign(StartAngle) * factor2 * TotalAngle);
                    Projectile.rotation = targetRotation + changeAngle;

                    //根据目前中心和距离中心的位置加上椭圆得到轨道（说的什么玩意，总之看码）
                    Vector2 slashCenter = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);

                    float current = Helper.EllipticalEase(changeAngle, halfShortAxis, halfLongAxis);
                    float Width = current * SlashWidth / startElliptical;
                    Projectile.Center = slashCenter + Projectile.rotation.ToRotationVector2() * Width;

                    Projectile.rotation += 1.57f;
                    return;
                }

                if (time == SlashTime)
                {
                    Vector2 dir = (Projectile.Center - new Vector2(Projectile.localAI[0], Projectile.localAI[1])).SafeNormalize(Vector2.Zero);
                    //根据角速度计算得到     v = ω * r
                    //                                      v = Δθ * r / Δt
                    Projectile.velocity = dir.RotatedBy(-Math.Sign(StartAngle) * 1.57f) * TotalAngle * SlashWidth / SlashTime;
                }

                return;
            }

            //尝试靠近敌人
            Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
            Vector2 targetDirection = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
            float distance = Vector2.Distance(targetCenter, Projectile.Center);
            float targetAngle = targetDirection.ToRotation();

            if (distance > SlashWidth * 2 + 20)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) + targetDirection * turnSpeed) / 21f;

            //控制旋转，接近时逐渐转到对应的角度
            float factor = 0.008f * turnSpeed + 0.01f * roughlyVelocity;
            Projectile.rotation = Projectile.rotation.AngleLerp(targetAngle + 1.57f + StartAngle, factor);

            if (distance < SlashWidth * 2)
            {
                canSlash = true;
                Vector2 slashCenter = Projectile.Center - (Projectile.rotation - 1.57f).ToRotationVector2() * SlashWidth;
                Projectile.localAI[0] = slashCenter.X;
                Projectile.localAI[1] = slashCenter.Y;
                targetRotation = targetAngle;
                yujianProj.InitTrailCache();
                trail?.SetVetical(StartAngle < 0);      //开始角度为正时设为false
            }
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            //StartAngle = -StartAngle;
            canSlash = false;
        }

        protected override bool UpdateTime(BaseYujianProj yujianProj)
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, yujianProj.Projectile.oldPos.Length, new NoTip(), factor => yujianProj.Projectile.width / 2,
            factor =>
            {
                return Color.Lerp(yujianProj.color1, yujianProj.color2, factor.X) * 0.8f;
            },
            filpVertical: StartAngle < 0
            );

            trail.Positions = yujianProj. Projectile.oldPos;
            return canSlash;
        }

        public override void DrawPrimitives(BaseYujianProj yujianProj)
        {
            int time = StartTime - (int)yujianProj.Timer;
            if (!canSlash || time > SlashTime || smoother.Smoother(time, SlashTime) > 0.99f)
                return;

            Effect effect = Filters.Scene["SimpleTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>(yujianProj.SlashTexture).Value);

            trail?.Render(effect);
        }
    }
}
