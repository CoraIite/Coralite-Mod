using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 二阶段待机：半透明地绕着玩家转圈、头随机小幅摆动、本身无敌。<br/>
    /// 这是轮换表末位的"喘息"槽，时长由选招口写进 <c>ShootCount</c>（默认 300 帧），也是玩家收拾场上残留弹幕的窗口。<br/>
    /// 原 <c>P2_Idle</c> / <c>P2_Idle_Tick</c>（Phase.P2_Dream.cs:1902-1927）与 BT 序列 <c>BuildP2IdleTree</c>。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.p2_Idle, typeof(NightmarePlanteraContext))]
    internal sealed class NPDreamIdleState : NPDreamStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.p2_Idle;

        protected override void Phase2Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (boss.alpha > NightmarePlanteraDirector.P2IdleAlphaFloor)
            {
                boss.alpha -= NightmarePlanteraDirector.P2IdleAlphaStep;
            }

            ctx.Invulnerable = true;

            float currentRot = ctx.ShootCount + (Timer / NightmarePlanteraDirector.P2DreamOrbitPeriod * MathHelper.TwoPi);
            ctx.DeclareApproach(ctx.TargetCenter + (currentRot.ToRotationVector2() * NightmarePlanteraDirector.P2DreamOrbitRadius),
                NightmarePlanteraDirector.P2DreamOrbitTurn, NightmarePlanteraDirector.P2DreamOrbitMaxSpeed,
                NightmarePlanteraDirector.P2DreamOrbitBlend, NightmarePlanteraDirector.P2DreamOrbitSpeedRange);

            // 抽取走同步种子，两端都要跑（C3）。
            npc.rotation += boss.NextAttackFloat(NightmarePlanteraDirector.P2IdleWobbleMin, NightmarePlanteraDirector.P2IdleWobbleMax);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => Timer > ctx.ShootCount ? NPDreamP2State.Commit(ctx) : null;
    }
}
