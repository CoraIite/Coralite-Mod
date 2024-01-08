using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSparkle_Normal : BaseNightmareSparkle
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 400;

            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 32;
            ShineColor = NightmarePlantera.nightmareSparkleColor;
            mainSparkleScale = new Vector2(1f, 2f);
            circleSparkleScale = 0.4f;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 16)
            {
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.2f;
            }

            float dir2 = ((Projectile.timeLeft % 30) > 15 ? -1 : 1) * 0.025f;
            Projectile.velocity = Projectile.velocity.RotatedBy(dir2);

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(6))
            {
                Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<NightmareStar>(),
                    dir * Main.rand.NextFloat(1f, 4f), newColor: NightmarePlantera.lightPurple, Scale: Main.rand.NextFloat(1f, 1.75f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
            }
        }
    }

    public class NightmareSparkle_Red : BaseNightmareSparkle
    {
        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;

            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 32;
            ShineColor = NightmarePlantera.nightmareRed;
            mainSparkleScale = new Vector2(1f, 2f);
            circleSparkleScale = 0.4f;
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() < 17)
            {
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.35f;
            }

            float dir2 = ((Projectile.timeLeft % 30) > 15 ? -1 : 1) * 0.03f;
            Projectile.velocity = Projectile.velocity.RotatedBy(dir2);

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(6))
            {
                Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<NightmareStar>(),
                    dir * Main.rand.NextFloat(1f, 4f), newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(1f, 1.75f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
            }
        }
    }

    /// <summary>
    /// 使用ai0输入旋转时间，大于旋转时间后会射出<br></br>
    /// 使用ai1控制是否会在结束旋转后生成美梦光弹幕<br></br>
    /// 使用ai2传入目标的长度
    /// </summary>
    public class NightmareSparkle_Rolling : BaseNightmareSparkle
    {
        public Player Owner => Main.player[Projectile.owner];

        public ref float RollingTime => ref Projectile.ai[0];
        public ref float CanExchange => ref Projectile.ai[1];
        public ref float TargetLenght => ref Projectile.ai[2];

        public ref float Angle => ref Projectile.localAI[0];
        public ref float State => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.localAI[2];

        private bool init = true;
        private float length;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 32;
            //ShineColor = Color.Transparent;
            //mainSparkleScale = new Vector2(1.5f, 3f);
            //circleSparkleScale = 0.5f;
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
                        Angle += 0.03f;
                        float factor = Timer / RollingTime;

                        ShineColor = Color.Lerp(Color.Transparent, NightmarePlantera.nightPurple, factor);
                        ShineColor.A = 0;
                        mainSparkleScale = Vector2.Lerp(Vector2.Zero, new Vector2(1f, 2f), factor);
                        circleSparkleScale = MathHelper.Lerp(0, 0.4f, factor);

                        Projectile.Center = Owner.Center + Angle.ToRotationVector2() * MathHelper.Lerp(length, TargetLenght, factor);
                        Projectile.rotation = Angle;

                        if ((int)CanExchange == 1)
                        {
                            Projectile.rotation += Main.rand.NextFloat(-0.2f, 0.2f);
                        }

                        if (Timer > RollingTime)
                        {
                            State++;
                            Timer = 0;
                            if ((int)CanExchange == 1)
                            {
                                State++;
                                Projectile.velocity = -Angle.ToRotationVector2() * 8;
                                break;
                            }

                            Projectile.velocity = -Angle.ToRotationVector2();
                            break;
                        }

                        Timer++;
                    }
                    break;
                case 1: //普通的射出
                    {
                        if (Projectile.velocity.Length() < 20)
                        {
                            Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.1f;
                            Projectile.rotation = Projectile.velocity.ToRotation();
                        }
                    }
                    break;
                case 2://抽搐一会
                    {
                        Projectile.velocity *= 0.95f;
                        Projectile.rotation += Main.rand.NextFloat(0.1f, 0.2f);

                        if (Timer > 120)
                        {
                            NightmarePlantera.TargetFantasySparkle = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<FantasySparkle>());
                            Projectile.Kill();
                        }

                        Timer++;
                    }
                    break;

            }

        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }
    }
}
