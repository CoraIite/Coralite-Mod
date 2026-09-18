using Coralite.Content.NPCs.Crystalline.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline.States
{
    /// <summary>
    /// 一阶段碎岩（血量首次跌破 75% 时替换掉那一次飞弹）：站定摆出发力动作，
    /// 第 35 帧把环绕本体的三块浮石轰出去变成实体碎岩，50 帧后回站立。<br/>
    /// 旧 <c>P1Rock</c>（CrystallineSentinel.cs:1153-1195）。脱战回血时碎岩机会会还回来，所以整场可以出多次。
    /// </summary>
    [VaultState((int)CrystallineSentinelStateId.P1Rock, typeof(CrystallineSentinelContext))]
    internal sealed class CrystallineSentinelP1RockState : CrystallineSentinelStateBase
    {
        public override CrystallineSentinelStateId StateIndex => CrystallineSentinelStateId.P1Rock;

        /// <summary>旧代码在招式体开头就 <c>Timer++</c>，之后的出手拍与收招拍读的都是自增后的值，这里对齐。</summary>
        private float PostTimer => AttackTimer + 1;

        protected override void SharedUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (AtFrame(0))
            {
                SetFrame(ctx, CrystallineSentinelDirector.RockFrameColumn, 0);
                ctx.Npc.velocity = Vector2.Zero;
            }

            if (AttackTimer > 0 && AttackTimer % CrystallineSentinelDirector.RockFrameRate == 0)
            {
                AdvanceFrame(ctx, CrystallineSentinelDirector.RockFrameMaxY);
            }

            ctx.DeclareStandStill();
            ctx.FaceTarget();

            // 出手拍：环绕的浮石粒子读到这条声明就自毁，本体同时长出实体碎岩
            if (PostTimer == CrystallineSentinelDirector.RockReleaseFrame)
            {
                ctx.RockReleaseCue = true;
                BlastEffect(ctx);
            }
        }

        /// <summary>
        /// 出手的碎岩爆点（纯本地）。旧代码把粒子和实体一起放在服务端分支里，联机的客户端什么都看不到；
        /// 这里挪到两端同跑的表现路径，随机量只喂粒子（C8）。
        /// </summary>
        private static void BlastEffect(CrystallineSentinelContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < CrystallineSentinelDirector.RockCount; i++)
            {
                Vector2 dir = (rot + (i * MathHelper.TwoPi / 3)).ToRotationVector2();
                CrystallineRockBlast blast = PRTLoader.NewParticle<CrystallineRockBlast>(
                    ctx.Npc.Center + (dir * CrystallineSentinelDirector.RockSpawnRadius), Vector2.Zero);
                if (blast != null)
                {
                    blast.Rotation = dir.ToRotation();
                }
            }
        }

        protected override IVaultState<CrystallineSentinelContext> AuthorityUpdate(VaultStateMachine<CrystallineSentinelContext> machine, CrystallineSentinelContext ctx)
        {
            if (ctx.RockReleaseCue)
            {
                ReleaseRocks(ctx);
            }

            if (PostTimer > CrystallineSentinelDirector.RockTotalFrames)
            {
                return ReturnToIdle(ctx, IdleFramesOnFinish);
            }

            return null;
        }

        /// <summary>三块浮石绕本体均分出生、朝斜上方散开。旧 CrystallineSentinel.cs:1175-1191</summary>
        private static void ReleaseRocks(CrystallineSentinelContext ctx)
        {
            NPC npc = ctx.Npc;
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);

            for (int i = 0; i < CrystallineSentinelDirector.RockCount; i++)
            {
                float speed = Main.rand.NextFloat(CrystallineSentinelDirector.RockSpeedMin, CrystallineSentinelDirector.RockSpeedMax)
                    * CrystallineSentinelDirector.RockSpeedScale;
                Vector2 vel = -Vector2.UnitY.RotatedBy(-MathHelper.Pi / 3 + (i * MathHelper.PiOver4)).RotateByRandom(0.1f, 0.5f) * speed;
                Vector2 pos = npc.Center + (rot + (i * MathHelper.TwoPi / 3)).ToRotationVector2() * CrystallineSentinelDirector.RockSpawnRadius;

                NPC floatStone = NPC.NewNPCDirect(npc.GetSource_FromAI(), pos, ModContent.NPCType<CrystallineSentinelFloatStoneGrow>());
                floatStone.velocity = vel;
                floatStone.netUpdate = true;
            }

            ctx.MarkDecision();
        }
    }
}
