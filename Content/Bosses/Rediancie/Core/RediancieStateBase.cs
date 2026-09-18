using Coralite.Content.Bosses.Rediancie.States;
using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie.Core
{
    /// <summary>
    /// 状态索引，写入 <c>npc.ai[0]</c> 同步。成员与数值沿用旧 <c>Rediancie.AIStates</c>（线格式不变），追加 <see cref="hub"/>。
    /// </summary>
    internal enum RediancieStateId : int
    {
        onSpawnAnim = 0,
        onKillAnim = 1,
        /// <summary>赤色脉冲</summary>
        pulse = 2,
        /// <summary>赤玉烟花</summary>
        firework = 3,
        /// <summary>蓄力大爆炸</summary>
        accumulate = 4,
        /// <summary>赤色爆冲</summary>
        dash = 5,
        /// <summary>三连炸</summary>
        explosion = 6,
        /// <summary>赤玉雨</summary>
        upShoot = 7,
        /// <summary>赤玉激光</summary>
        magicShoot = 8,
        /// <summary>召唤小赤玉灵</summary>
        summon = 9,
        /// <summary>下砸</summary>
        slamDown = 10,
        /// <summary>连接段 + 唯一提交口（新增；<see cref="RediancieDirector.HubFrames"/> 为 0 时通常不驻留）</summary>
        hub = 11,
    }

    /// <summary>
    /// 赤玉灵状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——写运动 / 朝向 / 无敌声明、推进确定性子拍、弹药环绕几何、客户端粒子音效。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕 / 召唤 / 掷骰，返回下一状态；本类把死亡请求与超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>，出招一律经 <see cref="RediancieHubState.Commit"/>；招式体内不得出现 <c>ChangeState</c>。
    /// </summary>
    internal abstract class RediancieStateBase : CoraliteBossState<RediancieContext>
    {
        public abstract RediancieStateId StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>超时兜底帧数；演出态可覆盖为更大值。</summary>
        protected virtual int TimeoutFrames => RediancieDirector.StateTimeoutFrames;

        protected sealed override IVaultState<RediancieContext> ServerUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx)
        {
            // CheckDead 只登记请求，换态统一从这里经返回值走（客户端读 ai[0] 跟随）。
            if (ctx.KillRequested && StateIndex != RediancieStateId.onKillAnim)
            {
                return Create(RediancieStateId.onKillAnim);
            }

            // 超时兜底：状态机永远不许死在一个状态里；收招不留残速。
            if (Counter++ > TimeoutFrames)
            {
                ctx.Npc.velocity *= 0.6f;
                return EndAttack(ctx);
            }

            return AuthorityUpdate(machine, ctx);
        }

        /// <summary>仅权威端：弹幕生成、召唤、掷骰；返回下一状态或 null。</summary>
        protected abstract IVaultState<RediancieContext> AuthorityUpdate(VaultStateMachine<RediancieContext> machine, RediancieContext ctx);

        #region 公共小件

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        protected static IVaultState<RediancieContext> Create(RediancieStateId id)
            => VaultStateRegistry<RediancieContext>.Create((int)id);

        /// <summary>收招：经 hub 的唯一提交口选下一招（<see cref="RediancieDirector.HubFrames"/> = 0 时不多占一帧）。</summary>
        protected static IVaultState<RediancieContext> EndAttack(RediancieContext ctx)
            => RediancieHubState.EndAttack(ctx);

        /// <summary>是否比玩家高出下砸门槛。</summary>
        protected static bool AboveTarget(RediancieContext ctx)
            => ctx.Npc.Center.Y < ctx.Target.Center.Y - RediancieDirector.SlamHeightMargin;

        /// <summary>本体前方 9 帧位移处（爆炸出生点）。</summary>
        protected static Vector2 Ahead(RediancieContext ctx)
            => ctx.Npc.Center + (ctx.Npc.velocity * RediancieDirector.AheadFrames);

        /// <summary>震屏（纯本地）。</summary>
        protected static void Shake(RediancieContext ctx, int strength, float vibration, int frames)
        {
            if (Main.dedServ)
            {
                return;
            }

            PunchCameraModifier modifier = new PunchCameraModifier(ctx.Npc.Center, Main.rand.NextVector2CircularEdge(1, 1),
                strength, vibration, frames, RediancieDirector.ShakeFalloffDistance);
            Main.instance.CameraModifiers.Add(modifier);
        }

        /// <summary>蓄力宝石尘：count 粒、散布 count×3、缩放 1 + count×gain（出生 / 蓄力 / 三连炸共用，纯本地）。</summary>
        protected static void ChargeDust(RediancieContext ctx, int count, float scaleGain)
        {
            if (Main.dedServ)
            {
                return;
            }

            float spread = count * RediancieDirector.ChargeDustSpreadPerCount;
            for (int i = 0; i < count; i++)
            {
                Dust dust = Dust.NewDustPerfect(ctx.Npc.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(spread, spread),
                    DustID.GemRuby, Vector2.Zero, 0, default, 1f + (count * scaleGain));
                dust.noGravity = true;
            }
        }

        /// <summary>赤玉大爆炸弹幕（仅权威端调用）。</summary>
        protected static void SpawnBigBoom(RediancieContext ctx, Vector2 position, int damage, float knockback)
        {
            Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), position, Vector2.Zero,
                ModContent.ProjectileType<Rediancie_BigBoom>(), damage, knockback);
        }

        #endregion
    }
}
