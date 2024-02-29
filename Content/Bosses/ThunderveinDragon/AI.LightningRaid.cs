using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public partial class ThunderveinDragon
    {
        public void LightningRaidP1()
        {
            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0://先飞到离玩家比较近或者计时器到点了
                    {
                        const int chasingTime = 60 * 4;
                        NPC.QuickSetDirection();

                        //追踪玩家
                        GetLengthToTargetPos(Target.Center, out float xLength, out float yLength);

                        if (xLength < 200)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, 18f, 0.3f, 0.6f, 0.95f);
                        else if (xLength > 400)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 18f, 0.3f, 0.6f, 0.95f);
                        else
                            NPC.velocity.X *= 0.95f;

                        if (NPC.directionY < 0)
                            FlyingUp(0.25f, 15, 0.95f);
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

                        float targetRot = NPC.velocity.Length() * 0.03f * NPC.direction + (NPC.direction > 0 ? 0 : MathHelper.Pi);
                        NPC.rotation = NPC.rotation.AngleLerp(targetRot, 0.08f);
                        Timer++;
                        if (Timer > chasingTime || (xLength > 200 && xLength < 400 && yLength < 70))
                        {
                            SonState++;
                            Timer = 0;
                            DashFrame();
                        }
                    }
                    break;
                case 1://开始乱窜几下
                    {
                        const int dashTime = 15;

                        if (Timer == 0)
                        {
                            //生成弹幕并随机速度方向

                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            float targetrot = (Target.Center - NPC.Center).ToRotation();
                            targetrot += Main.rand.NextFromList(-1, 1) * Main.rand.NextFloat(1f, 1.37f);
                            NPC.velocity = targetrot.ToRotationVector2() * 16;
                            NPC.rotation = NPC.velocity.ToRotation();
                        }
                        else if (Timer % dashTime == 0)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            float targetrot = (Target.Center - NPC.Center).ToRotation();
                            targetrot += (Timer / dashTime > 1 ? -1 : 1) * Main.rand.NextFloat(1f, 1.37f);
                            NPC.velocity = targetrot.ToRotationVector2() * 16;
                            NPC.rotation = NPC.velocity.ToRotation();
                        }

                        Timer++;
                        if (Timer > dashTime * 3 - 2)
                        {
                            SonState++;
                            Timer = 0;
                            NPC.velocity *= 0;
                            NPC.frame.X = 0;
                            NPC.frame.Y = 4;
                            NPC.frameCounter = 0;
                            NPC.rotation = NPC.direction > 0 ? 0 : 3.141f;
                        }
                    }
                    break;
                case 2://准备冲刺！
                    {
                        NPC.QuickSetDirection();

                        float targetRot = NPC.velocity.Length() * 0.03f * NPC.direction + (NPC.direction > 0 ? 0 : MathHelper.Pi);
                        NPC.rotation = NPC.rotation.AngleLerp(targetRot, 0.08f);

                        if (NPC.velocity.Length() < 8)
                        {
                            NPC.velocity += (NPC.Center - Target.Center).SafeNormalize(Vector2.Zero) * 0.25f;
                        }
                        //向后扇一下翅膀
                        if (++NPC.frameCounter > 7)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > 7)
                            {
                                SonState++;
                                Timer = 0;

                                DashFrame();

                                NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 16;
                                NPC.rotation = NPC.velocity.ToRotation();
                            }
                        }
                    }
                    break;
                case 3://冲刺！冲刺！
                    {
                        Timer++;
                        if (Timer > 30)
                            ResetStates();
                    }
                    break;
            }
        }
    }
}
