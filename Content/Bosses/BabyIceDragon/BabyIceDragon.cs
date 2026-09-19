using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    /*小冰龙宝宝
    * 小小的也很可爱
    *
    */

    /// <summary>
    /// 冰龙宝宝主控：只留钩子、状态机宿主、<see cref="AI"/> 固定顺序、<see cref="ApplyDeclaredMovement"/>、网络接线与绘制。<br/>
    /// 招式体在 <c>States/</c> 一状态一文件，数字在 <see cref="BabyIceDragonDirector"/>，声明总线与跨帧事实在 <see cref="BabyIceDragonContext"/>。
    /// </summary>
    [AutoloadBossHead]
    public class BabyIceDragon : ModNPC
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        public static Asset<Texture2D> GlowTex;

        internal BabyIceDragonContext AiContext;
        internal CoraliteBossStateMachine<BabyIceDragonContext> StateMachine;

        /// <summary>当前顶层状态 ID；状态机未建立时读 ai[0]（中途加入者在第一帧 AI 之前也能拿到正确值）。</summary>
        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)NPC.ai[CoraliteBossContext.StateAiSlot];

        private bool spwan;
        /// <summary>残影缓存是否已就位（<see cref="BabyIceDragonContext.DrawShadows"/> 的上升沿时重置，纯表现）。</summary>
        private bool shadowsPrimed;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 106;
            NPC.height = 68;
            NPC.damage = 40;
            NPC.defense = 6;
            NPC.lifeMax = 4500;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 6, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = GetInstance<BabyIceDragonBossBar>();
            GetInstance<BabyIceDragonBossBar>().Reset(NPC);

            //BGM：冰结寒流
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            //if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            //{
            //    if (nPCStrengthHelper.IsExpertMode)
            //    {
            //        NPC.lifeMax = (int)((3820 + (numPlayers * 1750)) / journeyScale);
            //        NPC.damage = 55;
            //        NPC.defense = 15;
            //    }

            //    if (nPCStrengthHelper.IsMasterMode)
            //    {
            //        NPC.lifeMax = (int)((4720 + (numPlayers * 2100)) / journeyScale);
            //        NPC.damage = 60;
            //        NPC.defense = 18;
            //    }

            //    if (Main.getGoodWorld)
            //    {
            //        NPC.damage = 70;
            //        NPC.defense = 20;
            //    }

            //    if (Main.zenithWorld)
            //    {
            //        NPC.scale = 0.4f;
            //    }

            //    return;
            //}

            NPC.lifeMax = 3820 + (numPlayers * 1750);
            NPC.damage = 55;
            NPC.defense = 15;

            if (Main.masterMode)
            {
                NPC.lifeMax = 4720 + (numPlayers * 2100);
                NPC.damage = 60;
                NPC.defense = 18;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 5320 + (numPlayers * 2200);
                NPC.damage = 70;
                NPC.defense = 20;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 0.4f;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<IcicleSoulStone>(), 4));

            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BabyIceDragonRelic>()));
            npcLoot.Add(ItemDropRule.BossBag(ItemType<BabyIceDragonBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonTrophy>(), 10));
            npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonMask>(), 7));

            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleCrystal>(), 1, 3, 5));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleScale>(), 1, 2, 4));
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleBreath>(), 1, 4, 7));
            npcLoot.Add(notExpertRule);
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GlowTex = Request<Texture2D>(AssetDirectory.BabyIceDragon + Name + "_Glow");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            GlowTex = null;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(CoraliteSoundID.DigIce, NPC.Center);

            //残血被暴击时掉冰鳞，仅权威端掉落（Item.NewItem 在服务端自带同步）
            if (AiContext == null || VaultUtils.isClient)
                return;

            if (NPC.life < NPC.lifeMax * BabyIceDragonDirector.DropScaleLifeRatio && hit.Crit
                && AiContext.DropScaleCount < BabyIceDragonDirector.DropScaleMax)
            {
                AiContext.DropScaleCount++;
                Item.NewItem(NPC.GetSource_DropAsItem(), NPC.getRect(), ItemType<IcicleScale>());
            }
        }

        public override void OnKill()
        {
            DownedBossSystem.DownBabyIceDragon();
            if (!VaultUtils.isClient)
            {
                IceEggSpawner.BabyIceDragonSlain();
                Main.StopRain();
                Main.SyncRain();
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            return CurrentStateId != (int)BabyIceDragonStateId.smashDown && CurrentStateId != (int)BabyIceDragonStateId.dizzy;
        }

        /// <summary>
        /// 死亡拦截：不在这里换态。本地把血锁到 1 并无敌（命中方客户端与服务端都会跑到这里），
        /// 权威端登记 <see cref="BabyIceDragonContext.KillRequested"/>，由状态基类的 ServerUpdate 经返回值切到死亡演出，客户端读 ai[0] 跟随。
        /// 死亡演出结束时权威端 <c>NPC.Kill()</c> 再次进来，此时已在演出态 → 放行真死。
        /// </summary>
        public override bool CheckDead()
        {
            if (StateMachine == null || CurrentStateId == (int)BabyIceDragonStateId.onKillAnim)
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
            int width = (int)(BabyIceDragonDirector.HitboxWidth * NPC.scale);
            int height = (int)(BabyIceDragonDirector.HitboxHeight * NPC.scale);
            npcHitbox = new Rectangle((int)(NPC.Center.X - (width / 2)), (int)(NPC.Center.Y - (height / 2)), width, height);
            return true;
        }

        #endregion

        #region AI

        /// <summary>
        /// 固定顺序（Phase 1 逐字镜像 Rediancie）：懒构造 → 客户端纠偏帧首 → 目标与脱战 → 只读事实 → 声明回默认 → 状态机 → 热字段兜底收养 → 落地运动 → 表现 → 客户端记预测。
        /// </summary>
        public override void AI()
        {
            EnsureAiMachine();

            if (!spwan)
            {
                //首帧：残影缓存与初始帧图（招池 / 转阶段闸 / 招式计数由上下文构造时初始化）
                NPC.oldPos = new Vector2[BabyIceDragonDirector.ShadowCacheLength];
                NPC.TargetClosest(false);
                NPC.frame.Y = BabyIceDragonDirector.SpawnFrameY;
                NPC.netUpdate = true;

                spwan = true;
            }

            if (VaultUtils.isClient)
                AiContext.Net.BeginClientFrame(NPC);

            if (!FindTarget())
            {
                //脱战：朝向回正、横速收住、扇翅飞走（两端同算的运动数学，不经状态机）
                AiContext.BeginFrameDefaults();
                AiContext.DeclareDampX(BabyIceDragonDirector.DespawnDampX);
                AiContext.DeclareFlyUp();
                AiContext.DeclareRotation(BabyIceDragonRotationMode.TowardsZero, BabyIceDragonDirector.DespawnRotationStep);
                ApplyDeclaredMovement();
                NPC.EncourageDespawn(BabyIceDragonDirector.DespawnEncourageFrames);

                if (!Main.dedServ)
                    UpdatePresentation();
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

            if (!Main.dedServ)
                UpdatePresentation();
            if (VaultUtils.isClient)
                AiContext.Net.EndClientFrame(NPC);
        }

        /// <summary>
        /// 目标与脱战判定照旧：离开雪原累计 6 秒或距离超过 3000 px 就重新选目标，仍不合格则离场。
        /// 出生与死亡演出期间不判脱战。旧 BabyIceDragon.cs:308-328
        /// </summary>
        private bool FindTarget()
        {
            if (AiContext.Target.ZoneSnow)
                AiContext.FlyAwayTimer = 0;
            else
                AiContext.FlyAwayTimer++;

            int state = CurrentStateId;
            if (state == (int)BabyIceDragonStateId.onKillAnim || state == (int)BabyIceDragonStateId.onSpawnAnim)
                return true;

            Player target = AiContext.Target;
            bool lost = NPC.target < 0 || NPC.target == 255 || target.dead || !target.active
                || target.Distance(NPC.Center) > BabyIceDragonDirector.DespawnDistance
                || AiContext.FlyAwayTimer > BabyIceDragonDirector.FlyAwayFrames;
            if (!lost)
                return true;

            NPC.TargetClosest();
            target = AiContext.Target;
            return !(target.dead || !target.active || target.Distance(NPC.Center) > BabyIceDragonDirector.DespawnDistance || !target.ZoneSnow);
        }

        /// <summary>
        /// 懒构造；初态从 ai[0] 重建——中途加入的客户端与 SetDefaults 之后才收到 ai[] 的重建路径都靠这个，未注册 id 回退出生动画。
        /// 上下文构造里已置 <c>UseLegacySpeedValve = false</c>。
        /// </summary>
        private void EnsureAiMachine()
        {
            if (StateMachine != null)
                return;

            AiContext = new BabyIceDragonContext(this);
            StateMachine = new CoraliteBossStateMachine<BabyIceDragonContext>(AiContext);

            IVaultState<BabyIceDragonContext> initial = VaultStateRegistry<BabyIceDragonContext>.Create((int)NPC.ai[CoraliteBossContext.StateAiSlot])
                ?? VaultStateRegistry<BabyIceDragonContext>.Create((int)BabyIceDragonStateId.onSpawnAnim);
            StateMachine.SetInitialState(initial);
        }

        /// <summary>
        /// 两端同跑：把本帧声明翻译成 velocity / 帧图 / rotation / 无敌与碰撞标志，全局运动法则只在这一处。<br/>
        /// 顺序沿用旧代码：先算速度（扇翅上飞读的是本帧翻页<b>之前</b>的帧行），再翻页，最后定朝向（飞行倾斜读的是<b>新</b>速度）。
        /// </summary>
        private void ApplyDeclaredMovement()
        {
            BabyIceDragonContext ctx = AiContext;

            switch (ctx.MoveMode)
            {
                case BabyIceDragonMoveMode.Keep:
                case BabyIceDragonMoveMode.Direct:
                    break;
                case BabyIceDragonMoveMode.Axes:
                    ApplyAxisX(ctx);
                    ApplyAxisY(ctx);
                    break;
                default:
                    NPC.velocity *= BabyIceDragonDirector.HoldDamp;
                    break;
            }

            ApplyFrame(ctx);
            ApplyRotation(ctx);

            //原版不同步这三个标志，两端按同一份声明每帧落地；死亡请求期间保持无敌直到演出态接管
            NPC.dontTakeDamage = ctx.Invulnerable || ctx.KillRequested;
            NPC.noGravity = !ctx.Gravity;
            NPC.noTileCollide = !ctx.TileCollide;
        }

        private void ApplyAxisX(BabyIceDragonContext ctx)
        {
            switch (ctx.XMode)
            {
                case BabyIceDragonAxisMode.Damp:
                    NPC.velocity.X *= ctx.XDamp;
                    break;
                case BabyIceDragonAxisMode.Chase:
                    Helper.Movement_SimpleOneLine(ref NPC.velocity.X, ctx.XDir, ctx.XSpeed, ctx.XAccel, ctx.XTurn, ctx.XDamp);
                    break;
                default:
                    break;
            }
        }

        private void ApplyAxisY(BabyIceDragonContext ctx)
        {
            switch (ctx.YMode)
            {
                case BabyIceDragonAxisMode.Damp:
                    NPC.velocity.Y *= ctx.YDamp;
                    break;
                case BabyIceDragonAxisMode.Chase:
                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, ctx.YDir, ctx.YSpeed, ctx.YAccel, ctx.YTurn, ctx.YDamp);
                    break;
                case BabyIceDragonAxisMode.Flap:
                    //扇翅的那两帧才有向上加速度，其余帧减速：一下一下往上蹿的手感来源
                    if (NPC.frame.Y >= BabyIceDragonDirector.FlapFrameYStart && NPC.frame.Y <= BabyIceDragonDirector.FlyingFrameMaxY)
                        NPC.velocity.Y -= ctx.YAccel;
                    else
                        NPC.velocity.Y *= ctx.YDamp;

                    if (NPC.velocity.Y < ctx.YLimit)
                        NPC.velocity.Y = ctx.YLimit;
                    break;
                case BabyIceDragonAxisMode.Accel:
                    NPC.velocity.Y += ctx.YAccel;
                    if (NPC.velocity.Y > ctx.YLimit)
                        NPC.velocity.Y = ctx.YLimit;
                    break;
                default:
                    break;
            }
        }

        private void ApplyFrame(BabyIceDragonContext ctx)
        {
            switch (ctx.FrameMode)
            {
                case BabyIceDragonFrameMode.Flying:
                    NPC.frame.X = ctx.FrameX;
                    NPC.frameCounter++;
                    if (NPC.frameCounter > BabyIceDragonDirector.FlyingFrameTicks)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y++;
                        if (NPC.frame.Y > BabyIceDragonDirector.FlyingFrameMaxY)
                            NPC.frame.Y = 0;
                    }
                    break;
                case BabyIceDragonFrameMode.Dizzy:
                    NPC.frame.X = BabyIceDragonDirector.DizzyFrameX;
                    if (NPC.velocity.Y < BabyIceDragonDirector.DizzyGroundedSpeedY && Framing.GetTileSafely(NPC.Bottom).HasTile)
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter > BabyIceDragonDirector.DizzyFrameTicks)
                        {
                            NPC.frameCounter = 0;
                            NPC.frame.Y++;
                            if (NPC.frame.Y > BabyIceDragonDirector.FlyingFrameMaxY)
                                NPC.frame.Y = 0;
                        }
                    }
                    else
                        NPC.frame.Y = BabyIceDragonDirector.DizzyFrameAirY;
                    break;
                default:
                    break;
            }
        }

        private void ApplyRotation(BabyIceDragonContext ctx)
        {
            switch (ctx.RotationMode)
            {
                case BabyIceDragonRotationMode.TiltBySpeed:
                    NPC.rotation = NPC.direction * NPC.velocity.Y * BabyIceDragonDirector.TiltPerSpeedY;
                    break;
                case BabyIceDragonRotationMode.TowardsZero:
                    NPC.rotation = NPC.rotation.AngleTowards(0f, ctx.RotationStep);
                    break;
                case BabyIceDragonRotationMode.LerpToZero:
                    NPC.rotation = NPC.rotation.AngleLerp(0f, ctx.RotationStep);
                    break;
                case BabyIceDragonRotationMode.FaceVelocity:
                    NPC.rotation = NPC.velocity.ToRotation() + ctx.FacingFlip;
                    break;
                case BabyIceDragonRotationMode.TowardsVelocity:
                    NPC.rotation = NPC.rotation.AngleTowards(NPC.velocity.ToRotation() + ctx.FacingFlip, ctx.RotationStep);
                    break;
                default:
                    break;
            }
        }

        /// <summary>纯本地表现：推进残影缓存。不回写任何 gameplay 量（C8 / D8）。</summary>
        private void UpdatePresentation()
        {
            if (!AiContext.DrawShadows)
            {
                shadowsPrimed = false;
                return;
            }

            if (!shadowsPrimed)
            {
                //起冲那一帧把整条缓存填成当前位置，否则残影会从上一招的旧坐标拉出一条假拖影
                for (int i = 0; i < NPC.oldPos.Length; i++)
                    NPC.oldPos[i] = NPC.Center;
                shadowsPrimed = true;
                return;
            }

            for (int i = 0; i < NPC.oldPos.Length - 1; i++)
                NPC.oldPos[i] = NPC.oldPos[i + 1];
            NPC.oldPos[^1] = NPC.Center;
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            //贴图是 3 列 × 5 行的帧表，帧号由声明总线维护
            Rectangle frameBox = mainTex.Frame(3, 5, NPC.frame.X, NPC.frame.Y);
            SpriteEffects effects = NPC.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 origin = frameBox.Size() / 2;
            //表现偏移只加在绘制位上，判定盒不跟抖
            Vector2 drawOffset = AiContext?.DrawOffset ?? Vector2.Zero;

            if (AiContext != null && AiContext.DrawShadows)
            {
                Color color = BabyIceDragonDirector.ShadowColor * BabyIceDragonDirector.ShadowAlpha;
                float scale = NPC.scale;
                for (int i = BabyIceDragonDirector.ShadowCacheLength - 1; i > -1; i -= 2)
                {
                    spriteBatch.Draw(mainTex, NPC.oldPos[i] + drawOffset - screenPos, frameBox, color, NPC.rotation, origin, scale, effects, 0f);
                    color *= BabyIceDragonDirector.ShadowColorFalloff;
                    scale *= BabyIceDragonDirector.ShadowScaleFalloff;
                }
            }

            spriteBatch.Draw(mainTex, NPC.Center + drawOffset - screenPos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);

            float glow = AiContext?.GlowAlpha ?? 0f;
            if (glow > BabyIceDragonDirector.GlowVisibleAlpha)
                spriteBatch.Draw(GlowTex.Value, NPC.Center + drawOffset - screenPos, frameBox, Color.White * glow, NPC.rotation, origin, NPC.scale, effects, 0f);

            return false;
        }

        #endregion

        #region NetWork

        /// <summary>热字段（Timer / Counter / Beat / 自用槽）+ boss 事实（帧图相位）随 SyncNPC 原子过线。Phase 1 各 boss 就这两行。</summary>
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
    }
}
