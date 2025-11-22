using Coralite.Core.Loaders;
using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem
{
    /// <summary>
    /// 魔能等级，
    /// </summary>
    public abstract class MagikeLevel : ModType
    {
        /// <summary>
        /// 内部ID
        /// </summary>
        public ushort Type { get; private set; }

        /// <summary>
        /// 当前等级是否可用
        /// </summary>
        public abstract bool Available { get; }

        /// <summary>
        /// 偏振滤镜物品ID
        /// </summary>
        public virtual int PolarizedFilterItemType { get => -1; }

        /// <summary>
        /// 在计算魔能合成表消耗的时候使用的值
        /// </summary>
        public abstract float MagikeCostValue { get; }

        protected sealed override void Register()
        {
            ModTypeLookup<MagikeLevel>.Register(this);

            MagikeLoader.levels ??= new List<MagikeLevel>();
            MagikeLoader.levels.Add(this);

            Type = MagikeLoader.ReserveMagikeLevelID();
        }
    }
}
