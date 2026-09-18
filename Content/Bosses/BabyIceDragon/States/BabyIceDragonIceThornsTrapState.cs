using Coralite.Content.Bosses.BabyIceDragon.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 冰刺陷阱（二阶段）：靠近目标 → 合翅停住 → 50 帧吼叫（吼声与吼叫波是预告）→ 60 帧在目标周围一圈随机落 4/5/6 个冰刺陷阱 NPC → 120 帧收招。<br/>
    /// 陷阱环半径 240~350 px，留出中间的安全区与外圈的逃生道；boss 自己在吼叫期间不动，是玩家的输出窗。旧 AI.IceThronsTrap.cs
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.iceThornsTrap, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonIceThornsTrapState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.iceThornsTrap;

        private enum Beat
        {
            /// <summary>靠近到 440 px 内。</summary>
            Approach,
            /// <summary>停住吼叫并布置陷阱。</summary>
            Roar,
        }

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if ((Beat)BeatIndex == Beat.Approach)
            {
                if (RoarAttackApproach(ctx))
                {
                    return;
                }

                ctx.DeclareKeep();
                ChangeBeat(ctx, (int)Beat.Roar);
                return;
            }

            RoarAttackShared(ctx);
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if ((Beat)BeatIndex == Beat.Approach)
            {
                return Timer > BabyIceDragonDirector.RoarAttackApproachTimeout ? EndAttack(ctx) : null;
            }

            if (Timer == BabyIceDragonDirector.RoarAttackSpawnFrame)
            {
                SpawnTraps(ctx);
            }

            if (Timer > BabyIceDragonDirector.RoarAttackEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>在目标周围均分一圈、每个半径随机的冰刺陷阱。旧 AI.IceThronsTrap.cs:77-95</summary>
        private static void SpawnTraps(BabyIceDragonContext ctx)
        {
            int howMany = BabyIceDragonDirector.ThornsTrapCount();
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < howMany; i++)
            {
                int randomWidth = Main.rand.Next(BabyIceDragonDirector.ThornsTrapRadiusMin, BabyIceDragonDirector.ThornsTrapRadiusMax);
                NPC.NewNPCDirect(ctx.Npc.GetSource_FromAI(), ctx.Target.Center + (rot.ToRotationVector2() * randomWidth), ModContent.NPCType<IceThornsTrap>());
                rot += MathHelper.TwoPi / howMany;
            }

            ctx.MarkDecision();
        }
    }
}
