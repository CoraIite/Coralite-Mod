using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void StygianThunder()
        {
            const int burstTime = 50;
            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case -1://幻影被打破
                    {
                        Timer++;

                        FlyingFrame();
                        TurnToNoRot(1);
                        selfAlpha += 1 / 140f;
                        if (Timer > 140)
                            ResetStates();
                    }
                    break;
                case 0://靠近玩家
                    {
                        const int chasingTime = 60 * 4;

                        Vector2 targetPos = Target.Center;

                        NPC.direction = NPC.spriteDirection = targetPos.X > NPC.Center.X ? 1 : -1;
                        NPC.directionY = targetPos.Y > NPC.Center.Y ? 1 : -1;
                        SetRotationNormally();

                        //追踪玩家
                        GetLengthToTargetPos(targetPos, out float xLength, out float yLength);

                        if (xLength > 600)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 18f, 0.3f, 0.6f, 0.95f);
                        else if (xLength < 200)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 18f, 0.3f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.55f, 20, 0.9f);
                        else if (yLength > 70)
                        {
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 15f, 0.25f, 0.6f, 0.95f);
                            FlyingFrame();
                        }
                        else
                        {
                            NPC.velocity.Y *= 0.95f;
                            FlyingFrame();
                        }

                        Timer++;
                        if (Timer > chasingTime || (xLength > 200 && xLength < 600 && yLength < 300))
                        {
                            SonState++;
                            Timer = 0;
                            DashFrame();
                            ResetAllOldCaches();
                            canDrawShadows = true;
                            shadowScale = 1.15f;
                            shadowAlpha = 1;

                            currentSurrounding = true;

                            NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero)
                                .RotatedBy(-NPC.direction * MathHelper.PiOver2) * 20;
                        }
                    }
                    break;
                case 1://旋转飞进背景中，并生成幻影
                    {
                        const int rollingTime = 65;
                        const int rollingFactor = 60;

                        UpdateAllOldCaches();
                        NPC.velocity = NPC.velocity.RotatedBy(NPC.spriteDirection * MathHelper.TwoPi / rollingFactor);
                        NPC.velocity *= 0.996f;
                        NPC.rotation = NPC.velocity.ToRotation();

                        shadowAlpha = selfAlpha = 1 - (Timer / rollingTime);

                        Timer++;
                        if (Timer > rollingTime)
                        {
                            SonState++;
                            Timer = 0;
                            TurnToNoRot();
                            canDrawShadows = false;

                            currentSurrounding = false;
                            NPC.dontTakeDamage = true;
                            NPC.velocity *= 0;
                            Recorder = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ThunderPhantom>(),
                                  ai0: NPC.whoAmI, Target: NPC.target);
                        }
                    }
                    break;
                case 2://幻影释放雷击
                case 3:
                case 4:
                case 5:
                    {
                        if (!Recorder.GetNPCOwner<ThunderPhantom>(out _, () =>
                        {
                            SonState = -1;
                            Timer = 0;

                            NPC.dontTakeDamage = false;
                            NPC.QuickSetDirection();
                            ResetAllOldCaches();
                            TurnToNoRot(1);
                        }))
                            break;

                        NPC.Center = Target.Center + new Vector2(0, -400);
                    }
                    break;
                case 6://nmd输出这么低打不死幻影是吧，那你死了，直接闪现到玩家头顶释放交错闪电
                    {
                        NPC.Center = Target.Center + new Vector2(0, -400);
                        NPC.velocity *= 0.96f;
                        NPC.QuickSetDirection();
                        TurnToNoRot();

                        selfAlpha = shadowAlpha = Timer / 10;

                        if (NPC.frame.Y != 4)
                            FlyingFrame();
                        else
                        {
                            Timer++;
                            if (Timer > 30)
                            {
                                SonState++;
                                Timer = 0;

                                NPC.TargetClosest();
                                int damage = Helper.GetProjDamage(400, 400, 400);
                                //生成爆炸弹幕
                                NPC.NewProjectileDirectInAI<EndThunder>(
                                    NPC.Center + new Vector2(0, -200)
                                    , NPC.Center + new Vector2(0, 800), damage, 0, NPC.target, 20, NPC.whoAmI, 70);

                                SoundEngine.PlaySound(CoraliteSoundID.BubbleShield_Electric_NPCHit43, NPC.Center);
                                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                                SoundEngine.PlaySound(CoraliteSoundID.Thunder, NPC.Center);
                                var modifyer = new PunchCameraModifier(NPC.Center, Vector2.UnitY * 1.4f, 26, 26, 25, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);

                                canDrawShadows = true;
                                currentSurrounding = true;
                                NPC.dontTakeDamage = false;
                                SetBackgroundLight(0.7f, burstTime, 12);

                                ResetAllOldCaches();
                            }
                        }
                    }
                    break;
                case 7:
                    {
                        UpdateAllOldCaches();
                        float factor = Coralite.Instance.SqrtSmoother.Smoother(Timer / burstTime);
                        shadowScale = Helper.Lerp(1f, 2.5f, factor);
                        shadowAlpha = Helper.Lerp(1f, 0f, factor);

                        if (NPC.frame.Y != 0)
                        {
                            NPC.frame.X = 1;

                            if (++NPC.frameCounter > 1)
                            {
                                NPC.frameCounter = 0;
                                if (++NPC.frame.Y > 7)
                                    NPC.frame.Y = 0;
                            }
                        }
                        Timer++;
                        if (Timer > burstTime)
                        {
                            canDrawShadows = false;
                            currentSurrounding = false;

                            Timer = 0;
                            SonState++;
                        }
                    }
                    break;
                case 8://短暂后摇
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 20)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
