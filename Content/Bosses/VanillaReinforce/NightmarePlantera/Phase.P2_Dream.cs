using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera
    {
        public void Dream_Phase2()
        {
            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;

            //设置三条拖尾

            rotateTentacles ??= new RotateTentacle[3]
            {
                new RotateTentacle(20, TentacleColor, TentacleWidth, tentacleTex, tentacleFlowTex)
                {
                    targetPos=NPC.Center
                },
                new RotateTentacle(20,TentacleColor,TentacleWidth,tentacleTex,tentacleFlowTex)
                {
                    targetPos=NPC.Center
                },
                new RotateTentacle(20,TentacleColor,TentacleWidth,tentacleTex,tentacleFlowTex)
                {
                    targetPos=NPC.Center
                },
            };

            switch ((int)State)
            {
                default:
                case (int)AIStates.nightmareBite:
                    NormallySetTentacle();
                    NightmareBite();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.rollingThenBite:
                    RollingThenBite();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.spikeBalls:
                    NormallySetTentacle();
                    SpikeBall();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.hookSlash:
                    NormallySetTentacle();
                    HookSlash();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.spikesAndSparkles:
                    NormallySetTentacle();
                    SpikesAndSparkles();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.spikeHell:
                    NormallySetTentacle();
                    SpikeHell();
                    NormallyUpdateTentacle();
                    break;
            }

        }

        #region AI

        public void NightmareBite()
        {
            switch ((int)SonState)
            {
                default:
                case 0: //逐渐消失
                    {
                        Phase2Fade(() =>
                        {
                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return Target.Center + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return Target.Center + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(40, 40, 30, 30);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<NightmareBite>(), damage, 4, ai0: 0, ai1: 60, ai2: -1);
                            //SoundStyle st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            //st.Volume -= 0.2f;
                            //st.Pitch = -1;
                            //SoundEngine.PlaySound(st, NPC.Center);
                        });
                    }
                    break;
                case 1:
                    {
                        DoRotation(0.1f);

                        if (Vector2.Distance(NPC.Center, Target.Center) > 220)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.65f;
                            if (speed > 24)
                                speed = 24;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                        {
                            NPC.velocity *= 0.8f;
                        }

                        if (Timer > 60)
                        {
                            useMeleeDamage = false;
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2: //咬完了之后的后摇阶段
                    {
                        NPC.velocity *= 0.9f;

                        if (Timer == 7)
                        {
                            SoundStyle st = CoraliteSoundID.BottleExplosion_Item107;
                            st.Pitch = 1f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 20)
                        {
                            DoRotation(0.04f);
                        }

                        if (Timer > 40)
                        {
                            SetPhase2States();
                        }
                    }
                    break;
            }

            Timer++;
        }

        public void RollingThenBite()
        {
            switch ((int)SonState)
            {
                default:
                case 0://逐渐消失，然后瞬移到玩家面前
                    {
                        Phase2Fade(() => Target.Center + new Vector2(-Target.direction, 0) * Main.rand.NextFloat(300, 400),
                            () =>
                            {
                                SonState++;
                                NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                            });

                        NormallySetTentacle();
                    }
                    break;
                case 1://旋转起来然后向周围放出噩梦光弹幕
                    {
                        const int RollingTime = 360;
                        const int fadeTime = 45;

                        float rotFactor = Math.Clamp(Timer / ((float)RollingTime / 3), 0, 1);
                        NPC.rotation += rotFactor * 0.35f;

                        int delay = 30 - (int)(rotFactor * 20);
                        if (Timer % delay == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(50, 40, 40, 40);
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 dir = (NPC.rotation + i * MathHelper.PiOver2).ToRotationVector2();

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir, ProjectileType<NightmareSparkle_Normal>(),
                                    damage, 0);
                            }
                            SoundStyle st = CoraliteSoundID.CrystalSerpent_Item109;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        Vector2 center = NPC.Center;
                        float factor2 = Timer / (float)RollingTime;

                        for (int i = 0; i < 3; i++)
                        {
                            RotateTentacle tentacle = rotateTentacles[i];
                            float targetRot = factor2 * MathHelper.TwoPi * 10 + i * MathHelper.TwoPi / 3;
                            Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                                center + 170 * targetRot.ToRotationVector2(), 0.2f);
                            tentacle.SetValue(selfPos, NPC.Center, targetRot);
                            tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20);
                        }

                        if (Timer > RollingTime - fadeTime)
                        {
                            if (Timer == RollingTime - fadeTime)
                                SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, NPC.Center);

                            if (alpha > 0)
                            {
                                alpha -= 1 / (float)fadeTime;
                                if (alpha < 0)
                                    alpha = 0;
                            }

                            float factor = Timer / (float)fadeTime;
                            canDrawWarp = true;
                            warpScale = MathF.Sin(factor * MathHelper.Pi) * 2f;

                            if (Timer == RollingTime - fadeTime * 3 / 4)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    Vector2 dir2 = Helper.NextVec2Dir();
                                    Dust dust = Dust.NewDustPerfect(NPC.Center + dir2 * Main.rand.Next(0, 64), DustType<NightmareStar>(),
                                        dir2 * Main.rand.NextFloat(2f, 6f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
                                    dust.rotation = dir2.ToRotation() + MathHelper.PiOver2;
                                }

                                SoundStyle st = CoraliteSoundID.NoUse_SuperMagicShoot_Item68;
                                st.Pitch = -1;
                                SoundEngine.PlaySound(st, NPC.Center);
                            }
                        }

                        if (Timer > RollingTime)
                        {
                            SonState++;
                            NPC.velocity = new Vector2(0, 48);
                            alpha = 1;
                            Timer = 0;
                            canDrawWarp = false;

                            int direction = Math.Sign(Target.Center.X - NPC.Center.X);
                            NPC.Center = Target.Center + new Vector2(direction * Main.rand.Next(400, 600), -600);
                            NPC.rotation = MathHelper.PiOver2;

                            for (int i = 0; i < 3; i++)
                            {
                                RotateTentacle tentacle = rotateTentacles[i];
                                tentacle.pos = tentacle.targetPos = NPC.Center;
                                tentacle.rotation = NPC.rotation;
                            }

                            //生成裂缝的拖尾弹幕
                            int damage = Helper.ScaleValueForDiffMode(40, 40, 30, 30);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<NightmareBite>(), damage, 4, ai0: 0, ai1: 10, ai2: -1);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ProjectileType<NightmareSlit>(), damage, 4);
                        }
                    }
                    break;
                case 2://瞬移到玩家另一侧之后从上向下咬下
                    {
                        const int MaxDashTime = 20;
                        NormallySetTentacle();

                        if (Timer < MaxDashTime)
                            break;

                        NPC.velocity *= 0.9f;
                        DoRotation(0.04f);

                        if (Timer < MaxDashTime + 20)
                            break;

                        SonState++;
                        Timer = 0;
                        NightmareSlit.StopTracking();
                    }
                    break;
                case 3://后摇
                    {
                        DoRotation(0.3f);
                        CircleMovement(340, 16, 0.25f, 180);

                        if (Timer == 20)
                        {
                            NightmareSlit.Exposion();
                        }

                        if (Timer > 9 * 15 + 70)
                        {
                            NPC.velocity *= 0;
                            SetPhase2States();
                        }

                        NormallySetTentacle(); 
                    }
                    break;
            }

            Timer++;
        }

        public void SpikeBall()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0: //消失，之后瞬移到玩家头上
                    {
                        Phase2Fade(() => Target.Center + new Vector2(0, -Main.rand.NextFloat(300, 400)),
                            () =>
                            {
                                SonState++;
                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                                NPC.velocity *= 0;
                            });
                    }
                    break;
                case 1: //每隔一段时间射出一个梦魇球
                    {
                        DoRotation(0.1f);
                        CircleMovement(540, 16, 0.25f, 360);

                        if (Timer % 25 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int damage = Helper.ScaleValueForDiffMode(50, 40, 40, 40);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center,
                                NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)),
                                ProjectileType<BlackHole>(), damage, 0, NPC.target, Timer + Main.rand.Next(0, 360), 250);
                        }

                        if (Timer > 150)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2: //冲到玩家脸上吓唬一下
                    {
                        DoRotation(0.3f);
                        CircleMovement(540, 16, 0.25f, 360);

                        if (Timer > 400)
                            SetPhase2States();
                    }
                    break;
            }

            Timer++;
        }

        public void HookSlash()
        {
            switch ((int)SonState)
            {
                default:
                case 0://瞬移到玩家面前
                    {
                        Phase2Fade(() => Target.Center + (SonState - 1) * MathHelper.Pi.ToRotationVector2() * Main.rand.NextFloat(300, 400),
                            () =>
                            {
                                SonState = Main.rand.Next(1, 3);
                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                                ShootCount = Main.rand.Next(3, 5);
                            });
                    }
                    break;
                case 1://放出触手并持续追踪玩家
                case 2:
                    {
                        HookSlashMovement();
                        if (Timer % 60 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(50, 40, 40, 40);
                            float angle2 = (Timer % 120 == 0 ? MathHelper.PiOver4 : -MathHelper.PiOver4) + Main.rand.NextFloat(-0.25f, 0.25f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (NPC.Center - Target.Center).SafeNormalize(Vector2.One) * 64, Vector2.Zero, ProjectileType<HookSlash>(), damage,
                                0, NPC.target, ai1: angle2, ai2: 100);
                        }

                        if (Timer < ShootCount * 60 + 30)
                            break;

                        SonState = 3;
                        Timer = 0;
                        SoundStyle st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                        st.Pitch = -0.7f;
                        SoundEngine.PlaySound(st, NPC.Center);
                        canDrawWarp = true;

                    }
                    break;
                case 3://自己冲向玩家
                    {
                        if (Timer < 20)
                        {
                            Vector2 dir = (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero);
                            if (NPC.velocity.Length() < 12)
                                NPC.velocity += dir * 0.15f;

                            DoRotation(0.3f);
                            alpha -= 0.5f / 20f;
                            warpScale += 1.5f / 20f;
                            break;
                        }

                        if (Timer < 40)
                        {
                            Vector2 dir = (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero);
                            if (NPC.velocity.Length() < 12)
                                NPC.velocity += dir * 0.15f;

                            DoRotation(0.3f);
                            alpha += 0.5f / 20f;
                            warpScale -= 1.5f / 20f;
                        }

                        if (Timer == 40)
                        {
                            alpha = 1;
                            warpScale = 0;
                            canDrawWarp = false;
                            useMeleeDamage = true;
                            NPC.velocity += NPC.rotation.ToRotationVector2() * 20;
                            if (NPC.velocity.Length() > 50)
                            {
                                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 50;
                            }
                        }

                        if (Timer > 80 || Vector2.Distance(NPC.Center, Target.Center) > 1400)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 4://后摇
                    {
                        NPC.velocity *= 0.96f;
                        DoRotation(0.3f);

                        if (Timer == 14)
                        {
                            int damage = Helper.ScaleValueForDiffMode(40, 40, 30, 30);
                            for (int i = 0; i < 7; i++)
                            {
                                Vector2 dir = (NPC.rotation + i * 1 / 7f * MathHelper.TwoPi).ToRotationVector2();

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, dir, ProjectileType<NightmareSparkle_Normal>(),
                                    damage, 0);
                            }
                        }

                        if (Timer > 50)
                            SetPhase2States();
                    }
                    break;
            }

            Timer++;
        }

        public void SpikesAndSparkles()
        {
            switch ((int)SonState)
            {
                default:
                case 0:   //瞬移到玩家某一侧，瞬移之后在另一侧生成一堆尖刺
                    {
                        Phase2Fade(() => Target.Center + ((SonState - 1) * MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat(700, 800),
                            () =>
                            {
                                SonState = Main.rand.Next(1, 3);
                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                                ShootCount = Main.rand.Next(3, 5);

                                float rot = (SonState - 1) * MathHelper.Pi+MathHelper.Pi;
                                int damage = Helper.ScaleValueForDiffMode(40, 40, 30, 30);

                                for (int i = -4; i < 4; i++)
                                {
                                    Vector2 dir = (rot + i * 0.4f).ToRotationVector2();

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), Target.Center + dir * Main.rand.NextFloat(500, 800) + Target.velocity * 15,
                                        -dir, ProjectileType<ConfusionHole>(), damage, 0, NPC.target, 120, -1, Main.rand.Next(800, 1200));
                                }
                            });
                    }
                    break;
                case 1:  //不断向玩家吐噩梦光并在另一侧不断生成尖刺
                case 2:
                    {
                        DoRotation(0.3f);

                        float angle = (SonState - 1) * MathHelper.Pi + MathHelper.PiOver4/2 * MathF.Sin(Timer * 0.0314f);
                        Vector2 center = Target.Center + angle.ToRotationVector2() * 850;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 400f, 0, 1) * 30 ;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);

                        if (Timer % 12 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(40, 40, 30, 30);
                            Vector2 dir2 = NPC.rotation.ToRotationVector2();
                            Vector2 furture = Target.velocity * Target.velocity.Length() * 3f;

                            Vector2 center2 = Target.Center + dir2.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(500, 800) + furture;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), center2,
                                (Target.Center + furture - center2).SafeNormalize(Vector2.Zero), ProjectileType<ConfusionHole>(), damage, 0, NPC.target, Main.rand.Next(60, 80), -1, Main.rand.Next(600, 900));

                            center2 = Target.Center + ((Timer % 24 == 0 ? 0 : MathHelper.Pi) + Main.rand.NextFloat(MathHelper.PiOver2 - 0.3f, MathHelper.PiOver2 + 0.3f)).ToRotationVector2() * Main.rand.Next(500, 700) + furture;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), center2,
                                (Target.Center + furture - center2).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)), ProjectileType<ConfusionHole>(), damage, 0, NPC.target, 75, -1, Main.rand.Next(800, 1200));
                        }

                        if (Timer % 28 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(40, 40, 30, 30);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2(), ProjectileType<NightmareSparkle_Normal>(),
                                damage, 0);
                        }

                        if (Timer>300)
                        {
                            SonState = 3;
                            Timer = 0;
                        }
                    }
                    break;
                case 3:  //后摇
                    {
                        SetPhase2States();
                    }
                    break;
            }

            Timer++;
        }

        public void SpikeHell()
        {
            switch ((int)SonState)
            {
                default:
                case 0://瞬移到玩家左侧
                    {
                        Phase2Fade(() => Target.Center + (SonState % 2 * MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat(700, 800),
                            () =>
                            {
                                SonState++;
                                NPC.rotation = (SonState % 2 * MathHelper.Pi) + MathHelper.PiOver2;
                                NPC.velocity = NPC.rotation.ToRotationVector2();
                            });
                    }
                    break;
                case 1://在玩家周围旋转并放出尖刺
                case 2:
                case 3:
                    {
                        float baseRot = (SonState % 2) * MathHelper.Pi;

                        const float RollingTime = 120f;
                        if (Timer < RollingTime)
                        {
                            float currentRot = baseRot + Timer / RollingTime * MathHelper.TwoPi;

                            Vector2 center = Target.Center + currentRot.ToRotationVector2() * 800;
                            Vector2 dir = center - NPC.Center;

                            float velRot = NPC.velocity.ToRotation();
                            float targetRot = dir.ToRotation();

                            float speed = NPC.velocity.Length();
                            float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 46;

                            NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.65f);
                            NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.3f);

                            if (Timer % 5 == 0)
                            {
                                int damage = Helper.ScaleValueForDiffMode(40, 40, 30, 30);

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center,
                                    (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero), ProjectileType<ConfusionHole>(), damage, 0, NPC.target, 50, -1, 1300);
                            }
                            break;
                        }

                        const float fadeTime=30;
                        if (Timer == RollingTime)
                            SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, NPC.Center);

                        DoRotation(0.3f);
                        NPC.velocity *= 0.93f;

                        if (alpha > 0)
                        {
                            alpha -= 1 / fadeTime;
                            if (alpha < 0)
                                alpha = 0;
                        }

                        float factor = Timer / (float)fadeTime;
                        canDrawWarp = true;
                        warpScale = MathF.Sin(factor * MathHelper.Pi) * 2f;

                        if (Timer == RollingTime - fadeTime * 3 / 4)
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                Vector2 dir2 = Helper.NextVec2Dir();
                                Dust dust = Dust.NewDustPerfect(NPC.Center + dir2 * Main.rand.Next(0, 64), DustType<NightmareStar>(),
                                    dir2 * Main.rand.NextFloat(2f, 6f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
                                dust.rotation = dir2.ToRotation() + MathHelper.PiOver2;
                            }

                            SoundStyle st = CoraliteSoundID.NoUse_SuperMagicShoot_Item68;
                            st.Pitch = -1;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > RollingTime+fadeTime)
                        {
                            SonState++;
                            canDrawWarp = false;
                            alpha = 1;
                            Timer = 0;


                            float angle = SonState % 2 * MathHelper.Pi;
                            NPC.Center = Target.Center + angle.ToRotationVector2() * Main.rand.NextFloat(700, 800);
                            NPC.rotation = angle + MathHelper.PiOver2;
                            NPC.velocity = NPC.rotation.ToRotationVector2();

                            for (int i = 0; i < 3; i++)
                            {
                                RotateTentacle tentacle = rotateTentacles[i];
                                tentacle.pos = tentacle.targetPos = NPC.Center;
                                tentacle.rotation = NPC.rotation;
                            }
                        }
                    }
                    break;
                case 4://后摇
                    {
                        SetPhase2States();
                    }
                    break;
            }

            Timer++;
        }

        #endregion

        #region MiscMethods

        /// <summary>
        /// 自身逐渐消散，之后瞬移
        /// </summary>
        /// <param name="fadeTime"></param>
        /// <param name="teleportPos">要传送到的地方</param>
        public void Phase2Fade(Func<Vector2> teleportPos, Action OnTeleport, int fadeTime = 45)
        {
            if (Timer == 1)
            {
                SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, NPC.Center);
            }

            if (alpha > 0)
            {
                alpha -= 1 / (float)fadeTime;
                if (alpha < 0)
                    alpha = 0;
            }

            DoRotation(0.1f);
            Vector2 dir = (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero);
            if (NPC.velocity.Length() < 8)
                NPC.velocity += dir * 0.15f;

            float factor = Timer / (float)fadeTime;
            canDrawWarp = true;
            warpScale = MathF.Sin(factor * MathHelper.Pi) * 2f;

            if (Timer == fadeTime * 3 / 4)
            {
                for (int i = 0; i < 16; i++)
                {
                    Vector2 dir2 = Helper.NextVec2Dir();
                    Dust dust = Dust.NewDustPerfect(NPC.Center + dir2 * Main.rand.Next(0, 64), DustType<NightmareStar>(),
                        dir2 * Main.rand.NextFloat(2f, 6f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
                    dust.rotation = dir2.ToRotation() + MathHelper.PiOver2;
                }

                SoundStyle st = CoraliteSoundID.NoUse_SuperMagicShoot_Item68;
                st.Pitch = -1;
                SoundEngine.PlaySound(st, NPC.Center);
            }

            if (Timer > fadeTime)
            {
                NPC.velocity *= 0;
                alpha = 1;
                Timer = 0;
                canDrawWarp = false;
                warpScale = 0;
                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                OnTeleport?.Invoke();

                NPC.Center = teleportPos();

                for (int i = 0; i < 3; i++)
                {
                    RotateTentacle tentacle = rotateTentacles[i];
                    tentacle.pos = tentacle.targetPos = NPC.Center;
                    tentacle.rotation = NPC.rotation;
                }
            }
        }

        public void CircleMovement(float distance, float speedMax, float accelFactor = 0.25f, float rollingFactor = 360f)
        {
            Vector2 center = Target.Center + (Timer / rollingFactor * MathHelper.TwoPi).ToRotationVector2() * distance;
            Vector2 dir = center - NPC.Center;

            float velRot = NPC.velocity.ToRotation();
            float targetRot = dir.ToRotation();

            float speed = NPC.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 800f, 0, 1) * speedMax;

            NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, accelFactor);
        }

        /// <summary>
        /// 爪击中所使用到的移动方式
        /// </summary>
        public void HookSlashMovement()
        {
            DoRotation(0.3f);

            float angle = (SonState - 1) * MathHelper.Pi + MathHelper.PiOver4 * MathF.Sin(Timer * 0.0314f);
            Vector2 center = Target.Center + angle.ToRotationVector2() * 250;
            Vector2 dir = center - NPC.Center;

            float velRot = NPC.velocity.ToRotation();
            float targetRot = dir.ToRotation();

            float speed = NPC.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 800f, 0, 1) * 24;

            NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);
        }

        public void NormallySetTentacle()
        {
            Vector2 center = NPC.Center - NPC.velocity * 2;
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                float factor = MathF.Sin((float)Main.timeForVisualEffects / 10 + i * 1.5f);
                float targetRot = tentacle.rotation.AngleLerp(NPC.rotation + factor * 1.3f, 0.4f);
                Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                    center + (i * 30 + 140) * (NPC.rotation + factor * 0.65f + MathHelper.Pi).ToRotationVector2(), 0.2f);
                tentacle.SetValue(selfPos, NPC.Center, targetRot);
            }
        }

        public void NormallyUpdateTentacle()
        {
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20);
            }
        }

        #endregion

        #region States

        public void SetPhase2States()
        {
            NPC.dontTakeDamage = false;
            warpScale = 0;
            canDrawWarp = false;
            useMeleeDamage = false;
            tentacleColor = nightPurple;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            //State = (int)MoveCount switch
            //{
            //    0 => (int)AIStates.rollingThenBite,
            //    1 => (int)AIStates.nightmareBite,
            //    2 => (int)AIStates.hookSlash,
            //    3 =>(int)AIStates.spikesAndSparkles,
            //    4 => (int)AIStates.nightmareBite,
            //    5 => (int)AIStates.spikeBalls,
            //    6 =>,
            //    7 =>,
            //    8 => (int)AIStates.nightmareBite,
            //    9 =>  (int)AIStates.spikeHell,
            //    _ => ,
            //};

            State = MoveCount switch    //测试专用，最后记得注释掉
            {
                0 => (int)AIStates.nightmareBite,
                1 => (int)AIStates.rollingThenBite,
                2 => (int)AIStates.spikeBalls,
                3 => (int)AIStates.spikesAndSparkles,
                4 => (int)AIStates.spikeHell,
                _ => (int)AIStates.hookSlash,
            };

            State = (int)AIStates.spikeHell;

            MoveCount++;
            if (MoveCount > 5)
                MoveCount = 0;

            Timer = 0;
            SonState = 0;
            NPC.netUpdate = true;
        }

        /// <summary>
        /// 梦境阶段的战斗的重设状态
        /// </summary>
        public void SetPhase2DreamingStates()
        {

        }

        #endregion
    }
}
