using Terraria;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// <see cref="Initialize"/> 在preai中初始化。
    /// </summary>
    public abstract class CoraliteBossHostileProj : ModProjectile
    {
        private bool _initialized;

        /// <summary>生成时传入的 player owner</summary>
        protected Player Owner => Main.player[Projectile.owner];

        /// <summary>弹幕生成后调用一次</summary>
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
