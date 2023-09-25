using Terraria;

namespace Coralite.Core
{
    public interface IExtensionCoating
    {
        /// <summary>
        /// 是否能将物品提取到物品容器中
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool CanExtractItem(Item item);
    }
}
