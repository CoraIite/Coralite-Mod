using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// ai0传入状态，0大1小，ai1传入飞行时间
    /// </summary>
    public class MucuryDroplet : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float FlyTime => ref Projectile.ai[1];
        public int R = -1;
        public float A;
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

        public override bool? CanDamage()
        {
            if (Timer < FlyTime+30)
            {
                return false;
            }

            return base.CanDamage();
        }

        public override void AI()
        {
            const int slowTime = 45;

            if (Timer < FlyTime)
            {
                A=Helper.SqrtEase(Timer/FlyTime)*0.4f;
                if (Recorder == 0)
                {
                    Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    Recorder = -MathF.Sign(Projectile.velocity.X);
                    Recorder2 = Projectile.velocity.Length();
                }

                Projectile.rotation += Projectile.velocity.Length() / 50f;
            }
            else if (Timer < FlyTime + slowTime)
            {
                A = Helper.Lerp(A, 1, 0.2f);

                Projectile.velocity *= 0.95f;
                if (State == 0)//转向
                {
                    Projectile.rotation = Projectile.rotation.AngleLerp(MathHelper.PiOver2 /*+ Recorder * Recorder2 / 30f*/, 0.2f);
                }
            }
            else if (Timer == FlyTime + slowTime)
            {
                Projectile.tileCollide = true;

                if (State == 0)//悬浮并释放激光
                {
                    R = -1;
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

                    if (Timer > FlyTime + slowTime + 180)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    if (Timer < FlyTime + slowTime + Projectile.ai[2])
                    {
                        Projectile.velocity *= 0.93f;
                        if (Timer== FlyTime + Projectile.ai[2])
                        {
                            SpawnAimLine(16 * 100, 20, 10);
                        }
                    }
                    else if (Timer == FlyTime + slowTime + Projectile.ai[2])
                    {
                        R = Projectile.NewProjectileFromThis<MucuryLaser_4Proj>(Projectile.Center, Vector2.Zero, Projectile.damage, 0, Projectile.whoAmI, 80, 0);
                    }
                    else if (Timer < FlyTime + slowTime + Projectile.ai[2] + 30)
                    {

                    }
                    else
                    {
                        if (Projectile.velocity.Y < 14)
                        {
                            Projectile.velocity.Y += 0.25f;
                        }

                        if (Timer > FlyTime + slowTime + Projectile.ai[2] + 30 + 180)
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }

            Timer++;
        }

        public void SpawnAimLine(float targetLength, int spawnTime, int contiunedTime)
        {
            if (VaultUtils.isServer)
                return;

            var p = PRTLoader.NewParticle<BeamShotParticle>(Projectile.Center, Vector2.Zero, Color.Silver);

            p.bottomWidth = Projectile.width / 3;
            p.targetLength = targetLength;
            p.followProjIndex = Projectile.whoAmI;
            p.followEntityRot = true;
            p.spawnTime = spawnTime;
            p.contiundTime = contiunedTime;
            p.Rotation = Projectile.rotation;
        }

        public override void OnKill(int timeLeft)
        {
            if (State == 0 && R.GetProjectileOwner<MucuryLaser_4Proj>(out Projectile p) && p.ai[0] == Projectile.whoAmI)
            {
                p.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle box = new Rectangle(0, (int)State, 1, 2);
            Projectile.DrawShadowTrailsSacleStep(Color.White * A, 0.5f, 0.5f / 10, 1, 10, 2, 0.6f / 10f, box, 0, -1);

            Projectile.QuickFrameDraw(box, Color.White * A, 0);

            return false;
        }
    }
}
