using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 照影（小球侧）：聚拢到本体周围，然后错开节拍向玩家连发影子导弹。<br/>
    /// 旧 <c>SmallShadowBall.ShadowShoot</c>（P1S.ShadowShoot.cs:9-46）。<br/><br/>
    /// <b>当前没有下令方</b>：本体那一半（<c>ShadowBall.ShadowShoot</c> 招式体、<c>ShadowBallStateId.ShadowShoot</c>
    /// 与设计文档里「照影」整节）是作者在 51b88bc4 一笔里删掉的，本轮按同一份 git 证据把残留的本体方法也清掉了。
    /// 小球这一侧自 ba5c6df6 起就是独立存在的，招式体完整，所以保留注册 —— 作者想接回去只要在本体加一条下令路径。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.ShadowShoot, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallShadowShootState : SmallShadowBallStateBase
    {
        /// <summary>照影的子拍。旧 <c>SonState</c> 0~1。</summary>
        private enum Beat
        {
            /// <summary>聚拢到本体周围。</summary>
            Gather,
            /// <summary>连发影子导弹。</summary>
            ReleaseMissiles,
        }

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.ShadowShoot;

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            switch ((Beat)BeatIndex)
            {
                default:
                case Beat.Gather:
                    {
                        int count = Math.Max(1, owner.smallBalls.Count);
                        Vector2 targetPosition = owner.NPC.Center
                            + ((ctx.Ball.selfIndex * MathHelper.TwoPi / count).ToRotationVector2()
                                * SmallShadowBallDirector.ShadowShootGatherRadius);

                        MoveToAttackPosition(ctx, targetPosition);
                        if (Timer > SmallShadowBallDirector.ShadowShootMoveFrames)
                        {
                            SwitchBeat(ctx, (int)Beat.ReleaseMissiles);
                        }
                    }

                    break;

                case Beat.ReleaseMissiles:
                    ctx.Npc.velocity *= SmallShadowBallDirector.ShadowShootDamp;
                    break;
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            if ((Beat)BeatIndex != Beat.ReleaseMissiles)
            {
                return null;
            }

            if (Timer > SmallShadowBallDirector.ShadowShootStart
                && Timer % SmallShadowBallDirector.ShadowShootInterval
                    == ctx.Ball.selfIndex % SmallShadowBallDirector.ShadowShootStaggerMod)
            {
                ctx.Npc.NewProjectileDirectInAI_Server<ShadowBallShadowMissile>(ctx.Npc.Center,
                    (owner.Target.Center - ctx.Npc.Center).SafeNormalize(Vector2.Zero) * SmallShadowBallDirector.ShadowShootMissileSpeed,
                    SmallShadowBallDirector.ShadowShootMissileDamage(), 0, ai0: owner.NPC.target);
                ctx.MarkDecision();
            }

            if (Timer > SmallShadowBallDirector.ShadowShootFrames)
            {
                return EndAttack();
            }

            return null;
        }
    }
}
