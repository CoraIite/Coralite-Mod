using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core
{
    /// <summary>
    /// "自旋喷光 → 拉开距离 → 撕开一道裂缝冲过去"这一族两招（转圈咬 / 下方闪光咬）的公共件。<br/>
    /// 两招的差别只在瞬移落点、自旋时的位移、喷光路数与落点方位；冲刺贯穿与后摇引爆完全一致。<br/>
    /// 自旋的触手表演与淡出在 <see cref="NPDreamStateBase"/> 上（蝙蝠与乌鸦也用同一套）。<br/>
    /// 原 <c>RollingThenBite</c> / <c>BelowSparkleThenBite</c>（Phase.P2_Dream.cs:562-906）。
    /// </summary>
    internal abstract class NPDreamSlitStateBase : NPDreamStateBase
    {
        /// <summary>自旋段自己摆触手（要绕本体转 10 圈），外壳不能再摆一次。</summary>
        protected override bool AutoTentacle => false;

        protected enum SlitBeat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>自旋喷光</summary>
            Spin,
            /// <summary>撕裂缝冲刺</summary>
            Dash,
            /// <summary>后摇引爆</summary>
            Recover,
        }

        protected SlitBeat CurrentBeat => (SlitBeat)BeatIndex;

        /// <summary>自旋段末尾的淡出，总长固定 360 帧。</summary>
        protected void SpinFadeOut(NightmarePlanteraContext ctx, Vector2 dustCenter)
            => SpinFadeOut(ctx, NightmarePlanteraDirector.P2RollingFrames, dustCenter);

        /// <summary>落点就位后撕开裂缝：一发短预警咬击 + 一条会跟着本体走的裂缝。</summary>
        protected static void SpawnSlit(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.NewProjectileInAI_Server<NightmareBite>(npc.Center, Vector2.Zero,
                NightmarePlanteraDirector.P2SparkleDamage(), 4,
                ai0: 0, ai1: NightmarePlanteraDirector.P2SlitBiteAi1, ai2: ctx.Boss.ZenithProjSeed());
            npc.NewProjectileInAI_Server<NightmareSlit>(npc.Center, Vector2.Zero, NightmarePlanteraDirector.P2SlitDamage(), 4);
            ctx.MarkDecision();
        }

        /// <summary>冲刺贯穿：前 20 帧全速穿过去，之后 20 帧刹车回正，最后停掉裂缝跟随。</summary>
        protected void DashBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.NormallySetTentacle();

            if (Timer < NightmarePlanteraDirector.P2SlitDashFrames)
            {
                return;
            }

            ctx.DeclareDamp(NightmarePlanteraDirector.P2SlitBrakeDamp);
            ctx.DeclareRotationTowardsTarget(0.04f);

            if (Timer < NightmarePlanteraDirector.P2SlitDashFrames + NightmarePlanteraDirector.P2SlitBrakeFrames)
            {
                return;
            }

            NightmareSlit.StopTracking();
            SwitchBeat(ctx, (int)SlitBeat.Recover);
        }

        /// <summary>后摇：绕圈拉开，第 20 帧把裂缝整条引爆。</summary>
        protected void RecoverBeat(NightmarePlanteraContext ctx)
        {
            ctx.Boss.DoRotation(0.3f);
            CircleMovement(ctx, Timer, NightmarePlanteraDirector.P2SlitCircleDistance, NightmarePlanteraDirector.P2SlitCircleSpeed,
                NightmarePlanteraDirector.P2SlitCircleAccel, NightmarePlanteraDirector.P2SlitCircleRolling);

            if (Timer == NightmarePlanteraDirector.P2SlitExplodeFrame)
            {
                NightmareSlit.Exposion();
            }

            // 收招最后一帧把残速清干净（D2），两端同跑。
            if (Timer > NightmarePlanteraDirector.P2SlitRecoverFrames)
            {
                ctx.Npc.velocity *= 0;
            }

            ctx.Boss.NormallySetTentacle();
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == SlitBeat.Recover && Timer > NightmarePlanteraDirector.P2SlitRecoverFrames
                ? NPDreamP2State.Commit(ctx)
                : null;
    }
}
