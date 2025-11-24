namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public interface ICrystalCluster
    {
        /// <summary>
        /// 这个晶簇的等级
        /// </summary>
        ushort Level { get; }

        /// <summary>
        /// 这个晶簇的物品类型
        /// </summary>
        int ItemType { get; }
        int ItemStack { get; }

        int MagikeCost { get; }
    }
}
