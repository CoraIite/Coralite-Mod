using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    /// <summary>
    /// 使用ai1传入BOSS，使用ai2控制能否分裂，ai2为3以上时无法分裂
    /// </summary>
    public class SlimeAvatar : ModNPC
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        private Vector2 Scale;

        private float LifePercentScale => Math.Clamp(NPC.life / (float)NPC.lifeMax, 0.5f, 1);

        internal ref float JumpState => ref NPC.ai[0];
        internal float JumpTimer;
        internal ref float State => ref NPC.ai[3];

        public float xVel;
        public float yVel;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 20;
            NPC.scale = 1.5f;
            NPC.lifeMax = 125;
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0.1f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = CoraliteSoundID.Fleshy_NPCHit1;
            NPC.hide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = 75 + numPlayers * 75;
            if (Main.getGoodWorld)
            {
                NPC.damage = 60;
            }
        }

        public override bool? CanFallThroughPlatforms() => NPC.Center.Y < (Main.player[NPC.target].Center.Y - NPC.height);

        public override void OnSpawn(IEntitySource source)
        {
            Scale = Vector2.One;
        }

        public override void AI()
        {
            //跳一跳，之后分裂
            switch ((int)State)
            {
                case -1:
                    NPC boss = Main.npc[(int)NPC.ai[1]];
                    if (!boss.active)
                    {
                        NPC.noGravity = false;
                        State = 0;
                        return;
                    }

                    NPC.Center = Vector2.Lerp(NPC.Center, boss.Center, 0.04f);
                    break;
                default:
                    Jump(yVel, xVel, onLanding: () =>
                    {
                        xVel = Main.rand.NextFloat(3f, 6f);
                        yVel = Main.rand.NextFloat(1f, 3f);
                        NPC.netUpdate = true;
                        if (NPC.life > NPC.lifeMax * 2 / 3 && NPC.ai[2] < 2)    //只有血量大于一定值以及当前为半血以上时才会分裂
                            State++;
                    });
                    break;
                case 7:
                    Scale = Vector2.Lerp(Scale, new Vector2(1.25f, 0.8f), 0.1f);
                    if (Scale.X > 1.2f)
                    {
                        State++;
                    }
                    break;
                case 8:
                    Scale = Vector2.Lerp(Scale, new Vector2(0.8f, 1.25f), 0.1f);
                    if (Scale.Y > 1.2f)
                    {
                        SoundEngine.PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient) //分裂成2个
                        {
                            int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Left.X, (int)NPC.Left.Y, ModContent.NPCType<SlimeAvatar>(), ai1: NPC.ai[1], ai2: NPC.ai[2] + 1, Target: NPC.target);
                            Main.npc[index].lifeMax = Main.npc[index].life = NPC.lifeMax * 3 / 4;
                            Main.npc[index].width = NPC.width * 2 / 3;
                            Main.npc[index].height = NPC.height * 2 / 3;
                            Main.npc[index].scale = NPC.scale * 2 / 3;
                            Main.npc[index].netUpdate = true;

                            index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Right.X, (int)NPC.Right.Y, ModContent.NPCType<SlimeAvatar>(), ai1: NPC.ai[1], ai2: NPC.ai[2] + 1, Target: NPC.target);
                            Main.npc[index].lifeMax = Main.npc[index].life = NPC.lifeMax * 3 / 4;
                            Main.npc[index].width = NPC.width * 2 / 3;
                            Main.npc[index].height = NPC.height * 2 / 3;
                            Main.npc[index].scale = NPC.scale * 2 / 3;
                            Main.npc[index].netUpdate = true;
                        }

                        NPC.Kill();
                    }
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
                                if (NPC.Center.Y < (Main.player[NPC.target].Center.Y - 100)) //比玩家高，那么只判断实心物块
                                    for (int i = 0; i < NPC.width; i += 16)
                                    {
                                        Tile tile = Framing.GetTileSafely(NPC.BottomLeft + new Vector2(i, 0));
                                        if (tile.HasTile && Main.tileSolid[tile.TileType])
                                        {
                                            NPC.frame.Y = 0;
                                            NPC.noGravity = true;
                                            JumpTimer = -1;
                                            break;
                                        }
                                    }
                                else    //比玩家低，那么要判断其他顶部实心的物块
                                    for (int i = 0; i < NPC.width; i += 16)
                                    {
                                        Tile tile = Framing.GetTileSafely(NPC.BottomLeft + new Vector2(i, 0));
                                        if (tile.HasTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] || TileID.Sets.Platforms[tile.TileType]))
                                        {
                                            NPC.frame.Y = 0;
                                            NPC.noGravity = true;
                                            JumpTimer = -1;
                                            break;
                                        }
                                    }
                            }

                            break;
                        case -1: //刚落地之后的变扁阶段
                            NPC.velocity *= 0f;
                            Scale = Vector2.Lerp(Scale, new Vector2(1.35f, 0.85f), 0.15f);
                            if (Scale.X > 1.3f)
                            {
                                JumpTimer = -2;
                            }
                            break;

                        case -2://回弹
                            Scale = Vector2.Lerp(Scale, new Vector2(0.75f, 1.25f), 0.15f);
                            if (Scale.X < 0.8f)
                            {
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
                                NPC.frame.Y = 0;
                                JumpTimer = 0;
                            }
                            break;
                    }

                    break;
                case (int)JumpStates.ReadyToJump:  //起跳准备的阶段
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter > 20)
                        {
                            NPC.frame.Y++;
                            NPC.frameCounter = 0;
                        }

                        Scale = Vector2.Lerp(Scale, new Vector2(1.25f, 0.85f), 0.1f);

                        if (NPC.frame.Y == 1)
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
                    float targetScaleX = Math.Clamp(1 - jumpYVelocity / 15, 0.75f, 1f);
                    Scale = Vector2.Lerp(Scale, new Vector2(targetScaleX, 1.2f), 0.15f);

                    if (JumpTimer > 8)
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
            }

            if (NPC.life <= 0)
                SoundEngine.PlaySound(CoraliteSoundID.Fleshy_NPCDeath1, NPC.Center);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(xVel);
            writer.Write(yVel);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            xVel = reader.ReadSingle();
            yVel = reader.ReadSingle();
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            Rectangle frameBox = mainTex.Frame(1, Main.npcFrameCount[Type], 0, NPC.frame.Y);
            Vector2 origin = new Vector2(frameBox.Width / 2, frameBox.Height);
            Vector2 scale = Scale * NPC.scale;

            if (Main.zenithWorld)
                drawColor = SlimeEmperor.BlackSlimeColor;

            spriteBatch.Draw(mainTex, NPC.Bottom + new Vector2(0, 4) - screenPos, frameBox, drawColor, NPC.rotation, origin, scale, 0, 0f);
            return false;
        }


        private enum JumpStates
        {
            CheckForLanding = 0,
            ReadyToJump = 1,
            Jumping = 2,
        }
    }
}
