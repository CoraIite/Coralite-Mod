using Coralite.Core.Loaders;
using System.Collections.Generic;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class FairyCatcherCore : ModTexturedType
    {
        public int Type { get; internal set; }

        public override string Texture => AssetDirectory.FairyCatcherCore + Name;

        /// <summary>
        /// 边缘线的颜色
        /// </summary>
        public abstract Color EdgeColor { get; }
        /// <summary>
        /// 内部的颜色
        /// </summary>
        public abstract Color InnerColor { get; }

        protected override void Register()
        {
            ModTypeLookup<FairyCatcherCore>.Register(this);

            FairyCatcherCoreLoader.cores ??= new List<FairyCatcherCore>();
            FairyCatcherCoreLoader.cores.Add(this);

            Type = FairyLoader.ReserveFairyID();
        }
    }
}
