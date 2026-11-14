using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Content.Particles;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    [VaultState((int)ShadowBallStates.P1Dizziness, typeof(ShadowBallContext))]
    public sealed class P1Dizziness : ShadowBallStateBase
    {
        public override ShadowBallStates StateIndex => ShadowBallStates.P1Dizziness;

        public override void OnEnter(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);

            for (int i = 0; i < 20; i++)
            {
                float r = Main.rand.NextFloat(MathHelper.TwoPi);
                ctx.SpawnMucuryDust(ctx.Npc.Center + r.ToRotationVector2() * Main.rand.NextFloat(12, 30), r);
            }
        }

        public int DizzyTime => Helper.ScaleValueForDiffMode(60 * 5, 60 * 5, 60 * 4, 60 * 3);

        protected override void SharedUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            ctx.SetVelocitySlowDown(0.95f);

            if (Timer== DizzyTime)
            {
                ctx.Boss.SwitchLockState(ShadowBall.LockStates.Normal);
                return;
            }
            else if (Timer > DizzyTime)
            {
                ctx.Boss.CommonShellBreath(2);

                ctx.Boss.LayerAlpha = Helper.Lerp(ctx.Boss.LayerAlpha, 1f, 0.15f);
                return;
            }

            if (Timer == 1)//眩晕锁环状态，生成眩晕粒子
            {
                ctx.Boss.SwitchLockState(ShadowBall.LockStates.Dizziness);
                for (int i = 0; i < 2; i++)
                    DizzyStar.Spawn(ctx.DizzyStarCenter(), i * MathHelper.TwoPi / 2, DizzyTime, 15, ctx.DizzyStarCenter);
            }

            ctx.SetRotationLetItGo();
            ctx.Boss.ShellBreath_ShowCore(2);
            ctx.Boss.LayerAlpha = Helper.Lerp(ctx.Boss.LayerAlpha, 0.2f, 0.1f);

            //生成水银破碎粒子
            if (Timer % 3 == 0)
            {
                float r = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 dir = r.ToRotationVector2();
                ctx.SpawnMucuryDust(ctx.Npc.Center + dir * Main.rand.NextFloat(12, 30), r);
            }
        }

        protected override IVaultState<ShadowBallContext> AuthorityUpdate(VaultStateMachine<ShadowBallContext> machine, ShadowBallContext ctx)
        {
            if (Timer <= DizzyTime+60)
                return null;

            return ShadowBallHubState.CommitTest(ctx, ShadowBallStates.P1Test);
        }
    }
}
