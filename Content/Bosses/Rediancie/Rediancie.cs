using Coralite.Content.Bosses.Rediancie.Core;
using Coralite.Content.Items.RedJades;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.Rediancie
{
    //简单写下Rediancie这个英文名由来
    //其实就是Red+Diancie ,红色+蒂安希，捏他自宝可梦中的钻石公主蒂安希
    //另外它的召唤物，小赤玉灵名字捏他的是小碎钻。
    //
    //                                            饿啊  ， 吃我钻石风暴！！！
    //
    //                      💎💎                                       💎💎                                      💎💎
    //                 💎💎💎💎                             💎💎💎💎                            💎💎💎💎
    //           💎💎💎💎💎💎                   💎💎💎💎💎💎                  💎💎💎💎💎💎
    //      💎💎💎💎💎💎💎              💎💎💎💎💎💎💎             💎💎💎💎💎💎💎
    // 💎💎💎💎💎💎💎💎         💎💎💎💎💎💎💎💎        💎💎💎💎💎💎💎💎
    //      💎💎💎💎💎💎💎              💎💎💎💎💎💎💎             💎💎💎💎💎💎💎
    //           💎💎💎💎💎💎                   💎💎💎💎💎💎                  💎💎💎💎💎💎
    //                 💎💎💎💎                             💎💎💎💎                            💎💎💎💎
    //                      💎💎                                       💎💎                                      💎💎
    //
    //MEGA蒂安希 160物攻 本系100威力群攻技能，你接的下?
    //你十万条命都接不下
    //打完还50%概率上升2段物防，又硬又能打（虽然mega后物防还低了）

    /// <summary>
    /// 赤玉灵主控：只留钩子、状态机宿主、<see cref="AI"/> 固定顺序、<see cref="ApplyDeclaredMovement"/>、网络接线与绘制。<br/>
    /// 招式体在 <c>States/</c> 一状态一文件，数字在 <see cref="RediancieDirector"/>，声明总线与弹药编排在 <see cref="RediancieContext"/>。
    /// Phase 1 的其它 boss 镜像本文件的接线方式。
    /// </summary>
    [AutoloadBossHead]
    public class Rediancie : ModNPC
    {
        public override string Texture => AssetDirectory.Rediancie + Name;

        internal RediancieContext AiContext;
        internal CoraliteBossStateMachine<RediancieContext> StateMachine;

        /// <summary>当前顶层状态 ID；状态机未建立时读 ai[0]（中途加入者在第一帧 AI 之前也能拿到正确值）。</summary>
        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)NPC.ai[CoraliteBossContext.StateAiSlot];

        internal static readonly Color red = new(221, 50, 50);
        internal static readonly Color grey = new(91, 93, 102);

        private bool spwan;

        #region tml hooks

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 85;
            NPC.damage = 25;
            NPC.defense = 6;
            NPC.lifeMax = 1500;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;

            NPC.BossBar = GetInstance<RediancieBossBar>();
            GetInstance<RediancieBossBar>().Reset(NPC);

            //BGM：赤色激流
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/RedTorrent");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((1800 + (numPlayers * 450)) / journeyScale);
                    NPC.damage = 30;
                    NPC.defense = 6;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((2000 + (numPlayers * 550)) / journeyScale);
                    NPC.damage = 45;
                    NPC.defense = 6;
                }

                if (Main.getGoodWorld)
                {
                    NPC.defense = 4;//因为FTW种能够拥有非常多的弹药所以就降低一下基础防御了
                }

                return;
            }

            NPC.lifeMax = 1800 + (numPlayers * 450);
            NPC.damage = 30;
            NPC.defense = 6;

            if (Main.masterMode)
            {
                NPC.lifeMax = 2000 + (numPlayers * 550);
                NPC.damage = 45;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 2300 + (numPlayers * 600);
                NPC.damage = 45;
                NPC.defense = 4;//因为FTW种能够拥有非常多的弹药所以就降低一下基础防御了
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<RediancieRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<RedianciePet>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<RediancieBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ItemType<RediancieMask>(), 7));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<RedJade>(), 1, 28, 32));
            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int j = 0; j < 3; j++)
                {
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore2").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore3").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore4").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), Mod.Find<ModGore>("Rediancie_Gore0").Type);
                }

                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), Mod.Find<ModGore>("Rediancie_Gore1").Type);
                AiContext?.Followers.Clear();
            }

            DownedBossSystem.DownRediancie();
        }

        public override void Load()
        {
            if (Main.dedServ)
            {
                return;
            }
            RediancieFollower.tex1 = Request<Texture2D>(AssetDirectory.Rediancie + "RediancieFollower1");
            RediancieFollower.tex2 = Request<Texture2D>(AssetDirectory.Rediancie + "RediancieFollower2");

            for (int i = 0; i < 5; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.BossGores + "Rediancie_Gore" + i);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            AiContext?.OnHit(hit.Damage);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.pick > 0)
                modifiers.SourceDamage.Flat += item.pick;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            AiContext?.OnHit(hit.Damage);
        }

        /// <summary>
        /// 死亡拦截：不在这里换态。本地把血锁到 1 并无敌（命中方客户端与服务端都会跑到这里），
        /// 权威端登记 <see cref="RediancieContext.KillRequested"/>，由状态基类的 ServerUpdate 经返回值切到死亡演出，客户端读 ai[0] 跟随。
        /// 死亡演出结束时权威端 <c>NPC.Kill()</c> 再次进入这里，此时已在演出态 → 放行真死。
        /// </summary>
        public override bool CheckDead()
        {
            if (StateMachine == null || CurrentStateId == (int)RediancieStateId.onKillAnim)
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

        #endregion

        #region AI

        /// <summary>
        /// 固定顺序（Phase 1 逐字镜像）：懒构造 → 客户端纠偏帧首 → 目标与脱战 → 只读事实 → 声明回默认 → 状态机 → 热字段兜底收养 → 落地运动 → 表现 → 客户端记预测。
        /// </summary>
        public override void AI()
        {
            EnsureAiMachine();

            if (!spwan)
            {
                // 首帧：自带弹药、瞬移到最近玩家头顶（两端同算；服务端随后发包对齐）
                AiContext.SpawnFollowers(RediancieDirector.SpawnFollowerCount);

                NPC.TargetClosest(false);
                if (NPC.target != -1)
                {
                    NPC.Center = AiContext.Target.Center - new Vector2(0, RediancieDirector.SpawnHeightAboveTarget);
                }
                NPC.netUpdate = true;

                spwan = true;
            }

            if (VaultUtils.isClient)
                AiContext.Net.BeginClientFrame(NPC);

            if (!FindTarget())
            {
                // 脱战：没有玩家存活时缓缓离开（两端同算的运动数学，不经状态机）
                NPC.velocity.X *= RediancieDirector.DespawnDampX;
                NPC.velocity.Y += RediancieDirector.DespawnGravity;
                NPC.EncourageDespawn(RediancieDirector.DespawnEncourageFrames);
                ApplyRotation(RediancieRotationMode.Normal, 0f);
                AiContext.UpdateFollowersIdle(0);

                if (VaultUtils.isClient)
                    AiContext.Net.EndClientFrame(NPC);
                return;
            }

            AiContext.UpdateFacts();
            AiContext.BeginFrameDefaults();

            // 状态只写声明；转移仅 ServerUpdate 返回值，客户端由 ai[0] 跟随
            StateMachine.Update();
            AiContext.ConsumePendingHotAdopt();

            ApplyDeclaredMovement();

            if (VaultUtils.isClient)
                AiContext.Net.EndClientFrame(NPC);
        }

        /// <summary>目标与脱战判定照旧；返回 false 表示该离场。旧 Rediancie.cs:326-339</summary>
        private bool FindTarget()
        {
            Player target = AiContext.Target;
            if (NPC.target < 0 || NPC.target == 255 || target.dead || !target.active || target.Distance(NPC.Center) > RediancieDirector.DespawnDistance)
            {
                NPC.TargetClosest();
                target = AiContext.Target;

                if (target.dead || !target.active || target.Distance(NPC.Center) > RediancieDirector.DespawnDistance)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 懒构造；初态从 ai[0] 重建——中途加入的客户端与 SetDefaults 之后才收到 ai[] 的重建路径都靠这个，未注册 id 回退出生动画。
        /// 上下文构造里已置 <c>UseLegacySpeedValve = false</c>。
        /// </summary>
        private void EnsureAiMachine()
        {
            if (StateMachine != null)
                return;

            AiContext = new RediancieContext(this);
            StateMachine = new CoraliteBossStateMachine<RediancieContext>(AiContext);

            IVaultState<RediancieContext> initial = VaultStateRegistry<RediancieContext>.Create((int)NPC.ai[CoraliteBossContext.StateAiSlot])
                ?? VaultStateRegistry<RediancieContext>.Create((int)RediancieStateId.onSpawnAnim);
            StateMachine.SetInitialState(initial);
        }

        /// <summary>
        /// 两端同跑：把本帧声明翻译成 velocity / rotation / 无敌标志。状态里没有裸的运动法则，全局规则只在这里改一处。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            RediancieContext ctx = AiContext;

            switch (ctx.MoveMode)
            {
                case RediancieMoveMode.Keep:
                case RediancieMoveMode.Direct:
                    break;
                case RediancieMoveMode.Damp:
                    NPC.velocity *= ctx.DampFactor;
                    break;
                case RediancieMoveMode.Hover:
                    NPC.velocity.X *= ctx.HoverDampX;
                    NPC.velocity.Y += ctx.HoverAccelY;
                    if (NPC.velocity.Y < ctx.HoverLimitY)
                        NPC.velocity.Y = ctx.HoverLimitY;
                    break;
                case RediancieMoveMode.Chase:
                    Helper.Movement_SimpleOneLine(ref NPC.velocity.X, ctx.ChaseDirX, ctx.ChaseSpeedX, ctx.ChaseAccelX, ctx.ChaseTurnX, ctx.ChaseDampX);
                    if (ctx.ChaseY)
                    {
                        float yLength = System.Math.Abs(ctx.Target.Center.Y - NPC.Center.Y);
                        if (!ctx.ChaseDeadZoneY || yLength > RediancieDirector.ChaseDeadZoneY)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, ctx.ChaseDirY, ctx.ChaseSpeedY, ctx.ChaseAccelY, ctx.ChaseTurnY, ctx.ChaseDampY);
                        else
                            NPC.velocity.Y *= RediancieDirector.ChaseDeadZoneDampY;
                    }
                    break;
                default:
                    NPC.velocity *= RediancieDirector.HoldDamp;
                    break;
            }

            ApplyRotation(ctx.RotationMode, ctx.RotationLerp);

            // 原版不同步这两个标志，两端按同一份声明每帧落地；死亡请求期间保持无敌直到演出态接管
            NPC.dontTakeDamage = ctx.Invulnerable || ctx.KillRequested;
            NPC.reflectsProjectiles = ctx.ReflectsProjectiles;
        }

        private void ApplyRotation(RediancieRotationMode mode, float lerp)
        {
            float speedRot = NPC.velocity.Length() * RediancieDirector.RotationPerSpeed * NPC.direction;
            switch (mode)
            {
                case RediancieRotationMode.Normal:
                    NPC.rotation = NPC.rotation.AngleTowards(speedRot, RediancieDirector.RotationTowardsStep);
                    break;
                case RediancieRotationMode.LerpToSpeed:
                    NPC.rotation = NPC.rotation.AngleLerp(speedRot, lerp);
                    break;
                case RediancieRotationMode.LerpToZero:
                    NPC.rotation = NPC.rotation.AngleLerp(0f, lerp);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region NetWork

        /// <summary>热字段（Timer / Counter / Beat / 自用槽）+ boss 事实（弹药数）随 SyncNPC 原子过线。Phase 1 各 boss 就这两行。</summary>
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

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
                return Main.DiscoColor;

            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = drawColor;
            if (Main.zenithWorld)
                color = Main.DiscoColor;

            if (AiContext != null)
                foreach (RediancieFollower follower in AiContext.Followers)
                {
                    if (follower.drawBehind)
                        follower.Draw(spriteBatch, color);
                }

            DrawSelf(spriteBatch, screenPos, color);

            if (AiContext != null)
                foreach (RediancieFollower follower in AiContext.Followers)
                {
                    if (!follower.drawBehind)
                        follower.Draw(spriteBatch, color);
                }

            return false;
        }

        private void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Vector2 origin = mainTex.Size() / 2;
            // 表现偏移只加在绘制位上，判定盒不跟抖
            Vector2 drawOffset = AiContext?.DrawOffset ?? Vector2.Zero;

            spriteBatch.Draw(mainTex, NPC.Center + drawOffset - screenPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
