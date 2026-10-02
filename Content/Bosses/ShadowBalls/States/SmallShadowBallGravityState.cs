using Coralite.Content.Bosses.ShadowBalls.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ShadowBalls.States
{
    /// <summary>
    /// 引力（小球侧）：始终绕本体排成圆环缓慢旋转，点名后前摇并向外发射一次激光，打完留在环上。
    /// </summary>
    [VaultState((int)SmallShadowBallStateId.Gravity, typeof(SmallShadowBallContext))]
    public sealed class SmallShadowBallGravityState : SmallShadowBallStateBase
    {
        private enum Beat
        {
            /// <summary>等待本体点名。</summary>
            Wait = 0,
            /// <summary>前摇，然后持续发射激光。</summary>
            Attack = 1,
            /// <summary>已经发射过，只环绕，不再参与点名。</summary>
            Spent = 2,
        }

        private const float OrbitRadius = 160f;
        private const float OrbitSpeed = 0.005f;
        private const int LaserFrames = 60;

        // 前摇与音效只在本地记账；激光只在权威端记账，避免重复生成。
        private bool channelLineSpawned;
        private bool laserSoundPlayed;
        private bool laserFired;

        public override SmallShadowBallStateId StateIndex => SmallShadowBallStateId.Gravity;

        public int GravityBeat => BeatIndex;

        public override void OnEnter(VaultStateMachine<SmallShadowBallContext> machine, SmallShadowBallContext ctx)
        {
            base.OnEnter(machine, ctx);
            channelLineSpawned = false;
            laserSoundPlayed = false;
            laserFired = false;
        }

        /// <summary>本体只能在服务器端把尚未发射的小球从等待拍点名到攻击拍。</summary>
        public void ServerSetBeat(SmallShadowBallContext ctx, int beat)
        {
            if (VaultUtils.isClient || BeatIndex != (int)Beat.Wait || beat != (int)Beat.Attack)
            {
                return;
            }

            SwitchBeat(ctx, beat);
        }

        protected override void SharedUpdate(
            VaultStateMachine<SmallShadowBallContext> machine,
            SmallShadowBallContext ctx,
            ShadowBall owner)
        {
            // 用本体的连续计时器，而不是换拍就清零的 Timer，保证所有子拍共用同一个圆环。
            int count = Math.Max(1, owner.smallBalls.Count);
            Vector2 targetPos = owner.NPC.Center + ctx.Ball.Rotate3D(ctx.Ball.selfIndex / (float)count,
                OrbitRadius, owner.LockTimer * OrbitSpeed, 0f, 0f);

            ctx.Npc.velocity = Vector2.Zero;
            ctx.Npc.Center = Vector2.SmoothStep(ctx.Npc.Center, targetPos, SmallShadowBallDirector.RollingMoveLerp);
            ctx.Npc.rotation = ctx.Npc.rotation.AngleLerp((ctx.Npc.Center - owner.NPC.Center).ToRotation(),
                SmallShadowBallDirector.RollingRotLerp);

            

            if ((Beat)BeatIndex != Beat.Attack || Main.dedServ)
            {
                return;
            }

            int channelFrames = ShadowBallDirector.SpikeBallChannelTime();
            if (!channelLineSpawned && Timer <= channelFrames)
            {
                channelLineSpawned = true;
                // 子拍由服务器点名，客户端可能晚几帧收到；预警只播放剩余前摇，不拖到激光之后。
                int remainingFrames = channelFrames - Timer + 1;
                int spawnFrames = Math.Min(channelFrames / 3, remainingFrames);
                ctx.Ball.SpawnAimLine(SmallShadowBallDirector.AimLineLength,
                    spawnFrames, remainingFrames - spawnFrames);
            }

            if (!laserSoundPlayed && Timer > channelFrames)
            {
                laserSoundPlayed = true;
                if (Timer <= channelFrames + CueCatchUpGrace)
                {
                    Helper.PlayPitched("Shadows/ShadowLaser", 0.2f, 0f, ctx.Npc.Center);
                }
            }
        }

        protected override IVaultState<SmallShadowBallContext> AuthorityUpdate(
            VaultStateMachine<SmallShadowBallContext> machine,
            SmallShadowBallContext ctx,
            ShadowBall owner)
        {
            if ((Beat)BeatIndex != Beat.Attack)
            {
                return null;
            }

            int channelFrames = ShadowBallDirector.SpikeBallChannelTime();
            if (!laserFired && Timer > channelFrames)
            {
                laserFired = true;
                ctx.Npc.NewProjectileDirectInAI_Server<SmallLaser>(ctx.Npc.Center, Vector2.Zero,
                    SmallShadowBallDirector.RollingLaserDamage(), 0,
                    ai0: ctx.Npc.whoAmI, ai1: LaserFrames);
                ctx.MarkDecision();
            }

            if (Timer > channelFrames + LaserFrames)
            {
                // 不回 Idle 或等待拍：继续保持重力圆环，本体的 GravityBeat == 0 筛选不会再选中它。
                SwitchBeat(ctx, (int)Beat.Spent);
            }

            return null;
        }
    }
}
