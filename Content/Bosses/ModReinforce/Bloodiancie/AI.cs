using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    public partial class Bloodiancie
    {
        public List<int> meleeList = new List<int>();
        public List<int> shootList = new List<int>();

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            followers = new List<BloodiancieFollower>();
            SpawnFollowers(6);

            NPC.TargetClosest(false);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                State = (int)AIStates.onSpawnAnim;
                NPC.Center = Target.Center - new Vector2(0, 600);
                NPC.netUpdate = true;
            }
        }

        private enum AIStates : int
        {
            onKillAnim = -6,
            onSpawnAnim = -5,
            /// <summary> 赤色脉冲 </summary>
            pulse = -4,
            /// <summary> 生成血球并爆出血雨 </summary>
            bloodRain = -3,
            /// <summary> 烟花攻击 </summary>
            firework = -2,
            /// <summary> 追踪玩家后横向爆炸多次 </summary>
            explosionHorizontally = -1,
            /// <summary> 赤色爆冲 </summary>
            dash = 0,
            /// <summary> 多段爆炸 </summary>
            explosion = 1,
            /// <summary> 向上射击 </summary>
            upShoot = 2,
            /// <summary> 射出会爆炸的炸弹 </summary>
            shootBomb = 3,
            /// <summary> 赤玉激光 </summary>
            magicShoot = 4,
            /// <summary> 召唤小赤玉灵 </summary>
            summon = 5
        }

        public enum CyclingType : int
        {
            /// <summary> 一次近战一次远程循环 </summary>
            one_one,
            /// <summary> 两次近战一次远程循环 </summary>
            two_one,
            /// <summary> 两次近战两次远程循环 </summary>
            two_two,
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)//没有玩家存活时离开
                {
                    NPC.velocity.X *= 0.97f;
                    NPC.velocity.Y += 0.04f;
                    NPC.EncourageDespawn(10);
                    SkyManager.Instance.Deactivate("BloodJadeSky");
                    ChangeRotationNormally();
                    UpdateFollower_Idle();
                    return;
                }
            }

            if (OwnedFollowersCount != followers.Count)
                RespawnFollowers();

            float distanceX = Target.Center.X - NPC.Center.X;
            NPC.direction = distanceX > 0 ? 1 : -1;
            NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;

            Lighting.AddLight(NPC.Center, Color.Red.ToVector3());
            switch ((int)State)
            {
                case (int)AIStates.onKillAnim:
                    {
                        SlowDownAndGoUp(0.96f, -0.05f, -0.5f);
                        ChangeRotationNormally();
                        UpdateFollower_Summon();

                        if (Timer < 30)
                            break;

                        if (Timer > 40 && Timer % 20 == 0 && Main.netMode != NetmodeID.Server)
                        {
                            Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, 1).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore2").Type);
                            Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, 1).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore3").Type);
                            Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore4").Type);
                        }

                        if (Timer < 230 && Timer % 15 == 0)
                            Helper.RedJadeExplosion(NPC.Center + Main.rand.NextVector2Circular(30, 40));

                        if (Timer == 245)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<Bloodiancie_BigBoom>(), 150, 8f);
                            if (Main.netMode != NetmodeID.Server)
                            {
                                var modifier = new PunchCameraModifier(NPC.Center, Main.rand.NextVector2CircularEdge(1, 1), 10, 6f, 20, 1000f);
                                Main.instance.CameraModifiers.Add(modifier);
                            }
                        }

                        if (Timer > ON_KILL_ANIM_TIME)
                            NPC.Kill();
                    }
                    break;
                case (int)AIStates.onSpawnAnim:         //生成时的动画
                    {
                        if (Timer == 0) //生成动画弹幕
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<Bloodiancie_OnSpawnAnim>(), 0, 0);
                            NPC.velocity = new Vector2(0, 1.5f);
                            NPC.dontTakeDamage = true;
                        }

                        if (Timer % 5 == 0 && Main.netMode != NetmodeID.Server)
                        {
                            int count = Timer / 25;
                            for (int i = 0; i < count; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 3, count * 3), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.2f);
                                dust.noGravity = true;
                            }
                        }

                        if (Timer > 120)
                            NPC.velocity *= 0.998f;

                        if (Timer == 260)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<Bloodiancie_BigBoom>(), 80, 8f);
                            SkyManager.Instance.Activate("BloodJadeSky");
                        }

                        if (Timer == 270 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            State = (int)AIStates.explosion;
                            RefillAIList(1);
                            Timer = 0;
                            NPC.TargetClosest();
                            NPC.dontTakeDamage = false;
                            NPC.netUpdate = true;
                        }

                        ChangeRotationNormally();
                        UpdateFollower_Idle();
                    }
                    break;
                case (int)AIStates.pulse:               //蓄力射出大型弹幕
                    {
                        float yLength = NPC.Center.Y - Target.Center.Y;
                        if (yLength > -150)
                            SlowDownAndGoUp(0.98f, -0.14f, -1.5f);
                        else
                            NPC.velocity *= 0.99f;

                        Vector2 targetVec = Target.Center - NPC.Center;
                        float factor = Math.Clamp(targetVec.Length() / 150f, 0f, 1f);
                        Vector2 targetDir = targetVec.SafeNormalize(Vector2.One);
                        Vector2 targetCenter = NPC.Center + targetDir * (32 + factor * 32);
                        int realTime = Timer - 100;

                        ChangeRotationNormally();

                        if (Timer < 100)
                        {
                            UpdateFollower_Pulse(targetCenter, targetDir, factor, -1, 0.1f + 0.5f * Timer / 60);
                            for (int i = 0; i < 2; i++)
                                Helper.SpawnTrailDust(targetCenter + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, (d) => -targetDir * 6f, Scale: 1.1f);
                            break;
                        }
                        else if (Timer < 255)
                        {
                            UpdateFollower_Pulse(targetCenter, targetDir, factor, realTime % 45);
                            Helper.SpawnTrailDust(targetCenter + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, (d) => -targetDir * 6f, Scale: 1.1f + 2f * (realTime % 65) / 65f);
                        }
                        else
                            UpdateFollower_Idle(0.08f);

                        if (realTime % 45 == 0)//生成弹幕
                        {
                            if (!CanDespawnFollower())
                            {
                                ResetState();
                                break;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = Helper.ScaleValueForDiffMode(30, 35, 35, 30);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), targetCenter,
                                    (Target.Center - NPC.Center + Main.rand.NextVector2CircularEdge(48, 48)).SafeNormalize(Vector2.UnitY) * 12f,
                                    ProjectileType<RedPulse>(), damage, 5f, NPC.target);
                            }

                            Helper.PlayPitched("RedJade/RedJadeBeam", 0.13f, 0f, NPC.Center);
                            if (!DespawnFollowers(1))
                                ResetState();
                        }

                        if (Timer > 275)
                            ResetState();
                    }
                    break;
                case (int)AIStates.bloodRain:
                    {
                        if (Timer < 150)
                        {
                            float xLength = Math.Abs(Target.Center.X - NPC.Center.X);
                            if (xLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 9.5f, 0.25f, 0.3f, 0.97f);
                            else
                                NPC.velocity.X *= 0.96f;
                            //控制Y方向的移动
                            float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6.5f, 0.2f, 0.4f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Timer % 5 == 0)
                            {
                                int count = Timer / 15;
                                for (int i = 0; i < count; i++)
                                {
                                    Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 4, count * 4), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.2f);
                                    dust.noGravity = true;
                                }
                            }
                        }
                        else
                            NPC.velocity *= 0.995f;

                        if (Timer == 160)       //生成弹幕
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero
                                , ProjectileType<Bloodiancie_BigBoom>(), Helper.ScaleValueForDiffMode(55, 55, 50, 45), 8f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Target.Center.X > NPC.Center.X ? 1 : -1, 0) * 12, ProjectileType<BloodBall>(), 1, 8f);
                            SpawnFollowers(8);
                        }

                        if (Timer > 180)
                            ResetState();

                        ChangeRotationNormally();
                        UpdateFollower_Firework();

                        break;
                    }
                case (int)AIStates.firework:
                    {
                        //控制X方向的移动
                        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 6f, 0.2f, 0.3f, 0.97f);

                        //控制Y方向的移动
                        float yLength2 = Math.Abs(Target.Center.Y - NPC.Center.Y);
                        if (yLength2 > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 4f, 0.2f, 0.3f, 0.97f);
                        else
                            NPC.velocity.Y *= 0.96f;

                        if (Timer == 2)
                        {
                            NPC.reflectsProjectiles = true;
                            NPC.dontTakeDamage = true;
                            RedShield.Spawn(NPC, 250);
                            SpawnFollowers(Main.getGoodWorld ? 6 : 3);
                        }

                        ChangeRotationNormally();
                        if (OwnedFollowersCount == 0 || followers.Count == 0)
                        {
                            ResetState();
                            break;
                        }

                        UpdateFollower_Firework();

                        if (Timer < 49)
                            break;

                        if (Timer % 25 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
                            int damage = NPC.GetAttackDamage_ForProjectiles(10, 15);
                            int timeleft = 16;
                            int howMany = Main.rand.Next(3, 6);
                            for (int i = 0; i < howMany; i++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), followers[^1].center, rot.ToRotationVector2() * 12, ProjectileType<RedFirework>(), damage, 5f, NPC.target, 0, timeleft + i * 10);
                                rot += MathHelper.TwoPi / howMany;
                            }

                            SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                            if (!DespawnFollowers(1))
                            {
                                ResetState();
                                RedShield.Kill();
                            }
                        }

                        if (Timer > 265)
                            ResetState();
                    }
                    break;
                case (int)AIStates.explosionHorizontally:          //追逐玩家并蓄力爆炸
                    {
                        //控制X方向的移动
                        if (Timer < 160)
                        {
                            float xLength = Math.Abs(Target.Center.X - NPC.Center.X);
                            if (xLength > 350)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 9.5f, 0.3f, 0.4f, 0.97f);
                            else if (xLength < 300)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 9.5f, 0.3f, 0.4f, 0.97f);
                            else
                                NPC.velocity.X *= 0.96f;
                            //控制Y方向的移动
                            float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 7.5f, 0.3f, 0.4f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Timer % 5 == 0)
                            {
                                int count = Timer / 13;
                                for (int i = 0; i < count; i++)
                                {
                                    Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 4, count * 4), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.2f);
                                    dust.noGravity = true;
                                }
                            }
                        }
                        else
                            NPC.velocity *= 0.997f;

                        if (Timer == 170)       //生成弹幕
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero
                                , ProjectileType<Bloodiancie_BigBoom>(), Helper.ScaleValueForDiffMode(55, 55, 50, 45), 8f);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Target.Center.X > NPC.Center.X ? 1 : -1, 0) * 12, ProjectileType<BloodWave>(), 1, 8f);
                            SpawnFollowers(8);
                        }

                        if (Timer > 210)
                            ResetState();

                        ChangeRotationNormally();
                        UpdateFollower_Idle();
                    }
                    break;
                case (int)AIStates.dash:            //连续5次冲刺攻击
                    {
                        int realTime = (Timer - 45) % 65;

                        do
                        {
                            if (Timer < 45)
                            {
                                NPC.rotation = NPC.rotation.AngleLerp((Target.Center - NPC.Center).ToRotation() + 1.57f, Timer / 45f);
                                if (Timer % 4 == 0)
                                {
                                    Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(70, 70), Vector2.UnitY * -2,
                                        CoraliteContent.ParticleType<HorizontalStar>(), Coralite.Instance.RedJadeRed, Main.rand.NextFloat(0.5f, 0.8f));
                                }
                                break;
                            }

                            if (realTime == 18 && Main.netMode != NetmodeID.Server)
                            {
                                SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                                Particle.NewParticle(NPC.Center + new Vector2(0, -16), Vector2.Zero, CoraliteContent.ParticleType<Flash_WithOutLine>(), Coralite.Instance.RedJadeRed, 1.5f);
                            }

                            if (realTime < 20)
                            {
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 5f, 0.2f, 0.3f, 0.97f);
                                float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);

                                if (yLength > 50)//控制Y方向的移动
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 4f, 0.15f, 0.3f, 0.97f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                break;
                            }

                            if (realTime == 22)//开始冲刺
                            {
                                SpawnFollowers(3);
                                NPC.velocity = (Target.Center + new Vector2(0, Timer / 70 % 2 == 0 ? 50 : -50) - NPC.Center).SafeNormalize(Vector2.One) * 14f;
                                NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            }

                            if (realTime < 51)//边冲边炸
                            {
                                if (realTime % 6 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<Rediancie_Explosion>()
                                        , Helper.ScaleValueForDiffMode(20, 30, 35, 30), 5f);

                                break;
                            }

                            if (realTime == 52 && Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ProjectileType<Bloodiancie_BigBoom>()
                                    , Helper.ScaleValueForDiffMode(100, 100, 90, 80), 5f);

                            float targetRot = NPC.velocity.Length() * 0.04f * NPC.direction;
                            NPC.rotation = NPC.rotation.AngleLerp(targetRot, 0.1f);

                            NPC.velocity *= 0.97f;
                        } while (false);

                        if (Timer > 65 * 5 + 45)
                            ResetState();

                        UpdateFollower_Idle();
                    }
                    break;
                default:
                case (int)AIStates.explosion:       //追着玩家并爆炸
                    {
                        do
                        {
                            if (Timer <= 210)
                            {
                                //控制X方向的移动
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 9f, 0.28f, 0.38f, 0.97f);

                                //控制Y方向的移动
                                float yLength2 = Math.Abs(Target.Center.Y - NPC.Center.Y);
                                if (yLength2 > 50)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 6.5f, 0.2f, 0.25f, 0.97f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                if (Timer % 3 == 0)
                                {
                                    int count = Timer % 70 / 10;
                                    for (int i = 0; i < count; i++)
                                    {
                                        Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 4, count * 4), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.06f);
                                        dust.noGravity = true;
                                    }
                                }

                                if (Timer % 70 == 0)//生成弹幕
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero,
                                        ProjectileType<Rediancie_BigBoom>(), Helper.ScaleValueForDiffMode(50, 50, 45, 45), 5f);
                                    SpawnFollowers(2);
                                }

                                break;
                            }

                            if (Timer < 310)
                            {
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 11.5f, 0.38f, 0.54f, 0.97f);

                                if (Timer % 10 == 0)//生成弹幕
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero
                                        , ProjectileType<Rediancie_Explosion>(), Helper.ScaleValueForDiffMode(30, 35, 35, 30), 5f);
                                }

                                //控制Y方向的移动
                                float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);
                                if (yLength > 50)
                                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 7.5f, 0.3f, 0.44f, 0.97f);
                                else
                                    NPC.velocity.Y *= 0.96f;

                                if (Timer % 5 == 0)
                                {
                                    int count = Timer / 25;
                                    for (int i = 0; i < count; i++)
                                    {
                                        Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 3, count * 3), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.2f);
                                        dust.noGravity = true;
                                    }
                                }
                                break;
                            }

                            if (Timer == 310)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero
                                    , ProjectileType<Bloodiancie_BigBoom>(), Helper.ScaleValueForDiffMode(80, 80, 75, 70), 8f);
                                SpawnFollowers(8);
                            }

                            if (Timer < 320)
                            {
                                NPC.velocity *= 0.9f;
                                break;
                            }

                            ResetState();

                        } while (false);

                        ChangeRotationNormally();
                        UpdateFollower_Idle();
                    }
                    break;
                case (int)AIStates.upShoot:         //朝上连续射击
                    {
                        float xLength = Math.Abs(Target.Center.X - NPC.Center.X);
                        if (xLength > 250)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 7.5f, 0.2f, 0.3f, 0.97f);
                        else
                            NPC.velocity.X *= 0.96f;

                        float yLength = NPC.Center.Y - Target.Center.Y;
                        if (yLength > -250)
                        {
                            NPC.velocity.Y -= 0.25f;
                            if (NPC.velocity.Y < -6)
                                NPC.velocity.Y = -6;
                        }
                        else if (yLength < -400)
                        {
                            NPC.velocity.Y += 0.2f;
                            if (NPC.velocity.Y < 6)
                                NPC.velocity.Y = 6;
                        }
                        else
                            NPC.velocity.Y *= 0.98f;

                        ChangeRotationNormally();

                        if (OwnedFollowersCount == 0 || followers.Count == 0)
                        {
                            ResetState();
                            break;
                        }

                        UpdateFollower_UpShoot();

                        if (Timer < 30)
                            break;

                        if (Timer < 200 && Timer % 20 == 0)//隔固定时间射弹幕
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int index = Main.rand.Next(followers.Count);
                                int damage = Helper.ScaleValueForDiffMode(40, 40, 35, 30);
                                int shootCount = Helper.ScaleValueForDiffMode(2, 3, 3, 4);
                                for (int i = 0; i < shootCount; i++)
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), followers[index].center, new Vector2(0, -10).RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), ProjectileType<Rediancie_Strike>(), damage, 5f, NPC.target);
                            }

                            SoundEngine.PlaySound(SoundID.Item5, NPC.Center);
                            if (!DespawnFollowers(1))
                                ResetState();
                        }

                        if (Timer > 260)
                            ResetState();
                    }
                    break;
                case (int)AIStates.shootBomb:
                    {
                        if (Timer < 60)
                        {
                            float xLength = Math.Abs(Target.Center.X - NPC.Center.X);
                            if (xLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 9.5f, 0.2f, 0.3f, 0.97f);
                            else
                                NPC.velocity.X *= 0.96f;
                            //控制Y方向的移动
                            float yLength = Math.Abs(Target.Center.Y - NPC.Center.Y);
                            if (yLength > 50)
                                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 7.5f, 0.2f, 0.4f, 0.97f);
                            else
                                NPC.velocity.Y *= 0.96f;

                            if (Timer % 5 == 0)
                            {
                                int count = Timer / 5;
                                for (int i = 0; i < count; i++)
                                {
                                    Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(count * 4, count * 4), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.2f);
                                    dust.noGravity = true;
                                }
                            }
                        }
                        else
                            NPC.velocity *= 0.995f;

                        if (Timer == 70)
                        {
                            Vector2 dir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                            float length = (Target.Center - NPC.Center).Length();
                            for (int i = -3; i < 4; i++)
                            {
                                int shootTime = Main.rand.Next(45, 80);
                                float velLength = length * Main.rand.NextFloat(0.55f, 2f) / shootTime;
                                if (velLength > 16)
                                    velLength = 16;

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir.RotatedBy(i * 0.3f) * velLength
                                    , ProjectileType<RedBomb>(), 1, 4, ai0: shootTime, ai2: Main.rand.Next(120, 480));

                                SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, NPC.Center);

                                if (!DespawnFollowers(1))
                                    break;
                            }
                        }

                        if (Timer > 120)
                            ResetState();

                        ChangeRotationNormally();
                        UpdateFollower_Idle();

                        break;
                    }
                case (int)AIStates.magicShoot:      //蓄力射好多魔法弹幕
                    {
                        float yLength = NPC.Center.Y - Target.Center.Y;
                        if (yLength > -150)
                            SlowDownAndGoUp(0.98f, -0.14f, -1.5f);
                        else
                            NPC.velocity *= 0.99f;

                        Vector2 targetVec = Target.Center - NPC.Center;
                        float factor = Math.Clamp(targetVec.Length() / 150f, 0f, 1f);
                        Vector2 targetDir = targetVec.SafeNormalize(Vector2.One);
                        Vector2 targetCenter = NPC.Center + targetDir * (32 + factor * 32);

                        if (Timer % 3 == 0)
                            for (int i = 0; i < 6; i++)
                                Helper.SpawnTrailDust(targetCenter + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby,
                                    (dust) => (NPC.Center - dust.position).SafeNormalize(Vector2.UnitY) * 3f, Scale: 1.3f);

                        ChangeRotationNormally();

                        if (Timer < 60)
                        {
                            UpdateFollower_MagicShoot(targetCenter, targetDir, factor, 0.1f + 0.5f * Timer / 60);
                            break;
                        }
                        else if (Timer < 140)
                            UpdateFollower_MagicShoot(targetCenter, targetDir, factor);
                        else
                            UpdateFollower_Idle(0.1f);


                        if (Timer % 15 == 0)//生成弹幕
                        {
                            if (!CanDespawnFollower())
                            {
                                ResetState();
                                break;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int damage = Helper.ScaleValueForDiffMode(40, 50, 40, 40);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), targetCenter, (Target.Center - NPC.Center + new Vector2(0, 30 * (Timer / 30) == 1 ? 1 : -1)).SafeNormalize(Vector2.UnitY) * 14f, ProjectileType<BloodiancieBeam>(), damage, 5f);
                            }

                            Helper.PlayPitched("RedJade/RedJadeBeam", 0.13f, 0f, NPC.Center);
                            if (!DespawnFollowers(1))
                                ResetState();
                        }

                        if (Timer > 155)
                            ResetState();
                    }
                    break;
                case (int)AIStates.summon:          //召唤小赤玉灵
                    {
                        float yLength = NPC.Center.Y - Target.Center.Y;
                        if (yLength > -150)
                            SlowDownAndGoUp(0.98f, -0.14f, -1.5f);
                        else
                            NPC.velocity *= 0.99f;

                        ChangeRotationNormally();

                        if (OwnedFollowersCount == 0 || followers.Count == 0)
                        {
                            ResetState();
                            break;
                        }

                        UpdateFollower_Summon();

                        if (Timer % 10 == 0)
                            for (int i = 0; i < 6; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(followers[^1].center + Main.rand.NextVector2Circular(20, 20), DustID.GemRuby, Vector2.Zero, 0, default, 1.3f);
                                dust.noGravity = true;
                            }

                        if (Timer % 40 == 0)
                        {
                            if (!CanDespawnFollower())
                            {
                                ResetState();
                                break;
                            }

                            //为了保证同场召唤物数量不会过多所以还是保留了这一段
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (Main.npc.Count((n) => n.active && n.type == NPCType<BloodiancieMinion>()) < Helper.ScaleValueForDiffMode(2, 3, 3, 4))
                                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)followers[^1].center.X, (int)followers[^1].center.Y, NPCType<BloodiancieMinion>());
                                else
                                {
                                    ResetState();
                                    break;
                                }
                            }

                            SoundEngine.PlaySound(CoraliteSoundID.MagicStaff_Item8, NPC.Center);
                            if (!DespawnFollowers(1))
                                ResetState();
                        }

                        if (Timer > 200)//防止出BUG
                            ResetState();
                    }
                    break;
            }

            Timer++;
        }

        public void ResetState()
        {
            NPC.reflectsProjectiles = false;
            NPC.dontTakeDamage = false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int phase = 1;
            if (NPC.life < NPC.lifeMax / 2)
                phase = 2;

            GetAICycling((CyclingType)MoveCyclingType, out int meleeCount, out int ShootCount);
            bool useMelee = MoveCount < meleeCount;
            //bool useShoot = MoveCount < meleeCount + ShootCount;

            if (ExchangeState && phase == 2)    //血量低于一半固定放小弟
            {
                State = (int)AIStates.summon;
                ExchangeState = false;
            }
            else if (useMelee)   //近战
            {
                State = Main.rand.NextFromCollection(meleeList);
                meleeList.Remove((int)State);
            }
            else   //远程
            {
                State = Main.rand.NextFromCollection(shootList);
                shootList.Remove((int)State);
            }

            //State = (int)AIStates.bloodRain;
            MoveCount += 1;
            if (MoveCount >= meleeCount + ShootCount)   //如果一轮全部执行完成那么就在次随机一下循环方式
            {
                MoveCount = 0;
                RefillAIList(phase);
                MoveCyclingType = Main.rand.Next(3) switch
                {
                    0 => (int)CyclingType.one_one,
                    1 => (int)CyclingType.two_one,
                    _ => (int)CyclingType.two_two,
                };
            }

            Timer = 0;
            NPC.TargetClosest();
            NPC.netUpdate = true;
        }

        public void RefillAIList(int phase)
        {
            meleeList.Clear();
            shootList.Clear();

            switch (phase)
            {
                default:
                case 1://一阶段
                    if (Main.masterMode || Main.getGoodWorld)   //大师模式及以上
                    {
                        meleeList.Add((int)AIStates.explosionHorizontally);
                        meleeList.Add((int)AIStates.explosion);
                        meleeList.Add((int)AIStates.explosion);

                        shootList.Add((int)AIStates.shootBomb);
                        shootList.Add((int)AIStates.upShoot);
                        shootList.Add((int)AIStates.magicShoot);
                        shootList.Add((int)AIStates.magicShoot);
                    }
                    else        //其他模式
                    {
                        meleeList.Add((int)AIStates.explosion);
                        meleeList.Add((int)AIStates.explosion);
                        meleeList.Add((int)AIStates.explosion);

                        shootList.Add((int)AIStates.upShoot);
                        shootList.Add((int)AIStates.upShoot);
                        shootList.Add((int)AIStates.magicShoot);
                        shootList.Add((int)AIStates.magicShoot);
                    }
                    break;
                case 2:     //二阶段
                    if (Main.masterMode || Main.getGoodWorld)   //大师模式及以上
                    {
                        meleeList.Add((int)AIStates.explosionHorizontally);
                        meleeList.Add((int)AIStates.explosion);
                        meleeList.Add((int)AIStates.dash);

                        shootList.Add((int)AIStates.shootBomb);
                        shootList.Add((int)AIStates.bloodRain);
                        shootList.Add((int)AIStates.pulse);
                        shootList.Add((int)AIStates.firework);
                        shootList.Add((int)AIStates.summon);
                    }
                    else        //其他模式
                    {
                        meleeList.Add((int)AIStates.explosion);
                        meleeList.Add((int)AIStates.explosion);
                        meleeList.Add((int)AIStates.explosion);

                        shootList.Add((int)AIStates.shootBomb);
                        shootList.Add((int)AIStates.upShoot);
                        shootList.Add((int)AIStates.magicShoot);
                        shootList.Add((int)AIStates.summon);
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取一次攻击动作循环种AI的近战类与远程类的具体个数
        /// </summary>
        /// <param name="cyclingType"></param>
        /// <param name="meleeMoveCount"></param>
        /// <param name="ShootMoveCount"></param>
        public static void GetAICycling(CyclingType cyclingType, out int meleeMoveCount, out int ShootMoveCount)
        {
            switch (cyclingType)
            {
                default:
                case CyclingType.one_one:
                    meleeMoveCount = 1;
                    ShootMoveCount = 1;
                    break;
                case CyclingType.two_one:
                    meleeMoveCount = 2;
                    ShootMoveCount = 1;
                    break;
                case CyclingType.two_two:
                    meleeMoveCount = 2;
                    ShootMoveCount = 2;
                    break;
            }
        }

        #region HelperMethods

        public void SlowDownAndGoUp(float slowDownX, float accelY, float velocityLimitY)
        {
            NPC.velocity.X *= slowDownX;
            NPC.velocity.Y += accelY;
            if (NPC.velocity.Y < velocityLimitY)
                NPC.velocity.Y = velocityLimitY;
        }

        /// <summary> 最普通的改变旋转角度的方式 </summary>
        public void ChangeRotationNormally()
        {
            float targetRot = NPC.velocity.Length() * 0.04f * NPC.direction;
            NPC.rotation = NPC.rotation.AngleTowards(targetRot, 0.01f);
        }
        #endregion

        #endregion

        #region Followers

        /// <summary>
        /// 获得弹药
        /// </summary>
        /// <param name="howMany">获得弹药的数量</param>
        public void SpawnFollowers(int howMany)
        {
            int maxFollowers = Helper.ScaleValueForDiffMode(14, 18, 24, 30);
            if (OwnedFollowersCount >= maxFollowers)    ///弹药数已经达到上限
                return;

            if (OwnedFollowersCount + howMany > maxFollowers)   ///弹药数加上获得的弹药超出弹药上限时
            {
                int count = maxFollowers - (int)OwnedFollowersCount;
                OwnedFollowersCount = maxFollowers;
                for (int i = 0; i < count; i++)
                {
                    followers.Add(new BloodiancieFollower(NPC.Center));
                }

                NPC.defense = NPC.defDefense + (int)OwnedFollowersCount;
                return;
            }

            ///正常情况下
            OwnedFollowersCount += howMany;
            for (int i = 0; i < howMany; i++)
            {
                followers.Add(new BloodiancieFollower(NPC.Center));
            }

            NPC.defense = NPC.defDefense + (int)OwnedFollowersCount;
        }

        public void RespawnFollowers()
        {
            followers.Clear();

            int maxFollowers = Helper.ScaleValueForDiffMode(14, 18, 24, 30);
            if (OwnedFollowersCount >= maxFollowers)    ///弹药数已经达到上限
                OwnedFollowersCount = maxFollowers;

            for (int i = 0; i < (int)OwnedFollowersCount; i++)
            {
                followers.Add(new BloodiancieFollower(NPC.Center));
            }

            NPC.defense = NPC.defDefense + (int)OwnedFollowersCount;
        }

        /// <summary> 消耗弹药，返回false说明无法消耗弹药，返回true说明成功消耗了弹药 </summary>
        /// <param name="howMany"></param>
        /// <returns></returns>
        public bool DespawnFollowers(int howMany)
        {
            if (OwnedFollowersCount == 0 || followers.Count == 0)
                return false;

            for (int i = 0; i < howMany; i++)
            {
                OwnedFollowersCount -= 1;
                followers.RemoveAt(followers.Count - 1);
            }

            NPC.defense = NPC.defDefense + (int)OwnedFollowersCount;

            if (OwnedFollowersCount == 0 || followers.Count == 0)
                return false;

            return true;
        }

        /// <summary> 是否能消耗弹药 </summary>
        public bool CanDespawnFollower() => OwnedFollowersCount != 0;

        public void UpdateFollower_Idle(float centerLerpSpeed = 0.6f)
        {
            float velLength = NPC.velocity.Length();
            float baseRot = Timer * 0.08f + velLength * 0.15f;
            float length = 48 + velLength / 2 + OwnedFollowersCount;
            ///额...总之是非常复杂的立体解析几何，用于计算当前这个圆以X轴为轴的旋转角度，根据玩家位置来的
            float CircleRot = 1.57f - Math.Clamp((Target.Center.Y - NPC.Center.Y) / 200, -1f, 1f) * 0.4f;
            for (int i = 0; i < followers.Count; i++)
            {
                FollowersAI_Idle(followers[i], i, baseRot, length, CircleRot, centerLerpSpeed);
            }
        }

        public void UpdateFollower_UpShoot(float centerLerpSpeed = 0.6f)
        {
            float baseRot = Timer * 0.1f;
            float length = 48 + Timer * 0.4f;
            float CircleRot = 1.57f - Math.Clamp((Target.Center.Y - NPC.Center.Y) / 200, -1f, 1f) * 0.4f;
            for (int i = 0; i < followers.Count; i++)
            {
                FollowersAI_UpShoot(followers[i], i, baseRot, length, CircleRot, centerLerpSpeed);
            }
        }

        public void UpdateFollower_MagicShoot(Vector2 targetCenter, Vector2 targetDir, float factor, float centerLerpSpeed = 0.6f)
        {
            if (OwnedFollowersCount == 0 || followers.Count == 0)
            {
                ResetState();
                return;
            }

            BloodiancieFollower lastFollower = followers[^1];
            lastFollower.center = Vector2.Lerp(lastFollower.center, targetCenter, 0.6f);
            lastFollower.rotation = lastFollower.rotation.AngleLerp(targetDir.ToRotation() + 1.57f, 0.2f);
            lastFollower.drawBehind = false;
            lastFollower.scale = 1f;

            float baseRot = Timer * 0.1f;
            float lengthFactor;
            //只是计算长度，大概有一个后坐力的效果
            if (Timer < 60)
                lengthFactor = 0f;
            else
            {
                float timeFactor = 1 - Timer % 15 / 15;
                float x = 1.465f * timeFactor;
                lengthFactor = x * MathF.Sin(x * x * x) / 1.186f;
            }

            float length = 46 + lengthFactor * 60;
            Matrix XRot = Matrix.CreateRotationX(0.2f + targetDir.Y * factor * 1.1f);
            Matrix YRot = Matrix.CreateRotationY(-(0.2f + targetDir.X * factor * 1.1f));
            for (int i = 0; i < followers.Count - 1; i++)
            {
                FollowerAI_MagicShoot(followers[i], i, baseRot, length, targetCenter, XRot, YRot, centerLerpSpeed);
            }
        }

        public void UpdateFollower_Summon()
        {
            float baseRot = Timer * 0.06f;
            float length = 48 + Math.Clamp(Timer * 30, 0, 30);
            float CircleRot = 1.57f - Math.Clamp((Target.Center.Y - NPC.Center.Y) / 200, -1f, 1f) * 0.4f;

            for (int i = 0; i < followers.Count; i++)
            {
                FollowersAI_Idle(followers[i], i, baseRot, length, CircleRot, 0.6f);
            }
        }

        public void UpdateFollower_Firework()
        {
            float baseRot = Timer * 0.06f;
            float length = 46 + Math.Clamp(Timer * 30, 0, 30);

            for (int i = 0; i < followers.Count; i++)//因为比较简单所以就直接写在里面了
            {
                BloodiancieFollower follower = followers[i];

                float rot = baseRot + i / (float)followers.Count * MathHelper.TwoPi;
                follower.center = Vector2.Lerp(follower.center, NPC.Center + rot.ToRotationVector2() * length, 0.1f + 0.5f * Math.Clamp(Timer / 60f, 0, 1));
                follower.rotation = follower.rotation.AngleLerp(NPC.rotation, 0.6f);
                follower.drawBehind = false;
                follower.scale = 1f;
            }
        }

        public void UpdateFollower_Pulse(Vector2 targetCenter, Vector2 targetDir, float factor, float timer = -1, float centerLerpSpeed = 0.6f)
        {
            if (OwnedFollowersCount == 0 || followers.Count == 0)
            {
                ResetState();
                return;
            }

            BloodiancieFollower lastFollower = followers[^1];
            lastFollower.center = Vector2.Lerp(lastFollower.center, targetCenter + targetDir * 16, 0.6f);
            lastFollower.rotation = lastFollower.rotation.AngleLerp(targetDir.ToRotation() + 1.57f, 0.2f);
            lastFollower.drawBehind = false;
            lastFollower.scale = 1.3f;

            float baseRot = Timer * 0.1f;
            float lengthFactor;
            //只是计算长度，大概有一个后坐力的效果
            if (timer < 0)
                lengthFactor = 0f;
            else
            {
                float timeFactor = 1 - timer / 65;
                float x = 1.465f * timeFactor;
                lengthFactor = x * MathF.Sin(x * x * x) / 1.186f;
            }

            float length = 26 + lengthFactor * 86;
            Matrix XRot = Matrix.CreateRotationX(0.2f + targetDir.Y * factor * 1.1f);
            Matrix YRot = Matrix.CreateRotationY(-(0.2f + targetDir.X * factor * 1.1f));
            for (int i = 0; i < followers.Count - 1; i++)
            {
                FollowerAI_MagicShoot(followers[i], i, baseRot, length, targetCenter, XRot, YRot, centerLerpSpeed);
            }
        }


        /// <summary>
        /// 默认动作，在赤玉灵身边环绕，需要输入计算好的角度及长度数值
        /// </summary>
        /// <param name="follower"></param>
        public void FollowersAI_Idle(BloodiancieFollower follower, int whoamI, float baseRot, float length, float CircleRot, float centerLerpSpeed)
        {
            float rot = baseRot + whoamI / (float)followers.Count * MathHelper.TwoPi;

            Vector2 vector2D = rot.ToRotationVector2();
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), Matrix.CreateRotationX(CircleRot));///将二维的向量转为3维的并绕着X轴旋转一下
            vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(NPC.rotation));///以Z为轴旋转，用来配合赤玉灵自身的旋转

            //将3维向量投影到二维
            float k1 = -1000 / (vector3D.Z - 1000);
            Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = NPC.Center + targetDir * length * follower.lengthOffset + new Vector2(0, MathF.Sin(whoamI * 1.2f) * 6);
            follower.center = Vector2.Lerp(follower.center, targetCenter, centerLerpSpeed);
            follower.rotation = follower.rotation.AngleLerp(NPC.rotation, 0.2f);
            follower.drawBehind = vector3D.Z > 0;
            follower.scale = 0.9f - vector3D.Z * 0.2f;
        }

        public void FollowersAI_UpShoot(BloodiancieFollower follower, int whoamI, float baseRot, float length, float CircleRot, float centerLerpSpeed)
        {
            float rot = baseRot + whoamI / (float)followers.Count * MathHelper.TwoPi;

            Vector2 vector2D = rot.ToRotationVector2();
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), Matrix.CreateRotationX(CircleRot));///将二维的向量转为3维的并绕着X轴旋转一下
            vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(NPC.rotation));///以Z为轴旋转，用来配合赤玉灵自身的旋转

            //将3维向量投影到二维
            float k1 = -1000 / (vector3D.Z - 1000);
            Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = NPC.Center + targetDir * length * follower.lengthOffset + new Vector2(0, MathF.Sin(whoamI * 1.2f) * 6);
            follower.center = Vector2.Lerp(follower.center, targetCenter, centerLerpSpeed);
            follower.rotation = follower.rotation.AngleLerp(NPC.rotation, 0.2f);
            follower.drawBehind = vector3D.Z > 0;
            follower.scale = 1f - vector3D.Z * (0.2f + 0.4f * Math.Clamp(length / 168f, 0, 1));
        }

        public void FollowerAI_MagicShoot(BloodiancieFollower follower, int whoamI, float baseRot, float length, Vector2 center, Matrix XRot, Matrix YRot, float centerLerpSpeed)
        {
            float totalCount = followers.Count - 1 == 0 ? 1 : followers.Count - 1;  //分母不能为0
            float rot = baseRot + MathHelper.TwoPi * whoamI / totalCount;

            Vector2 vector2D = rot.ToRotationVector2();
            ///在XY平面的圆，先以X轴为轴旋转，再以Y轴为轴旋转，最后达到大概瞄准玩家的圆圈的效果
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), XRot);
            vector3D = Vector3.Transform(vector3D, YRot);

            float k1 = -1000 / (vector3D.Z - 1000);
            Vector2 CircleDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = center + CircleDir * length * (follower.lengthOffset + 0.45f);

            follower.center = Vector2.Lerp(follower.center, targetCenter, centerLerpSpeed);
            follower.rotation = follower.rotation.AngleLerp(CircleDir.ToRotation() + 1.57f, 0.4f);
            follower.drawBehind = vector3D.Z > 0;
            follower.scale = 0.9f - vector3D.Z * 0.3f;
        }

        #endregion
    }
}
