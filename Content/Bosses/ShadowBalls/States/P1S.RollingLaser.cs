using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 旋转激光（小球侧）：按本体分配的层号错开前摇，三层圈围着本体缓慢反向旋转并依次发射激光，打满两轮回待机。<br/>
    /// 旧 <c>SmallShadowBall.RollingLaser</c>（P1S.RollingLaser.cs:8-93）。<br/>
    /// 层号 / 层内序号 / 本层总数由本体在权威端派活，随小球自己的 <c>SendExtraAI</c> 过线（见 <see cref="SmallShadowBallContext"/>）。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.RollingLaser, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallRollingLaserState : SmallShadowBallStateBase
    {
        /// <summary>旋转激光的子拍。旧 <c>SonState</c> 0~3。</summary>
        private enum Beat
        {
            /// <summary>激光发射中</summary>
            Firing,
            /// <summary>激光发射中</summary>
            Firing2,
            Over,
        }

        public const int LaserTime = 35;

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.RollingLaser;

        /// <summary>一轮射击的时长</summary>
        public static int PerShotTime
            => LaserTime + ShadowBallDirector.RollingLaserChannelTime();

        protected override void SharedUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            ShellMove(ctx, this, owner, (Beat)BeatIndex == Beat.Firing2);

            switch ((Beat)BeatIndex)
            {
                case Beat.Firing:
                case Beat.Firing2:
                    int readyTime = PerShotTime * ctx.OrbitLayer;
                    if (readyTime == 0)
                    {
                        readyTime = 1;
                    }

                    if (Timer <= readyTime)
                    {
                        ctx.Hot[BossSlots.E] = Helper.Lerp(0,600,Timer/(float)readyTime);
                        if (Timer == readyTime)
                            SpawnChannelLine(ctx);
                        break;
                    }
                    else if (Timer <= readyTime + ShadowBallDirector.RollingLaserChannelTime() / 2)
                    {
                        ctx.Hot[BossSlots.E] = Helper.Lerp(600, 0, (Timer - readyTime) / (float)(ShadowBallDirector.RollingLaserChannelTime() / 2));
                    }
                    else if (Timer > readyTime + PerShotTime + (ShadowBallRollingLaserState.HowmanyLayers() - 1 - ctx.OrbitLayer) * PerShotTime)
                    {
                        ctx.Hot[BossSlots.E] = 0;
                        SwitchBeat(ctx, ((Beat)BeatIndex == Beat.Firing) ? (int)Beat.Firing2 : (int)Beat.Over);
                    }
                    break;
                default:
                case Beat.Over:
                    break;
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx, ShadowBall owner)
        {
            switch ((Beat)BeatIndex)
            {
                case Beat.Firing:
                case Beat.Firing2:
                    int readyTime = PerShotTime * ctx.OrbitLayer;

                    if (Timer == 0)
                    {
                        return null;
                    }

                    if (Timer == readyTime + ShadowBallDirector.RollingLaserChannelTime())
                    {
                        Player target = Main.player[Main.npc[(int)ctx.OwnerIndex].target];

                        ctx.Npc.NewProjectileDirectInAI_Server<SmallLaser>(ctx.Npc.Center, Vector2.Zero, SmallLaserDamage(), 0, ai0: ctx.Npc.whoAmI, ai1: LaserTime,ai2: (target.Center - ctx.Npc.Center).Length());
                        ctx.NetUpdate();
                    }
                    return null;
                default:
                case Beat.Over:
                    ctx.Ball.SetReady();
                    return EndAttack();
            }
        }

        /// <summary>
        /// 层内环绕：三层圈按层号取不同半径与转速、奇偶层反向，小球贴着自己那一层的位置走
        /// </summary>
        private static void ShellMove(SmallShadowBallContext ctx, SmallShadowBallStateBase state, ShadowBall owner,bool isSecondShoot)
        {
            int layer = ctx.OrbitLayer;
            int index = (int)ctx.OrbitIndex;
            int perLayer = ctx.OrbitLayerCount;

            if (perLayer < 1)
                return;

            //每层旋转速度
            float baseRot = (owner.LockTimer + ctx.Hot[BossSlots.E]) * (0.004f +  (layer * 0.001f)) * (layer % 2 == 0 ? 1 : -1);

            Vector2 targetPos = owner.NPC.Center + ctx.Ball.Rotate3D(index / (float)perLayer, 110 + (layer * 20), baseRot, 0f, 0f, owner.LockLerpPercent);

            float f = isSecondShoot ? 0.8f : (0.1f + 0.7f * Helper.Clamp(state.Timer / 120f, 0, 1));

            ctx.Npc.Center = Vector2.SmoothStep(ctx.Npc.Center, targetPos, f);
            ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp((ctx.Npc.Center - owner.NPC.Center).ToRotation(), f);
        }

        /// <summary>蓄力预警线</summary>
        private static void SpawnChannelLine(SmallShadowBallContext ctx)
        {
            int time = ShadowBallDirector.RollingLaserChannelTime();
            ctx.Ball.SpawnAimLine(SmallShadowBallDirector.AimLineLength, time * 2 / 3, time / 3);
        }
    }
}
