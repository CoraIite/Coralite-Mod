using InnoVault;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>小影子球顶层 FSM 状态 ID，与旧 <see cref="SmallShadowBall.AIStates"/> 数值一致，占用 <c>npc.ai[1]</c> 同步。</summary>
    public enum SmallShadowBallStateId
    {
        OnKillAnmi = 1,
        Idle = 2,
        RollingLaser = 3,
        ConvergeLaser = 4,
        LaserWithBeam_Laser = 5,
        LaserWithBeam_Beam = 6,
        LeftRightLaser = 7,
        RollingShadowPlayer = 8,
        RandomLaser = 9,
    }

    /// <summary>小影子球专用状态机，状态 ID 走 <c>ai[1]</c>（<c>ai[0]</c> 保留主人索引）。</summary>
    public sealed class SmallShadowBallStateMachine : NpcStateMachine<SmallShadowBallContext>
    {
        public SmallShadowBallStateMachine(SmallShadowBallContext context)
            : base(context, SmallShadowBallContext.StateAiSlot)
        {
        }
    }

    /// <summary>
    /// 小影子球状态基类：SharedUpdate / ServerUpdate 拆分，与 CoraliteBossState 同理念但不占用 ai[0]。
    /// </summary>
    public abstract class SmallShadowBallBossState : VaultState<SmallShadowBallContext>
    {
        protected SmallShadowBall Ball { get; set; }

        public sealed override IVaultState<SmallShadowBallContext> OnUpdate(
            VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            SharedUpdate(machine, ctx);

            if (VaultUtils.isClient)
            {
                return null;
            }

            return ServerUpdate(machine, ctx);
        }

        public override void OnEnter(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            Ball = ctx.Ball;
            ctx.ResetAttackLocals();
            Ball.RefreshAttackRandom();
        }

        protected virtual void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            Ball = ctx.Ball;
        }

        protected virtual IVaultState<SmallShadowBallContext> ServerUpdate(
            VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
            => null;
    }

    /// <summary>招式包壳态：双端跑确定性运动/视觉，生成已在方法内服务端守卫。</summary>
    public abstract class SmallShadowBallAttackWrapperState : SmallShadowBallBossState
    {
        protected abstract void RunAttack(SmallShadowBall ball, NPC owner);

        protected virtual bool IncrementTimer => true;

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            if (!ctx.Ball.GetOwner(out NPC owner))
            {
                return;
            }

            RunAttack(ctx.Ball, owner);
            if (IncrementTimer)
            {
                ctx.Ball.Timer++;
            }
        }
    }

    [VaultState((int)SmallShadowBallStateId.Idle, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallIdleState : SmallShadowBallBossState
    {
        public override void OnEnter(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            // Idle 由主球编排写入 Timer/Recorder，不在此清零或 roll 种子。
            Ball = ctx.Ball;
        }

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            Ball = ctx.Ball;
            Ball.NPC.velocity *= 0.9f;
            Ball.NPC.rotation += 0.05f;

            if (Ball.Timer <= 0)
            {
                return;
            }

            Ball.Timer--;
        }

        protected override IVaultState<SmallShadowBallContext> ServerUpdate(
            VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.ServerUpdate(machine, ctx);
            if (Ball.Timer > 0)
            {
                return null;
            }

            var next = (SmallShadowBall.AIStates)(int)Ball.Recorder;
            return VaultStateRegistry<SmallShadowBallContext>.Create(SmallShadowBall.AIStatesToStateId(next));
        }
    }

    [VaultState((int)SmallShadowBallStateId.RollingLaser, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallRollingLaserState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.RollingLaser(owner);
    }

    [VaultState((int)SmallShadowBallStateId.ConvergeLaser, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallConvergeLaserState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.ConvergeLaser(owner);
    }

    [VaultState((int)SmallShadowBallStateId.LaserWithBeam_Laser, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallLaserWithBeamLaserState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.LaserWithBeam_Laser(owner);
    }

    [VaultState((int)SmallShadowBallStateId.LaserWithBeam_Beam, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallLaserWithBeamBeamState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.LaserWithBeam_Beam(owner);
    }

    [VaultState((int)SmallShadowBallStateId.LeftRightLaser, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallLeftRightLaserState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.LeftRightLaser(owner);
    }

    [VaultState((int)SmallShadowBallStateId.RollingShadowPlayer, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallRollingShadowPlayerState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.RollingShadowPlayer(owner);
    }

    [VaultState((int)SmallShadowBallStateId.RandomLaser, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallRandomLaserState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.RandomLaser(owner);
    }
}
