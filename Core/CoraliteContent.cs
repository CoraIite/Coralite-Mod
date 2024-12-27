using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.KeySystem;
using Coralite.Core.Systems.MTBStructure;
using InnoVault.PRT;

namespace Coralite.Core
{
    public static class CoraliteContent
    {
        /// <summary>
        /// 根据类型获取这个粒子的ID（type）。假设一个类一个实例。
        /// </summary>
        public static int ParticleType<T>() where T : Particle => PRTLoader.GetParticleID<T>();

        public static int FairyType<T>() where T : Fairy => ModContent.GetInstance<T>()?.Type ?? 0;

        public static int MTBSType<T>() where T : MultiblockStructure => ModContent.GetInstance<T>()?.Type ?? 0;

        public static MultiblockStructure GetMTBS<T>() where T : MultiblockStructure => MTBStructureLoader.GetMTBStructure(ModContent.GetInstance<T>()?.Type ?? 0);

        public static KeyKnowledge GetKKnowledge(int ID)
            => KeyKnowledgeLoader.GetKeyKnowledge(ID);
    }
}
