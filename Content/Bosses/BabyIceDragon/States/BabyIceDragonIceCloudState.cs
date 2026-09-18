using Coralite.Content.Bosses.BabyIceDragon.Core;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 冰雹云（二阶段）：与冰刺陷阱同一套吼叫演出，60 帧时在目标头顶 300 px 处叫出一朵冰云，之后由云自己往下砸冰锥。<br/>
    /// 云本体伤害只有 1（真正的威胁是它落下的冰锥），所以玩家的应对是横向离开云的投影，而不是硬吃。旧 AI.IceCloud.cs
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.iceCloud, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonIceCloudState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.iceCloud;

        private enum Beat
        {
            /// <summary>靠近到 440 px 内。</summary>
            Approach,
            /// <summary>停住吼叫并叫出冰云。</summary>
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
                Vector2 spawn = ctx.Target.Center
                    + new Vector2(Main.rand.Next(-BabyIceDragonDirector.CloudOffsetX, BabyIceDragonDirector.CloudOffsetX), BabyIceDragonDirector.CloudOffsetY);
                ctx.SpawnHostile<IceyCloud>(ctx.Npc.GetSource_FromAI(), spawn, Vector2.Zero,
                    BabyIceDragonDirector.CloudDamage, BabyIceDragonDirector.CloudKnockback, ctx.Npc.target);
                ctx.MarkDecision();
            }

            if (Timer > BabyIceDragonDirector.RoarAttackEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
