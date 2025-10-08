using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public class YujianAI_DoubleSlash : YujianAI_BaseSlash
    {
        protected readonly float canSlashLength;

        private bool canSlash = false;
        protected bool doubleSlash = true;
        private Trail trail;

        public YujianAI_DoubleSlash(int startTime, int slashWidth, int slashTime, float startAngle, float totalAngle, float turnSpeed, float roughlyVelocity, float halfShortAxis, float halfLongAxis, ISmoother smoother) : base(startTime, slashWidth, slashTime, startAngle, totalAngle, turnSpeed, roughlyVelocity, halfShortAxis, halfLongAxis, smoother)
        {
            canSlashLength = SlashWidth * halfLongAxis / startElliptical;
        }

        protected override void Attack(BaseYujianProj yujianProj)
        {
            Projectile Projectile = yujianProj.Projectile;

            //先尝试接近NPC，距离小于一定值后开始斩击
            if (canSlash)
            {
                //斩击AI
                int time = StartTime - (int)yujianProj.Timer;

                if (time < SlashTime)
                {
                    Slash(Projectile, time);
                    return;
                }

                if (time == SlashTime)
                {
                    if (doubleSlash)
                    {
                        Reset();
                        doubleSlash = false;
                        yujianProj.Timer = StartTime;
                        startElliptical = Helper.EllipticalEase(StartAngle, halfShortAxis, halfLongAxis);
                        //重设中心点以及角度和拖尾数组
                        Vector2 slashCenter = new(Projectile.localAI[0], Projectile.localAI[1]);
                        Projectile.rotation = targetRotation + StartAngle;
                        Projectile.Center = slashCenter + (Projectile.rotation.ToRotationVector2() * SlashWidth);
                        Projectile.rotation += 1.57f;

                        yujianProj.InitTrailCaches();
                        trail?.SetFlipState(StartAngle < 0);      //开始角度为正时设为false

                        return;
                    }

                    canDamage = false;
                    AfterSlash(Projectile);
                }

                return;
            }

            TryGetClosed2Target(yujianProj, out float distance, out float targetAngle);

            if (distance < canSlashLength && distance > canSlashLength * 0.8f)
            {
                canSlash = true;
                canDamage = true;
                StartSlash(Projectile, targetAngle);
                yujianProj.InitTrailCaches();
                trail?.SetFlipState(StartAngle < 0);      //开始角度为正时设为false
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            }
        }

        public override void UpdateVelocityWhenTracking(Projectile Projectile, float distance, Vector2 targetDirection)
        {
            if (distance > canSlashLength)
                Projectile.velocity = ((Projectile.velocity * (20f + roughlyVelocity)) + (targetDirection * turnSpeed)) / 21f;
            else if (distance < canSlashLength * 0.8f)
                Projectile.velocity = ((Projectile.velocity * (20f + roughlyVelocity)) - (targetDirection * turnSpeed)) / 21f;
            else if (Projectile.velocity == Vector2.Zero)
                Projectile.velocity = ((Projectile.velocity * (20f + roughlyVelocity)) + (targetDirection * turnSpeed)) / 21f;
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            //StartAngle = -StartAngle;
            yujianProj.Projectile.velocity += (yujianProj.Projectile.rotation - 1.57f).ToRotationVector2() * 0.02f;
            Init();
            doubleSlash = true;
            canDamage = false;
            canSlash = false;
        }

        protected override bool UpdateTime(BaseYujianProj yujianProj)
        {
            trail ??= new Trail(Main.instance.GraphicsDevice, yujianProj.Projectile.oldPos.Length, new EmptyMeshGenerator(), factor => yujianProj.Projectile.height / 2,
            factor =>
            {
                return Color.Lerp(yujianProj.color1, yujianProj.color2, factor.X) * 0.8f;
            }, flipVertical: StartAngle < 0);

            trail.TrailPositions = yujianProj.Projectile.oldPos;
            return canSlash;
        }

        public override void DrawPrimitives(BaseYujianProj yujianProj)
        {
            int time = StartTime - (int)yujianProj.Timer;
            if (!canSlash || time > SlashTime || time < yujianProj.trailCacheLength || smoother.Smoother(time, SlashTime) > 0.99f)
                return;

            Effect effect = ShaderLoader.GetShader("SimpleTrail");

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>(yujianProj.SlashTexture).Value);

            trail?.DrawTrail(effect);
        }

        /// <summary>
        /// 重设挥舞相关数据
        /// </summary>
        public virtual void Reset()
        {

        }

        /// <summary>
        /// 重新设置默认值
        /// <br>所以明明在构造器里设置了一遍，却还要再手动重置，为什么要这样做呢？</br>
        /// <br>baka! baka! baka代码！！！</br>
        /// </summary>
        public virtual void Init()
        {

        }
    }
}
