using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
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
    /// 史莱姆皇帝，加强版史莱姆王<br></br>
    /// <br></br>
    /// “蚩尤说：又是这个囊地过分的波斯”<br></br>
    /// “300颗够吗，应该够了吧”<br></br>
    /// “这个武器打这个BOSS，从来没试过哦”<br></br>
    /// “这个波斯对我来说超囊的”<br></br>
    /// “来吧，试一下米妮”<br></br>
    /// “诶呀，亡了亡了，我没有史莱姆ang啊”<br></br>
    /// <br></br>
    ///    591 60 15 3<br></br>
    /// <br></br>
    /// 粘滑生物真正的领袖，史莱姆王？不过是个小弟罢了<br></br>
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
    [VaultLoaden(AssetDirectory.SlimeEmperor)]
    public partial class SlimeEmperor : ModNPC
    {
        public override string Texture => AssetDirectory.SlimeEmperor + Name;

        private Player Target => Main.player[NPC.target];

        // ai 槽全部归基座约定：ai[0]=状态ID（AiSlotNetSync 同步）、ai[1]=AttackSeed、ai[2]=SonState、ai[3]=SyncTimer。
        // 旧的 movePhase / movingMode / shoot2State / melee2State / localAI[0..1] 已搬到 SlimeEmperorContext，随热字段与同步事实过线。
        internal SlimeEmperorContext AiContext;
        internal CoraliteBossStateMachine<SlimeEmperorContext> StateMachine;

        /// <summary>当前顶层状态 ID；状态机未建立时读 ai[0]，中途加入者在第一帧 AI 之前也拿得到。</summary>
        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)NPC.ai[CoraliteBossContext.StateAiSlot];

        /// <summary>血量比例，钳在 0.65~1；体型、跳跃力度、绘制缩放都吃它。</summary>
        private float LifePercentScale => Math.Clamp(NPC.life / (float)NPC.lifeMax, SlimeEmperorDirector.LifeScaleMin, 1);

        /// <summary>纯属视觉效果的缩放；招式拍点拿它当完成判据，所以随热字段过线（<c>SlimeEmperorStateBase.WriteHot</c>）。</summary>
        internal Vector2 Scale;
        private CrownDatas crown;
        private bool span;

        [VaultLoaden("{@classPath}" + "SlimeEmperorCrown")]
        public static ATex CrownTex { get; private set; }

        public static Color BlackSlimeColor = Color.Black;

        private Slime1Knowledge _Knowledge;
        public Slime1Knowledge Knowledge
        {
            get
            {
                _Knowledge ??= (Slime1Knowledge)CoraliteContent.GetKnowledge<Slime1Knowledge>();
                return _Knowledge;
            }
        }

        /// <summary>
        /// 危险挑战
        /// </summary>
        public bool DangerousChallenge { get; set; }

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
            //手感缩放随热字段过线，必须在收包之前就有值——放在 Initialize 里会把中途加入时刚收养的缩放又抹回 1
            Scale = Vector2.One;

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
            int expertBaseLife = 6400;
            int MasterBaseLife = 7600;

            int expertMultLife = 2060;
            int masterMultLife = 3030;

            int expertDefence = 14;
            int masterDefence = 20;

            //if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            //{
            //    if (nPCStrengthHelper.IsExpertMode)
            //    {
            //        NPC.lifeMax = (int)((expertBaseLife + (numPlayers * expertMultLife)) / journeyScale);
            //        NPC.damage = 75;
            //        NPC.defense = expertDefence;
            //    }

            //    if (nPCStrengthHelper.IsMasterMode)
            //    {
            //        NPC.lifeMax = (int)((MasterBaseLife + (numPlayers * masterMultLife)) / journeyScale);
            //        NPC.scale *= 1.25f;
            //        NPC.defense = masterDefence;
            //        NPC.damage = 100;
            //    }

            //    if (Main.getGoodWorld)
            //    {
            //        NPC.damage = 120;
            //        NPC.scale *= 1.25f;
            //        NPC.defense = 24;
            //    }

            //    return;
            //}

            NPC.lifeMax = expertBaseLife + (numPlayers * expertMultLife);
            NPC.damage = 75;
            NPC.defense = expertDefence;

            if (Main.masterMode)
            {
                NPC.lifeMax = MasterBaseLife + (numPlayers * masterMultLife);
                NPC.scale *= 1.25f;
                NPC.defense = masterDefence;
                NPC.damage = 100;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 8800 + (numPlayers * 4060);
                NPC.damage = 140;
                NPC.scale *= 1.25f;
                NPC.defense = 24;
            }


            if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.LifeMaxBonus_3))
                NPC.lifeMax *= 2;
            else if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.LifeMaxBonus_2))
                NPC.lifeMax = (int)(NPC.lifeMax * 1.75f);
            else if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.LifeMaxBonus_1))
                NPC.lifeMax = (int)(NPC.lifeMax * 1.5f);

            if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.DefenceBonus_2))
                NPC.defense += 15;
            else if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.DefenceBonus_1))
                NPC.defense += 8;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<GelThrone>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<SovereignSip>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<SlimeEmperorSoulBox>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<RoyalGelCannon>(), 10));
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

            //王冠gore
            GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.SlimeEmperor + "SlimeEmperorCrown");
        }

        public override void OnKill()
        {
            DownedBossSystem.DownSlimeEmperor();
            if (DangerousChallenge)
                Knowledge.RecordChallengeAndNewTip(Color.SkyBlue, Slime1DangerousPage.Title);
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

        /// <summary>
        /// 死亡拦截：不在这里换态。<c>CheckDead</c> 在命中方客户端上也会跑，本地只把血锁到 1 并无敌；
        /// 权威端登记 <see cref="SlimeEmperorContext.KillRequested"/>，由状态基类的 ServerUpdate 经返回值切到死亡演出，客户端读 ai[0] 跟随。
        /// 演出结束时权威端 <c>NPC.Kill()</c> 再次进来，此时已在演出态 → 放行真死。
        /// </summary>
        public override bool CheckDead()
        {
            if (StateMachine == null || CurrentStateId == (int)AIStates.OnKillAnim)
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

        /// <summary>
        /// 首招固定为泰山压顶（沿用旧 <c>SetInitialState</c>）。写进 ai[0] 而不是构造时硬塞，
        /// 是因为 ai[0] = 0 本身是一个合法状态（凝胶射击），中途加入的客户端只能靠这个槽分辨“开局”与“正在打”。
        /// </summary>
        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[CoraliteBossContext.StateAiSlot] = (int)AIStates.BodySlam;
        }

        public override bool? CanFallThroughPlatforms() => NPC.Center.Y < (Target.Center.Y - NPC.height);

        #endregion

        #region AI
        public void Initialize()
        {
            //CanUseHealGelBall = true;
            if (Knowledge.GeCurrentDangerous() > 0)
                DangerousChallenge = true;

            if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.HitLimit_3))
                Helper.StartHitLimitChallenge(SlimeEmperorDirector.HitLimitNormal, OnChallengeFail);
            else if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.HitLimit_S_5))
                Helper.StartHitLimitChallenge(SlimeEmperorDirector.HitLimitStrict, OnChallengeFail);

            if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.WeaponLimit_4))
                foreach (var p in Main.ActiveProjectiles)
                    if (p.friendly)
                        p.Kill();

            crown = new CrownDatas
            {
                Bottom = NPC.Top + new Vector2(0, SlimeEmperorDirector.CrownSpawnOffsetY)
            };
            NPC.TargetClosest(false);
            //首招在 OnSpawn 里就写进 ai[0] 了，这里只是补一次同步
            if (!VaultUtils.isClient)
                NPC.netUpdate = true;
        }

        public void OnChallengeFail()
        {
            DangerousChallenge = false;
            Main.NewText(KnowledgeSystem.ChallengeFailText.Value, Color.Red);

            int selfType = NPCType<SlimeEmperor>();
            int Fly = NPCType<GelFlippy>();
            int Ava = NPCType<SlimeAvatar>();
            int EBall = NPCType<ElasticGelBall>();

            foreach (var n in Main.ActiveNPCs)
                if (n.type == selfType || n.type == Fly || n.type == Ava || n.type == EBall)
                    n.InstanceKill();

            int GBall = ProjectileType<GelBall>();
            int SpikeBall = ProjectileType<SpikeGelBall>();
            int Spike = ProjectileType<GelSpike>();
            int SmallBall = ProjectileType<SmallGelBall>();
            int Sticky = ProjectileType<StickyGel>();
            int Gel = ProjectileType<GelProj>();

            foreach (var p in Main.ActiveProjectiles)
                if (p.type == GBall || p.type == SpikeBall || p.type == Spike || p.type == SmallBall || p.type == Sticky || p.type == Gel)
                    p.Kill();
        }

        /// <summary>
        /// 固定顺序：懒构造 → 客户端纠偏帧首 → 目标与脱战 → 只读事实 → 声明回默认 → 状态机 → 热字段兜底收养 → 落地运动 → 客户端记预测。<br/>
        /// 招式体全在 <c>States/</c>，这里不留任何 AI 逻辑。
        /// </summary>
        public override void AI()
        {
            EnsureAiMachine();

            if (!span)
            {
                Initialize();
                span = true;
            }

            if (VaultUtils.isClient)
                AiContext.Net.BeginClientFrame(NPC);

            if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.SpeedBonus1_1))
                NPC.GravityMultiplier *= SlimeEmperorDirector.GravityBonusMult;

            if (!FindTarget())
            {
                //脱战：没有玩家存活时向上飘走（两端同算的运动数学，不经状态机）
                NPC.noGravity = true;
                NPC.noTileCollide = true;
                NPC.velocity.Y -= SlimeEmperorDirector.DespawnRiseAccel;
                AiContext.DrawShadow = true;
                NPC.EncourageDespawn(SlimeEmperorDirector.DespawnEncourageFrames);

                if (VaultUtils.isClient)
                    AiContext.Net.EndClientFrame(NPC);
                return;
            }

            AiContext.UpdateFacts();
            AiContext.BeginFrameDefaults();

            //状态只写声明；转移仅 ServerUpdate 返回值，客户端由 ai[0] 跟随
            StateMachine.Update();
            AiContext.ConsumePendingHotAdopt();

            ApplyDeclaredMovement();

            if (VaultUtils.isClient)
                AiContext.Net.EndClientFrame(NPC);
        }

        /// <summary>
        /// 目标与脱战判定照旧；重新锁到目标时登记一次收招请求（旧 <c>AI()</c> 里那句 <c>ResetStates()</c>），
        /// 真正的换态仍只发生在权威端的 ServerUpdate。返回 false 表示该离场。沿用旧值 SlimeEmperor.cs:375-390
        /// </summary>
        private bool FindTarget()
        {
            if (NPC.target >= 0 && NPC.target != 255 && !Target.dead && Target.active)
                return true;

            NPC.TargetClosest();
            if (Target.dead || !Target.active)
                return false;

            //收招请求只在权威端登记，客户端跟着 ai[0] 走就行
            if (!VaultUtils.isClient)
                AiContext.RetargetRequested = true;

            return true;
        }

        /// <summary>
        /// 懒构造；初态从 ai[0] 重建——中途加入的客户端靠这个不重放开场，未注册 id 回退泰山压顶（<c>OnSpawn</c> 写的首招）。
        /// 上下文构造里已置 <c>UseLegacySpeedValve = false</c>。
        /// </summary>
        private void EnsureAiMachine()
        {
            if (StateMachine != null)
                return;

            AiContext = new SlimeEmperorContext(this);
            StateMachine = new CoraliteBossStateMachine<SlimeEmperorContext>(AiContext);

            IVaultState<SlimeEmperorContext> initial = VaultStateRegistry<SlimeEmperorContext>.Create((int)NPC.ai[CoraliteBossContext.StateAiSlot])
                ?? VaultStateRegistry<SlimeEmperorContext>.Create((int)AIStates.BodySlam);
            StateMachine.SetInitialState(initial);
        }

        /// <summary>
        /// 两端同跑：把本帧声明翻译成速度与各种原版不同步的标志。<br/>
        /// 王冠形态的每帧效果（防御、霸体、弹幕反弹、无重力穿墙、方形判定盒）也在这里按 <see cref="SlimeEmperorContext.CrownForm"/> 重算——
        /// 旧代码在 <c>CrownMode()</c> 里设一次就不管了，客户端被 NetSync 切进王冠招式时根本没跑过那一次。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            SlimeEmperorContext ctx = AiContext;

            if (ctx.MoveMode == SlimeEmperorMoveMode.Damp)
                NPC.velocity *= ctx.DampFactor;

            bool superArmor = ctx.SuperArmor;
            bool reflects = ctx.ReflectsProjectiles;

            if (ctx.CrownForm)
            {
                int bonus = SlimeEmperorDirector.CrownDefenseBonus;
                if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.CrownBonus_1))
                    bonus = SlimeEmperorDirector.CrownDefenseBonusChallenge;
                else if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.CrownBonus_S_2))
                {
                    bonus = SlimeEmperorDirector.CrownDefenseBonusInvincible;
                    superArmor = true;
                    reflects = true;
                }

                NPC.defense = NPC.defDefense + bonus;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                ApplyCrownHitbox();
            }
            else
                NPC.defense = NPC.defDefense;

            //原版不同步这三个标志，两端按同一份声明每帧落地；死亡请求期间保持无敌直到演出态接管
            NPC.SuperArmor = superArmor;
            NPC.reflectsProjectiles = reflects;
            NPC.dontTakeDamage = ctx.Invulnerable || ctx.KillRequested;
        }

        /// <summary>王冠形态的方形判定盒，只在尺寸变化时重算并保住中心。沿用旧值 SlimeEmperor.cs:944-946</summary>
        private void ApplyCrownHitbox()
        {
            int size = (int)(SlimeEmperorDirector.CrownHitboxSize * NPC.scale);
            if (NPC.width == size && NPC.height == size)
                return;

            Vector2 center = NPC.Center;
            NPC.width = NPC.height = size;
            NPC.Center = center;
        }

        /// <summary>死亡演出用：掉王冠 gore（纯本地，由 <c>SlimeEmperorOnKillAnimState</c> 调）。沿用旧值 SlimeEmperor.cs:422-430</summary>
        internal void SpawnCrownGore()
        {
            Gore gore = Gore.NewGoreDirect(NPC.GetSource_Death(), crown.Bottom, Main.rand.NextVector2Circular(1, 1), Mod.Find<ModGore>("SlimeEmperorCrown").Type);
            gore.scale = NPC.scale;
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

            //常规形态的判定盒跟着血量缩；王冠形态的方形判定盒归 ApplyCrownHitbox
            if (!(AiContext?.CrownForm ?? false))
            {
                int newWidth = (int)(LifePercentScale * NPC.scale * SlimeEmperorDirector.BodyWidthMax);
                int newHeight = (int)(LifePercentScale * NPC.scale * SlimeEmperorDirector.BodyHeightMax);
                if (NPC.width != newWidth || NPC.height != newHeight)
                {
                    Vector2 bottom = NPC.Bottom;
                    NPC.width = newWidth;
                    NPC.height = newHeight;
                    NPC.Bottom = bottom;
                }
            }

            //王冠是纯绘制物，服务端不用算（C8：表现不碰物理，没有任何玩法量读它）
            if (!Main.dedServ)
                UpdateCrown();

            if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.WeaponLimit_4)
                && Helper.WeaponLimitChallenge(SlimeEmperorDirector.WeaponLimitScore, ItemRarityID.LightRed))
                OnChallengeFail();

            if (Knowledge.DangerousSet(Slime1Knowledge.Dangerous.ArmorLimit_4)
                && Helper.ArmorLimitChallenge(SlimeEmperorDirector.ArmorLimitScore, ItemRarityID.Pink))
                OnChallengeFail();
        }

        /// <summary>王冠的独立小物理：常规形态自由落体 + 落地弹起 + 回正，王冠形态自旋。沿用旧值 SlimeEmperor.cs:488-536</summary>
        private void UpdateCrown()
        {
            if (AiContext?.CrownForm ?? false)
            {
                crown.Rotation += SlimeEmperorDirector.CrownSpinSpeed;
                return;
            }

            float groundHeight = CrownRestHeight();
            crown.Bottom.X = MathHelper.Lerp(crown.Bottom.X, NPC.Center.X, SlimeEmperorDirector.CrownFollowLerpX);

            if (crown.Bottom.Y < groundHeight - SlimeEmperorDirector.CrownGroundEpsilon)
            {
                crown.Velocity_Y += NPC.gravity * SlimeEmperorDirector.CrownGravityMult;
                if (crown.Velocity_Y > SlimeEmperorDirector.CrownFallMax)
                    crown.Velocity_Y = SlimeEmperorDirector.CrownFallMax;
            }

            crown.Bottom.Y += crown.Velocity_Y;
            if (crown.Bottom.Y > groundHeight)
            {
                crown.Bottom.Y = groundHeight;
                //落得够快就弹一下并歪一个随机角度，否则直接跟着本体停住
                if (NPC.velocity.Y < SlimeEmperorDirector.CrownBounceBodySpeed && crown.Velocity_Y > SlimeEmperorDirector.CrownBounceSpeed)
                {
                    float angle = CrownTiltAngle();
                    crown.Rotation = Main.rand.NextFloat(-angle, angle);
                    crown.Velocity_Y *= SlimeEmperorDirector.CrownBounceFactor;
                }
                else
                    crown.Velocity_Y = NPC.velocity.Y;
            }

            crown.Rotation = crown.Rotation.AngleLerp(0, SlimeEmperorDirector.CrownRotationBackLerp);
        }

        /// <summary>王冠停在头顶时的世界 Y 坐标。</summary>
        private float CrownRestHeight() => NPC.Bottom.Y - (Scale.Y * GetCrownBottom());

        /// <summary>按落速换算的倾角。沿用旧值 SlimeEmperor.cs:521</summary>
        private float CrownTiltAngle()
            => Math.Clamp(crown.Velocity_Y / SlimeEmperorDirector.CrownAngleDiv, SlimeEmperorDirector.CrownAngleMin, SlimeEmperorDirector.CrownAngleMax);

        /// <summary>把王冠按住贴在身体上（缩进王冠的四拍每帧调）。沿用旧值 AI.CrownStrike.cs:196-201</summary>
        internal void PinCrownToBody() => crown.Bottom.Y = CrownRestHeight();

        /// <summary>变回史莱姆时王冠归位并随机一个倾角。沿用旧值 SlimeEmperor.cs:962-964</summary>
        internal void ResetCrownToTop()
        {
            crown.Velocity_Y *= 0;
            crown.Rotation = Main.rand.NextFloat(MathHelper.Pi + (MathHelper.Pi / 4), MathHelper.TwoPi - (MathHelper.Pi / 4)) + (MathHelper.Pi / 2);
            crown.Bottom = NPC.Top;
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

        /// <summary>把王冠往上顶一下（落地回弹、砸地回弹时调）。沿用旧值 SlimeEmperor.cs:563-570</summary>
        internal void CrownJumpUp(float velLimit, float jumpUpSpeed)
        {
            if (Math.Abs(crown.Velocity_Y) < velLimit)
                crown.Velocity_Y -= jumpUpSpeed;

            float angle = CrownTiltAngle();
            crown.Rotation = Main.rand.NextFloat(-angle, angle);
        }

        #endregion

        #region States

        internal enum AIStates : int
        {
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
            HealGelBall = 11,

            //负值状态会被 AiSlotNetSync 跳过同步，迁移后改用非负 ID 以保证多人下死亡/出生状态可同步
            OnSpawnAnim = 12,
            OnKillAnim = 13,

            /// <summary> 连接段 + 唯一提交口（迁移时新增，取未用值） </summary>
            Hub = 14,
        }

        //三张轮换表与它们的阶段枚举已原样搬进 SlimeEmperorHubState（表内顺序、分支条件、游标推进逐字照搬），
        //这里不再留第二份实现；收招统一经 hub 的 Commit 出去。

        #endregion

        #region NetWork

        /// <summary>
        /// 热字段（Timer / Counter / Beat + 跳跃机与手感缩放）与 boss 事实（王冠形态、三个轮换游标）随 SyncNPC 原子过线。<br/>
        /// 接线就这两行，具体顺序与收养时机全在基座 <c>CoraliteBossContext.WriteNet / ReadNet</c> 里。
        /// </summary>
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

        #region Draw

        //public override void DrawBehind(int index)
        //{
        //    Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        //}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Texture2D crownTex = CrownTex.Value;

            Vector2 crownOrigin;
            Vector2 crownPos;

            if (Main.zenithWorld)
                drawColor = BlackSlimeColor;

            //表现偏移只加在绘制位上，判定盒不跟抖（C8）
            Vector2 drawOffset = AiContext?.DrawOffset ?? Vector2.Zero;

            if (AiContext?.CrownForm ?? false)
            {
                if (NPC.reflectsProjectiles)
                    drawColor = Color.Lerp(drawColor, Color.Red, 0.4f);

                crownOrigin = crownTex.Size() / 2;
                crownPos = NPC.Center + drawOffset - screenPos;
            }
            else
            {
                crownOrigin = new Vector2(crownTex.Width / 2, crownTex.Height);
                crownPos = crown.Bottom + drawOffset - screenPos;

                //绘制本体，以底部为中心进行绘制
                Vector2 scale = Scale * NPC.scale * LifePercentScale;
                Vector2 offset = new Vector2(0, 4 * scale.Y) + drawOffset - Main.screenPosition;

                DrawSelf(spriteBatch, drawColor, mainTex, scale, NPC.Bottom + offset);
                if (AiContext?.DrawShadow ?? false)
                {
                    Vector2 toBottom = new(NPC.width / 2, NPC.height);
                    Rectangle ShadowFrame = mainTex.Frame(4, Main.npcFrameCount[Type], 3, NPC.frame.Y);
                    Vector2 origin = new(ShadowFrame.Width / 2, ShadowFrame.Height);

                    for (int i = 1; i < 12; i += 2)
                    {
                        spriteBatch.Draw(mainTex, NPC.oldPos[i] + toBottom + offset, ShadowFrame, drawColor * (0.4f - (i * 0.04f)), NPC.rotation, origin, scale, 0, 0f);
                    }
                }
            }

            //绘制王冠
            spriteBatch.Draw(crownTex, crownPos, null, drawColor, crown.Rotation, crownOrigin, NPC.scale, 0, 0f);

            return false;
        }

        private void DrawSelf(SpriteBatch spriteBatch, Color drawColor, Texture2D mainTex, Vector2 scale, Vector2 pos)
        {
            Rectangle frameBox = mainTex.Frame(4, Main.npcFrameCount[Type], 3, NPC.frame.Y);
            Vector2 origin = new(frameBox.Width / 2, frameBox.Height);

            //底层
            spriteBatch.Draw(mainTex, pos, frameBox, drawColor * 0.7f, NPC.rotation, origin, scale, 0, 0f);
            //次底层
            frameBox = mainTex.Frame(4, Main.npcFrameCount[Type], 2, NPC.frame.Y);
            spriteBatch.Draw(mainTex, pos, frameBox, drawColor * 0.8f, NPC.rotation, origin, scale, 0, 0f);
            //中层
            frameBox = mainTex.Frame(4, Main.npcFrameCount[Type], 1, NPC.frame.Y);
            spriteBatch.Draw(mainTex, pos, frameBox, drawColor * 0.9f, NPC.rotation, origin, scale, 0, 0f);
            //上层
            frameBox = mainTex.Frame(4, Main.npcFrameCount[Type], 0, NPC.frame.Y);
            spriteBatch.Draw(mainTex, pos, frameBox, drawColor, NPC.rotation, origin, scale, 0, 0f);
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
