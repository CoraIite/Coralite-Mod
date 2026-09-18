using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 旋转激光（小球侧）：按本体分配的层号错开前摇，三层圈围着本体缓慢反向旋转并依次发射激光，打满两轮回待机。<br/>
    /// 旧 <c>SmallShadowBall.RollingLaser</c>（P1S.RollingLaser.cs:8-93）。<br/>
    /// 层号 / 层内序号 / 本层总数由本体在权威端派活，随小球自己的 <c>SendExtraAI</c> 过线（见 <see cref="SmallShadowBallContext"/>）。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.RollingLaser, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallRollingLaserState : SmallShadowBallStateBase
    {
        /// <summary>旋转激光的子拍。旧 <c>SonState</c> 0~3。</summary>
        private enum Beat
        {
            /// <summary>按层号错开前摇。</summary>
            WaitLayer,
            /// <summary>蓄力，预警线走完就开火。</summary>
            Channel,
            /// <summary>激光发射中。</summary>
            Firing,
            /// <summary>等下一轮。</summary>
            Reload,
        }

        /// <summary>已经打了几轮（旧 <c>Recorder4</c> = <c>localAI[3]</c>）。进热槽 A。</summary>
        private int volleyCount;

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.RollingLaser;

        public override void OnEnter(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            volleyCount = 0;
        }

        public override void WriteHot(SmallShadowBallContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = volleyCount;
        }

        public override void ReadHot(SmallShadowBallContext ctx)
        {
            base.ReadHot(ctx);
            volleyCount = (int)ctx.Hot[CoraliteBossHotSlots.A];
        }

        /// <summary>一轮的总长度 = 激光时长 + 蓄力时长，层与层之间就按这个长度错开。旧 P1S.RollingLaser.cs:27。</summary>
        private static int VolleyFrames
            => SmallShadowBallDirector.RollingLaserTime + ShadowBallDirector.SpikeBallChannelTime();

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            ShellMove(ctx, owner);

            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.WaitLayer:
                    if (Timer > VolleyFrames * ctx.OrbitLayer)
                    {
                        SpawnChannelLine(ctx);
                        SwitchBeat(ctx, (int)Beat.Channel);
                    }

                    break;

                case Beat.Channel:
                    if (Timer > ShadowBallDirector.SpikeBallChannelTime())
                    {
                        SwitchBeat(ctx, (int)Beat.Firing);
                    }

                    break;

                case Beat.Firing:
                    break;

                case Beat.Reload:
                    if (Timer > VolleyFrames * 2)
                    {
                        SpawnChannelLine(ctx);
                        SwitchBeat(ctx, (int)Beat.Channel);
                    }

                    break;
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            switch ((Beat)BeatIndex)
            {
                // Timer == 0 = 本帧 SharedUpdate 刚换到发射拍。
                case Beat.Firing when Timer == 0:
                    ctx.Npc.NewProjectileDirectInAI_Server<SmallLaser>(ctx.Npc.Center, Vector2.Zero,
                        SmallShadowBallDirector.RollingLaserDamage(), 0,
                        ai0: ctx.Npc.whoAmI, ai1: SmallShadowBallDirector.RollingLaserTime);
                    ctx.MarkDecision();
                    return null;

                case Beat.Firing:
                    if (Timer <= SmallShadowBallDirector.RollingLaserTime)
                    {
                        return null;
                    }

                    volleyCount++;
                    if (volleyCount >= SmallShadowBallDirector.RollingVolleyCount)
                    {
                        return EndAttack();
                    }

                    SwitchBeat(ctx, (int)Beat.Reload);
                    return null;

                default:
                    return null;
            }
        }

        /// <summary>
        /// 层内环绕：三层圈按层号取不同半径与转速、奇偶层反向，小球贴着自己那一层的位置走。旧 P1S.RollingLaser.cs:78-92。
        /// </summary>
        private static void ShellMove(SmallShadowBallContext ctx, ShadowBall owner)
        {
            int layer = ctx.OrbitLayer;
            int index = ctx.OrbitIndex;
            int perLayer = ctx.OrbitLayerCount;

            if (perLayer < 1)
            {
                // 派活参数还没过线（中途加入或掉包），这一帧先别动，免得除零把位置打成 NaN。
                return;
            }

            float baseRot = owner.LockTimer
                * (SmallShadowBallDirector.RollingSpinBase + (layer * SmallShadowBallDirector.RollingSpinPerLayer))
                * (layer % 2 == 0 ? 1 : -1);

            Vector2 targetPos = owner.NPC.Center + ctx.Ball.Rotate3D(index / (float)perLayer,
                SmallShadowBallDirector.RollingRadiusBase + (layer * SmallShadowBallDirector.RollingRadiusPerLayer),
                baseRot, 0f, 0f);

            ctx.Npc.Center = Vector2.SmoothStep(ctx.Npc.Center, targetPos, SmallShadowBallDirector.RollingMoveLerp);
            ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp((ctx.Npc.Center - owner.NPC.Center).ToRotation(),
                SmallShadowBallDirector.RollingRotLerp);
        }

        /// <summary>蓄力预警线：前 1/3 生成、后 2/3 保持。旧 P1S.RollingLaser.cs:32-33。</summary>
        private static void SpawnChannelLine(SmallShadowBallContext ctx)
        {
            int time = ShadowBallDirector.SpikeBallChannelTime();
            ctx.Ball.SpawnAimLine(SmallShadowBallDirector.AimLineLength, time / 3, time / 3 * 2);
        }
    }
}
