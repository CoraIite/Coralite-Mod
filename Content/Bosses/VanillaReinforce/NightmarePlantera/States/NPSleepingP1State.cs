using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 一阶段连接段：入场后、脱战重置后落在这里，下一帧就转进待机或转阶段。<br/>
    /// 一阶段的两个选招口也挂在这个类上——<see cref="Commit"/> 是"收招回待机"，<see cref="PickAttack"/> 是"待机满了出招"。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.sleeping_P1, typeof(NightmarePlanteraContext))]
    internal sealed class NPSleepingP1State : NightmarePlanteraStateBase
    {
        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.sleeping_P1;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            ApplyPhase1Presentation(ctx);
        }

        protected override void SharedUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            Phase1Tick(ctx);
            ctx.MeleeDamage = true;
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => Commit(ctx);

        /// <summary>一阶段每帧公共件：补齐三只钩爪、按钩爪重心追玩家、推帧动画。</summary>
        internal static void Phase1Tick(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.EnsureHooks();
            boss.Phase1_Movement();
            boss.UpdateFrameNormally();
        }

        /// <summary>一阶段的常驻表现：紫色触手、不透明、可被打。沿用旧 <c>SetPhase1Idle</c> 的那一串复位。</summary>
        internal static void ApplyPhase1Presentation(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.alpha = 1;
            boss.warpScale = 0;
            boss.canDrawWarp = false;
            boss.useDreamMove = false;
            boss.tentacleColor = NightmarePlantera.lightPurple;
            boss.DreamMoveCount = 0;
            boss.fantasyKillCount = 0;
            boss.tentacleStarFrame = 0;
        }

        /// <summary>
        /// 收招口（旧 <c>SetPhase1Idle</c>）：血量掉到 3/4 以下就直接进转阶段演出，否则回待机。
        /// </summary>
        internal static IVaultState<NightmarePlanteraContext> Commit(NightmarePlanteraContext ctx)
        {
            if (ctx.Npc.life < ctx.Npc.lifeMax * NightmarePlanteraDirector.Phase1EndLifeRatio)
            {
                ctx.Boss.PreparePhase1Exchange();
                return Create(NightmarePlanteraStateId.exchange_P1_P2);
            }

            ApplyPhase1Presentation(ctx);
            return Create(NightmarePlanteraStateId.p1_Idle);
        }

        /// <summary>
        /// 出招口（旧 <c>SetPhase1States</c>）：贴脸时有 1/3 概率放沉眠之雾，否则飞叶 / 触手五五开。<br/>
        /// 只在权威端调，掷骰用 <c>Main.rand</c>——结果随 ai[0] 过线，客户端不需要复现这一掷。
        /// </summary>
        internal static IVaultState<NightmarePlanteraContext> PickAttack(NightmarePlanteraContext ctx)
        {
            if (Vector2.Distance(ctx.Npc.Center, ctx.TargetCenter) < NightmarePlanteraDirector.P1CloseDistance
                && Main.rand.NextBool(NightmarePlanteraDirector.P1FogChance))
            {
                SoundStyle style = CoraliteSoundID.WallOfFlesh_NPCDeath10;
                style.Pitch = -0.5f;
                SoundEngine.PlaySound(style, ctx.Npc.Center);
                return Create(NightmarePlanteraStateId.hypnotizeFog);
            }

            SoundEngine.PlaySound(CoraliteSoundID.MoonLord2_Zombie94, ctx.Npc.Center);
            return Create(Main.rand.Next(0, 2) switch
            {
                0 => NightmarePlanteraStateId.darkLeaves,
                _ => NightmarePlanteraStateId.darkTentacle,
            });
        }
    }
}
