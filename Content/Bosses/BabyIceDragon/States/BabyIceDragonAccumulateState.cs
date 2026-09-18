using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 蓄力冰球（有破绽招）：贴近目标 → 蓄力 → 在身前凝出一颗冰球 NPC → 绕着它吐冰雾续力，直到冰球被打碎或 900 帧兜底。<br/>
    /// 破绽在于绕圈期间龙自己在慢速固定轨道上，且冰球被打碎会让它进入眩晕（由 <c>IceCube</c> 发起请求）。<br/>
    /// 冰球的 NPC 下标是客户端运动数学要读的量（绕圈方向由它算），旧代码用 <c>movePhase</c> 装并经 SendExtraAI 过线，
    /// 这里改走状态自用热槽 A；收不到时按类型兜底搜一次，搜不到就原地收速等权威端收招（C3）。旧 BabyIceDragon.cs:596-677
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.accumulate, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonAccumulateState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.accumulate;

        /// <summary>冰球 NPC 下标（-1 未知），随热槽 A 过线。</summary>
        private int cubeIndex = -1;

        public override void OnEnter(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            base.OnEnter(machine, ctx);
            cubeIndex = -1;
        }

        public override void WriteHot(BabyIceDragonContext ctx)
        {
            base.WriteHot(ctx);
            ctx.Hot[CoraliteBossHotSlots.A] = cubeIndex;
        }

        public override void ReadHot(BabyIceDragonContext ctx)
        {
            base.ReadHot(ctx);
            cubeIndex = (int)ctx.Hot[CoraliteBossHotSlots.A];
        }

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if (Timer < BabyIceDragonDirector.AccumulateApproachFrames)
            {
                UpdateApproach(ctx);
                return;
            }

            if (CueDue(BabyIceDragonDirector.AccumulateChargeCueFrame))
            {
                ChargeCue(ctx);
            }

            if (Timer < BabyIceDragonDirector.AccumulateSpawnFrame)
            {
                ctx.DeclareDamp(BabyIceDragonDirector.AccumulateBrakeDamp);
                ctx.DeclareFlyingFrame(0, false);
                return;
            }

            UpdateOrbit(ctx);
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            if (Timer < BabyIceDragonDirector.AccumulateSpawnFrame)
            {
                return null;
            }

            NPC npc = ctx.Npc;
            if (Timer == BabyIceDragonDirector.AccumulateSpawnFrame)
            {
                cubeIndex = NPC.NewNPC(npc.GetSource_FromAI(),
                    (int)npc.Center.X + (int)(npc.direction * BabyIceDragonDirector.AccumulateCubeOffsetX),
                    (int)npc.Center.Y + (int)BabyIceDragonDirector.AccumulateCubeOffsetY,
                    ModContent.NPCType<IceCube>());
                ctx.MarkDecision();
                return null;
            }

            // 冰球没了（被打碎或被清场）就收招；旧代码每 20 帧查一次
            if (Timer % BabyIceDragonDirector.AccumulateCubeCheckInterval == 0
                && Helper.GetNPCByType(ModContent.NPCType<IceCube>()) == -1)
            {
                return EndAttack(ctx);
            }

            if (Timer > BabyIceDragonDirector.AccumulateEndFrame)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        /// <summary>就位：Y 追目标、X 慢速贴近、朝向回正。旧 BabyIceDragon.cs:601-617</summary>
        private static void UpdateApproach(BabyIceDragonContext ctx)
        {
            ctx.FaceTarget();
            ctx.DeclareHoverY(0f, 0f, BabyIceDragonDirector.AccumulateDeadZoneY, BabyIceDragonDirector.AccumulateSpeedY,
                BabyIceDragonDirector.AccumulateAccelY, BabyIceDragonDirector.AccumulateTurnY, BabyIceDragonDirector.AccumulateChaseDamp);
            ctx.DeclareChaseX(BabyIceDragonDirector.AccumulateSpeedX, BabyIceDragonDirector.AccumulateAccelX,
                BabyIceDragonDirector.AccumulateTurnX, BabyIceDragonDirector.AccumulateChaseDamp);
            ctx.DeclareRotation(BabyIceDragonRotationMode.TowardsZero, BabyIceDragonDirector.AccumulateRotationStep);
            ctx.DeclareFlyingFrame(0, false);
        }

        /// <summary>绕冰球飞并朝它吐冰雾。旧 BabyIceDragon.cs:652-658</summary>
        private void UpdateOrbit(BabyIceDragonContext ctx)
        {
            ctx.DeclareFlyingFrame(1, false);

            NPC cube = ResolveCube(ctx);
            if (cube == null)
            {
                ctx.DeclareDamp(BabyIceDragonDirector.AccumulateBrakeDamp);
                return;
            }

            NPC npc = ctx.Npc;
            Vector2 targetDir = (cube.Center - npc.Center).SafeNormalize(Vector2.One);
            npc.velocity = targetDir.RotatedBy(-BabyIceDragonDirector.QuarterTurn) * BabyIceDragonDirector.AccumulateOrbitSpeed * npc.direction;
            ctx.DeclareDirect();
            ctx.DeclareRotation(BabyIceDragonRotationMode.TowardsVelocity, BabyIceDragonDirector.AccumulateOrbitRotationStep);

            if (Main.dedServ)
            {
                return;
            }

            PRTLoader.NewParticle(ctx.MouthCenter(),
                targetDir.RotatedBy(Main.rand.NextFloat(-BabyIceDragonDirector.AccumulateFogAngle, BabyIceDragonDirector.AccumulateFogAngle)) * BabyIceDragonDirector.AccumulateFogSpeed,
                CoraliteContent.ParticleType<Fog>(), Color.AliceBlue,
                Main.rand.NextFloat(BabyIceDragonDirector.AccumulateFogScaleMin, BabyIceDragonDirector.AccumulateFogScaleMax));
        }

        /// <summary>取冰球实体：先认热槽里的下标，失效就按类型搜一次（客户端在收到包之前靠这条兜底）。</summary>
        private NPC ResolveCube(BabyIceDragonContext ctx)
        {
            int type = ModContent.NPCType<IceCube>();
            if (cubeIndex > -1 && cubeIndex < Main.maxNPCs && Main.npc[cubeIndex].active && Main.npc[cubeIndex].type == type)
            {
                return Main.npc[cubeIndex];
            }

            int found = Helper.GetNPCByType(type);
            if (found == -1)
            {
                return null;
            }

            cubeIndex = found;
            return Main.npc[found];
        }
    }
}
