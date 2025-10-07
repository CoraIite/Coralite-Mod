using Coralite.Helpers;
using System;
using Terraria;
using Terraria.ID;

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
                    Jump(1f, 8f, () => SonState++);
                    break;
                case 3: //重置AI
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
                                JumpState = (int)JumpStates.ReadyToJump;
                                JumpTimer = -1;
                                break;
                            }

                            JumpTimer++;

                            if (Math.Abs(NPC.velocity.Y) < 0.05f)//如果脚下有方块且速度小于一定值，那么判断为在地上
                            {
                                if (NPC.Center.Y < (Target.Center.Y - 100)) //比玩家高，那么只判断实心物块
                                    for (int i = -16; i < NPC.width + 16; i += 16)
                                    {
                                        for (int j = 0; j < 2; j++)
                                        {
                                            Tile tile = Framing.GetTileSafely(NPC.BottomLeft + new Vector2(i, j));
                                            if (tile.HasTile && Main.tileSolid[tile.TileType])
                                            {
                                                NPC.frame.Y = 0;
                                                NPC.noGravity = true;
                                                JumpTimer = -1;
                                                break;
                                            }
                                        }
                                    }
                                else    //比玩家低，那么要判断其他顶部实心的物块
                                    for (int i = -16; i < NPC.width + 16; i += 16)
                                    {
                                        for (int j = 0; j < 2; j++)
                                        {
                                            Tile tile = Framing.GetTileSafely(NPC.BottomLeft + new Vector2(i, j));
                                            if (tile.HasTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] || TileID.Sets.Platforms[tile.TileType]))
                                            {
                                                NPC.frame.Y = 0;
                                                NPC.noGravity = true;
                                                JumpTimer = -1;
                                                break;
                                            }
                                        }
                                    }
                            }

                            break;
                        case -1: //刚落地之后的变扁阶段
                            NPC.velocity *= 0f;
                            Scale = Vector2.Lerp(Scale, new Vector2(1.25f, 0.95f), 0.15f);
                            if (Scale.X > 1.2f)
                            {
                                JumpTimer = -2;
                            }
                            break;

                        case -2://回弹
                            Scale = Vector2.Lerp(Scale, new Vector2(0.85f, 1.2f), 0.15f);
                            if (Scale.X < 0.9f)
                            {
                                CrownJumpUp(0.05f, 4f);
                                JumpTimer = -3;
                            }
                            break;

                        case -3://回到正常大小
                            Scale = Vector2.Lerp(Scale, Vector2.One, 0.15f);
                            if (Math.Abs(Scale.Y - 1) < 0.05f)
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

                        Scale = Vector2.Lerp(Scale, new Vector2(1.25f, 0.85f), 0.15f);

                        if (NPC.frame.Y == 3)
                        {
                            NPC.noGravity = false;
                            onStartJump?.Invoke();
                            JumpState = (int)JumpStates.Jumping;
                            JumpTimer = 0;
                            NPC.TargetClosest();
                        }
                    }
                    break;

                case (int)JumpStates.Jumping:  //跳跃
                    float targetScaleX = Math.Clamp(1 - (jumpYVelocity / 15), 0.75f, 1f);
                    Scale = Vector2.Lerp(Scale, new Vector2(targetScaleX, 1.2f), 0.15f);

                    if (NPC.frame.Y < 5)
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter > 4)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                        }
                    }
                    else
                    {
                        JumpTimer = 0;
                        JumpState = (int)JumpStates.CheckForLanding;
                        NPC.noGravity = false;
                        onJumpFinish?.Invoke();
                    }

                    if (JumpTimer < 4)
                    {
                        NPC.velocity.Y -= jumpYVelocity * (1 - (2 * JumpTimer / 16)) * (2f - LifePercentScale);
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * jumpXVelocity * (1.8f - LifePercentScale), 0.2f);
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
                    if (tile.HasReallySolidTile())
                    {
                        JumpState = (int)JumpStates.ReadyToJump;
                        NPC.frame.Y = 0;
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
