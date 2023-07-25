using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void ThreeMiniJump()
        {
            //连续3次的小跳
            switch ((int)SonState)
            {
                default:
                case 0:
                case 1:
                case 2: //3次小跳，仅当落到地面之后才能够进行跳跃
                    Jump(2f, 8f, () => SonState++);
                    break;
                case 3:
                    //重置AI
                    ResetStates();
                    break;
            }
        }

        public void Jump(float jumpYVelocity, float jumpXVelocity, Action onJumpFinish = null, Action onLanding = null, Action onStartJump = null)
        {
            switch ((int)JumpState)
            {
                default:
                case (int)JumpStates.CheckForLanding: //检测落地的阶段
                    switch ((int)JumpTimer)
                    {
                        default:
                            if (JumpTimer > 600)
                            {
                                NPC.frame.Y = 0;
                                NPC.noGravity = true;
                                CanDrawShadow = false;
                                JumpState = (int)JumpStates.ReadyToJump;
                                JumpTimer = -1;
                                break;
                            }

                            JumpTimer++;

                            if (Math.Abs(NPC.velocity.Y) < 0.1f)
                                for (int i = 0; i < NPC.width; i += 16) //如果脚下有方块且速度小于一定值，那么判断为在地上
                                {
                                    Tile tile = Framing.GetTileSafely(NPC.BottomLeft + new Vector2(i, 0));
                                    if (tile.HasTile && Main.tileSolid[tile.TileType])
                                    {
                                        NPC.frame.Y = 0;
                                        NPC.noGravity = true;
                                        CanDrawShadow = false;
                                        JumpTimer = -1;
                                        break;
                                    }
                                }
                            break;

                        case -1: //刚落地之后的变扁阶段
                            NPC.velocity *= 0f;
                            Scale.X = MathHelper.Lerp(Scale.X, 0.95f, 0.15f);
                            Scale.Y = MathHelper.Lerp(Scale.Y, 1.2f, 0.15f);
                            if (Scale.Y > 1.1f)
                            {
                                if (Math.Abs(crown.Velocity_Y) < 0.3f)
                                    crown.Velocity_Y -= 8f;
                                JumpTimer = -2;
                            }
                            break;

                        case -2://回弹
                            Scale.X = MathHelper.Lerp(Scale.X, 1.1f, 0.15f);
                            Scale.Y = MathHelper.Lerp(Scale.Y, 0.85f, 0.15f);
                            if (Scale.Y < 0.95f)
                            {
                                JumpTimer = -3;
                            }
                            break;

                        case -3://回到正常大小
                            Scale.X = MathHelper.Lerp(Scale.X, 1f, 0.15f);
                            Scale.Y = MathHelper.Lerp(Scale.Y, 1f, 0.15f);
                            if (Scale.Y - 1 < 0.05f)
                            {
                                Scale = Vector2.One;
                                onLanding?.Invoke();
                                JumpState = (int)JumpStates.ReadyToJump;
                                JumpTimer = 0;
                            }
                            break;
                    }

                    break;
                case (int)JumpStates.ReadyToJump:  //起跳准备的阶段
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter > 4)
                        {
                            NPC.frame.Y++;
                            NPC.frameCounter = 0;
                        }

                        Scale.X = MathHelper.Lerp(Scale.X, 0.95f, 0.15f);
                        Scale.Y = MathHelper.Lerp(Scale.Y, 1.1f, 0.15f);

                        if (NPC.frame.Y == 3)
                        {
                            NPC.noGravity = false;
                            CanDrawShadow = true;
                            onStartJump?.Invoke();
                            JumpState = (int)JumpStates.Jumping;
                            JumpTimer = 0;
                            NPC.TargetClosest();
                        }
                    }
                    break;

                case (int)JumpStates.Jumping:  //跳跃
                    float targetScaleX = Math.Clamp(1 - jumpYVelocity / 50, 0.75f, 1f);
                    Scale.X = MathHelper.Lerp(Scale.X, targetScaleX, 0.15f);
                    Scale.Y = MathHelper.Lerp(Scale.Y, 1.05f, 0.15f);

                    if (NPC.frame.Y < 5)
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter > 4)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                        }
                    }

                    if (JumpTimer < 4)
                    {
                        NPC.velocity.Y -= jumpYVelocity * (1 - (2 * JumpTimer / 16));
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * jumpXVelocity * (1.8f - LifePercentScale), 0.2f);
                    }
                    else
                    {
                        JumpTimer = 0;
                        JumpState = (int)JumpStates.Jumping;
                        NPC.noGravity = false;
                        onJumpFinish?.Invoke();
                    }

                    JumpTimer++;
                    break;
            }
        }

        /// <summary>
        /// 帮助方法，用于在开始跳跃时判定当前状态
        /// </summary>
        public void StartJump()
        {
            if (Math.Abs(NPC.velocity.Y) < 0.1f)
                for (int i = 0; i < NPC.width; i += 16) //如果脚下有方块且速度小于一定值，那么判断为在地上
                {
                    Tile tile = Framing.GetTileSafely(NPC.BottomLeft + new Vector2(i, 0));
                    if (tile.HasSolidTile())
                    {
                        JumpState = (int)JumpStates.ReadyToJump;
                        return;
                    }
                }

            JumpState = (int)JumpStates.CheckForLanding;
        }

        private enum JumpStates
        {
            CheckForLanding = 0,
            ReadyToJump = 1,
            Jumping = 2,
        }
    }
}
