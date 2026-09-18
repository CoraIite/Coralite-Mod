using Coralite.Content.NPCs.Crystalline.Core;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 转阶段演出（血量跌破一半触发，全程无敌）：<br/>
    /// Break：外壳按 16 块 Gore 逐块崩开、每 2 帧一个碎岩爆点，帧图 6 帧一格推到第 7 帧。<br/>
    /// Hold：第 7 帧定格 45 帧——这是整场唯一的长定格，让玩家看清它正在重组。<br/>
    /// Rebuild：帧图推到第 19 帧，然后重建二阶段部位（浮游炮 + 双手）、解除无敌、喊
    /// “![ ▼ M ▼ ]＃”并带一个向上的初速转入悬浮。<br/>
    /// 旧 <c>Exchange</c>（CrystallineSentinel.cs:1809-1880）。演出跑在一阶段常态 AI 下（旧 <c>IsPhase2State(Exchange)</c> 为假）。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.Exchange, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelExchangeState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.Exchange;

        protected override int TimeoutFrames => CrystallineSentinelDirector.AnimStateTimeoutFrames;

        private enum Beat
        {
            /// <summary>外壳崩开</summary>
            Break = 0,
            /// <summary>第 7 帧定格</summary>
            Hold = 1,
            /// <summary>重组</summary>
            Rebuild = 2,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            ctx.DeclareStandStill();
            ctx.Invulnerable = true;

            switch (CurrentBeat)
            {
                case Beat.Hold:
                    SetFrame(ctx, 0, CrystallineSentinelDirector.ExchangeHoldFrame);
                    break;
                case Beat.Rebuild:
                    if (AttackTimer % CrystallineSentinelDirector.ExchangeFrameRate == 0)
                    {
                        AdvanceFrame(ctx, CrystallineSentinelDirector.ExchangeFrameMaxY);
                    }

                    break;
                default:
                    UpdateBreak(ctx);
                    break;
            }
        }

        private void UpdateBreak(CrystallineSentinelContext ctx)
        {
            if (AtFrame(0))
            {
                SetFrame(ctx, 0, 0);
                ctx.SetText(CrystallineSentinelTextType.Broken, CrystallineSentinelDirector.ExchangeBrokenTextFrames);
            }

            if (AttackTimer < CrystallineSentinelDirector.ExchangeGoreFrames)
            {
                SpawnGore(ctx);
            }

            if (ctx.Npc.frame.Y < CrystallineSentinelDirector.ExchangeHoldFrame && AttackTimer % 2 == 0)
            {
                BlastEffect(ctx);
            }

            if (AttackTimer % CrystallineSentinelDirector.ExchangeFrameRate == 0)
            {
                AdvanceFrame(ctx, CrystallineSentinelDirector.ExchangeHoldFrame);
            }
        }

        /// <summary>按帧号把外壳的 16 块碎片崩出去，每块记着自己该在多少帧后飞回来重组。旧 CrystallineSentinel.cs:1823-1838</summary>
        private void SpawnGore(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            int frame = CrystallineGore.TimerToFrame((int)AttackTimer);
            float dir = Utils.Remap(frame, 0, CrystallineSentinelDirector.ExchangeGoreFrames, 0, MathHelper.TwoPi);
            float offset = Main.rand.NextFloat(2, 20);
            float speed = Main.rand.NextFloat(1, 7);

            CrystallineGore gore = PRTLoader.NewParticle<CrystallineGore>(ctx.Npc.Center + (dir.ToRotationVector2() * offset),
                dir.ToRotationVector2() * speed);
            if (gore == null)
            {
                return;
            }

            gore.FollowNPCIndex = ctx.Npc.whoAmI;
            gore.Frame.Y = frame;
            gore.TimeToRebuild = CrystallineSentinelDirector.ExchangeGoreRebuild
                + Main.rand.Next(-CrystallineSentinelDirector.ExchangeGoreRebuildJitter, CrystallineSentinelDirector.ExchangeGoreRebuildJitter);
            gore.Dir = dir;
            gore.Offset = offset;
        }

        /// <summary>碎岩爆点（纯本地）。旧 CrystallineSentinel.cs:1839-1844</summary>
        private static void BlastEffect(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Vector2 offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(CrystallineSentinelDirector.DyingBlastRadiusMin, CrystallineSentinelDirector.DyingBlastRadiusMax);
            CrystallineRockBlast blast = PRTLoader.NewParticle<CrystallineRockBlast>(ctx.Npc.Center + offset, Vector2.Zero);
            if (blast != null)
            {
                blast.Rotation = offset.ToRotation();
            }
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Hold:
                    if (AttackTimer >= CrystallineSentinelDirector.ExchangeHoldFrames)
                    {
                        NextBeat(ctx, (int)Beat.Rebuild);
                    }

                    return null;
                case Beat.Rebuild:
                    // 旧代码的收尾判定：帧图顶到末帧后的下一个帧速余数拍
                    if (AttackTimer % CrystallineSentinelDirector.ExchangeFrameRate == 1
                        && ctx.Npc.frame.Y == CrystallineSentinelDirector.ExchangeFrameMaxY)
                    {
                        return Finish(ctx);
                    }

                    return null;
                default:
                    if (AttackTimer > CrystallineSentinelDirector.ExchangeBreakFrames)
                    {
                        NextBeat(ctx, (int)Beat.Hold);
                    }

                    return null;
            }
        }

        /// <summary>重组完成：重建部位、解除无敌、带一个向上初速进悬浮。旧 CrystallineSentinel.cs:1852-1877</summary>
        private static IVaultState<CrystallineSentinelContext> Finish(CrystallineSentinelContext ctx)
        {
            SetFrame(ctx, 0, 0);
            ctx.ResetPartRig();
            ctx.SetText(CrystallineSentinelTextType.Angry, CrystallineSentinelDirector.ExchangeAngryTextFrames);
            ctx.Npc.velocity = new Vector2(0, CrystallineSentinelDirector.ExchangeLaunchY);

            return CrystallineSentinelHubState.ToState(ctx, CrystallineSentinelStateId.P2Idle, CrystallineSentinelDirector.ExchangeToIdleFrames);
        }
    }
}
