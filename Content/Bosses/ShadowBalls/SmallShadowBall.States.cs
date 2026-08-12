using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>小影子球顶层 FSM 状态 ID，与旧 <see cref="SmallShadowBall.AIStates"/> 数值一致，占用 <c>npc.ai[1]</c> 同步。</summary>
    public enum SmallShadowBallStateId
    {
        OnSpawnAnim,
        OnKillAnmi,
        Idle,

        /// <summary> 一阶段招式：影之公转 </summary>
        Revolution,
        /// <summary> 一阶段招式：星轨 </summary>
        Starline,
        /// <summary> 一阶段招式：月食 </summary>
        LunarEclipse,
        /// <summary> 一阶段招式：小球到场地左右两边射激光 </summary>
        //LeftRightLaser,
        /// <summary> 一阶段招式：照影 </summary>
        ShadowShoot,
        /// <summary> 一阶段招式：影刺 </summary>
        ShadowSpike,
        ///// <summary> 一阶段招式：依次射激光 </summary>
        //RandomLaser_Master,
        /// <summary> 一阶段特殊招式：黑暗窥视 </summary>
        DarkSeek,
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
            //Ball.RefreshAttackRandom();
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
            if (!ctx.Ball.OwnerIndex.GetNPCOwner<ShadowBall>(out NPC owner))
            {
                return;
            }

            RunAttack(ctx.Ball, owner);
            if (IncrementTimer)
                ctx.Ball.Timer++;
        }
    }

    [VaultState((int)SmallShadowBallStateId.Idle, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallIdleState : SmallShadowBallBossState
    {
        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.SharedUpdate(machine, ctx);
            if (!ctx.Ball.OwnerIndex.GetNPCOwner<ShadowBall>(out NPC owner))
            {
                return;
            }

            ctx.Ball.Idle(owner);
            ctx.Ball.Timer++;
        }

        //public override void OnEnter(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        //{
        //    // Idle 由主球编排写入 Timer/Recorder，不在此清零或 roll 种子。
        //    Ball = ctx.Ball;
        //}

        //protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        //{
        //    base.SharedUpdate(machine, ctx);
        //    Ball = ctx.Ball;
        //    Ball.NPC.velocity *= 0.9f;
        //    Ball.NPC.rotation += 0.05f;

        //    if (Ball.Timer <= 0)
        //    {
        //        return;
        //    }

        //    Ball.Timer--;
        //}

        //protected override IVaultState<SmallShadowBallContext> ServerUpdate(
        //    VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        //{
        //    base.ServerUpdate(machine, ctx);
        //    if (Ball.Timer > 0)
        //    {
        //        return null;
        //    }

        //    var next = (SmallShadowBall.AIStates)(int)Ball.Recorder;
        //    return VaultStateRegistry<SmallShadowBallContext>.Create(SmallShadowBall.AIStatesToStateId(next));
        //}
    }

    [VaultState((int)SmallShadowBallStateId.OnSpawnAnim, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallOnSpawnAnimState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.OnSpawnAnmi(owner);
    }

    [VaultState((int)SmallShadowBallStateId.Revolution, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallRevolutionState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.Revolution(owner);
    }

    [VaultState((int)SmallShadowBallStateId.ShadowSpike, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallShadowSpikeState : SmallShadowBallAttackWrapperState
    {
        protected override void RunAttack(SmallShadowBall ball, NPC owner) => ball.ShadowSpike(owner);
    }

}
