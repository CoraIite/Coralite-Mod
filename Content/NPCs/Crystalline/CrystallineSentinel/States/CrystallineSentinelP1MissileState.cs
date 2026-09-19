using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 一阶段飞弹：<br/>
    /// Approach：慢慢走最多 2 秒（这一段没有踏台阶，撞到坎就提前结算），站定后摆出发射姿态并喊“[ O ^ O ]”。<br/>
    /// Fire：帧速 5 推满 20 帧，第 35 / 45 / 55 帧各射一发扇形上抛的追踪飞弹——飞弹会自己找玩家，也能被玩家引到本体头上。<br/>
    /// Recover：60 帧后回站立 1 秒。<br/>
    /// 旧 <c>P1Missile</c>（CrystallineSentinel.cs:1056-1150）。血量跌破 75% 时这一招的第一次会被提交口改道成碎岩。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P1Missile, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP1MissileState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P1Missile;

        private enum Beat
        {
            /// <summary>走位（旧 Recorder = 0）</summary>
            Approach = 0,
            /// <summary>发射（旧 Recorder = 1）</summary>
            Fire = 1,
            /// <summary>后摇（旧 Recorder = 2）</summary>
            Recover = 2,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fire:
                    UpdateFire(ctx);
                    break;
                case Beat.Recover:
                    ctx.DeclareStandStill();
                    break;
                default:
                    UpdateApproach(ctx);
                    break;
            }
        }

        private void UpdateApproach(CrystallineSentinelContext ctx)
        {
            if (AttackTimer % CrystallineSentinelDirector.TurnInterval == 0)
            {
                ctx.FaceTarget();
            }

            ctx.DeclareGroundWalk(CrystallineSentinelDirector.MissileApproachMaxSpeed, CrystallineSentinelDirector.MissileApproachAccel,
                zeroOnReverse: true, stepUp: false);
            WalkFrame(ctx);
        }

        private void UpdateFire(CrystallineSentinelContext ctx)
        {
            ctx.DeclareStandStill();

            // 颜文字跟着子拍走（旧代码写在换拍那一句旁边，但换拍现在只发生在权威端，客户端要靠子拍自己补上）
            if (AtFrame(0))
            {
                ctx.SetText(CrystallineSentinelTextType.Fire, CrystallineSentinelDirector.FireTextFrames);
            }

            if (AttackTimer % CrystallineSentinelDirector.TurnInterval == 0)
            {
                ctx.FaceTarget();
            }

            if (AttackTimer > 0 && AttackTimer % CrystallineSentinelDirector.MissileFrameRate == 0)
            {
                AdvanceFrame(ctx, CrystallineSentinelDirector.MissileFrameMaxY);
            }

            for (int i = 0; i < 3; i++)
            {
                if (AtFrame(CrystallineSentinelDirector.MissileFirstCue + (i * CrystallineSentinelDirector.MissileCueStep)))
                {
                    MuzzleEffect(ctx, i);
                }
            }
        }

        /// <summary>出膛表现（起手语音 + 发射音 + 冲击尘），纯本地。旧 CrystallineSentinel.cs:1109-1125</summary>
        private static void MuzzleEffect(CrystallineSentinelContext ctx, int index)
        {
            if (Main.dedServ)
            {
                return;
            }

            NPC npc = ctx.Npc;
            if (index == 0)
            {
                Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "Sentinel_Missile", 0.4f, 0, 0, 2, ctx.HeadPos);
            }

            Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, npc.Center, pitch: 1f);

            Vector2[] poses = [new(2, -72), new(-2, -68), new(-10, -58)];
            Vector2 dustPos = npc.Center + new Vector2(poses[index].X * npc.direction, poses[index].Y - 16);
            Dust d = Dust.NewDustPerfect(dustPos, ModContent.DustType<VinicBigImpact>(), Vector2.Zero, Scale: 0.5f);
            d.rotation = -MathHelper.PiOver2;
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fire:
                    for (int i = 0; i < 3; i++)
                    {
                        if (AtFrame(CrystallineSentinelDirector.MissileFirstCue + (i * CrystallineSentinelDirector.MissileCueStep)))
                        {
                            LaunchMissile(ctx, i);
                        }
                    }

                    if (AttackTimer > CrystallineSentinelDirector.MissileFireFrames)
                    {
                        ctx.FaceTarget();
                        ctx.Npc.velocity.X = 0f;
                        NextBeat(ctx, (int)Beat.Recover);
                        SetFrame(ctx, 0, 0);
                    }

                    return null;
                case Beat.Recover:
                    if (AttackTimer > CrystallineSentinelDirector.MissileRecoverFrames)
                    {
                        return ReturnToIdle(ctx, CrystallineSentinelDirector.MissileRecoverIdleFrames);
                    }

                    return null;
                default:
                    if (AttackTimer > CrystallineSentinelDirector.MissileApproachTimeout || !CanWalkForward(ctx))
                    {
                        ctx.FaceTarget();
                        ctx.Npc.velocity.X = 0f;
                        NextBeat(ctx, (int)Beat.Fire);
                        SetFrame(ctx, 5, 0);
                    }

                    return null;
            }
        }

        /// <summary>三发飞弹按 0.3 / −0.2 / −0.7 rad 上抛。旧 CrystallineSentinel.cs:1106-1120</summary>
        private static void LaunchMissile(CrystallineSentinelContext ctx, int index)
        {
            NPC npc = ctx.Npc;
            Vector2 pos = npc.Center + new Vector2(CrystallineSentinelDirector.MissileMuzzleX * npc.direction, CrystallineSentinelDirector.MissileMuzzleY);
            Vector2 vel = (-Vector2.UnitY * CrystallineSentinelDirector.MissileLaunchSpeed)
                .RotatedBy(CrystallineSentinelDirector.MissileSpreadBase + (index * CrystallineSentinelDirector.MissileSpreadStep));

            NPC missile = NPC.NewNPCDirect(npc.GetSource_FromAI(), pos, ModContent.NPCType<CrystallineSentinelMissile>());
            missile.velocity = vel;
            missile.netUpdate = true;
            ctx.MarkDecision();
        }
    }
}
