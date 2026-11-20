using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// ai0传入状态，0大1小，ai1传入飞行时间
    /// </summary>
    public class MucuryDroplet:ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls+Name;

        public ref float State => ref Projectile.ai[0];
        public ref float FlyTime => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float Recorder => ref Projectile.localAI[1];
        public ref float Recorder2 => ref Projectile.localAI[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Timer < FlyTime)
            {
                if (Recorder == 0)
                {
                    Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    Recorder = -MathF.Sign(Projectile.velocity.X);
                    Recorder2 = Projectile.velocity.Length();
                }

                Projectile.rotation += Projectile.velocity.Length() / 50f;
            }
            else if (Timer < FlyTime + 30)
            {
                Projectile.velocity *= 0.93f;
                if (State == 0)//转向
                {
                    Projectile.rotation = Projectile.rotation.AngleLerp(MathHelper.PiOver2 /*+ Recorder * Recorder2 / 30f*/, 0.2f);
                }
            }
            else if (Timer == FlyTime + 30)
            {
                Projectile.tileCollide = true;

                if (State == 0)//悬浮并释放激光
                {
                }
                else
                {
                }
            }
            else
            {
                if (State == 1)//掉落
                {
                    if (Projectile.velocity.Y < 14)
                    {
                        Projectile.velocity.Y += 0.15f;
                    }

                    Projectile.rotation += Projectile.velocity.Y / 40f;

                    if (Timer > FlyTime + 30 + 180)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    if (Timer< FlyTime + 30+ Projectile.ai[2])
                    {
                        Projectile.velocity *= 0.93f;
                    }
                    else if (Timer == FlyTime + 30 + Projectile.ai[2])
                    {
                        Projectile.NewProjectileFromThis<MucuryLaser_4Proj>(Projectile.Center, Vector2.Zero, Projectile.damage, 0, Projectile.whoAmI, 80, 16 * 80);
                    }
                    else if (Timer < FlyTime + 30 + Projectile.ai[2]+30)
                    {

                    }
                    else
                    {
                        if (Projectile.velocity.Y < 14)
                        {
                            Projectile.velocity.Y += 0.25f;
                        }

                        if (Timer > FlyTime + 30 + Projectile.ai[2] + 30 + 180)
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle box = new Rectangle(0, (int)State, 1, 2);
            Projectile.DrawShadowTrailsSacleStep(Color.White, 0.5f, 0.5f / 10, 1, 10, 1, 1 / 10f, box, 0, -1);

            Projectile.QuickFrameDraw(box, Color.White, 0);

            return false;
        }
    }
}
