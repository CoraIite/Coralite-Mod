using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0传入颜色，0紫色，-1红色<br></br>
    /// 使用ai1传入追踪的角度<br></br>
    /// 使用ai2传入蓄力时间
    /// </summary>
    public class HookSlash : ModProjectile, INightmareTentacle
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "NightmareHook";

        private RotateTentacle tentacle;

        private Player Owner => Main.player[Projectile.owner];

        public float State;
        public ref float ColorState => ref Projectile.ai[0];
        public ref float Angle => ref Projectile.ai[1];
        public ref float ChannelTime => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        private bool init = true;
        private Color tentacleColor;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
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

                init = false;
            }

            tentacle ??= new RotateTentacle(30, factor =>
                {
                    return Color.Lerp(tentacleColor, Color.Transparent, factor);
                }, factor =>
                {
                    if (factor > 0.6f)
                        return Helper.Lerp(25, 0, (factor - 0.6f) / 0.4f);

                    return Helper.Lerp(0, 25, factor / 0.6f);
                }, NightmarePlantera.tentacleTex, NightmarePlantera.waterFlowTex);

            tentacle.SetValue(Projectile.Center, np.Center, Projectile.rotation + MathHelper.Pi);
            tentacle.UpdateTentacle(Vector2.Distance(np.Center, Projectile.Center) * 1.5f / 30);

            switch ((int)State)
            {
                default:
                case 0: //跟踪玩家
                    {
                        Projectile.frameCounter++;
                        if (Projectile.frameCounter > 10)
                        {
                            Projectile.frameCounter = 0;
                            Projectile.frame++;
                            if (Projectile.frame > 3)
                                Projectile.frame = 0;
                        }

                        float angle = Angle + 0.2f * MathF.Sin(Timer * 0.0314f);
                        Vector2 center = Owner.Center + angle.ToRotationVector2() * 600;
                        Vector2 dir = center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 1200f, 0, 1) * 16;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);
                        Projectile.rotation = Projectile.rotation.AngleTowards((Owner.Center - Projectile.Center).ToRotation(), 0.2f);

                        if (Timer > ChannelTime)
                        {
                            Timer = 0;
                            State++;
                            Projectile.frame = 2;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.velocity += (Projectile.rotation + Main.rand.NextFromList(-0.2f, 0.2f)).ToRotationVector2() * 14;
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                    break;
                case 1://咬下
                    {
                        if (Timer > 60)
                        {
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://爆开生成噩梦光弹幕
                    {
                        if (Projectile.frame > 0)
                        {
                            Projectile.frameCounter++;
                            if (Projectile.frameCounter > 4)
                            {
                                Projectile.frameCounter = 0;
                                Projectile.frame--;
                            }
                        }
                        Projectile.velocity *= 0.9f;
                        Projectile.rotation = Projectile.rotation.AngleTowards((Owner.Center - Projectile.Center).ToRotation(), 0.05f);

                        if (Timer > 25)
                        {
                            int damage = 0;
                            if (ColorState == 0)
                            {
                                damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                                for (int i = 0; i < 4; i++)
                                {
                                    Vector2 dir = (Projectile.rotation + i * 1 / 4f * MathHelper.TwoPi).ToRotationVector2();

                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir, ModContent.ProjectileType<NightmareSparkle_Normal>(),
                                        Projectile.damage, 0);
                                }
                            }
                            else
                            {
                                damage = Helper.ScaleValueForDiffMode(30, 15, 10, 10);
                                for (int i = 0; i < 5; i++)
                                {
                                    Vector2 dir = (Projectile.rotation + i * 1 / 5f * MathHelper.TwoPi).ToRotationVector2();

                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir, ModContent.ProjectileType<NightmareSparkle_Red>(),
                                        Projectile.damage, 0);
                                }
                            }


                            Projectile.Kill();
                        }
                    }
                    break;
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            Vector2 selforigin = frameBox.Size() / 2;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White * 0.8f, Projectile.rotation + MathHelper.PiOver2, selforigin, Projectile.scale, 0, 0);

            return false;
        }

        public void DrawTentacle()
        {
            tentacle?.DrawTentacle_NoEndBegin(i => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));
        }
    }
}
