﻿using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
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

        public static FairyCircleCore FairyCatcherCoreType<T>() where T : FairyCircleCore
            => ModContent.GetInstance<T>();
        public static FairyCircleCore GetFairyCatcherCore<T>() where T : FairyCircleCore
            => ModContent.GetInstance<T>();
        public static FairyCircleCore GetFairyCatcherCore(int type)
            => FairyCircleCoreLoader.GetFairyCatcherCore(type);

        public static int MTBSType<T>() where T : Multiblock => ModContent.GetInstance<T>()?.Type ?? 0;

        public static Multiblock GetMTBS<T>() where T : Multiblock => MultiblockLoader.GetMTBStructure(ModContent.GetInstance<T>()?.Type ?? 0);

        public static KeyKnowledge GetKKnowledge(int ID)
            => KeyKnowledgeLoader.GetKeyKnowledge(ID);

        public static KeyKnowledge GetKKnowledge<T>() where T : KeyKnowledge
            => KeyKnowledgeLoader.GetKeyKnowledge(ModContent.GetInstance<T>()?.InnerType ?? 0);
    }
}
