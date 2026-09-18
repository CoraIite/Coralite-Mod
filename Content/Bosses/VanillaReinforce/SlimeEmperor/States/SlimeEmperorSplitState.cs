using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core;
using InnoVault.StateMachines;
using Terraria;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 分裂：落地 → 压扁 → 回弹时把分身甩出来 → 复原 → 一次喘息小跳。<br/>
    /// 节拍：Ground → Squash → Rebound（分裂）→ Restore → Jump → Done。<br/>
    /// 压扁与回弹两拍全程撒凝胶飞沫，插值只有 0.1（比尖刺球慢一倍），飞沫量就是“要分裂了”的预告；
    /// 挑战 <c>SpeedBonus3_1</c> 下省掉收尾那一跳。分身随后会被聚合射击吞掉换成弹幕量，两招是一对。<br/>
    /// 旧 <c>SlimeEmperor.Split</c>（AI.Split.cs:12-53）。
    /// </summary>
    [VaultState((int)AIStates.Split, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorSplitState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.Split;

        private enum Beat
        {
            /// <summary>先落地（旧 SonState 0）</summary>
            Ground = 0,
            /// <summary>压扁（旧 1）</summary>
            Squash = 1,
            /// <summary>回弹，到位即分裂（旧 2）</summary>
            Rebound = 2,
            /// <summary>复原（旧 3）</summary>
            Restore = 3,
            /// <summary>喘息小跳，挑战下跳过（旧 4）</summary>
            Jump = 4,
            /// <summary>收招（旧 5）</summary>
            Done = 5,
        }

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Ground:
                    if (ctx.UpdateJump(SlimeEmperorDirector.SplitJumpY, SlimeEmperorDirector.SplitJumpX) == SlimeJumpEvent.Landed)
                    {
                        EnterBeat(ctx, (int)Beat.Squash);
                    }

                    break;

                case Beat.Squash:
                    //旧代码这一拍的飞沫在插值之后撒，下一拍在插值之前，照抄
                    ScaleBeat(ctx, SlimeEmperorDirector.SplitSquashX, SlimeEmperorDirector.SplitSquashY, SlimeEmperorDirector.SplitScaleLerp,
                        ctx.Scale.X > SlimeEmperorDirector.SplitSquashDone, (int)Beat.Rebound);
                    SpawnSplitGelDust(ctx);
                    break;

                case Beat.Rebound:
                    SpawnSplitGelDust(ctx);
                    if (ScaleBeat(ctx, SlimeEmperorDirector.SpikeStretchX, SlimeEmperorDirector.SpikeStretchY, SlimeEmperorDirector.SplitScaleLerp,
                        ctx.Scale.Y > SlimeEmperorDirector.SpikeStretchDone, (int)Beat.Restore))
                    {
                        PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, ctx);
                    }

                    break;

                case Beat.Restore:
                    ScaleBeat(ctx, 1f, 1f, SlimeEmperorDirector.SplitScaleLerp, ctx.ScaleRestored(true), (int)Beat.Jump);
                    break;

                case Beat.Jump:
                    //挑战下这一跳整个不跑，换态由 AuthorityUpdate 裁决
                    if (SkipLastJump(ctx))
                    {
                        break;
                    }

                    if (ctx.UpdateJump(SlimeEmperorDirector.SplitJumpY, SlimeEmperorDirector.SplitJumpX) == SlimeJumpEvent.Landed)
                    {
                        EnterBeat(ctx, (int)Beat.Done);
                    }

                    break;
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (EnteredBeat == (int)Beat.Restore)
            {
                int count = SlimeEmperorDirector.SplitAvatarCount();
                for (int i = 0; i < count; i++)
                {
                    NPC.NewNPC(ctx.Npc.GetSource_FromAI(), (int)ctx.Npc.Center.X, (int)ctx.Npc.Center.Y,
                        ModContent.NPCType<SlimeAvatar>(), Target: ctx.Npc.target, ai1: ctx.Npc.whoAmI);
                }
            }

            if (BeatIndex == (int)Beat.Jump && SkipLastJump(ctx))
            {
                return EndAttack(ctx);
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }

        /// <summary>挑战 SpeedBonus3_1：分裂完直接收招。沿用旧值 AI.Split.cs:41-45</summary>
        private static bool SkipLastJump(SlimeEmperorContext ctx)
            => ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus3_1);
    }
}
