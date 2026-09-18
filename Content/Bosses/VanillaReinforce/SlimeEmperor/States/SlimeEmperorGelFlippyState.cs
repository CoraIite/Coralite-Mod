using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Content.CoraliteNotes.SlimeChapter1;
using Coralite.Core;
using InnoVault.StateMachines;
using Terraria;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 凝胶僚机：落地 → 压扁 → 回弹时吐出飞翼史莱姆 → 两次小跳把场面交还给玩家。<br/>
    /// 节拍：Ground → Flatten → Rebound（召唤）→ Jump1 → Jump2 → Done。<br/>
    /// 召唤后那两跳不是招式而是<b>声明出来的喘息</b>：本体只是在原地蹦，玩家有时间去处理刚出场的僚机；
    /// 挑战 <c>SpeedBonus3_1</c> 下直接跳过第二跳收招，喘息被砍掉一半。<br/>
    /// 旧 <c>SlimeEmperor.GelFlippy</c>（AI.GelFlippy.cs:11-51）。
    /// </summary>
    [VaultState((int)AIStates.GelFlippy, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorGelFlippyState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.GelFlippy;

        private enum Beat
        {
            /// <summary>先落地（旧 SonState 0）</summary>
            Ground = 0,
            /// <summary>压扁（旧 1）</summary>
            Flatten = 1,
            /// <summary>回弹，到位即召唤（旧 2）</summary>
            Rebound = 2,
            /// <summary>喘息小跳（旧 3）</summary>
            Jump1 = 3,
            /// <summary>喘息高跳，挑战下跳过（旧 4）</summary>
            Jump2 = 4,
            /// <summary>收招（旧 5）</summary>
            Done = 5,
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
                    if (ScaleBeat(ctx, SlimeEmperorDirector.SpikeStretchX, SlimeEmperorDirector.SpikeStretchY, SlimeEmperorDirector.SpikeScaleLerp,
                        ctx.Scale.Y > SlimeEmperorDirector.SpikeStretchDone, (int)Beat.Jump1))
                    {
                        PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, ctx);
                    }

                    break;

                case Beat.Jump1:
                    if (ctx.UpdateJump(SlimeEmperorDirector.FlippyJump1Y, SlimeEmperorDirector.FlippyJumpX) == SlimeJumpEvent.JumpFinished)
                    {
                        EnterBeat(ctx, (int)Beat.Jump2);
                    }

                    break;

                case Beat.Jump2:
                    //挑战下这一跳整个不跑，换态由 AuthorityUpdate 裁决
                    if (SkipSecondJump(ctx))
                    {
                        break;
                    }

                    if (ctx.UpdateJump(SlimeEmperorDirector.FlippyJump2Y, SlimeEmperorDirector.FlippyJumpX) == SlimeJumpEvent.JumpFinished)
                    {
                        EnterBeat(ctx, (int)Beat.Done);
                    }

                    break;
            }
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (EnteredBeat == (int)Beat.Jump1)
            {
                int count = SlimeEmperorDirector.FlippyCount();
                for (int i = 0; i < count; i++)
                {
                    NPC.NewNPC(ctx.Npc.GetSource_FromAI(), (int)ctx.Npc.Center.X, (int)ctx.Npc.Center.Y,
                        ModContent.NPCType<GelFlippy>(), Target: ctx.Npc.target);
                }
            }

            if (BeatIndex == (int)Beat.Jump2 && SkipSecondJump(ctx))
            {
                return EndAttack(ctx);
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }

        /// <summary>挑战 SpeedBonus3_1：召唤完直接收招，不给第二次喘息跳。沿用旧值 AI.GelFlippy.cs:39-43</summary>
        private static bool SkipSecondJump(SlimeEmperorContext ctx)
            => ctx.Dangerous(Slime1Knowledge.Dangerous.SpeedBonus3_1);
    }
}
