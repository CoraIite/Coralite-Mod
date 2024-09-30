namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public interface IUpgradeable
    {
        /// <summary>
        /// 请自行判断传入的等级，返回<see langword="true"/>为能升级<br></br>
        /// 
        /// </summary>
        /// <param name="incomeLevel"></param>
        /// <returns></returns>
        public abstract void Upgrade(MALevel incomeLevel);

        /// <summary>
        /// 能否升级
        /// </summary>
        /// <param name="incomeLevel"></param>
        public abstract bool CanUpgrade(MALevel incomeLevel);
    }
}
