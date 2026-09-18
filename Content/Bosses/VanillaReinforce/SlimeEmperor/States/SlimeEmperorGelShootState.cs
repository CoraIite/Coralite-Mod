using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.Core;
using Coralite.Core;
using InnoVault.StateMachines;
using System;
using Terraria;
using AIStates = Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.SlimeEmperor.AIStates;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor.States
{
    /// <summary>
    /// 凝胶射击：两次 (2, 6) 的小跳，每次腾空段结束时重锁目标并打一轮弹弹凝胶球。<br/>
    /// 一轮 = 背身一侧的斜射扇（第 i 发偏 1 + 0.3i 弧度，速度 12）+ 少一发的上抛（速度 16，散角 ±0.5），
    /// 斜射打地面、上抛封天花板，两边都留着中间的缝。<br/>
    /// 节拍：Volley1 → Volley2 → Done；出手在腾空结束那一帧，蓄力帧图即预告。<br/>
    /// 旧 <c>SlimeEmperor.GelShoot</c>（AI.GelShoot.cs:12-52）。
    /// </summary>
    [VaultState((int)AIStates.GelShoot, typeof(SlimeEmperorContext))]
    internal sealed class SlimeEmperorGelShootState : SlimeEmperorStateBase
    {
        public override AIStates StateIndex => AIStates.GelShoot;

        private enum Beat
        {
            /// <summary>第一轮（旧 SonState 0）</summary>
            Volley1 = 0,
            /// <summary>第二轮（旧 1）</summary>
            Volley2 = 1,
            /// <summary>收招（旧 2）</summary>
            Done = SlimeEmperorDirector.GelShootVolleys,
        }

        /// <summary>本帧腾空结束、该出手了（<see cref="AuthorityUpdate"/> 同帧消费）。</summary>
        private bool volleyThisFrame;

        protected override void SharedUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            volleyThisFrame = false;
            if (BeatIndex >= (int)Beat.Done)
            {
                return;
            }

            if (ctx.UpdateJump(SlimeEmperorDirector.GelShootJumpY, SlimeEmperorDirector.GelShootJumpX) != SlimeJumpEvent.JumpFinished)
            {
                return;
            }

            EnterBeat(ctx, BeatIndex + 1);
            ctx.Npc.TargetClosest();
            volleyThisFrame = true;
            PlaySound(CoraliteSoundID.QueenSlime2_Bubble_Item155, ctx);
        }

        protected override IVaultState<SlimeEmperorContext> AuthorityUpdate(VaultStateMachine<SlimeEmperorContext> machine, SlimeEmperorContext ctx)
        {
            if (volleyThisFrame)
            {
                volleyThisFrame = false;
                Shoot(ctx);
            }

            return ReadyToFinish((int)Beat.Done) ? EndAttack(ctx) : null;
        }

        private static void Shoot(SlimeEmperorContext ctx)
        {
            NPC npc = ctx.Npc;
            int sign = Math.Sign(npc.Center.X - ctx.Target.Center.X);
            int count = SlimeEmperorDirector.GelShootCount();
            int damage = SlimeEmperorDirector.GelShootDamage();

            //斜射扇：以连线为基准朝背身一侧张开
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.UnitY)
                    .RotatedBy(sign * (SlimeEmperorDirector.GelShootAngleBase + (i * SlimeEmperorDirector.GelShootAngleStep)))
                    * SlimeEmperorDirector.GelShootSpeed;
                ctx.SpawnHostile<GelBall>(npc.GetSource_FromAI(), ProjSpawnPos(ctx), velocity, damage, SlimeEmperorDirector.GelShootKnockback, npc.target);
            }

            //上抛：比斜射少一发，至少一发
            int upCount = count > 1 ? count - 1 : 1;
            ShootUpward<GelBall>(ctx, upCount, damage, SlimeEmperorDirector.GelShootUpSpeed,
                SlimeEmperorDirector.GelShootUpSpread, SlimeEmperorDirector.GelShootKnockback);
        }
    }
}
