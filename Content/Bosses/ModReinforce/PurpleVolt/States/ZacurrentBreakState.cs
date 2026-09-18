using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 紫伏击穿后的恢复期：首帧炸开一圈红电 + 头顶挂两颗转圈星星，之后一路刹停回正，
    /// 到时间收招——这一段就是给玩家的输出窗口，难度越高越短。<br/>
    /// 收招时权威端把紫伏旗关掉，回 hub 重新选招。旧 <c>ZacurrentDragon.BreakState</c>（ZcurrentAI.cs:281-314）
    /// 与旧壳 ZacurrentDragon.States.cs:127-147。
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.Break, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentBreakState : ZacurrentAttackState
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.Break;

        protected override bool RunAttack(ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;
            int time = ZacurrentDirector.BreakFrames();

            if (ctx.Timer == 0)
            {
                if (!VaultUtils.isServer)
                {
                    Vector2 anchor() => boss.GetMousePos() + new Vector2(0, ZacurrentDirector.BreakStarAnchorUp);
                    DizzyStar.Spawn(npc.Center, -ZacurrentDirector.BreakStarRotation, time, ZacurrentDirector.BreakStarLength, anchor);
                    DizzyStar.Spawn(npc.Center, ZacurrentDirector.BreakStarRotation, time, ZacurrentDirector.BreakStarLength, anchor);

                    Helper.PlayPitched(CoraliteSoundID.NoUse_ElectricMagic_Item122, npc.Center);
                    ZacurrentDragon.BurstRing(npc.Center);
                    SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, npc.Center);
                }
            }

            npc.velocity *= ZacurrentDirector.IdleDamp;
            ctx.DeclareDirect();
            boss.FlyingFrame();
            boss.TurnToNoRot();

            ctx.Timer++;
            return ctx.Timer > time;
        }

        protected override IVaultState<ZacurrentDragonContext> AuthorityUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            if (!ctx.AttackFinished)
            {
                return null;
            }

            // 恢复期结束才真正退出紫伏形态（击穿那一刻 ModifyIncomingHit 已把计数清零）
            ctx.Boss.PurpleVolt = false;
            ctx.MarkDecision();
            return EndAttack(ctx);
        }
    }
}
