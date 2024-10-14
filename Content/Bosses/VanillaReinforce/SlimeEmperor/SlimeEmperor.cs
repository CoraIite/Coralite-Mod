using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    /// <summary>
    /// 史莱姆皇帝，加强版史莱姆王
    /// 
    /// “蚩尤说：又是这个囊地过分的波斯”
    /// “300颗够吗，应该够了吧”
    /// “这个武器打这个BOSS，从来没试过哦”
    /// “这个波斯对我来说超囊的”
    /// “来吧，试一下米妮”
    /// “诶呀，亡了亡了，我没有史莱姆ang啊”
    /// 
    ///    591 60 15 3
    /// 
    /// 粘滑生物真正的领袖，史莱姆王？不过是个小弟罢了
    ///                                     /\
    ///                                |\  /  \  /|
    ///                                | \/    \/ |
    ///                                |    ◇     |
    ///                                 ——————————
    ///                             /                \
    ///                           /      □       □     \
    ///                         /                        \
    ///                        |                □        |
    ///                         \                       /
    ///                            ---————————————----
    /// </summary>
    [AutoloadBossHead]
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

        private float LifePercentScale => Math.Clamp(NPC.life / (float)NPC.lifeMax, 0.65f, 1);

        internal int shoot2State;
        internal int melee2State;

        private bool CanDrawShadow;
        //private bool CanUseHealGelBall = true;

        internal int Timer { get; private set; }
        /// <summary> 纯属视觉效果的缩放 </summary>
        internal Vector2 Scale;
        private CrownDatas crown;

        private static Asset<Texture2D> CrownTex;
        private const int WidthMax = 158;
        private const int HeightMax = 100;

        public static Color BlackSlimeColor = Color.Black;

        #region tml hooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 12;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.GravityMultiplier *= 2f;
            NPC.width = 60;
            NPC.height = 85;
            NPC.scale = 1.5f;
            NPC.damage = 40;
            NPC.defense = 8;
            NPC.lifeMax = 6100;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 20f;
            NPC.value = Item.buyPrice(0, 4, 0, 0);
            NPC.HitSound = CoraliteSoundID.Fleshy_NPCHit1;

            NPC.noGravity = false;
            NPC.noTileCollide = true;
            NPC.boss = true;
            //NPC.hide = true;
            //NPC.BossBar = GetInstance<RediancieBossBar>();

            //BGM：暂无
            //if (!Main.dedServ)
            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/？？？");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((5600 + (numPlayers * 1460)) / journeyScale);
                    NPC.damage = 75;
                    NPC.defense = 10;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((6200 + (numPlayers * 3030)) / journeyScale);
                    NPC.scale *= 1.25f;
                    NPC.defense = 14;
                    NPC.damage = 100;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 120;
                    NPC.scale *= 1.25f;
                    NPC.defense = 16;
                }

                return;
            }

            NPC.lifeMax = 5600 + (numPlayers * 1460);
            NPC.damage = 75;
            NPC.defense = 10;

            if (Main.masterMode)
            {
                NPC.lifeMax = 6200 + (numPlayers * 3030);
                NPC.scale *= 1.25f;
                NPC.defense = 14;
                NPC.damage = 100;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 6800 + (numPlayers * 4060);
                NPC.damage = 140;
                NPC.scale *= 1.25f;
                NPC.defense = 16;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<GelThrone>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<SovereignSip>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<SlimeEmperorSoulBox>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<RoyalGelCannon>(), 10));
            npcLoot.Add(ItemDropRule.Common(ItemType<SlimeEmperorMask>(), 7));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            IItemDropRule[] weaponTypes = [
                ItemDropRule.Common(ItemType<SlimeEruption>(), 1, 1, 1),
                ItemDropRule.Common(ItemType<GelWhip>(), 1, 1, 1),
                ItemDropRule.Common(ItemType<RoyalClassics>(), 1, 1, 1),
                ItemDropRule.Common(ItemType<SlimeSceptre>(), 1, 1, 1),
            ];

            notExpertRule.OnSuccess(new OneFromRulesRule(1, weaponTypes));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.Gel, 1, 30, 100));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<EmperorGel>(), 1, 15, 30));

            npcLoot.Add(notExpertRule);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if ((projectile.penetrate < 0 || projectile.penetrate > 1) && modifiers.DamageType != DamageClass.Melee)
                modifiers.SourceDamage *= 0.75f;

        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            CrownTex = Request<Texture2D>(AssetDirectory.SlimeEmperor + "SlimeEmperorCrown");
            //王冠gore
            GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.SlimeEmperor + "SlimeEmperorCrown");
        }

        public override void Unload()
        {
            CrownTex = null;
        }

        public override void OnKill()
        {
            DownedBossSystem.DownSlimeEmperor();
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

        public override bool? CanFallThroughPlatforms() => NPC.Center.Y < (Target.Center.Y - NPC.height);

        #endregion

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            //CanUseHealGelBall = true;
            PolymerizeTime = Helper.ScaleValueForDiffMode(240, 240, 150, 60);
            Scale = Vector2.One;
            crown = new CrownDatas();
            crown.Bottom = NPC.Top + new Vector2(0, -50);
            NPC.TargetClosest(false);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                State = (int)AIStates.BodySlam;
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
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.Y -= 0.3f;
                    CanDrawShadow = true;
                    NPC.EncourageDespawn(10);
                    return;
                }
                else
                    ResetStates();
            }

            switch ((int)State)
            {
                case (int)AIStates.OnKillAnim:
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            //生成王冠gore
                            Gore gore = Gore.NewGoreDirect(NPC.GetSource_Death(), crown.Bottom, Main.rand.NextVector2Circular(1, 1), Mod.Find<ModGore>("SlimeEmperorCrown").Type);
                            gore.scale = NPC.scale;
                        }

                        NPC.Kill();
                    }
                    break;
                case (int)AIStates.OnSpawnAnim:
                    {
                        ResetStates();
                    }
                    break;
                default:
                case (int)AIStates.GelShoot:
                    GelShoot();   //√
                    break;
                case (int)AIStates.CrownStrike:
                    CrownStrike();   //√
                    break;
                case (int)AIStates.SpikeGelBall:
                    SpikeGelBall();   //√
                    break;
                case (int)AIStates.PolymerizeShot:
                    PolymerizeShot();
                    break;
                case (int)AIStates.BodySlam:
                    BodySlam();  //√
                    break;
                case (int)AIStates.Split:
                    Split();  //√
                    break;
                case (int)AIStates.GelFlippy:
                    GelFlippy(); //√
                    break;
                case (int)AIStates.StickyGel:
                    StickyGel();
                    break;
                case (int)AIStates.TransportSplit:
                    TransportSplit(); //√
                    break;
                case (int)AIStates.MiniJump:
                    ThreeMiniJump();   //√
                    break;
                case (int)AIStates.BigJump:
                    {
                        switch ((int)SonState)
                        {
                            default:
                            case 0:
                                Jump(3f, 10, () => SonState++,
                                    () =>
                                    {
                                        if (Main.getGoodWorld && Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            for (int i = 0; i < 4; i++)
                                            {
                                                Vector2 vel = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 10;
                                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 3, NPC.height / 3), vel, ModContent.ProjectileType<SpikeGelBall>(),
                                                    20, 4f, NPC.target);
                                            }
                                        }
                                    },
                                    onStartJump: () =>
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            int howMany = Helper.ScaleValueForDiffMode(1, 2, 4, 6);
                                            for (int i = 0; i < howMany; i++)
                                            {
                                                Point pos = NPC.Center.ToPoint();
                                                pos.X += Main.rand.Next(-NPC.width, NPC.width);
                                                pos.Y += Main.rand.Next(-32, 32);
                                                NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromAI(), pos.X, pos.Y, NPCType<ElasticGelBall>());
                                                npc.velocity = -Vector2.UnitY * Main.rand.NextFloat(2, 5);
                                            }
                                        }
                                    });
                                break;
                            case 1:
                                ResetStates();
                                break;
                        }
                    }
                    break;   //√
                case (int)AIStates.HealGelBall:
                    ResetStates();
                    break;
            }
        }

        //在这里单独更新王冠
        //以及更新NPC的位置
        public override void PostAI()
        {
            if (Main.zenithWorld)
            {
                BlackSlimeColor = Color.Lerp(new Color(25, 25, 25, 200), new Color(110, 60, 100, 50),
                    (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) / 2);
            }

            switch ((int)MovingMode)
            {
                default:
                case (int)MovingModeID.Normal:
                    {
                        int newWidth = (int)(LifePercentScale * NPC.scale * WidthMax);
                        int newHeight = (int)(LifePercentScale * NPC.scale * HeightMax);
                        if (NPC.width != newWidth || NPC.height != newHeight)
                        {
                            Vector2 bottom = NPC.Bottom;
                            NPC.width = newWidth;
                            NPC.height = newHeight;
                            NPC.Bottom = bottom;
                        }

                        int height = GetCrownBottom();
                        float groundHeight = NPC.Bottom.Y - (Scale.Y * height);
                        crown.Bottom.X = MathHelper.Lerp(crown.Bottom.X, NPC.Center.X, 0.5f);

                        if (crown.Bottom.Y < groundHeight - 2) //重力
                        {
                            crown.Velocity_Y += NPC.gravity * 1.25f;
                            if (crown.Velocity_Y > 16)
                                crown.Velocity_Y = 16;
                        }

                        crown.Bottom.Y += crown.Velocity_Y;     //更新位置
                        if (crown.Bottom.Y > groundHeight)    //如果超过了地面那么就进行判断
                        {
                            crown.Bottom.Y = groundHeight;  //将位置拉回
                            if (NPC.velocity.Y < 0.5f && crown.Velocity_Y > 10)  //速度很大，向上弹起
                            {
                                //随机一个角度
                                float angle = Math.Clamp(crown.Velocity_Y / 40, 0.1f, 0.55f);
                                crown.Rotation = Main.rand.NextFloat(-angle, angle);

                                crown.Velocity_Y *= -0.1f;
                            }
                            else
                                crown.Velocity_Y = NPC.velocity.Y;  //速度不够直接停止
                        }

                        crown.Rotation = crown.Rotation.AngleLerp(0, 0.04f);
                    }

                    break;
                case (int)MovingModeID.Crown:
                    crown.Rotation += 0.3f;
                    break;
            }
        }

        private int GetCrownBottom()
        {
            int frameBaseHeight = NPC.frame.Y switch
            {
                0 => 80,  //108
                1 => 80 + 6,  //114
                2 => 80,  //108
                3 => 80 - 6,  //100
                4 => 80 - 4,  //104
                _ => 80 - 4  //98
            };

            return (int)(LifePercentScale * NPC.scale * Scale.Y * frameBaseHeight);
        }

        private void CrownJumpUp(float velLimit, float JumpUpSpeed)
        {
            if (Math.Abs(crown.Velocity_Y) < velLimit)
                crown.Velocity_Y -= JumpUpSpeed;

            float angle = Math.Clamp(crown.Velocity_Y / 40, 0.1f, 0.55f);
            crown.Rotation = Main.rand.NextFloat(-angle, angle);
        }

        private void ScaleToTarget(float targetX, float targetY, float amount, bool whenToStop, Action OnStop)
        {
            Scale = Vector2.Lerp(Scale, new Vector2(targetX, targetY), amount);

            if (whenToStop)
                OnStop.Invoke();
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

        private enum NormalAIPhases
        {
            MiniJump = 0,
            Shoot1 = 1,
            Melee1 = 2,
            BigJump = 3,
            Shoot2 = 4,
            Melee2 = 5,
        }

        private enum FTWAIPhases
        {
            Shoot1 = 0,
            Melee1 = 1,
            Jump = 2,
            Shoot2 = 3,
            Melee2 = 4,
        }

        public void ResetStates()
        {
            CanDrawShadow = false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            //if (CanUseHealGelBall &&
            //    NPC.life / (float)NPC.lifeMax < 0.30f)      //首次到达30%血量以下时使用回血球
            //{
            //    State = (int)AIStates.HealGelBall;
            //    CanUseHealGelBall = false;
            //    goto ResetProprieties;
            //}

            if (Main.getGoodWorld)
                FTWSetState();
            else
                NormallySetState();

            //ResetProprieties:

            //State = (int)AIStates.StickyGel;

            NPC.dontTakeDamage = false;
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            Timer = 0;
            SonState = 0;
            NPC.TargetClosest();
            NPC.netUpdate = true;
            StartJump();
        }

        private void NormallySetState()
        {
            switch (MovePhase)
            {
                default:
                case (int)NormalAIPhases.MiniJump:
                    State = (int)AIStates.MiniJump;
                    break;

                case (int)NormalAIPhases.Shoot1:
                    if (Main.masterMode)
                        State = Main.rand.Next(2) switch
                        {
                            0 => (int)AIStates.GelShoot,
                            _ => (int)AIStates.GelFlippy
                        };
                    else
                        State = (int)AIStates.GelShoot;
                    break;

                case (int)NormalAIPhases.Melee1:
                    State = (int)AIStates.CrownStrike;
                    break;

                case (int)NormalAIPhases.BigJump:
                    State = (int)AIStates.BigJump;
                    break;

                case (int)NormalAIPhases.Shoot2:
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

                case (int)NormalAIPhases.Melee2:
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
            switch (MovePhase)
            {
                default:
                case (int)FTWAIPhases.Shoot1:
                    if (Main.masterMode)
                        State = Main.rand.Next(2) switch
                        {
                            0 => (int)AIStates.GelShoot,
                            _ => (int)AIStates.GelFlippy
                        };
                    else
                        State = (int)AIStates.GelShoot;
                    break;

                case (int)FTWAIPhases.Melee1:
                    State = Main.rand.Next(2) switch
                    {
                        0 => (int)AIStates.CrownStrike,
                        _ => (int)AIStates.BodySlam
                    };
                    break;

                case (int)FTWAIPhases.Jump:
                    State = Main.rand.Next(2) switch
                    {
                        0 => (int)AIStates.MiniJump,
                        _ => (int)AIStates.BigJump
                    };
                    break;

                case (int)NormalAIPhases.Shoot2:
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

                case (int)NormalAIPhases.Melee2:
                    if (Main.masterMode)
                        State = shoot2State switch
                        {
                            0 => (int)AIStates.TransportSplit,
                            _ => (int)AIStates.Split
                        };
                    else
                        State = (int)AIStates.Split;

                    melee2State++;
                    if (melee2State > 1)
                        melee2State = 0;
                    break;
            }

            MovePhase++;
            if (MovePhase > 4)
                MovePhase = 0;
        }

        /// <summary>
        /// 变为王冠状态
        /// </summary>
        private void CrownMode()
        {
            MovingMode = (int)MovingModeID.Crown;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.defense = NPC.defDefense + 40;

            Vector2 center = NPC.Center;
            NPC.width = NPC.height = (int)(68 * NPC.scale);
            NPC.Center = center;
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

            crown.Velocity_Y *= 0;
            crown.Rotation = Main.rand.NextFloat(MathHelper.Pi + (MathHelper.Pi / 4), MathHelper.TwoPi - (MathHelper.Pi / 4)) + (MathHelper.Pi / 2);
            crown.Bottom = NPC.Top;
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

        //public override void DrawBehind(int index)
        //{
        //    Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        //}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Texture2D crownTex = CrownTex.Value;

            Rectangle frameBox = mainTex.Frame(1, Main.npcFrameCount[Type], 0, NPC.frame.Y);
            Vector2 origin = new(frameBox.Width / 2, frameBox.Height);

            Vector2 crownOrigin;
            Vector2 crownPos;

            if (Main.zenithWorld)
                drawColor = BlackSlimeColor;

            switch ((int)MovingMode)
            {
                default:
                case (int)MovingModeID.Normal:
                    crownOrigin = new Vector2(crownTex.Width / 2, crownTex.Height);
                    crownPos = crown.Bottom - screenPos;
                    //绘制本体，以底部为中心进行绘制
                    Vector2 scale = Scale * NPC.scale * LifePercentScale;
                    Vector2 offset = new Vector2(0, 4 * scale.Y) - Main.screenPosition;
                    spriteBatch.Draw(mainTex, NPC.Bottom + offset, frameBox, drawColor, NPC.rotation, origin, scale, 0, 0f);
                    if (CanDrawShadow)
                    {
                        Vector2 toBottom = new(NPC.width / 2, NPC.height);
                        for (int i = 1; i < 12; i += 2)
                        {
                            spriteBatch.Draw(mainTex, NPC.oldPos[i] + toBottom + offset, frameBox, drawColor * (0.4f - (i * 0.04f)), NPC.rotation, origin, scale, 0, 0f);
                        }
                    }

                    break;

                case (int)MovingModeID.Crown:
                    crownOrigin = crownTex.Size() / 2;
                    crownPos = NPC.Center - screenPos;
                    break;
            }

            //绘制王冠
            spriteBatch.Draw(crownTex, crownPos, null, drawColor, crown.Rotation, crownOrigin, NPC.scale, 0, 0f);

            return false;
        }

        #endregion

        public struct CrownDatas
        {
            public Vector2 Bottom;
            public float Velocity_Y;
            public float Rotation;
        }
    }
}
