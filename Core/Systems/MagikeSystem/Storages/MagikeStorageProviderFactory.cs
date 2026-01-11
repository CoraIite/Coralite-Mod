using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using InnoVault.Storages;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Storages
{
    /// <summary>
    /// 魔能存储提供者工厂
    /// 用于创建魔能物品容器的存储提供者
    /// </summary>
    public class MagikeStorageProviderFactory : IStorageProviderFactory
    {
        /// <summary>
        /// 工厂的唯一标识符
        /// </summary>
        public string Identifier => "Coralite.MagikeStorageFactory";

        /// <summary>
        /// 工厂优先级，魔能系统的优先级设为100
        /// </summary>
        public int Priority => 100;

        /// <summary>
        /// 检查此工厂是否可用
        /// </summary>
        public bool IsAvailable => ModLoader.HasMod("Coralite");

        /// <summary>
        /// 在指定范围内查找存储目标
        /// </summary>
        public IEnumerable<IStorageProvider> FindStorageProviders(Point16 position, int range, Item item)
        {
            //计算搜索范围（转换为物块坐标）
            int tileRange = range / 16;
            int minX = position.X - tileRange;
            int maxX = position.X + tileRange;
            int minY = position.Y - tileRange;
            int maxY = position.Y + tileRange;

            //用于记录已经处理过的位置，避免重复
            HashSet<Point16> processedPositions = [];

            //遍历范围内的所有物块位置
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    //尝试获取该位置的魔能实体
                    if (!MagikeHelper.TryGetEntity(x, y, out MagikeTP magikeTP))
                        continue;

                    //避免重复处理同一个实体
                    if (processedPositions.Contains(magikeTP.Position))
                        continue;

                    processedPositions.Add(magikeTP.Position);

                    //检查是否有物品容器组件
                    //优先返回可读写的ItemContainer
                    if (magikeTP.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                    {
                        var provider = new MagikeStorageProvider(magikeTP, container);
                        if (item == null || item.IsAir || provider.CanAcceptItem(item))
                            yield return provider;
                    }

                    //然后返回只读的GetOnlyItemContainer
                    if (magikeTP.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer getOnlyContainer))
                    {
                        yield return new MagikeGetOnlyStorageProvider(magikeTP, getOnlyContainer);
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定位置的存储提供者
        /// </summary>
        public IStorageProvider GetStorageProviders(Point16 position, Item item)
        {
            //尝试获取该位置的魔能实体
            if (!MagikeHelper.TryGetEntity(position.X, position.Y, out MagikeTP magikeTP))
                return null;

            //优先返回可读写的ItemContainer
            if (magikeTP.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
            {
                var provider = new MagikeStorageProvider(magikeTP, container);
                if (item == null || item.IsAir || provider.CanAcceptItem(item))
                    return provider;
            }

            //然后返回只读的GetOnlyItemContainer
            if (magikeTP.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer getOnlyContainer))
            {
                return new MagikeGetOnlyStorageProvider(magikeTP, getOnlyContainer);
            }

            return null;
        }
    }
}
