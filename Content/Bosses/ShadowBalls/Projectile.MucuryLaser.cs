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
    /// <summary>
    /// 使用ai0传入持有者,ai1传入射击时间，ai2传入基础长度
    /// </summary>
    [VaultLoaden(AssetDirectory.ShadowBalls)]
    public class MucuryLaser : CoraliteBossHostileProj
    {
        public override string Texture => AssetDirectory.Lasers + "MultLines";

        public List<Vector2> laserTrailPoints = new();

        protected ref float OwnerIndex => ref Projectile.ai[0];
        protected ref float ShootTime => ref Projectile.ai[1];
        protected ref float BaseLength => ref Projectile.ai[2];

        protected ref float LaserWidth => ref Projectile.localAI[0];
        protected ref float Random => ref Projectile.localAI[1];

        protected float timer;

        public static ATex MercuryBurstGradient { get; private set; }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (laserTrailPoints.Count < 1)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, laserTrailPoints[^1], 10, ref Projectile.localAI[2]);
        }

        public override bool? CanCutTiles() => false;

        public override void Initialize()
        {
            Projectile.timeLeft = (int)ShootTime;
            Random = Main.rand.NextFloat(3f) * 10f;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner(out NPC owner))
                return;

            const float maxLaserWidth = 32;

            //始终朝向面朝方向
            laserTrailPoints.Clear();

            Projectile.rotation = owner.rotation;
            Vector2 dir = owner.rotation.ToRotationVector2();
            Projectile.Center = owner.Center + (dir * owner.width / 2);

            if (timer < 8)
            {
                LaserWidth += 50 / 8f;
                UpdateCachesNormally(dir, Projectile.Center);
            }
            else if (timer < (ShootTime - 12))
            {
                LaserWidth = Helper.Lerp(LaserWidth, maxLaserWidth, 0.5f);
                UpdateCachesNormally(dir, Projectile.Center);
            }
            else
            {
                LaserWidth -= maxLaserWidth / 12f;

                Vector2 offset = (Projectile.rotation + 1.57f).ToRotationVector2();
                float factor = (timer - ShootTime + 10) / 10;

                offset *= factor * 14;

                laserTrailPoints.Add(Projectile.Center);

                for (int i = 0; i < 100; i++)
                {
                    float currLength = i * 12;

                    Vector2 currentPos = Projectile.Center + (dir * currLength) + (offset * MathF.Sin(Random + (i * 0.1f) + (timer / 4)));
                    if (currLength > BaseLength && Helper.PointInTile(currentPos))
                        break;

                    laserTrailPoints.Add(currentPos);
                }
            }

            timer++;
        }

        public void UpdateCachesNormally(Vector2 dir, Vector2 originPos)
        {
            laserTrailPoints.Add(Projectile.Center);

            for (int i = 0; i < 100; i++)
            {
                float currLength = i * 12;

                Vector2 currentPos = originPos + (dir * currLength);
                if (currLength > BaseLength && Helper.PointInTile(currentPos))
                {
                    for (int j = 0; j < 12; j++)
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
            if (laserTrailPoints == null || laserTrailPoints.Count < 1)
                return false;

            DrawPrimitive(Main.spriteBatch);

            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
            var pos = laserTrailPoints[^1] - Main.screenPosition;
            var origin = mainTex.Size() / 2;
            Color c = Color.Silver;
            c.A = 0;

            Vector2 scale = new(LaserWidth / 90, LaserWidth / 130);

            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale * 0.75f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale * 0.5f, 0, 0);

            return false;
        }

        /// <summary>
        /// 从0-1的激光宽度
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public float GetWidh(float factor)
        {
            if (factor < 0.2f)
                return MathF.Sin(MathHelper.PiOver2 * factor / 0.2f) * LaserWidth;
            return LaserWidth;
        }

        public virtual void DrawPrimitive(SpriteBatch spriteBatch)
        {
            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            float count = laserTrailPoints.Count;
            Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
            Texture2D mainTex = Projectile.GetTextureValue();
            float per = 5f * count / mainTex.Width;

            for (int i = 0; i < count; i++)
            {
                float factor = 1f - (i / count);
                Vector2 Center = laserTrailPoints[i];
                Vector2 width = GetWidh(1f - factor) * dir;
                Vector2 Top = Center + width;
                Vector2 Bottom = Center - width;
                bars.Add(new(Top, Color.White, new Vector2(factor * per, 0)));
                bars.Add(new(Bottom, Color.White, new Vector2(factor * per, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = ShaderLoader.GetShader("ShadowLaser");

                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 3 + Projectile.whoAmI * 0.2143f);
                effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                effect.Parameters["sampleTexture"].SetValue(mainTex);
                effect.Parameters["gradientTexture"].SetValue(MercuryBurstGradient.Value);
                effect.Parameters["extTexture"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
                effect.Parameters["warptTexture"].SetValue(CoraliteAssets.Distortion.Twist.Value);
                effect.Parameters["cOff"].SetValue(0.05f);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }
            }
        }
    }

    public class MucuryLaser_4Proj: MucuryLaser
    {
        public override void AI()
        {
            if (!OwnerIndex.GetProjectileOwner(out Projectile owner))
                return;

            const float maxLaserWidth = 32;

            //始终朝向面朝方向
            laserTrailPoints.Clear();

            Projectile.rotation = owner.rotation;
            Vector2 dir = owner.rotation.ToRotationVector2();
            Projectile.Center = owner.Center + (dir * owner.width / 2);

            if (timer < 8)
            {
                LaserWidth += 50 / 8f;
                UpdateCachesNormally(dir, Projectile.Center);
            }
            else if (timer < (ShootTime - 12))
            {
                LaserWidth = Helper.Lerp(LaserWidth, maxLaserWidth, 0.5f);
                UpdateCachesNormally(dir, Projectile.Center);
            }
            else
            {
                LaserWidth -= maxLaserWidth / 12f;

                Vector2 offset = (Projectile.rotation + 1.57f).ToRotationVector2();
                float factor = (timer - ShootTime + 10) / 10;

                offset *= factor * 14;

                laserTrailPoints.Add(Projectile.Center);

                for (int i = 0; i < 100; i++)
                {
                    float currLength = i * 12;

                    Vector2 currentPos = Projectile.Center + (dir * currLength) + (offset * MathF.Sin(Random + (i * 0.1f) + (timer / 4)));
                    if (currLength > BaseLength && Helper.PointInTile(currentPos))
                        break;

                    laserTrailPoints.Add(currentPos);
                }
            }

            timer++;
        }
    }
}
