using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using InnoVault.StateMachines;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 尖刺凝胶球：落地 → 压扁 → 向上喷一束尖刺球 → 回弹 → 复原。<br/>
    /// 节拍：Ground → Flatten → Rebound → Restore → Done。压扁那一拍就是预告（身体越扁越接近出手），
    /// 出手在 Flatten 结束的同一帧，弹幕全部朝上（散角 ±0.3），贴脸站着才会吃满，拉开就是可躲的。<br/>
    /// 旧 <c>SlimeEmperor.SpikeGelBall</c>（AI.SpikeGelBall.cs:9-50）。
    /// </summary>
    [VaultState((int)AIStates.SpikeGelBall, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorSpikeGelBallState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.SpikeGelBall;

        private enum Beat
        {
            /// <summary>先落地（旧 SonState 0）</summary>
            Ground = 0,
            /// <summary>压扁，到位即出手（旧 1）</summary>
            Flatten = 1,
            /// <summary>回弹（旧 2）</summary>
            Rebound = 2,
            /// <summary>复原（旧 3）</summary>
            Restore = 3,
            /// <summary>收招（旧 default）</summary>
            Done = 4,
        }

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ground:
                    if (ctx.UpdateJump(SlimeEmperorDirector.GroundJumpY, SlimeEmperorDirector.GroundJumpX) == SlimeJumpEvent.Landed)
                    {
                        EnterBeat(ctx, (int)Beat.Flatten);
                    }

                    break;

                case Beat.Flatten:
                    ScaleBeat(ctx, SlimeEmperorDirector.SpikeFlatX, SlimeEmperorDirector.SpikeFlatY, SlimeEmperorDirector.SpikeScaleLerp,
                        ctx.Scale.X > SlimeEmperorDirector.SpikeFlatDone, (int)Beat.Rebound);
                    break;

                case Beat.Rebound:
                    ScaleBeat(ctx, SlimeEmperorDirector.SpikeStretchX, SlimeEmperorDirector.SpikeStretchY, SlimeEmperorDirector.SpikeScaleLerp,
                        ctx.Scale.Y > SlimeEmperorDirector.SpikeStretchDone, (int)Beat.Restore);
                    break;

                case Beat.Restore:
                    if (ScaleBeat(ctx, 1f, 1f, SlimeEmperorDirector.SpikeScaleLerp, ctx.ScaleRestored(), (int)Beat.Done))
                    {
                        ctx.Scale = Vector2.One;
                    }

                    break;
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (EnteredBeat == (int)Beat.Rebound)
            {
                ShootUpward<SpikeGelBall>(ctx, SlimeEmperorDirector.SpikeCount(), SlimeEmperorDirector.SpikeDamage(),
                    SlimeEmperorDirector.SpikeSpeed, SlimeEmperorDirector.SpikeSpread, SlimeEmperorDirector.SpikeKnockback);
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }
    }
}
