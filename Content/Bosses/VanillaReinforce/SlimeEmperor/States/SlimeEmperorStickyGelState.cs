using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using InnoVault.StateMachines;
using Terraria;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 黏黏凝胶：落地 → 慢速压扁 → 回弹时甩出一把黏胶 → 慢速复原。<br/>
    /// 与尖刺凝胶球同一副骨架，差别是压扁 / 复原用 0.05 的慢插值（预告窗明显更长），
    /// 出手改在<b>回弹</b>那一拍，且以连线<b>反向</b>为基准散 ±2 弧度——弹幕是甩出去的一片，不是对着脸打。<br/>
    /// 节拍：Ground → Flatten → Rebound → Restore → Done。<br/>
    /// 旧 <c>SlimeEmperor.StickyGel</c>（AI.StickyGel.cs:9-55）。
    /// </summary>
    [VaultState((int)AIStates.StickyGel, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorStickyGelState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.StickyGel;

        private enum Beat
        {
            /// <summary>先落地（旧 SonState 0）</summary>
            Ground = 0,
            /// <summary>慢速压扁（旧 1）</summary>
            Flatten = 1,
            /// <summary>回弹，到位即出手（旧 2）</summary>
            Rebound = 2,
            /// <summary>慢速复原（旧 3）</summary>
            Restore = 3,
            /// <summary>收招（旧 4）</summary>
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
                    ScaleBeat(ctx, SlimeEmperorDirector.SpikeFlatX, SlimeEmperorDirector.SpikeFlatY, SlimeEmperorDirector.StickySlowLerp,
                        ctx.Scale.X > SlimeEmperorDirector.SpikeFlatDone, (int)Beat.Rebound);
                    break;

                case Beat.Rebound:
                    ScaleBeat(ctx, SlimeEmperorDirector.SpikeStretchX, SlimeEmperorDirector.SpikeStretchY, SlimeEmperorDirector.SpikeScaleLerp,
                        ctx.Scale.Y > SlimeEmperorDirector.SpikeStretchDone, (int)Beat.Restore);
                    break;

                case Beat.Restore:
                    if (ScaleBeat(ctx, 1f, 1f, SlimeEmperorDirector.StickySlowLerp, ctx.ScaleRestored(), (int)Beat.Done))
                    {
                        ctx.Scale = Vector2.One;
                    }

                    break;
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (EnteredBeat == (int)Beat.Restore)
            {
                Shoot(ctx);
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }

        private static void Shoot(SlimeEmperorContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 targetDir = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero);
            int count = SlimeEmperorDirector.StickyCount();
            int damage = SlimeEmperorDirector.StickyDamage();

            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = -targetDir.RotatedBy(Main.rand.NextFloat(-SlimeEmperorDirector.StickySpread, SlimeEmperorDirector.StickySpread))
                    * SlimeEmperorDirector.StickySpeed;
                ctx.SpawnHostile<StickyGel>(npc.GetSource_FromAI(), ProjSpawnPos(ctx), velocity, damage, SlimeEmperorDirector.StickyKnockback, npc.target);
            }
        }
    }
}
