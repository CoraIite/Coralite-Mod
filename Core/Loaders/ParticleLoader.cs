using Coralite.Core.Systems.ParticleSystem;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    /// <summary>
    /// 基本全抄的源码
    /// </summary>
    public class ParticleLoader
    {
        internal static IList<ModParticle> modParticles;
        internal static int ParticleCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取粒子
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModParticle GetParticle(int type)
                 => type < ParticleCount ? modParticles[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveParticleID() => ParticleCount++;

        /// <summary>
        /// 粒子生成时执行
        /// </summary>
        /// <param name="particle"></param>
        internal static void SetupParticle(Particle particle)
        {
            ModParticle modParticle = GetParticle(particle.type);

            if (modParticle != null)
                modParticle.OnSpawn(particle);
        }

        internal static void Unload()
        {
            foreach (var item in modParticles)
            {
                item.Unload();
            }

            modParticles.Clear();
            modParticles = null;
            ParticleCount = 0;
        }
    }
}
