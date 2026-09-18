using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 死亡演出：60 帧以 1 px/f 上浮、白光叠加透明度线性拉满 → 第 60 帧炸出 30 圈 × 5 粒紫电并真正死亡。<br/>
    /// <c>CheckDead</c> 只登记 <see cref="ThunderveinDragonContext.KillRequested"/>，切到本态由状态基类的 ServerUpdate 返回值完成，客户端读 ai[0] 跟随。<br/>
    /// 旧 ThunderveinDragon.cs:600-634
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.onKillAnim, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinOnKillAnimState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.onKillAnim;

        /// <summary>本帧越过爆散拍，权威端同帧真正死亡。</summary>
        private bool burstThisFrame;

        /// <summary>爆散只放一次；客户端在服务端删除本 NPC 之前会多跑几帧，不能每帧重放。</summary>
        private bool burstPlayed;

        public override void OnEnter(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            base.OnEnter(machine, ctx);
            burstPlayed = false;

            // 可能死在落雷 / 冥雷的隐身段（SelfAlpha 为 0），死亡演出必须看得见
            ctx.SelfAlpha = 1f;
            ctx.KillAnimAlpha = 0f;
        }

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            burstThisFrame = false;

            // 旧 CheckDead 在切态那一帧置的三个标志，改为每帧声明（原版不同步 dontTakeDamage）
            ctx.Invulnerable = true;
            ctx.CurrentSurrounding = true;
            ctx.SelfAlpha = 1f;

            NPC npc = ctx.Npc;
            if (T < ThunderveinDirector.KillAnimRiseFrames)
            {
                ctx.KillAnimAlpha = MathHelper.Clamp((T + 1) / (float)ThunderveinDirector.KillAnimRiseFrames, 0f, 1f);
                npc.velocity = -Vector2.UnitY * ThunderveinDirector.KillAnimRiseSpeed;
                npc.frame.X = 1;
                npc.frame.Y = 0;
                ctx.DeclareDirect();
                return;
            }

            ctx.KillAnimAlpha = 1f;
            ctx.DeclareKeep();

            if (burstPlayed)
            {
                return;
            }

            burstPlayed = true;
            burstThisFrame = true;
            PlayBurst(ctx);
        }

        /// <summary>纯本地：30 圈电粒子由内向外铺开 + 爆炸音。</summary>
        private static void PlayBurst(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            for (int i = 0; i < ThunderveinDirector.KillAnimRingCount; i++)
            {
                float factor = i / (float)ThunderveinDirector.KillAnimRingCount;
                float length = Helper.Lerp(ThunderveinDirector.KillAnimRingRadiusFrom, ThunderveinDirector.KillAnimRingRadiusTo, factor);

                for (int j = 0; j < ThunderveinDirector.KillAnimParticlesPerRing; j++)
                {
                    PRTLoader.NewParticle(ctx.Npc.Center + Main.rand.NextVector2CircularEdge(length, length), Vector2.Zero,
                        CoraliteContent.ParticleType<ElectricParticle_Purple>(),
                        Scale: Main.rand.NextFloat(ThunderveinDirector.KillAnimParticleScaleMin, ThunderveinDirector.KillAnimParticleScaleMax));
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, ctx.Npc.Center);
            SoundEngine.PlaySound(CoraliteSoundID.BigBOOM_Item62, ctx.Npc.Center);
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            if (burstThisFrame)
            {
                burstThisFrame = false;
                // 此时 CurrentStateId 已是本态，CheckDead 放行真死
                ctx.Npc.Kill();
            }

            return null;
        }
    }
}
