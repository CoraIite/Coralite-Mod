using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public sealed partial class NightmarePlantera : ModNPC, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private Player Target => Main.player[NPC.target];

        // ai[0]=平坦状态 id（FSM 同步，永不手写），ai[1]=AttackSeed，ai[2]=SonState，ai[3]=SyncTimer
        // ai[2]/ai[3] 只剩尚未拆平的二阶段旧招式 switch 在用；迁移完的状态走热字段。
        internal ref float Phase => ref NPC.ai[0];
        internal ref float SonState => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];
        internal ref float State => ref NPC.localAI[0];
        internal ref float MoveCount => ref NPC.localAI[1];

        internal NightmarePlanteraContext AiContext;
        internal CoraliteBossStateMachine<NightmarePlanteraContext> StateMachine;
        internal Random AttackRandom;
        private bool aiBootstrapped;

        /// <summary>当前平坦状态 id；中途加入的客户端也能直接从 ai[0] 读出来。</summary>
        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)NPC.ai[0];

        /// <summary>当前状态所属的宏观阶段。BOSS 头像 / 伤害修正 / 外部弹幕都读它，不再直接比 ai[0]。</summary>
        internal int CurrentMacroPhase => (int)NightmarePlanteraStateBase.MacroPhaseOf(CurrentStateId);

        public float EXai1;
        public float ShootCount;
        public int tentacleStarFrame;
        internal bool useMeleeDamage;
        public bool canOnlyBeHitByFantasyGod;
        public RotateTentacle[] rotateTentacles;
        public Color tentacleColor;
        private bool span;

        //public static FlowerParticle[] particles_front;
        //public static FlowerParticle[] particles_ffront;

        public static int phase2HeadSlot = -1;
        public static int phase3HeadSlot = -1;

        /// <summary>
        /// 击杀美梦光的数量
        /// </summary>
        public int fantasyKillCount;

        public static Asset<Texture2D> tentacleTex;
        public static Asset<Texture2D> tentacleFlowTex;
        public static Asset<Texture2D> waterFlowTex;
        public static Asset<Texture2D> flowerParticleTex;

        /// <summary> 自身BOSS的索引，用于方便爪子获取自身/ </summary>
        public static int NPBossIndex = -1;

        public float alpha = 1f;

        public static Color[] phantomColors;
        public static Color nightPurple = new(204, 170, 242, 230);
        public static Color lightPurple = new(195, 116, 219, 230);
        public static Color nightmareSparkleColor = new(111, 80, 180, 230);
        public static Color nightmareRed = new(250, 0, 100);

        #region tml hooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;

            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = 12;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            //NPCID.Sets.DebuffImmunitySets[Type] = new NPCDebuffImmunityData()
            //{
            //    SpecificallyImmuneTo = new int[]
            //    {
            //        BuffID.PotionSickness,
            //        BuffID.OnFire,
            //        BuffID.Bleeding,
            //        BuffID.Ichor,
            //        BuffID.Venom,
            //        BuffID.OnFire3,
            //        BuffID.BloodButcherer,
            //        BuffID.Confused
            //    }
            //};

            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 88;
            NPC.height = 88;
            NPC.lifeMax = 20_0000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.damage = 80;
            NPC.defense = 35;
            NPC.lifeMax = 5_0000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 20f;
            NPC.value = Item.buyPrice(0, 50);
            NPC.HitSound = CoraliteSoundID.Fleshy_NPCHit1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            //NPC.hide = true;

            if (VisualEffectSystem.UseNightmareBossBar)
                NPC.BossBar = ModContent.GetInstance<NPBossBar>();
            else if (Main.BigBossProgressBar.TryGetSpecialVanillaBossBar(NPCID.KingSlime, out var bar))
                NPC.BossBar = bar;

            //BGM：来世-世纪之花
            if (!Main.dedServ)
                Music = MusicID.OtherworldlyPlantera;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((23_5000 + (numPlayers * 5_4000)) / journeyScale);
                    NPC.damage = 100;
                    NPC.defense = 35;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((30_8000 + (numPlayers * 7_8000)) / journeyScale);
                    NPC.defense = 55;
                    NPC.damage = 120;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 140;
                    NPC.defense = 65;
                }

                return;
            }

            NPC.lifeMax = 23_5000 + (numPlayers * 5_4000);
            NPC.damage = 100;
            NPC.defense = 55;

            if (Main.masterMode)
            {
                NPC.lifeMax = 30_8000 + (numPlayers * 7_8000);
                NPC.defense = 55;
                NPC.damage = 120;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 43_2000 + (numPlayers * 10_0000);
                NPC.damage = 140;
                NPC.defense = 65;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<NightmareBed>()));
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<RedianciePet>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<GriefSeed>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NightmarePlanteraMask>(), 7));

            //掉落磷叶石，之后记得删掉
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Phosphophyllite>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ImpactCapsule>(), 20));

            //npcLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<NightmareHeart>()));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            IItemDropRule[] weaponTypes = [
                ItemDropRule.Common(ModContent.ItemType<LostSevensideHook>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<DreamShears>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<EuphorbiaMilii>(), 1, 1, 1),

                ItemDropRule.Common(ModContent.ItemType<Lycoris>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<BoneRing>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<QueensWreath>(), 1, 1, 1),

                ItemDropRule.Common(ModContent.ItemType<DevilsClaw>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<BarrenThornsStaff>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<Lullaby>(), 1, 1, 1),

                ItemDropRule.Common(ModContent.ItemType<PurpleToeStaff>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<Dreamcatcher>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<Eden>(), 1, 1, 1),
            ];

            notExpertRule.OnSuccess(new FewFromRulesRule(2, 1, weaponTypes));

            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            //if ((projectile.penetrate < 0 || projectile.penetrate > 1) && modifiers.DamageType != DamageClass.Melee)
            //    modifiers.SourceDamage *= 0.75f;
            //if (projectile.type == ProjectileID.FinalFractal)
            //{
            //    modifiers.SourceDamage *= 0.6f;
            //    return;
            //}

            if (projectile.hostile)
                modifiers.SetMaxDamage(1);

            //if (projectile.type == ModContent.ProjectileType<HyacinthBullet>() || projectile.type == ModContent.ProjectileType<HyacinthBullet2>()
            //    || projectile.type == ModContent.ProjectileType<HyacinthExplosion>())
            //{
            //    modifiers.SourceDamage *= 0.6f;
            //    return;
            //}

            modifiers.ModifyHitInfo += Modifiers_ModifyHitInfo;
        }

        private void Modifiers_ModifyHitInfo(ref NPC.HitInfo info)
        {
            if (CurrentMacroPhase == (int)AIPhases.Dream_P2 && NPC.life < NPC.lifeMax / 5)
            {
                info.Damage = 1;
            }
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            tentacleTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "Tentacle");
            tentacleFlowTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "TentacleFlow");
            CircleWarpTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "CircleWarp");
            BlackBack = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "BlackBack");
            NameLine = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NPNameLine");
            waterFlowTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "WaterFlow");
            flowerParticleTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "FlowerParticle");

            phantomColors = new Color[7];
            for (int i = 0; i < 7; i++)
            {
                phantomColors[i] = Main.hslToRgb(i * 1 / 7f, 45 / 100f, 30 / 100f, 200);
            }

            phase2HeadSlot = Mod.AddBossHeadTexture(AssetDirectory.NightmarePlantera + "Phase2Head", -1);
            phase3HeadSlot = Mod.AddBossHeadTexture(AssetDirectory.NightmarePlantera + "Phase3Head", -1);

        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            tentacleTex = null;
            tentacleFlowTex = null;
            CircleWarpTex = null;
            BlackBack = null;
            NameLine = null;
            waterFlowTex = null;
            flowerParticleTex = null;
        }

        public override bool PreKill()
        {
            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, NPC.Center, pitch: -0.5f);

            for (int i = 0; i < 24; i++)
            {
                Color color = Main.rand.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => nightmareRed
                };

                PRTLoader.NewParticle(NPC.Center + Main.rand.NextVector2Circular(64, 64), Helper.NextVec2Dir(6, 24f),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: Main.rand.NextFloat(0.5f, 1.5f));
            }

            return base.PreKill();
        }

        public override void OnKill()
        {
            NPBossIndex = -1;
            DownedBossSystem.DownNightmarePlantera();
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (canOnlyBeHitByFantasyGod)
                return projectile.type == ModContent.ProjectileType<FantasyBall>();

            return null;
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            if (canOnlyBeHitByFantasyGod)
                return false;

            return true;
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            if (canOnlyBeHitByFantasyGod)
                return false;

            return null;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<FantasyBall>() || projectile.type == ModContent.ProjectileType<FantasySpike>())
            {
                NPC.rotation += Main.rand.NextFloat(-0.2f, 0.2f);
            }
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
            //if (State != (int)AIStates.OnKillAnim)
            //{
            //    State = (int)AIStates.OnKillAnim;
            //    Timer = 0;
            //    NPC.dontTakeDamage = true;
            //    NPC.life = 1;
            //    return false;
            //}

            return true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => useMeleeDamage;

        public override void BossHeadSlot(ref int index)
        {
            int macro = CurrentMacroPhase;
            if (macro == (int)AIPhases.Dream_P2 && phase2HeadSlot != -1)
            {
                index = phase2HeadSlot;
                return;
            }

            if ((macro == (int)AIPhases.Nightemare_P3 || macro == (int)AIPhases.WakeUp_P4) && phase3HeadSlot != -1)
            {
                index = phase3HeadSlot;
                return;
            }
        }

        #endregion

        #region AI
        public void Initialize()
        {
            NPC.TargetClosest(false);
            EnsureAiMachine();

            if (Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera NCamera))
                NCamera.Reset();

            NPC.Center = Target.Center + new Vector2(Target.direction * 300, -200);
            alpha = 0;
            NPC.dontTakeDamage = true;

            Helper.PlayPitched("Music/Heart", 1f, 0f, NPC.Center);
            Music = 0;

            if (!Main.dedServ)
                ((NightmareSky)SkyManager.Instance["NightmareSky"]).color = nightPurple;
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }

            EnsureAiMachine();

            if (VaultUtils.isClient)
            {
                AiContext.Net.BeginClientFrame(NPC);
            }

            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Main.dayTime)
            {
                NPC.TargetClosest();

                do
                {
                    if (Main.dayTime)
                    {
                        AiContext.RequestState(NightmarePlanteraStateId.rampage);
                        break;
                    }

                    if (Target.dead || !Target.active)
                    {
                        NPC.EncourageDespawn(NightmarePlanteraDirector.DespawnEncourageFrames);
                        NPC.dontTakeDamage = true;
                        NPC.velocity.Y += NightmarePlanteraDirector.DespawnGravity;
                        if (!Main.dedServ)
                        {
                            ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = NightmarePlanteraDirector.SkyTimeleft;
                            if (rotateTentacles != null)
                            {
                                NormallySetTentacle();
                                NormallyUpdateTentacle();
                            }
                        }

                        if (VaultUtils.isClient)
                        {
                            AiContext.Net.EndClientFrame(NPC);
                        }

                        return;
                    }

                    // 脱战后重新咬住目标：按血量回到对应阶段的选招口。
                    AiContext.RequestState(ResumeStateId());
                } while (false);
            }

            NPBossIndex = NPC.whoAmI;

            AiContext.BeginFrameDefaults();
            StateMachine.Update();
            AiContext.ConsumePendingHotAdopt();
            ApplyDeclaredMovement();

            if (VaultUtils.isClient)
            {
                AiContext.Net.EndClientFrame(NPC);
            }
        }

        /// <summary>
        /// 落地本帧的运动 / 朝向 / 判定声明。两端同跑——客户端跑的是同一套数学，所以位置能自己预测出来（C1）。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            switch (AiContext.MoveMode)
            {
                case NPMoveMode.Damp:
                    NPC.velocity *= AiContext.DampFactor;
                    break;
                case NPMoveMode.Approach:
                    {
                        Vector2 dir = AiContext.ApproachAnchor - NPC.Center;
                        float speed = NPC.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / AiContext.ApproachSpeedRange, 0, 1) * AiContext.ApproachMaxSpeed;
                        NPC.velocity = NPC.velocity.ToRotation()
                            .AngleTowards(dir.ToRotation(), AiContext.ApproachTurn)
                            .ToRotationVector2() * Helper.Lerp(speed, aimSpeed, AiContext.ApproachBlend);
                    }
                    break;
            }

            switch (AiContext.RotationMode)
            {
                case NPRotationMode.TowardsTarget:
                    NPC.rotation = NPC.rotation.AngleTowards((Target.Center - NPC.Center).ToRotation(), AiContext.RotationStep);
                    break;
                case NPRotationMode.TowardsVelocity:
                    NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation(), AiContext.RotationStep);
                    break;
                case NPRotationMode.Absolute:
                    NPC.rotation = AiContext.RotationTarget;
                    break;
            }

            // 原版 SyncNPC 不带这几个开关，只能每帧两端重声明。
            NPC.dontTakeDamage = AiContext.Invulnerable;
            useMeleeDamage = AiContext.MeleeDamage;
            canOnlyBeHitByFantasyGod = AiContext.OnlyHitByFantasyGod;
        }

        private void EnsureAiMachine()
        {
            if (aiBootstrapped)
            {
                return;
            }

            AiContext = new NightmarePlanteraContext(this);
            StateMachine = new CoraliteBossStateMachine<NightmarePlanteraContext>(AiContext);
            SetupPhaseController();

            if (StateMachine.CurrentState == null)
            {
                // 中途加入 / 世界重载：初态只从已同步的 ai[0] 重建，不重放任何拍子（C7）。
                int initialId = (int)Phase;
                IVaultState<NightmarePlanteraContext> initial =
                    VaultStateRegistry<NightmarePlanteraContext>.Create(initialId)
                    ?? VaultStateRegistry<NightmarePlanteraContext>.Create((int)NightmarePlanteraStateId.onSpawnAnmi_P0);

                StateMachine.SetInitialState(initial);
            }

            RefreshAttackRandom();
            aiBootstrapped = true;
        }

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

        private void SetupPhaseController()
        {
            PhaseController.For(StateMachine)
                .OnCondition(
                    ctx => ctx.Boss.IsInterruptibleForPhase3()
                        && ctx.Npc.life <= ctx.Npc.lifeMax * NightmarePlanteraDirector.Phase2EndLifeRatio,
                    () => VaultStateRegistry<NightmarePlanteraContext>.Create((int)NightmarePlanteraStateId.exchange_P2_P3),
                    ctx => ctx.Boss.OnExchangeToP3(),
                    "NP_P2ToP3")
                .Apply();
        }

        /// <summary>
        /// 只有"正跑在二阶段宏观态里"时才允许 PhaseController 插手。<br/>
        /// 旧代码还要额外排掉"正在转阶段"，现在转阶段本身就是一个独立的顶层状态，落在这里就已经排掉了。
        /// </summary>
        internal bool IsInterruptibleForPhase3()
            => CurrentStateId == (int)NightmarePlanteraStateId.dream_P2;

        /// <summary>脱战重新咬人时该回到哪个状态：按血量决定阶段的选招口。</summary>
        internal NightmarePlanteraStateId ResumeStateId()
        {
            if (!haveBeenPhase2 && NPC.life > NPC.lifeMax * NightmarePlanteraDirector.Phase1EndLifeRatio)
            {
                return NightmarePlanteraStateId.sleeping_P1;
            }

            return NPC.life > NPC.lifeMax * NightmarePlanteraDirector.Phase2EndLifeRatio
                ? NightmarePlanteraStateId.dream_P2
                : NightmarePlanteraStateId.nightemare_P3;
        }

        public void RefreshAttackRandom()
            => AttackRandom = AiContext?.CreateAttackRandom() ?? new Random(NPC.whoAmI + 1);

        internal void SyncAttackFields() => AiContext?.SyncAttackFields();

        /// <summary>三条旋转触手的懒构造：玩家用奇葩手段跳过二阶段时三阶段也要有。</summary>
        internal void EnsureRotateTentacles()
        {
            rotateTentacles ??= new RotateTentacle[3]
            {
                new(20, TentacleColor, TentacleWidth, tentacleTex, waterFlowTex) { pos = NPC.Center, targetPos = NPC.Center },
                new(20, TentacleColor, TentacleWidth, tentacleTex, waterFlowTex) { pos = NPC.Center, targetPos = NPC.Center },
                new(20, TentacleColor, TentacleWidth, tentacleTex, waterFlowTex) { pos = NPC.Center, targetPos = NPC.Center },
            };
        }

        /// <summary>瞬移后把触手整条摁到新位置，否则会拖出一条横跨全屏的线。</summary>
        internal void ResetTentaclesTo(Vector2 center, float rotation)
        {
            if (rotateTentacles == null)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                tentacle.pos = tentacle.targetPos = center;
                tentacle.rotation = rotation;
            }
        }

        #endregion

        #region States

        public enum AIPhases
        {
            /// <summary>
            /// 生成动画
            /// </summary>
            OnSpawnAnmi_P0,
            /// <summary> 一阶段：入梦 </summary>
            Sleeping_P1,
            /// <summary> 一阶段和二阶段的切换 </summary>
            Exchange_P1_P2,
            /// <summary> 二阶段：噩梦 </summary>
            Dream_P2,
            ///<summary> 三阶段：梦魇 </summary>
            Nightemare_P3,
            /// <summary> 尾杀：惊醒 </summary>
            WakeUp_P4,
            /// <summary> 狂暴 </summary>
            Rampage,
            /// <summary> 秒杀玩家的动作 </summary>
            SuddenDeath
        }

        public enum AIStates
        {
            /// <summary> 沉眠之雾 </summary>
            hypnotizeFog,
            /// <summary> 黑暗之触 </summary>
            darkTentacle,
            /// <summary> 黑暗飞叶 </summary>
            darkLeaves,
            /// <summary> 一阶段中的idle,这时候为准备攻击阶段，什么也不干 </summary>
            P1_Idle,

            //以下为2阶段招式

            /// <summary> 噩梦之咬  </summary>
            nightmareBite,
            /// <summary> 噩梦冲刺 </summary>
            nightmareDash,
            /// <summary> 假装咬但张开嘴后消失并射出2个荆棘刺 </summary>
            fakeBite,
            /// <summary>
            /// 噩梦之咬，但是出现美梦光帮助你
            /// </summary>
            fantasyHelp,
            /// <summary> 转圈圈放弹幕，之后瞬移到另一方向咬下 </summary>
            rollingThenBite,
            /// <summary> 在玩家下方转圈圈并放弹幕，之后瞬移到玩家面朝方向上方向斜上方咬下 </summary>
            belowSparkleThenBite,
            /// <summary> 射出一些球球，然后从中刺出尖刺 </summary>
            spikeBalls,
            /// <summary> 放出蝙蝠限制玩家走位，同时射出蝙蝠和绕圈的乌鸦 </summary>
            batsAndCrows,
            /// <summary> 爪击 </summary>
            hookSlash,
            /// <summary> 在一边生成刺+，自身吐出弹幕</summary>
            spikesAndSparkles,
            /// <summary> 旋转并放出尖刺 </summary>
            spikeHell,
            /// <summary> 瞬移冲刺，之后放出鬼手 </summary>
            ghostDash,
            /// <summary> 瞬移后射弹幕 </summary>
            teleportSparkle,
            /// <summary> 梦境之光 </summary>
            dreamSparkle,
            /// <summary> 第二阶段的idle，只是在玩家身边绕圈圈 </summary>
            P2_Idle,
            /// <summary> 美梦猎杀，持续生成并尝试击杀美梦光 </summary>
            fantasyHunting,
            /// <summary> 黑洞，未能收集梦境之光的惩罚招式 </summary>
            blackHole,

            //三阶段
            /// <summary> 二三阶段切换，只是简单爆开而已 </summary>
            exchange_P2_P3,
            /// <summary> 幻象之咬，随机放出几个幻象，之后才自己咬上来 </summary>
            illusionBite,
            /// <summary> 三重尖刺地狱 </summary>
            tripleSpikeHell,
            /// <summary> 花之舞，转圈圈弹幕 </summary>
            flowerDance,
            /// <summary> 一堆爪击 </summary>
            superHookSlash,
            /// <summary> 召唤荆棘刺然后刺出 </summary>
            vineSpurt,
            /// <summary> 三阶段的蝙蝠渡鸦 </summary>
            P3_batsAndCrows,
            /// <summary> 射出追踪的荆棘，并在沿途释放种子球 </summary>
            vinesAndSeeds,
            /// <summary> 三阶段的刺+弹幕 </summary>
            P3_SpikesAndSparkles,
            /// <summary> 瞬移转圈圈 </summary>
            P3_teleportSparkles,
            P3_nightmareBite,
            P3_nightmareDash,
            P3_fakeBite,
        }

        /// <summary>回到当前血量对应阶段的选招口。换态本身由状态基类在 <c>ServerUpdate</c> 里消费请求完成。</summary>
        public void ResetStates()
        {
            if (VaultUtils.isClient || AiContext == null)
            {
                return;
            }

            AiContext.RequestState(ResumeStateId());
        }

        public void ChangeToSuddenDeath(Player player)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            if (CurrentMacroPhase is (int)AIPhases.Sleeping_P1 or (int)AIPhases.Exchange_P1_P2)
            {
                return;
            }

            NPC.target = player.whoAmI;
            NPC.NewProjectileInAI_Server<SuddenDeath>(Target.Center, Vector2.Zero, 0, 0, NPC.target);
            AiContext.RequestState(NightmarePlanteraStateId.suddenDeath);
            State = 0;
            SonState = 0;
            Timer = 0;
            ShootCount = 0;
            SyncAttackFields();
        }

        #endregion

        #region HelperMethods

        public static bool NightmarePlanteraAlive(out NPC np)
        {
            if (NPBossIndex >= 0 && NPBossIndex < 201 && Main.npc[NPBossIndex].active && Main.npc[NPBossIndex].type == ModContent.NPCType<NightmarePlantera>())
            {
                np = Main.npc[NPBossIndex];
                return true;
            }

            np = null;
            return false;
        }

        public static void NightmareHit(Player player)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            player.AddBuff(ModContent.BuffType<DreamErosion>(), 18000);
            if (!NightmarePlanteraAlive(out NPC np))
            {
                return;
            }

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.nightmareCount < 28)
                {
                    byte howMany = (byte)Helper.ScaleValueForDiffMode(2, 2, 2, 4);
                    if (cp.HasEffect(nameof(NightmareHeart)))
                    {
                        howMany -= 1;
                        if (howMany < 1)
                            howMany = 1;
                    }

                    SetNightmareCount(cp, cp.nightmareCount + howMany);
                }

                if (cp.nightmareCount >= 28)
                {
                    SetNightmareCount(cp, 28);
                    (np.ModNPC as NightmarePlantera).ChangeToSuddenDeath(player);
                }
            }
        }

        /// <summary>服务端权威写入 nightmareCount 并广播 CoralitePlayer 同步包。</summary>
        public static void SetNightmareCount(CoralitePlayer cp, int value)
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            cp.nightmareCount = value;
            cp.SendPlayerSync();
        }

        public static void ClearNightmareCountForPlayer(Player player)
        {
            if (VaultUtils.isClient || !player.TryGetModPlayer(out CoralitePlayer cp))
            {
                return;
            }

            SetNightmareCount(cp, 0);
            player.ClearBuff(ModContent.BuffType<DreamErosion>());
        }

        public Vector2 GetPhase1MousePos()
        {
            return NPC.Center + (NPC.rotation.ToRotationVector2() * 100);
        }

        public void DoRotation(float maxChange)
        {
            NPC.rotation = NPC.rotation.AngleTowards((Target.Center - NPC.Center).ToRotation(), maxChange);
        }

        public Color TentacleColor(float factor)
        {
            return Color.Lerp(tentacleColor, Color.Transparent, factor) * alpha;
        }

        public static float TentacleWidth(float factor)
        {
            if (factor > 0.5f)
                return Helper.Lerp(25, 0, (factor - 0.5f) / 0.5f);

            return Helper.Lerp(0, 25, factor / 0.5f);
        }

        public void NormallySetTentacle()
        {
            Vector2 center = NPC.Center - (NPC.velocity * 2);
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                float factor = MathF.Sin(((float)Main.timeForVisualEffects / 12) + (i * 1.5f));
                float targetRot = tentacle.rotation.AngleLerp(NPC.rotation + (factor * 1.3f), 0.4f);
                Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                    center + (((i * 30) + 140) * (NPC.rotation + (factor * 0.65f) + MathHelper.Pi).ToRotationVector2()), 0.2f);
                tentacle.SetValue(selfPos, NPC.Center, targetRot);
            }
        }

        public void NormallyUpdateTentacle()
        {
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
            }
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Rectangle frameBox = mainTex.Frame(4, Main.npcFrameCount[NPC.type], NPC.frame.X, NPC.frame.Y);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 pos = NPC.Center - screenPos;
            float selfRot = NPC.rotation + MathHelper.PiOver2;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

            if (rotateTentacles != null)
                for (int j = 0; j < 3; j++)
                    rotateTentacles[j]?.DrawTentacle_NoEndBegin(i => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly), 2);

            for (int k = 0; k < Main.maxProjectiles; k++) //Projectiles
                if (Main.projectile[k].active && Main.projectile[k].ModProjectile is INightmareTentacle)
                    (Main.projectile[k].ModProjectile as INightmareTentacle).DrawTentacle();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, default, null, Main.Transform);
            if (alpha != 1)
            {
                //绘制7个幻影
                float angle = alpha * MathHelper.Pi;
                float distance = MathF.Sin(alpha * MathHelper.Pi) * 64;

                for (int i = 0; i < 7; i++)
                {
                    Color c = phantomColors[i] * alpha;
                    spriteBatch.Draw(mainTex, pos + (((i * 1 / 7f * MathHelper.TwoPi) + angle).ToRotationVector2() * distance), frameBox, c * alpha, selfRot, origin, NPC.scale, 0, 0);
                }
            }

            //绘制自己
            spriteBatch.Draw(mainTex, NPC.Center - Main.screenPosition, frameBox, Color.White * alpha, selfRot, origin, NPC.scale, 0, 0);

            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (rotateTentacles != null)
            {
                Texture2D sparkleTex = ConfusionHole.SparkleTex.Value;
                var frameBox = sparkleTex.Frame(1, 2, 0, tentacleStarFrame);
                Vector2 origin = frameBox.Size() / 2;
                //float rot = Main.GlobalTimeWrappedHourly * 0.5f;
                Color c = Color.White;
                c.A = (byte)(200 * alpha);
                for (int j = 0; j < 3; j++) //绘制触手上的三个小星星
                {
                    Vector2 pos = rotateTentacles[j].pos - Main.screenPosition;
                    spriteBatch.Draw(sparkleTex, pos, frameBox, c, rotateTentacles[j].rotation, origin, 0.3f + Main.rand.NextFloat(0, 0.02f), 0, 0);
                }
            }
        }

        #endregion
    }
}
