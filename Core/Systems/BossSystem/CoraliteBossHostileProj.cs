using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Boss 敌对弹幕基座：不继承 <see cref="InnoVault.GameContent.BaseEntity.BaseHeldProj"/>。<br/>
    /// BaseHeldProj 假定 <c>Projectile.owner</c> 为玩家，并在 PostAI 中 owner 无效时 Kill 弹幕；
    /// Boss 敌对弹幕的 owner 常为 <c>npc.target</c> 或 -1，且生命周期由 ai 槽中的 Boss 索引驱动，不能走手持弹幕逻辑。<br/>
    /// 仍保留 BaseHeldProj 的 <see cref="Initialize"/> 一次性初始化钩子（由 PreAI 触发）。
    /// </summary>
    public abstract class CoraliteBossHostileProj : ModProjectile
    {
        private bool _initialized;

        /// <summary>生成时传入的 player owner（常为 <c>npc.target</c>），仅作坐标参考，不做存活绑定。</summary>
        protected Player Owner => Main.player[Projectile.owner];

        /// <summary>弹幕生成后调用一次，等价于 BaseHeldProj.Initialize。</summary>
        public virtual void Initialize()
        {
        }

        public sealed override bool PreAI()
        {
            if (!_initialized)
            {
                Initialize();
                _initialized = true;
            }

            return true;
        }
    }
}
