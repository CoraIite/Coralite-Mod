using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera
    {
        public void Nightmare_Phase3()
        {
            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = 100;

            //设置三条拖尾，如果玩家通过某些奇葩手段跳过了二阶段的话在3阶段设置这个
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

            switch ((int)State)
            {
                default:
                    SetPhase3States();
                    break;
                case (int)AIStates.exchange_P2_P3:
                    NormallySetTentacle();
                    Exchange_P2_P3();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.illusionBite:
                    NormallySetTentacle();
                    IllusionBite();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.tripleSpikeHell:
                    NormallySetTentacle();
                    TripleSpikeHell();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.flowerDance:
                    FlowerDance();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.superHookSlash:
                    NormallySetTentacle();
                    SuperHookSlash();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.P3_SpikesAndSparkles:
                    NormallySetTentacle();
                    P3_SpikesAndSparkles();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.P3_teleportSparkles:
                    P3_TeleportSparkles();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.P3_nightmareBite:
                    NormallySetTentacle();
                    P3_NightmareBite();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.P3_nightmareDash:
                    NormallySetTentacle();
                    P3_NightmareDash();
                    NormallyUpdateTentacle();
                    break;
                case (int)AIStates.vineSpurt:
                    NormallySetTentacle();
                    VineSpurt();
                    NormallyUpdateTentacle();
                    break;

            }
        }

        #region Phase3AI

        public void Exchange_P2_P3()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
                    break;
                case 0:
                    {
                        Color c = ((NightmareSky)SkyManager.Instance["NightmareSky"]).color;
                        ((NightmareSky)SkyManager.Instance["NightmareSky"]).color = Color.Lerp(c,nightmareRed,0.02f);
                        Phase3Fade(() => Target.Center + new Vector2(0, -250), () =>
                        {
                            canDrawWarp = true;
                            SonState++;
                        }, PostTeleport: () =>
                        {
                            NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                            SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            st.Pitch = -0.75f;
                            st.Volume -= 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY, 15, 8, 20, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        });
                    }
                    break;
                case 1://转圈圈并爆开
                    {
                        if (Timer % 10 == 0)
                        {
                            var modifyer = new PunchCameraModifier(NPC.Center, Helper.NextVec2Dir(), 15, 8, 10, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }

                        warpScale += 0.3f;
                        if (warpScale > 10)
                        {
                            canDrawWarp = false;
                            warpScale = 0;
                        }

                        NPC.rotation += 0.3f;
                        NPC.velocity *= 0.9f;

                        Vector2 dir = Helper.NextVec2Dir();
                        Dust dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), DustType<NightmareDust>(), dir * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;

                        if (Timer % 3 == 0)
                        {
                            dir = Helper.NextVec2Dir();
                            dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), DustType<NightmareStar>(), dir * Main.rand.NextFloat(4f, 8f), newColor: nightmareRed, Scale: Main.rand.NextFloat(1f, 4f));
                            dust.rotation = dir.ToRotation() + MathHelper.PiOver2;

                            dir = Helper.NextVec2Dir();
                            Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), DustID.VilePowder, dir * Main.rand.NextFloat(4f, 10f), newColor: nightmareRed, Scale: Main.rand.NextFloat(1f, 1.3f));
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            Color color = Main.rand.Next(0, 2) switch
                            {
                                0 => new Color(110, 68, 200),
                                _ => nightmareRed
                            };

                            Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir() * Main.rand.NextFloat(6, 24f),
                                CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        }

                        if (Timer % 8 == 0)
                        {
                            SoundStyle st = CoraliteSoundID.FireBallExplosion_Item74;
                            st.Volume -= 0.2f;
                            st.Pitch -= 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer > 80)
                            SetPhase3States();
                    }
                    break;
            }

            Timer++;
        }

        public void IllusionBite()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
                    break;
                case 0://自身隐形，之后召唤几个假幻象之咬，最后召唤一个真的
                case 1:
                case 2:
                    {
                        float currentRot =EXai1+ Timer / 400f * MathHelper.TwoPi;

                        Vector2 center = Target.Center + currentRot.ToRotationVector2() * 500;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 450f, 0, 1) * 34;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.3f);

                        if (Timer < (int)ShootCount && Timer % 8 == 0)
                        {
                            NPC.NewProjectileDirectInAI<NightmareBite>(Target.Center + Main.rand.NextVector2CircularEdge(400, 400), Vector2.Zero,
                                1, 0, NPC.target, 2, 40, -1);
                        }

                        if (Timer == (int)ShootCount)
                        {
                            Vector2 pos = Target.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                pos += new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                pos += Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);

                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileDirectInAI<NightmareBite>(pos + Main.rand.NextVector2CircularEdge(100, 100), Vector2.Zero,
                                damage, 0, NPC.target, 1, 50, -2);
                        }

                        if (Timer > ShootCount + 40)
                        {
                            SonState++;
                            EXai1 = (NPC.Center - Target.Center).ToRotation();
                            Timer = 0;
                            ShootCount = Main.rand.NextFromList(8 * 3, 8 * 4, 8 * 5, 8 * 6, 8 * 7);
                        }
                    }
                    break;
                case 3://自身瞬移到玩家身边
                    {
                        Phase3Fade(() =>
                        {
                            Vector2 pos = Target.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(450, 600);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(450, 600);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 60, ai2: -2);
                        }, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;
                            NPC.rotation = (pos - NPC.Center).ToRotation();
                        });
                    }
                    break;
                case 4://咬下
                    {
                        Vector2 pos = Target.Center;

                        if (Timer == 10 || Timer == 20)
                        {
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 60, ai2: -2);
                        }

                        NPC.rotation = NPC.rotation.AngleTowards((pos - NPC.Center).ToRotation(), 0.3f);
                        if (Vector2.Distance(NPC.Center, pos) > 220)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.8f;
                            if (speed > 30)
                                speed = 30;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                            NPC.velocity *= 0.8f;

                        if (Timer > 75)
                        {
                            useMeleeDamage = false;
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 5://后摇
                    {
                        NPC.velocity *= 0.9f;

                        if (Timer > 5)
                            DoRotation(0.04f);

                        if (Timer > 15)
                            SetPhase3States();
                    }
                    break;
            }

            Timer++;
        }

        public void TripleSpikeHell()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
                    break;
                case 0://瞬移到玩家左侧
                    {
                        Phase3Fade(() => Target.Center + (SonState % 2 * MathHelper.Pi).ToRotationVector2() * Main.rand.NextFloat(700, 800),
                            () =>
                            {
                                SonState++;
                                ShootCount = SonState % 2 == 0 ? -1 : 1;
                                NPC.rotation = (SonState % 2 * MathHelper.Pi) + MathHelper.PiOver2;
                                NPC.velocity = NPC.rotation.ToRotationVector2();
                                float rot = SonState % 2 * MathHelper.Pi;
                                int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                                for (int i = 1; i < 3; i++)
                                {
                                    float currentRot = rot + i * MathHelper.TwoPi / 3;
                                    NPC.NewProjectileInAI<IllusionSpikeHell>(Target.Center + currentRot.ToRotationVector2() * Main.rand.NextFloat(700, 800),
                                         (currentRot + ShootCount * MathHelper.PiOver2).ToRotationVector2(), damage, 0, NPC.target, currentRot, ShootCount);
                                }
                            });
                    }
                    break;
                case 1://在玩家周围旋转并放出尖刺
                case 2:
                case 3:
                    {
                        float baseRot = (SonState % 2) * MathHelper.Pi;

                        const int RollingTime = 120;
                        if (Timer < RollingTime)
                        {
                            float currentRot = baseRot + ShootCount * Timer / RollingTime * MathHelper.TwoPi;

                            Vector2 center = Target.Center + currentRot.ToRotationVector2() * 800;
                            Vector2 dir = center - NPC.Center;

                            float velRot = NPC.velocity.ToRotation();
                            float targetRot = dir.ToRotation();

                            float speed = NPC.velocity.Length();
                            float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 56;

                            NPC.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                            NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), 0.3f);

                            if (Timer % 7 == 0)
                            {
                                int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                                Vector2 dir2 = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                                NPC.NewProjectileInAI<ConfusionHole>(NPC.Center - dir2 * 200, dir2, damage, 0, NPC.target, 25, -2, 1100);
                            }
                            break;
                        }

                        const int fadeTime = 25;

                        if (Timer == (int)RollingTime)
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
                                    dir2 * Main.rand.NextFloat(2f, 6f), newColor: nightmareRed, Scale: Main.rand.NextFloat(1f, 4f));
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
                            float rot = SonState % 2 * MathHelper.Pi;
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            if (SonState < 4)
                                for (int i = 1; i < 3; i++)
                                {
                                    float currentRot = rot + i * MathHelper.TwoPi / 3;
                                    NPC.NewProjectileInAI<IllusionSpikeHell>(Target.Center + currentRot.ToRotationVector2() * Main.rand.NextFloat(700, 800),
                                        (currentRot + ShootCount * MathHelper.PiOver2).ToRotationVector2(), damage, 0, NPC.target, currentRot, ShootCount);
                                }


                            for (int i = 0; i < 3; i++)
                            {
                                RotateTentacle tentacle = rotateTentacles[i];
                                tentacle.pos = tentacle.targetPos = NPC.Center;
                                tentacle.rotation = NPC.rotation;
                            }
                        }
                    }
                    break;
            }

            Timer++;
        }

        public void FlowerDance()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
                    break;
                case 0:
                    {
                        NormallySetTentacle();
                        Phase3Fade(() => Target.Center + new Vector2(0, -400), () =>
                        {
                            canDrawWarp = true;
                            SonState++;
                        }, PostTeleport: () =>
                        {
                            NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                            SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
                            st.Pitch = -0.5f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            st.Pitch = -0.75f;
                            st.Volume -= 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY, 15, 8, 20, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        });
                    }
                    break;
                case 1://转圈圈并爆开，生成几圈黑暗叶弹幕
                    {
                        Vector2 center = NPC.Center;
                        float factor2 = Timer / (float)80;

                        for (int i = 0; i < 3; i++)
                        {
                            RotateTentacle tentacle = rotateTentacles[i];
                            float targetRot = factor2 * MathHelper.TwoPi * 10 + i * MathHelper.TwoPi / 3;
                            Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                                center + 170 * targetRot.ToRotationVector2(), 0.2f);
                            tentacle.SetValue(selfPos, NPC.Center, targetRot);
                            tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
                        }

                        if (Timer % 10 == 0)
                        {
                            var modifyer = new PunchCameraModifier(NPC.Center, Helper.NextVec2Dir(), 5, 8, 10, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);
                        }

                        warpScale += 0.3f;
                        if (warpScale > 10)
                        {
                            canDrawWarp = false;
                            warpScale = 0;
                        }

                        NPC.rotation += 0.3f;
                        NPC.velocity *= 0.5f;

                        Vector2 dir = Helper.NextVec2Dir();
                        Dust dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), DustType<NightmareDust>(), dir * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;

                        if (Timer %4 == 0)
                        {
                            dir = Helper.NextVec2Dir();
                            dust = Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), DustType<NightmareStar>(), dir * Main.rand.NextFloat(4f, 8f), newColor: nightmareRed, Scale: Main.rand.NextFloat(1f, 4f));
                            dust.rotation = dir.ToRotation() + MathHelper.PiOver2;

                            dir = Helper.NextVec2Dir();
                            Dust.NewDustPerfect(NPC.Center + dir * Main.rand.NextFloat(64f), DustID.VilePowder, dir * Main.rand.NextFloat(4f, 10f), newColor: nightmareRed, Scale: Main.rand.NextFloat(1f, 1.3f));
                        }

                        for (int i = 0; i < 2; i++)
                        {
                            Color color = Main.rand.Next(0, 2) switch
                            {
                                0 => new Color(110, 68, 200),
                                _ => nightmareRed
                            };

                            Particle.NewParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir() * Main.rand.NextFloat(6, 16f),
                                CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                        }

                        if (Timer % 8 == 0)
                        {
                            SoundStyle st = CoraliteSoundID.NoUse_BlowgunPlus_Item65;
                            st.Volume -= 0.2f;
                            st.Pitch -= 0.2f;
                            SoundEngine.PlaySound(st, NPC.Center);
                        }

                        if (Timer % 6 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            int color = 0;
                            if (ShootCount % 9 < 1)
                            {
                                color = 1;
                                if (Timer < 60)
                                    ShootCount += 3;
                                else
                                    ShootCount -= 3;
                            }

                            for (int i = 0; i < 7; i++)
                            {
                                NPC.NewProjectileInAI<DarkLeaf>(NPC.Center, (ShootCount * 0.14f + i * MathHelper.TwoPi / 7).ToRotationVector2() * 10, damage, 0, ai0: color);
                            }

                            if (Timer < 60)
                                ShootCount++;
                            else
                                ShootCount--;
                        }

                        if (Timer > 120)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;


            }

            Timer++;
        }

        public void SuperHookSlash()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
                    break;
                case 0://瞬移到玩家面前
                    {
                        Phase3Fade(() => Target.Center + (SonState - 1) * MathHelper.Pi.ToRotationVector2() * Main.rand.NextFloat(500, 600),
                            () =>
                            {
                                SonState = Main.rand.Next(1, 3);
                                NPC.rotation = (Target.Center - NPC.Center).ToRotation();
                                ShootCount = Main.rand.Next(3, 6);
                            });
                    }
                    break;
                case 1://放出触手并持续追踪玩家
                case 2:
                    {
                        HookSlashMovement();
                        if (Timer < ShootCount * 15 && Timer % 15 == 0)
                            for (int i = 0; i < 2; i++)
                            {
                                int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                                float angle2 = (NPC.Center - Target.Center).ToRotation() + (Timer % 30 == 0 ? MathHelper.PiOver4 / 2 : -MathHelper.PiOver4 / 2) + Main.rand.NextFloat(-0.15f, 0.15f);
                                NPC.NewProjectileInAI<HookSlash>(NPC.Center + (NPC.Center - Target.Center).SafeNormalize(Vector2.One) * 64, Vector2.Zero, damage,
                                    0, NPC.target, ai0: -1, ai1: angle2, ai2: 120);
                            }

                        if (Timer < ShootCount * 15 + 80)
                            break;

                        SonState++;
                        Timer = 0;
                        SoundStyle st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                        st.Pitch = -0.7f;
                        SoundEngine.PlaySound(st, NPC.Center);
                        canDrawWarp = true;
                    }
                    break;
                case 3://后摇
                    {
                        NPC.velocity *= 0.96f;
                        DoRotation(0.3f);

                        if (Timer > 90)
                            SetPhase3States();
                    }
                    break;
            }

            Timer++;
        }

        public void P3_SpikesAndSparkles()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
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
                                int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);

                                for (int i = -4; i < 4; i++)
                                {
                                    Vector2 dir = (rot + i * 0.4f).ToRotationVector2();

                                    NPC.NewProjectileInAI<ConfusionHole>(Target.Center + dir * Main.rand.NextFloat(500, 800) + Target.velocity * 15,
                                        -dir, damage, 0, NPC.target, 60, -2, Main.rand.Next(800, 1200));
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
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            Vector2 dir2 = NPC.rotation.ToRotationVector2();
                            Vector2 furture = Target.velocity * Target.velocity.Length() * 3f;

                            Vector2 center2 = Target.Center + dir2.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(500, 800) + furture;
                            NPC.NewProjectileInAI<ConfusionHole>(center2, (Target.Center + furture - center2).SafeNormalize(Vector2.Zero),
                                damage, 0, NPC.target, Main.rand.Next(60, 80), -2, Main.rand.Next(600, 900));

                            center2 = Target.Center + ((Timer % 24 == 0 ? 0 : MathHelper.Pi) + Main.rand.NextFloat(MathHelper.PiOver2 - 0.3f, MathHelper.PiOver2 + 0.3f)).ToRotationVector2() * Main.rand.Next(500, 700) + furture;
                            NPC.NewProjectileInAI<ConfusionHole>(center2, (Target.Center + furture - center2).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)),
                                damage, 0, NPC.target, 75, -2, Main.rand.Next(800, 1200));
                        }

                        if (Timer % 28 == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            int howmany = Main.rand.Next(1, 4);
                            float baseRot = NPC.rotation - (howmany - 1) * 0.35f;
                            for (int i = 0; i < howmany; i++)
                            {
                                NPC.NewProjectileInAI<NightmareSparkle_Red>(NPC.Center, (baseRot + i * 0.7f).ToRotationVector2(), damage, 0);
                            }
                        }

                        if (Timer > 300)
                        {
                            SonState = 3;
                            Timer = 0;
                        }
                    }
                    break;
                case 3:  //后摇
                    {
                        SetPhase3States();
                    }
                    break;
            }

            Timer++;
        }

        public void P3_TeleportSparkles()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
                    break;
                case 0://逐渐消失，然后瞬移到玩家面前
                    {
                        Phase3Fade(() => Target.Center + new Vector2(-Target.direction, 0) * Main.rand.NextFloat(300, 400),
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

                        float rotFactor = Math.Clamp(Timer / ((float)RollingTime / 3), 0, 1);
                        NPC.rotation += rotFactor * 0.35f;

                        int delay = 30 - (int)(rotFactor * 20);
                        if (Timer % delay == 0)
                        {
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 dir = (NPC.rotation + i * MathHelper.TwoPi / 5).ToRotationVector2();

                                NPC.NewProjectileInAI<NightmareSparkle_Red>(NPC.Center, dir, damage, 0);
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

                        if (Timer % 100 == 0)
                        {
                            NPC.velocity *= 0;
                            alpha = 1;
                            canDrawWarp = false;
                            warpScale = 0;
                            NPC.rotation = (Target.Center - NPC.Center).ToRotation();

                            NPC.Center = Target.Center + Main.rand.NextVector2CircularEdge(550, 550);
                            for (int i = 0; i < 16; i++)
                            {
                                Vector2 dir2 = Helper.NextVec2Dir();
                                Dust dust = Dust.NewDustPerfect(NPC.Center + dir2 * Main.rand.Next(0, 64), DustType<NightmareStar>(),
                                    dir2 * Main.rand.NextFloat(2f, 6f), newColor: nightmareRed, Scale: Main.rand.NextFloat(1f, 4f));
                                dust.rotation = dir2.ToRotation() + MathHelper.PiOver2;
                            }

                            SoundStyle st = CoraliteSoundID.NoUse_SuperMagicShoot_Item68;
                            st.Pitch = -1;
                            SoundEngine.PlaySound(st, NPC.Center);

                            for (int i = 0; i < 3; i++)
                            {
                                RotateTentacle tentacle = rotateTentacles[i];
                                tentacle.pos = tentacle.targetPos = NPC.Center;
                                tentacle.rotation = NPC.rotation;
                            }
                        }

                        if (Timer > RollingTime)
                        {
                            SonState++;
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
                        }
                    }
                    break;
                case 2://后摇
                    {
                        DoRotation(0.3f);
                        CircleMovement(340, 16, 0.25f, 180);

                        if (Timer >  30)
                        {
                            NPC.velocity *= 0;
                            SetPhase3States();
                        }

                        NormallySetTentacle();
                    }
                    break;
            }

            Timer++;
        }

        public void P3_NightmareBite()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
                    break;
                case 0: //逐渐消失
                    {
                        Phase3Fade(() =>
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
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 60, ai2: -2);
                            SoundStyle st = CoraliteSoundID.EmpressOfLight_Dash_Item160;
                            st.Volume -= 0.2f;
                            st.Pitch = -1;
                            SoundEngine.PlaySound(st, NPC.Center);
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

                        if (Timer == 10 || Timer == 20)
                        {
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 60, ai2: -2);
                        }

                        NPC.rotation = NPC.rotation.AngleTowards((pos - NPC.Center).ToRotation(), 0.3f);
                        if (Vector2.Distance(NPC.Center, pos) > 220)
                        {
                            float speed = NPC.velocity.Length();
                            speed += 0.8f;
                            if (speed > 30)
                                speed = 30;

                            float velRot = NPC.velocity.ToRotation();
                            NPC.velocity = velRot.AngleTowards(NPC.rotation, 0.3f).ToRotationVector2() * speed;
                        }
                        else
                            NPC.velocity *= 0.8f;

                        if (Timer > 75)
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
                            DoRotation(0.04f);

                        if (Timer > 20)
                            SetPhase3States();
                    }
                    break;
            }

            Timer++;
        }

        public void P3_NightmareDash()
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

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(Target.direction, 0) * Main.rand.NextFloat(125, 150);
                            else
                                return pos + Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(125, 150);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 60, ai2: -2);
                        }, 20, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;

                            SoundStyle st = CoraliteSoundID.DeathCalling_Item103;
                            st.Pitch -= 0.4f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            NPC.rotation = (pos - NPC.Center).ToRotation();
                            ShootCount = (NPC.Center - pos).ToRotation();
                        });
                    }
                    break;
                case 4://消失并瞬移到玩家背后
                    {
                        Phase2Fade(() =>
                        {
                            Vector2 pos = Target.Center;

                            if (Math.Abs(Target.velocity.X) < 0.1f && Math.Abs(Target.velocity.Y) < 0.1f)
                                return pos + new Vector2(-Target.direction, 0) * Main.rand.NextFloat(125, 150);
                            else
                                return pos - Target.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(125, 150);
                        }, () =>
                        {
                            useMeleeDamage = true;
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<NightmareBite>(NPC.Center, Vector2.Zero, damage, 4, ai0: 0, ai1: 60, ai2: -2);
                        }, 10, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;

                            SoundStyle st = CoraliteSoundID.DeathCalling_Item103;
                            st.Pitch -= 0.4f;
                            SoundEngine.PlaySound(st, NPC.Center);
                            NPC.rotation = (pos - NPC.Center).ToRotation();
                            ShootCount = (NPC.Center - pos).ToRotation();
                        });
                    }

                    break;
                case 1: //先短暂同步玩家位置，之后向一个方向冲刺咬下
                case 5:
                    {
                        float factor = Timer / 48f;

                        Vector2 pos = Target.Center + new Vector2(Target.direction * factor * 80, 0) + Target.velocity * 14;
                        if (FantasySparkleAlive(out NPC fs))
                            pos = fs.Center;

                        if (Timer < 40)
                            NPC.rotation += MathHelper.TwoPi / 40;
                        else
                        {
                            float targetAngle = (pos - NPC.Center).ToRotation();
                            NPC.rotation = NPC.rotation.AngleTowards(targetAngle, 0.3f);
                        }

                        if (Timer < 40)
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

                        if (Timer > 48)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://冲刺并咬下
                case 6:
                    {
                        NPC.velocity *= 0.97f;
                        if (Timer > 6)
                        {
                            Vector2 pos = Target.Center + new Vector2(Target.direction * 80, 0) + Target.velocity * 14;

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
                        Vector2 dir = -NPC.velocity.SafeNormalize(Vector2.Zero);
                        for (int i = 0; i < 5; i++)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustType<NightmarePetal>(), newColor: nightmareRed);
                            dust.velocity = dir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.5f, 8);
                            dust.noGravity = true;
                        }

                        if (Timer > 15)
                        {
                            NPC.velocity *= 0;
                            Timer = 0;
                            SonState++;
                        }
                    }
                    break;

                case 7:
                    {
                        if (Timer > 12)
                        {
                            NPC.velocity *= 0.9f;
                            DoRotation(0.04f);
                        }

                        Vector2 dir = -NPC.velocity.SafeNormalize(Vector2.Zero);
                        for (int i = 0; i < 5; i++)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustType<NightmarePetal>(), newColor: nightmareRed);
                            dust.velocity = dir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.5f, 8);
                            dust.noGravity = true;
                        }

                        if (Timer > 17)
                        {
                            Timer = 0;
                            SonState++;
                        }
                    }
                    break;
                case 8:
                    {
                        if (Timer > 12)
                        {
                            NPC.velocity *= 0.9f;
                            DoRotation(0.04f);
                        }

                        Vector2 dir = -NPC.velocity.SafeNormalize(Vector2.Zero);
                        for (int i = 0; i < 5; i++)
                        {
                            Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustType<NightmarePetal>(), newColor: nightmareRed);
                            dust.velocity = dir.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.5f, 8);
                            dust.noGravity = true;
                        }

                        if (Timer > 17)
                            SetPhase3States();
                    }
                    break;
            }

            Timer++;
        }

        public void VineSpurt()
        {
            switch ((int)SonState)
            {
                default:
                    SetPhase3States();
                    break;
                case 0://瞬移到玩家面前
                    {
                        Phase3Fade(() =>
                        {
                            Vector2 pos = Target.Center;

                            return pos + new Vector2(Target.direction, 0).RotatedBy(Main.rand.NextFromList(-0.75f, 0.57f)) * Main.rand.NextFloat(350, 450);
                        }, () =>
                        {
                            SonState++;
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                        }, PostTeleport: () =>
                        {
                            Vector2 pos = Target.Center;

                            NPC.rotation = (pos - NPC.Center).ToRotation();
                            ShootCount = (NPC.Center - pos).ToRotation();
                            EXai1 = Main.rand.Next(4, 7) * 25 + 40;
                            int side = -1;
                            float rot = (NPC.Center - Target.Center).ToRotation() + side * 0.9f;
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<VineSpike>(NPC.Center, rot.ToRotationVector2(), damage, 8, NPC.target, -1, rot, 25);
                        });
                    }
                    break;
                case 1://每隔一段时间射出一个荆棘刺
                    {
                        DoRotation(0.3f);

                        float angle = ShootCount + MathHelper.PiOver4 / 4 * MathF.Sin(Timer * 0.0314f);
                        Vector2 center = Target.Center + angle.ToRotationVector2() * 400;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 56;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.3f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.45f);

                        if (Timer % 25 == 0)
                        {
                            int side = Timer % 50 == 0 ? -1 : 1;
                            float rot = (NPC.Center - Target.Center).ToRotation() + side * 0.9f;
                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);
                            NPC.NewProjectileInAI<VineSpike>(NPC.Center, rot.ToRotationVector2(), damage, 8, NPC.target, -1, rot, 25);
                        }

                        if (Timer > EXai1)
                        {
                            SonState++;
                            Timer = 0;
                            Vector2 pos = Target.Center;

                            NPC.rotation = (pos - NPC.Center).ToRotation();
                            ShootCount = (NPC.Center - pos).ToRotation();

                            int damage = Helper.ScaleValueForDiffMode(30, 20, 15, 15);

                            float rot = (NPC.Center - Target.Center).ToRotation() -  1.2f;
                            for (int i = 0; i < 4; i++)
                            {
                                NPC.NewProjectileInAI<VineSpike>(NPC.Center, rot.ToRotationVector2(), damage, 8, NPC.target, -1, rot, 35);
                                rot += 2.4f / 4;
                            }
                        }
                    }   
                    break;
                case 2:
                    {
                        DoRotation(0.3f);

                        float angle = ShootCount + MathHelper.PiOver4 / 4 * MathF.Sin(Timer * 0.0314f);
                        Vector2 center = Target.Center + angle.ToRotationVector2() * 550;
                        Vector2 dir = center - NPC.Center;

                        float velRot = NPC.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 56;

                        NPC.velocity = velRot.AngleTowards(targetRot, 0.3f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.45f);

                        if (Timer > 75)
                            SetPhase3States();
                    }
                    break;
            }

            Timer++;
        }

        #endregion

        #region MiscMethods

        public void Phase3Fade(Func<Vector2> teleportPos, Action OnTeleport, int fadeTime = 30, Action PostTeleport = null)
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
                        dir2 * Main.rand.NextFloat(2f, 6f), newColor: nightmareRed, Scale: Main.rand.NextFloat(1f, 4f));
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
                PostTeleport?.Invoke();

                for (int i = 0; i < 3; i++)
                {
                    RotateTentacle tentacle = rotateTentacles[i];
                    tentacle.pos = tentacle.targetPos = NPC.Center;
                    tentacle.rotation = NPC.rotation;
                }
            }
        }

        #endregion

        #region States

        public void SetPhase3States()
        {
            NPC.dontTakeDamage = false;
            warpScale = 0;
            canDrawWarp = false;
            useMeleeDamage = false;
            useDreamMove = false;
            tentacleColor = nightmareRed;
            DreamMoveCount = 0;
            fantasyKillCount = 0;
            tentacleStarFrame = 1;
            alpha = 1;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Timer = 0;
            SonState = 0;
            NPC.netUpdate = true;

            State = (int)MoveCount switch
            {
                0 => (int)AIStates.illusionBite,
                1 => (int)AIStates.tripleSpikeHell,
                2 => P3_RandomBite(),
                3 => (int)AIStates.illusionBite,
                4 => Main.rand.Next(0, 2) switch
                {
                    0 => (int)AIStates.superHookSlash,
                    _ => (int)AIStates.vineSpurt,
                },
                5 => P3_RandomBite(),
                6 => (int)AIStates.P3_SpikesAndSparkles,
                7 => P3_RandomBite(),
                8 => (int)AIStates.illusionBite,
                9 => (int)AIStates.P3_teleportSparkles,
                _ => (int)AIStates.flowerDance,
            };

            //State = (int)AIStates.vineSpurt;

            switch ((int)State)
            {
                default:
                    ShootCount = 0;
                    break;
                case (int)AIStates.illusionBite:
                    {
                        ShootCount = Main.rand.NextFromList(8 * 3, 8 * 4, 8 * 5, 8 * 6, 8 * 7);
                    }
                    break;
            }

            MoveCount++;
            if (MoveCount > 9)
                MoveCount = 0;
        }

        public static int P3_RandomBite()
        {
            return Main.rand.Next(0, 2) switch
            {
                0 => (int)AIStates.P3_nightmareBite,
                _ => (int)AIStates.P3_nightmareDash,
            };
        }

        public void OnExchangeToP3()
        {
            MoveCount = 0;
            NPC.dontTakeDamage = false;
            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
            {
                NCamera.useShake = false;
                NCamera.useScreenMove = false;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc != null && npc.active && npc.type == NPCType<FantasySparkle>())
                    npc.Kill();
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Phase = (int)AIPhases.Nightemare_P3;

            warpScale = 0;
            NPC.dontTakeDamage = true;
            useMeleeDamage = false;
            useDreamMove = false;
            DreamMoveCount = 0;
            fantasyKillCount = 0;
            alpha = 1;

            State = (int)AIStates.exchange_P2_P3;
            Timer = 0;
            SonState = 0;
            NPC.netUpdate = true;
        }

        #endregion
    }
}
