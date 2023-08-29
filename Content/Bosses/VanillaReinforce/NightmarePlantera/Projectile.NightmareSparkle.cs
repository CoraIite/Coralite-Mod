using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSparkle_Normal : BaseNightmareSparkle
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;

            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 48;
            ShineColor = NightmarePlantera.nightmareSparkleColor;
            mainSparkleScale = new Vector2(1.5f, 3f);
            circleSparkleScale = 0.5f;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 24)
            {
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.3f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                Dust dust = Dust.NewDustPerfect(Projectile.Center , ModContent.DustType<NightmareStar>(),
                    dir * Main.rand.NextFloat(1f, 4f), newColor: NightmarePlantera.lightPurple, Scale: Main.rand.NextFloat(1f, 3f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
            }
        }
    }

    /// <summary>
    /// 使用ai0输入旋转时间，大于旋转时间后会射出<br></br>
    /// 使用ai1控制是否会在结束旋转后生成美梦光弹幕
    /// </summary>
    public class NightmareSparkle_Rolling:BaseNightmareSparkle
    {
        public Player Owner => Main.player[Projectile.owner];

        public ref float RollingTime => ref Projectile.ai[0];
        public ref float CanExchange => ref Projectile.ai[1];

        public ref float Angle => ref Projectile.localAI[0];
        public ref float State => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.localAI[2];

        private bool init=true;
        private float length;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 32;
            ShineColor = NightmarePlantera.nightPurple;
            mainSparkleScale = new Vector2(1.5f, 3f);
            circleSparkleScale = 0.5f;
        }

        public override void AI()
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.VilePowder,
                  Projectile.velocity * 0.4f, 240, NightmarePlantera.nightPurple, Main.rand.NextFloat(1f, 1.5f));
            d.noGravity = true;

            if (init)
            {
                Vector2 dir = Projectile.Center - Owner.Center;
                Angle = dir.ToRotation();
                length = dir.Length();
                init = false;
            }

            switch ((int)State)
            {
                default:
                case 0: //在玩家身边环绕
                    {
                        Angle += 0.05f;
                        Projectile.Center = Owner.Center + Angle.ToRotationVector2() * length;
                        Projectile.rotation = Angle;
                        if ((int)CanExchange==1)
                        {
                            Projectile.rotation += Main.rand.NextFloat(-0.03f, 0.03f);
                        }
                        if (Timer>RollingTime)
                        {
                            State++;
                            Timer = 0;
                            if ((int)CanExchange == 1)
                            {
                                //TODO：生成美梦光NPC
                                Projectile.Kill();
                                return;
                            }

                            Projectile.velocity = -Angle.ToRotationVector2();
                            break;
                        }

                        Timer++;
                    }
                    break;
                case 1: //普通的射出
                    {
                        if (Projectile.velocity.Length() < 24)
                        {
                            Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.3f;
                            Projectile.rotation = Projectile.velocity.ToRotation();
                        }
                    }
                    break;
            }

        }
    }
}
