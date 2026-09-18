using Coralite.Content.Bosses.BabyIceDragon.Core;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.BabyIceDragon.States
{
    /// <summary>
    /// 眩晕：撞墙或冰球被击破后被弹开、带着两颗转圈星星落地趴一会儿，结束接一小段休息。<br/>
    /// 这是 skill 说的「声明出来的间隙」——它和 <see cref="BabyIceDragonRestState"/> 一起承担招式之间的喘息，所以 hub 停留帧数才能是 0。<br/>
    /// 入口是外部请求 <see cref="BabyIceDragonContext.PendingDizzyFrames"/>（俯冲撞墙 / <c>IceCube</c> 被击破），
    /// 由状态基类在任何状态下经 ServerUpdate 返回值切进来；本状态在权威端消费掉这个请求，否则退出时会被立刻拉回来。<br/>
    /// 旧 BabyIceDragon.cs:547-571（body）+ :902-933（入场）
    /// </summary>
    [VaultState((int)BabyIceDragonStateId.dizzy, typeof(BabyIceDragonContext))]
    internal sealed class BabyIceDragonDizzyState : BabyIceDragonStateBase
    {
        public override BabyIceDragonStateId StateIndex => BabyIceDragonStateId.dizzy;

        /// <summary>本次眩晕帧数（权威端量：只用来裁决退出，客户端跟 ai[0] 走，不需要过线）。</summary>
        private int dizzyFrames = BabyIceDragonDirector.DizzyFrames;

        protected override void SharedUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            // 眩晕期间掉重力、撞物块（旧 Dizzy() 把 noGravity / noTileCollide 都关了）
            ctx.Gravity = true;
            ctx.TileCollide = true;

            if (Timer == 1)
            {
                EnterCue(ctx);
                return;
            }

            ctx.DeclareDampX(BabyIceDragonDirector.DizzyDampX);
            ctx.DeclareDizzyFrame();
        }

        protected override IVaultState<BabyIceDragonContext> AuthorityUpdate(VaultStateMachine<BabyIceDragonContext> machine, BabyIceDragonContext ctx)
        {
            // 请求必须在本状态里消费掉，否则退出时会被基类的前置检查立刻拉回来；
            // 眩晕中又被请求一次就重开一段（旧 Dizzy() 是直接重设计时器），首帧消费时不重置，免得入场演出放两遍
            if (ctx.PendingDizzyFrames > 0)
            {
                dizzyFrames = ctx.PendingDizzyFrames;
                ctx.PendingDizzyFrames = 0;
                // 旧代码在眩晕时把普通招计数清零（注释原话：虽说感觉可能没什么用的样子）
                ctx.NormalMoveCount = 0;
                if (Timer > 1)
                {
                    Timer = 0;
                }

                ctx.MarkDecision();
                return null;
            }

            if (Timer >= dizzyFrames)
            {
                return EnterRest(ctx, BabyIceDragonDirector.DizzyRestFrames);
            }

            return null;
        }

        /// <summary>
        /// 入场一帧：面向目标、被弹回、朝向归零、帧图切到眩晕列，并放碎裂声 / 两颗转圈星星 / 8 粒碎冰。<br/>
        /// 运动量两端同算（只读已同步的位置与目标），粒子音效只在非专用服务器。旧 BabyIceDragon.cs:902-921
        /// </summary>
        private static void EnterCue(BabyIceDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.FaceTarget();
            npc.velocity = new Vector2(-npc.direction * BabyIceDragonDirector.DizzyKnockX, BabyIceDragonDirector.DizzyKnockY);
            npc.rotation = 0f;
            npc.frameCounter = 0;
            ctx.SetFrame(BabyIceDragonDirector.DizzyFrameX, BabyIceDragonDirector.DizzyFrameAirY);
            ctx.DeclareDirect();

            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched("Icicle/Broken", BabyIceDragonDirector.BrokenSoundVolume, 0f, npc.Center);
            DizzyStar.Spawn(npc.Center, -BabyIceDragonDirector.DizzyStarRotation, BabyIceDragonDirector.DizzyFrames, BabyIceDragonDirector.DizzyStarLength, ctx.DizzyStarCenter);
            DizzyStar.Spawn(npc.Center, BabyIceDragonDirector.DizzyStarRotation, BabyIceDragonDirector.DizzyFrames, BabyIceDragonDirector.DizzyStarLength, ctx.DizzyStarCenter);

            Vector2 mouth = ctx.MouthCenter();
            for (int j = 0; j < BabyIceDragonDirector.DizzyDustCount; j++)
            {
                Dust.NewDustPerfect(mouth, ModContent.DustType<CrushedIceDust>(),
                    -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-BabyIceDragonDirector.DizzyDustAngle, BabyIceDragonDirector.DizzyDustAngle))
                        * Main.rand.NextFloat(BabyIceDragonDirector.DizzyDustSpeedMin, BabyIceDragonDirector.DizzyDustSpeedMax),
                    Scale: Main.rand.NextFloat(BabyIceDragonDirector.DizzyDustScaleMin, BabyIceDragonDirector.DizzyDustScaleMax));
            }
        }
    }
}
