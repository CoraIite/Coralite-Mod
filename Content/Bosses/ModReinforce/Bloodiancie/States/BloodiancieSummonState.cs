using Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core;
using Coralite.Core;
using InnoVault.StateMachines;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.States
{
    /// <summary>
    /// 召唤小血玉灵（二阶段远程 / 转阶段首招）：悬停在玩家上方，每 40 帧在末位弹药处召唤一只（消耗 1 弹药），
    /// 场上小血玉灵达难度上限则收招；200 帧兜底收招。旧 <c>Bloodiancie.Summon</c>（AI.cs:740-792）。
    /// </summary>
    [VaultState((int)BloodiancieStateId.summon, typeof(BloodiancieContext))]
    internal sealed class BloodiancieSummonState : BloodiancieStateBase
    {
        public override BloodiancieStateId StateIndex => BloodiancieStateId.summon;

        /// <summary>召唤拍：每 40 帧（两端同算）。</summary>
        private bool SummonBeat => Timer % BloodiancieDirector.SummonInterval == 0;

        protected override void SharedUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            ctx.DeclareHoverAboveTarget();
            ctx.DeclareRotation(BloodiancieRotationMode.Normal);

            if (ctx.FollowersEmpty)
            {
                return;
            }

            ctx.UpdateFollowersSummon(Timer);

            if (Main.dedServ)
            {
                return;
            }

            if (Timer % BloodiancieDirector.SummonDustInterval == 0)
            {
                Vector2 anchor = ctx.Followers[^1].center;
                for (int i = 0; i < BloodiancieDirector.SummonDustCount; i++)
                {
                    Dust dust = Dust.NewDustPerfect(anchor + Main.rand.NextVector2Circular(BloodiancieDirector.SummonDustSpread, BloodiancieDirector.SummonDustSpread),
                        DustID.GemRuby, Vector2.Zero, 0, default, BloodiancieDirector.SummonDustScale);
                    dust.noGravity = true;
                }
            }

            // 召唤成功那一拍的音效；小怪表两端都有（NPC 原版同步），上限判定客户端也能算
            if (SummonBeat && ctx.CanDespawnFollower() && !MinionCapReached())
            {
                SoundEngine.PlaySound(CoraliteSoundID.MagicStaff_Item8, ctx.Npc.Center);
            }
        }

        /// <summary>场上小血玉灵是否已达难度上限。旧 AI.cs:776</summary>
        private static bool MinionCapReached()
        {
            int minionType = ModContent.NPCType<BloodiancieMinion>();
            return Main.npc.Count((n) => n.active && n.type == minionType) >= BloodiancieDirector.SummonMinionCap();
        }

        protected override IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
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

                // 为了保证同场召唤物数量不会过多，达到上限直接收招（旧语义）
                if (MinionCapReached())
                {
                    return EndAttack(ctx);
                }

                Vector2 anchor = ctx.Followers[^1].center;
                NPC.NewNPC(ctx.Npc.GetSource_FromThis(), (int)anchor.X, (int)anchor.Y, ModContent.NPCType<BloodiancieMinion>());

                if (!ctx.DespawnFollowers(1))
                {
                    return EndAttack(ctx);
                }
            }

            if (Timer > BloodiancieDirector.SummonEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
