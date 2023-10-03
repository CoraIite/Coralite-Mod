using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0传入颜色，0紫色，-1红色<br></br>
    /// 使用ai1传入追踪的角度<br></br>
    /// 使用ai2传入蓄力时间
    /// </summary>
    public class VineSpike : BaseNightmareProj, INightmareTentacle,IDrawAdditive
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private RotateTentacle tentacle;

        private Player Owner => Main.player[Projectile.owner];

        public float State;
        public ref float ColorState => ref Projectile.ai[0];
        public ref float Angle => ref Projectile.ai[1];
        public ref float ChannelTime => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public float alpha;
        private bool init = true;
        private Color tentacleColor;
        private Vector2 rotateVec2;
        public SpriteEffects effect;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.width = 142;
            Projectile.height = 56;
            Projectile.scale = 1.2f;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State == 1)
            {
                float a = 0;
                Vector2 dir = rotateVec2 * Projectile.width / 2;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                    Projectile.Center - dir, Projectile.Center + dir, 20, ref a);
            }

            return false;
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }

            if (init)
            {
                if (ColorState == 0)
                    tentacleColor = NightmarePlantera.lightPurple;
                else if (ColorState == -1)
                    tentacleColor = NightmarePlantera.nightmareRed;
                else
                    tentacleColor = Main.hslToRgb(new Vector3(Math.Clamp(ColorState, 0, 1f), 1f, 0.8f));

                Projectile.rotation = Projectile.velocity.ToRotation();
                effect = Main.rand.Next(0, 2) switch
                {
                    0 => SpriteEffects.None,
                    _ => SpriteEffects.FlipVertically
                };
                init = false;
            }

            tentacle ??= new RotateTentacle(30, factor =>
            {
                return Color.Lerp(tentacleColor * alpha, Color.Transparent, factor);
            }, factor =>
            {
                if (factor > 0.6f)
                    return Helper.Lerp(25, 0, (factor - 0.6f) / 0.4f);

                return Helper.Lerp(0, 25, factor / 0.6f);
            }, NightmarePlantera.tentacleTex, NightmarePlantera.waterFlowTex);

            tentacle.SetValue(Projectile.Center, np.Center, Projectile.rotation + MathHelper.Pi);
            tentacle.UpdateTentacle(Vector2.Distance(np.Center, Projectile.Center) / 30, 0.5f);

            switch ((int)State)
            {
                default:
                case 0: //伸出后向后蓄力并瞄准玩家
                    {
                        if (alpha < 1)
                        {
                            alpha += 1 / ChannelTime;
                            if (alpha > 1)
                                alpha = 1;
                        }

                        float factor = Timer / ChannelTime;
                        Vector2 center = Owner.Center + new Vector2(Owner.direction * 40 * factor, 0) + Owner.velocity * 20 * factor;
                        Vector2 dir = center - Projectile.Center + Angle.ToRotationVector2() * Helper.Lerp(200, 650, factor);

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 45;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.65f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.55f);
                        Projectile.rotation = Projectile.rotation.AngleTowards((center - Projectile.Center).ToRotation(), 0.35f);

                        if (Timer > ChannelTime)
                        {
                            Timer = 0;
                            State++;
                            Projectile.velocity = rotateVec2 * 64;
                            Helper.PlayPitched("Misc/Spike", 0.8f, -0.4f, Projectile.Center);
                            var modifyer = new PunchCameraModifier(Projectile.Center, rotateVec2, 10, 6, 12, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }
                    }
                    break;
                case 1://快速戳出
                    {
                        //for (int i = 0; i < 2; i++)
                        //{
                            Color c = Main.rand.Next(0, 1) switch
                            {
                                0 => tentacleColor,
                                _ => tentacleColor * 2f,
                            };
                                Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(32, 32), Projectile.velocity * Main.rand.NextFloat(0.05f, 0.2f),
                                    CoraliteContent.ParticleType<SpeedLine>(), c, Main.rand.NextFloat(0.3f, 0.5f));
                        //}
                        if (Main.rand.NextBool())
                        {
                            Color c2 = Main.rand.Next(0, 1) switch
                            {
                                0 => tentacleColor,
                                _ => tentacleColor * 2f,
                            };
                            Particle.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(16, 16), -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.3f),
                                CoraliteContent.ParticleType<SpeedLine>(), c2, Main.rand.NextFloat(0.3f, 0.5f));
                        }

                        if (Timer > 17)
                        {
                            State++;
                            Timer = 0;
                            Projectile.velocity *= 0;
                            alpha -= 0.2f;
                        }
                    }
                    break;
                case 2://收回并小幅度摇摆
                    {
                        if (alpha > 0)
                        {
                            alpha -= 0.015f;
                            if (alpha < 0)
                            {
                                alpha = 0;
                                Projectile.Kill();
                            }
                        }
                        float velLength = Projectile.velocity.Length();
                        if (velLength < 44)
                            velLength += 0.75f;
                        Vector2 dir = np.Center - Projectile.Center;
                        Vector2 dir2 = dir.SafeNormalize(Vector2.Zero);
                        Projectile.velocity = dir2 * velLength;
                        Projectile.rotation = dir2.ToRotation() + MathHelper.Pi + 0.35f * MathF.Sin(Timer * 0.1f);

                        if (dir.Length() < 50 || Timer > 180)
                            Projectile.Kill();
                    }
                    break;
            }

            rotateVec2 = Projectile.rotation.ToRotationVector2();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 selforigin = mainTex.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition;
            Color c = tentacleColor * alpha;

            for (int i = 0; i < 8; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter, null,
                c * (0.5f - i * 0.5f / 8), Projectile.oldRot[i], mainTex.Size() / 2, Projectile.scale * (1 + i * 0.05f), effect, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, selforigin, Projectile.scale, effect, 0);

            return false;
        }

        public void DrawTentacle()
        {
            tentacle?.DrawTentacle_NoEndBegin(i => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 selforigin = mainTex.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Color c = tentacleColor;
            c.A = (byte)(c.A * alpha);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, selforigin, Projectile.scale, effect, 0);
            c.A = (byte)(c.A * 0.4f);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, selforigin, Projectile.scale*1.35f, effect, 0);
        }
    }
}
