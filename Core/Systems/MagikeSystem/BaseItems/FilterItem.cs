using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class FilterItem : ModItem
    {
        /// <summary>
        /// 获取滤镜组件
        /// </summary>
        /// <returns></returns>
        public abstract MagikeFilter GetFilterComponent();
    }
}
