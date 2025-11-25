namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public interface IUpgradeable
    {
        /// <summary>
        /// 执行初始化，需要在<see cref="InnoVault.TileProcessors.TileProcessor.Initialize"/>中调用
        /// <br></br>否则没用！
        /// </summary>
        void InitializeLevel();

        /// <summary>
        /// 请自行判断传入的等级
        /// </summary>
        /// <param name="incomeLevel"></param>
        /// <returns></returns>
        void Upgrade(ushort incomeLevel);

        /// <summary>
        /// 能否升级
        /// </summary>
        /// <param name="incomeLevel"></param>
        bool CanUpgrade(ushort incomeLevel);
    }

    public interface IUpgradeLoadable
    {
        int TileType {  get; }
    }
}
