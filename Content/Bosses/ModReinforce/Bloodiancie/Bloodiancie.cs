using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Content.Bosses.Rediancie;
using Coralite.Content.Items.RedJades;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    /// <summary>
    /// 赤血玉灵主控：只留钩子、状态机宿主、<see cref="AI"/> 固定顺序、<see cref="ApplyDeclaredMovement"/>、网络接线与绘制。<br/>
    /// 招式体在 <c>States/</c> 一状态一文件，数字在 <see cref="BloodiancieDirector"/>，声明总线与弹药编排在 <see cref="BloodiancieContext"/>。
    /// 接线方式与同模板的赤玉灵 <c>Rediancie.cs</c> 一致。
    /// </summary>
    [AutoloadBossHead]
    public class Bloodiancie : ModNPC
    {
        public override string Texture => AssetDirectory.Bloodiancie + Name;

        internal BloodiancieContext AiContext;
        internal CoraliteBossStateMachine<BloodiancieContext> StateMachine;

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
            NPC.width = 68;
            NPC.height = 80;
            NPC.damage = 75;
            NPC.defense = 40;
            NPC.lifeMax = 23000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);

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
            //if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            //{
            //    if (nPCStrengthHelper.IsExpertMode)
            //    {
            //        NPC.lifeMax = (int)((21000 + (numPlayers * 9500)) / journeyScale);
            //        NPC.damage = 90;
            //        NPC.defense = 18;
            //    }

            //    if (nPCStrengthHelper.IsMasterMode)
            //    {
            //        NPC.lifeMax = (int)((25500 + (numPlayers * 11500)) / journeyScale);
            //        NPC.damage = 115;
            //        NPC.defense = 20;
            //    }

            //    if (Main.getGoodWorld)
            //    {
            //        NPC.defense = 14;//因为FTW种能够拥有非常多的弹药所以就降低一下基础防御了
            //    }

            //    return;
            //}

            NPC.lifeMax = 21000 + (numPlayers * 9500);
            NPC.damage = 90;
            NPC.defense = 18;

            if (Main.masterMode)
            {
                NPC.lifeMax = 25500 + (numPlayers * 11500);
                NPC.damage = 115;
                NPC.defense = 20;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 30000 + (numPlayers * 14500);
                NPC.damage = 115;
                NPC.defense = 14;//因为FTW种能够拥有非常多的弹药所以就降低一下基础防御了
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BloodiancieRelic>()));
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<RedianciePet>(), 4));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<BloodiancieBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<RediancieTrophy>(), 10));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<BloodJade>(), 1, 30, 38));
            npcLoot.Add(notExpertRule);
        }

        public override void BossLoot(ref int potionType, ref int potionStack, ref int heartStack)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                SkyManager.Instance.Deactivate("BloodJadeSky");

                for (int j = 0; j < 5; j++)
                {
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore2").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore3").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), Mod.Find<ModGore>("Rediancie_Gore4").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), Mod.Find<ModGore>("Rediancie_Gore0").Type);
                }

                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center + Main.rand.NextVector2Circular(30, 40), new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), Mod.Find<ModGore>("Rediancie_Gore1").Type);
                AiContext?.Followers.Clear();
            }

            DownedBossSystem.DownBloodiancie();
        }

        public override void Load()
        {
            BloodiancieFollower.tex1 = Request<Texture2D>(AssetDirectory.Bloodiancie + "BloodiancieFollower1");
            BloodiancieFollower.tex2 = Request<Texture2D>(AssetDirectory.Bloodiancie + "BloodiancieFollower2");

            //for (int i = 0; i < 5; i++)
            //    GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.BossGores + "Rediancie_Gore" + i);
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
        /// 权威端登记 <see cref="BloodiancieContext.KillRequested"/>，由状态基类的 ServerUpdate 经返回值切到死亡演出，客户端读 ai[0] 跟随。
        /// 血玉天空由死亡演出态的 OnEnter 在每个客户端各自熄灭。
        /// 死亡演出结束时权威端 <c>NPC.Kill()</c> 再次进入这里，此时已在演出态 → 放行真死。
        /// </summary>
        public override bool CheckDead()
        {
            if (StateMachine == null || CurrentStateId == (int)BloodiancieStateId.onKillAnim)
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
        /// 固定顺序（与赤玉灵逐字一致）：懒构造 → 客户端纠偏帧首 → 目标与脱战 → 只读事实 → 声明回默认 → 状态机 → 热字段兜底收养 → 落地运动 → 表现 → 客户端记预测。
        /// </summary>
        public override void AI()
        {
            EnsureAiMachine();

            if (!spwan)
            {
                // 首帧自带 6 发弹药（数量随 WriteFacts 纠正）；瞬移到最近玩家头顶是一次决策，只在权威端做并发包。旧 AI.cs:26-38
                AiContext.SpawnFollowers(BloodiancieDirector.SpawnFollowerCount);

                NPC.TargetClosest(false);
                if (!VaultUtils.isClient && NPC.target != -1)
                {
                    NPC.Center = AiContext.Target.Center - new Vector2(0, BloodiancieDirector.SpawnHeightAboveTarget);
                    NPC.netUpdate = true;
                }

                spwan = true;
            }

            if (VaultUtils.isClient)
                AiContext.Net.BeginClientFrame(NPC);

            if (!FindTarget())
            {
                // 脱战：没有玩家存活时缓缓离开（两端同算的运动数学，不经状态机）
                NPC.velocity.X *= BloodiancieDirector.DespawnDampX;
                NPC.velocity.Y += BloodiancieDirector.DespawnGravity;
                NPC.EncourageDespawn(BloodiancieDirector.DespawnEncourageFrames);

                // 天空是纯表现，每个有画面的端各自关
                if (!Main.dedServ)
                    SkyManager.Instance.Deactivate("BloodJadeSky");

                ApplyRotation(BloodiancieRotationMode.Normal, 0f);
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

            // 表现层：赤色光照。旧 AI.cs:120
            if (!Main.dedServ)
                Lighting.AddLight(NPC.Center, Color.Red.ToVector3());

            if (VaultUtils.isClient)
                AiContext.Net.EndClientFrame(NPC);
        }

        /// <summary>目标与脱战判定照旧；返回 false 表示该离场。旧 AI.cs:97-111</summary>
        private bool FindTarget()
        {
            Player target = AiContext.Target;
            if (NPC.target < 0 || NPC.target == 255 || target.dead || !target.active || target.Distance(NPC.Center) > BloodiancieDirector.DespawnDistance)
            {
                NPC.TargetClosest();
                target = AiContext.Target;

                if (target.dead || !target.active || target.Distance(NPC.Center) > BloodiancieDirector.DespawnDistance)
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

            AiContext = new BloodiancieContext(this);
            StateMachine = new CoraliteBossStateMachine<BloodiancieContext>(AiContext);

            IVaultState<BloodiancieContext> initial = VaultStateRegistry<BloodiancieContext>.Create((int)NPC.ai[CoraliteBossContext.StateAiSlot])
                ?? VaultStateRegistry<BloodiancieContext>.Create((int)BloodiancieStateId.onSpawnAnim);
            StateMachine.SetInitialState(initial);
        }

        /// <summary>
        /// 两端同跑：把本帧声明翻译成 velocity / rotation / 无敌与反弹标志。状态里没有裸的运动法则，全局规则只在这里改一处。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            BloodiancieContext ctx = AiContext;

            switch (ctx.MoveMode)
            {
                case BloodiancieMoveMode.Keep:
                case BloodiancieMoveMode.Direct:
                    break;
                case BloodiancieMoveMode.Damp:
                    NPC.velocity *= ctx.DampFactor;
                    break;
                case BloodiancieMoveMode.Hover:
                    NPC.velocity.X *= ctx.HoverDampX;
                    NPC.velocity.Y += ctx.HoverAccelY;
                    if (NPC.velocity.Y < ctx.HoverLimitY)
                        NPC.velocity.Y = ctx.HoverLimitY;
                    break;
                case BloodiancieMoveMode.Chase:
                    // 方向为 0 = 落入死区，该轴改按死区系数衰减（旧代码每个招式里的 xLength / yLength 分支）
                    if (ctx.ChaseDirX == 0)
                        NPC.velocity.X *= ctx.ChaseDeadDampX;
                    else
                        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, ctx.ChaseDirX, ctx.ChaseSpeedX, ctx.ChaseAccelX, ctx.ChaseTurnX, ctx.ChaseDampX);

                    // ChaseY 未声明时 Y 由状态自管（向上射击），宿主不碰
                    if (ctx.ChaseY)
                    {
                        if (ctx.ChaseDirY == 0)
                            NPC.velocity.Y *= ctx.ChaseDeadDampY;
                        else
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, ctx.ChaseDirY, ctx.ChaseSpeedY, ctx.ChaseAccelY, ctx.ChaseTurnY, ctx.ChaseDampY);
                    }
                    break;
                default:
                    NPC.velocity *= BloodiancieDirector.HoldDamp;
                    break;
            }

            ApplyRotation(ctx.RotationMode, ctx.RotationLerp);

            // 原版不同步这两个标志，两端按同一份声明每帧落地；死亡请求期间保持无敌直到演出态接管
            NPC.dontTakeDamage = ctx.Invulnerable || ctx.KillRequested;
            NPC.reflectsProjectiles = ctx.ReflectsProjectiles;
        }

        private void ApplyRotation(BloodiancieRotationMode mode, float lerp)
        {
            float speedRot = NPC.velocity.Length() * BloodiancieDirector.RotationPerSpeed * NPC.direction;
            switch (mode)
            {
                case BloodiancieRotationMode.Normal:
                    NPC.rotation = NPC.rotation.AngleTowards(speedRot, BloodiancieDirector.RotationTowardsStep);
                    break;
                case BloodiancieRotationMode.LerpToSpeed:
                    NPC.rotation = NPC.rotation.AngleLerp(speedRot, lerp);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region NetWork

        /// <summary>热字段（Timer / Counter / Beat / 自用槽）+ boss 事实（弹药数）随 SyncNPC 原子过线。就这两行。</summary>
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
                foreach (BloodiancieFollower follower in AiContext.Followers)
                {
                    if (follower.drawBehind)
                        follower.Draw(spriteBatch, color);
                }

            DrawSelf(spriteBatch, screenPos, color);

            if (AiContext != null)
                foreach (BloodiancieFollower follower in AiContext.Followers)
                {
                    if (!follower.drawBehind)
                        follower.Draw(spriteBatch, color);
                }

            return false;
        }

        private void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //绘制自己
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Vector2 origin = mainTex.Size() / 2;
            // 表现偏移只加在绘制位上，判定盒不跟抖
            Vector2 drawOffset = AiContext?.DrawOffset ?? Vector2.Zero;

            spriteBatch.Draw(mainTex, NPC.Center + drawOffset - screenPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
