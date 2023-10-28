using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera
    {
        /// <summary>
        /// 使用前请检测是否存在该NPC
        /// </summary>
        public static int TargetFantasySparkle = -1;

        /// <summary>
        /// 美梦神的index<br></br>
        /// 使用前请检测是否存在该NPC
        /// </summary>
        public static int FantasyGod = -1;

        public bool useDreamMove;
        public float DreamMoveCount;

        public bool haveBeenPhase2;

        public bool useFantasyHelp=true;

        public void Dream_Phase2()
        {
            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;

            //设置三条拖尾
            rotateTentacles ??= new RotateTentacle[3]
            {
                new RotateTentacle(20, TentacleColor, TentacleWidth, tentacleTex, waterFlowTex)
                {
                    pos = NPC.Center,
                    targetPos = NPC.Center
                },
                new RotateTentacle(20,TentacleColor,TentacleWidth,tentacleTex,waterFlowTex)
                {
                    pos = NPC.Center,
                    targetPos = NPC.Center
                },
                new RotateTentacle(20,TentacleColor,TentacleWidth,tentacleTex,waterFlowTex)
                {
                    pos = NPC.Center,
                    targetPos = NPC.Center
                },
            };
            UpdateFrame_P2();

            if (useDreamMove)
                switch ((int)State)
                {
                    default:
                        SetPhase2DreamingStates();
                        break;
                    case (int)AIStates.nightmareBite:
                        NormallySetTentacle();
                        DreamingNightmareBite();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.spikeHell:
                        NormallySetTentacle();
                        DreamingSpikeHell();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.fantasyHunting:
                        NormallySetTentacle();
                        DreamingFantasyHunting();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.P2_Idle:
                        NormallySetTentacle();
                        Dreaming_Idle();
                        NormallyUpdateTentacle();
                        break;
                }
            else
                switch ((int)State)
                {
                    default:
                        SetPhase2States();
                        break;
                    case (int)AIStates.nightmareBite:
                        NormallySetTentacle();
                        NightmareBite();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.nightmareDash:
                        NormallySetTentacle();
                        NightmareDash();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.fakeBite:
                        NormallySetTentacle();
                        FakeBite();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.fantasyHelp:
                        NormallySetTentacle();
                        FantayHelp();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.rollingThenBite:
                        RollingThenBite();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.belowSparkleThenBite:
                        BelowSparkleThenBite();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.spikeBalls:
                        NormallySetTentacle();
                        SpikeBall();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.batsAndCrows:
                        BatsAndCrows();
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
                    case (int)AIStates.ghostDash:
                        NormallySetTentacle();
                        GhostDash();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.teleportSparkle:
                        NormallySetTentacle();
                        TeleportSparkle();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.dreamSparkle:
                        NormallySetTentacle();
                        DreamSparkle();
                        NormallyUpdateTentacle();
                        break;
                    case (int)AIStates.P2_Idle:
                        NormallySetTentacle();
                        P2_Idle();
                        NormallyUpdateTentacle();
                        break;
                }
        }

        public void UpdateFrame_P2()
        {
            if (++NPC.frameCounter>7)
            {
                NPC.frameCounter = 0;
                NPC.frame.X++;
                if (NPC.frame.X > 3)
                    NPC.frame.X = 0;
            }
        }

        #region Phase2AI

        public void NightmareBite()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0: //逐渐消失
                    {
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 10, 10);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 75, ai2: -1);
                            //SoundStyle st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            //st.Volume -= 0.2f;
                            //st.Pitch = -1;
                            //SoundEngine.PlaySound(st, NPC.Center);
                        }, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            NPC.rotation = (pos - NPC.Center).ToRotation();
                        });
                    }
                    break;
                case 1:
                    {
                        Vector2 pos = Target.Center;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        NPC.rotation = NPC.rotation.AngleTowards((pos - NPC.Center).ToRotation(), 0.3f);
                        if (Vector2.Distance(NPC.Center, pos) > 220)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.65f;
                            if (speed > 26)
                                speed = 26;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                        {
                            NPC.velocity *= 0.8f;
                        }

                        if (Timer > 66)
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

                        if (Timer > 10)
                        {
                            DoRotation(0.04f);
                        }

                        if (Timer > 20)
                        {
                            SetPhase2States();
                        }
                    }
                    break;
            }

            Timer++;
            Target.hurtCooldowns[1] = Target.hurtCooldowns[1];
        }

        public void NightmareDash()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0: //逐渐消失
                    {
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(125, 150);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(125, 150);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 10, 10);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 90, ai2: -1);
                        }, 30,PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            SoundStyle st = CoraliteSoundID.DeathCalling_Item103;
                            st.Pitch -= 0.4f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            NPC.rotation = (pos - NPC.Center).ToRotation();
                            ShootCount = (NPC.Center - pos).ToRotation();
                        });
                    }
                    break;
                case 1: //先短暂同步玩家位置，之后向一个方向冲刺咬下
                    {
                        float factor = Timer / 78f;

                        Vector2 pos = Target.Center + new Vector2(Target.direction * factor * 80, 0) + Target.velocity * 14;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        if (Timer < 55)
                            NPC.rotation += MathHelper.TwoPi / 55;
                        else
                        {
                            float targetAngle = (pos - NPC.Center).ToRotation();
                            NPC.rotation = NPC.rotation.AngleTowards(targetAngle, 0.3f);
                        }

                        if (Timer < 70)
                        {
                            Vector2 center = Target.Center + ShootCount.ToRotationVector2() * Helper.Lerp(140, 480, factor);
                            Vector2 dir = center - NPC.Center;

                            float velRot = NPC.velocity.ToRotation();
                            float targetRot = dir.ToRotation();

                            float speed = NPC.velocity.Length();
                            float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 56;

                            NPC.velocity = velRot.AngleTowards(targetRot, 0.5f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        }
                        else
                            NPC.velocity *= 0.98f;

                        if (Timer > 78)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://冲刺并咬下
                    {
                        NPC.velocity *= 0.97f;
                        if (Timer > 6)
                        {
                            Vector2 pos = Target.Center + new Vector2(Target.direction * 80, 0) + Target.velocity * 14;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            NPC.velocity = (pos - NPC.Center).SafeNormalize(Vector2.One) * 48;
                            NPC.rotation = NPC.velocity.ToRotation();
                            Timer = 0;
                            SonState++;
                            useMeleeDamage = false;
                        }
                    }
                    break;
                case 3: //咬完了之后的后摇阶段
                    {
                        if (Timer>12)
                        {
                            NPC.velocity *= 0.9f;
                            DoRotation(0.04f);
                        }

                        Vector2 dir = -NPC.velocity.SafeNormalize(Vector2.Zero);
                        for (int i = 0; i < 5; i++)
                        {
                            Color c = Main.rand.Next(0, 2) switch
                            {
                                0 => nightPurple,
                                _=>lightPurple,
                            };
                            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustType<NightmarePetal>(), newColor: c);
                            dust.velocity = dir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.5f, 8);
                            dust.noGravity = true;
                        }

                        if (Timer > 20)
                        {
                            SetPhase2States();
                        }
                    }
                    break;
            }

            Timer++;
        }

        public void FakeBite()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0: //逐渐消失
                    {
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 10, 10);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 4, ai1: 65, ai2: -1);
                        }, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            NPC.rotation = (pos - NPC.Center).ToRotation();
                        });
                    }
                    break;
                case 1://假装靠近
                    {
                        Vector2 pos = Target.Center;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        if (Timer == 10)
                            SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, NPC.Center);

                        NPC.rotation = NPC.rotation.AngleTowards((pos - NPC.Center).ToRotation(), 0.3f);

                        if (Timer < 50&&Timer%3==0)
                            for (int i = -1; i < 2; i += 2)
                            {
                                Color c = Main.rand.Next(0, 1) switch
                                {
                                    0 => tentacleColor,
                                    _ => tentacleColor * 2f,
                                };
                                Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(32, 32), (NPC.rotation + i * 2.6f).ToRotationVector2() * Main.rand.NextFloat(12f, 24f),
                                    CoraliteContent.ParticleType<SpeedLine>(), c, Main.rand.NextFloat(0.3f, 0.5f));
                            }

                        if (Vector2.Distance(NPC.Center, pos) > 220)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.65f;
                            if (speed > 26)
                                speed = 26;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                        {
                            NPC.velocity *= 0.8f;
                        }

                        if (Timer > 65)
                        {
                            SonState++;
                            Timer = 0;
                            ShootCount = (NPC.Center - Target.Center).ToRotation();

                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<VineSpike>(NPC.Center, (ShootCount + 2f).ToRotationVector2(), damage, 8, NPC.target, 0, ShootCount + 0.9f, 35);
                            NPC.NewProjectileInAI<VineSpike>(NPC.Center, (ShootCount - 2f).ToRotationVector2(), damage, 8, NPC.target, 0, ShootCount - 0.9f, 65);
                        }
                    }
                    break;
                case 2: //射出2个荆棘刺，之后依次射出
                    {
                        DoRotation(0.3f);

                        float angle = ShootCount + MathHelper.PiOver4 / 4 * MathF.Sin(Timer * 0.0314f);
                        Vector2 center = Target.Center + angle.ToRotationVector2() * 450;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 400f, 0, 1) * 56;
                        NPC.velocity = velRot.AngleTowards(targetRot, 0.5f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.15f);

                        if (Timer > 80)
                        {
                            SetPhase2States();
                        }
                    }
                    break;
            }

            Timer++;
        }

        public void FantayHelp()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0: //逐渐消失
                    {
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 10, 10);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 125, ai2: -1);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)Target.Center.X, (int)Target.Center.Y, NPCType<FantasySparkle>(), NPC.whoAmI, 3, Target: NPC.target);
                        }, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            NPC.rotation = (pos - NPC.Center).ToRotation();
                        });
                    }
                    break;
                case 1:
                    {
                        Vector2 pos = Target.Center;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        NPC.rotation = NPC.rotation.AngleTowards((pos - NPC.Center).ToRotation(), 0.3f);
                        if (Vector2.Distance(NPC.Center, pos) > 220)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.65f;
                            if (speed > 26)
                                speed = 26;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                        {
                            NPC.velocity *= 0.8f;
                        }

                        if (Timer > 116)
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

                        if (Timer > 10)
                        {
                            DoRotation(0.04f);
                        }

                        if (Timer > 20)
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
                    SetPhase2States();
                    break;
                case 0://逐渐消失，然后瞬移到玩家面前
                    {
                        Phase2Fade(() => Target.Center + new Vector2(-Target.direction, 0) * Main.rand.NextFloat(300, 400),
                            () =>
                            {
                                SonState++;
                                NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                            }, fadeTime: 30);

                        NormallySetTentacle();
                    }
                    break;
                case 1://旋转起来然后向周围放出噩梦光弹幕
                    {
                        const int RollingTime = 360;
                        const int fadeTime = 45;

                        Color color = Main.rand.Next(0, 2) switch
                        {
                            0 => new Color(110, 68, 200),
                            _ => new Color(122, 110, 134)
                        };

                        Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(32, 32), Helper.NextVec2Dir() * Main.rand.NextFloat(2, 10f),
                            CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1f));

                        float rotFactor = Math.Clamp(Timer / ((float)RollingTime / 3), 0, 1);
                        NPC.rotation += rotFactor * 0.35f;

                        int delay = 30 - (int)(rotFactor * 20);
                        if (Timer % delay == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 dir = (NPC.rotation + i * MathHelper.PiOver2).ToRotationVector2();

                                NPC.NewProjectileInAI<NightmareSparkle_Normal>(NPC.Center, dir, damage, 0);
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
                            tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
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
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 10, ai2: -1);
                            NPC.NewProjectileInAI<NightmareSlit>(NPC.Center, Vector2.Zero, Helper.ScaleValueForDiffMode(45, 30, 25, 20), 4);
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

                        if (Timer > 9 * 4 + 20)
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

        public void BelowSparkleThenBite()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0://逐渐消失，然后瞬移到玩家下方
                    {
                        Phase2Fade(() => Target.Center + new Vector2(0,350),
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

                        Color color = Main.rand.Next(0, 2) switch
                        {
                            0 => new Color(110, 68, 200),
                            _ => new Color(122, 110, 134)
                        };

                        Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(32, 32), Helper.NextVec2Dir() * Main.rand.NextFloat(2, 10f),
                            CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1f));

                        float rotFactor = Math.Clamp(Timer / ((float)RollingTime / 3), 0, 1);
                        NPC.rotation += rotFactor * 0.35f;

                        Vector2 center = NPC.Center;

                        if (Timer < RollingTime)//追踪玩家底部的位置，并左右摇晃
                        {
                            Vector2 center2 = Target.Center + new Vector2(620 * MathF.Sin(24 * Timer / (float)RollingTime), 460);
                            Vector2 dir3 = center2 - center;

                            float velRot = NPC.velocity.ToRotation();
                            float targetRot2 = dir3.ToRotation();

                            float speed = NPC.velocity.Length();
                            float aimSpeed = Math.Clamp(dir3.Length() / 200f, 0, 1) * 66;

                            NPC.velocity = velRot.AngleTowards(targetRot2, 0.52f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);

                            if (Timer % 20 == 0)
                            {
                                int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);

                                NPC.NewProjectileInAI<NightmareSparkle_Normal>(center, -Vector2.UnitY, damage, 0);
                            }
                        }
                        else
                            NPC.velocity *= 0.95f;

                        int delay = 35 - (int)(rotFactor * 20);
                        if (Timer % delay == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 dir = (NPC.rotation + i * MathHelper.TwoPi/3).ToRotationVector2();
                                NPC.NewProjectileInAI<NightmareSparkle_Normal>(center, dir, damage, 0);
                            }

                            SoundStyle st = CoraliteSoundID.CrystalSerpent_Item109;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, center);
                        }

                        float factor2 = Timer / (float)RollingTime;

                        for (int i = 0; i < 3; i++)
                        {
                            RotateTentacle tentacle = rotateTentacles[i];
                            float targetRot = factor2 * MathHelper.TwoPi * 10 + i * MathHelper.TwoPi / 3;
                            Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                                center + 170 * targetRot.ToRotationVector2(), 0.2f);
                            tentacle.SetValue(selfPos, center, targetRot);
                            tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
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
                                    Dust dust = Dust.NewDustPerfect(center + dir2 * Main.rand.Next(0, 64), DustType<NightmareStar>(),
                                        dir2 * Main.rand.NextFloat(2f, 6f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
                                    dust.rotation = dir2.ToRotation() + MathHelper.PiOver2;
                                }

                                SoundStyle st = CoraliteSoundID.NoUse_SuperMagicShoot_Item68;
                                st.Pitch = -1;
                                SoundEngine.PlaySound(st, center);
                            }
                        }

                        if (Timer > RollingTime)
                        {
                            SonState++;
                            alpha = 1;
                            Timer = 0;
                            canDrawWarp = false;

                            int direction = Target.direction;
                            NPC.Center = Target.Center + new Vector2(direction * Main.rand.Next(800, 1000), 200);
                            NPC.velocity = (direction == 1 ? -0.785f * 3 : -0.785f).ToRotationVector2() * 48;   //根据玩家面朝方向决定咬的方向，为斜向上
                            NPC.rotation = NPC.velocity.ToRotation();

                            for (int i = 0; i < 3; i++)
                            {
                                RotateTentacle tentacle = rotateTentacles[i];
                                tentacle.pos = tentacle.targetPos = NPC.Center;
                                tentacle.rotation = NPC.rotation;
                            }

                            //生成裂缝的拖尾弹幕
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 10, ai2: -1);
                            NPC.NewProjectileInAI<NightmareSlit>(NPC.Center, Vector2.Zero, Helper.ScaleValueForDiffMode(45, 30, 25, 20), 4);
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

                        if (Timer > 9 * 4 + 20)
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

                        if (Timer % 20 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            NPC.NewProjectileInAI<BlackHole>(NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)),
                                 damage, 0, NPC.target, Timer + Main.rand.Next(0, 360), 200);
                        }

                        if (Timer % 30 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);

                            NPC.NewProjectileInAI<NightmareSparkle_Normal>(NPC.Center, NPC.rotation.ToRotationVector2(), damage, 0);

                            SoundStyle st = CoraliteSoundID.CrystalSerpent_Item109;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 110)
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

                        if (Timer > 30 && Timer % 30 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);

                            NPC.NewProjectileInAI<NightmareSparkle_Normal>(NPC.Center, NPC.rotation.ToRotationVector2(), damage, 0);

                            SoundStyle st = CoraliteSoundID.CrystalSerpent_Item109;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 280)
                            SetPhase2States();
                    }
                    break;
            }

            Timer++;
        }

        public void BatsAndCrows()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0://瞬移到玩家面前
                    {
                        NormallySetTentacle();
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(600, 800);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(600, 800);
                        }, () =>
                        {
                            SonState++;
                        }, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;
                            NPC.rotation = (pos - NPC.Center).ToRotation();
                            ShootCount = (NPC.Center - pos).ToRotation();
                        });
                    }
                    break;
                case 1:
                    {
                        NormallySetTentacle();
                        CircleMovement(650, 56, 0.7f, 550, 0.34f, ShootCount);
                        DoRotation(0.3f);
                        if (Timer>30)
                        {
                            Vector2 pos = Target.Center;
                            ShootCount = (NPC.Center - pos).ToRotation();
                            SonState++;
                            Timer = 0;
                        }
                    }

                    break;
                case 2://在玩家身边绕圈，并向左右方向射出蝙蝠，同时瞄准玩家射出蝙蝠，偶尔射出绕自身旋转的乌鸦弹幕
                    {
                        NormallySetTentacle();
                        CircleMovement(650, 56, 0.7f, 550, 0.34f, ShootCount);
                        DoRotation(0.3f);

                        if (Timer % 10 == 0)
                        {
                            float targetRot = (Target.Center - NPC.Center).ToRotation();
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = -1; i < 2; i += 2)
                            {
                                NPC.NewProjectileInAI<NightmareBat>(GetPhase1MousePos(), (targetRot + i * 0.6f + Main.rand.NextFloat(-0.04f, 0.04f)).ToRotationVector2() * 18
                                    , damage, 1, -1, -1, 0);
                            }

                            SoundStyle st = CoraliteSoundID.Fairy_NPCHit5;

                            st.Pitch += 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 20 && Timer % 20 == 0)//生成瞄准玩家射出的蝙蝠弹幕
                        {
                            float targetRot = (Target.Center - NPC.Center).ToRotation()+Main.rand.NextFloat(-0.55f, 0.55f);
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            NPC.NewProjectileInAI<NightmareBat>(GetPhase1MousePos(), targetRot.ToRotationVector2() * 10
                                , damage, 1, -1, -1, 0);
                        }

                        if (Timer > 40 && Timer % 28 == 0)//生成绕圈圈的渡鸦弹幕
                        {
                            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = 0; i < 3; i++)
                            {
                                NPC.NewProjectileInAI<NightmareCrow>(GetPhase1MousePos(), (rot + i * MathHelper.TwoPi / 3).ToRotationVector2() * 11
                                    , damage, 1, -1, -2, 0, Main.rand.NextFromList(-1, 1) * 0.017f);
                            }

                            SoundStyle st = CoraliteSoundID.BloodThron_Item113;
                            st.Pitch += 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        const int RollingTime = 320;
                        const int fadeTime = 45;

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
                            NPC.Center = Target.Center + new Vector2(direction * Main.rand.Next(680, 820), Main.rand.Next(-200, 200));
                            NPC.rotation = MathHelper.PiOver2;

                            for (int i = 0; i < 3; i++)
                            {
                                RotateTentacle tentacle = rotateTentacles[i];
                                tentacle.pos = tentacle.targetPos = NPC.Center;
                                tentacle.rotation = NPC.rotation;
                            }

                            ShootCount = (NPC.Center - Target.Center).ToRotation();

                            //生成一圈蝙蝠
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = 0; i < 7; i++)
                            {
                                NPC.NewProjectileInAI<NightmareBat>(GetPhase1MousePos(), (NPC.rotation + i * MathHelper.TwoPi / 7).ToRotationVector2() * 10
                                    , damage, 1, -1, -2, 1, 0.015f);
                            }

                            SoundStyle st = CoraliteSoundID.BloodThron_Item113;
                            st.Pitch += 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }
                    }
                    break;
                case 3://瞬移到玩家另外一边并射出旋转的蝙蝠弹幕
                    {
                        Vector2 center = NPC.Center;
                        const float RollingTime = 120f;
                        float factor2 = Timer / RollingTime;

                        float rotFactor = Math.Clamp(1 - Timer / RollingTime, 0, 1);
                        NPC.rotation += rotFactor * 0.35f;

                        CircleMovement(640, 40, 0.4f, 720, 0.14f, ShootCount);

                        Color color = Main.rand.Next(0, 2) switch
                        {
                            0 => new Color(110, 68, 200),
                            _ => new Color(122, 110, 134)
                        };

                        Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(32, 32), Helper.NextVec2Dir() * Main.rand.NextFloat(2, 10f),
                            CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1f));

                        for (int i = 0; i < 3; i++)
                        {
                            RotateTentacle tentacle = rotateTentacles[i];
                            float targetRot = factor2 * MathHelper.TwoPi * 3 + i * MathHelper.TwoPi / 3;
                            Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                                center + 170 * targetRot.ToRotationVector2(), 0.2f);
                            tentacle.SetValue(selfPos, NPC.Center, targetRot);
                            tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
                        }

                        if (Timer % 30 == 0)
                        {
                            //生成一圈蝙蝠
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = 0; i < 7; i++)
                            {
                                NPC.NewProjectileInAI<NightmareBat>(GetPhase1MousePos(), (NPC.rotation + i * MathHelper.TwoPi / 7).ToRotationVector2() * 10
                                    , damage, 1, -1, -2, 0, (Timer % 60 == 0 ? 1 : -1) * 0.015f);
                            }

                            SoundStyle st = CoraliteSoundID.BloodThron_Item113;
                            st.Pitch += 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 20 && Timer % 55 == 0)
                        {
                            //生成一圈渡鸦
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = 0; i < 7; i++)
                            {
                                NPC.NewProjectileInAI<NightmareCrow>(GetPhase1MousePos(), (NPC.rotation + i * MathHelper.TwoPi / 7).ToRotationVector2() * 12
                                    , damage, 1, -1, -1, 0, -1 * 0.0175f);
                            }

                            SoundStyle st = CoraliteSoundID.BloodThron_Item113;
                            st.Pitch += 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > RollingTime)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 4:
                    {
                        DoRotation(0.3f);
                        CircleMovement(100, 30);
                        NormallySetTentacle();

                        if (Timer > 40)
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
                    SetPhase2States();
                    break;
                case 0://瞬移到玩家面前
                    {
                        Phase2Fade(() => Target.Center + (SonState - 1) * MathHelper.Pi.ToRotationVector2() * Main.rand.NextFloat(500, 600),
                            () =>
                            {
                                SonState = Main.rand.Next(1, 3);
                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                                ShootCount = Main.rand.Next(4, 7);
                            });
                    }
                    break;
                case 1://放出触手并持续追踪玩家
                case 2:
                    {
                        HookSlashMovement();
                        if (Timer < ShootCount * 30 && Timer % 30 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(40, 30, 25, 25);
                            float angle2 = (NPC.Center - Target.Center).ToRotation() + (Timer % 60 == 0 ? MathHelper.PiOver4 : -MathHelper.PiOver4) + Main.rand.NextFloat(-0.25f, 0.25f);
                            NPC.NewProjectileInAI<HookSlash>(NPC.Center + (NPC.Center - Target.Center).SafeNormalize(Vector2.One) * 64, Vector2.Zero, damage,
                                0, NPC.target, ai1: angle2, ai2: 160);
                        }

                        if (Timer < ShootCount * 30 + 60)
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
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = 0; i < 7; i++)
                            {
                                Vector2 dir = (NPC.rotation + i * 1 / 7f * MathHelper.TwoPi).ToRotationVector2();

                                NPC.NewProjectileInAI<NightmareSparkle_Normal>(NPC.Center, dir, damage, 0);
                            }
                        }

                        if (Timer > 90)
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
                    SetPhase2States();
                    break;
                case 0:   //瞬移到玩家某一侧，瞬移之后在另一侧生成一堆尖刺
                    {
                        Phase2Fade(() => Target.Center + ((SonState - 1) * MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat(700, 800),
                            () =>
                            {
                                SonState = Main.rand.Next(1, 3);
                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                                ShootCount = Main.rand.Next(3, 5);

                                float rot = (SonState - 1) * MathHelper.Pi + MathHelper.Pi;
                                int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);

                                for (int i = -4; i < 4; i++)
                                {
                                    Vector2 dir = (rot + i * 0.4f).ToRotationVector2();

                                    NPC.NewProjectileInAI<ConfusionHole>(Target.Center + dir * Main.rand.NextFloat(500, 800) + Target.velocity * 15,
                                        -dir, damage, 0, NPC.target, 120, -1, Main.rand.Next(800, 1200));
                                }
                            });
                    }
                    break;
                case 1:  //不断向玩家吐噩梦光并在另一侧不断生成尖刺
                case 2:
                    {
                        DoRotation(0.3f);

                        float angle = (SonState - 1) * MathHelper.Pi + MathHelper.PiOver4 / 2 * MathF.Sin(Timer * 0.0314f);
                        Vector2 center = Target.Center + angle.ToRotationVector2() * 850;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 400f, 0, 1) * 30;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);

                        if (Timer % 12 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            Vector2 dir2 = NPC.rotation.ToRotationVector2();
                            Vector2 furture = Target.velocity * Target.velocity.Length() * 3f;

                            Vector2 center2 = Target.Center + dir2.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(500, 800) + furture;
                            NPC.NewProjectileInAI<ConfusionHole>(center2, (Target.Center + furture - center2).SafeNormalize(Vector2.Zero),
                                damage, 0, NPC.target, Main.rand.Next(60, 80), -1, Main.rand.Next(600, 900));

                            center2 = Target.Center + ((Timer % 24 == 0 ? 0 : MathHelper.Pi) + Main.rand.NextFloat(MathHelper.PiOver2 - 0.3f, MathHelper.PiOver2 + 0.3f)).ToRotationVector2() * Main.rand.Next(500, 700) + furture;
                            NPC.NewProjectileInAI<ConfusionHole>(center2, (Target.Center + furture - center2).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)),
                                damage, 0, NPC.target, 75, -1, Main.rand.Next(800, 1200));
                        }

                        if (Timer % 28 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);

                            NPC.NewProjectileInAI<NightmareSparkle_Normal>(NPC.Center, NPC.rotation.ToRotationVector2(), damage, 0);
                        }

                        if (Timer > 300)
                        {
                            Vector2 pos = Target.Center;
                            ShootCount = (NPC.Center - pos).ToRotation();
                            SonState = 3;
                            Timer = 0;
                        }
                    }
                    break;
                case 3:  //后摇
                    {
                        CircleMovement(650, 56, 0.7f, 550, 0.34f, ShootCount);
                        DoRotation(0.3f);
                        if (Timer > 30)
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
                    SetPhase2States();
                    break;
                case 0://瞬移到玩家面前
                    {
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            SonState++;
                            Timer = 0;
                        }, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;

                            NPC.rotation = (pos - NPC.Center).ToRotation();
                        });
                    }
                    break;
                case 1://放出5个刺
                    {
                        NormallySetTentacle();
                        CircleMovement(650, 56, 0.7f, 550, 0.34f, ShootCount);
                        DoRotation(0.3f);

                        if (Timer > 9 && Timer < (30 * 7 + 10) && (Timer - 10) % 30 == 0)
                        {
                            float startAngle = ((Timer - 10) / 30) * (MathHelper.TwoPi / 15);
                            int damage = Helper.ScaleValueForDiffMode(35, 20, 5, 5);
                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 center = Target.Center + startAngle.ToRotationVector2() * 450;
                                NPC.NewProjectileInAI<ConfusionHole>(center, (Target.Center - center).SafeNormalize(Vector2.One), damage, 0, ai0: 60,ai1:-1,ai2:440);
                                startAngle += MathHelper.TwoPi / 5;
                            }
                        }

                        if (Timer > 30 * 7 + 55)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://瞬移到玩家左侧
                    {
                        Phase2Fade(() => Target.Center + (SonState % 2 * MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat(700, 800),
                            () =>
                            {
                                SonState++;
                                ShootCount = SonState % 2 == 0 ? -1 : 1;
                                NPC.rotation = (SonState % 2 * MathHelper.Pi) + MathHelper.PiOver2;
                                NPC.velocity = NPC.rotation.ToRotationVector2();
                            });
                    }
                    break;
                case 3://在玩家周围旋转并放出尖刺
                case 4:
                case 5:
                    {
                        float baseRot = (SonState % 2) * MathHelper.Pi;

                        const int RollingTime = 120;
                        if (Timer < RollingTime)
                        {
                            float currentRot = baseRot + ShootCount * Timer / (float)RollingTime * MathHelper.TwoPi;

                            Vector2 center = Target.Center + currentRot.ToRotationVector2() * 800;
                            Vector2 dir = center - NPC.Center;

                            float velRot = NPC.velocity.ToRotation();
                            float targetRot = dir.ToRotation();

                            float speed = NPC.velocity.Length();
                            float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 56;

                            NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                            NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.3f);

                            if (Timer % 5 == 0)
                            {
                                int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);

                                NPC.NewProjectileInAI<ConfusionHole>(NPC.Center, (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero), damage, 0, NPC.target, 45, -1, 1300);
                            }
                            break;
                        }

                        const float fadeTime = 30;
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

                        if (Timer == RollingTime + fadeTime * 3 / 4)
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

                        if (Timer > RollingTime + fadeTime)
                        {
                            SonState++;
                            canDrawWarp = false;
                            alpha = 1;
                            Timer = 0;

                            ShootCount = SonState % 2 == 0 ? -1 : 1;
                            float angle = SonState % 2 * MathHelper.Pi;
                            NPC.Center = Target.Center + angle.ToRotationVector2() * Main.rand.NextFloat(700, 800);
                            NPC.rotation = angle + ShootCount * MathHelper.PiOver2;
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
                case 6://后摇
                    {
                        SetPhase2States();
                    }
                    break;
            }

            Timer++;
        }

        public void GhostDash()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0://瞬移
                    {
                        Phase2Fade(() =>
                        {
                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return Target.Center + new Vector2(Target.direction, 0) * Main.rand.NextFloat(750, 900);
                            else
                                return Target.Center + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(750, 900);
                        },
                            () =>
                            {
                                SonState++;
                                float targetRot = (Target.Center - NPC.Center).ToRotation() + MathHelper.PiOver4;
                                NPC.velocity = targetRot.ToRotationVector2() * 36;
                                NPC.rotation = targetRot;

                            }, PostTeleport: () =>
                            {
                                int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                                ShootCount = NPC.NewProjectileInAI<GhostSlit>(NPC.Center, Vector2.Zero, damage, 0, NPC.target, ai1: -1);
                            });
                    }
                    break;
                case 1://锯齿形冲刺
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    {
                        if (Timer > 8)
                        {
                            NPC.velocity *= 0.94f;
                            DoRotation(0.3f);
                        }

                        if (Timer > 17)
                        {
                            SonState++;
                            Timer = 0;

                            SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, NPC.Center);
                            if (Main.projectile[(int)ShootCount].active && Main.projectile[(int)ShootCount].type == ProjectileType<GhostSlit>())
                            {
                                (Main.projectile[(int)ShootCount].ModProjectile as GhostSlit).StopTracking();
                            }

                            if (SonState < 8)
                            {
                                float targetRot = (Target.Center - NPC.Center).ToRotation() + (SonState % 2 == 0 ? -1 : 1) * 0.45f;
                                NPC.velocity = targetRot.ToRotationVector2() * 36;
                                NPC.rotation = targetRot;

                                int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                                ShootCount = NPC.NewProjectileInAI<GhostSlit>(NPC.Center, Vector2.Zero, damage, 0, NPC.target, ai1: -1);
                            }
                        }
                    }
                    break;
                case 9://后摇并让拖尾爆开生成鬼手
                    {
                        NPC.velocity *= 0.93f;
                        DoRotation(0.3f);

                        if (Timer == 30)
                        {
                            GhostSlit.Exposion();
                        }

                        CircleMovement(540, 16, 0.25f, 360);

                        if (Timer > 110)
                        {
                            SetPhase2States();
                        }
                    }
                    break;
            }

            Timer++;
        }

        public void TeleportSparkle()
        {
            switch ((int)SonState)
            {
                default:
                case 0://瞬移到玩家面前
                case 4:
                case 6:
                    {
                        Phase2Fade(() =>
                        {
                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return Target.Center + new Vector2(Target.direction, 0) * Main.rand.NextFloat(150, 200);
                            else
                                return Target.Center + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(150, 200);
                        }, () =>
                        {
                            SonState++;
                        }, 30,PostTeleport: () =>
                        {
                            NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                            NPC.velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2();
                            ShootCount = NPC.rotation + MathHelper.Pi;
                        });
                    }
                    break;
                case 1://射几个弹幕
                case 5:
                    {
                        DoRotation(0.3f);

                        Vector2 center = Target.Center + ShootCount.ToRotationVector2() * 650;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 400f, 0, 1) * 30;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);

                        if (Timer > 15 && Timer % 15 == 0)
                        {
                            float howmany = 3f * ((Timer - 25) / 15f) / 2f;
                            for (float i = -(int)howmany; i < howmany; i++)
                            {
                                int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                                NPC.NewProjectileInAI<NightmareSparkle_Normal>(NPC.Center, (NPC.rotation + i * 0.25f).ToRotationVector2(), damage, 0);
                            }

                            SoundStyle st = CoraliteSoundID.CrystalSerpent_Item109;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 65)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://瞬移
                    {
                        Phase2Fade(() =>
                        {
                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return Target.Center + new Vector2(Target.direction, 0) * Main.rand.NextFloat(400, 600);
                            else
                                return Target.Center + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(400, 600);
                        }, () =>
                        {
                            SonState++;
                        }, 30, PostTeleport: () =>
                        {
                            NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                            ShootCount = (NPC.Center - Target.Center).ToRotation();
                            NPC.velocity = (ShootCount + MathHelper.PiOver2).ToRotationVector2();
                        });
                    }
                    break;
                case 3://自身转圈圈并射出尖刺
                    {
                        NPC.rotation += 0.55f;

                        const int RollingTime = 240;
                        float currentRot = ShootCount + Timer / (float)RollingTime * 2 * MathHelper.TwoPi;

                        Vector2 center = Target.Center + currentRot.ToRotationVector2() * 700;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 56;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.35f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);

                        if (Timer % 6 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            Vector2 dir2 = (Target.Center - NPC.Center).SafeNormalize(Vector2.One);
                            NPC.NewProjectileInAI<ConfusionHole>(NPC.Center, dir2.RotatedBy(MathF.Cos((Timer/6) * (MathHelper.TwoPi/12)) * 1.6f), damage, 0, NPC.target, 30, -1, 1300);
                        }

                        if (Timer > RollingTime)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 7://后摇
                    {
                        float factor = Timer / 25f;
                        Vector2 center = Target.Center + (ShootCount + factor * MathHelper.PiOver4).ToRotationVector2() * Helper.Lerp(150, 450, factor);
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 400f, 0, 1) * 30;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.3f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);
                        DoRotation(0.3f);
                        if (Timer > 25)
                            SetPhase2States();
                    }
                    break;
            }

            Timer++;
        }

        public void DreamSparkle()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2States();
                    break;
                case 0://瞬移到玩家面前
                    {
                        Phase2Fade(() =>
                        {
                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return Target.Center + new Vector2(Target.direction, 0) * Main.rand.NextFloat(150, 200);
                            else
                                return Target.Center + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(150, 200);
                        }, () =>
                        {
                            SonState++;
                        }, PostTeleport: () =>
                        {
                            NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                            NPC.velocity = (NPC.rotation + MathHelper.Pi).ToRotationVector2();
                            ShootCount = NPC.rotation + MathHelper.Pi;
                        });
                    }
                    break;
                case 1://自身围绕玩家旋转 每隔一段时间在玩家周围生成一圈的噩梦光弹幕
                    {
                        float currentRot = ShootCount + Timer / 400f * MathHelper.TwoPi;

                        Vector2 center = Target.Center + currentRot.ToRotationVector2() * 500;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 450f, 0, 1) * 34;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.3f);

                        if (Timer == 10 || Timer == 160)
                        {
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            int rollingTime = Main.rand.Next(60, 80);
                            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int i = 0; i < 14; i++)//生成噩梦光弹幕
                            {
                                Vector2 pos = Target.Center + (i * MathHelper.Pi / 14 + rot).ToRotationVector2() * 150;
                                NPC.NewProjectileInAI<NightmareSparkle_Rolling>(pos, Vector2.Zero, damage, 0, NPC.target, rollingTime + i * 6, ai2: 550);
                            }
                            SoundStyle st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            st.Volume -= 0.2f;
                            st.Pitch = -1;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 210)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2: //射出会变成美梦光的弹幕
                    {
                        float currentRot = ShootCount + Timer / 400f * MathHelper.TwoPi;

                        Vector2 center = Target.Center + currentRot.ToRotationVector2() * 500;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 450f, 0, 1) * 34;

                        if (Timer == 80)
                        {
                            int whichToExchange = Main.rand.Next(14);
                            bool exchanged = false;
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            int rollingTime = Main.rand.Next(150, 180);
                            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int i = 0; i < 14; i++)//生成噩梦光弹幕
                            {

                                Vector2 pos = Target.Center + (i * MathHelper.Pi / 14 + rot).ToRotationVector2() * 150;
                                float ai1 = 0;
                                if (!exchanged && i == whichToExchange)
                                {
                                    exchanged = true;
                                    ai1 = 1;
                                }
                                NPC.NewProjectileInAI<NightmareSparkle_Rolling>(pos, Vector2.Zero, damage, 0, NPC.target, rollingTime + i * 6, ai1, 550);
                            }
                            SoundStyle st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            st.Volume -= 0.2f;
                            st.Pitch = -1;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer < 140 && Timer % 20 == 0)
                        {
                            float angle = (NPC.Center - Target.Center).ToRotation();
                            int damage = Helper.ScaleValueForDiffMode(20, 10, 5, 5);
                            for (int i = 0; i < 4; i++)
                            {
                                NPC.NewProjectileDirectInAI<ConfusionHole>(Target.Center + (angle + i * MathHelper.PiOver2).ToRotationVector2() * 600,
                                    (angle + i * MathHelper.PiOver2 + MathHelper.Pi + Timer / 28 * 0.1f).ToRotationVector2(), damage, 0, NPC.target, 45, -1, 700);
                            }
                        }

                        if (Timer > 200)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
            }

            Timer++;
        }

        public void P2_Idle()
        {
            if (alpha > 0.75f)
                alpha -= 0.02f;

            NPC.dontTakeDamage = true;
            float currentRot = ShootCount + Timer / 400f * MathHelper.TwoPi;

            Vector2 center = Target.Center + currentRot.ToRotationVector2() * 500;
            Vector2 dir = center - NPC.Center;

            float velRot = NPC.velocity.ToRotation();
            float targetRot = dir.ToRotation();

            float speed = NPC.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 450f, 0, 1) * 34;

            NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
            NPC.rotation +=Main.rand.NextFloat(-0.2f,0.3f);

            if (Timer > ShootCount)
            {
                SetPhase2States();
            }

            Timer++;
        }

        #endregion

        #region DreamFightAI

        public void DreamingNightmareBite()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2DreamingStates();
                    break;
                case 0:
                    {
                        alpha = 0.6f + MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.35f;

                        NPC.dontTakeDamage = true;
                        float currentRot = ShootCount + Timer / 400f * MathHelper.TwoPi;

                        Vector2 pos = Target.Center;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        Vector2 center = pos + currentRot.ToRotationVector2() * 350;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 450f, 0, 1) * 34;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.3f);

                        if (Timer > ShootCount)
                        {
                            Timer = 0;
                            SonState++;
                            alpha = 1;
                        }
                    }
                    break;
                case 1: //逐渐消失
                    {
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(80, 70, 60, 60);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 60, ai2: -2);
                            //SoundStyle st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            //st.Volume -= 0.2f;
                            //st.Pitch = -1;
                            //SoundEngine.PlaySound(st, NPC.Center);
                        }, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            NPC.rotation = (pos - NPC.Center).ToRotation();
                        });
                    }
                    break;
                case 2:
                    {
                        Vector2 pos = Target.Center;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        NPC.rotation = NPC.rotation.AngleTowards((pos - NPC.Center).ToRotation(), 0.5f);
                        if (Vector2.Distance(NPC.Center, pos) > 200)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.85f;
                            if (speed > 26)
                                speed = 26;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                        {
                            NPC.velocity *= 0.78f;
                        }

                        if (Timer > 55)
                        {
                            useMeleeDamage = false;
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 3: //咬完了之后的后摇阶段
                    {
                        NPC.velocity *= 0.9f;

                        if (Timer == 7)
                        {
                            SoundStyle st = CoraliteSoundID.BottleExplosion_Item107;
                            st.Pitch = 1f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 10)
                        {
                            DoRotation(0.04f);
                        }

                        if (Timer > 20)
                        {
                            SetPhase2DreamingStates();
                        }
                    }
                    break;
            }

            Timer++;
        }

        public void Dreaming_Idle()
        {
            NPC.velocity *= 0.8f;
            //NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.3f);

            if (FantasyGodAlive(out NPC fg))
            {
                //NPC.rotation = NPC.rotation.AngleTowards((fg.Center - NPC.Center).ToRotation(), 0.3f);

                if (Timer > 3600)
                {
                    SetPhase2States();
                }

                Timer++;
            }
            else
                SetPhase2States();
        }

        public void DreamingSpikeHell()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2DreamingStates();
                    break;
                case 0://瞬移到美梦光面前
                    {
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                        }, 30, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;
                            if (FantasySparkleAlive(out NPC fs))
                                pos = fs.Center;

                            NPC.rotation = (pos - NPC.Center).ToRotation();
                        });
                    }
                    break;
                case 1://追踪一会，之后在自身位置和美梦光位置召唤一堆的尖刺
                    {
                        Vector2 pos = Target.Center;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;
                        else
                        {
                            SonState++;
                            Timer = 0;
                        }

                        NPC.rotation = NPC.rotation.AngleTowards((pos - NPC.Center).ToRotation(), 0.5f);
                        if (Vector2.Distance(NPC.Center, pos) > 160)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.85f;
                            if (speed > 26)
                                speed = 26;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                            NPC.velocity *= 0.78f;

                        if (Timer > 75)
                        {
                            if (Timer % 20 == 0)
                            {
                                int damage = Helper.ScaleValueForDiffMode(20, 10, 15, 15);
                                NPC.NewProjectileInAI<ConfusionHole>(NPC.Center, (pos - NPC.Center).SafeNormalize(Vector2.Zero), damage, 0, NPC.target, 30, -2, 1300);
                            }
                        }

                        if (Timer > 500)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://后摇
                    {
                        NPC.rotation += 0.2f;

                        if (Timer % 7 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(50, 60, 60, 60);
                            NPC.NewProjectileInAI<ConfusionHole>(NPC.Center, (Timer * MathHelper.TwoPi / 49).ToRotationVector2(), damage, 0, NPC.target, 30, -2, 1300);
                        }

                        if (Timer > 50)
                            SetPhase2DreamingStates();
                    }
                    break;
            }

            Timer++;
        }

        public void DreamingFantasyHunting()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase2DreamingStates();
                    break;
                case 0://追踪美梦光一段时间
                    {
                        alpha = 0.6f + MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.35f;

                        NPC.dontTakeDamage = true;
                        float currentRot = ShootCount + Timer / 400f * MathHelper.TwoPi;

                        Vector2 pos = Target.Center;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        Vector2 center = pos + currentRot.ToRotationVector2() * 350;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 450f, 0, 1) * 34;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.3f);

                        if (Timer > ShootCount)
                        {
                            Timer = 0;
                            SonState++;
                            alpha = 1;
                            NPC.dontTakeDamage = false;
                        }
                    }
                    break;
                case 1://射出
                    {
                        Vector2 pos = Target.Center;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        NPC.rotation = NPC.rotation.AngleTowards((pos - NPC.Center).ToRotation(), 0.5f);
                        if (Vector2.Distance(NPC.Center, pos) > 360)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.85f;
                            if (speed > 26)
                                speed = 26;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                        {
                            NPC.velocity *= 0.78f;
                        }

                        if (Timer % 25 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int howmany = Main.rand.NextFromList(1, 3);
                            float baseRot = (pos - NPC.Center).ToRotation() - howmany / 2 * 0.15f;
                            int damage = Helper.ScaleValueForDiffMode(50, 50, 25, 25);
                            for (int i = 0; i < howmany; i++)
                            {
                                NPC.NewProjectileInAI<NightmareSparkle_Red>(NPC.Center, (baseRot + i * 0.15f).ToRotationVector2(), damage, 0);
                            }
                        }

                        if (Timer > 120)
                        {
                            SetPhase2DreamingStates();
                        }
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
        public void Phase2Fade(Func<Vector2> teleportPos, Action OnTeleport, int fadeTime = 45, Action PostTeleport = null)
        {
            if (Timer == 1)
                SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, NPC.Center);

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

                for (int i = 0; i < 8; i++)
                {
                    Color color = Main.rand.Next(0, 2) switch
                    {
                        0 => new Color(110, 68, 200),
                        _ => new Color(122, 110, 134)
                    };

                    Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(32, 32), Helper.NextVec2Dir() * Main.rand.NextFloat(6, 10f),
                        CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }

                for (int i = 0; i < 16; i++)
                {
                    Vector2 dir2 = Helper.NextVec2Dir();
                    Dust dust = Dust.NewDustPerfect(NPC.Center + dir2 * Main.rand.Next(0, 64), DustType<NightmareStar>(),
                        dir2 * Main.rand.NextFloat(4f, 10f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 4f));
                    dust.rotation = dir2.ToRotation() + MathHelper.PiOver2;

                    dir2 = Helper.NextVec2Dir();
                    Dust.NewDustPerfect(NPC.Center + dir2 * Main.rand.Next(0, 64), DustID.VilePowder,
                        dir2 * Main.rand.NextFloat(2f, 6f), newColor: new Color(153, 88, 156, 230), Scale: Main.rand.NextFloat(1f, 1.3f));
                }

                PostTeleport?.Invoke();

                for (int i = 0; i < 3; i++)
                {
                    RotateTentacle tentacle = rotateTentacles[i];
                    tentacle.pos = tentacle.targetPos = NPC.Center;
                    tentacle.rotation = NPC.rotation;
                }
            }
        }

        public void CircleMovement(float distance, float speedMax, float accelFactor = 0.25f, float rollingFactor = 360f,float angleFactor=0.08f,float baseRot=0f)
        {
            Vector2 center = Target.Center + (baseRot+Timer / rollingFactor * MathHelper.TwoPi).ToRotationVector2() * distance;
            Vector2 dir = center - NPC.Center;

            float velRot = NPC.velocity.ToRotation();
            float targetRot = dir.ToRotation();

            float speed = NPC.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 800f, 0, 1) * speedMax;

            NPC.velocity = velRot.AngleTowards(targetRot, angleFactor).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, accelFactor);
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

        public static bool FantasySparkleAlive(out NPC fs)
        {
            if (TargetFantasySparkle >= 0 && TargetFantasySparkle < 201 && Main.npc[TargetFantasySparkle].active && Main.npc[TargetFantasySparkle].type == NPCType<FantasySparkle>())
            {
                fs = Main.npc[TargetFantasySparkle];
                return true;
            }

            TargetFantasySparkle = -1;
            fs = null;
            return false;
        }

        public static bool FantasyGodAlive(out NPC fg)
        {
            if (FantasyGod >= 0 && FantasyGod < 201 && Main.npc[FantasyGod].active && Main.npc[FantasyGod].type == NPCType<FantasyGod>())
            {
                fg = Main.npc[FantasyGod];
                return true;
            }

            TargetFantasySparkle = -1;
            fg = null;
            return false;
        }

        #endregion

        #region States

        public void SetPhase2States()
        {
            NPC.dontTakeDamage = false;
            warpScale = 0;
            canDrawWarp = false;
            useMeleeDamage = false;
            useDreamMove = false;
            tentacleColor = lightPurple;
            DreamMoveCount = 0;
            fantasyKillCount = 0;
            tentacleStarFrame = 0;
            alpha = 1;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Timer = 0;
            SonState = 0;
            NPC.netUpdate = true;

            if (NPC.life < NPC.lifeMax / 5) //五分之一血量以下进入三阶段
            {
                OnExchangeToP3();
                return;
            }

            if (FantasySparkleAlive(out _))
            {
                State = (int)AIStates.nightmareBite;
                return;
            }

            State = (int)MoveCount switch
            {
                0 => Main.rand.Next(0, 2) switch
                {
                    0 => (int)AIStates.rollingThenBite,
                    _ => (int)AIStates.belowSparkleThenBite,
                },
                1 => RandomBite(),
                2 => Main.rand.Next(0, 3) switch
                {
                    0 => (int)AIStates.teleportSparkle,
                    1 => (int)AIStates.batsAndCrows,
                    _ => (int)AIStates.ghostDash,
                },
                3 => UseFantasyHelp(),
                4 => Main.rand.Next(0, 3) switch
                {
                    0 => (int)AIStates.spikeBalls,
                    1 => (int)AIStates.batsAndCrows,
                    _ => (int)AIStates.spikesAndSparkles,
                },
                5 => SpecialMove2((int)State),
                6 => RandomBite(),
                7 => (int)AIStates.hookSlash,
                8 => RandomBite(),
                9 => (int)AIStates.spikeHell,
                10 => (int)AIStates.dreamSparkle,
                _ => (int)AIStates.P2_Idle,
            };

            switch ((int)State)
            {
                default:
                    break;
                case (int)AIStates.P2_Idle:
                    ShootCount = 300;
                    break;
            }

            //State = (int)AIStates.dreamSparkle;

            MoveCount++;
            if (MoveCount > 11)
                MoveCount = 0;
        }

        public static int RandomBite()
        {
            return Main.rand.Next(0, 3) switch
            {
                0 => (int)AIStates.nightmareBite,
                1 => (int)AIStates.fakeBite,
                _ => (int)AIStates.nightmareDash,
            } ;
        }

        public int UseFantasyHelp()
        {
            if (useFantasyHelp)
            {
                useFantasyHelp = false;
                return (int)AIStates.fantasyHelp;
            }

            return RandomBite();
        }

        public static int SpecialMove2(int oldState)
        {
            List<int> list = new List<int>
            {
                (int)AIStates.spikeBalls,
                (int)AIStates.batsAndCrows,
                (int)AIStates.spikesAndSparkles
            };

            list.Remove(oldState);

            return Main.rand.NextFromList(list.ToArray());
        }

        /// <summary>
        /// 梦境阶段的战斗的重设状态
        /// </summary>
        public void SetPhase2DreamingStates()
        {
            NPC.dontTakeDamage = false;
            warpScale = 0;
            haveBeenPhase2 = true;
            canDrawWarp = false;
            useMeleeDamage = false;
            tentacleColor = lightPurple;
            alpha = 1;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Timer = 0;
            SonState = 0;
            MoveCount = 0;
            NPC.netUpdate = true;

            if (fantasyKillCount > 7)
                KillFantasySparkle();

            if (NPC.life < NPC.lifeMax / 5) //五分之一血量以下进入三阶段
            {
                OnExchangeToP3();
                return;
            }

            switch ((int)DreamMoveCount)
            {
                case 0://咬它
                case 2:
                case 4:
                    {
                        State = (int)AIStates.nightmareBite;
                        ShootCount = 60;
                        Vector2 center = Target.Center + new Vector2(Main.rand.NextFromList(-1, 1) * 500, -300);
                        TargetFantasySparkle = NPC.NewNPC(NPC.GetSource_FromAI(), (int)center.X, (int)center.Y, NPCType<FantasySparkle>());
                    }
                    break;
                case 1://尖刺
                case 3:
                case 5:
                    {
                        State = (int)AIStates.spikeHell;
                        Vector2 center = Target.Center + new Vector2(Main.rand.NextFromList(-1, 1) * 500, -300);
                        TargetFantasySparkle = NPC.NewNPC(NPC.GetSource_FromAI(), (int)center.X, (int)center.Y, NPCType<FantasySparkle>());
                    }
                    break;
                default:
                    {
                        ShootCount = 60;
                        State = (int)AIStates.fantasyHunting;
                        Vector2 center = Target.Center + new Vector2(Main.rand.NextFromList(-1, 1) * 500, -300);
                        TargetFantasySparkle = NPC.NewNPC(NPC.GetSource_FromAI(), (int)center.X, (int)center.Y, NPCType<FantasySparkle>());
                    }
                    break;
            }

            DreamMoveCount++;
        }

        public void Exchange2DreamingStates()
        {
            int howmany = 0;
            haveBeenPhase2 = true;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (!n.active || !n.friendly || n.type != NPCType<FantasySparkle>())
                    continue;

                howmany++;
            }

            if (howmany >= 7)
            {
                Vector2 pos = Vector2.Zero;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC n = Main.npc[i];
                    if (!n.active || !n.friendly || n.type != NPCType<FantasySparkle>())
                        continue;

                    n.ai[0] = -2;
                    pos += n.Center;
                }

                pos /= howmany;
                
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC n = Main.npc[i];
                    if (!n.active || !n.friendly || n.type != NPCType<FantasySparkle>())
                        continue;

                    n.ai[1] = pos.X;
                    n.ai[2] = pos.Y;
                }

                FantasyGod = NPC.NewNPC(NPC.GetSource_FromAI(), (int)pos.X, (int)pos.Y, NPCType<FantasyGod>());

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    CoralitePlayer cp = Main.LocalPlayer.GetModPlayer<CoralitePlayer>();
                    cp.nightmareCount = 0;
                    Main.LocalPlayer.ClearBuff(BuffType<DreamErosion>());
                }
                else
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        Player p = Main.player[i];
                        if (p.active && p.HasBuff<DreamErosion>())
                        {
                            CoralitePlayer cp = p.GetModPlayer<CoralitePlayer>();
                            cp.nightmareCount = 0;
                            p.ClearBuff(BuffType<DreamErosion>());
                        }
                    }

                State = (int)AIStates.P2_Idle;
                NPC.dontTakeDamage = false;
                warpScale = 0;
                canDrawWarp = false;
                useMeleeDamage = false;
                tentacleColor = lightPurple;
                alpha = 1;
                Timer = 0;
                SonState = 0;
                MoveCount = 0;
                NPC.netUpdate = true;

                return;
            }

            if (!useDreamMove)
            {
                DreamMoveCount = 0;
                useDreamMove = true;
            }

            SetPhase2DreamingStates();
        }

        public void KillFantasySparkle()
        {
            if (FantasyGodAlive(out _))
                return;

            fantasyKillCount++;

            if (fantasyKillCount > 7)
            {
                NPC.dontTakeDamage = false;
                warpScale = 0;
                canDrawWarp = false;
                useMeleeDamage = false;
                tentacleColor = lightPurple;
                alpha = 1;
                Timer = 0;
                fantasyKillCount = 0;
                useDreamMove = false;
                SonState = 0;

                State = (int)AIStates.blackHole;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC n = Main.npc[i];
                    if (!n.active || !n.friendly || n.type != NPCType<FantasySparkle>())
                        continue;

                    n.Kill();
                }

                NPC.netUpdate = true;
            }
        }

        #endregion
    }
}
