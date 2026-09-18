using Coralite.Content.Bosses.Rediancie.Core;
using Coralite.Core;
using InnoVault.StateMachines;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie.States
{
    /// <summary>
    /// 召唤小赤玉灵（二阶段远程 / 转阶段首招）：悬停在玩家上方，每 40 帧在末位弹药处召唤一只（消耗 1 弹药），
    /// 场上小赤玉灵达上限则收招；200 帧兜底收招。旧 <c>Rediancie.Summon</c>（Rediancie.cs:983-1038）。
    /// </summary>
    [VaultState((int)RediancieStateId.summon, typeof(RediancieContext))]
    internal sealed class RediancieSummonState : RediancieStateBase
    {
        public override RediancieStateId StateIndex => RediancieStateId.summon;

        private bool SummonBeat => Timer % RediancieDirector.SummonInterval == 0;

        protected override void SharedUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            ctx.DeclareHoverAboveTarget();
            ctx.DeclareRotation(RediancieRotationMode.Normal);

            if (ctx.FollowersEmpty)
            {
                return;
            }

            ctx.UpdateFollowersSummon(Timer);

            if (Main.dedServ)
            {
                return;
            }

            if (Timer % RediancieDirector.SummonDustInterval == 0)
            {
                Vector2 anchor = ctx.Followers[^1].center;
                for (int i = 0; i < RediancieDirector.SummonDustCount; i++)
                {
                    Dust dust = Dust.NewDustPerfect(anchor + Main.rand.NextVector2Circular(RediancieDirector.SummonDustSpread, RediancieDirector.SummonDustSpread),
                        DustID.GemRuby, Vector2.Zero, 0, default, RediancieDirector.SummonDustScale);
                    dust.noGravity = true;
                }
            }

            // 召唤成功那一拍的音效；小怪表两端都有（NPC 原版同步），上限判定客户端也能算
            if (SummonBeat && ctx.CanDespawnFollower() && !MinionCapReached())
            {
                SoundEngine.PlaySound(CoraliteSoundID.MagicStaff_Item8, ctx.Npc.Center);
            }
        }

        /// <summary>场上小赤玉灵是否已达难度上限。</summary>
        private static bool MinionCapReached()
        {
            int minionType = ModContent.NPCType<RediancieMinion>();
            return Main.npc.Count((n) => n.active && n.type == minionType) >= RediancieDirector.SummonMinionCap();
        }

        protected override IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            if (ctx.FollowersEmpty)
            {
                return EndAttack(ctx);
            }

            if (SummonBeat)
            {
                if (!ctx.CanDespawnFollower())
                {
                    return EndAttack(ctx);
                }

                if (MinionCapReached())
                {
                    return EndAttack(ctx);
                }

                Vector2 anchor = ctx.Followers[^1].center;
                NPC.NewNPC(ctx.Npc.GetSource_FromThis(), (int)anchor.X, (int)anchor.Y, ModContent.NPCType<RediancieMinion>());

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > RediancieDirector.SummonEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
