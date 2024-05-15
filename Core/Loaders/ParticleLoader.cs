using Coralite.Core.Systems.ParticleSystem;
using System.Collections.Generic;

namespace Coralite.Core.Loaders
{
    /// <summary>
    /// 基本全抄的源码
    /// </summary>
    public class ParticleLoader
    {
        internal static IList<Particle> Particles;
        internal static int ParticleCount { get; private set; } = 0;

        /// <summary>
        /// 根据类型获取粒子
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Particle GetParticle(int type)
                 => type < ParticleCount ? Particles[type] : null;

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <returns></returns>
        public static int ReserveParticleID() => ParticleCount++;

        internal static void Unload()
        {
            foreach (var item in Particles)
            {
                item.Unload();
            }

            Particles.Clear();
            Particles = null;
            ParticleCount = 0;
        }
    }
}
