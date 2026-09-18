using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 泰山压顶：缩进王冠 → 瞬移到玩家头顶 450 px → 追着头顶飘 → 悬停蓄力 → 变回史莱姆整个砸下来 → 四拍回弹。<br/>
    /// 节拍：Shrink0..3 → Teleport → Chase → Drop → Bounce1..3 → Restore → Done。<br/>
    /// 公平阀分三层：瞬移前 60 帧（挑战 30）蓄力尘读秒；头顶追踪 90 帧里玩家可以跑位甩开锚点；
    /// 真正的出砸前还有 25 帧悬停（速度衰减到近乎静止 + 每帧 4 粒尘），这一段是唯一的“它要砸了”的定身预告。<br/>
    /// 下坠段 <c>noGravity + noTileCollide</c> 每帧重声明，客户端中途收养也能接上；120 帧没砸到地面就算砸空，跳过回弹直接收招。<br/>
    /// 旧 <c>SlimeEmperor.BodySlam</c>（AI.BodySlam.cs:13-264）。
    /// </summary>
    [VaultState((int)AIStates.BodySlam, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorBodySlamState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.BodySlam;

        private enum Beat
        {
            /// <summary>缩进四拍（旧 SonState 0..3）</summary>
            Shrink0 = 0,
            Shrink1 = 1,
            Shrink2 = 2,
            Shrink3 = 3,
            /// <summary>蓄力 → 瞬移到头顶（旧 4）</summary>
            Teleport = 4,
            /// <summary>头顶追踪 + 悬停蓄力（旧 5）</summary>
            Chase = 5,
            /// <summary>下坠砸地（旧 6）</summary>
            Drop = 6,
            /// <summary>回弹一：压扁（旧 7）</summary>
            Bounce1 = 7,
            /// <summary>回弹二：拉长，王冠被顶起（旧 8）</summary>
            Bounce2 = 8,
            /// <summary>回弹三：再压一下（旧 9）</summary>
            Bounce3 = 9,
            /// <summary>复原（旧 10）</summary>
            Restore = 10,
            /// <summary>收招（旧 11，砸空也直接到这）</summary>
            Done = 11,
        }

        /// <summary>瞬移前的旧位置（弹力球撒在这里），<see cref="AuthorityUpdate"/> 同帧消费。</summary>
        private Vector2 teleportFrom;
        private bool teleportedThisFrame;
        private bool smashedThisFrame;

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            teleportedThisFrame = false;
            smashedThisFrame = false;

            //砸地后的残影一直留到收招（旧 CanDrawShadow 只在换招时才复位），这里由拍号 + Timer 确定性推出，不用额外过线
            if (BeatIndex > (int)Beat.Drop || (BeatIndex == (int)Beat.Drop && Timer >= SlimeEmperorDirector.BodySlamShadowFrame))
            {
                ctx.DrawShadow = true;
            }

            switch ((Beat)BeatIndex)
            {
                case Beat.Shrink0:
                case Beat.Shrink1:
                case Beat.Shrink2:
                case Beat.Shrink3:
                    UpdateCrownShrinkBeats(ctx);
                    break;

                case Beat.Teleport:
                    UpdateTeleport(ctx);
                    break;

                case Beat.Chase:
                    UpdateChase(ctx);
                    break;

                case Beat.Drop:
                    UpdateDrop(ctx);
                    break;

                case Beat.Bounce1:
                    ScaleBeat(ctx, SlimeEmperorDirector.BodySlamBounce1X, SlimeEmperorDirector.BodySlamBounce1Y, SlimeEmperorDirector.BodySlamBounce1Lerp,
                        ctx.Scale.Y < SlimeEmperorDirector.BodySlamBounce1Done, (int)Beat.Bounce2);
                    break;

                case Beat.Bounce2:
                    if (ScaleBeat(ctx, SlimeEmperorDirector.BodySlamBounce2X, SlimeEmperorDirector.BodySlamBounce2Y, SlimeEmperorDirector.BodySlamBounce2Lerp,
                        ctx.Scale.X < SlimeEmperorDirector.BodySlamBounce2Done, (int)Beat.Bounce3))
                    {
                        ctx.Boss.CrownJumpUp(SlimeEmperorDirector.BodySlamCrownPopLimit, SlimeEmperorDirector.BodySlamCrownPopSpeed);
                    }

                    break;

                case Beat.Bounce3:
                    ScaleBeat(ctx, SlimeEmperorDirector.BodySlamBounce3X, SlimeEmperorDirector.BodySlamBounce3Y, SlimeEmperorDirector.BodySlamBounce2Lerp,
                        ctx.Scale.Y < SlimeEmperorDirector.BodySlamBounce3Done, (int)Beat.Restore);
                    break;

                case Beat.Restore:
                    if (ScaleBeat(ctx, 1f, 1f, SlimeEmperorDirector.BodySlamRestoreLerp, ctx.ScaleRestored(true), (int)Beat.Done))
                    {
                        ctx.Scale = Vector2.One;
                    }

                    break;
            }
        }

        /// <summary>蓄力后瞬移到玩家正上方 450 px。</summary>
        private void UpdateTeleport(SlimeEmperorContext ctx)
        {
            int readyFrames = ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus2_1)
                ? SlimeEmperorDirector.TeleportReadyFramesFast
                : SlimeEmperorDirector.TeleportReadyFrames;

            if (Timer < readyFrames)
            {
                SpawnTeleportChargeDust(ctx, Timer / (float)readyFrames);
                return;
            }

            if (Timer > readyFrames)
            {
                EnterBeat(ctx, (int)Beat.Chase);
                ctx.Npc.TargetClosest();
                return;
            }

            NPC npc = ctx.Npc;
            npc.TargetClosest();
            teleportFrom = npc.Center;
            npc.Center = ctx.Target.Center + new Vector2(0, SlimeEmperorDirector.BodySlamHeight);

            PlaySound(CoraliteSoundID.SlimeMount_Item81, ctx);
            SpawnTeleportDust(ctx, teleportFrom);
            ctx.DeclareDirect();
            ctx.MarkDecision();
            teleportedThisFrame = true;
        }

        /// <summary>追着玩家头顶的锚点飘，然后悬停蓄力；蓄满就换形态出砸。</summary>
        private void UpdateChase(SlimeEmperorContext ctx)
        {
            NPC npc = ctx.Npc;
            int chaseFrames = ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus2_1)
                ? SlimeEmperorDirector.BodySlamChaseFramesFast
                : SlimeEmperorDirector.BodySlamChaseFrames;

            if (Timer < chaseFrames)
            {
                Vector2 anchor = ctx.Target.Center + new Vector2(0, SlimeEmperorDirector.BodySlamHeight);
                KeepDistance(ctx, anchor, SlimeEmperorDirector.BodySlamKeepRadius,
                    SlimeEmperorDirector.BodySlamChaseAccel, SlimeEmperorDirector.BodySlamChaseMaxSpeed);
                return;
            }

            //悬停蓄力：这是出砸前唯一的定身预告窗
            if (Timer < chaseFrames + SlimeEmperorDirector.BodySlamHoverFrames)
            {
                ctx.DeclareDamp(SlimeEmperorDirector.BodySlamHoverDamp);
                SpawnHoverDust(ctx);
                return;
            }

            EnterBeat(ctx, (int)Beat.Drop);
            ctx.EnterSlimeForm();
            ctx.Scale = new Vector2(SlimeEmperorDirector.BodySlamDropScaleX, SlimeEmperorDirector.BodySlamDropScaleY);
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.velocity.X *= SlimeEmperorDirector.BodySlamDropDampX;
            npc.velocity.Y = SlimeEmperorDirector.BodySlamDropSpeedY;
            npc.frame.Y = 0;
            npc.TargetClosest();
            ctx.DeclareDirect();
            PlaySound(CoraliteSoundID.SlimeMount_Item81, ctx);
        }

        /// <summary>下坠：加速到 24，脚下有可站立物块就砸；120 帧还没砸到就算砸空。</summary>
        private void UpdateDrop(SlimeEmperorContext ctx)
        {
            NPC npc = ctx.Npc;

            //形态标志每帧重声明：客户端被 NetSync 切进这一拍时同样要处在穿墙下坠状态
            npc.noGravity = true;
            npc.noTileCollide = true;

            npc.velocity.Y += SlimeEmperorDirector.BodySlamFallAccel;
            if (npc.velocity.Y > SlimeEmperorDirector.BodySlamFallMax)
            {
                npc.velocity.Y = SlimeEmperorDirector.BodySlamFallMax;
            }

            ctx.DeclareDirect();

            if (Timer < SlimeEmperorDirector.BodySlamFallTimeout)
            {
                //只在不高于玩家 100 px 时才检测地面，否则会砸在玩家所在平台的上一层
                if (npc.Center.Y > ctx.Target.Center.Y - SlimeEmperorDirector.BodySlamGroundMargin && TouchingGround(npc))
                {
                    Smash(ctx);
                }

                return;
            }

            //砸空：收住速度直接收招，不演回弹
            npc.rotation = 0;
            npc.velocity *= 0;
            npc.noTileCollide = false;
            npc.noGravity = false;
            EnterBeat(ctx, (int)Beat.Done);
        }

        /// <summary>砸地那一帧：刹停、落回物块碰撞、烟尘与音效（两端），震地弹幕交给权威端。</summary>
        private void Smash(SlimeEmperorContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.rotation = 0;
            npc.velocity *= 0;
            npc.noGravity = false;
            npc.noTileCollide = false;

            PlaySound(CoraliteSoundID.QueneSlimeFalling_Item167, ctx);
            SpawnSmashSmoke(npc);
            smashedThisFrame = true;
            EnterBeat(ctx, (int)Beat.Bounce1);
        }

        /// <summary>脚下 width/16 列 × 第 1~2 行内有可站立物块。沿用旧值 AI.BodySlam.cs:187-191</summary>
        private static bool TouchingGround(NPC npc)
        {
            Point position = npc.BottomLeft.ToTileCoordinates();
            int width = npc.width / 16;
            for (int i = 0; i < width; i++)
            {
                for (int j = SlimeEmperorDirector.BodySlamGroundRowFrom; j < SlimeEmperorDirector.BodySlamGroundRowTo; j++)
                {
                    if (WorldGen.ActiveAndWalkableTile(position.X + i, position.Y + j))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (teleportedThisFrame)
            {
                teleportedThisFrame = false;
                SpawnElasticBalls(ctx, teleportFrom, SlimeEmperorDirector.BodySlamBallCount(),
                    () => Helper.NextVec2Dir(SlimeEmperorDirector.BallScatterSpeedMin, SlimeEmperorDirector.BallScatterSpeedMax));
            }

            if (smashedThisFrame)
            {
                smashedThisFrame = false;
                ctx.SpawnHostile(ctx.Npc.GetSource_FromAI(), ctx.Npc.Bottom, Vector2.Zero,
                    ProjectileID.QueenSlimeSmash, SlimeEmperorDirector.BodySlamSmashDamage, 0f, Main.myPlayer);
            }

            //大师模式回弹二：顺势向上喷一束弹弹凝胶球
            if (EnteredBeat == (int)Beat.Bounce3 && Main.masterMode)
            {
                ShootUpward<GelBall>(ctx, SlimeEmperorDirector.BodySlamGelBallCount(), SlimeEmperorDirector.BodySlamGelBallDamage(),
                    SlimeEmperorDirector.BodySlamGelBallSpeed, SlimeEmperorDirector.BodySlamGelBallSpread, SlimeEmperorDirector.BodySlamGelBallKnockback);
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }

        #region 表现（纯本地）

        /// <summary>悬停蓄力尘。</summary>
        private static void SpawnHoverDust(SlimeEmperorContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            for (int i = 0; i < SlimeEmperorDirector.BodySlamHoverDustCount; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    npc.Center + Main.rand.NextVector2Circular(SlimeEmperorDirector.BodySlamHoverDustScatter, SlimeEmperorDirector.BodySlamHoverDustScatter),
                    DustID.TintableDust,
                    -npc.velocity * Main.rand.NextFloat(SlimeEmperorDirector.BodySlamHoverDustSpeedMin, SlimeEmperorDirector.BodySlamHoverDustSpeedMax),
                    SlimeEmperorDirector.GelDustAlpha, SlimeEmperorDirector.GelDustColor, SlimeEmperorDirector.BodySlamHoverDustScale);
                dust.noGravity = true;
            }
        }

        /// <summary>砸地烟尘。宽度用的是<b>格数</b>（旧代码如此，不是像素），照搬。沿用旧值 AI.BodySlam.cs:202-209</summary>
        private static void SpawnSmashSmoke(NPC npc)
        {
            if (Main.dedServ)
            {
                return;
            }

            int width = npc.width / 16;
            for (int i = 0; i < SlimeEmperorDirector.BodySlamSmokeCount; i++)
            {
                int index = Dust.NewDust(npc.Bottom - new Vector2(width / 2, SlimeEmperorDirector.BodySlamSmokeHeight),
                    width, (int)SlimeEmperorDirector.BodySlamSmokeHeight, DustID.Smoke,
                    npc.velocity.X, npc.velocity.Y, SlimeEmperorDirector.SmokeDustAlpha, SlimeEmperorDirector.GelDustColor);
                Main.dust[index].noGravity = true;
                Main.dust[index].velocity.Y = -SlimeEmperorDirector.BodySlamSmokeUpMin + (Main.rand.NextFloat() * -SlimeEmperorDirector.BodySlamSmokeUpMax);
                Main.dust[index].velocity.X *= SlimeEmperorDirector.BodySlamSmokeSpreadX;
            }
        }

        #endregion
    }
}
