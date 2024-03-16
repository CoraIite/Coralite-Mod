using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public abstract class YujianAI_BaseSlash : YujianAI
    {
        protected readonly float turnSpeed;
        protected readonly float roughlyVelocity;
        protected float halfShortAxis;
        protected float halfLongAxis;
        protected float startElliptical;
        protected ISmoother smoother;
        protected float targetRotation;

        public int SlashWidth { get; protected set; }
        public int SlashTime { get; protected set; }
        public float StartAngle { get; protected set; }
        public float TotalAngle { get; protected set; }

        public YujianAI_BaseSlash(int startTime, int slashWidth, int slashTime, float startAngle, float totalAngle, float turnSpeed, float roughlyVelocity, float halfShortAxis, float halfLongAxis, ISmoother smoother)
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

        public virtual void TryGetClosed2Target(BaseYujianProj yujianProj, out float distance, out float targetAngle)
        {
            //尝试靠近敌人
            Projectile Projectile = yujianProj.Projectile;
            Vector2 targetCenter = yujianProj.GetTargetCenter(IsAimingMouse);
            Vector2 targetDirection = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero);
            distance = Vector2.Distance(targetCenter, Projectile.Center);
            targetAngle = targetDirection.ToRotation();

            UpdateVelocityWhenTracking(Projectile, distance, targetDirection);

            //控制旋转，接近时逐渐转到对应的角度
            float factor = 0.008f * turnSpeed + 0.01f * roughlyVelocity;
            Projectile.rotation = Projectile.rotation.AngleLerp(targetAngle + 1.57f + StartAngle, factor);
        }

        public virtual void UpdateVelocityWhenTracking(Projectile Projectile, float distance, Vector2 targetDirection)
        {
            if (distance > SlashWidth * 2)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) + targetDirection * turnSpeed) / 21f;
            else if (distance < SlashWidth * 1.8f)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) - targetDirection * turnSpeed) / 21f;
            else if (Projectile.velocity == Vector2.Zero)
                Projectile.velocity = (Projectile.velocity * (20f + roughlyVelocity) + targetDirection * turnSpeed) / 21f;
        }

        public virtual void StartSlash(Projectile Projectile, float targetAngle)
        {
            Projectile.rotation = targetAngle + 1.57f + StartAngle;
            Vector2 slashCenter = Projectile.Center - (Projectile.rotation - 1.57f).ToRotationVector2() * SlashWidth;
            Projectile.localAI[0] = slashCenter.X;
            Projectile.localAI[1] = slashCenter.Y;
            targetRotation = targetAngle;
            Projectile.velocity *= 0f;
        }

        public virtual void Slash(Projectile Projectile, int time)
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
        }

        public virtual void AfterSlash(Projectile Projectile)
        {
            Vector2 dir = (Projectile.Center - new Vector2(Projectile.localAI[0], Projectile.localAI[1])).SafeNormalize(Vector2.Zero);
            //根据角速度计算得到     v = ω * r
            //                                      v = Δθ * r / Δt
            int rounds = (int)(TotalAngle / 6.282f) + 1;
            Projectile.velocity = dir.RotatedBy(-Math.Sign(StartAngle) * 1.57f) * (TotalAngle % 6.282f) * SlashWidth / (SlashTime / rounds);
        }
    }
}
