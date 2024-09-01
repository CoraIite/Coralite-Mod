using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 尖刺球专用弹幕，使用ai0传入时间插值
    /// 使用ai1控制在玩家周围绕圈的时间
    /// </summary>
    public class BlackHole : BaseNightmareProj, IDrawNonPremultiplied, IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "BlackBall";

        private ref float TimeFactor => ref Projectile.ai[0];
        private ref float RollingTime => ref Projectile.ai[1];
        private ref float State => ref Projectile.ai[2];

        private Player Owner => Main.player[Projectile.owner];

        private float Timer;

        public static Asset<Texture2D> WarpTex;
        public static Asset<Texture2D> CircleTex;
        public static Asset<Texture2D> BlackHoleTex;

        private float scale;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            WarpTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "HaloWarp");
            CircleTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "PurpleCircle");
            BlackHoleTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "BlackHole");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            WarpTex = null;
            CircleTex = null;
            BlackHoleTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 64;
            Projectile.timeLeft = 3000;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            switch ((int)State)
            {
                default:
                case 0: //尝试追逐玩家身边的某个位置
                    {
                        if (scale < 0.4f)
                        {
                            scale += 0.05f;
                        }

                        Vector2 targetCenter = Owner.Center +
                            (((TimeFactor + Timer) / 360 * MathHelper.TwoPi).ToRotationVector2() * (450 + (100 * MathF.Sin(TimeFactor / 100))));

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = (targetCenter - Projectile.Center).ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(Vector2.Distance(Projectile.Center, targetCenter) / 800f, 0, 1) * (10 + (Timer / (RollingTime / 3) * 12));

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.65f);
                        Projectile.rotation += 0.1f;

                        Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(64, 64), DustID.VilePowder,
                              Projectile.velocity * 0.4f, 240, NightmarePlantera.nightPurple, Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;

                        if (Timer > RollingTime)
                        {
                            State++;
                            Timer = 0;
                            Projectile.velocity *= 0;
                            for (int i = 0; i < 7; i++) //生成噩梦尖刺弹幕
                            {
                                Vector2 dir = (Projectile.rotation + (i * 1 / 7f * MathHelper.TwoPi)).ToRotationVector2();
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + (dir * 48), dir, ModContent.ProjectileType<NightmareSpike>(), Projectile.damage, 0, ai0: 40, ai1: Main.zenithWorld ? Main.rand.NextFloat(0, 1) : -1, ai2: 750);
                            }
                        }

                    }
                    break;
                case 1://向周围7个方向射出尖刺

                    if (Timer == 40)
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.IceMist_Item120, Projectile.Center);
                    }
                    if (Timer > 100)
                    {
                        State++;
                    }
                    break;
                case 2:
                    scale *= 0.9f;
                    if (scale < 0.02f)
                    {
                        Projectile.Kill();
                    }
                    break;
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;

            spriteBatch.Draw(mainTex, pos, null, Color.White, 0, mainTex.Size() / 2, scale, 0, 0);

            Texture2D circleTex = CircleTex.Value;
            Texture2D blackholeTex = BlackHoleTex.Value;
            spriteBatch.Draw(blackholeTex, pos, null, Color.White, 0, blackholeTex.Size() / 2, new Vector2(scale * MathF.Sin(Main.GlobalTimeWrappedHourly), scale * 1.5f), 0, 0);

            spriteBatch.Draw(circleTex, pos, null, Color.White, Projectile.rotation + Main.GlobalTimeWrappedHourly, circleTex.Size() / 2, scale * 0.6f, 0, 0);
        }

        public void DrawWarp()
        {
            Texture2D warpTex = WarpTex.Value;
            Main.spriteBatch.Draw(warpTex, Projectile.Center - Main.screenPosition, null, Color.White * 0.3f,
                Main.GlobalTimeWrappedHourly * 5, warpTex.Size() / 2, scale + (MathF.Sin(Main.GlobalTimeWrappedHourly * 3) * 0.1f), 0, 0);
        }
    }
}
