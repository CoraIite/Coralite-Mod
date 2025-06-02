using Coralite.Core.Loaders;
using System.Collections.Generic;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class FairyCircleCore : ModTexturedType
    {
        public int Type { get; internal set; }

        public override string Texture => AssetDirectory.FairyCircleCore + Name;

        /// <summary>
        /// 边缘线的颜色
        /// </summary>
        public virtual Color? EdgeColor { get => null; }
        /// <summary>
        /// 内部的颜色
        /// </summary>
        public virtual Color? InnerColor { get=> null; }

        protected override void Register()
        {
            ModTypeLookup<FairyCircleCore>.Register(this);

            FairyCircleCoreLoader.cores ??= new List<FairyCircleCore>();
            FairyCircleCoreLoader.cores.Add(this);

            Type = FairyCircleCoreLoader.ReserveID();
        }
    }
}
