using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.ParticleSystem;

namespace Coralite.Core
{
    public static class CoraliteContent
    {
        /// <summary>
        /// 根据类型获取这个粒子的ID（type）。假设一个类一个实例。
        /// </summary>
        public static int ParticleType<T>() where T : Particle => ModContent.GetInstance<T>()?.Type ?? 0;

        public static int FairyType<T>() where T : Fairy => ModContent.GetInstance<T>()?.Type ?? 0;

    }
}
