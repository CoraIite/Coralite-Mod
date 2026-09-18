using Coralite.Content.Bosses.ThunderveinDragon.Core;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Bosses.ThunderveinDragon.States
{
    /// <summary>
    /// 冥雷（由 <c>PhaseController</c> 在三 / 四阶段阈值上触发，不进加权表）：旋进背景放幻影，打不掉就吃终结雷暴。<br/>
    /// 节拍：Chase（横向带宽 200~600 px，最长 240 帧）→ Roll（旋转 65 帧同时淡出，转入背景并生成 <c>ThunderPhantom</c>）
    /// → Phantom（隐身悬在玩家头顶，幻影每打完一轮雷暴给本体 <c>ai[2]</c> 加一，四轮后自毁）
    /// → Flash（显形 30 帧）→ Burst（终结雷暴 50 帧，天空亮 0.7）→ Recover（20 帧后摇）。<br/>
    /// 打破分支：幻影被玩家击杀 → Broken（140 帧慢慢显形后收招，本招作废）——这就是这招的解法。<br/>
    /// 公平阀：隐身期本体无敌但完全不出手，压力全在幻影身上；终结雷暴前有 30 帧显形预告。<br/>
    /// 旧 AI.StygianThunder.cs:11-222
    /// </summary>
    [VaultState((int)ThunderveinDragon.AIStates.StygianThunder, typeof(ThunderveinDragonContext))]
    internal sealed class ThunderveinStygianThunderState : ThunderveinStateBase
    {
        public override ThunderveinDragon.AIStates StateIndex => ThunderveinDragon.AIStates.StygianThunder;

        private enum Beat
        {
            /// <summary>拉到带宽内（旧 SonState 0）</summary>
            Chase = 0,
            /// <summary>旋转淡出 + 放幻影（旧 1）</summary>
            Roll = 1,
            /// <summary>隐身等幻影打完（旧 2~5）</summary>
            Phantom = 2,
            /// <summary>闪现显形（旧 6）</summary>
            Flash = 3,
            /// <summary>终结雷暴（旧 7）</summary>
            Burst = 4,
            /// <summary>后摇（旧 8）</summary>
            Recover = 5,
            /// <summary>幻影被打破（旧 -1）</summary>
            Broken = 6,
        }

        private Beat CurrentBeat => (Beat)BeatIndex;

        /// <summary>幻影的 NPC 索引（热字段 A；旧 localAI[0] Recorder）。</summary>
        private float phantomIndex;

        private bool summonThisFrame;
        private bool fireThisFrame;

        public override void OnEnter(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            base.OnEnter(machine, ctx);

            // 本态由 PhaseController 直接换入，不经 hub 的 Commit，自行过账
            if (!VaultUtils.isClient)
            {
                ctx.RecordPick((int)StateIndex);
            }
        }

        protected override void SharedUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            summonThisFrame = false;
            fireThisFrame = false;

            switch (CurrentBeat)
            {
                case Beat.Roll:
                    UpdateRoll(ctx);
                    break;
                case Beat.Phantom:
                    UpdatePhantom(ctx);
                    break;
                case Beat.Flash:
                    UpdateFlash(ctx);
                    break;
                case Beat.Burst:
                    UpdateBurst(ctx);
                    break;
                case Beat.Recover:
                    ctx.Boss.FlyingFrame();
                    ctx.DeclareKeep();
                    break;
                case Beat.Broken:
                    UpdateBroken(ctx);
                    break;
                default:
                    UpdateChase(ctx);
                    break;
            }
        }

        private void UpdateChase(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            Vector2 targetPos = ctx.Target.Center;
            npc.direction = npc.spriteDirection = targetPos.X > npc.Center.X ? 1 : -1;
            npc.directionY = targetPos.Y > npc.Center.Y ? 1 : -1;
            ctx.DeclareRotationNormal();
            ctx.DeclareChase(targetPos, ThunderveinDirector.StygianChase);

            ctx.Boss.GetLengthToTargetPos(targetPos, out float xLength, out float yLength);
            bool ready = xLength > ThunderveinDirector.StygianChaseExitXMin
                && xLength < ThunderveinDirector.StygianChaseExitXMax
                && yLength < ThunderveinDirector.StygianChaseExitY;
            if (Timer <= ThunderveinDirector.StygianChaseFrames && !ready)
            {
                return;
            }

            ctx.Boss.DashFrame();
            ctx.Boss.ResetAllOldCaches();
            npc.velocity = (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero)
                .RotatedBy(-npc.direction * MathHelper.PiOver2) * ThunderveinDirector.RollSpeed;
            ctx.DeclareDirect();
            SwitchBeat(ctx, (int)Beat.Roll);
        }

        /// <summary>旋进背景：与吐息的绕飞反向，且边转边衰减、边淡出。旧 :81-112</summary>
        private void UpdateRoll(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.ShadowScale = ThunderveinDirector.DashShadowScale;
            ctx.Boss.UpdateAllOldCaches();

            npc.velocity = npc.velocity.RotatedBy(npc.spriteDirection * MathHelper.TwoPi / ThunderveinDirector.RollTurnDivisor);
            npc.velocity *= ThunderveinDirector.StygianRollDamp;
            npc.rotation = npc.velocity.ToRotation();
            ctx.DeclareDirect();

            float fade = 1f - (T / (float)ThunderveinDirector.StygianRollFrames);
            ctx.ShadowAlpha = fade;
            ctx.SelfAlpha = fade;

            if (Timer <= ThunderveinDirector.StygianRollFrames)
            {
                return;
            }

            ctx.DeclareNoRot();
            npc.velocity *= 0;
            ctx.SelfAlpha = 0f;
            ctx.ShadowAlpha = 0f;
            summonThisFrame = true;
            SwitchBeat(ctx, (int)Beat.Phantom);
        }

        /// <summary>
        /// 隐身期：悬在玩家头顶 400 px。两个出口（幻影被打掉 / 打满四轮）都只由权威端在 <see cref="AuthorityUpdate"/> 里裁决——
        /// 幻影索引是服务端生成的，客户端在收到那一包之前无从判断“幻影还在不在”，自行裁决会立刻误判成打破。旧 :113-133
        /// </summary>
        private void UpdatePhantom(ThunderveinDragonContext ctx)
        {
            ctx.Invulnerable = true;
            ctx.SelfAlpha = 0f;
            ctx.ShadowAlpha = 0f;
            ctx.DeclareKeep();
            ctx.Npc.Center = ctx.Target.Center + new Vector2(0, ThunderveinDirector.StygianHoverOffsetY);
        }

        /// <summary>幻影被打破：140 帧慢慢显形，本招作废直接收招。旧 :19-29</summary>
        private void UpdateBroken(ThunderveinDragonContext ctx)
        {
            if (T == 0)
            {
                ctx.Npc.QuickSetDirection();
                ctx.Boss.ResetAllOldCaches();
            }

            ctx.Boss.FlyingFrame();
            ctx.DeclareNoRot(1f);
            ctx.DeclareKeep();
            ctx.SelfAlpha = MathHelper.Clamp(Timer / (float)ThunderveinDirector.StygianBrokenFrames, 0f, 1f);
        }

        /// <summary>闪现到头顶显形，翅膀到位后 30 帧放终结雷暴。旧 :134-182</summary>
        private void UpdateFlash(ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;
            ctx.Invulnerable = true;
            npc.Center = ctx.Target.Center + new Vector2(0, ThunderveinDirector.StygianHoverOffsetY);
            ctx.DeclareDamp(ThunderveinDirector.StygianFlashDamp);
            npc.QuickSetDirection();
            ctx.DeclareNoRot();

            float fade = T / ThunderveinDirector.StygianFlashFadeFrames;
            ctx.SelfAlpha = fade;
            ctx.ShadowAlpha = fade;

            if (npc.frame.Y != 4)
            {
                ctx.Boss.FlyingFrame();
                HoldTimer();
                return;
            }

            if (Timer <= ThunderveinDirector.StygianFlashFrames)
            {
                return;
            }

            npc.TargetClosest();
            fireThisFrame = true;
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.Invulnerable = false;
            ctx.Boss.ResetAllOldCaches();
            PlayFireCue(ctx);
            SwitchBeat(ctx, (int)Beat.Burst);
        }

        private static void PlayFireCue(ThunderveinDragonContext ctx)
        {
            if (Main.dedServ)
            {
                return;
            }

            SoundEngine.PlaySound(CoraliteSoundID.BubbleShield_Electric_NPCHit43, ctx.Npc.Center);
            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, ctx.Npc.Center);
            SoundEngine.PlaySound(CoraliteSoundID.Thunder, ctx.Npc.Center);
            Shake(ctx, Vector2.UnitY * ThunderveinDirector.BurstShakeDirY, ThunderveinDirector.BigBurstShakeStrength,
                ThunderveinDirector.BigBurstShakeVibration, ThunderveinDirector.BigBurstShakeFrames);
            ThunderveinDragon.SetBackgroundLight(ThunderveinDirector.StygianSkyLight,
                ThunderveinDirector.StygianBurstFrames, ThunderveinDirector.StygianSkyExchange);
        }

        private void UpdateBurst(ThunderveinDragonContext ctx)
        {
            ctx.DrawShadows = true;
            ctx.CurrentSurrounding = true;
            ctx.SelfAlpha = 1f;
            ctx.DeclareKeep();
            ctx.Boss.UpdateAllOldCaches();
            BurstShadowEnvelope(ctx, T / (float)ThunderveinDirector.StygianBurstFrames, ThunderveinDirector.BurstShadowScaleTo);
            BurstMouthFrame(ctx.Npc);

            if (Timer > ThunderveinDirector.StygianBurstFrames)
            {
                SwitchBeat(ctx, (int)Beat.Recover);
            }
        }

        protected override void OnBeatAdopted(ThunderveinDragonContext ctx, int previousBeat)
        {
            switch (CurrentBeat)
            {
                case Beat.Burst:
                    ctx.Boss.ResetAllOldCaches();
                    PlayFireCue(ctx);
                    break;
                case Beat.Broken:
                    ctx.Boss.ResetAllOldCaches();
                    break;
                default:
                    break;
            }
        }

        protected override IVaultState<ThunderveinDragonContext> AuthorityUpdate(VaultStateMachine<ThunderveinDragonContext> machine, ThunderveinDragonContext ctx)
        {
            NPC npc = ctx.Npc;

            if (summonThisFrame)
            {
                summonThisFrame = false;
                phantomIndex = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y,
                    ModContent.NPCType<ThunderPhantom>(), ai0: npc.whoAmI, Target: npc.target);
                // 幻影轮数从 2 起（旧代码进入幻影段时 SonState 恰为 2，幻影共打 4 轮）
                ctx.SonState = ThunderveinDirector.StygianPhantomSonStateStart;
                ctx.MarkDecision();
            }

            // 隐身期的两个出口只在权威端裁决，客户端靠 Beat 热字段跟随
            if (CurrentBeat == Beat.Phantom)
            {
                if (!phantomIndex.GetNPCOwner<ThunderPhantom>(out _))
                {
                    SwitchBeat(ctx, (int)Beat.Broken);
                }
                else if (ctx.SonState > ThunderveinDirector.StygianPhantomSonStateEnd)
                {
                    SwitchBeat(ctx, (int)Beat.Flash);
                }

                return null;
            }

            if (fireThisFrame)
            {
                fireThisFrame = false;
                ctx.MarkDecision();
                npc.NewProjectileDirectInAI<EndThunder>(
                    npc.Center + new Vector2(0, ThunderveinDirector.StygianEndFromY),
                    npc.Center + new Vector2(0, ThunderveinDirector.StygianEndToY),
                    ThunderveinDirector.StygianEndDamage(), 0, npc.target,
                    ThunderveinDirector.StygianEndProjAi0, npc.whoAmI, ThunderveinDirector.StygianEndProjAi2);
            }

            if (CurrentBeat == Beat.Recover && Timer > ThunderveinDirector.StygianRecoverFrames)
            {
                return EndAttack(ctx);
            }

            if (CurrentBeat == Beat.Broken && Timer > ThunderveinDirector.StygianBrokenFrames)
            {
                return EndAttack(ctx);
            }

            return null;
        }

        protected override void WriteSlots(ThunderveinDragonContext ctx)
            => ctx.Hot[CoraliteBossHotSlots.A] = phantomIndex;

        protected override void ReadSlots(ThunderveinDragonContext ctx)
            => phantomIndex = ctx.Hot[CoraliteBossHotSlots.A];
    }
}
