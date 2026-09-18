using Coralite.Content.Bosses.ModReinforce.Bloodiancie.States;
using Coralite.Core.Systems.BossSystem;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie.Core
{
    /// <summary>
    /// 状态索引，写入 <c>npc.ai[0]</c> 同步。成员与数值沿用旧 <c>Bloodiancie.AIStates</c>（线格式不变），追加 <see cref="hub"/>。
    /// </summary>
    internal enum BloodiancieStateId : int
    {
        onSpawnAnim = 0,
        onKillAnim = 1,
        /// <summary>赤色脉冲</summary>
        pulse = 2,
        /// <summary>生成血球并爆出血雨</summary>
        bloodRain = 3,
        /// <summary>烟花攻击</summary>
        firework = 4,
        /// <summary>追踪玩家后横向爆炸多次</summary>
        explosionHorizontally = 5,
        /// <summary>赤色爆冲</summary>
        dash = 6,
        /// <summary>多段爆炸</summary>
        explosion = 7,
        /// <summary>向上射击</summary>
        upShoot = 8,
        /// <summary>射出会爆炸的炸弹</summary>
        shootBomb = 9,
        /// <summary>赤玉激光</summary>
        magicShoot = 10,
        /// <summary>召唤小血玉灵</summary>
        summon = 11,
        /// <summary>连接段 + 唯一提交口（新增；<see cref="BloodiancieDirector.HubFrames"/> 为 0 时通常不驻留）</summary>
        hub = 12,
    }

    /// <summary>
    /// 赤血玉灵状态基类。<br/>
    /// · <c>SharedUpdate</c>：两端同跑——写运动 / 朝向 / 无敌声明、推进确定性子拍、弹药环绕几何、客户端粒子音效。<br/>
    /// · <see cref="AuthorityUpdate"/>：仅权威端——弹幕 / 召唤 / 掷骰，返回下一状态；本类把死亡请求与超时兜底垫在它前面。<br/>
    /// · 收招一律 <see cref="EndAttack"/>，出招一律经 <see cref="BloodiancieHubState.Commit"/>；招式体内不得出现 <c>ChangeState</c>。<br/>
    /// · <c>OnEnter</c> 保持基座实现（ai[1..3] 已让位给基座约定，不再需要旧的空 OnEnter）。
    /// </summary>
    internal abstract class BloodiancieStateBase : CoraliteBossState<BloodiancieContext>
    {
        public abstract BloodiancieStateId StateIndex { get; }

        public override int StateId => (int)StateIndex;

        /// <summary>超时兜底帧数；演出态可覆盖为更大值。</summary>
        protected virtual int TimeoutFrames => BloodiancieDirector.StateTimeoutFrames;

        protected sealed override IVaultState<BloodiancieContext> ServerUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx)
        {
            // CheckDead 只登记请求，换态统一从这里经返回值走（客户端读 ai[0] 跟随）。
            if (ctx.KillRequested && StateIndex != BloodiancieStateId.onKillAnim)
            {
                return Create(BloodiancieStateId.onKillAnim);
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
        protected abstract IVaultState<BloodiancieContext> AuthorityUpdate(VaultStateMachine<BloodiancieContext> machine, BloodiancieContext ctx);

        #region 公共小件

        /// <summary>按 id 建状态；未注册返回 null（调用方要兜底）。</summary>
        protected static IVaultState<BloodiancieContext> Create(BloodiancieStateId id)
            => VaultStateRegistry<BloodiancieContext>.Create((int)id);

        /// <summary>收招：经 hub 的唯一提交口选下一招（<see cref="BloodiancieDirector.HubFrames"/> = 0 时不多占一帧）。</summary>
        protected static IVaultState<BloodiancieContext> EndAttack(BloodiancieContext ctx)
            => BloodiancieHubState.EndAttack(ctx);

        /// <summary>本体前方 9 帧位移处（爆炸出生点）。</summary>
        protected static Vector2 Ahead(BloodiancieContext ctx)
            => ctx.Npc.Center + (ctx.Npc.velocity * BloodiancieDirector.AheadFrames);

        /// <summary>震屏（纯本地）。</summary>
        protected static void Shake(BloodiancieContext ctx, int strength, float vibration, int frames)
        {
            if (Main.dedServ)
            {
                return;
            }

            PunchCameraModifier modifier = new PunchCameraModifier(ctx.Npc.Center, Main.rand.NextVector2CircularEdge(1, 1),
                strength, vibration, frames, BloodiancieDirector.ShakeFalloffDistance);
            Main.instance.CameraModifiers.Add(modifier);
        }

        /// <summary>蓄力宝石尘：count 粒、散布 count×spreadPerCount、缩放 1 + count×scaleGain（各招共用，纯本地）。</summary>
        protected static void ChargeDust(BloodiancieContext ctx, int count, float scaleGain, float spreadPerCount = BloodiancieDirector.ChargeDustSpreadWide)
        {
            if (Main.dedServ)
            {
                return;
            }

            float spread = count * spreadPerCount;
            for (int i = 0; i < count; i++)
            {
                Dust dust = Dust.NewDustPerfect(ctx.Npc.Center + new Vector2(0, -16) + Main.rand.NextVector2Circular(spread, spread),
                    DustID.GemRuby, Vector2.Zero, 0, default, 1f + (count * scaleGain));
                dust.noGravity = true;
            }
        }

        /// <summary>血玉大爆炸弹幕（仅权威端调用）。</summary>
        protected static void SpawnBigBoom(BloodiancieContext ctx, Vector2 position, int damage, float knockback)
        {
            Projectile.NewProjectile(ctx.Npc.GetSource_FromThis(), position, Vector2.Zero,
                ModContent.ProjectileType<Bloodiancie_BigBoom>(), damage, knockback);
        }

        #endregion
    }
}
