using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 超级钩爪斩：贴到玩家身上左右摆荡，每 15 帧从身侧甩出两道钩爪斩，甩 2~3 轮后拖一段长后摇。<br/>
    /// 节拍结构：淡出 30 帧 → 摆荡甩爪（ShootCount × 15 帧 + 80 帧余波）→ 后摇 135 帧。<br/>
    /// 公平阀：钩爪斩自带 120 帧的伸出时间，左右交替（每 30 帧换一侧）所以两侧轮流开缺口；
    /// 后摇 135 帧是三阶段最长的喘息窗，用来平衡这一手的贴脸压制。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.superHookSlash, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareSuperHookSlashState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>淡出瞬移</summary>
            Fade,
            /// <summary>摆荡并甩爪</summary>
            Slash,
            /// <summary>后摇</summary>
            Recover,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.superHookSlash;

        private Beat CurrentBeat => (Beat)BeatIndex;

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            switch (CurrentBeat)
            {
                case Beat.Fade:
                    FadeBeat(ctx);
                    break;
                case Beat.Slash:
                    SlashBeat(ctx);
                    break;
                default:
                    ctx.DeclareDamp(NightmarePlanteraDirector.HookSlashRecoverDamp);
                    ctx.Boss.DoRotation(0.3f);
                    break;
            }
        }

        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (!FadeTickP3(ctx, () => TeleportPos(ctx), onTeleport: () =>
                {
                    npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
                    ctx.ShootCount = boss.AttackRandom.Next(NightmarePlanteraDirector.HookSlashMinRounds,
                        NightmarePlanteraDirector.HookSlashMaxRounds + 1);
                }))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Slash);
        }

        /// <summary>
        /// 落点。旧代码写的是 <c>(SonState - 1) * MathHelper.Pi.ToRotationVector2() * NextAttackFloat(500, 600)</c>
        /// ——括号位置使得它先把 π 转成单位向量再乘 <c>(SonState - 1)</c>，而 <c>SonState</c> 在这一刻已被置 1，
        /// 于是整个偏移恒为零向量、本体直接落在玩家身上。这是已上线的行为（后面的摆荡会立刻把它拉到 250 px 外），
        /// 照搬以保持手感不变；判断见报告"遗留与建议"。随机抽取必须保留，否则随机流会与客户端错开。
        /// </summary>
        private static Vector2 TeleportPos(NightmarePlanteraContext ctx)
        {
            ctx.Boss.NextAttackFloat(NightmarePlanteraDirector.HookSlashTeleportMin, NightmarePlanteraDirector.HookSlashTeleportMax);
            return ctx.TargetCenter;
        }

        /// <summary>摆荡：半圆锚点 + 正弦摆动，每 15 帧从背离玩家的一侧甩出两道爪。</summary>
        private void SlashBeat(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            HookSlashMovement(ctx, Timer, 0f);

            int volleyEnd = (int)ctx.ShootCount * NightmarePlanteraDirector.HookSlashInterval;

            if (Timer <= volleyEnd && Timer % NightmarePlanteraDirector.HookSlashInterval == 0)
            {
                Vector2 away = (npc.Center - ctx.TargetCenter).SafeNormalize(Vector2.One);
                float baseAngle = (npc.Center - ctx.TargetCenter).ToRotation()
                    + (Timer % NightmarePlanteraDirector.HookSlashAlternatePeriod == 0 ? MathHelper.PiOver4 / 2 : -MathHelper.PiOver4 / 2);

                for (int i = 0; i < NightmarePlanteraDirector.HookSlashPerVolley; i++)
                {
                    npc.NewProjectileInAI_Server<HookSlash>(npc.Center + (away * NightmarePlanteraDirector.HookSlashOffset),
                        Vector2.Zero, NightmarePlanteraDirector.P3BiteDamage(), 0, npc.target,
                        ai0: ctx.Boss.ZenithProjSeed(),
                        ai1: baseAngle + (i * NightmarePlanteraDirector.HookSlashSpreadStep),
                        ai2: NightmarePlanteraDirector.HookSlashAi2);
                }

                ctx.MarkDecision();
            }

            if (Timer < volleyEnd + NightmarePlanteraDirector.HookSlashTail)
            {
                return;
            }

            if (!Main.dedServ)
            {
                Helper.PlayPitched(CoraliteSoundID.EmpressOfLight_Dash_Item160, npc.Center, pitch: -0.7f);
            }

            ctx.Boss.canDrawWarp = true;
            SwitchBeat(ctx, (int)Beat.Recover);
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => CurrentBeat == Beat.Recover && Timer > NightmarePlanteraDirector.HookSlashRecoverFrames
                ? NPNightmareP3State.Commit(ctx)
                : null;
    }
}
