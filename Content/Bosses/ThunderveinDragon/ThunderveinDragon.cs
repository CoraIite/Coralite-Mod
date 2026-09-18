using Coralite.Content.Bosses.ThunderveinDragon.Core;
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

        // ai 槽全部让给基座约定：ai[0]=顶层状态 ID（AiSlotNetSync 同步）、ai[1]=AttackSeed、ai[2]=SonState、ai[3]=SyncTimer。
        // 旧的 localAI[0..3]（Recorder / Recorder2 / StateRecorder / UseMoveCount）与 phaseValue 全部搬到 Context 与状态热字段。
        internal ThunderveinDragonContext AiContext;
        internal CoraliteBossStateMachine<ThunderveinDragonContext> StateMachine;

        /// <summary>当前顶层状态 ID；状态机未建立时读 ai[0]（中途加入者在第一帧 AI 之前也能拿到正确值）。</summary>
        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)NPC.ai[CoraliteBossContext.StateAiSlot];

        /// <summary>幻影轮数计数（ai[2]）。从属 <see cref="ThunderPhantom"/> 每打完一轮雷暴给它 +1，签名不能改。</summary>
        internal ref float SonState => ref NPC.ai[CoraliteBossContext.SonStateAiSlot];

        /// <summary>阶段 1~4，落在 Context 上并随 <c>SendExtraAI</c> 过线；天空与残影配色读它。</summary>
        internal int Phase
        {
            get => AiContext?.Phase ?? 1;
            set
            {
                if (AiContext != null)
                {
                    AiContext.Phase = value;
                }
            }
        }

        /// <summary>当前吐息 / 电磁炮的瞄准角，弹幕 <see cref="ElectromagneticCannon"/> 读它跟随（旧 localAI[0]，只读）。</summary>
        internal float Recorder => AiContext?.AimAngle ?? 0f;

        /// <summary>冥雷期幻影的 NPC 索引，天空层 <see cref="ThunderveinSky"/> 读它绘制幻影（旧 localAI[0]，只读；-1 = 没有）。</summary>
        internal float PhantomIndex => AiContext?.PhantomIndex ?? -1f;

        public int hitCount;

        public readonly int trailCacheLength = 12;
        public Point[] oldFrame;
        public int[] oldDirection;

        public static Color ThunderveinYellowAlpha = new(255, 202, 101, 0);
        public static Color ThunderveinPurpleAlpha = new(135, 94, 255, 0);
        public static Color ThunderveinOrangeAlpha = new(219, 114, 22, 0);

        public static Color ThunderveinYellow = new(255, 202, 101);
        public static Color ThunderveinPurple = new(135, 94, 255);
        public static Color ThunderveinOrange = new(219, 114, 22);

        /// <summary>上一帧的贴图朝向，翻面时给 rotation 补半圈用（纯本地）。</summary>
        public int oldSpriteDirection;

        public bool Initialize = true;

        #region tmlHooks

        /// <summary>热字段（Timer / Counter / Beat / 状态自用槽）+ boss 事实（阶段）随 SyncNPC 原子过线。</summary>
        public override void SendExtraAI(BinaryWriter writer)
        {
            EnsureAiMachine();
            AiContext.WriteNet(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            EnsureAiMachine();
            AiContext.ReadNet(reader);
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

        /// <summary>本帧是否带电（由状态每帧声明，两端一致；命中方客户端也会读到）。</summary>
        internal bool CurrentSurrounding => AiContext != null && AiContext.CurrentSurrounding;

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (CurrentSurrounding)
                modifiers.SourceDamage -= 0.4f;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Colliding(projectile.getRect(), HeadHitBox()))
                modifiers.SourceDamage += 0.15f;

            if (CurrentSurrounding)
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
            if (NPC.life < NPC.lifeMax / 6 && hitCount < 6 && !CurrentSurrounding
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

        /// <summary>
        /// 死亡拦截：不在这里换态。<c>CheckDead</c> 会在造成最后一击的那一端（含客户端）跑，本地只把血锁到 1 并无敌，
        /// 权威端登记 <see cref="ThunderveinDragonContext.KillRequested"/>，由状态基类的 ServerUpdate 经返回值切到死亡演出，客户端读 ai[0] 跟随。<br/>
        /// 死亡演出末尾权威端 <c>NPC.Kill()</c> 再次进来，此时已在演出态 → 放行真死。
        /// </summary>
        public override bool CheckDead()
        {
            if (StateMachine == null || CurrentStateId == (int)AIStates.onKillAnim)
                return true;

            NPC.dontTakeDamage = true;
            NPC.life = 1;

            if (!VaultUtils.isClient)
            {
                AiContext.KillRequested = true;
                NPC.netUpdate = true;
            }

            return false;
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
            StygianThunder,

            /// <summary> 连接段 + 唯一提交口（迁移新增，取未用值 15；0 保持未用，这样 ai[0] 默认值不会解析成有效状态） </summary>
            Hub
        }

        /// <summary>
        /// 固定顺序（Phase 1 逐字镜像 Rediancie）：懒构造 → 客户端纠偏帧首 → 目标与脱战 → 只读事实 → 声明回默认
        /// → 状态机（状态只写声明，转移仅 ServerUpdate 返回值）→ 热字段兜底收养 → 落地运动 → 客户端记预测。
        /// </summary>
        public override void AI()
        {
            EnsureAiMachine();

            if (!spwan)
            {
                ResetAllOldCaches();

                // 阶段只由权威端起始化：中途加入的客户端此时 Phase 已由 ReceiveExtraAI 带到，重置成 1 会让它的运动数学（短冲帧数等）错一整个心跳周期。
                if (!VaultUtils.isClient)
                    AiContext.Phase = 1;

                if (!VaultUtils.isServer && !SkyManager.Instance["ThunderveinSky"].IsActive())//如果这个天空没激活
                {
                    SkyManager.Instance.Activate("ThunderveinSky");
                }

                spwan = true;
                if (!VaultUtils.isClient)
                    NPC.netUpdate = true;
            }

            if (VaultUtils.isClient)
                AiContext.Net.BeginClientFrame(NPC);

            ThunderveinPurpleAlpha = new Color(135, 94, 255, 0);

            if (!FindTarget())
            {
                Despawn();

                if (VaultUtils.isClient)
                    AiContext.Net.EndClientFrame(NPC);
                return;
            }

            UpdateSky();
            RefreshPaletteOnce();

            AiContext.UpdateFacts();
            AiContext.BeginFrameDefaults();

            // 状态只写声明；转移仅 ServerUpdate 返回值，客户端由 ai[0] 跟随
            StateMachine.Update();
            AiContext.ConsumePendingHotAdopt();

            ApplyDeclaredMovement();

            if (VaultUtils.isClient)
                AiContext.Net.EndClientFrame(NPC);
        }

        /// <summary>目标与脱战判定照旧；返回 false 表示该离场。旧 ThunderveinDragon.cs:377-392</summary>
        private bool FindTarget()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active
                || Target.Distance(NPC.Center) > ThunderveinDirector.RetargetDistance)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > ThunderveinDirector.DespawnDistance)
                    return false;
            }

            return true;
        }

        /// <summary>没有玩家存活时缓缓向上离场（两端同算的运动数学，不经状态机）。</summary>
        private void Despawn()
        {
            NPC.dontTakeDamage = false;
            AiContext.Invulnerable = false;
            AiContext.DrawShadows = false;
            AiContext.IsDashing = false;
            AiContext.CurrentSurrounding = false;
            NPC.spriteDirection = 1;
            NPC.rotation = NPC.rotation.AngleTowards(0f, ThunderveinDirector.DespawnRotRate);
            NPC.velocity.X *= ThunderveinDirector.DespawnDampX;
            FlyingUp(ThunderveinDirector.DespawnFlyUpAccel, ThunderveinDirector.DespawnFlyUpMax, ThunderveinDirector.DespawnFlyUpSlow);
            NPC.EncourageDespawn(ThunderveinDirector.DespawnEncourageFrames);
        }

        /// <summary>天顶世界换一套配色（一次性）。</summary>
        private void RefreshPaletteOnce()
        {
            if (!Initialize)
                return;

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

        /// <summary>
        /// 两端同跑：把本帧声明翻译成 velocity / rotation / 无敌标志。运动法则只在这一处，状态里没有裸的运动代码。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            ThunderveinDragonContext ctx = AiContext;

            switch (ctx.MoveMode)
            {
                case ThunderveinMoveMode.Keep:
                case ThunderveinMoveMode.Direct:
                    break;
                case ThunderveinMoveMode.Damp:
                    NPC.velocity *= ctx.DampFactor;
                    break;
                case ThunderveinMoveMode.Chase:
                    ApplyChase(ctx);
                    break;
                default:
                    NPC.velocity *= ThunderveinDirector.HoldDamp;
                    break;
            }

            ApplyRotation(ctx.RotationMode, ctx.RotationRate);

            // 原版不同步 dontTakeDamage，两端按同一份声明每帧落地；死亡请求期间保持无敌直到演出态接管
            NPC.dontTakeDamage = ctx.Invulnerable || ctx.KillRequested;
        }

        /// <summary>分轴追踪：旧各招式开头那段追击块的唯一实现。</summary>
        private void ApplyChase(ThunderveinDragonContext ctx)
        {
            ThunderveinChaseProfile profile = ctx.Chase;
            GetLengthToTargetPos(ctx.ChaseTarget, out float xLength, out float yLength);

            if (profile.Near > 0f && xLength < profile.Near)
                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, -NPC.direction, profile.SpeedX, profile.AccelX, profile.TurnX, ThunderveinDirector.ChaseDamp);
            else if (xLength > profile.Far)
                Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, profile.SpeedX, profile.AccelX, profile.TurnX, ThunderveinDirector.ChaseDamp);
            else
                NPC.velocity.X *= ThunderveinDirector.ChaseDamp;

            if (profile.FlyUp && NPC.directionY < 0)
            {
                FlyingUp(profile.FlyUpAccel, profile.FlyUpMax, profile.FlyUpSlow);
                return;
            }

            if (yLength > profile.ThresholdY)
                Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, profile.SpeedY, profile.AccelY, profile.TurnY, ThunderveinDirector.ChaseDamp);
            else
                NPC.velocity.Y *= ThunderveinDirector.ChaseDamp;

            if (profile.AdvanceFlyingFrame)
                FlyingFrame(profile.OpenMouth);
        }

        private void ApplyRotation(ThunderveinRotationMode mode, float rate)
        {
            if (mode == ThunderveinRotationMode.Keep)
                return;

            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += ThunderveinDirector.FlipRotation;

            if (mode == ThunderveinRotationMode.Normal)
            {
                float targetRot = (NPC.velocity.Y * ThunderveinDirector.RotationPerVelY * NPC.spriteDirection)
                    + (NPC.spriteDirection > 0 ? 0f : MathHelper.Pi);
                NPC.rotation = NPC.rotation.AngleLerp(targetRot, rate);
                return;
            }

            NPC.rotation = NPC.rotation.AngleLerp(NPC.spriteDirection > 0 ? 0f : MathHelper.Pi, rate);
        }

        /// <summary>
        /// 懒构造；初态从 ai[0] 重建——中途加入的客户端与 SetDefaults 之后才收到 ai[] 的重建路径都靠这个，未注册 id 回退出生动画。
        /// 上下文构造里已置 <c>UseLegacySpeedValve = false</c>。
        /// </summary>
        private void EnsureAiMachine()
        {
            if (StateMachine != null)
                return;

            AiContext = new ThunderveinDragonContext(this);
            StateMachine = new CoraliteBossStateMachine<ThunderveinDragonContext>(AiContext);

            SetupPhaseController();

            IVaultState<ThunderveinDragonContext> initial =
                VaultStateRegistry<ThunderveinDragonContext>.Create((int)NPC.ai[CoraliteBossContext.StateAiSlot])
                ?? VaultStateRegistry<ThunderveinDragonContext>.Create((int)AIStates.onSpawnAnmi);
            StateMachine.SetInitialState(initial);
        }

        /// <summary>
        /// 用 <see cref="PhaseController"/> 表达"按血量阈值递降"的宏观切换（权威端裁决，一次性触发）：<br/>
        /// 仅在处于可打断的常规招式时命中，避免打断出生 / 死亡 / 切换 / 冥雷自身。
        /// <c>OnFire</c> 写的 <c>Phase</c> 落在 Context 上并经 <c>WriteFacts</c> 过线，客户端读得到。
        /// </summary>
        private void SetupPhaseController()
        {
            static float HpFrac(ThunderveinDragonContext ctx) => ctx.Npc.life / (float)ctx.Npc.lifeMax;

            PhaseController.For(StateMachine)
                .OnCondition(ctx => ctx.Phase == 1 && HpFrac(ctx) <= ThunderveinDirector.PhaseLifeRatio(2) && ctx.IsInterruptibleAttack(),
                    () => VaultStateRegistry<ThunderveinDragonContext>.Create((int)AIStates.ExchangeP1_P2),
                    ctx => ctx.Phase = 2,
                    "ThunderveinP1ToP2")
                .OnCondition(ctx => ctx.Phase == 2 && HpFrac(ctx) <= ThunderveinDirector.PhaseLifeRatio(3) && ctx.IsInterruptibleAttack(),
                    () => VaultStateRegistry<ThunderveinDragonContext>.Create((int)AIStates.StygianThunder),
                    ctx => ctx.Phase = 3,
                    "ThunderveinP2ToP3")
                .OnCondition(ctx => ctx.Phase == 3 && HpFrac(ctx) <= ThunderveinDirector.PhaseLifeRatio(4) && ctx.IsInterruptibleAttack(),
                    () => VaultStateRegistry<ThunderveinDragonContext>.Create((int)AIStates.StygianThunder),
                    ctx => ctx.Phase = 4,
                    "ThunderveinP3ToP4")
                .Apply();
        }

        /// <summary>表现层：记朝向、带电时撒跟随电粒子。纯本地，不写任何 gameplay 量。</summary>
        public override void PostAI()
        {
            oldSpriteDirection = NPC.spriteDirection;

            if (!VaultUtils.isServer && CurrentSurrounding && Main.rand.NextBool(3))
            {
                Vector2 offset = Main.rand.NextVector2Circular(100 * NPC.scale, 70 * NPC.scale);
                ElectricParticle_Follow.Spawn(NPC.Center, offset, () => NPC.Center, Main.rand.NextFloat(0.75f, 1f));
            }
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

            // 表现偏移只加在绘制位上，判定盒不跟抖（C8）
            Vector2 drawOffset = AiContext?.DrawOffset ?? Vector2.Zero;
            float selfAlpha = AiContext?.SelfAlpha ?? 1f;
            float shadowAlpha = AiContext?.ShadowAlpha ?? 1f;
            float shadowScale = AiContext?.ShadowScale ?? 1f;
            bool canDrawShadows = AiContext != null && AiContext.DrawShadows;
            bool isDashing = AiContext != null && AiContext.IsDashing;

            var frameBox = mainTex.Frame(3, 8, NPC.frame.X, NPC.frame.Y);
            var pos = NPC.Center + drawOffset - screenPos;
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
                    Vector2 oldPos = NPC.oldPos[i] + drawOffset - screenPos;
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

            if (CurrentStateId == (int)AIStates.onKillAnim)
            {
                Texture2D whiteTex = ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Highlight").Value;

                spriteBatch.Draw(whiteTex, pos, frameBox, Color.White * (AiContext?.KillAnimAlpha ?? 0f), rot, origin, NPC.scale, effects, 0);
            }

            return false;
        }

        #endregion
    }
}
