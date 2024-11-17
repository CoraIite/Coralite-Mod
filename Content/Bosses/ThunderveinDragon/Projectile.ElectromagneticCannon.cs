using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    /// <summary>
    /// 使用ai0传入冲刺时间，ai1传入主人
    /// 使用ai2传入闪电每个点间的间隔
    /// 使用速度传入中心点的位置，位置传入末端的位置
    /// 激光长度2000，激光旋转跟随recorder1;
    /// </summary>
    public class ElectromagneticCannon : LightningDash
    {
        const int DelayTime = 30;
        private float laserWidth;

        public List<Vector2> laserTrailPoints = new();

        public static Asset<Texture2D> gradientTex;

        public static Asset<Texture2D> laserTex;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                gradientTex = ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "LaserGradient");
                laserTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB");
            }
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                gradientTex = null;
                laserTex = null;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2800;
        }

        public override bool? CanDamage()
        {
            if (Timer > DashTime + (DelayTime / 2))
                return false;

            return null;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, Projectile.Kill))
                return;

            InitTrails();

            ThunderveinDragon dragon = owner.ModNPC as ThunderveinDragon;

            if (Timer < DashTime)
            {
                Projectile.velocity = dragon.GetMousePos();
                Projectile.Center = Projectile.velocity + (dragon.Recorder.ToRotationVector2() * 2000);
                Projectile.rotation = (Projectile.Center - Projectile.velocity).ToRotation();

                SpawnDusts();

                Vector2 pos2 = Projectile.velocity;
                List<Vector2> pos = new()
                {
                    Projectile.velocity
                };
                if (Vector2.Distance(Projectile.velocity, Projectile.Center) < PointDistance)
                    pos.Add(Projectile.Center);
                else
                    for (int i = 0; i < 40; i++)
                    {
                        pos2 = pos2.MoveTowards(Projectile.Center, PointDistance);
                        if (Vector2.Distance(pos2, Projectile.Center) < PointDistance)
                        {
                            pos.Add(Projectile.Center);
                            break;
                        }
                        else
                            pos.Add(pos2);
                    }

                foreach (var trail in thunderTrails)
                {
                    trail.BasePositions = pos.ToArray();
                    trail.SetExpandWidth(4);
                }

                if (Timer % 4 == 0)
                {
                    foreach (var trail in thunderTrails)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                float factor = Timer / DashTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);

                ThunderWidth = 30 + (sinFactor * 30);
                if (ThunderAlpha < 1)
                {
                    ThunderAlpha += 1 / 10f;
                }
                if (Timer < 8)
                {
                    laserWidth += 120 / 8;
                }
                else
                    laserWidth = Helper.Lerp(laserWidth, 50, 0.5f);
            }
            else if ((int)Timer == (int)DashTime)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                laserWidth -= 50f / DelayTime;

                float factor = (Timer - DashTime) / DelayTime;
                ThunderWidth = 30 * (1 - factor);
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange(GetRange(factor));
                    trail.SetExpandWidth(GetExpandWidth(factor));

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                if (Timer > DashTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
            UpdateCachesNormally();
        }

        public override void SpawnDusts()
        {
            if (Main.rand.NextBool())
            {
                Vector2 pos = Vector2.Lerp(Projectile.velocity, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                    + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                if (Main.rand.NextBool())
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>()
                        , Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2, 4)
                        , newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.2f));
                }
            }
        }

        public void InitTrails()
        {
            if (thunderTrails == null)
            {
                Projectile.Resize((int)PointDistance, 40);

                thunderTrails = new ThunderTrail[3];
                for (int i = 0; i < 3; i++)
                {
                    thunderTrails[i] = new ThunderTrail(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrailB2")
                        , ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].SetRange((5, 20));
                    thunderTrails[i].BasePositions =
                    [
                        Projectile.Center,Projectile.Center,Projectile.Center
                    ];
                }
            }
        }

        public void UpdateCachesNormally()
        {
            laserTrailPoints.Clear();

            laserTrailPoints.Add(Projectile.velocity);

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 100; i++)
            {
                Vector2 currentPos = Projectile.velocity + (dir * i * 20);

                laserTrailPoints.Add(currentPos);
            }
        }

        public float GetWidh(float factor)
        {
            if (factor < 0.5f)
                return MathF.Sin(MathHelper.PiOver2 * factor / 0.5f) * laserWidth;
            return laserWidth;
        }

        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinYellowAlpha, MathF.Sin(factor * MathHelper.Pi));
        }

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return Color.Lerp(ThunderveinDragon.ThunderveinPurpleAlpha, ThunderveinDragon.ThunderveinOrangeAlpha, MathF.Sin(factor * MathHelper.Pi));
        }

        public virtual (float, float) GetRange(float factor)
        {
            float sinFactor = MathF.Sin(factor * MathHelper.Pi);

            return (5, 20 + (sinFactor * PointDistance / 2));
        }

        public virtual float GetExpandWidth(float factor)
        {
            return (1 - factor) * PointDistance / 3;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrimitive(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Light").Value;
            var pos = laserTrailPoints[^1] - Main.screenPosition;
            var origin = mainTex.Size() / 2;
            Color c = new(189, 109, 255, 0);
            c.A = 0;

            Vector2 scale = new(laserWidth / 90, laserWidth / 130);

            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale * 0.75f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale * 0.5f, 0, 0);

            base.PreDraw(ref lightColor);
            return false;
        }

        public virtual void DrawPrimitive(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();
            float count = laserTrailPoints.Count;
            Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
            for (int i = 0; i < count; i++)
            {
                float factor = 1f - (i / count);
                Vector2 Center = laserTrailPoints[i];
                Vector2 width = GetWidh(1f - factor) * dir;
                Vector2 Top = Center + width;
                Vector2 Bottom = Center - width;

                bars.Add(new(Top.Vec3(), Color.White, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), Color.White, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Effect effect = Filters.Scene["LaserAlpha"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 2);
                effect.Parameters["exAdd"].SetValue(0.2f);
                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(laserTex.Value);
                effect.Parameters["gradientTexture"].SetValue(gradientTex.Value);
                effect.Parameters["extTexture"].SetValue(CoraliteAssets.Laser.VanillaFlowA.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
