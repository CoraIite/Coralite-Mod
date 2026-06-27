using Coralite.Content.Items.Thunder;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    [AutoloadBossHead]
    public partial class ThunderveinDragon : ModNPC
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + Name;

        private Player Target => Main.player[NPC.target];
        private bool spwan;

        // 迁移到 InnoVault 状态机基座后的 ai 槽约定：
        // ai[0]=顶层招式状态ID（=AIStates 数值，由 AiSlotNetSync 经 CoraliteBossStateMachine 同步）
        // ai[1]=AttackSeed（基座），ai[2]=SonState（基座），ai[3]=Timer（基座 SyncTimer）
        internal ref float State => ref NPC.ai[0];
        internal ref float SonState => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];

        // 阶段不再占用 ai[0]，改用后备字段承载并经 SendExtraAI 同步
        private float phaseValue = 1;
        internal float Phase { get => phaseValue; set => phaseValue = value; }

        internal ref float Recorder => ref NPC.localAI[0];
        internal ref float Recorder2 => ref NPC.localAI[1];
        internal ref float StateRecorder => ref NPC.localAI[2];
        internal ref float UseMoveCount => ref NPC.localAI[3];

        internal ThunderveinDragonContext AiContext;
        internal CoraliteBossStateMachine<ThunderveinDragonContext> StateMachine;
        internal System.Random AttackRandom;
        private bool aiBootstrapped;

        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)AIStates.onSpawnAnmi;

        public int hitCount;

        public readonly int trailCacheLength = 12;
        public Point[] oldFrame;
        public int[] oldDirection;

        public float selfAlpha = 1f;
        public float anmiAlpha;

        public static Color ThunderveinYellowAlpha = new(255, 202, 101, 0);
        public static Color ThunderveinPurpleAlpha = new(135, 94, 255, 0);
        public static Color ThunderveinOrangeAlpha = new(219, 114, 22, 0);

        public static Color ThunderveinYellow = new(255, 202, 101);
        public static Color ThunderveinPurple = new(135, 94, 255);
        public static Color ThunderveinOrange = new(219, 114, 22);

        /// <summary>
        /// 是否绘制残影
        /// </summary>
        public bool canDrawShadows;
        /// <summary>
        /// 是否绘制冲刺是的特殊贴图
        /// </summary>
        public bool isDashing;

        /// <summary>
        /// 残影的透明度
        /// </summary>
        public float shadowAlpha = 1f;
        /// <summary>
        /// 残影的大小
        /// </summary>
        public float shadowScale = 1f;

        public int oldSpriteDirection;

        /// <summary>
        /// 身上有电流环绕，会减伤并生成闪电粒子
        /// </summary>
        public bool currentSurrounding;

        public bool Initialize = true;

        #region tmlHooks

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phaseValue);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phaseValue = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 100;
            NPC.damage = 60;
            NPC.defense = 50;
            NPC.lifeMax = 45500;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 12, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = ModContent.GetInstance<ThunderveinDragonBossBar>();
            ModContent.GetInstance<ThunderveinDragonBossBar>().Reset(NPC);
            InitOldFrame();

            //BGM：雷龙
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ThunderDragon");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((39000 + (numPlayers * 15500)) / journeyScale);
                    NPC.damage = 66;
                    NPC.defense = 50;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((45500 + (numPlayers * 19550)) / journeyScale);
                    NPC.damage = 72;
                    NPC.defense = 50;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 50;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 2.4f;
                }

                return;
            }

            NPC.lifeMax = 39000 + (numPlayers * 15500);
            NPC.damage = 66;
            NPC.defense = 50;

            if (Main.masterMode)
            {
                NPC.lifeMax = 45500 + (numPlayers * 19550);
                NPC.damage = 72;
                NPC.defense = 50;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 52000 + (numPlayers * 25850);
                NPC.damage = 80;
                NPC.defense = 50;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 2.4f;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ThunderveinDragonRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<ThunderveinSoulStone>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ThunderveinDragonBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThunderveinDragonTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ThunderveinDragonMask>(), 7));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ZapCrystal>(), 1, 6, 8));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ElectrificationWing>(), 1, 3, 4));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<InsulationCortex>(), 1, 8, 12));
            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool? CanCollideWithPlayerMeleeAttack(Player player, Item item, Rectangle meleeAttackHitbox)
        {
            return base.CanCollideWithPlayerMeleeAttack(player, item, meleeAttackHitbox);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (currentSurrounding)
                modifiers.SourceDamage -= 0.4f;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Colliding(projectile.getRect(), HeadHitBox()))
                modifiers.SourceDamage += 0.15f;

            if (currentSurrounding)
                modifiers.SourceDamage -= 0.4f;

            if (projectile.hostile)
                modifiers.SourceDamage -= 0.5f;
        }

        public Rectangle HeadHitBox()
        {
            Vector2 pos = GetMousePos();
            return new Rectangle((int)(pos.X - 22), (int)(pos.Y - 22), 44, 44);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                PRTLoader.NewParticle(NPC.Center.MoveTowards(projectile.Center, 50), Vector2.Zero,
                    CoraliteContent.ParticleType<LightningParticle>(), Scale: Main.rand.NextFloat(1f, 1.5f));
            }

            //残血，没有在带电状态，受到暴击时掉落绝缘壳
            if (NPC.life < NPC.lifeMax / 6 && hitCount < 6 && !currentSurrounding
                && hit.Crit && Main.rand.NextBool(3))
            {
                Item.NewItem(NPC.GetSource_OnHit(projectile), NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height)
                    , ModContent.ItemType<InsulationCortex>());
                hitCount++;
            }
        }

        public override void OnKill()
        {
            DownedBossSystem.DownThunderveinDragon();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.StopRain();
                Main.SyncRain();
            }
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override bool CheckDead()
        {
            if (StateMachine == null)
                return true;

            if (VaultUtils.isClient)
                return CurrentStateId == (int)AIStates.onKillAnim;

            if (CurrentStateId != (int)AIStates.onKillAnim)
            {
                StateMachine.ChangeState((int)AIStates.onKillAnim);
                SonState = 0;
                Timer = 0;
                NPC.dontTakeDamage = true;
                currentSurrounding = true;
                canDrawShadows = false;
                isDashing = false;
                NPC.life = 1;
                return false;
            }

            return true;
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            int width = (int)(95 * NPC.scale);
            int height = (int)(70 * NPC.scale);
            npcHitbox = new Rectangle((int)(NPC.Center.X - (width / 2)), (int)(NPC.Center.Y - (height / 2)), width, height);
            return true;
        }

        #endregion

        #region AI

        public enum AIStates
        {
            onSpawnAnmi = 1,
            onKillAnim,

            /// <summary> 短冲，用于调整身位 </summary>
            SmallDash,
            /// <summary> 闪电突袭，先3段短冲后进行一次长冲，二阶段的长冲会在路径上留下电球 </summary>
            LightningRaid,
            /// <summary> 放电，在身体周围生成电流环绕 </summary>
            Discharging,
            /// <summary> 闪电吐息，原地转一圈后使用吐息，二阶段改为使用电磁炮，会根据玩家位置持续调整方向 </summary>
            LightningBreath,
            /// <summary> 电球，吐出一个电球，二阶段时吐出多个 </summary>
            LightningBall,
            /// <summary> 电球，吐出一个电球，飞行一段时间后向四周爆开 </summary>
            CrossLightingBall,
            /// <summary> 落雷，先吼叫一声后飞向空中并隐身，之后选择落点，再下落，二阶段会连续使用，最多3次 </summary>
            FallingThunder,

            /// <summary> 一二阶段的切换动画 </summary>
            ExchangeP1_P2,

            /// <summary> 先冲刺，再放电 </summary>
            DashDischarging,
            /// <summary> 引力雷球 </summary>
            GravitationThunder,
            /// <summary> 电磁炮 </summary>
            ElectromagneticCannon,
            /// <summary> 冥雷，旋转飞，之后进入背景，并生成一些幻影，在天被照亮时才能看到，击破一定数量幻影后打断招式并使用落雷<br></br>
            /// 否则就释放超大范围放电
            /// </summary>
            StygianThunder
        }

        public override void AI()
        {
            if (!spwan)
            {
                ResetAllOldCaches();
                Phase = 1;

                if (!VaultUtils.isServer && !SkyManager.Instance["ThunderveinSky"].IsActive())//如果这个天空没激活
                {
                    SkyManager.Instance.Activate("ThunderveinSky");
                }

                spwan = true;
                if (!VaultUtils.isClient)
                    NPC.netUpdate = true;
            }

            EnsureAiMachine();

            ThunderveinPurpleAlpha = new Color(135, 94, 255, 0);
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 4500)//没有玩家存活时离开
                {
                    NPC.dontTakeDamage = false;
                    canDrawShadows = false;
                    isDashing = false;
                    NPC.spriteDirection = 1;
                    NPC.rotation = NPC.rotation.AngleTowards(0f, 0.14f);
                    NPC.velocity.X *= 0.98f;
                    FlyingUp(0.3f, 20, 0.9f);
                    NPC.EncourageDespawn(30);
                    return;
                }
            }

            UpdateSky();

            if (Initialize)
            {
                if (Main.zenithWorld)
                {
                    ThunderveinYellowAlpha = new Color(255, 171, 248, 0);
                    ThunderveinPurpleAlpha = new Color(6, 184, 217, 0);
                    ThunderveinOrangeAlpha = new Color(255, 157, 175, 0);
                }
                else
                {
                    ThunderveinYellowAlpha = new Color(255, 202, 101, 0);
                    ThunderveinPurpleAlpha = new Color(135, 94, 255, 0);
                    ThunderveinOrangeAlpha = new Color(219, 114, 22, 0);
                }
                Initialize = false;
            }

            // 顶层招式 FSM：状态 ID 走 ai[0]（服务端权威，客户端经 AiSlotNetSync 反推）。
            StateMachine.Update();
        }

        /// <summary>
        /// 懒初始化 FSM：注册 <see cref="SetupPhaseController"/>（含 P1→P2 切换与冥雷特招），并以出生动画为初始态。
        /// </summary>
        private void EnsureAiMachine()
        {
            if (aiBootstrapped)
                return;

            AiContext = new ThunderveinDragonContext(this);
            StateMachine = new CoraliteBossStateMachine<ThunderveinDragonContext>(AiContext);

            SetupPhaseController();

            StateMachine.SetInitialState(VaultStateRegistry<ThunderveinDragonContext>.Create((int)AIStates.onSpawnAnmi));
            RefreshAttackRandom();
            aiBootstrapped = true;
        }

        /// <summary>
        /// 用 <see cref="PhaseController"/> 表达"按血量阈值递降"的宏观切换（服务端权威，一次性触发）：<br/>
        /// 仅在处于可打断的常规招式时命中，避免打断出生/死亡/切换/冥雷自身。
        /// </summary>
        private void SetupPhaseController()
        {
            static float HpFrac(ThunderveinDragonContext ctx) => ctx.Npc.life / (float)ctx.Npc.lifeMax;

            bool masterLike = Main.masterMode || Main.getGoodWorld;
            float p2 = masterLike ? 0.75f : 0.5f;
            float p3 = masterLike ? 0.5f : 0.25f;
            float p4 = masterLike ? 0.25f : 0.125f;

            PhaseController.For(StateMachine)
                .OnCondition(ctx => ctx.Boss.Phase == 1 && HpFrac(ctx) <= p2 && ctx.Boss.IsInterruptibleAttack(),
                    () => VaultStateRegistry<ThunderveinDragonContext>.Create((int)AIStates.ExchangeP1_P2),
                    ctx => ctx.Boss.Phase = 2,
                    "ThunderveinP1ToP2")
                .OnCondition(ctx => ctx.Boss.Phase == 2 && HpFrac(ctx) <= p3 && ctx.Boss.IsInterruptibleAttack(),
                    () => VaultStateRegistry<ThunderveinDragonContext>.Create((int)AIStates.StygianThunder),
                    ctx => ctx.Boss.Phase = 3,
                    "ThunderveinP2ToP3")
                .OnCondition(ctx => ctx.Boss.Phase == 3 && HpFrac(ctx) <= p4 && ctx.Boss.IsInterruptibleAttack(),
                    () => VaultStateRegistry<ThunderveinDragonContext>.Create((int)AIStates.StygianThunder),
                    ctx => ctx.Boss.Phase = 4,
                    "ThunderveinP3ToP4")
                .Apply();
        }

        /// <summary>当前是否处于"可被阶段切换打断"的常规招式（排除出生/死亡/阶段切换/冥雷自身）。</summary>
        internal bool IsInterruptibleAttack()
        {
            int id = CurrentStateId;
            return id != (int)AIStates.onSpawnAnmi
                && id != (int)AIStates.onKillAnim
                && id != (int)AIStates.ExchangeP1_P2
                && id != (int)AIStates.StygianThunder;
        }

        public void RefreshAttackRandom()
            => AttackRandom = AiContext?.CreateAttackRandom() ?? new System.Random(NPC.whoAmI + 1);

        /// <summary>招内确定性随机：从已同步的 AttackSeed 派生，两端同序调用结果一致。</summary>
        internal float AttackRandFloat(float min, float max) => min + ((max - min) * (float)AttackRandom.NextDouble());
        internal bool AttackRandBool() => AttackRandom.Next(2) == 0;
        internal bool AttackRandBool(int num, int den) => AttackRandom.Next(den) < num;
        internal int AttackRandSign() => AttackRandom.Next(2) == 0 ? -1 : 1;

        public override void PostAI()
        {
            oldSpriteDirection = NPC.spriteDirection;

            if (!VaultUtils.isServer && currentSurrounding && Main.rand.NextBool(3))
            {
                Vector2 offset = Main.rand.NextVector2Circular(100 * NPC.scale, 70 * NPC.scale);
                ElectricParticle_Follow.Spawn(NPC.Center, offset, () => NPC.Center, Main.rand.NextFloat(0.75f, 1f));
            }
        }

        public void OnSpawnAnmi()
        {
            switch (SonState)
            {
                default:
                    ResetStates();
                    break;
                case 0:
                    {
                        NPC.TargetClosest();
                        NPC.dontTakeDamage = true;
                        TurnToNoRot(1);
                        Recorder = -1;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile proj = Main.projectile[i];
                            if (proj.active && proj.type == ModContent.ProjectileType<ThunderSpawn>())
                            {
                                Recorder = i;
                                break;
                            }
                        }

                        if (Recorder == -1)
                        {
                            NPC.Center = Target.Center + new Vector2(0, -400);
                        }
                        else
                        {
                            NPC.Center = Main.projectile[(int)Recorder].Center + new Vector2(0, -400);
                        }

                        SonState++;
                        selfAlpha = 0;
                    }
                    break;
                case 1:
                    {
                        FlyingFrame();
                        selfAlpha += 1 / 80f;
                        Timer++;
                        if (Timer > 80)
                        {
                            SonState++;
                            Timer = 0;
                            //生成名称
                            if (!VaultUtils.isClient)
                                NPC.NewProjectileDirectInAI<ThunderveinDragon_OnSpawnAnim>(NPC.Center, Vector2.Zero, 1, 0, NPC.target);
                        }
                    }
                    break;
                case 2://出现
                    {
                        FlyingFrame();
                        Timer++;
                        if (Timer > 30)
                        {
                            SonState++;
                            Timer = 0;
                        }
                    }
                    break;
                case 3://吼叫
                    {
                        UpdateAllOldCaches();

                        NPC.QuickSetDirection();
                        TurnToNoRot();
                        NPC.velocity *= 0.9f;
                        if (Timer == 0 && NPC.frame.Y != 4)
                        {
                            FlyingFrame();
                            break;
                        }

                        if (Timer == 15)
                        {
                            NPC.frame.Y = 0;
                            NPC.frame.X = 1;
                            NPC.velocity *= 0;
                            Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, NPC.Center, pitch: 0.4f);
                            SoundEngine.PlaySound(CoraliteSoundID.Roar, NPC.Center);
                        }
                        else if (Timer > 15 && Timer < 130 && !VaultUtils.isServer)
                        {
                            Vector2 pos = NPC.Center + (NPC.rotation.ToRotationVector2() * 60 * NPC.Center);
                            if ((int)Timer % 10 == 0)
                            {
                                var modifyer = new PunchCameraModifier(NPC.Center, Helper.NextVec2Dir(), 8, 12, 20, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);
                                PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringWave>(), Coralite.ThunderveinYellow, 0.2f);
                            }
                            if ((int)Timer % 20 == 0)
                                PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<RoaringLine>(), Color.White, 0.2f);
                        }

                        Timer++;
                        if (Timer > 150)
                            ResetStates();
                    }
                    break;

            }
        }

        public void OnKillAnmi()
        {
            if (Timer < 60)
            {
                anmiAlpha += 1 / 60f;
                NPC.velocity = -Vector2.UnitY;
                NPC.frame.X = 1;
                NPC.frame.Y = 0;
            }
            else
            {
                if (!VaultUtils.isServer)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        float factor = i / 30f;
                        float length = Helper.Lerp(80, 400, factor);

                        for (int j = 0; j < 5; j++)
                        {
                            PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2CircularEdge(length, length),
                                Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.9f, 1.3f));
                        }
                    }
                }

                SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, NPC.Center);

                if (!VaultUtils.isClient)
                    NPC.Kill();
            }

            Timer++;
        }

        #endregion

        #region States

        /// <summary>
        /// 招式收尾：服务端按当前阶段挑选下一个常规招式并经 FSM 切换（ai[0] 自动同步给客户端）。<br/>
        /// 阶段升级（P1→P2 / 冥雷特招）已交由 <see cref="SetupPhaseController"/> 每帧裁决，这里只负责常规招式轮换。<br/>
        /// 视觉量清理统一在 <see cref="ThunderveinDragonState.OnEnter"/> 完成。
        /// </summary>
        public void ResetStates()
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            // 客户端不主动挑招，跟随服务端经 ai[0] 同步过来的状态
            if (VaultUtils.isClient || StateMachine == null)
                return;

            List<int> moves = new();
            int oldState = (int)State;
            float distance = Vector2.Distance(NPC.Center, Target.Center);
            int dir = Target.Center.X > NPC.Center.X ? 1 : -1;

            moves.Add((int)AIStates.LightningRaid);
            moves.Add((int)AIStates.FallingThunder);
            moves.Add((int)AIStates.LightningBall);
            moves.Add((int)AIStates.LightningBreath);

            if (Main.masterMode)
                moves.Add((int)AIStates.CrossLightingBall);

            if (dir != NPC.spriteDirection)//玩家在背后时，大概率使用闪电突袭
            {
                for (int i = 0; i < 5; i++)
                    moves.Add((int)AIStates.LightningRaid);
            }

            if (oldState != (int)AIStates.SmallDash)//如果上次招式不是小冲刺那就小冲一下
            {
                for (int i = 0; i < 7; i++)
                    moves.Add((int)AIStates.SmallDash);
            }

            if (distance < 420)//距离较近是大概率使用放电
            {
                for (int i = 0; i < 4; i++)
                    moves.Add((int)AIStates.Discharging);
            }

            if (Phase == 1)//一阶段
            {
                if (distance > 800)//距离较大，使用闪电突袭，距离再大就直接落雷
                {
                    if (distance > 1400)
                        for (int i = 0; i < 7; i++)
                            moves.Add((int)AIStates.FallingThunder);
                    else
                        for (int i = 0; i < 7; i++)
                            moves.Add((int)AIStates.LightningRaid);
                }
            }
            else//二阶段及之后
            {
                moves.Add((int)AIStates.DashDischarging);

                if (distance > 800)
                {
                    if (distance > 1400)
                        for (int i = 0; i < 7; i++)
                            moves.Add((int)AIStates.FallingThunder);
                    else
                        for (int i = 0; i < 7; i++)
                            moves.Add((int)AIStates.DashDischarging);
                }

                if (UseMoveCount > 7)
                {
                    for (int i = 0; i < (int)UseMoveCount; i++)
                        moves.Add((int)AIStates.GravitationThunder);
                }
            }

            //当上次使用的是短距离冲刺的话，额外移除上上次所使用的招式
            if (oldState == (int)AIStates.SmallDash)
                moves.RemoveAll(i => i == (int)StateRecorder);
            //移除上次使用的招式
            moves.RemoveAll(i => i == oldState);

            int next = Main.rand.NextFromList(moves.ToArray());

            if (Phase != 1)
            {
                UseMoveCount++;
                //如果使用了引力雷球那么重置计时
                if (next == (int)AIStates.GravitationThunder)
                    UseMoveCount = 0;
            }

            //如果本次使用的是短距离冲刺那么记录上一招
            if (next == (int)AIStates.SmallDash)
                StateRecorder = oldState;

            StateMachine.ChangeState(next);
        }

        /// <summary>服务端切换到指定招式（视觉清理在 OnEnter 完成）。</summary>
        public void ResetToSelectedState(AIStates state)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            if (VaultUtils.isClient || StateMachine == null)
                return;

            StateMachine.ChangeState((int)state);
        }

        #endregion

        #region HelperMethods

        public void GetLengthToTargetPos(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        /// <summary>
        /// 向上飞，会改变速度
        /// </summary>
        /// <param name="acc">加速度</param>
        /// <param name="velMax">速度最大值</param>
        /// <param name="slowDownPercent">减速率</param>
        public void FlyingUp(float acc, float velMax, float slowDownPercent)
        {
            FlyingFrame();

            if (NPC.frame.Y <= 4)
            {
                NPC.velocity.Y -= acc;
                if (NPC.velocity.Y > velMax)
                    NPC.velocity.Y = velMax;
            }
            else
                NPC.velocity.Y *= slowDownPercent;
        }

        public void FlyingFrame(bool openMouse = false)
        {
            NPC.frame.X = openMouse ? 1 : 0;

            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }
        }

        public void DashFrame()
        {
            NPC.frame.X = 2;
            NPC.frame.Y = 0;
        }

        public Vector2 GetMousePos()
        {
            return NPC.Center + ((NPC.rotation - (NPC.direction * 0.1f)).ToRotationVector2() * 60 * NPC.scale);
        }

        /// <summary>
        /// 根据Y方向速度设置旋转
        /// </summary>
        public void SetRotationNormally(float rate = 0.08f)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;
            float targetRot = (NPC.velocity.Y * 0.05f * NPC.spriteDirection) + (NPC.spriteDirection > 0 ? 0 : MathHelper.Pi);
            NPC.rotation = NPC.rotation.AngleLerp(targetRot, rate);
        }

        /// <summary>
        /// 将身体回正
        /// </summary>
        /// <param name="rate"></param>
        public void TurnToNoRot(float rate = 0.2f)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.spriteDirection > 0 ? 0 : MathHelper.Pi, rate);
        }

        public static void SetBackgroundLight(float light, int fadeTime, int exchangeTime = 5)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            ThunderveinSky sky = (ThunderveinSky)SkyManager.Instance["ThunderveinSky"];
            sky.ExchangeTime = sky.MaxExchangeTime = exchangeTime;
            sky.targetLight = light;
            sky.oldLight = sky.light;
            sky.LightTime = fadeTime;
        }

        public static void UpdateSky()
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            ThunderveinSky sky = (ThunderveinSky)SkyManager.Instance["ThunderveinSky"];
            if (sky.Timeleft < 100)
                sky.Timeleft += 2;
            if (sky.Timeleft > 100)
                sky.Timeleft = 100;
        }

        public void InitOldFrame()
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            oldFrame ??= new Point[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldFrame[i] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void InitOldDirection()
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            oldDirection ??= new int[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldDirection[i] = NPC.spriteDirection;
        }

        public void UpdateOldFrame()
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            for (int i = 0; i < oldFrame.Length - 1; i++)
                oldFrame[i] = oldFrame[i + 1];
            oldFrame[^1] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void UpdateOldDirection()
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            for (int i = 0; i < oldDirection.Length - 1; i++)
                oldDirection[i] = oldDirection[i + 1];
            oldDirection[^1] = NPC.spriteDirection;
        }

        public void ResetAllOldCaches()
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            NPC.InitOldPosCache(trailCacheLength);
            NPC.InitOldRotCache(trailCacheLength);
            InitOldFrame();
            InitOldDirection();
        }

        public void UpdateAllOldCaches()
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            NPC.UpdateOldPosCache();
            NPC.UpdateOldRotCache();
            UpdateOldFrame();
            UpdateOldDirection();
        }

        #endregion

        #region 绘制部分

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            var frameBox = mainTex.Frame(3, 8, NPC.frame.X, NPC.frame.Y);
            var pos = NPC.Center - screenPos;
            var origin = frameBox.Size() / 2;
            float rot = NPC.rotation;

            SpriteEffects effects = SpriteEffects.None;

            if (NPC.spriteDirection < 0)
            {
                effects = SpriteEffects.FlipVertically;
            }

            //绘制残影
            if (canDrawShadows)
            {
                Color shadowColor = ThunderveinYellowAlpha;
                shadowColor.A = 50;
                shadowColor *= shadowAlpha;
                for (int i = 0; i < trailCacheLength; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i] - screenPos;
                    float oldrot = NPC.oldRot[i];
                    var frameOld = mainTex.Frame(3, 8, oldFrame[i].X, oldFrame[i].Y);
                    float factor = (float)i / trailCacheLength;
                    if (Phase == 2)
                    {
                        Color c1 = ThunderveinYellowAlpha;
                        c1.A = 50;
                        Color c2 = ThunderveinPurpleAlpha;
                        c2.A = 50;
                        shadowColor = Color.Lerp(c2, c1, factor);
                        shadowColor *= shadowAlpha;
                    }

                    SpriteEffects oldEffect = oldDirection[i] > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                    spriteBatch.Draw(mainTex, oldPos, frameOld, shadowColor * factor, oldrot, origin
                        , NPC.scale * shadowScale * (1 - ((1 - factor) * 0.3f)), oldEffect, 0);
                }
            }

            //绘制自己
            if (Main.zenithWorld)
                drawColor *= 0.2f;
            spriteBatch.Draw(mainTex, pos, frameBox, drawColor * selfAlpha, rot, origin, NPC.scale, effects, 0);
            //绘制glow
            spriteBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Glow").Value
                , pos, frameBox, Color.White * 0.75f * selfAlpha, rot, origin, NPC.scale, effects, 0);

            //绘制冲刺时的特效
            if (isDashing)
            {
                Texture2D exTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "StrikeTrail").Value;

                Vector2 exOrigin = new(exTex.Width * 6 / 10, exTex.Height / 2);

                Vector2 scale = new Vector2(1.3f, 1.5f) * NPC.scale;
                spriteBatch.Draw(exTex, pos, null, ThunderveinYellowAlpha, rot
                    , exOrigin, scale, effects, 0);
                scale.Y *= 1.2f;
                spriteBatch.Draw(exTex, pos - (NPC.rotation.ToRotationVector2() * 50), null, ThunderveinYellowAlpha * 0.5f, rot
                    , exOrigin, scale, effects, 0);
            }

            if (State == (int)AIStates.onKillAnim)
            {
                Texture2D whiteTex = ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Highlight").Value;

                spriteBatch.Draw(whiteTex, pos, frameBox, Color.White * anmiAlpha, rot, origin, NPC.scale, effects, 0);
            }

            return false;
        }

        #endregion
    }
}
