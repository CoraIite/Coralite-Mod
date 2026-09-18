using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Items.Thunder;
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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    [AutoloadBossHead]
    [VaultLoaden(AssetDirectory.ZacurrentDragon)]
    public partial class ZacurrentDragon : ModNPC
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + Name;

        internal Player Target => Main.player[NPC.target];

        #region FSM 宿主

        internal ZacurrentDragonContext AiContext;
        internal CoraliteBossStateMachine<ZacurrentDragonContext> StateMachine;

        /// <summary>
        /// 顶层状态枚举，写入 <c>npc.ai[0]</c> 同步。成员与数值沿用旧 <c>AIStates</c>（线格式不变，<c>ZacurrentSky</c> 也按它判断），
        /// 末尾追加 <see cref="AIStates.hub"/>。
        /// </summary>
        public enum AIStates
        {
            /// <summary>
            /// 等待，就是啥也不干只正常飞一飞
            /// </summary>
            Waiting,

            //动画阶段
            onSpawnAnmi,
            onKillAnim,

            /// <summary> 紫伏形态的切换 </summary>
            PurpleVoltExchange,

            //单招
            /// <summary> 闪电突袭，先短冲后进行一次长冲 </summary>
            LightningRaidNormal,
            /// <summary> 电流吐息，小 </summary>
            ElectricBreathSmall,
            /// <summary> 电流吐息，中 </summary>
            ElectricBreathMiddle,
            /// <summary> 电球 </summary>
            ElectricBall,
            /// <summary> 冲刺放电 </summary>
            DashDischarging,

            //2阶段单招
            /// <summary> 2阶段指针电球 </summary>
            PointerBall,
            /// <summary> 2阶段指针电球 </summary>
            LightningRaidVolt,

            //连段
            /// <summary>
            /// 吼叫=》电球=》引力电球=》电磁炮=》聚集电流
            /// </summary>
            NormalRoarCombo1,
            /// <summary>
            /// 吼叫=》闪电链=》闪电突袭=》聚集电流
            /// </summary>
            NormalRoarCombo2,
            /// <summary>
            /// 闪电链=》落雷=》冲刺放电=》聚集电流
            /// </summary>
            NormalChainCombo,
            /// <summary>
            /// 指针电球=》闪电链=》电流吐息（中）=》落雷=》聚集电流
            /// </summary>
            NormalPointerCombo,

            /// <summary>
            /// 二阶段超长连段<br></br>
            /// 吼叫=》闪电链=》 循环x3（  指针电球=》闪电突袭x1  ）=》引力电球（超长持续时间）<br></br>
			/// =》电流吐息（中）=》Z电球 =》电伏击穿 =》落雷
            /// </summary>
            VoltBigCombo,
            /// <summary>
            /// 二阶段短连招：闪电链=》电伏击穿=》电流吐息（中）
            /// </summary>
            VoltChainCombo,
            /// <summary>
            /// Z电球=》闪电突袭=》电伏击穿=》指针电流=》落雷
            /// </summary>
            VoltZBallChainCombo,

            //调整身位用招式
            SmallDash,
            SmallDashVolt,

            //其他
            /// <summary>
            /// 从紫伏状态回归，给玩家一些输出时间
            /// </summary>
            Break,

            /// <summary>连接段 + 唯一提交口（新增；<see cref="ZacurrentDirector.HubFrames"/> 为 0 时不驻留）</summary>
            hub,
        }

        /// <summary>
        /// 当前的攻击状态（读取已同步的 <c>ai[0]</c>，由 FSM 写入与同步）
        /// </summary>
        public AIStates State => (AIStates)NPC.ai[CoraliteBossContext.StateAiSlot];

        /// <summary>招内通用记录位，<c>Projectile.ElectromagneticCannon</c> 读它拿锁定角度。</summary>
        internal float Recorder => AiContext?.Recorder ?? 0f;

        private bool init = true;

        #endregion

        /// <summary>
        /// 是否处于紫伏状态
        /// </summary>
        public bool PurpleVolt { get; internal set; }

        /// <summary>
        /// 紫电计数（经 <see cref="SendExtraAI"/> 同步给客户端供血条绘制）
        /// </summary>
        public float PurpleVoltCount { get; internal set; }
        /// <summary>
        /// 是否绘制残影
        /// </summary>
        public bool canDrawShadows;
        /// <summary>
        /// 是否绘制冲刺是的特殊贴图，如果为true会按照特殊的方式绘制自身
        /// </summary>
        public bool IsDashing
        {
            get => NPC.frame.X == 1;
            set
            {
                if (value)
                    NPC.frame.X = 1;
                else
                    NPC.frame.X = 0;
            }
        }

        /// <summary>
        /// 身上有电流环绕，会减伤并生成闪电粒子
        /// </summary>
        public bool currentSurrounding;

        /// <summary>
        /// 是否张嘴，控制帧图
        /// </summary>
        public bool OpenMouse { get; internal set; }

        public float selfAlpha = 1f;

        /// <summary>残影用的历史帧图 / 朝向缓存（纯本地，服务端不建）。</summary>
        public Point[] oldFrame;
        public int[] oldDirection;
        public int oldSpriteDirection;

        [VaultLoaden("{@classPath}" + "ZacurrentDragon_Highlight")]
        public static ATex GlowTex { get; private set; }
        [VaultLoaden("{@classPath}" + "ZacurrentDragonWhite")]
        public static ATex WhiteTex { get; private set; }
        internal static Color ZacurrentDustPurple = new Color(233, 195, 255);
        internal static Color ZacurrentPurple = new(135, 94, 255);
        internal static Color ZacurrentPink = new(255, 115, 226);
        internal static Color ZacurrentPurpleAlpha = new(135, 94, 255, 0);
        internal static Color ZacurrentPinkAlpha = new(255, 115, 226, 0);
        internal static Color ZacurrentRed = new(255, 28, 110);
        internal static Color ZacurrentDustRed = new(255, 113, 160);
        /// <summary>
        /// 残影的透明度
        /// </summary>
        public float shadowAlpha = 1f;
        /// <summary>
        /// 残影的大小
        /// </summary>
        public float shadowScale = 1f;

        public readonly int trailCacheLength = 12;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 100;
            NPC.damage = 60;
            NPC.defense = 50;
            NPC.lifeMax = 85500;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 45, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = ModContent.GetInstance<ZacurrentDragonBossBar>();
            ModContent.GetInstance<ZacurrentDragonBossBar>().Reset(NPC);

            //BGM：暂无
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ThunderDragon");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            int expertBaseLife = 101254;
            int masterBaseLife = 116854;

            int expertAddLife = 17395;
            int masterAddLife = 28485;

            NPC.defDamage = 55;
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((expertBaseLife + (numPlayers * expertAddLife)) / journeyScale);
                    NPC.damage = 66;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((masterBaseLife + (numPlayers * masterAddLife)) / journeyScale);
                    NPC.damage = 72;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 1.6f;
                }

                return;
            }

            NPC.lifeMax = expertBaseLife + (numPlayers * expertAddLife);
            NPC.damage = 66;

            if (Main.masterMode)
            {
                NPC.lifeMax = masterBaseLife + (numPlayers * masterAddLife);
                NPC.damage = 72;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 92000 + (numPlayers * 25850);
                NPC.damage = 80;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 2.4f;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<ZacurrentRelic>()));
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            int width = (int)(95 * NPC.scale);
            int height = (int)(70 * NPC.scale);
            npcHitbox = new Rectangle((int)(NPC.Center.X - (width / 2)), (int)(NPC.Center.Y - (height / 2)), width, height);
            return true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (!IsDashing && currentSurrounding)
                return true;

            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Colliding(projectile.getRect(), HeadHitBox()))
                modifiers.SourceDamage += ZacurrentDirector.HeadHitBonus;

            if (projectile.hostile)
                modifiers.SourceDamage -= ZacurrentDirector.HostileProjReduction;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (PurpleVolt)
                modifiers.ModifyHitInfo += Modifiers_ModifyHitInfo;

            if (currentSurrounding)
            {
                modifiers.SourceDamage -= ZacurrentDirector.CurrentSurroundingReduction;
            }
        }

        public Rectangle HeadHitBox()
        {
            return Utils.CenteredRectangle(GetMousePos(), new Vector2(ZacurrentDirector.HeadHitBoxSize, ZacurrentDirector.HeadHitBoxSize));
        }

        /// <summary>紫伏期扣电：只登记击穿请求，换态由状态基类的 ServerUpdate 经返回值走。</summary>
        private void Modifiers_ModifyHitInfo(ref NPC.HitInfo info)
        {
            info.Damage = (int)(info.Damage * ZacurrentDirector.PurpleVoltDamageTaken);
            if (VaultUtils.isClient)
                return;

            PurpleVoltCount -= info.Damage * ZacurrentDirector.PurpleVoltBreakScale();
            if (PurpleVoltCount < 0)
            {
                PurpleVoltCount = 0;
                PurpleVolt = false;
                if (AiContext != null)
                    AiContext.BreakRequested = true;
                NPC.netUpdate = true;
            }
        }

        public override bool? CanFallThroughPlatforms() => true;

        /// <summary>
        /// 死亡拦截：不在这里换态。本地把血锁到 1 并无敌（命中方客户端与服务端都会跑到这里），
        /// 权威端登记 <see cref="ZacurrentDragonContext.KillRequested"/>，由状态基类的 ServerUpdate 经返回值切到击杀演出，客户端读 ai[0] 跟随。
        /// 演出结束时权威端 <c>NPC.Kill()</c> 再次进入这里，此时已在演出态 → 放行真死。
        /// </summary>
        public override bool CheckDead()
        {
            if (StateMachine == null || State == AIStates.onKillAnim)
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

        public override void OnKill()
        {
            DownedBossSystem.DownZacurrentDragon();
        }

        /// <summary>热字段（Timer / Counter / Beat / Recorder / Recorder2 / Combo）+ boss 事实（紫伏、紫电计数）随 SyncNPC 原子过线。</summary>
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

        #region AI

        /// <summary>
        /// 固定顺序（镜像 Rediancie）：懒构造 → 首帧初始化 → 客户端纠偏帧首 → 目标与脱战 → 天空 → 声明回默认
        /// → 状态机（只写声明，转移仅 ServerUpdate 返回值）→ 热字段兜底收养 → 落地运动 → 客户端记预测。
        /// </summary>
        public override void AI()
        {
            EnsureAiMachine();

            if (init)
            {
                Initialize();
                init = false;
            }

            if (VaultUtils.isClient)
                AiContext.Net.BeginClientFrame(NPC);

            if (CheckTarget())
            {
                if (VaultUtils.isClient)
                    AiContext.Net.EndClientFrame(NPC);
                return;
            }

            UpdateSky();

            AiContext.BeginFrameDefaults();
            StateMachine.Update();
            AiContext.ConsumePendingHotAdopt();

            ApplyDeclaredMovement();

            if (VaultUtils.isClient)
                AiContext.Net.EndClientFrame(NPC);
        }

        /// <summary>
        /// 懒构造；初态从 ai[0] 重建——中途加入的客户端靠这个不重放入场演出，未注册 id 回退登场态。
        /// 上下文构造里已置 <c>UseLegacySpeedValve = false</c>。
        /// </summary>
        private void EnsureAiMachine()
        {
            if (StateMachine != null)
                return;

            AiContext = new ZacurrentDragonContext(this);
            StateMachine = new CoraliteBossStateMachine<ZacurrentDragonContext>(AiContext);

            IVaultState<ZacurrentDragonContext> initial = VaultStateRegistry<ZacurrentDragonContext>.Create((int)NPC.ai[CoraliteBossContext.StateAiSlot])
                ?? VaultStateRegistry<ZacurrentDragonContext>.Create((int)AIStates.onSpawnAnmi);
            StateMachine.SetInitialState(initial);
        }

        /// <summary>目标与脱战判定照旧；返回 true 表示本帧已离场处理完，不再跑状态机。旧 ZcurrentAI.cs:206-226</summary>
        public bool CheckTarget()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active
                || Target.Distance(NPC.Center) > ZacurrentDirector.RetargetDistance || Main.dayTime)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > ZacurrentDirector.DespawnDistance || Main.dayTime)//没有玩家存活时离开
                {
                    NPC.dontTakeDamage = true;
                    canDrawShadows = false;
                    IsDashing = true;
                    NPC.velocity.X *= ZacurrentDirector.DespawnDampX;
                    NPC.velocity.Y = ZacurrentDirector.DespawnRiseY;
                    NPC.rotation = NPC.velocity.ToRotation();
                    NPC.EncourageDespawn(ZacurrentDirector.DespawnEncourageFrames);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 两端同跑：把本帧声明落地。招式体绝大多数自管速度（<c>Direct</c>），这里只负责统一的衰减模式与
        /// 原版不同步的 <c>dontTakeDamage</c>——凡是切换它的窗口都必须两端每帧声明。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            if (AiContext.MoveMode == ZacurrentMoveMode.Damp)
                NPC.velocity *= AiContext.DampFactor;

            NPC.dontTakeDamage = AiContext.Invulnerable || AiContext.KillRequested;
        }

        public void Initialize()
        {
            ResetAllOldCaches();
            NPC.netUpdate = true;

            if (!VaultUtils.isServer && !SkyManager.Instance["ZacurrentSky"].IsActive())//如果这个天空没激活
            {
                SkyManager.Instance.Activate("ZacurrentSky");
            }
        }

        public override void PostAI()
        {
            oldSpriteDirection = NPC.spriteDirection;

            if (!VaultUtils.isServer && currentSurrounding)
            {
                Lighting.AddLight(NPC.Center, ZacurrentPink.ToVector3());
                if (Main.rand.NextBool(3))
                {
                    Vector2 offset = Main.rand.NextVector2Circular(100 * NPC.scale, 70 * NPC.scale);
                    ElectricParticle_PurpleFollow.Spawn(NPC.Center, offset, () => NPC.Center, Main.rand.NextFloat(0.75f, 1f));
                }
            }
        }

        public static void UpdateSky()
        {
            if (VaultUtils.isServer)
                return;

            ZacurrentSky sky = (ZacurrentSky)SkyManager.Instance["ZacurrentSky"];
            if (sky.Timeleft < 100)
                sky.Timeleft += 2;
            if (sky.Timeleft > 100)
                sky.Timeleft = 100;
        }

        public static void SetBackgroundLight(float light, int fadeTime, int exchangeTime = 5)
        {
            if (VaultUtils.isServer)
                return;

            ZacurrentSky sky = (ZacurrentSky)SkyManager.Instance["ZacurrentSky"];
            sky.ExchangeTime = sky.MaxExchangeTime = exchangeTime;
            sky.targetLight = light;
            sky.oldLight = sky.light;
            sky.LightTime = fadeTime;
        }

        /// <summary>
        /// 获得紫电（仅权威端；<c>PurpleVoltBall</c> / <c>RedVoltBall</c> 被打碎时调用）。旧 ZcurrentAI.cs:510-531
        /// </summary>
        public void GetPurpleVolt(bool red)
        {
            if (VaultUtils.isClient)
                return;

            int count = red ? ZacurrentDirector.PurpleVoltRedGain() : ZacurrentDirector.PurpleVoltBallGain();

            PurpleVoltCount += count;
            if (PurpleVoltCount > GetPurpleVoltMax())
                PurpleVoltCount = GetPurpleVoltMax();

            NPC.netUpdate = true;
        }

        public int GetPurpleVoltMax() => ZacurrentDirector.PurpleVoltMax();

        #endregion

        #region 动画与运动小件（宿主级：帧图、朝向、残影缓存）

        /// <summary>一圈圈向外铺开的红色电粒子，死亡演出与击穿瞬间共用。纯视觉，服务端不跑。</summary>
        public static void BurstRing(Vector2 center)
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < ZacurrentDirector.BurstRingCount; i++)
            {
                float factor = i / (float)ZacurrentDirector.BurstRingCount;
                float length = Helper.Lerp(ZacurrentDirector.BurstRadiusMin, ZacurrentDirector.BurstRadiusMax, factor);

                for (int j = 0; j < ZacurrentDirector.BurstPerRing; j++)
                    PRTLoader.NewParticle(center + Main.rand.NextVector2CircularEdge(length, length),
                        Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Red>(),
                        Scale: Main.rand.NextFloat(ZacurrentDirector.BurstParticleScaleMin, ZacurrentDirector.BurstParticleScaleMax));
            }
        }

        /// <summary>
        /// 紫色蓄力电粒子（一半是发光球尘）。纯表现。<br/>
        /// 本体类上的公开静态成员，<c>Projectile.PurpleElectricBreath</c> 与 <c>Projectile.PurpleSmallThunderFall</c> 在用，名字与签名不变。
        /// 旧 AI.LightningRaidNormal.cs:235-241
        /// </summary>
        public static void PurpleElectricParticle(Vector2 pos)
        {
            if (Main.rand.NextBool())
            {
                PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Purple>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
            }
            else
            {
                Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ZacurrentPurple, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            }
        }

        /// <summary>紫伏版的红色蓄力电粒子。本体类上的公开静态成员，名字与签名不变。旧 AI.LightningRaidVolt.cs:259-265</summary>
        public static void RedElectricParticle(Vector2 pos)
        {
            if (Main.rand.NextBool())
            {
                PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle_Red>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
            }
            else
            {
                Dust.NewDustPerfect(pos, ModContent.DustType<LightningShineBall>(), Vector2.Zero, newColor: ZacurrentDustRed, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            }
        }

        public void ElectricSound()
        {
            Helper.PlayPitched("Electric/ElectricStrike" + Main.rand.NextFromList(0, 2).ToString(), 0.4f, -0.2f, NPC.Center);
        }

        public void GetLengthToTargetPos(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = Math.Abs(NPC.Center.X - targetPos.X);
            yLength = Math.Abs(NPC.Center.Y - targetPos.Y);
        }

        /// <summary>向上飞，会改变 Y 速度。</summary>
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

        /// <summary>获取嘴巴的位置（吐息 / 电球的出膛点）。</summary>
        public Vector2 GetMousePos()
            => NPC.Center + ((NPC.rotation + (NPC.direction * ZacurrentDirector.MouthAngleOffset)).ToRotationVector2() * ZacurrentDirector.MouthDistance * NPC.scale);

        public void FlyingFrame()
        {
            int frameCounterMax = 4;
            float speed = NPC.velocity.Length();
            if (speed > 8)
                frameCounterMax--;
            if (speed > 14)
                frameCounterMax--;

            if (++NPC.frameCounter > frameCounterMax)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }
        }

        /// <summary>根据 Y 方向速度设置旋转。</summary>
        public void SetRotationNormally(float rate = ZacurrentDirector.RotationRate)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += MathHelper.Pi;
            float targetRot = (NPC.velocity.Y * ZacurrentDirector.RotationPerSpeedY * NPC.spriteDirection) + (NPC.spriteDirection > 0 ? 0 : MathHelper.Pi);
            NPC.rotation = NPC.rotation.AngleLerp(targetRot, rate);
        }

        /// <summary>将身体回正。</summary>
        public void TurnToNoRot(float rate = ZacurrentDirector.TurnToNoRotRate)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += MathHelper.Pi;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.spriteDirection > 0 ? 0 : MathHelper.Pi, rate);
        }

        internal void SetSpriteDirectionFoTarget(Vector2? targetPos = null, float limit = ZacurrentDirector.SpriteFlipDeadZone)
        {
            Vector2 p = targetPos ?? Target.Center;
            if (MathF.Abs(p.X - NPC.Center.X) > limit)
                NPC.spriteDirection = p.X > NPC.Center.X ? 1 : -1;
        }

        public void InitOldFrame()
        {
            if (VaultUtils.isServer)
                return;

            oldFrame ??= new Point[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldFrame[i] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void InitOldDirection()
        {
            if (VaultUtils.isServer)
                return;

            oldDirection ??= new int[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldDirection[i] = NPC.spriteDirection;
        }

        public void UpdateOldFrame()
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < oldFrame.Length - 1; i++)
                oldFrame[i] = oldFrame[i + 1];
            oldFrame[^1] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void UpdateOldDirection()
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < oldDirection.Length - 1; i++)
                oldDirection[i] = oldDirection[i + 1];
            oldDirection[^1] = NPC.spriteDirection;
        }

        public void ResetAllOldCaches()
        {
            if (VaultUtils.isServer)
                return;

            NPC.InitOldPosCache(trailCacheLength);
            NPC.InitOldRotCache(trailCacheLength);
            InitOldFrame();
            InitOldDirection();
        }

        public void UpdateAllOldCaches()
        {
            if (VaultUtils.isServer)
                return;

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

            drawColor *= selfAlpha;
            var pos = NPC.Center - screenPos;
            float rot = NPC.rotation;

            SpriteEffects effects = SpriteEffects.None;

            if (NPC.spriteDirection < 0)
                effects = SpriteEffects.FlipVertically;

            //绘制残影
            if (canDrawShadows)
            {
                Color shadowColor = ZacurrentPurple;
                //shadowColor.A = 50;
                //shadowColor *= shadowAlpha;
                for (int i = 0; i < trailCacheLength; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i] - screenPos;
                    float oldrot = NPC.oldRot[i];
                    float factor = (float)i / trailCacheLength;
                    if (PurpleVolt)
                    {
                        Color c1 = ZacurrentPurpleAlpha with { A = 50 };
                        Color c2 = ZacurrentRed with { A = 50 };
                        shadowColor = Color.Lerp(c2, c1, factor);
                        shadowColor *= shadowAlpha;
                    }
                    else
                    {
                        Color c1 = ZacurrentPurpleAlpha with { A = 50 };
                        Color c2 = ZacurrentPinkAlpha with { A = 50 };
                        shadowColor = Color.Lerp(c2, c1, factor);
                        shadowColor *= shadowAlpha;
                    }

                    Color shadowColor2 = shadowColor * factor;
                    SpriteEffects oldEffect = oldDirection[i] > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

                    float oldScale = NPC.scale * shadowScale * (1 - ((1 - factor) * 0.3f));
                    int wingFrame = oldFrame[i].X == 0 ? oldFrame[i].Y : 8;
                    DrawBackWing(spriteBatch, mainTex, wingFrame, oldPos, shadowColor2, oldrot, oldScale, oldEffect);
                    DrawBody(spriteBatch, mainTex, oldFrame[i].X == 0 ? 0 : 1, oldPos, shadowColor2, oldrot, oldScale, oldEffect);
                    DrawHead(spriteBatch, mainTex, oldFrame[i].X == 0 ? 0 : 2, oldPos, shadowColor2, oldrot, oldScale, oldEffect);
                    DrawFrontWing(spriteBatch, mainTex, wingFrame, oldPos, shadowColor2, oldrot, oldScale, oldEffect);
                }
            }

            //绘制自己
            if (Main.zenithWorld)
                drawColor *= 0.2f;


            //绘制冲刺时的特效
            if (IsDashing)
            {
                //冲刺时使用特殊帧图
                DrawBackWing(spriteBatch, mainTex, 8, pos, drawColor, NPC.rotation, NPC.scale, effects);
                DrawBody(spriteBatch, mainTex, 1, pos, drawColor, NPC.rotation, NPC.scale, effects);
                DrawHead(spriteBatch, mainTex, 2, pos, drawColor, NPC.rotation, NPC.scale, effects);
                DrawFrontWing(spriteBatch, mainTex, 8, pos, drawColor, NPC.rotation, NPC.scale, effects);

                Texture2D exTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "StrikeTrail").Value;

                Vector2 exOrigin = new(exTex.Width * 6 / 10, exTex.Height / 2);

                Vector2 scale = new Vector2(1.3f, 1.5f) * NPC.scale;
                spriteBatch.Draw(exTex, pos, null, ZacurrentPurpleAlpha, rot
                    , exOrigin, scale, effects, 0);
                scale.Y *= 1.2f;
                spriteBatch.Draw(exTex, pos - (NPC.rotation.ToRotationVector2() * 50), null, ZacurrentPurpleAlpha * 0.5f, rot
                    , exOrigin, scale, effects, 0);
            }
            else
            {
                DrawBackWing(spriteBatch, mainTex, NPC.frame.Y, pos, drawColor, NPC.rotation, NPC.scale, effects);
                DrawBody(spriteBatch, mainTex, 0, pos, drawColor, NPC.rotation, NPC.scale, effects);
                DrawHead(spriteBatch, mainTex, OpenMouse ? 1 : 0, pos, drawColor, NPC.rotation, NPC.scale, effects);
                DrawFrontWing(spriteBatch, mainTex, NPC.frame.Y, pos, drawColor, NPC.rotation, NPC.scale, effects);
            }

            if (State == AIStates.onKillAnim)
            {
                Color whiteC = Color.White * shadowAlpha;
                DrawBackWing(spriteBatch, WhiteTex.Value, NPC.frame.Y, pos, whiteC, NPC.rotation, NPC.scale, effects);
                DrawBody(spriteBatch, WhiteTex.Value, 0, pos, whiteC, NPC.rotation, NPC.scale, effects);
                DrawHead(spriteBatch, WhiteTex.Value, OpenMouse ? 1 : 0, pos, whiteC, NPC.rotation, NPC.scale, effects);
                DrawFrontWing(spriteBatch, WhiteTex.Value, NPC.frame.Y, pos, whiteC, NPC.rotation, NPC.scale, effects);

                return false;
            }

            //var box = HeadHitBox();
            //var texxx = ModContent.Request<Texture2D>(AssetDirectory.DefaultItem).Value;
            //spriteBatch.Draw(texxx, box.TopLeft() - screenPos
            //    , null, Color.White * 0.5f, 0, Vector2.Zero,new Vector2( (float)box.Width / texxx.Width, (float)box.Height / texxx.Height), 0, 0);

            //if (State == (int)AIStates.onKillAnim)
            //{
            //    Texture2D whiteTex = ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Highlight").Value;

            //    spriteBatch.Draw(whiteTex, pos, frameBox, Color.White * anmiAlpha, rot, origin, NPC.scale, effects, 0);
            //}

            return false;
        }

        /// <summary>
        /// 绘制背后的翅膀
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mainTex"></param>
        /// <param name="pos"></param>
        /// <param name="frameX"></param>
        /// <param name="drawColor"></param>
        /// <param name="effects"></param>
        public void DrawBackWing(SpriteBatch spriteBatch, Texture2D mainTex, int frameY, Vector2 pos, Color drawColor, float rot, float scale, SpriteEffects effects)
        {
            Rectangle frameBox = new(0, frameY, 4, Main.npcFrameCount[NPC.type]);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, scale);
            //绘制glow
            GlowTex.Value.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, Color.White * selfAlpha * (drawColor.A / 255f), rot, scale);
        }

        /// <summary>
        /// 绘制身体
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mainTex"></param>
        /// <param name="pos"></param>
        /// <param name="frameX"></param>
        /// <param name="drawColor"></param>
        /// <param name="effects"></param>
        public void DrawBody(SpriteBatch spriteBatch, Texture2D mainTex, int frameY, Vector2 pos, Color drawColor, float rot, float scale, SpriteEffects effects)
        {
            Rectangle frameBox = new(1, frameY, 4, Main.npcFrameCount[NPC.type]);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, scale);
            //绘制glow
            GlowTex.Value.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, Color.White * selfAlpha * (drawColor.A / 255f), rot, scale);
        }

        /// <summary>
        /// 绘制头
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mainTex"></param>
        /// <param name="pos"></param>
        /// <param name="frameX"></param>
        /// <param name="drawColor"></param>
        /// <param name="effects"></param>
        public void DrawHead(SpriteBatch spriteBatch, Texture2D mainTex, int frameY, Vector2 pos, Color drawColor, float rot, float scale, SpriteEffects effects)
        {
            Rectangle frameBox = new(2, frameY, 4, Main.npcFrameCount[NPC.type]);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, scale);
            //绘制glow
            GlowTex.Value.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, Color.White * selfAlpha * (drawColor.A / 255f), rot, scale);
        }

        /// <summary>
        /// 绘制前面的翅膀
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mainTex"></param>
        /// <param name="pos"></param>
        /// <param name="drawColor"></param>
        /// <param name="effects"></param>
        public void DrawFrontWing(SpriteBatch spriteBatch, Texture2D mainTex, int frameY, Vector2 pos, Color drawColor, float rot, float scale, SpriteEffects effects)
        {
            Rectangle frameBox = new(3, frameY, 4, Main.npcFrameCount[NPC.type]);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, scale);
            //绘制glow
            GlowTex.Value.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, Color.White * selfAlpha * (drawColor.A / 255f), rot, scale);
        }

        #endregion
    }
}
