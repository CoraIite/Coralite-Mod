using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using InnoVault.Vectors;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Core.Systems.YujianSystem.YujianAIs
{
    public class YujianAI_Slash : YujianAI_BaseSlash
    {
        private bool canSlash = false;

        private StrokeStyle trailStyle;
        private BaseYujianProj slashOwner;

        public YujianAI_Slash(int startTime, int slashWidth, int slashTime, float startAngle, float totalAngle, float turnSpeed, float roughlyVelocity, float halfShortAxis, float halfLongAxis, ISmoother smoother) : base(startTime, slashWidth, slashTime, startAngle, totalAngle, turnSpeed, roughlyVelocity, halfShortAxis, halfLongAxis, smoother)
        {

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
                    canDamage = false;
                    AfterSlash(Projectile);
                }

                return;
            }

            TryGetClosed2Target(yujianProj, out float distance, out float targetAngle);

            if (distance < SlashWidth * 2 && distance > SlashWidth * 1.8f)
            {
                canSlash = true;
                canDamage = true;
                StartSlash(Projectile, targetAngle);
                yujianProj.InitTrailCaches();
                EnsureTrailStyle(yujianProj);
                trailStyle.FlipV = StartAngle < 0;      //开始角度为正时设为false
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            }
        }

        protected override void OnStartAttack(BaseYujianProj yujianProj)
        {
            //StartAngle = -StartAngle;
            yujianProj.Projectile.velocity += (yujianProj.Projectile.rotation - 1.57f).ToRotationVector2() * 0.02f;
            canDamage = false;
            canSlash = false;
        }

        protected override bool UpdateTime(BaseYujianProj yujianProj)
        {
            EnsureTrailStyle(yujianProj);
            return canSlash;
        }

        private void EnsureTrailStyle(BaseYujianProj yujianProj)
        {
            slashOwner = yujianProj;
            trailStyle ??= new StrokeStyle
            {
                Parameterization = StrokeParameterization.PointIndex,
                WidthFunction = TrailWidth,
                ColorFunction = TrailColor,
                FlipV = StartAngle < 0,
            };
        }

        private float TrailWidth(float t) => slashOwner.Projectile.height / 2 * 2f; //全宽
        private Color TrailColor(float t, float side) => Color.Lerp(slashOwner.color1, slashOwner.color2, t) * 0.8f;

        public override void DrawPrimitives(BaseYujianProj yujianProj)
        {
            int time = StartTime - (int)yujianProj.Timer;
            if (!canSlash || time > SlashTime || time < yujianProj.trailCacheLength || smoother.Smoother(time, SlashTime) > 0.99f)
                return;

            Effect effect = ShaderLoader.GetShader("SimpleTrail");

            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>(yujianProj.SlashTexture).Value);

            if (trailStyle == null)
                return;

            VectorRenderer.DrawStroke(yujianProj.Projectile.oldPos, trailStyle, new VectorDrawOptions(VectorSpace.World, effect)
            {
                Blend = BlendState.AlphaBlend,
                MatrixParameter = "transformMatrix",
            });
        }
    }
}
