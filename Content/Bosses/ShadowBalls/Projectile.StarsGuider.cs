using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class StarsGuider : CoraliteBossHostileProj
    {
        public override string Texture => AssetDirectory.Lasers + "MultLines";

        public const float ArcHalfAngle = 1.2f;

        public const int ArcLineWidth = 16 * 18;
        public const int MeteorShowerWidth = 16 * 30;

        public ref float Style => ref Projectile.ai[0];
        public ref float Distance => ref Projectile.ai[1];
        public ref float WarnTime => ref Projectile.ai[2];

        public ref float LineWidth => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float LinePercent => ref Projectile.localAI[2];

        public float alpha = 1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override void AI()
        {
            switch (Style)
            {
                default:
                case 0:
                    Projectile.rotation = (Owner.Center - Projectile.Center).ToRotation();
                    LineWidth = ArcLineWidth;

                    if (Timer > 30 && Timer < WarnTime + 50 && Timer % 4 == 0)
                    {
                        SpawnStars_Starline();
                    }
                    break;
                case 1:
                    {
                        LineWidth = MeteorShowerWidth;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                        if (Timer > 30 && Timer < WarnTime + 50 && Timer % 4 == 0)
                        {
                            SpawnStars_MeteorShower();
                        }
                    }
                    break;
            }

            Timer++;
            if (Timer < 30)
                LinePercent = Helper.SqrtEase(Timer / 30f);
            else if (Timer < 30 + WarnTime)
                LinePercent = 1;
            else if (Timer < 30 + WarnTime + 30)
                alpha = 1 - Helper.SqrtEase((Timer - 30 - WarnTime) / 30f);
            else
                Projectile.Kill();
        }

        public void SpawnStars_Starline()
        {
            Vector2 dir = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            //for (int i = 0; i < howmany; i++)
            {
                float totalR = ArcHalfAngle * 2 + Main.rand.NextFloat(-0.3f, 0.3f);

                Vector2 pos = Projectile.Center + dir.RotatedBy(-totalR / 2) * (Distance + Main.rand.NextFloat(-ArcLineWidth * 0.35f, ArcLineWidth * 0.35f));

                Projectile.NewProjectileFromThis<ShadowBallStar>(pos, Vector2.Zero, Helper.GetProjDamage(60, 65, 100), 0, ai0: 0, ai1: Projectile.whoAmI, ai2: totalR / (30 + 60 + 30) * Main.rand.NextFloat(0.9f, 1.4f));
            }
        }

        public void SpawnStars_MeteorShower()
        {
            Vector2 dir = Projectile.velocity;
            Vector2 n = Projectile.velocity.RotatedBy(-MathHelper.PiOver2);
            //for (int i = 0; i < howmany; i++)
            {
                Vector2 pos = Projectile.Center + n * (Main.rand.NextFloat(-MeteorShowerWidth * 0.45f, MeteorShowerWidth * 0.45f));
                int runTime = Main.rand.Next(10, 30);
                float speed = Distance / (30 + runTime + 30);

                Projectile.NewProjectileFromThis<ShadowBallStar>(pos, dir * speed, Helper.GetProjDamage(60, 65, 100), 0, ai0: 1, ai1: Projectile.whoAmI, ai2: runTime);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (LinePercent > 0)
            {
                CoraliteSystem.InitBars();
                List<ColoredVertex> bars;
                Texture2D mainTex = Projectile.GetTextureValue();

                switch (Style)
                {
                    default:
                    case 0:
                        bars = PrepareArcBars(mainTex);
                        break;
                    case 1:
                        bars = PrepareMeteorShowerBars(mainTex);
                        break;
                }

                Effect effect = ShaderLoader.GetShader("ShadowLaser");
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 1.5f + Projectile.whoAmI * 0.2143f);
                effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                effect.Parameters["sampleTexture"].SetValue(mainTex);
                effect.Parameters["gradientTexture"].SetValue(CoraliteAssets.Misc.White32x32.Value);
                effect.Parameters["extTexture"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
                effect.Parameters["warptTexture"].SetValue(CoraliteAssets.Distortion.Twist.Value);
                effect.Parameters["cOff"].SetValue(0.05f);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip,
                        bars.ToArray(), 0, bars.Count - 2);
                }

            }

            return false;
        }

        public List<ColoredVertex> PrepareArcBars(Texture2D mainTex)
        {
            const int pointCounts = 60;

            List<ColoredVertex> bars = CoraliteSystem.Vertexes;

            float per = 2f * (pointCounts + 1) / mainTex.Width;
            float centerAngle = (Owner.Center - Projectile.Center).ToRotation();
            float currAngle = ArcHalfAngle * 2 * MathHelper.Clamp(LinePercent, 0f, 1f);
            float radius = Distance;

            for (int i = 0; i <= pointCounts; i++)
            {
                float factor = i / (float)pointCounts;
                float angle = centerAngle - ArcHalfAngle + (currAngle * factor);

                Vector2 dir = angle.ToRotationVector2();

                Vector2 center = Projectile.Center + dir * radius;
                float f = MathF.Sin(factor * MathHelper.Pi);
                Vector2 width = dir * LineWidth / 2;
                Color color = Color.White * (0.3f * f * alpha);
                float u = (1f - factor) * per;

                bars.Add(new(center - width, color, new Vector2(u, 0f)));
                bars.Add(new(center + width, color, new Vector2(u, 1f)));
            }

            return bars;
        }

        public List<ColoredVertex> PrepareMeteorShowerBars(Texture2D mainTex)
        {
            const int pointCounts = 60;

            List<ColoredVertex> bars = CoraliteSystem.Vertexes;

            float per = 2f * (pointCounts + 1) / mainTex.Width;
            float totalLength = Distance * LinePercent;
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 n = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * LineWidth / 2;

            for (int i = 0; i <= pointCounts; i++)
            {
                float factor = i / (float)pointCounts;

                float length = factor * totalLength;

                Vector2 center = Projectile.Center + dir * length;
                float f = MathF.Sin(factor * MathHelper.Pi);
                Color color = Color.White * (0.3f * f * alpha);
                float u = (1f - factor) * per;

                bars.Add(new(center - n, color, new Vector2(u, 0f)));
                bars.Add(new(center + n, color, new Vector2(u, 1f)));
            }

            return bars;
        }
    }
}