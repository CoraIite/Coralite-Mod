using Coralite.Content.Items.Shadow;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault;
using InnoVault.StateMachines;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    ///                                               马赛克
    ///           ○○○○○○○○○○ ○                        l   l  
    ///       ○○○○○○○○○○○○○○○○○ ○                     l   l
    ///     ○○○○○○○○○○○○○○○○○○○○○ ○              _ _  l   l_  
    ///    ○○○○○{影}○○○○○○○○○○○{球}○ ○          !  !  l   l l ˉl
    ///   ○○○○○{影影影}○○子○○○{球球球}○ ○        l               l
    /// ○○○○○○○○{影}○○○○○○○○○○○{球}○○○ ○        l               l
    /// ○○○○○○○○○○○○○○○○○○○○○○○○○○○○○ ○          l             l
    ///  ○○○○○○==○○○○○○○○○○○○○○○○○○○ ○           l            l
    ///    ○○○○○○==○○○○○○○○○○○○○○○○ ○            l           l
    ///     ○○○○○○○=========○○○○○ ○              l           l
    ///       ○○○○○○○○○○○○○○○○○ ○
    ///           ○○○○○○○○○○ ○
    /// 
    ///             就贼搁赤玉灵嗷，别让我在影之城看见你嗷，
    ///                 抓到你，指定没你好果汁吃
    ///                     你记住我说的话嗷！
    /// 
    /// </summary>
    public partial class ShadowBall : ModNPC
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        internal ref float SonState => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];

        internal ShadowBallContext AiContext;
        internal CoraliteBossStateMachine<ShadowBallContext> StateMachine;
        internal Random AttackRandom;

        internal int Phase
        {
            get
            {
                int stateId = StateMachine?.CurrentState?.StateId ?? (int)ShadowBallStateId.OnSpawnAnim;
                return stateId >= (int)ShadowBallStateId.P1ToP2Exchange
                    ? (int)AIPhases.ShadowPlayer
                    : (int)AIPhases.WithSmallBalls;
            }
        }

        internal int CurrentStateId => StateMachine?.CurrentState?.StateId ?? (int)ShadowBallStateId.OnSpawnAnim;

        internal ref float Recorder => ref NPC.localAI[0];
        internal ref float Recorder2 => ref NPC.localAI[1];

        public Player Target => Main.player[NPC.target];

        public bool SpawnedSmallBalls;
        public List<NPC> smallBalls = new();
        public int smallBallCount;

        //public Rectangle MovementLimitRect;
        /// <summary>
        /// 生成时自下而上出现的高度
        /// </summary>
        public float SpawnOverflowHeight;
        public bool CanDamage;

        private Player ShadowPlayer;

        public ShadowCircleController[] shadowCircle;

        internal static readonly RasterizerState OverflowHiddenRasterizerState = new()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        public const int ShadowCount = 16;

        /// <summary>
        /// NPC的透明度
        /// </summary>
        public float alpha;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
            Main.npcFrameCount[Type] = 9;
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.damage = 50;
            NPC.defense = 6;
            NPC.lifeMax = 4500;
            NPC.knockBackResist = 0f;
            //NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            //NPC.BossBar = GetInstance<BabyIceDragonBossBar>();

            //BGM：冰结寒流
            //if (!Main.dedServ)
            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((3820 + (numPlayers * 1750)) / journeyScale);
                    NPC.damage = 35;
                    NPC.defense = 12;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((4720 + (numPlayers * 2100)) / journeyScale);
                    NPC.damage = 60;
                    NPC.defense = 15;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 15;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 0.4f;
                }

                return;
            }

            NPC.lifeMax = 3820 + (numPlayers * 1750);
            NPC.damage = 35;
            NPC.defense = 12;

            if (Main.masterMode)
            {
                NPC.lifeMax = 4720 + (numPlayers * 2100);
                NPC.damage = 60;
                NPC.defense = 15;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 5320 + (numPlayers * 2200);
                NPC.damage = 80;
                NPC.defense = 15;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 0.4f;
            }
        }

        public override bool CheckDead()
        {
            //if ((int)State != (int)AIStates.onKillAnim)
            //{
            //    State = (int)AIStates.onKillAnim;
            //    Timer = 0;
            //    NPC.dontTakeDamage = true;
            //    NPC.life = 1;
            //    return false;
            //}

            return true;
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BabyIceDragonRelic>()));
            //npcLoot.Add(ItemDropRule.BossBag(ItemType<BabyIceDragonBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonTrophy>(), 10));

            //LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleCrystal>(), 1, 3, 5));
            //npcLoot.Add(notExpertRule);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (CanDamage)
                return base.CanHitPlayer(target, ref cooldownSlot);

            return false;
        }

        #endregion

        #region AI

        public enum AIPhases
        {
            WithSmallBalls,
            ShadowPlayer,
            BigBallSmash
        }

        public enum AIStates
        {
            OnSpawnAnmi,
            /// <summary> 狂暴，为出框惩罚 </summary>
            Rampage,
            /// <summary> 一阶段招式：小球转转转后射激光 </summary>
            RollingLaser,
            /// <summary> 一阶段招式：小球瞄准玩家后射激光 </summary>
            ConvergeLaser,
            /// <summary> 一阶段招式：一个小球射激光，其他射弹幕 </summary>
            LaserWithBeam,
            /// <summary> 一阶段招式：小球到场地左右两边射激光 </summary>
            LeftRightLaser,
            /// <summary> 一阶段招式：旋转后释放影子玩家 </summary>
            RollingShadowPlayer,
            /// <summary> 一阶段招式：随便射点激光 </summary>
            RandomLaser,
            /// <summary> 一阶段招式：依次射激光 </summary>
            RandomLaser_Master,

            /// <summary> 一阶段和2阶段的切换，使用在2阶段 </summary>
            P1ToP2Exchange,
            /// <summary> 二阶段招式，跳起后斜向下冲刺之后玩家在头顶就升龙拳宰回旋砍，不在就只回旋砍 </summary>
            SmashDown,
            /// <summary> 二阶段招式，与玩家尝试水平后进行斩击，之后大风车 </summary>
            VerticalRolling,
            /// <summary> 二阶段招式，先向斜上方冲刺，之后下砸 </summary>
            SkyJump,
            /// <summary> 二阶段招式，横向冲刺，主要用于过渡 </summary>
            HorizontalDash,
            /// <summary> 二阶段招式，水平冲刺，之后冲向灯之影的位置并向四周抛出弹幕 </summary>
            NightmareKingDash,
        }

        private bool span;
        private bool aiBootstrapped;

        public void Initialize()
        {
            //NPC.Center = CoraliteWorld.shadowBallsFightArea.Center.ToVector2();
            NPC.dontTakeDamage = true;

            //MovementLimitRect = CoraliteWorld.shadowBallsFightArea;
            //MovementLimitRect.X += 200;
            //MovementLimitRect.Y += 200;
            //MovementLimitRect.Width -= 400;
            //MovementLimitRect.Height -= 400;

            //CanDamage = false;

            NPC.oldPos = new Vector2[ShadowCount];
            alpha = 1;
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }

            EnsureAiMachine();

            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Main.dayTime)
            {
                // 丢失目标：重新索敌后继续当前招式（保留阶段不变，避免顶层状态被强行回退导致阶段判定错乱）。
                NPC.TargetClosest();

                if (Main.dayTime && (Target.dead || !Target.active))
                {
                    NPC.EncourageDespawn(10);
                    NPC.dontTakeDamage = true;
                    NPC.velocity.Y += 0.25f;
                    return;
                }
            }

            Lighting.AddLight(NPC.Center, new Vector3(1f, 0.5f, 1.8f));

            // 一阶段每帧刷新小球列表（两端都跑，用于招式协调与阶段判定），与旧 AI 行为一致。
            if (Phase == (int)AIPhases.WithSmallBalls && CurrentStateId != (int)ShadowBallStateId.OnSpawnAnim)
            {
                GetSmallBalls();
            }

            StateMachine.Update();

            UpdateSharedVisuals();
        }

        private void EnsureAiMachine()
        {
            if (aiBootstrapped)
            {
                return;
            }

            AiContext = new ShadowBallContext(this);
            StateMachine = new CoraliteBossStateMachine<ShadowBallContext>(AiContext);

            // 阶段切换：一阶段处于普通招式时，小球全灭 -> 进入 P1ToP2Exchange（仅服务端裁决，客户端经 ai[0] 同步跟随）。
            PhaseController.For(StateMachine)
                .OnCondition(_ => IsInPhase1Attack() && smallBallCount == 0,
                    () => VaultStateRegistry<ShadowBallContext>.Create((int)ShadowBallStateId.P1ToP2Exchange))
                .Apply();

            StateMachine.SetInitialState(VaultStateRegistry<ShadowBallContext>.Create((int)ShadowBallStateId.OnSpawnAnim));
            RefreshAttackRandom();
            aiBootstrapped = true;
        }

        /// <summary>是否处于一阶段（带小球）的常规招式状态（排除出生动画/狂暴/阶段切换）。</summary>
        private bool IsInPhase1Attack()
        {
            int id = CurrentStateId;
            return id >= (int)ShadowBallStateId.RollingLaser && id <= (int)ShadowBallStateId.RandomLaser;
        }

        private void UpdateSharedVisuals()
        {
            if (Phase == (int)AIPhases.WithSmallBalls)
            {
                UpdateFrameNormally();

                if (shadowCircle != null)
                {
                    shadowCircle[0].xRotation += 0.03f;
                    shadowCircle[0].zRotation = NPC.rotation - 1.57f;
                    shadowCircle[0].selfRotation += 0.002f;
                    if (shadowCircle[0].selfRotation > 1)
                        shadowCircle[0].selfRotation -= 1;
                    shadowCircle[0].Update();
                    shadowCircle[1].xRotation += 0.03f;
                    shadowCircle[1].zRotation = NPC.rotation;
                    shadowCircle[1].selfRotation += 0.002f;
                    if (shadowCircle[1].selfRotation > 1)
                        shadowCircle[1].selfRotation -= 1;
                    shadowCircle[1].Update();
                    shadowCircle[2].xRotation += 0.01f;
                    shadowCircle[2].zRotation = 0f;
                    shadowCircle[2].selfRotation += 0.005f;
                    if (shadowCircle[2].selfRotation > 1)
                        shadowCircle[2].selfRotation -= 1;
                    shadowCircle[2].Update();
                }
            }
            else if (Phase == (int)AIPhases.ShadowPlayer && ShadowPlayer != null && !Main.dedServ)
            {
                ShadowPlayer.direction = NPC.spriteDirection;
                ShadowPlayer.velocity = NPC.velocity;
                ShadowPlayer.Center = NPC.Center;
                ShadowPlayer.UpdateDyes();
                ShadowPlayer.UpdateSocialShadow();
                ShadowPlayer.PlayerFrame();
            }
        }

        public void RefreshAttackRandom()
        {
            AttackRandom = AiContext?.CreateAttackRandom() ?? new Random(NPC.whoAmI + 1);
        }

        /// <summary>
        /// 一阶段招式权重表（等价旧 <c>Main.rand.Next(6)</c> 的均匀分布），仅服务端在 <see cref="CompleteCurrentAttack"/> 内选取。
        /// </summary>
        private static readonly WeightedRandomPicker<ShadowBallStateId> Phase1Picker = new(new (ShadowBallStateId, float)[]
        {
            (ShadowBallStateId.RollingLaser, 1f),
            (ShadowBallStateId.ConvergeLaser, 1f),
            (ShadowBallStateId.LaserWithBeam, 1f),
            (ShadowBallStateId.LeftRightLaser, 1f),
            (ShadowBallStateId.RollingShadowPlayer, 1f),
            (ShadowBallStateId.RandomLaser, 1f),
        });

        /// <summary>
        /// 二阶段招式权重表（等价旧 <c>Main.rand.Next(5)</c> 的均匀分布）。
        /// </summary>
        private static readonly WeightedRandomPicker<ShadowBallStateId> Phase2Picker = new(new (ShadowBallStateId, float)[]
        {
            (ShadowBallStateId.SmashDown, 1f),
            (ShadowBallStateId.VerticalRolling, 1f),
            (ShadowBallStateId.SkyJump, 1f),
            (ShadowBallStateId.HorizontalDash, 1f),
            (ShadowBallStateId.NightmareKingDash, 1f),
        });

        /// <summary>招式收尾：仅服务端推进到下一个招式状态（ai[0] 自动同步给客户端）。</summary>
        public void CompleteCurrentAttack()
        {
            if (VaultUtils.isClient || StateMachine == null)
            {
                return;
            }

            IVaultState<ShadowBallContext> next = PickNextAttackState();
            if (next != null)
            {
                StateMachine.ChangeState(next);
            }
        }

        /// <summary>
        /// 仅服务端：按当前阶段用权重选招器选取下一个招式。<br/>
        /// 服务端用 <see cref="Main.rand"/> 取 seed，权重选招纯函数化，结果以状态 ID 经 ai[0] 同步，无需再单独同步 seed。
        /// </summary>
        public IVaultState<ShadowBallContext> PickNextAttackState()
        {
            WeightedRandomPicker<ShadowBallStateId> picker =
                Phase == (int)AIPhases.ShadowPlayer ? Phase2Picker : Phase1Picker;

            int seed = Main.rand.Next();
            ShadowBallStateId pick = picker.Pick(seed).Item;
            return VaultStateRegistry<ShadowBallContext>.Create((int)pick);
        }

        #endregion

        #region States

        public void ExchangeToPhase2()
        {
            Timer = 0;
            SonState = 0;
            Recorder = 0;
            Recorder2 = 0;

            NPC.TargetClosest();
            ApplyPhase2Hitbox();
            ExchangeToPhase2VisualOnly();
        }

        public void ApplyPhase2Hitbox()
        {
            Vector2 center = NPC.Center;
            NPC.width = (int)(32 * NPC.scale);
            NPC.height = (int)(48 * NPC.scale);
            NPC.Center = center;
        }

        public void ExchangeToPhase2VisualOnly()
        {
            if (Main.dedServ)
            {
                return;
            }

            ShadowPlayer = Target.clientClone();
            ShadowPlayer.armor[10] = new Item(ModContent.ItemType<ShadowHead>());
            ShadowPlayer.armor[11] = new Item(ModContent.ItemType<ShadowBreastplate>());
            ShadowPlayer.armor[12] = new Item(ModContent.ItemType<ShadowLegs>());
            ShadowPlayer.ResetVisibleAccessories();
        }

        #endregion

        #region HelperMethods

        public bool GetSmallBalls()
        {
            smallBalls.Clear();
            int count = 0;
            for (int i = 0; i < 200; i++)
                if (Main.npc[i].active &&
                    Main.npc[i].type == ModContent.NPCType<SmallShadowBall>() &&
                    Main.npc[i].ai[0] == NPC.whoAmI &&//小球主人是自己
                    Main.npc[i].ai[1] != (int)SmallShadowBall.AIStates.OnKillAnmi)//小球不在死亡动画
                {
                    smallBalls.Add(Main.npc[i]);
                    count++;
                    if (count >= 5)
                    {
                        break;
                    }
                }

            smallBallCount = count;
            if (count == 0)
                return false;

            return true;
        }

        public bool CheckSmallBallsReady()
        {
            if (smallBallCount == 0)
            {
                return false;
            }

            foreach (var ball in smallBalls)
            {
                if (ball.ModNPC is not SmallShadowBall sb || !sb.IsOrchestrationReady(NPC))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>服务端：所有子球当前顶层 FSM 均已 MarkTerminated（招结束）。</summary>
        public bool CheckAllSmallBallsTerminated()
        {
            if (smallBallCount == 0)
            {
                return false;
            }

            foreach (var ball in smallBalls)
            {
                if (ball.ModNPC is not SmallShadowBall sb)
                {
                    return false;
                }

                sb.EnsureStateMachinePublic();
                if (sb.StateMachine == null || !sb.StateMachine.IsTerminated)
                {
                    return false;
                }
            }

            return true;
        }

        private static int NextSmallBallSeed(Random attackRandom)
        {
            return attackRandom.Next();
        }

        public void SetDirection(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            NPC.direction = NPC.spriteDirection = xLength > 0 ? -1 : 1;
            NPC.directionY = yLength > 0 ? -1 : 1;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        public void SpawnSmallBalls()
        {
            if (VaultUtils.isClient || SpawnedSmallBalls)
            {
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                int index = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y,
                    ModContent.NPCType<SmallShadowBall>(), NPC.whoAmI, NPC.whoAmI);
                (Main.npc[index].ModNPC as SmallShadowBall).smallBallType = i;
                (Main.npc[index].ModNPC as SmallShadowBall).shadowCircle =
                    new ShadowCircleController
                    (ModContent.Request<Texture2D>(AssetDirectory.ShadowBalls + "SmallCircle" + i, ReLogic.Content.AssetRequestMode.ImmediateLoad));
            }

            SpawnedSmallBalls = true;
        }

        //public void MovementLimit()
        //{
            //Vector2 center = NPC.Center;
            //center.X = Math.Clamp(center.X, MovementLimitRect.X, MovementLimitRect.X + MovementLimitRect.Width);
            //center.Y = Math.Clamp(center.Y, MovementLimitRect.Y, MovementLimitRect.Y + MovementLimitRect.Height);
            //NPC.Center = center;
        //}

        public void InitCaches()
        {
            for (int i = 0; i < ShadowCount; i++)
                NPC.oldPos[i] = NPC.Center;
        }

        public void UpdateCachesNormally()
        {
            for (int i = ShadowCount - 1; i > 0; i--)
                NPC.oldPos[i] = NPC.oldPos[i - 1];
            NPC.oldPos[0] = NPC.Center;
        }

        public void UpdateFrameNormally()
        {
            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 8)
                    NPC.frame.Y = 0;
            }
        }

        /// <summary>
        /// 让拖尾数组随机出现在NPC周围的一个圆圈范围
        /// </summary>
        /// <param name="width"></param>
        public void UpdateCacheRandom(float width, int percent)
        {
            for (int i = 0; i < ShadowCount; i++)
            {
                if (Main.rand.NextBool(percent, 100))
                    NPC.oldPos[i] = NPC.Center + Main.rand.NextVector2Circular(width, width);
            }
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.ModProjectile is IShadowBallPrimitive primitive)
                    primitive.DrawPrimitive(spriteBatch);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            switch (Phase)
            {
                default:
                case (int)AIPhases.WithSmallBalls:
                    {
                        var pos = NPC.Center - screenPos;

                        if (CurrentStateId == (int)ShadowBallStateId.OnSpawnAnim)
                        {
                            Texture2D mainTex = NPC.GetTexture();

                            var frameBox = mainTex.Frame(1, 9, 0, NPC.frame.Y);
                            var origin = frameBox.Size() / 2;

                            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

                            spriteBatch.End();
                            Rectangle scissorRectangle2 = Rectangle.Intersect(GetClippingRectangle(spriteBatch, pos, frameBox), spriteBatch.GraphicsDevice.ScissorRectangle);
                            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
                            spriteBatch.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

                            spriteBatch.Draw(mainTex, pos, frameBox, drawColor * alpha, 0, origin, NPC.scale, 0, 0);

                            rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
                            spriteBatch.End();
                            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
                            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null);

                            return false;
                        }

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap/*注意了奥*/, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

                        for (int i = 2; i >= 0; i--)
                        {
                            shadowCircle[i]?.DrawBackCircle_NoEndBegin(pos, drawColor);
                        }
                        DrawSelf(spriteBatch, screenPos, drawColor * alpha);
                        for (int i = 0; i < 3; i++)
                        {
                            shadowCircle[i]?.DrawFrontCircle(spriteBatch, pos, drawColor);
                        }

                        spriteBatch.End();
                        spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

                    }
                    break;
                case (int)AIPhases.ShadowPlayer:
                    {
                        //绘制影子玩家
                        Vector2 pos = NPC.Center - new Vector2(16, 24);

                        for (int i = 0; i < ShadowCount / 2; i++)
                        {
                            Main.PlayerRenderer.DrawPlayer(Main.Camera, ShadowPlayer, NPC.oldPos[i] - new Vector2(16, 24),
                                0, new Vector2(16, 24), Helper.Lerp(1 - alpha, 1, (float)i / (ShadowCount / 2)));
                        }

                        Main.PlayerRenderer.DrawPlayer(Main.Camera, ShadowPlayer, pos,
                            0, new Vector2(16, 24), 1 - alpha);
                    }
                    break;
            }

            return false;
        }

        public void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            var pos = NPC.Center - screenPos;
            var frameBox = mainTex.Frame(1, 9, 0, NPC.frame.Y);
            var origin = frameBox.Size() / 2;

            for (int i = 0; i < ShadowCount / 2; i++)
            {
                spriteBatch.Draw(mainTex, NPC.oldPos[i] - screenPos, frameBox, drawColor * alpha * (1 - ((float)i / (ShadowCount / 2)))
                    , 0, origin, NPC.scale, 0, 0);
            }

            spriteBatch.Draw(mainTex, pos, frameBox, drawColor * alpha, 0, origin, NPC.scale, 0, 0);

        }

        public Rectangle GetClippingRectangle(SpriteBatch spriteBatch, Vector2 center, Rectangle frameBox)
        {
            float height = SpawnOverflowHeight * frameBox.Height;
            Vector2 position = center + new Vector2(-frameBox.Width / 2, (frameBox.Height / 2) - height);
            Vector2 size = new(frameBox.Width, height);

            position = Vector2.Transform(position, Main.Transform);
            //size = Vector2.Transform(size, Main.Transform);
            size *= Main.GameZoomTarget;

            Rectangle rectangle = new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            int screenWidth = Main.screenWidth;
            int screenHeight = Main.screenHeight;
            rectangle.X = Utils.Clamp(rectangle.X, 0, screenWidth);
            rectangle.Y = Utils.Clamp(rectangle.Y, 0, screenHeight);
            rectangle.Width = Utils.Clamp(rectangle.Width, 0, screenWidth - rectangle.X);
            rectangle.Height = Utils.Clamp(rectangle.Height, 0, screenHeight - rectangle.Y);
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            int num3 = Utils.Clamp(rectangle.Left, scissorRectangle.Left, scissorRectangle.Right);
            int num4 = Utils.Clamp(rectangle.Top, scissorRectangle.Top, scissorRectangle.Bottom);
            int num5 = Utils.Clamp(rectangle.Right, scissorRectangle.Left, scissorRectangle.Right);
            int num6 = Utils.Clamp(rectangle.Bottom, scissorRectangle.Top, scissorRectangle.Bottom);
            return new Rectangle(num3, num4, num5 - num3, num6 - num4);
        }

        #endregion
    }
}
