using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 下砸：爬到目标头顶 430 px 且横向 250 px 内 → 停住闪光（可见起手）→ 加速砸下并在前 40 帧内追横向 → 落地炸出两侧共 8 根冰刺 + 震屏 → 进后摇。<br/>
    /// 公平阀：下落中 60 帧后才恢复物块碰撞，落点看得见；冰刺只沿地面向两侧扩散，纵向离地就安全；砸空 200 帧也会收招。<br/>
    /// 落地判定两端同算（只读物块），弹幕与换态只在权威端。旧 AI.SmashDown.cs + BabyIceDragon.cs:734-828
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.smashDown, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonSmashDownState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.smashDown;

        private enum Beat
        {
            /// <summary>爬升并对齐横向。</summary>
            Approach,
            /// <summary>砸下。</summary>
            Fall,
        }

        /// <summary>本地已判定落地（两端各自从物块推导，只用于一次性演出与权威端出手）。</summary>
        private bool landed;

        public override void OnEnter(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            base.OnEnter(machine, ctx);
            landed = false;
        }

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if ((Beat)BeatIndex == Beat.Approach)
            {
                UpdateApproach(ctx);
                return;
            }

            UpdateFall(ctx);
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if ((Beat)BeatIndex == Beat.Approach)
            {
                return Timer > BabyIceDragonDirector.SmashApproachTimeout ? EndAttack(ctx) : null;
            }

            if (landed)
            {
                SpawnThorns(ctx);
                return EnterRest(ctx, BabyIceDragonDirector.RestFrames);
            }

            if (Timer > BabyIceDragonDirector.SmashTimeoutFrame)
            {
                return EnterRest(ctx, BabyIceDragonDirector.RestFrames);
            }

            return null;
        }

        /// <summary>就位：不够高就扇翅上飞，够高就收 Y 速；横向超过 150 px 就追。旧 AI.SmashDown.cs:18-63</summary>
        private void UpdateApproach(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            bool lowerThanTarget = npc.Center.Y > ctx.Target.Center.Y - BabyIceDragonDirector.SmashClimbHeight;
            float xLength = Math.Abs(npc.Center.X - ctx.Target.Center.X);

            if (lowerThanTarget || xLength > BabyIceDragonDirector.SmashAlignX)
            {
                ctx.FaceTarget();
                if (lowerThanTarget)
                {
                    ctx.DeclareFlyUp();
                }
                else
                {
                    ctx.DeclareDampY(BabyIceDragonDirector.ClimbHoverDampY);
                    ctx.DeclareFlyingFrame();
                }

                ctx.DeclareApproachX(BabyIceDragonDirector.SmashApproachDeadZoneX, BabyIceDragonDirector.SmashApproachSpeedX,
                    BabyIceDragonDirector.SmashApproachAccelX, BabyIceDragonDirector.SmashApproachTurnX,
                    BabyIceDragonDirector.SmashApproachDamp, BabyIceDragonDirector.SmashApproachDamp);
                return;
            }

            // 起手：张嘴定格、横速砍到四成、纵速归零、闪光提示
            ctx.SetFrame(1, 1);
            npc.velocity.X *= BabyIceDragonDirector.SmashStartDampX;
            npc.velocity.Y = 0f;
            ctx.DeclareDirect();
            ctx.FaceTarget();

            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, npc.Center);
                PRTLoader.NewParticle(npc.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.IcicleCyan, BabyIceDragonDirector.SmashCueSparkScale);
            }

            ChangeBeat(ctx, (int)Beat.Fall);
        }

        /// <summary>砸下：匀加速下落带上限、朝向跟速度、前 40 帧追横向，60 帧后恢复物块碰撞并检测脚下。旧 AI.SmashDown.cs:65-128</summary>
        private void UpdateFall(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;

            // 砸空兜底：停住等权威端收招（两端同做，避免只有权威端刹住）
            if (Timer > BabyIceDragonDirector.SmashTimeoutFrame)
            {
                ctx.FaceTarget();
                npc.rotation = 0f;
                npc.velocity = Vector2.Zero;
                ctx.DeclareDirect();
                ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
                ctx.TileCollide = true;
                return;
            }

            ctx.DeclareRotation(BabyIceDragonRotationMode.TowardsVelocity, BabyIceDragonDirector.SmashFallRotationStep);
            ctx.DeclareAccelY(BabyIceDragonDirector.SmashFallAccelY, BabyIceDragonDirector.SmashFallMaxY);

            if (Timer < BabyIceDragonDirector.SmashChaseFrames)
            {
                ctx.DeclareChaseX(BabyIceDragonDirector.SmashChaseSpeedX, BabyIceDragonDirector.SmashChaseAccelX,
                    BabyIceDragonDirector.SmashChaseTurnX, BabyIceDragonDirector.SmashChaseDamp);
            }
            else
            {
                ctx.DeclareDampX(BabyIceDragonDirector.SmashChaseDamp);
            }

            FallDust(ctx);

            // 已经低于目标头顶就提前恢复物块碰撞（旧代码把 Timer 直接跳到 61）
            if (Timer < BabyIceDragonDirector.SmashCollideFrame)
            {
                if (npc.Center.Y > ctx.Target.Top.Y - BabyIceDragonDirector.SmashPassOffsetY)
                {
                    Timer = BabyIceDragonDirector.SmashCollideFrame + 1;
                    ctx.TileCollide = true;
                    ctx.MarkDecision();
                }

                return;
            }

            ctx.TileCollide = true;

            if (landed || !GroundBelow(ctx))
            {
                return;
            }

            landed = true;
            npc.rotation = 0f;
            npc.velocity = Vector2.Zero;
            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.Direct);
            LandingShake(ctx);
        }

        /// <summary>下落尘：每 2 帧 1 粒霜 + 2 粒风暴（纯本地）。旧 AI.SmashDown.cs:70-79</summary>
        private void FallDust(BabyIceDragonContext ctx)
        {
            if (Main.dedServ || Timer % BabyIceDragonDirector.SmashDustInterval != 0)
            {
                return;
            }

            NPC npc = ctx.Npc;
            Dust dust = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(BabyIceDragonDirector.SmashFrostDustSpread, BabyIceDragonDirector.SmashFrostDustSpread),
                DustID.FrostStaff, -npc.velocity * BabyIceDragonDirector.SmashFrostDustBack,
                Scale: Main.rand.NextFloat(BabyIceDragonDirector.SmashFrostDustScaleMin, BabyIceDragonDirector.SmashFrostDustScaleMax));
            dust.noGravity = true;

            for (int i = 0; i < BabyIceDragonDirector.SmashStormDustCount; i++)
            {
                Dust dust2 = Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(BabyIceDragonDirector.SmashStormDustSpread, BabyIceDragonDirector.SmashStormDustSpread),
                    DustID.ApprenticeStorm, -npc.velocity * BabyIceDragonDirector.SmashStormDustBack,
                    Scale: Main.rand.NextFloat(BabyIceDragonDirector.SmashStormDustScaleMin, BabyIceDragonDirector.SmashStormDustScaleMax));
                dust2.noGravity = true;
            }
        }

        /// <summary>脚下两行内有可站立物块即视为砸到地面。旧 AI.SmashDown.cs:100-119</summary>
        private static bool GroundBelow(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 position = npc.BottomLeft / 16f;
            int count = (npc.width / 16) + 1;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < BabyIceDragonDirector.SmashGroundCheckRows; j++)
                {
                    if (WorldGen.ActiveAndWalkableTile((int)position.X + i, (int)position.Y + j))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>落地震屏（纯本地）。旧 BabyIceDragon.cs:736-740</summary>
        private static void LandingShake(BabyIceDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            PunchCameraModifier modifier = new(ctx.Npc.Center, new Vector2(0f, 1f), BabyIceDragonDirector.ThornsShakeStrength,
                BabyIceDragonDirector.ThornsShakeVibration, BabyIceDragonDirector.ThornsShakeFrames,
                BabyIceDragonDirector.ShakeFalloffDistance, "BabyIceDragon");
            Main.instance.CameraModifiers.Add(modifier);
        }

        /// <summary>两侧各 4 根冰刺，逐根加大角度与体型。旧 BabyIceDragon.cs:742-759</summary>
        private static void SpawnThorns(BabyIceDragonContext ctx)
        {
            Point sourceTileCoords = ctx.Npc.Bottom.ToTileCoordinates();
            for (int i = 0; i < BabyIceDragonDirector.ThornsPerSide; i++)
            {
                TryMakingSpike(ctx, ref sourceTileCoords, 1, i * BabyIceDragonDirector.ThornsAngleIndexStep, i * BabyIceDragonDirector.ThornsScaleStep);
                sourceTileCoords.X += BabyIceDragonDirector.ThornsColumnStep;
            }

            sourceTileCoords = ctx.Npc.Bottom.ToTileCoordinates();
            for (int i = 0; i < BabyIceDragonDirector.ThornsPerSide; i++)
            {
                TryMakingSpike(ctx, ref sourceTileCoords, -1, i * BabyIceDragonDirector.ThornsAngleIndexStep, i * BabyIceDragonDirector.ThornsScaleStep);
                sourceTileCoords.X -= BabyIceDragonDirector.ThornsColumnStep;
            }

            ctx.MarkDecision();
        }

        /// <summary>
        /// 在给定列的地面上竖一根冰刺；<paramref name="whichOne"/> 决定外倾角度、<paramref name="scaleOffset"/> 决定体型。
        /// 位置换算里的 16 / 8 是格宽与半格，不是可调值。旧 BabyIceDragon.cs:761-773
        /// </summary>
        private static void TryMakingSpike(BabyIceDragonContext ctx, ref Point sourceTileCoords, int dir, int whichOne, float scaleOffset)
        {
            const int xOffset = 1;
            int howMany = BabyIceDragonDirector.ThornsAngleSteps;
            int positionX = sourceTileCoords.X + (xOffset * dir);
            int positionY = FindBestY(ctx, ref sourceTileCoords, positionX);
            if (!WorldGen.ActiveAndWalkableTile(positionX, positionY))
            {
                return;
            }

            Vector2 position = new((positionX * 16) + 8, (positionY * 16) - 8);
            Vector2 velocity = new Vector2(0f, -1f).RotatedBy(whichOne * dir * BabyIceDragonDirector.ThornsAngleFactor * (MathHelper.PiOver4 / howMany));
            float scale = BabyIceDragonDirector.ThornsBaseScale + scaleOffset + (xOffset * BabyIceDragonDirector.ThornsScaleXFactor / howMany);
            ctx.SpawnHostile(ctx.Npc.GetSource_FromAI(), position, velocity, ProjectileID.DeerclopsIceSpike,
                BabyIceDragonDirector.ThornsDamage(), 0f, ctx.Npc.target, 0f, scale);
        }

        /// <summary>沿目标脚下方向找最近的可站立格，再上探 / 下探修正。旧 BabyIceDragon.cs:775-828</summary>
        private static int FindBestY(BabyIceDragonContext ctx, ref Point sourceTileCoords, int x)
        {
            int positionY = sourceTileCoords.Y;
            NPCAimedTarget targetData = ctx.Npc.GetTargetData();
            if (!targetData.Invalid)
            {
                Rectangle hitbox = targetData.Hitbox;
                Vector2 footPoint = new(hitbox.Center.X, hitbox.Bottom);
                int targetRow = (int)(footPoint.Y / 16f);
                int step = Math.Sign(targetRow - positionY);
                int stopRow = targetRow + (step * BabyIceDragonDirector.ThornsSearchRows);
                int? best = null;
                float bestDistance = float.PositiveInfinity;
                for (int i = positionY; i != stopRow; i += step)
                {
                    if (!WorldGen.ActiveAndWalkableTile(x, i))
                    {
                        continue;
                    }

                    float distance = new Point(x, i).ToWorldCoordinates().Distance(footPoint);
                    if (!best.HasValue || !(distance >= bestDistance))
                    {
                        best = i;
                        bestDistance = distance;
                    }
                }

                if (best.HasValue)
                {
                    positionY = best.Value;
                }
            }

            for (int j = 0; j < BabyIceDragonDirector.ThornsProbeRows; j++)
            {
                if (positionY < BabyIceDragonDirector.ThornsWorldMargin || !WorldGen.SolidTile(x, positionY))
                {
                    break;
                }

                positionY--;
            }

            for (int k = 0; k < BabyIceDragonDirector.ThornsProbeRows; k++)
            {
                if (positionY > Main.maxTilesY - BabyIceDragonDirector.ThornsWorldMargin || WorldGen.ActiveAndWalkableTile(x, positionY))
                {
                    break;
                }

                positionY++;
            }

            return positionY;
        }
    }
}
