using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>伽玛射线暴的两极直线激光，宽度为 SmallLaser 的四倍。</summary>
    [VaultLoaden(AssetDirectory.ShadowBalls)]
    public class GammaRayBurstLaser : CoraliteBossHostileProj
    {
        private const float WidthScale = 4f;
        // SmallLaser 的渐粗段占全长 20%，这里延长至 40%。
        private const float TaperLength = 0.4f;

        public override string Texture => AssetDirectory.Lasers + "MultLines";

        /// <summary>影子球索引。</summary>
        public ref float OwnerIndex => ref Projectile.ai[0];
        /// <summary>激光持续总时间</summary>
        public ref float ShootTime => ref Projectile.ai[1];
        /// <summary>在哪一端，1或-1</summary>
        public ref float PoleDirection => ref Projectile.ai[2];

        protected ref float LaserWidth => ref Projectile.localAI[0];

        public static ATex LaserGradient { get; private set; }
        protected float timer;
        public List<Vector2> laserTrailPoints = new();

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (laserTrailPoints.Count < 2 || LaserWidth <= 0)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, laserTrailPoints[^1], 10 * WidthScale, ref Projectile.localAI[2]);
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage()
        {
            if (timer< 8||timer> ShootTime - 45)
            {
                return false;
            }

            return base.CanDamage();
        }

        public override void AI()
        {
            laserTrailPoints.Clear();
            if (!OwnerIndex.GetNPCOwner<ShadowBall>(out NPC owner))
            {
                Projectile.Kill();
                return;
            }

            const float maxLaserWidth = 32 * WidthScale;

            // 随本体转动，从 ai2 指定的正极或负极沿轴线向外发射。
            Projectile.rotation = owner.rotation + (PoleDirection *MathHelper.PiOver2);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Projectile.Center = owner.Center + dir * (owner.width * owner.scale / 2f);

            if (timer < 8)
                LaserWidth += 50 * WidthScale / 8f;
            else if (timer < ShootTime - 45)
                LaserWidth = Helper.Lerp(LaserWidth, maxLaserWidth, 0.5f);
            else if (timer <= ShootTime)
                LaserWidth = maxLaserWidth * (1 - Helper.SqrtEase((timer - ShootTime + 45) / 45));
            else
                Projectile.Kill();

            // 包括消退阶段在内，取点始终沿直线，不叠加正弦偏移。
            UpdateCachesNormally(dir, Projectile.Center);
            timer++;
        }

        public void UpdateCachesNormally(Vector2 dir, Vector2 originPos)
        {
            laserTrailPoints.Add(originPos);

            for (int i = 1; i < 100; i++)
            {
                Vector2 currentPos = originPos + dir * i * 16;
                if (Helper.PointInTile(currentPos))
                {
                    for (int j = 0; j < 16; j++)
                    {
                        if (Helper.PointInTile(currentPos))
                            currentPos -= dir;
                        else
                            break;
                    }
                    laserTrailPoints.Add(currentPos);
                    break;
                }

                laserTrailPoints.Add(currentPos);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (laserTrailPoints.Count < 2 || LaserWidth <= 0)
                return false;

            DrawPrimitive(Main.spriteBatch);

            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
            Vector2 pos = laserTrailPoints[^1] - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;
            Color c = new(189, 109, 255, 0);
            Vector2 scale = new(LaserWidth / 90, LaserWidth / 130);

            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale * 0.75f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale * 0.5f, 0, 0);

            return false;
        }

        /// <summary>沿激光从起点到末端的宽度，前段由细变粗。</summary>
        public float GetWidh(float factor)
        {
            if (factor < 0.5f)
                return MathF.Sin(MathHelper.PiOver2 * factor / 0.5f) * LaserWidth;
            return LaserWidth;
        }

        public virtual void DrawPrimitive(SpriteBatch spriteBatch)
        {
            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            float count = laserTrailPoints.Count;
            Vector2 dir = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();
            Texture2D mainTex = Projectile.GetTextureValue();
            float per = 2f * count / mainTex.Width;

            for (int i = 0; i < count; i++)
            {
                float factor = 1f - i / count;
                Vector2 center = laserTrailPoints[i];
                Vector2 width = GetWidh(1f - factor) * dir;
                bars.Add(new(center + width, Color.White, new Vector2(factor * per, 0)));
                bars.Add(new(center - width, Color.White, new Vector2(factor * per, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = ShaderLoader.GetShader("ShadowLaser");

                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 3 + Projectile.whoAmI * 0.2143f);
                effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                effect.Parameters["sampleTexture"].SetValue(mainTex);
                effect.Parameters["gradientTexture"].SetValue(LaserGradient.Value);
                effect.Parameters["extTexture"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
                effect.Parameters["warptTexture"].SetValue(CoraliteAssets.Distortion.Twist.Value);
                effect.Parameters["cOff"].SetValue(0.05f);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }
            }
        }
    }
}
