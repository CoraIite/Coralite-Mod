using System;

namespace Coralite.Core.Loaders
{
    public interface IReflactionLoader
    {
        /// <summary>
        /// 在加载之前调用
        /// </summary>
        /// <param name="Mod"></param>
        void PreLoad(Mod Mod) { }

        void Load(Mod Mod, Type type) { }

        void PostLoad(Mod Mod) { }

        void PreSetUp(Mod Mod) { }

        void SetUp(Mod Mod, Type type) { }

        void PostSetUp(Mod Mod) { }

        void PreUnload(Mod Mod) { }
        void Unload(Mod Mod, Type type) { }

        int Priority { get; }
        LoadSide Side { get; }

        public enum LoadSide
        {
            Cilent,
            All
        }
    }
}
