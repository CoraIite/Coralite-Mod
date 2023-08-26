using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai1传入追踪的角度<br></br>
    /// 使用ai2传入蓄力时间
    /// </summary>
    public class HookSlash : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "NightmareHook";

        private RotateTentacle tentacle;

        private static NPC NightmareOwner => Main.npc[NightmarePlantera.NPBossIndex];
        private Player Owner => Main.player[Projectile.owner];

        public ref float State => ref Projectile.ai[0];
        public ref float Angle => ref Projectile.ai[1];
        public ref float ChannelTime => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 32;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }


            tentacle ??= new RotateTentacle(30, factor =>
                {
                    return Color.Lerp(NightmarePlantera.nightPurple, Color.Transparent, factor);
                }, factor =>
                {
                    if (factor > 0.6f)
                        return Helper.Lerp(25, 0, (factor - 0.6f) / 0.4f);

                    return Helper.Lerp(0, 25, factor / 0.6f);
                }, NightmarePlantera.tentacleTex, NightmarePlantera.tentacleFlowTex);

            tentacle.SetValue(Projectile.Center, NightmareOwner.Center, Projectile.rotation + MathHelper.Pi);
            tentacle.UpdateTentacle(Vector2.Distance(NightmareOwner.Center, Projectile.Center) * 1.5f / 30);

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
                        Vector2 center = Owner.Center + angle.ToRotationVector2() * 350;
                        Vector2 dir = center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 800f, 0, 1) * 24;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);
                        Projectile.rotation = Projectile.rotation.AngleTowards((Owner.Center - Projectile.Center).ToRotation(), 0.2f);

                        if (Timer>ChannelTime)
                        {
                            Timer = 0;
                            State++;
                            Projectile.velocity += Projectile.rotation.ToRotationVector2() * 16;
                        }
                    }
                    break;
                case 1://咬下
                    {
                        if (Timer>32)
                        {
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://爆开生成噩梦光弹幕
                    {
                        Projectile.velocity *= 0.9f;
                        Projectile.rotation = Projectile.rotation.AngleTowards((Owner.Center - Projectile.Center).ToRotation(), 0.05f);

                        if (Timer > 25)
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                Vector2 dir = (Projectile.rotation + i * 1 / 7f * MathHelper.TwoPi).ToRotationVector2();

                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir, ModContent.ProjectileType<NightmareSparkle_Normal>(),
                                    Projectile.damage, 0);
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
            tentacle?.DrawTentacle(i => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));

            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            Vector2 selforigin = frameBox.Size() / 2;

           Main. spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White * 0.8f, Projectile.rotation+MathHelper.PiOver2, selforigin, Projectile.scale, 0, 0);

            return false;
        }
    }
}
