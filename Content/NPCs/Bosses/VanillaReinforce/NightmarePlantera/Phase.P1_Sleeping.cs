using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    [AutoloadBossHead]
    public sealed partial class NightmarePlantera
    {
        public void Sleeping_Phase1()
        {
            if (!spawnedHook)   //生成钩子
            {
                spawnedHook = true;
                for (int i = 0; i < 3; i++)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NightmareHook>(), NPC.whoAmI);
            }

            Phase1_Movement();
            UpdateFrameNormally();
            switch ((int)State)
            {
                default:
                case (int)AIStates.P1_Idle:
                    P1_Idle();
                    break;
                case (int)AIStates.hypnotizeFog:
                    HypnotizeFog();
                    break;
                case (int)AIStates.darkTentacle:
                    DarkTentacle();
                    break;
                case (int)AIStates.darkLeaves:
                    DarkLeaves();
                    break;
            }
        }

        #region AI

        public void Phase1_Movement()
        {
            float targetX = 0f;
            float targetY = 0f;
            int hookWhoAmI = 0;
            for (int i = 0; i < 200; i++)
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<NightmareHook>())
                {
                    targetX += Main.npc[i].Center.X;
                    targetY += Main.npc[i].Center.Y;
                    hookWhoAmI++;
                    if (hookWhoAmI > 2)
                        break;
                }

            if (hookWhoAmI < 3)   //少于3个钩子那么就生成一个
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NightmareHook>(), NPC.whoAmI);

            targetX /= hookWhoAmI;
            targetY /= hookWhoAmI;

            float num779 = 2.5f;
            float speed = 0.05f;
            if (NPC.life < NPC.lifeMax * 15 / 16)
            {
                num779 = 5f;
                speed = 0.075f;
            }

            if (NPC.life < NPC.lifeMax * 13 / 16)
                num779 = 7f;

            if (Main.expertMode)
            {
                num779 += 1f;
                num779 *= 1.1f;
                speed += 0.03f;
                speed *= 1.2f;
            }

            if (Main.getGoodWorld)
            {
                num779 *= 1.15f;
                speed *= 1.3f;
            }

            //由3个爪子共同决定的目标点
            Vector2 targetPos = new Vector2(targetX, targetY);
            float num781 = Target.Center.X - targetPos.X;
            float num782 = Target.Center.Y - targetPos.Y;

            float posToTargetLength = (float)Math.Sqrt(num781 * num781 + num782 * num782);
            int maxLength = 800;    //大概是最大延申距离

            if (Main.expertMode)
                maxLength += 250;

            if (posToTargetLength >= maxLength)
            {
                posToTargetLength = maxLength / posToTargetLength;
                num781 *= posToTargetLength;
                num782 *= posToTargetLength;
            }

            targetX += num781;
            targetY += num782;
            targetPos = new Vector2(NPC.Center.X, NPC.Center.Y);
            num781 = targetX - targetPos.X;
            num782 = targetY - targetPos.Y;
            posToTargetLength = (float)Math.Sqrt(num781 * num781 + num782 * num782);
            if (posToTargetLength < num779)
            {
                num781 = NPC.velocity.X;
                num782 = NPC.velocity.Y;
            }
            else
            {
                posToTargetLength = num779 / posToTargetLength;
                num781 *= posToTargetLength;
                num782 *= posToTargetLength;
            }

            if (NPC.velocity.X < num781)
            {
                NPC.velocity.X += speed;
                if (NPC.velocity.X < 0f && num781 > 0f)
                    NPC.velocity.X += speed * 2f;
            }
            else if (NPC.velocity.X > num781)
            {
                NPC.velocity.X -= speed;
                if (NPC.velocity.X > 0f && num781 < 0f)
                    NPC.velocity.X -= speed * 2f;
            }

            if (NPC.velocity.Y < num782)
            {
                NPC.velocity.Y += speed;
                if (NPC.velocity.Y < 0f && num782 > 0f)
                    NPC.velocity.Y += speed * 2f;
            }
            else if (NPC.velocity.Y > num782)
            {
                NPC.velocity.Y -= speed;
                if (NPC.velocity.Y > 0f && num782 < 0f)
                    NPC.velocity.Y -= speed * 2f;
            }
        }

        public void P1_Idle()
        {
            if (Timer < 180)
            {
                Timer++;
                DoRotation(0.1f);
                //血量少于一定值后加速
                if (NPC.life < NPC.lifeMax * 15 / 16)
                    Timer++;
                if (NPC.life < NPC.lifeMax * 13 / 16)
                    Timer++;

                //受击时候会加速
                if (NPC.justHit)
                    Timer++;
            }
            else
                SetPhase1States();
        }

        public void HypnotizeFog()
        {
            DoRotation(0.1f);

            do
            {
                if (Timer < 160)
                {
                    Vector2 pos = GetPhase1MousePos();
                    float factor = Timer / 160f;
                    float width = 60 - factor * 50;

                    Dust dust;
                    for (int i = 0; i < 2; i++)
                    {
                        dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(width, width), ModContent.DustType<NightmareDust>(), Scale: Main.rand.NextFloat(1f, 1.4f));
                        dust.velocity = (pos - dust.position).SafeNormalize(Vector2.Zero) * (3 - factor * 3) + NPC.velocity * factor;
                        dust.noGravity = true;
                    }

                    Dust.NewDustPerfect(pos, DustID.VilePowder, Helper.NextVec2Dir(1f, 3), Scale: Main.rand.NextFloat(1f, 2f));

                    break;
                }

                if (Timer == 160)
                {
                    Vector2 pos = GetPhase1MousePos();
                    Vector2 dir = NPC.rotation.ToRotationVector2();
                    int damage = Helper.ScaleValueForDiffMode(40, 35, 30, 25);
                    for (int i = -1; i < 2; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, dir.RotatedBy(i * 0.35f) * Main.rand.NextFloat(6f, 14f), ModContent.ProjectileType<HypnotizeFog>(),
                            damage, 4, Target.whoAmI);

                    SoundEngine.PlaySound(CoraliteSoundID.SpiritFlame_Item117, NPC.Center);
                    NPC.velocity = -dir * 6;
                }

                if (Timer < 200)
                    break;

                SetPhase1Idle();
            } while (false);
            Timer++;
        }

        public void DarkTentacle()
        {
            do
            {
                if (Timer < 30)
                {
                    DoRotation(0.1f);
                    break;
                }

                if (Timer == 30 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int notFreeTime = 60;
                    int MaxFreeTime = 240;
                    int extraTextacle = 0;

                    if (NPC.life < NPC.lifeMax * 15 / 16)
                    {
                        notFreeTime -= 20;
                        MaxFreeTime += 80;
                        extraTextacle += 2;
                    }

                    if (NPC.life < NPC.lifeMax * 13 / 16)
                    {
                        notFreeTime -= 20;
                        MaxFreeTime += 80;
                        extraTextacle += 2;
                    }

                    int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NightmareCatcher>(), NPC.whoAmI, ai2: notFreeTime, ai3: MaxFreeTime);
                    Main.npc[index].velocity = NPC.rotation.ToRotationVector2() * 8;

                    for (int i = 0; i < extraTextacle; i++)
                    {
                        int index2 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NightmareCatcher>(), NPC.whoAmI, ai2: notFreeTime * 0.75f, ai3: MaxFreeTime * 0.75f);
                        float angle = Main.rand.NextFromList(-1.1f, 1.1f) + Main.rand.NextFloat(-0.2f, 0.2f);
                        Main.npc[index2].velocity = (NPC.rotation + angle).ToRotationVector2() * 8;
                    }

                    SoundEngine.PlaySound(CoraliteSoundID.SpiderStaff_Item83, NPC.Center);
                    NPC.velocity = -NPC.rotation.ToRotationVector2() * 6;
                }

                if (Timer < 75)
                {
                    break;
                }

                SetPhase1Idle();
            } while (false);

            Timer++;
        }

        public void DarkLeaves()
        {
            int shootDelay = Helper.ScaleValueForDiffMode(12, 11, 10, 8);
            if (NPC.life < NPC.lifeMax * 15 / 16)
                shootDelay--;

            if (NPC.life < NPC.lifeMax * 13 / 16)
                shootDelay--;

            DoRotation(0.1f);
            if (Timer % shootDelay == 0)
            {
                Vector2 pos = GetPhase1MousePos();
                Vector2 dir = NPC.rotation.ToRotationVector2();
                int damage = Helper.ScaleValueForDiffMode(40, 35, 30, 25);

                if (Timer % (shootDelay * 4) == 0)    //每隔3次固定射出4发弹幕
                {
                    for (int i = -1; i < 2; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, dir.RotatedBy(i * 0.45f) * 13f,
                            ModContent.ProjectileType<DarkLeaf>(), damage, 4, NPC.target, 1);
                    }
                }
                else
                {
                    Vector2 vel = dir * 13;
                    if (Main.rand.NextBool(3))
                        vel = vel.RotatedBy(Main.rand.NextFromList(-0.35f, 0.35f));

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, vel,
                        ModContent.ProjectileType<DarkLeaf>(), damage, 4, NPC.target);
                }

                SoundEngine.PlaySound(CoraliteSoundID.NoUse_BlowgunPlus_Item65, NPC.Center);
                NPC.velocity = -dir * 2f;
            }

            int timeMax = 14 * 4;
            if (NPC.life < NPC.lifeMax * 15 / 16)
                timeMax += 14 * 4;

            if (NPC.life < NPC.lifeMax * 13 / 16)
                timeMax += 14 * 4;

            if (Timer > timeMax)
            {
                SetPhase1Idle();
                return;
            }

            Timer++;
        }

        #endregion

        #region States

        public void SetPhase1States()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Timer = 0;
            useMeleeDamage = true;

            if (Vector2.Distance(NPC.Center, Target.Center) < 300 && Main.rand.NextBool(3)) //距离小于一定值之后固定使用沉睡花雾
            {
                State = (int)AIStates.hypnotizeFog;
                NPC.netUpdate = true;
                SoundStyle style = CoraliteSoundID.WallOfFlesh_NPCDeath10;
                style.Pitch = -0.5f;
                SoundEngine.PlaySound(style, NPC.Center);
                return;
            }

            //随机一个状态
            State = Main.rand.Next(0, 2) switch
            {
                0 => (int)AIStates.darkLeaves,
                _ => (int)AIStates.darkTentacle
            };

            SoundEngine.PlaySound(CoraliteSoundID.MoonLord2_Zombie94, NPC.Center);
            NPC.netUpdate = true;
        }

        public void SetPhase1Idle()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (NPC.life < NPC.lifeMax * 3 / 4)
            {
                SetPhase1Exchange();
                return;
            }

            Timer = 0;
            alpha = 1;
            useMeleeDamage = true;
            Phase = (int)AIPhases.Sleeping_P1;
            State = (int)AIStates.P1_Idle;
            NPC.netUpdate = true;
            NPC.dontTakeDamage = false;
            warpScale = 0;
            canDrawWarp = false;
            useDreamMove = false;
            tentacleColor = lightPurple;
            DreamMoveCount = 0;
            fantasyKillCount = 0;
            tentacleStarFrame = 0;
        }

        #endregion
    }
}
