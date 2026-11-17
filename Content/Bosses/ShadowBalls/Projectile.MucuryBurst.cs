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
    /// ai0传入持有者，ai1传入时间，ai2传入长度
    /// </summary>
    [VaultLoaden(AssetDirectory.ShadowBalls)]
    public class MucuryBurst : CoraliteBossHostileProj
    {
        public override string Texture => AssetDirectory.Trails + "LightShotSPA";

        protected ref float OwnerIndex => ref Projectile.ai[0];
        protected ref float ShootTime => ref Projectile.ai[1];
        protected ref float BaseLength => ref Projectile.ai[2];

        protected ref float LaserWidth => ref Projectile.localAI[0];

        protected float state;
        protected float timer;
        protected float alpha;

        public static ATex MercuryBurstGradient { get; private set; }
        public Vector2 endPos;

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
            if (endPos==Vector2.Zero)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center+endPos, 10, ref Projectile.localAI[2]);
        }

        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner(out NPC owner))
                return;

            const float maxLaserWidth = 30;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = owner.Center + (Projectile.velocity * owner.width / 2);

            switch (state)
            {
                default:
                case 0:
                    LaserWidth = Helper.Lerp(LaserWidth, 6, 0.15f);
                    endPos = Vector2.Lerp(Vector2.Zero, Projectile.velocity * BaseLength, Helper.SqrtEase(timer / 30f));
                    alpha = Helper.SinEase(timer / 30f);

                    if (timer > 30)
                    {
                        state = 1;
                        timer = 0;
                    }
                    break;
                case 1:
                    if (timer < 8)
                    {
                        alpha = timer / 8f;
                        LaserWidth += 50 / 8f;
                        //UpdateCachesNormally(dir, Projectile.Center);
                    }
                    else if (timer < (ShootTime - 12))
                    {
                        alpha = 1;
                        LaserWidth = Helper.Lerp(LaserWidth, maxLaserWidth, 0.5f);
                        //UpdateCachesNormally(dir, Projectile.Center);
                    }
                    else
                    {
                        LaserWidth -= maxLaserWidth / 12f;
                        if (timer>ShootTime)
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
            }

            //alpha = 1;
            //LaserWidth = 32;
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitive(Main.spriteBatch);

            //Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
            //var pos = laserTrailPoints[^1] - Main.screenPosition;
            //var origin = mainTex.Size() / 2;
            //Color c = Color.Silver;
            //c.A = 0;

            //Vector2 scale = new(LaserWidth / 90, LaserWidth / 130);

            //Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, 0, 0);
            //Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale * 0.75f, 0, 0);
            //Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale * 0.5f, 0, 0);

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
                return (0.5f+MathF.Sin(MathHelper.PiOver2 * factor / 0.2f)*0.5f) * LaserWidth;
            return LaserWidth * (1 - (factor - 0.2f) / 0.8f * 0.4f);
        }

        public virtual void DrawPrimitive(SpriteBatch spriteBatch)
        {
            if (endPos==Vector2.Zero)
            {
                return;
            }
            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            Color c = Color.White;
            c.A = (byte)(alpha * 255);
            //float count = laserTrailPoints.Count;
            Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
            Texture2D mainTex = CoraliteAssets.Trail.BoosterASP.Value;
            Texture2D gTex= MercuryBurstGradient.Value;
            if (state==0)
            {
                gTex = CoraliteAssets.Misc.White32x32.Value;
            }

            //float per = 5f * count / mainTex.Width;

            for (int i = 0; i < 30; i++)
            {
                float factor = 1f - (i / 30f);
                Vector2 Center = Projectile.Center+endPos*i/30f;
                Vector2 width = GetWidh(1-factor) * dir;
                Vector2 Top = Center + width;
                Vector2 Bottom = Center - width;
                bars.Add(new(Top, c, new Vector2(factor, 0)));
                bars.Add(new(Bottom, c, new Vector2(factor, 1)));

                //Center += endPos;
                //Top = Center + width/2;
                //Bottom = Center - width/2;
                //bars.Add(new(Top, c, new Vector2(0, 0)));
                //bars.Add(new(Bottom, c, new Vector2(0, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = ShaderLoader.GetShader("ShadowLaser2");

                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 1.5f + Projectile.whoAmI * 0.2143f);
                effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                effect.Parameters["sampleTexture"].SetValue(mainTex);
                effect.Parameters["gradientTexture"].SetValue(gTex);
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
}
