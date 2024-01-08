using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0传入颜色，范围0-6
    /// </summary>
    public class NightmareEnergy : ModProjectile, INightmareTentacle
    {
        public override string Texture => AssetDirectory.Blank;

        private ref float ColorState => ref Projectile.ai[0];
        private ref float State => ref Projectile.ai[1];

        private bool Init = true;

        public RotateTentacle tentacle;
        public Color drawColor;

        public Vector2 secondPos;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 16;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }

            if (Init)
            {
                if (ColorState < 0)
                    drawColor = NightmarePlantera.nightmareSparkleColor;
                else
                {
                    int c = Math.Clamp((int)ColorState, 0, 6);
                    drawColor = NightmarePlantera.phantomColors[c];
                }
                secondPos = Projectile.Center;
                Init = false;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            tentacle ??= new RotateTentacle(30, factor => drawColor
            , factor =>
            {
                if (factor > 0.6f)
                    return Helper.Lerp(20, 0, (factor - 0.6f) / 0.4f);

                return Helper.Lerp(0, 20, factor / 0.6f);
            }, NightmarePlantera.tentacleTex, NightmarePlantera.waterFlowTex);

            tentacle.SetValue(Projectile.Center, secondPos, Projectile.rotation + MathHelper.Pi);
            tentacle.UpdateTentacle(Vector2.Distance(secondPos, Projectile.Center) / 30);

            switch (State)
            {
                default:
                case 0: //自身冲向梦魇花
                    {
                        Vector2 center = np.Center;
                        Vector2 dir = center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 200, 0, 1) * 12;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.035f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation(), 0.3f);

                        if (dir.Length() < 30)
                        {
                            Projectile.velocity *= 0;
                            State++;
                            Projectile.Center = np.Center;
                        }
                    }
                    break;
                case 1://拖尾收拢
                    {
                        Vector2 dir = np.Center - secondPos;
                        secondPos += dir.SafeNormalize(Vector2.Zero) * 10;
                        if (dir.Length() < 30)
                        {
                            State++;
                        }
                    }
                    break;
                case 2:
                    Projectile.Kill();
                    return;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawTentacle()
        {
            tentacle?.DrawTentacle_NoEndBegin(i => 2 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));
        }
    }

    /// <summary>
    /// 使用ai0传入颜色，范围0-6，-1为默认紫色<br></br>
    /// 使用ai1传入与梦魇花的距离<br></br>
    /// 使用ai2传入状态，0追踪梦魇花，1绕圈
    /// </summary>
    public class NightmareSpawnEnergy : ModProjectile, IDrawPrimitive, IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;

        private Trail trail;

        public const int TrailLength = 26;

        private ref float ColorState => ref Projectile.ai[0];
        private ref float DistanceToNiP => ref Projectile.ai[1];
        private ref float State => ref Projectile.ai[2];

        private ref float Timer => ref Projectile.localAI[0];

        public Color drawColor;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 800;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            if (ColorState < 0)
                drawColor = NightmarePlantera.nightmareSparkleColor;
            else
            {
                int c = Math.Clamp((int)ColorState, 0, 6);
                drawColor = NightmarePlantera.phantomColors[c];
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.oldPos = new Vector2[TrailLength];
            for (int i = 0; i < TrailLength; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                return;
            }

            switch (State)
            {
                default:
                case 0:
                    {
                        float factor = Math.Clamp(Timer / 240, 0, 1);

                        float rot = Projectile.velocity.ToRotation();
                        float length = Projectile.velocity.Length();
                        Projectile.velocity = rot.AngleLerp((np.Center - Projectile.Center).ToRotation(), factor).ToRotationVector2() * length;
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        if (Timer > 400)
                        {
                            Projectile.Kill();
                        }

                        if (Vector2.Distance(np.Center, Projectile.Center) < 16)
                        {
                            Projectile.velocity *= 0;
                            if (Vector2.Distance(Projectile.Center, Projectile.oldPos[TrailLength - 1]) < 16)
                                Projectile.Kill();
                        }

                        Timer++;
                    }
                    break;
                case 1:
                    {
                        if (DistanceToNiP > 16)
                        {
                            Projectile.velocity *= 0;

                            float rot = (Projectile.Center - np.Center).ToRotation();
                            rot += 0.1f;
                            DistanceToNiP -= 10;

                            Projectile.Center = np.Center + rot.ToRotationVector2() * DistanceToNiP;
                            Projectile.rotation = (Projectile.oldPos[TrailLength - 2] - Projectile.Center).ToRotation();
                        }
                        else
                        {
                            if (Vector2.Distance(Projectile.Center, Projectile.oldPos[TrailLength - 1]) < 16)
                                Projectile.Kill();
                        }

                        Timer++;
                    }
                    break;
            }

            for (int i = 0; i < TrailLength - 1; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[TrailLength - 1] = Projectile.Center + Projectile.velocity;

            trail ??= new Trail(Main.graphics.GraphicsDevice, TrailLength, new NoTip(), factor => Helper.Lerp(0, 16, factor)
            , factor =>
            {
                if (Timer < TrailLength)
                    return Color.Lerp(drawColor, new Color(0, 0, 0, 0),  Math.Clamp((1-factor.X) / (Timer / TrailLength), 0, 1));
                return Color.Lerp(new Color(0, 0, 0, 0), drawColor, factor.X);
            });

            trail.Positions = Projectile.oldPos;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Texture2D lightTex = BaseNightmareSparkle.MainLight.Value;
            var origin = lightTex.Size() / 2;

            Color c = NightmarePlantera.lightPurple;
            var scale = new Vector2(0.15f, 0.3f);
            spriteBatch.Draw(lightTex, pos, null, c, Projectile.rotation + 1.57f, origin, scale, 0, 0);
            spriteBatch.Draw(lightTex, pos, null, c, Projectile.rotation + 1.57f, origin, scale, 0, 0);

            spriteBatch.Draw(lightTex, pos, null, c, Projectile.rotation, origin, scale * 0.5f, 0, 0);
            spriteBatch.Draw(lightTex, pos, null, c, Projectile.rotation, origin, scale * 0.5f, 0, 0);

            Texture2D flowTex = BaseNightmareSparkle.Flow.Value;
            origin = flowTex.Size() / 2;

            Color shineC = drawColor * 0.75f;

            var scale2 = scale.X * 0.5f;
            spriteBatch.Draw(flowTex, pos, null, shineC, Projectile.rotation + 1.57f + Main.GlobalTimeWrappedHourly, origin, scale2, 0, 0);
            spriteBatch.Draw(flowTex, pos, null, c * 0.5f, Projectile.rotation - Main.GlobalTimeWrappedHourly, origin, scale2, 0, 0);

            //周围一圈小星星
            scale2 = 0.1f;
            for (int i = 0; i < 7; i++)
            {
                float rot2 = (Main.GlobalTimeWrappedHourly * 2 + i * MathHelper.TwoPi / 7);
                Vector2 dir = rot2.ToRotationVector2();
                dir = pos + dir * (18 + factor * 2);
                rot2 += 1.57f;
                spriteBatch.Draw(lightTex, dir, null, shineC, rot2, origin, scale2, 0, 0);
            }
        }

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["NightmareTentacle"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 2);
            effect.Parameters["sampleTexture"].SetValue(NightmarePlantera.tentacleTex.Value);
            effect.Parameters["extraTexture"].SetValue(NightmareSpike.FlowTex.Value);
            effect.Parameters["flowAlpha"].SetValue(0.5f);
            effect.Parameters["warpAmount"].SetValue(3);

            trail.Render(effect);
        }
    }
}
