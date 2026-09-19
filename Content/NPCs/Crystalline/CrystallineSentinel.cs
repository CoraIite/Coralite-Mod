using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 结晶战斗体主控：只留钩子、状态机宿主、<see cref="AI"/> 固定顺序、<see cref="ApplyDeclaredMovement"/>、网络接线与绘制。<br/>
    /// 招式体在 <c>CrystallineSentinel/States/</c> 一状态一文件，数字在 <see cref="CrystallineSentinelDirector"/>，
    /// 声明总线与两份常态 AI 在 <see cref="CrystallineSentinelContext"/>。接线方式镜像 <c>Rediancie.cs</c>。<br/>
    /// <b>注意</b>：本 NPC 没有设置 <c>NPC.boss</c>（旧代码就没设，保持不设）。
    /// </summary>
    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    [AutoloadBossHead]
    public class CrystallineSentinel : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [VaultLoaden("{@classPath}" + "CrystallineSentinel_Glow")]
        public static ATex GlowTex { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelFloatStone")]
        public static ATex FloatStone { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelGuard")]
        public static ATex GuardTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelExchange")]
        public static ATex ExchangeTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelExchange_Glow")]
        public static ATex ExchangeTex_Glow { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2")]
        public static ATex P2Head { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2_Glow")]
        public static ATex P2Head_Glow { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Float")]
        public static ATex P2Float { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Float_Glow")]
        public static ATex P2Float_Glow { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelHand")]
        public static ATex HandTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelHand_Glow")]
        public static ATex HandTex_Glow { get; private set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Spurt")]
        public static ATex P2SpurtTex { get; private set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelP2Spurt_Glow")]
        public static ATex P2SpurtTex_Glow { get; private set; }

        internal CrystallineSentinelContext AiContext;
        internal CoraliteBossStateMachine<CrystallineSentinelContext> StateMachine;

        /// <summary>当前顶层状态 ID；状态机未建立时读 ai[0]（中途加入者在第一帧 AI 之前也能拿到正确值）。</summary>
        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)NPC.ai[CoraliteBossContext.StateAiSlot];

        /// <summary>上一帧的碎岩旗，用来在脱战回血（旗由真变假）时把环绕浮石重新长出来。</summary>
        private bool lastReleasedRock;
        private bool rockFlagTracked;

        //==================== 供从属弹幕 / 粒子读取的挂点（名字不变，外部文件在用） ====================

        /// <summary>二阶段左手挂点。<c>CrystallineSentinelSwing</c> 与 <c>CrystallineSentinelTwinkle</c> 在用。</summary>
        public Vector2 P2LeftHandPos => NPC.Center + CrystallineSentinelDirector.LeftHandOffset;
        /// <summary>二阶段右手挂点。</summary>
        public Vector2 P2RightHandPos => NPC.Center + CrystallineSentinelDirector.RightHandOffset;

        /// <summary>这一刀用哪只手：+1 左手、−1 右手（旧代码让弹幕直接读 ai[2]，那个槽现在归基座）。</summary>
        public int SwingHandSign => AiContext?.SwingHandSign ?? 1;

        /// <summary>
        /// 当前招式的计时，等于旧代码的 <c>ai[1]</c>（那个槽现在是基座的攻击种子）。<br/>
        /// 由 <c>Timer</c> 与入场预充两个热字段合成，客户端能重建，所以从属弹幕两端读到的是同一个值。
        /// </summary>
        public float AttackTimer => AiContext?.StateAttackTimer ?? 0f;

        /// <summary>本帧是不是碎岩的出手拍：环绕的浮石粒子读到它就自毁。旧 <c>CheckCanReleaseRock</c></summary>
        public bool CheckCanReleaseRock() => AiContext?.RockReleaseCue ?? false;

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 21;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 80;
            NPC.damage = 60;
            NPC.defense = 45;

            NPC.lifeMax = 8000;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = false;
            NPC.netAlways = true;
            NPC.value = Item.buyPrice(0, 2);

            NPC.BossBar = ModContent.GetInstance<CrystallineSentinelBossBar>();

            ModContent.GetInstance<CrystallineSentinelBossBar>().Reset(NPC);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            int expertBaseLife = 9000;
            int masterBaseLife = 10500;

            int expertAddLife = 500;
            int masterAddLife = 750;

            NPC.defDamage = 60;
            //if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            //{
            //    if (nPCStrengthHelper.IsExpertMode)
            //    {
            //        NPC.lifeMax = (int)((expertBaseLife + (numPlayers * expertAddLife)) / journeyScale);
            //        NPC.damage = 62;
            //    }

            //    if (nPCStrengthHelper.IsMasterMode)
            //    {
            //        NPC.lifeMax = (int)((masterBaseLife + (numPlayers * masterAddLife)) / journeyScale);
            //        NPC.damage = 70;
            //    }

            //    if (Main.getGoodWorld)
            //    {
            //        NPC.damage = 75;
            //    }

            //    return;
            //}

            NPC.lifeMax = expertBaseLife + (numPlayers * expertAddLife);
            NPC.damage = 62;

            if (Main.masterMode)
            {
                NPC.lifeMax = masterBaseLife + (numPlayers * masterAddLife);
                NPC.damage = 70;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 11000 + (numPlayers * 1000);
                NPC.damage = 75;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 12, 36));
            //固定掉落矽卡岩
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Skarn>(), 1, 32, 64));
            //固定掉落矽卡砖
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Skarn>(), 1, 32, 64));

            //固定掉落印痕的蕴魔板
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineEngram>(), 1, 2, 4));

            //固定掉落蕴魔石板
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkarnKey>(), 1, 3, 6));

            //固定掉落魔方
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrillantRubiksCube>()));

            //掉落宝石原石
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SeniorRoughGemstone>(), 1, 2, 4));
        }

        /// <summary>接触伤害由状态每帧声明（旧 ai[3] CanHit）。</summary>
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => AiContext?.CanHit ?? false;

        public override int SpawnNPC(int tileX, int tileY)
        {
            for (int i = 0; i < 45; i++)
            {
                Tile t = Framing.GetTileSafely(tileX, tileY);
                if (t.HasTile && Main.tileSolid[t.TileType])
                    break;

                tileY++;
            }

            return NPC.NewNPC(new EntitySource_SpawnNPC(), tileX * 16 + 8, tileY * 16, NPC.type);
        }

        public override bool? CanFallThroughPlatforms() => true;

        /// <summary>护盾成形期间不画血条（旧 <c>State == P1Guard &amp;&amp; Recorder is 1 or 2</c> → 声明通道 <c>ShieldUp</c>）。</summary>
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (AiContext != null && AiContext.ShieldUp)
                return false;

            return null;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            if (AiContext != null && AiContext.ShieldUp)//防御状态鼠标移上去没效果
            {
                boundingBox = default;
                return;
            }

            boundingBox = Utils.CenteredRectangle(NPC.Center, NPC.Size);
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (AiContext == null)
                return;

            if (AiContext.Target == player)
                AiContext.AggroCounter = CrystallineSentinelDirector.AggroCounterMax;

            if (NearThreshold())
                CounterEffect(player.DirectionTo(NPC.Center), hit);

            if (AiContext.HitSlowdown)
                OnHitSlow();

            AiContext.OnHitTimer = 0;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (AiContext == null)
                return;

            if (Vector2.Distance(NPC.Center, AiContext.Target.Center) > CrystallineSentinelDirector.GuardCounterRange)
                AiContext.GuardCounter += damageDone;

            if (AiContext.Target == Main.player[projectile.owner])
                AiContext.AggroCounter = CrystallineSentinelDirector.AggroCounterMax;

            if (NearThreshold())
                CounterEffect(projectile.velocity, hit, 2);

            if (AiContext.HitSlowdown)
                OnHitSlow();

            AiContext.OnHitTimer = 0;
            if (CurrentStateId == (int)CrystallineSentinelStateId.P1Guard)
                AiContext.SetText(CrystallineSentinelTextType.Block, CrystallineSentinelDirector.HitTextFrames);
        }

        /// <summary>血量正贴在碎岩线 / 转阶段线上（这两下会被打断，所以爆一把碎片作反馈）。旧 CrystallineSentinel.cs:364-368</summary>
        private bool NearThreshold()
            => (NPC.life < NPC.lifeMax * CrystallineSentinelDirector.RockReleaseThreshold && !AiContext.ReleasedRock)
                || (NPC.life < NPC.lifeMax * CrystallineSentinelDirector.Phase2Threshold && !AiContext.IsPhase2);

        public void OnHitSlow()
        {
            NPC.velocity *= 0.99f;
        }

        /// <summary>
        /// 被打断时的效果（爆粒子）。纯表现，只在本地跑。
        /// </summary>
        /// <param name="dir">受击方向</param>
        /// <param name="hit"></param>
        public void CounterEffect(Vector2 dir, NPC.HitInfo hit, int dustCount = 5)
        {
            if (Main.dedServ)
                return;

            Vector2 dirnormaled = Vector2.Normalize(dir);
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 pos = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 6);
                Vector2 vel = dirnormaled.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(0, 3.5f);

                if (dir.Length() > 2f)
                {
                    float dot = Vector2.Dot(vel, dir * 0.3f);
                    vel *= dot / 4;
                }
                CrystallineFragmentParticle prt = PRTLoader.NewParticle<CrystallineFragmentParticle>(pos, vel);
                if (prt != null)
                    prt.Scale = Main.rand.NextFloat(0.4f, 1f);
            }
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            return attacker.type == ModContent.NPCType<CrystallineSentinelMissile>() && attacker.ai[0] > 60;
        }

        /// <summary>
        /// 自己的飞弹打中自己：压开盾冷却、弹颜文字，并<b>登记</b>破盾请求——真正换拍由
        /// <c>CrystallineSentinelP1GuardState</c> 的权威端消费（招式体外不换态，D5）。
        /// </summary>
        public void OnHitByMissile()
        {
            if (AiContext == null)
                return;

            AiContext.GuardCooldown = CrystallineSentinelDirector.GuardCooldownMax;
            AiContext.SetText(CrystallineSentinelTextType.HitSelf, CrystallineSentinelDirector.HitTextFrames);
            AiContext.ShieldBreakRequested = true;

            if (Main.dedServ)
                return;

            for (int i = 0; i < 6 * 6; i++)
            {
                Vector2 position = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 6);
                PRTLoader.NewParticle<CrystalFlashParticle>(position, vel * 0.75f + NPC.velocity * 0.25f);
            }
        }

        /// <summary>
        /// 死亡拦截：不在这里换态。本地把血锁到 1（命中方客户端与服务端都会跑到这里），
        /// 权威端登记 <see cref="CrystallineSentinelContext.KillRequested"/>，由状态基类的 ServerUpdate 经返回值切到死亡演出，客户端读 ai[0] 跟随。<br/>
        /// 死亡演出跑满 <see cref="CrystallineSentinelDirector.DyingDeadGate"/> 帧后再次进来时放行真死。旧 CrystallineSentinel.cs:456-469
        /// </summary>
        public override bool CheckDead()
        {
            if (StateMachine == null)
                return true;

            if (CurrentStateId == (int)CrystallineSentinelStateId.P2Dying
                && AiContext.StateTimer >= CrystallineSentinelDirector.DyingDeadGate)
                return true;

            NPC.life = 1;
            NPC.active = true;

            if (!VaultUtils.isClient)
            {
                AiContext.KillRequested = true;
                NPC.netUpdate = true;
            }

            return false;
        }

        public override void OnKill()
        {
            SentinelSpawner.SentinelKilled();
            DownedBossSystem.DownCrystallineSentinel();
        }

        #endregion

        #region AI

        /// <summary>
        /// 固定顺序：懒构造 → 客户端纠偏帧首 → 只读事实 → 声明回默认 → 状态机 → 热字段兜底收养 → 常态 AI → 落地运动 → 表现 → 客户端记预测。<br/>
        /// <b>与 brief 的模板有一处差异</b>：索敌与脱战不是单独的 <c>FindTarget()</c>，而是留在两份常态 AI 里
        /// （旧代码就是在招式体<b>之后</b>才索敌 / 判脱战，提到前面会整体挪动一帧的节拍与转向时机），
        /// 所以这里保持旧位置：<c>StateMachine.Update()</c> 之后、<c>ApplyDeclaredMovement()</c> 之前。
        /// </summary>
        public override void AI()
        {
            EnsureAiMachine();

            if (VaultUtils.isClient)
                AiContext.Net.BeginClientFrame(NPC);

            //NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly 会让 NPC 自然回血，每帧压掉（旧 NormalAI）
            NPC.friendlyRegen = 0;

            AiContext.UpdateFacts();
            AiContext.BeginFrameDefaults();

            // 状态只写声明；转移仅 ServerUpdate 返回值，客户端由 ai[0] 跟随
            StateMachine.Update();
            AiContext.ConsumePendingHotAdopt();

            UpdateCommonAI();
            ApplyDeclaredMovement();

            if (!Main.dedServ)
                UpdatePresentation();

            if (VaultUtils.isClient)
                AiContext.Net.EndClientFrame(NPC);
        }

        /// <summary>
        /// 懒构造；初态从 ai[0] 重建——中途加入的客户端与 <c>SetDefaults</c> 之后才收到 ai[] 的重建路径都靠这个，未注册 id 回退一阶段站立。<br/>
        /// 上下文构造里已置 <c>UseLegacySpeedValve = false</c>。
        /// </summary>
        private void EnsureAiMachine()
        {
            if (StateMachine != null)
                return;

            AiContext = new CrystallineSentinelContext(this);
            StateMachine = new CoraliteBossStateMachine<CrystallineSentinelContext>(AiContext);

            IVaultState<CrystallineSentinelContext> initial =
                VaultStateRegistry<CrystallineSentinelContext>.Create((int)NPC.ai[CoraliteBossContext.StateAiSlot])
                ?? VaultStateRegistry<CrystallineSentinelContext>.Create((int)CrystallineSentinelStateId.P1Idle);
            StateMachine.SetInitialState(initial);

            Initialize();

            // 旧代码的机体音挂在 SwitchState 里，出生这一次并不发声；初态是 SetInitialState 进的，所以建完机器才武装
            AiContext.SwitchSoundArmed = true;
        }

        /// <summary>初始化：环绕浮石（纯本地）+ 起手未警戒、仇恨为负（敌不动我不动）。旧 <c>Initialize</c>，CrystallineSentinel.cs:562-586</summary>
        private void Initialize()
        {
            AiContext.SpawnFloatStones();
            AiContext.AggroCounter = -1;
            AiContext.Alerted = false;
        }

        /// <summary>
        /// 两份常态 AI 的分派（旧 <c>IsPhase2 ? P2NormalAI() : P1NormalAI()</c>）：由当前状态自己申报跟哪一份，
        /// 免得新增的 hub 顶在枚举末尾把区间判定搞错。旧 CrystallineSentinel.cs:540-545
        /// </summary>
        private void UpdateCommonAI()
        {
            CrystallineSentinelCommon common = (StateMachine.CurrentState as CrystallineSentinelStateBase)?.Common
                ?? CrystallineSentinelCommon.PhaseOne;

            switch (common)
            {
                case CrystallineSentinelCommon.PhaseOne:
                    AiContext.UpdatePhaseOneCommon();
                    break;
                case CrystallineSentinelCommon.PhaseTwo:
                    AiContext.UpdatePhaseTwoCommon();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 两端同跑：把本帧声明翻译成 velocity 与各类开关。状态里没有裸的运动法则，全局规则只在这里改一处（C1）。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            CrystallineSentinelContext ctx = AiContext;

            switch (ctx.MoveMode)
            {
                case CrystallineSentinelMoveMode.StandStill:
                    NPC.velocity.X = 0f;
                    break;
                case CrystallineSentinelMoveMode.GroundWalk:
                    // 旧各接近段都是这三句：踏台阶 → 朝 direction 加速到上限 →（可选）反向即刹死
                    if (ctx.WalkStepUp)
                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                    if (MathF.Abs(NPC.velocity.X) < ctx.WalkMaxSpeed)
                        NPC.velocity.X += NPC.direction * ctx.WalkAccel;

                    if (ctx.WalkZeroOnReverse && MathF.Sign(NPC.velocity.X) != NPC.direction)
                        NPC.velocity.X = 0f;
                    break;
                case CrystallineSentinelMoveMode.Damp:
                    NPC.velocity *= ctx.DampFactor;
                    break;
                default:
                    // Keep / Direct：状态自己管速度（一阶段有重力自然下坠、二阶段靠各招的衰减收速）
                    break;
            }

            // 原版不同步这几个标志，两端按同一份声明每帧落地；死亡请求期间保持无敌直到演出态接管
            NPC.noTileCollide = ctx.NoTileCollide;
            NPC.noGravity = ctx.NoGravity;
            NPC.dontTakeDamage = ctx.Invulnerable || ctx.KillRequested;
            NPC.SuperArmor = ctx.SuperArmor;
        }

        /// <summary>
        /// 纯本地表现层：颜文字倒计时、受击闪光计时、部位动力学归位、浮石重建。不写任何 gameplay 量（C8 / D8）。
        /// </summary>
        private void UpdatePresentation()
        {
            AiContext.UpdateText();
            AiContext.OnHitTimer++;
            AiContext.EnsurePartRig();

            // 脱战回血把碎岩机会还回来（旗由真变假）时浮石重新长出来；旧代码把这段写在服务端分支里，联机客户端永远看不到
            if (rockFlagTracked && lastReleasedRock && !AiContext.ReleasedRock)
                AiContext.SpawnFloatStones();

            lastReleasedRock = AiContext.ReleasedRock;
            rockFlagTracked = true;
        }

        #endregion

        #region 网络同步

        /// <summary>热字段（Timer / Counter / Beat / Flags / 自用槽）+ boss 事实（警戒 / 碎岩旗）随 SyncNPC 原子过线。</summary>
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

        #endregion

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (AiContext == null)
                return false;

            // 表现偏移只加在绘制位上，判定盒不跟抖（C8）
            screenPos -= AiContext.DrawOffset;

            SpriteEffects effect = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            switch (Phase(CurrentStateId))
            {
                case DrawPhase.PhaseOne:
                    DrawPhaseOne(spriteBatch, screenPos, drawColor, effect);
                    break;
                case DrawPhase.Exchange:
                    DrawExchange(spriteBatch, screenPos, drawColor, effect);
                    break;
                case DrawPhase.Rolling:
                    DrawRolling(spriteBatch, screenPos, drawColor, effect);
                    break;
                default:
                    DrawPhaseTwo(spriteBatch, screenPos, drawColor, effect);
                    break;
            }

            DrawText(spriteBatch, NPC.Top - screenPos);

            return false;
        }

        private enum DrawPhase
        {
            PhaseOne,
            Exchange,
            PhaseTwo,
            Rolling,
        }

        /// <summary>状态 id → 用哪套贴图。hub（新增的提交口）按已锁定的阶段回落，免得偶尔停在它身上时本体消失。</summary>
        private DrawPhase Phase(int stateId) => (CrystallineSentinelStateId)stateId switch
        {
            CrystallineSentinelStateId.Exchange => DrawPhase.Exchange,
            CrystallineSentinelStateId.P2Rolling => DrawPhase.Rolling,
            CrystallineSentinelStateId.P2Idle or CrystallineSentinelStateId.P2Swing
                or CrystallineSentinelStateId.P2WhirlSlash or CrystallineSentinelStateId.P2WhirlSlashShort
                or CrystallineSentinelStateId.P2Rest or CrystallineSentinelStateId.P2Dying => DrawPhase.PhaseTwo,
            CrystallineSentinelStateId.hub => AiContext.IsPhase2 ? DrawPhase.PhaseTwo : DrawPhase.PhaseOne,
            _ => DrawPhase.PhaseOne,
        };

        /// <summary>一阶段：每帧尺寸 174*112 的 7 列帧图 + 辉光，护盾张开时叠护盾。旧 CrystallineSentinel.cs:2096-2108</summary>
        private void DrawPhaseOne(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, SpriteEffects effect)
        {
            Texture2D tex = NPC.GetTexture();
            Rectangle frameBox = tex.Frame(7, Main.npcFrameCount[NPC.type], NPC.frame.X, NPC.frame.Y);

            Vector2 drawPos = NPC.Center - screenPos + new Vector2(NPC.spriteDirection * 40, -16);
            spriteBatch.Draw(tex, drawPos, frameBox, drawColor, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(GlowTex.Value, drawPos, frameBox, Color.White, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);

            if (AiContext.GuardFactor != 0)
                DrawGuard(spriteBatch, screenPos);
        }

        /// <summary>转阶段：单列 20 帧的碎裂帧图。旧 CrystallineSentinel.cs:2110-2120</summary>
        private void DrawExchange(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, SpriteEffects effect)
        {
            Texture2D tex = ExchangeTex.Value;
            Rectangle frameBox = tex.Frame(1, 20, NPC.frame.X, NPC.frame.Y);

            Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, -22);
            spriteBatch.Draw(tex, drawPos, frameBox, drawColor, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(ExchangeTex_Glow.Value, drawPos, frameBox, Color.White, NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);
        }

        /// <summary>二阶段常态：双手 + 浮游炮 + 头。休息时本体与双手各自随修理进度抖。旧 CrystallineSentinel.cs:2121-2161</summary>
        private void DrawPhaseTwo(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, SpriteEffects effect)
        {
            bool faceLeft = NPC.spriteDirection < 0;

            Vector2 handOffset = Vector2.Zero;
            if (CurrentStateId == (int)CrystallineSentinelStateId.P2Rest)
            {
                float a = Utils.Remap(AiContext.RestFactor, 0, 1, 0, CrystallineSentinelDirector.RestShakeDrawBody);
                screenPos += new Vector2(Main.rand.NextFloat(-a, a), Main.rand.NextFloat(-a, a));

                float b = Utils.Remap(AiContext.RestFactor, 0, 1, 0, CrystallineSentinelDirector.RestShakeDrawHand);
                handOffset += new Vector2(Main.rand.NextFloat(-b, b), Main.rand.NextFloat(-b, b));
            }

            DrawP2Hand(spriteBatch, screenPos + handOffset, effect, drawColor, 0, faceLeft ? 0 : 1);
            DrawP2Hand(spriteBatch, screenPos + handOffset, effect, drawColor, 1, faceLeft ? 1 : 0);

            DrawP2Float(spriteBatch, screenPos, effect, drawColor);
            DrawP2Head(spriteBatch, screenPos, effect, drawColor);
        }

        /// <summary>螺旋冲刺：浮游炮 + 头 + 展开的刀刃（16 帧竖排）。旧 CrystallineSentinel.cs:2162-2186</summary>
        private void DrawRolling(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, SpriteEffects effect)
        {
            DrawP2Float(spriteBatch, screenPos, effect, drawColor);
            DrawP2Head(spriteBatch, screenPos, effect, drawColor);

            Texture2D tex = P2SpurtTex.Value;
            Rectangle frameBox = tex.Frame(1, 16, 0, AiContext.HandSpurtFrameY);
            SpriteEffects effect3 = NPC.spriteDirection <= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = NPC.spriteDirection > 0 ? new(60, 40) : new(151, 35);
            float offsetY = Utils.Remap(AiContext.StateTimer, 0f, 15f, -36, -12);
            Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, offsetY);
            spriteBatch.Draw(tex, drawPos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effect3, 0);
            spriteBatch.Draw(P2SpurtTex_Glow.Value, drawPos, frameBox, Color.White, NPC.rotation, origin, NPC.scale, effect3, 0);
        }

        private void DrawText(SpriteBatch spriteBatch, Vector2 pos)
        {
            if (!AiContext.ShowText)
                return;

            string text = AiContext.TextType switch
            {
                CrystallineSentinelTextType.Confusion => "[ o O ] ?",
                CrystallineSentinelTextType.Surprise => "[ o o ] !",
                CrystallineSentinelTextType.Block => "[ ◽ ◽ ]",
                CrystallineSentinelTextType.Fire => "[ O ^ O ]",
                CrystallineSentinelTextType.HitSelf => "[ ≥ x ≤ ]",
                CrystallineSentinelTextType.Broken => "[ x x ]",
                CrystallineSentinelTextType.Angry => "![ ▼ M ▼ ]＃",
                _ => ""
            };

            Utils.DrawBorderString(spriteBatch, text, pos + new Vector2(0, -8), Coralite.CrystallinePurple
                , 1f, 0.5f, 0.5f);
        }

        /// <summary>
        /// 0左手，1右手
        /// </summary>
        public void DrawP2Hand(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor, int leftOrRight, int texLorR)
        {
            Texture2D tex = HandTex.Value;

            Rectangle frameBox = tex.Frame(2, 13, texLorR, AiContext.HandFrame[leftOrRight]);

            spriteBatch.Draw(tex, AiContext.HandCenter[leftOrRight] - screenPos, frameBox, drawColor
                , 0, frameBox.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(HandTex_Glow.Value, AiContext.HandCenter[leftOrRight] - screenPos, frameBox, Color.White
                , 0, frameBox.Size() / 2, NPC.scale, effect, 0);
        }

        public void DrawP2Float(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor)
        {
            Texture2D tex = P2Float.Value;

            Rectangle frameBox = tex.Frame(1, 8, 0, NPC.frame.Y);
            float rot = Utils.Remap(MathF.Abs(NPC.velocity.X), 0, 14f, 0, MathHelper.PiOver4) * (NPC.velocity.X > 0).ToDirectionInt();

            spriteBatch.Draw(tex, AiContext.FloatCenter - screenPos, frameBox, drawColor
                , rot, frameBox.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(P2Float_Glow.Value, AiContext.FloatCenter - screenPos, frameBox, Color.White
                , rot, frameBox.Size() / 2, NPC.scale, effect, 0);
        }

        public void DrawP2Head(SpriteBatch spriteBatch, Vector2 screenPos, SpriteEffects effect, Color drawColor)
        {
            spriteBatch.Draw(P2Head.Value, NPC.Center - screenPos + new Vector2(0, -10), null, drawColor
                , NPC.rotation, P2Head.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(P2Head_Glow.Value, NPC.Center - screenPos + new Vector2(0, -10), null, Color.White
                , NPC.rotation, P2Head.Size() / 2, NPC.scale, effect, 0);
        }

        public void DrawGuard(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D guardTex = GuardTex.Value;
            Vector2 pos = NPC.Center + new Vector2(0, -20) - screenPos;
            float guardFactor = AiContext.GuardFactor;
            float scale = Helper.BezierEase(guardFactor);

            //最内层
            Rectangle framebox = guardTex.Frame(1, 3, 0, 0);
            spriteBatch.Draw(guardTex, pos, framebox, Color.White, Helper.SqrtEase(guardFactor) * MathHelper.TwoPi, framebox.Size() / 2, scale, 0, 0);

            //外层
            framebox = guardTex.Frame(1, 3, 0, 2);
            spriteBatch.Draw(guardTex, pos, framebox, Color.White, 0, framebox.Size() / 2, scale, 0, 0);

            //受击时闪光
            int onHitTimer = AiContext.OnHitTimer;
            if (onHitTimer > 18)
                return;
            framebox = guardTex.Frame(1, 3, 0, 1);
            SpriteEffects effects = onHitTimer < 6 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float alpha = 0.2f + MathF.Sin(onHitTimer / 18f * MathHelper.Pi + MathHelper.PiOver4) * 0.4f;
            spriteBatch.Draw(guardTex, pos, framebox, Color.White * alpha, 0, framebox.Size() / 2, scale, effects, 0);
        }

        #endregion

        /// <summary>
        /// 把atan2（vector2.torotation的输出）转为 <br/>
        /// 0~pi/2（第四象限）：0~pi/2（不变） <br/>
        /// pi/2~pi（第三象限）：-pi/2~0 <br/>
        /// -pi~-pi/2（第二象限）：0~pi/2 <br/>
        /// -pi/2~0（第一象限）：pi/2~0 <br/>
        /// </summary>
        public static float ConvertAtan2ToSpecialAngle(float theta)
        {
            //计算 |cos(theta)|
            float cosTheta = MathF.Cos(theta);
            float absCosTheta = MathF.Abs(cosTheta);

            //计算核心角度 [0, π/2]
            float angle = MathF.Acos(Math.Clamp(absCosTheta, -1f, 1f));

            //根据 sin(theta) 判断上下半圆
            float sinTheta = MathF.Sin(theta);
            float finalAngle = sinTheta > 0 ? -angle : angle;

            //右半圆反转符号
            if (theta.ToRotationVector2().X > 0)
            {
                finalAngle = -finalAngle;
            }

            return finalAngle;
        }
    }
}
