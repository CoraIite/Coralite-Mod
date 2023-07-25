using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    /// <summary>
    /// 史莱姆皇帝，加强版史莱姆王
    /// 
    /// “又是这个囊地过分的波斯”
    /// “300颗够吗，应该够了吧”
    /// “来吧，试一下米妮”
    /// “诶呀，亡了亡了，我没有史莱姆ang啊”
    /// 
    /// 粘滑生物真正的领袖，史莱姆王？不过是个小弟罢了
    ///                                            /\
    ///                                | \      /      \      /|
    ///                                |    \/           \/    |
    ///                                |            ◇           |
    ///                          ——————————
    ///                      /                                         \
    ///                   /              □               □             \
    ///                 /                                                   \
    ///               |                                □                     |
    ///                 \                                                  /
    ///                     ————————————
    /// </summary>
   // [AutoloadBossHead]
    public partial class SlimeEmperor : ModNPC
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        private Player Target => Main.player[NPC.target];

        /// <summary> 子状态，用于简化AI书写 </summary>
        internal ref float SonState => ref NPC.ai[0];
        /// <summary> 当前进行到哪个阶段的AI </summary>
        internal ref float MovePhase => ref NPC.ai[1];
        internal ref float State => ref NPC.ai[2];
        /// <summary> 移动方式 </summary>
        internal ref float MovingMode => ref NPC.ai[3];

        /// <summary> 这个不会同步！ </summary>
        internal ref float JumpState => ref NPC.localAI[0];
        internal ref float JumpTimer => ref NPC.localAI[1];

        private float LifePercentScale => Math.Clamp(NPC.life / (float)NPC.lifeMax, 0.5f, 1);

        internal int shoot2State;
        internal int melee2State;

        private bool CanDrawShadow;
        private bool CanUseHealGelBall = true;

        internal int Timer { get; private set; }
        /// <summary> 纯属视觉效果的缩放 </summary>
        internal Vector2 Scale;
        private CrownDatas crown;

        private static Asset<Texture2D> CrownTex;
        private const int WidthMax = 158;
        private const int HeightMax = 100;

        #region tml hooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.GravityMultiplier *= 2f;
            NPC.width = 60;
            NPC.height = 85;
            NPC.scale = 1.5f;
            NPC.damage = 30;
            NPC.defense = 6;
            NPC.lifeMax = 3000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 20f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);

            NPC.noGravity = false;
            NPC.noTileCollide = true;
            NPC.boss = true;

            //NPC.BossBar = GetInstance<RediancieBossBar>();

            //BGM：暂无
            //if (!Main.dedServ)
            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/？？？");

        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(1800 * bossAdjustment) + numPlayers * 450;
            NPC.damage = 30;
            NPC.defense = 6;

            if (Main.masterMode)
            {
                NPC.scale *= 1.5f;
                NPC.lifeMax = (int)(2000 * bossAdjustment) + numPlayers * 550;
                NPC.damage = 45;
            }

            if (Main.getGoodWorld)
            {
                NPC.scale *= 2f;
                NPC.lifeMax = (int)(2300 * bossAdjustment) + numPlayers * 600;
                NPC.defense = 4;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<RediancieRelic>()));
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<RedianciePet>(), 4));
            //npcLoot.Add(ItemDropRule.BossBag(ItemType<RediancieBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));

            //LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<RedJade>(), 1, 20, 24));
            //npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
            {
            }

        }

        public override void Load()
        {
            CrownTex = Request<Texture2D>(AssetDirectory.SlimeEmperor + "SlimeEmperorCrown");
        }

        public override void Unload()
        {
            CrownTex = null;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            //SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            OnHit(hit.Damage);
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            OnHit(hit.Damage);
        }

        public void OnHit(int damage)
        {
        }

        public override bool CheckDead()
        {
            if (State != (int)AIStates.OnKillAnim)
            {
                State = (int)AIStates.OnKillAnim;
                Timer = 0;
                NPC.dontTakeDamage = true;
                NPC.life = 1;
                return false;
            }

            return true;
        }

        #endregion

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            CanUseHealGelBall = true;
            Scale = Vector2.One;
            crown = new CrownDatas();
            crown.Bottom = NPC.Top + new Vector2(0, -50);
            NPC.TargetClosest(false);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                State = (int)AIStates.OnSpawnAnim;
                // NPC.Center = Target.Center - new Vector2(0, 600);
                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)//没有玩家存活时离开
                {
                    Jump(8f, 12f);
                    NPC.EncourageDespawn(10);
                    return;
                }
            }

            switch ((int)State)
            {
                case (int)AIStates.OnKillAnim:
                    {

                    }
                    break;
                case (int)AIStates.OnSpawnAnim:
                    {
                        ResetStates();
                    }
                    break;
                default:
                case (int)AIStates.GelShoot:
                    GelShoot();
                    break;
                case (int)AIStates.CrownStrike:
                    CrownStrike();
                    break;
                case (int)AIStates.SpikeGelBall:
                    SpikeGelBall();
                    break;
                case (int)AIStates.PolymerizeShot:
                    break;
                case (int)AIStates.BodySlam:
                    break;
                case (int)AIStates.Split:
                    break;
                case (int)AIStates.GelFlippy:
                    break;
                case (int)AIStates.StickyGel:
                    break;
                case (int)AIStates.TransportSplit:
                    break;
                case (int)AIStates.MiniJump:
                    ThreeMiniJump();
                    break;
                case (int)AIStates.BigJump:
                    {
                        switch ((int)SonState)
                        {
                            default:
                            case 0:
                                Jump(10, 10, () => SonState++);
                                break;
                            case 1:
                                ResetStates();
                                break;
                        }
                    }
                        break;
                case (int)AIStates.HealGelBall:
                    break;
            }
        }

        //在这里单独更新王冠
        //以及更新NPC的位置
        public override void PostAI()
        {
            switch ((int)MovingMode)
            {
                default:
                case (int)MovingModeID.Normal:
                    {
                        int newWidth = (int)(LifePercentScale *NPC.scale* WidthMax);
                        int newHeight = (int)(LifePercentScale *NPC.scale* HeightMax);
                        if (NPC.width != newWidth || NPC.height != newHeight)
                        {
                            Vector2 bottom = NPC.Bottom;
                            NPC.width = newWidth;
                            NPC.height = newHeight;
                            NPC.Bottom = bottom;
                        }

                        int height = (int)(LifePercentScale * NPC.scale * 86);
                        float groundHeight = NPC.Bottom.Y - Scale.Y * height;
                        crown.Bottom.X = MathHelper.Lerp(crown.Bottom.X, NPC.Center.X, 0.3f);

                        if (crown.Bottom.Y < groundHeight - 16) //重力
                        {
                            crown.Velocity_Y += NPC.gravity;
                            if (crown.Velocity_Y > 18)
                                crown.Velocity_Y = 18;
                        }

                        crown.Bottom.Y += crown.Velocity_Y;     //更新位置
                        if (crown.Bottom.Y > groundHeight)    //如果超过了地面那么就进行判断
                        {
                            crown.Bottom.Y = groundHeight;  //将位置拉回
                            if (NPC.velocity.Y < 0.5f && crown.Velocity_Y > 12)  //速度很大，向上弹起
                            {
                                //随机一个角度
                                float angle = Math.Clamp(crown.Velocity_Y / 40, 0, 0.35f);
                                crown.Rotation = Main.rand.NextFloat(-angle, angle);

                                crown.Velocity_Y *= -0.4f;
                            }
                            else
                                crown.Velocity_Y *= 0;  //速度不够直接停止
                        }

                        crown.Rotation = crown.Rotation.AngleLerp(0, 0.04f);
                    }

                    break;
                case (int)MovingModeID.Crown:
                    NPC.width = NPC.height = 68;

                    crown.Rotation += 0.3f;
                    break;
            }
        }

        #endregion

        #region States

        private enum MovingModeID
        {
            Normal = 0,
            Crown = 1
        }

        private enum AIStates : int
        {
            OnKillAnim = -2,
            OnSpawnAnim = -1,

            /// <summary> 凝胶射击 </summary>
            GelShoot = 0,
            /// <summary> 王冠冲击 </summary>
            CrownStrike = 1,
            /// <summary> 尖刺凝胶球 </summary>
            SpikeGelBall = 2,
            /// <summary> 聚合射击 </summary>
            PolymerizeShot = 3,
            /// <summary> 泰山压顶 </summary>
            BodySlam = 4,
            /// <summary> 分裂 </summary>
            Split = 5,

            /// <summary> 凝胶僚机 </summary>
            GelFlippy = 6,
            /// <summary> 黏黏凝胶 </summary>
            StickyGel = 7,
            /// <summary> 移位分裂 </summary>
            TransportSplit = 8,

            /// <summary> 小跳步 </summary>
            MiniJump = 9,
            /// <summary> 大跳 </summary>
            BigJump = 10,

            /// <summary> 回血球 </summary>
            HealGelBall = 11
        }

        private enum AIPhases
        {
            MiniJump = 0,
            Shoot1 = 1,
            Melee1 = 2,
            BigJump = 3,
            Shoot2 = 4,
            Melee2 = 5,
        }

        public void ResetStates()
        {
            CanDrawShadow = false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (CanUseHealGelBall &&
                NPC.life / (float)NPC.lifeMax < 0.30f)      //首次到达30%血量以下时使用回血球
            {
                State = (int)AIStates.HealGelBall;
                CanUseHealGelBall = false;
                goto ResetProprieties;
            }

            if (Main.getGoodWorld)
                FTWSetState();
            else
                NormallySetState();

            ResetProprieties:

            State = (int)AIStates.MiniJump;

            NPC.dontTakeDamage = false;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            Timer = 0;
            SonState = 0;
            NPC.TargetClosest(false);
            NPC.netUpdate = true;
            StartJump();
        }

        private void NormallySetState()
        {
            switch (MovePhase)
            {
                default:
                case (int)AIPhases.MiniJump:
                    State = (int)AIStates.MiniJump;
                    break;

                case (int)AIPhases.Shoot1:
                    if (Main.masterMode)
                        State = Main.rand.Next(2) switch
                        {
                            0 => (int)AIStates.GelShoot,
                            _ => (int)AIStates.GelFlippy
                        };
                    else
                        State = (int)AIStates.GelShoot;
                    break;

                case (int)AIPhases.Melee1:
                    State = (int)AIStates.CrownStrike;
                    break;

                case (int)AIPhases.BigJump:
                    State = (int)AIStates.BigJump;
                    break;

                case (int)AIPhases.Shoot2:
                    if (Main.masterMode)
                        State = shoot2State switch
                        {
                            0 => (int)AIStates.SpikeGelBall,
                            1 => (int)AIStates.StickyGel,
                            _ => (int)AIStates.PolymerizeShot
                        };
                    else
                        State = shoot2State switch
                        {
                            0 => (int)AIStates.SpikeGelBall,
                            1 => (int)AIStates.SpikeGelBall,
                            _ => (int)AIStates.PolymerizeShot
                        };

                    shoot2State++;
                    if (shoot2State > 2)
                        shoot2State = 0;
                    break;

                case (int)AIPhases.Melee2:
                    if (Main.masterMode)
                        State = shoot2State switch
                        {
                            0 => (int)AIStates.Split,
                            1 => (int)AIStates.TransportSplit,
                            _ => (int)AIStates.BodySlam
                        };
                    else
                        State = shoot2State switch
                        {
                            0 => (int)AIStates.Split,
                            1 => (int)AIStates.Split,
                            _ => (int)AIStates.BodySlam
                        };

                    melee2State++;
                    if (melee2State > 2)
                        melee2State = 0;
                    break;
            }

            MovePhase++;
            if (MovePhase > 5)
                MovePhase = 0;
        }

        private void FTWSetState()
        {

        }

        /// <summary>
        /// 变为王冠状态
        /// </summary>
        private void CrownMode()
        {
            MovingMode = (int)MovingModeID.Crown;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.defense = NPC.defDefense * 4;
        }

        /// <summary>
        /// 切换为普通状态
        /// </summary>
        private void SlimeMode()
        {
            MovingMode = (int)MovingModeID.Normal;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.defense = NPC.defDefense;

            crown.Rotation = 0;
        }

        #endregion

        #region NetWork

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(shoot2State);
            writer.Write(melee2State);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            shoot2State = reader.ReadInt32();
            melee2State = reader.ReadInt32();
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Texture2D crownTex = CrownTex.Value;

            Rectangle frameBox = mainTex.Frame(1, Main.npcFrameCount[Type], 0, NPC.frame.Y);
            Vector2 origin = new Vector2(frameBox.Width / 2, frameBox.Height);

            bool canDrawSelf = true;
            Vector2 crownOrigin;
            Vector2 crownPos;

            switch ((int)MovingMode)
            {
                default:
                case (int)MovingModeID.Normal:
                    crownOrigin = new Vector2(crownTex.Width / 2, crownTex.Height);
                    crownPos = crown.Bottom - screenPos;
                    break;

                case (int)MovingModeID.Crown:
                    crownOrigin = crownTex.Size() / 2;
                    crownPos = NPC.Center - screenPos;
                    canDrawSelf = false;
                    break;
            }

            if (canDrawSelf)            //绘制本体，以底部为中心进行绘制
                spriteBatch.Draw(mainTex, NPC.Bottom+new Vector2(0,4) - screenPos, frameBox, drawColor, NPC.rotation, origin, Scale, 0, 0f);

            //绘制王冠
            spriteBatch.Draw(crownTex, crownPos, null, drawColor, crown.Rotation, crownOrigin, NPC.scale, 0, 0f);

            return false;
        }

        #endregion



        private struct CrownDatas
        {
            public Vector2 Bottom;
            public float Velocity_Y;
            public float Rotation;
        }
    }
}
