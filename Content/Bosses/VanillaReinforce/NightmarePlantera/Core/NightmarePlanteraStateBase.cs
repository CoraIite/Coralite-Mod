using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.States;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core
{
    /// <summary>
    /// 梦魇世纪花的平坦状态索引，写入 <c>npc.ai[0]</c> 同步。<br/>
    /// 0~7 沿用旧 <see cref="NightmarePlantera.AIPhases"/> 的数值（线格式不变），8 起是从旧 <c>localAI[0]</c> 招式层搬上来的招式态。
    /// </summary>
    internal enum NightmarePlanteraStateId : int
    {
        //---------- 旧宏观阶段，数值不变 ----------
        /// <summary>出生演出</summary>
        onSpawnAnmi_P0 = 0,
        /// <summary>一阶段：沉睡（连接段 + 一阶段选招口）</summary>
        sleeping_P1 = 1,
        /// <summary>一阶段 → 二阶段转阶段演出</summary>
        exchange_P1_P2 = 2,
        /// <summary>二阶段：梦境（第一步里仍以旧招式 switch 分派，第二步拆平）</summary>
        dream_P2 = 3,
        /// <summary>三阶段：噩梦（连接段 + 三阶段选招口）</summary>
        nightemare_P3 = 4,
        /// <summary>预留的四阶段，旧代码就没有注册过实现，保留数值占位</summary>
        wakeUp_P4 = 5,
        /// <summary>白天狂暴</summary>
        rampage = 6,
        /// <summary>噩梦值溢出的处决</summary>
        suddenDeath = 7,

        //---------- 一阶段招式 ----------
        /// <summary>一阶段待机</summary>
        p1_Idle = 8,
        /// <summary>沉眠之雾</summary>
        hypnotizeFog = 9,
        /// <summary>黑暗之触</summary>
        darkTentacle = 10,
        /// <summary>黑暗飞叶</summary>
        darkLeaves = 11,

        //---------- 三阶段招式 ----------
        /// <summary>二阶段 → 三阶段转阶段</summary>
        exchange_P2_P3 = 12,
        /// <summary>幻影撕咬</summary>
        illusionBite = 13,
        /// <summary>噩梦撕咬</summary>
        p3_nightmareBite = 14,
        /// <summary>噩梦冲刺</summary>
        p3_nightmareDash = 15,
        /// <summary>虚假撕咬</summary>
        p3_fakeBite = 16,
        /// <summary>三重尖刺地狱</summary>
        tripleSpikeHell = 17,
        /// <summary>群花乱舞</summary>
        flowerDance = 18,
        /// <summary>超级钩爪斩</summary>
        superHookSlash = 19,
        /// <summary>尖刺与闪光</summary>
        p3_spikesAndSparkles = 20,
        /// <summary>瞬移闪光</summary>
        p3_teleportSparkles = 21,
        /// <summary>藤蔓喷发</summary>
        vineSpurt = 22,

        //---------- 二阶段招式（第二步落地，先占号，避免二次改线格式） ----------
        nightmareBite = 23,
        nightmareDash = 24,
        fakeBite = 25,
        fantasyHelp = 26,
        rollingThenBite = 27,
        belowSparkleThenBite = 28,
        spikeBalls = 29,
        batsAndCrows = 30,
        hookSlash = 31,
        spikesAndSparkles = 32,
        spikeHell = 33,
        ghostDash = 34,
        teleportSparkle = 35,
        dreamSparkle = 36,
        p2_Idle = 37,
        fantasyHunting = 38,
        blackHole = 39,
        dreamingNightmareBite = 40,
        dreamingSpikeHell = 41,
        dreamingFantasyHunting = 42,
        dreaming_Idle = 43,
    }

    /// <summary>
    /// 梦魇世纪花状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——运动 / 朝向 / 无敌声明、确定性子拍、以及**参数里会推进 <c>AttackRandom</c> 的生成调用**
    ///   （<c>NewProjectileInAI_Server</c> 一族只在客户端跳过生成，实参照常求值，这是两端随机序列一致的前提，不能挪进权威端）。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——掷骰、改血、换态；本类把强制换态请求、死亡请求与超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>；招式体内不得出现 <c>ChangeState</c>。
    /// </summary>
    internal abstract class NightmarePlanteraStateBase : CoraliteBossState<NightmarePlanteraContext>
    {
        public abstract NightmarePlanteraStateId StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>超时兜底帧数；演出态覆盖为更大值。</summary>
        protected virtual int TimeoutFrames => NightmarePlanteraDirector.StateTimeoutFrames;

        public override void OnEnter(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            // 基座在权威端 roll 新种子并随同一包过线；两端随后从同一个 ai[1] 重建 AttackRandom。
            base.OnEnter(machine, ctx);
            ctx.Boss.RefreshAttackRandom();
        }

        protected sealed override IVaultState<NightmarePlanteraContext> ServerUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx)
        {
            // 外部强制换态（白天狂暴、脱战重置、噩梦值处决、转阶段）：只从这里出门。
            if (ctx.TryConsumePendingState(out int requested) && requested != StateId)
            {
                IVaultState<NightmarePlanteraContext> forced = VaultStateRegistry<NightmarePlanteraContext>.Create(requested);
                if (forced != null)
                {
                    return forced;
                }
            }

            // 超时兜底：状态机永远不许死在一个状态里。
            if (Counter++ > TimeoutFrames)
            {
                ctx.Npc.velocity *= 0.6f;
                return EndAttack(ctx);
            }

            return AuthorityUpdate(machine, ctx);
        }

        /// <summary>仅权威端：掷骰、改血、返回下一状态或 null。</summary>
        protected abstract IVaultState<NightmarePlanteraContext> AuthorityUpdate(VaultStateMachine<NightmarePlanteraContext> machine, NightmarePlanteraContext ctx);

        #region 状态构造与路由

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        internal static IVaultState<NightmarePlanteraContext> Create(NightmarePlanteraStateId id)
            => VaultStateRegistry<NightmarePlanteraContext>.Create((int)id);

        /// <summary>状态 id → 宏观阶段。BOSS 头像、伤害修正、外部弹幕的阶段判断都走这里，不再直接比 <c>ai[0]</c>。</summary>
        internal static NightmarePlantera.AIPhases MacroPhaseOf(int stateId) => (NightmarePlanteraStateId)stateId switch
        {
            NightmarePlanteraStateId.onSpawnAnmi_P0 => NightmarePlantera.AIPhases.OnSpawnAnmi_P0,
            NightmarePlanteraStateId.sleeping_P1
                or NightmarePlanteraStateId.p1_Idle
                or NightmarePlanteraStateId.hypnotizeFog
                or NightmarePlanteraStateId.darkTentacle
                or NightmarePlanteraStateId.darkLeaves => NightmarePlantera.AIPhases.Sleeping_P1,
            NightmarePlanteraStateId.exchange_P1_P2 => NightmarePlantera.AIPhases.Exchange_P1_P2,
            NightmarePlanteraStateId.rampage => NightmarePlantera.AIPhases.Rampage,
            NightmarePlanteraStateId.suddenDeath => NightmarePlantera.AIPhases.SuddenDeath,
            NightmarePlanteraStateId.wakeUp_P4 => NightmarePlantera.AIPhases.WakeUp_P4,
            >= NightmarePlanteraStateId.exchange_P2_P3 and <= NightmarePlanteraStateId.vineSpurt
                => NightmarePlantera.AIPhases.Nightemare_P3,
            NightmarePlanteraStateId.nightemare_P3 => NightmarePlantera.AIPhases.Nightemare_P3,
            _ => NightmarePlantera.AIPhases.Dream_P2,
        };

        /// <summary>
        /// 收招：回到本阶段的选招口。演出态 / 狂暴 / 处决走到这里只可能是超时兜底，
        /// 那种情况下按血量回到对应阶段（硬塞二阶段会让残血的 boss 掉回上一阶段）。
        /// </summary>
        protected IVaultState<NightmarePlanteraContext> EndAttack(NightmarePlanteraContext ctx)
            => MacroPhaseOf(StateId) switch
            {
                NightmarePlantera.AIPhases.Sleeping_P1 => NPSleepingP1State.Commit(ctx),
                NightmarePlantera.AIPhases.Nightemare_P3 => NPNightmareP3State.Commit(ctx),
                NightmarePlantera.AIPhases.Dream_P2 => Create(NightmarePlanteraStateId.dream_P2),
                _ => Create(ctx.Boss.ResumeStateId()),
            };

        #endregion

        #region 阶段每帧公共件

        /// <summary>三阶段每招开头：天空续命、补拖尾、推帧动画，并按需摆好触手目标点。</summary>
        protected static void P3Begin(NightmarePlanteraContext ctx, bool setTentacle = true)
        {
            NightmarePlantera boss = ctx.Boss;

            // 触手要在服务端也存在：下面的 NormallySetTentacle / ResetTentaclesTo 两端都会跑，
            // 旧代码只在非服务端建它，专用服务器上一进三阶段就会空引用。贴图为 null 无所谓，服务端不绘制。
            boss.EnsureRotateTentacles();

            if (!VaultUtils.isServer)
            {
                ((NightmareSky)SkyManager.Instance["NightmareSky"]).Timeleft = NightmarePlanteraDirector.SkyTimeleft;
                boss.UpdateFrame_P3();
            }

            if (setTentacle)
            {
                boss.NormallySetTentacle();
            }
        }

        /// <summary>三阶段每招结尾：推进触手模拟。</summary>
        protected static void P3End(NightmarePlanteraContext ctx) => ctx.Boss.NormallyUpdateTentacle();

        /// <summary>
        /// 三阶段每招起手的表现复位（旧 <c>SetPhase3States</c> 的双端那一半，Phase3_Nightemare.cs:1486-1496）。<br/>
        /// 必须在 <c>OnEnter</c> 里做而不是在提交口里做：客户端不选招，它是被 NetSync 换过来的，只有 <c>OnEnter</c> 两端都会跑。<br/>
        /// <c>dontTakeDamage</c> 不在这里写——它已经是每帧声明的通道（<see cref="NightmarePlanteraContext.Invulnerable"/>）。
        /// </summary>
        protected static void ApplyPhase3Presentation(NightmarePlanteraContext ctx)
        {
            NightmarePlantera boss = ctx.Boss;
            boss.warpScale = 0;
            boss.canDrawWarp = false;
            boss.useDreamMove = false;
            boss.tentacleColor = NightmarePlantera.nightmareRed;
            boss.DreamMoveCount = 0;
            boss.fantasyKillCount = 0;
            boss.tentacleStarFrame = 1;
            boss.alpha = 1;
            boss.EXai1 = 0;
        }

        #endregion

        #region 运动公共件

        /// <summary>
        /// 绕玩家公转：锚点在玩家外侧 distance、每 rollingFactor 帧转一圈，本体朝锚点追。<br/>
        /// 与旧 <c>CircleMovement</c> 同一套公式，只是把 <c>Timer</c> 变成显式入参。
        /// </summary>
        protected static void CircleMovement(NightmarePlanteraContext ctx, int timer, float distance, float speedMax,
            float accelFactor = 0.25f, float rollingFactor = 360f, float angleFactor = 0.08f, float baseRot = 0f)
        {
            NPC npc = ctx.Npc;
            Vector2 center = ctx.TargetCenter + ((baseRot + (timer / rollingFactor * MathHelper.TwoPi)).ToRotationVector2() * distance);
            Vector2 dir = center - npc.Center;

            float velRot = npc.velocity.ToRotation();
            float targetRot = dir.ToRotation();
            float speed = npc.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 800f, 0, 1) * speedMax;

            npc.velocity = velRot.AngleTowards(targetRot, angleFactor).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, accelFactor);
        }

        /// <summary>爪击移动：左右各一侧的半圆锚点 + 正弦摆动。与旧 <c>HookSlashMovement</c> 一致，side 取代旧的 <c>SonState - 1</c>。</summary>
        protected static void HookSlashMovement(NightmarePlanteraContext ctx, int timer, float side)
        {
            NPC npc = ctx.Npc;
            ctx.Boss.DoRotation(0.3f);

            float angle = (side * MathHelper.Pi) + (MathHelper.PiOver4 * MathF.Sin(timer * 0.0314f));
            Vector2 center = ctx.TargetCenter + (angle.ToRotationVector2() * 250);
            Vector2 dir = center - npc.Center;

            float velRot = npc.velocity.ToRotation();
            float targetRot = dir.ToRotation();
            float speed = npc.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 800f, 0, 1) * 24;

            npc.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);
        }

        /// <summary>
        /// 扑咬位移：离得远就朝头朝向加速，近了就刹车。三阶段全部咬击共用。<br/>
        /// <paramref name="rotateStep"/> 是头转向玩家的速率，<paramref name="velocityTurn"/> 是速度矢量转向头朝向的速率；
        /// 三阶段的咬击两者都是 0.3，秒杀处决的头只转 0.1（预告更长），所以要能分开给。
        /// </summary>
        protected static void LungeToTarget(NightmarePlanteraContext ctx, float rotateStep, float distance, float accel, float maxSpeed, float damp,
            float? velocityTurn = null)
        {
            NPC npc = ctx.Npc;
            Vector2 pos = ctx.TargetCenter;
            npc.rotation = npc.rotation.AngleTowards((pos - npc.Center).ToRotation(), rotateStep);

            if (Vector2.Distance(npc.Center, pos) > distance)
            {
                float speed = npc.velocity.Length() + accel;
                if (speed > maxSpeed)
                {
                    speed = maxSpeed;
                }

                npc.velocity = npc.velocity.ToRotation().AngleTowards(npc.rotation, velocityTurn ?? rotateStep).ToRotationVector2() * speed;
            }
            else
            {
                npc.velocity *= damp;
            }
        }

        #endregion

        #region 瞬移淡出

        /// <summary>
        /// 淡出 → 瞬移。返回 true 表示本帧刚完成瞬移，调用方随即换拍。<br/>
        /// 内部的 <c>AttackRandom</c> 抽取两端都要跑，所以这个方法只能从 <c>SharedUpdate</c> 里调。
        /// </summary>
        protected bool FadeTick(NightmarePlanteraContext ctx, int fadeTime, Color dustColor, Color fogColor,
            Func<Vector2> teleportPos, Action onTeleport = null, Action postTeleport = null)
        {
            NightmarePlantera boss = ctx.Boss;
            NPC npc = ctx.Npc;

            if (Timer == 1)
            {
                SoundEngine.PlaySound(CoraliteSoundID.ShieldDestroyed_NPCDeath58, npc.Center);
            }

            if (boss.alpha > 0)
            {
                boss.alpha -= 1 / (float)fadeTime;
                if (boss.alpha < 0)
                {
                    boss.alpha = 0;
                }
            }

            boss.DoRotation(0.1f);
            Vector2 away = (npc.Center - ctx.TargetCenter).SafeNormalize(Vector2.Zero);
            if (npc.velocity.Length() < 8)
            {
                npc.velocity += away * 0.15f;
            }

            boss.canDrawWarp = true;
            boss.warpScale = MathF.Sin(Timer / (float)fadeTime * MathHelper.Pi) * 2f;

            if (Timer == fadeTime * 3 / 4)
            {
                for (int i = 0; i < NightmarePlanteraDirector.P3FadeDustCount; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    Dust dust = Dust.NewDustPerfect(npc.Center + (dir * boss.AttackRandom.Next(0, 64)), DustType<NightmareStar>(),
                        dir * boss.NextAttackFloat(2f, 6f), newColor: dustColor, Scale: boss.NextAttackFloat(1f, 4f));
                    dust.rotation = dir.ToRotation() + MathHelper.PiOver2;
                }

                Helper.PlayPitched(CoraliteSoundID.NoUse_SuperMagicShoot_Item68, npc.Center, pitch: -1f);
            }

            if (Timer <= fadeTime)
            {
                return false;
            }

            npc.velocity *= 0;
            boss.alpha = 1;
            boss.canDrawWarp = false;
            boss.warpScale = 0;
            npc.rotation = (ctx.TargetCenter - npc.Center).ToRotation();
            onTeleport?.Invoke();

            npc.Center = teleportPos();

            for (int i = 0; i < NightmarePlanteraDirector.P3TeleportFogCount; i++)
            {
                Color color = boss.AttackRandom.Next(0, 2) switch
                {
                    0 => new Color(110, 68, 200),
                    _ => fogColor
                };

                PRTLoader.NewParticle(npc.Center + Main.rand.NextVector2Circular(32, 32), Helper.NextVec2Dir(6, 10f),
                    CoraliteContent.ParticleType<BigFog>(), color, Scale: boss.NextAttackFloat(0.5f, 1.5f));
            }

            for (int i = 0; i < NightmarePlanteraDirector.P3FadeDustCount; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust dust = Dust.NewDustPerfect(npc.Center + (dir * boss.AttackRandom.Next(0, 64)), DustType<NightmareStar>(),
                    dir * boss.NextAttackFloat(4f, 10f), newColor: dustColor, Scale: boss.NextAttackFloat(1f, 4f));
                dust.rotation = dir.ToRotation() + MathHelper.PiOver2;

                dir = Helper.NextVec2Dir();
                Dust.NewDustPerfect(npc.Center + (dir * boss.AttackRandom.Next(0, 64)), DustID.VilePowder,
                    dir * boss.NextAttackFloat(2f, 6f), newColor: dustColor, Scale: boss.NextAttackFloat(1f, 1.3f));
            }

            postTeleport?.Invoke();
            boss.ResetTentaclesTo(npc.Center, npc.rotation);
            return true;
        }

        /// <summary>三阶段配色的淡出瞬移（红）。</summary>
        protected bool FadeTickP3(NightmarePlanteraContext ctx, Func<Vector2> teleportPos, int fadeTime = NightmarePlanteraDirector.P3FadeFrames,
            Action onTeleport = null, Action postTeleport = null)
            => FadeTick(ctx, fadeTime, NightmarePlantera.nightmareRed, NightmarePlantera.nightmareRed, teleportPos, onTeleport, postTeleport);

        /// <summary>二阶段配色的淡出瞬移（紫）。沿用旧 <c>Phase2Fade</c> 的两种颜色。</summary>
        protected bool FadeTickP2(NightmarePlanteraContext ctx, Func<Vector2> teleportPos, int fadeTime = 45,
            Action onTeleport = null, Action postTeleport = null)
            => FadeTick(ctx, fadeTime, new Color(153, 88, 156, 230), new Color(122, 110, 134), teleportPos, onTeleport, postTeleport);

        #endregion

        #region 表现小件

        /// <summary>震屏（纯本地）。</summary>
        protected static void Shake(NightmarePlanteraContext ctx, Vector2 direction, float strength, float vibration, int frames)
        {
            if (Main.dedServ)
            {
                return;
            }

            PunchCameraModifier modifier = new PunchCameraModifier(ctx.Npc.Center, direction, strength, vibration, frames, 1000);
            Main.instance.CameraModifiers.Add(modifier);
        }

        #endregion
    }
}
