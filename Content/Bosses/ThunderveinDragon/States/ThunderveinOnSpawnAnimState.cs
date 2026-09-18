using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 出生演出：闪电杆召唤点上方 400 px 现身 → 80 帧淡入 → 出名牌停 30 帧 → 吼叫 150 帧 → 首招。<br/>
    /// 节拍：Place（一帧，定位 + 透明度归零）/ FadeIn / Appear / Roar。全程无敌（每帧重声明，原版不同步 dontTakeDamage）。<br/>
    /// 公平阀：整段不出手，玩家有 260 帧准备时间。旧 ThunderveinDragon.cs:495-598
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.onSpawnAnmi, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinOnSpawnAnimState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.onSpawnAnmi;

        private enum Beat
        {
            /// <summary>定位（旧 SonState 0）</summary>
            Place = 0,
            /// <summary>淡入（旧 1）</summary>
            FadeIn = 1,
            /// <summary>名牌停留（旧 2）</summary>
            Appear = 2,
            /// <summary>吼叫（旧 3）</summary>
            Roar = 3,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>本帧淡入结束，权威端同帧出名牌弹幕。</summary>
        private bool nameplateThisFrame;

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            nameplateThisFrame = false;

            // 整段无敌；原版不同步 dontTakeDamage，两端每帧同声明
            ctx.Invulnerable = true;

            switch (CurrentBeat)
            {
                case Beat.FadeIn:
                    UpdateFadeIn(ctx);
                    break;
                case Beat.Appear:
                    UpdateAppear(ctx);
                    break;
                case Beat.Roar:
                    UpdateRoar(ctx);
                    break;
                default:
                    UpdatePlace(ctx);
                    break;
            }
        }

        /// <summary>定位：站到召唤弹幕（找不到就站到目标）头顶 400 px，透明度归零准备淡入。旧 :502-530</summary>
        private void UpdatePlace(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            npc.TargetClosest();
            ctx.DeclareNoRot(1f);
            ctx.DeclareDirect();

            Vector2 anchor = ctx.Target.Center;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<ThunderSpawn>())
                {
                    anchor = proj.Center;
                    break;
                }
            }

            npc.Center = anchor + new Vector2(0, -ThunderveinDirector.SpawnHeightAboveAnchor);
            ctx.SelfAlpha = 0f;
            SwitchBeat(ctx, (int)Beat.FadeIn);
        }

        /// <summary>淡入 80 帧，到点出名牌。旧 :531-545</summary>
        private void UpdateFadeIn(ThunderveinDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.Boss.FlyingFrame();
            // 用 T 直接算而不是累加：客户端收养 Timer 之后也能得到同一个值
            ctx.SelfAlpha = MathHelper.Clamp((T + 1) / (float)ThunderveinDirector.SpawnFadeInFrames, 0f, 1f);

            if (Timer > ThunderveinDirector.SpawnFadeInFrames)
            {
                SwitchBeat(ctx, (int)Beat.Appear);
                nameplateThisFrame = true;
            }
        }

        /// <summary>名牌停留 30 帧。旧 :546-556</summary>
        private void UpdateAppear(ThunderveinDragonContext ctx)
        {
            ctx.DeclareKeep();
            ctx.Boss.FlyingFrame();
            ctx.SelfAlpha = 1f;

            if (Timer > ThunderveinDirector.SpawnAppearFrames)
            {
                SwitchBeat(ctx, (int)Beat.Roar);
            }
        }

        /// <summary>吼叫 150 帧：等翅膀帧到位 → 第 15 帧张嘴定格 + 音效 → 15~130 帧声波 / 声线 / 震屏。旧 :557-595</summary>
        private void UpdateRoar(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.Boss.UpdateAllOldCaches();
            npc.QuickSetDirection();
            ctx.DeclareNoRot();
            ctx.DeclareDamp(ThunderveinDirector.SpawnRoarDamp);

            // 旧代码等翅膀帧回到 4 才起拍，期间不计时
            if (T == 0 && npc.frame.Y != 4)
            {
                ctx.Boss.FlyingFrame();
                HoldTimer();
                return;
            }

            if (T == ThunderveinDirector.SpawnRoarCueFrame)
            {
                npc.frame.Y = 0;
                npc.frame.X = 1;
                npc.velocity *= 0;
                ctx.DeclareDirect();
                PlayRoarSound(ctx);
            }
            else if (T > ThunderveinDirector.SpawnRoarCueFrame && T < ThunderveinDirector.SpawnRoarFxEnd)
            {
                RoarFx(ctx);
                if (T % ThunderveinDirector.RoarWaveInterval == 0)
                {
                    Shake(ctx, Helper.NextVec2Dir(), ThunderveinDirector.SpawnRoarShakeStrength,
                        ThunderveinDirector.SpawnRoarShakeVibration, ThunderveinDirector.SpawnRoarShakeFrames);
                }
            }
        }

        private static void PlayRoarSound(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            Helper.PlayPitched(CoraliteSoundID.LightningOrb_Item121, ctx.Npc.Center, pitch: ThunderveinDirector.RoarPitch);
            SoundEngine.PlaySound(CoraliteSoundID.Roar, ctx.Npc.Center);
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            if (nameplateThisFrame)
            {
                nameplateThisFrame = false;
                ctx.Npc.NewProjectileDirectInAI<ThunderveinDragon_OnSpawnAnim>(ctx.Npc.Center, Vector2.Zero, 1, 0, ctx.Npc.target);
            }

            if (CurrentBeat == Beat.Roar && Timer > ThunderveinDirector.SpawnRoarFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }
    }
}
