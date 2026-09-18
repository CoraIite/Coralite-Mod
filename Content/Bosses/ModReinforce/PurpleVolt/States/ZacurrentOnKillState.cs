using Coralite.Content.Bosses.ModReinforce.PurpleVolt.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt.States
{
    /// <summary>
    /// 死亡演出：匀速上浮、残影淡入、每 4 帧甩一条雷电，120 帧后炸开一圈电并由权威端真正 <c>Kill</c>。<br/>
    /// 进入时把无敌与视觉开关声明成每帧量（原版 SyncNPC 不同步 <c>dontTakeDamage</c>，只能两端各自声明）。<br/>
    /// 旧 ZcurrentAI.cs:240-278 / ZacurrentDragon.States.cs:106-125
    /// </summary>
    [VaultState((int)ZacurrentDragon.AIStates.onKillAnim, typeof(ZacurrentDragonContext))]
    public sealed class ZacurrentOnKillState : ZacurrentStateBase
    {
        public override ZacurrentDragon.AIStates StateIndex => ZacurrentDragon.AIStates.onKillAnim;

        /// <summary>演出不能被超时打断。</summary>
        protected override int TimeoutFrames => int.MaxValue;

        protected override void SharedUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            ZacurrentDragon boss = ctx.Boss;
            NPC npc = ctx.Npc;

            ctx.Invulnerable = true;
            boss.currentSurrounding = true;
            boss.canDrawShadows = false;
            boss.IsDashing = false;

            npc.velocity = new Vector2(0, ZacurrentDirector.KillAnimRiseSpeed);
            boss.shadowAlpha = Math.Clamp(ctx.Timer / ZacurrentDirector.KillAnimFadeFrames, 0, 1);

            if (ctx.Timer % ZacurrentDirector.KillAnimThunderInterval == 0 && !VaultUtils.isServer)
            {
                float speed = Main.rand.NextFloat(ZacurrentDirector.KillAnimThunderSpeedMin, ZacurrentDirector.KillAnimThunderSpeedMax);
                Vector2 dir = Helper.NextVec2Dir();
                PurpleThunderParticle.Spawn(() => npc.Center, dir * speed,
                    ZacurrentDirector.ThunderParticleMaxTime, ZacurrentDirector.ThunderParticleFadeTime,
                    ZacurrentDirector.ThunderParticlePointCount, ZacurrentDirector.ThunderParticleWidth, ZacurrentDragon.ZacurrentRed);
            }

            ctx.Timer++;
            if (ctx.Timer > ZacurrentDirector.KillAnimFrames && !ctx.AttackFinished)
            {
                ctx.AttackFinished = true;
                Helper.PlayPitched(CoraliteSoundID.NoUse_ElectricMagic_Item122, npc.Center);
                ZacurrentDragon.BurstRing(npc.Center);
                SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, npc.Center);
            }
        }

        protected override IVaultState<ZacurrentDragonContext> AuthorityUpdate(VaultStateMachine<ZacurrentDragonContext> machine, ZacurrentDragonContext ctx)
        {
            //真正的死亡只由权威端执行，CheckDead 此时直接放行
            if (ctx.AttackFinished)
                ctx.Npc.Kill();

            return null;
        }
    }
}
