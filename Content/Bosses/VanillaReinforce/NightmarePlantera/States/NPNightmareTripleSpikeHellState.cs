using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States
{
    /// <summary>
    /// 三重尖刺地狱：本体在玩家外围 800 px 绕一整圈并沿途丢尖刺洞，同时另两条"手臂"在 120° 外同步转
    /// ——三条弧线一起收口。转完一圈就瞬移到对侧重来，共三轮。<br/>
    /// 节拍结构（每轮）：绕行 120 帧（每 7 帧一个尖刺洞）→ 收束 25 帧（第 18 帧星尘爆）→ 瞬移到对侧。<br/>
    /// 公平阀：尖刺洞自带 40 帧预警、射程 1100，绕行方向每轮翻转（<c>ShootCount</c> = ±1），
    /// 玩家学会一轮就能读下一轮；三条弧线之间的 120° 缺口就是逃生道。
    /// </summary>
    [VaultState((int)NightmarePlanteraStateId.tripleSpikeHell, typeof(NightmarePlanteraContext))]
    internal sealed class NPNightmareTripleSpikeHellState : NPNightmareStateBase
    {
        private enum Beat
        {
            /// <summary>开场淡出瞬移到玩家一侧</summary>
            Fade,
            /// <summary>绕行 + 收束（共用一个计时，与旧代码一致）</summary>
            Roll,
        }

        public override NightmarePlanteraStateId StateIndex => NightmarePlanteraStateId.tripleSpikeHell;

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>第几轮（槽 A）。沿用旧 <c>SonState</c> 的取值，因为绕行方向与起始角都是它的奇偶决定的。</summary>
        private int round;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            base.OnEnter(machine, ctx);
            round = 0;
        }

        protected override void Phase3Update(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            if (CurrentBeat == Beat.Fade)
            {
                FadeBeat(ctx);
                return;
            }

            RollBeat(ctx);
        }

        /// <summary>
        /// 开场淡出。注意 <c>FadeTick</c> 里 <c>onTeleport</c> 先跑、落点后算，所以落点读到的已经是自增后的轮数
        /// ——旧代码就是这个顺序（Phase3_Nightemare.cs:709-725），起始角因此是"对侧"。
        /// </summary>
        private void FadeBeat(NightmarePlanteraContext ctx)
        {
            if (!FadeTickP3(ctx, () => SideAnchor(ctx), onTeleport: () => OpenRound(ctx)))
            {
                return;
            }

            SwitchBeat(ctx, (int)Beat.Roll);
        }

        /// <summary>本轮的落点：玩家左右两侧交替，距离 700~800。</summary>
        private Vector2 SideAnchor(NightmarePlanteraContext ctx)
            => ctx.TargetCenter + ((round % 2 * MathHelper.Pi).ToRotationVector2()
                * ctx.Boss.NextAttackFloat(NightmarePlanteraDirector.TripleTeleportMin, NightmarePlanteraDirector.TripleTeleportMax));

        /// <summary>开一轮：定绕行方向与朝向，并在 120° 外放出另两条手臂。</summary>
        private void OpenRound(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            round++;

            ctx.ShootCount = round % 2 == 0 ? -1 : 1;
            float baseRot = round % 2 * MathHelper.Pi;
            npc.rotation = baseRot + MathHelper.PiOver2;
            npc.velocity = npc.rotation.ToRotationVector2();

            SpawnArms(ctx, baseRot);
        }

        /// <summary>另两条手臂：与本体同角速度、同方向，相位差 120°。</summary>
        private void SpawnArms(NightmarePlanteraContext ctx, float baseRot)
        {
            NPC npc = ctx.Npc;
            int damage = NightmarePlanteraDirector.P3BiteDamage();

            for (int i = 1; i < NightmarePlanteraDirector.TripleSpikeArms; i++)
            {
                float armRot = baseRot + (i * MathHelper.TwoPi / NightmarePlanteraDirector.TripleSpikeArms);
                npc.NewProjectileInAI_Server<IllusionSpikeHell>(
                    ctx.TargetCenter + (armRot.ToRotationVector2()
                        * ctx.Boss.NextAttackFloat(NightmarePlanteraDirector.TripleTeleportMin, NightmarePlanteraDirector.TripleTeleportMax)),
                    (armRot + (ctx.ShootCount * MathHelper.PiOver2)).ToRotationVector2(),
                    damage, 0, npc.target, armRot, ctx.ShootCount);
            }

            ctx.MarkDecision();
        }

        /// <summary>绕行 120 帧丢尖刺洞，之后 25 帧收束并瞬移到对侧；共用一个计时，与旧代码的拍点一致。</summary>
        private void RollBeat(NightmarePlanteraContext ctx)
        {
            if (Timer < NightmarePlanteraDirector.TripleRollingFrames)
            {
                Rolling(ctx);
                return;
            }

            Closing(ctx);
        }

        private void Rolling(NightmarePlanteraContext ctx)
        {
            NPC npc = ctx.Npc;
            float baseRot = round % 2 * MathHelper.Pi;
            float currentRot = baseRot
                + (ctx.ShootCount * Timer / (float)NightmarePlanteraDirector.TripleRollingFrames * MathHelper.TwoPi);

            ctx.DeclareApproach(ctx.TargetCenter + (currentRot.ToRotationVector2() * NightmarePlanteraDirector.TripleOrbitRadius),
                NightmarePlanteraDirector.TripleOrbitTurn, NightmarePlanteraDirector.TripleOrbitMaxSpeed,
                NightmarePlanteraDirector.TripleOrbitBlend, NightmarePlanteraDirector.TripleOrbitSpeedRange);
            ctx.DeclareRotationTowardsVelocity(0.3f);

            if (Timer % NightmarePlanteraDirector.TripleHoleInterval != 0)
            {
                return;
            }

            Vector2 toTarget = (ctx.TargetCenter - npc.Center).SafeNormalize(Vector2.Zero);
            npc.NewProjectileInAI_Server<ConfusionHole>(npc.Center - (toTarget * NightmarePlanteraDirector.TripleHoleOffset),
                toTarget, NightmarePlanteraDirector.P3BiteDamage(), 0, npc.target,
                NightmarePlanteraDirector.TripleHoleAi1, ctx.Boss.ZenithProjSeedNeg2(), NightmarePlanteraDirector.TripleHoleAi3);
            ctx.MarkDecision();
        }

        /// <summary>收束：刹车转头，扭曲圈脉动，第 18 帧一次星尘爆，到点瞬移到对侧开下一轮。</summary>
        private void Closing(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;
            int fadeTime = NightmarePlanteraDirector.TripleFadeFrames;

            if (Timer == NightmarePlanteraDirector.TripleRollingFrames && !Main.dedServ)
            {
                SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, npc.Center);
            }

            boss.DoRotation(0.3f);
            ctx.DeclareDamp(NightmarePlanteraDirector.TripleCloseDamp);

            // 旧代码这里写的是 alpha -= 1 / fadeTime，整数除法结果是 0，所以本体实际并不淡出；
            // 照搬这个行为以保持表现不变，判断见报告"遗留与建议"。
            float factor = Timer / (float)fadeTime;
            boss.canDrawWarp = true;
            boss.warpScale = MathF.Sin(factor * MathHelper.Pi) * 2f;

            if (Timer == NightmarePlanteraDirector.TripleRollingFrames + (fadeTime * 3 / 4))
            {
                StarBurst(ctx);
            }

            if (Timer <= NightmarePlanteraDirector.TripleRollingFrames + fadeTime)
            {
                return;
            }

            boss.canDrawWarp = false;
            boss.alpha = 1;
            round++;

            ctx.ShootCount = round % 2 == 0 ? -1 : 1;
            float baseRot = round % 2 * MathHelper.Pi;
            npc.Center = SideAnchor(ctx);
            npc.rotation = baseRot + (ctx.ShootCount * MathHelper.PiOver2);
            npc.velocity = npc.rotation.ToRotationVector2();

            if (round <= NightmarePlanteraDirector.TripleRounds)
            {
                SpawnArms(ctx, baseRot);
            }

            boss.ResetTentaclesTo(npc.Center, npc.rotation);
            SwitchBeat(ctx, (int)Beat.Roll);
        }

        /// <summary>瞬移前的星尘爆。抽取两端同跑（落点靠它后面的抽取，见 C3）。</summary>
        private static void StarBurst(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            for (int i = 0; i < NightmarePlanteraDirector.P3FadeDustCount; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * boss.AttackRandom.Next(0, 64)), DustType<NightmareStar>(),
                    dir * boss.NextAttackFloat(2f, 6f), newColor: NightmarePlantera.nightmareRed, Scale: boss.NextAttackFloat(1f, 4f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
            }

            if (!Main.dedServ)
            {
                Helper.PlayPitched(CoraliteSoundID.NoUse_SuperMagicShoot_Item68, npc.Center, pitch: -1f);
            }
        }

        protected override IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
            => round > NightmarePlanteraDirector.TripleRounds ? NPNightmareP3State.Commit(ctx) : null;

        public override void WriteHot(NightmarePlanteraContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = round;
        }

        public override void ReadHot(NightmarePlanteraContext ctx)
        {
            base.ReadHot(ctx);
            round = (int)ctx.Hot[CoraliteBossHotSlots.A];
        }
    }
}
