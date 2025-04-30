namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        /// <summary>
        /// 梦魇花战斗中的受击次数
        /// </summary>
        public int nightmareCount;
        /// <summary> 噩梦能量 </summary>
        public short nightmareEnergy;
        /// <summary> 噩梦能量上限 </summary>
        public short nightmareEnergyMax;

        /// <summary>
        /// 限制噩梦能量数量
        /// </summary>
        private void LimitNightmareEnergy()
        {
            if (nightmareEnergy > nightmareEnergyMax)
                nightmareEnergy = nightmareEnergyMax;
        }

        /// <summary>
        /// 获取噩梦能量
        /// </summary>
        /// <param name="howMany"></param>
        public void GetNightmareEnergy(short howMany)
        {
            nightmareEnergy += howMany;
            if (nightmareEnergy > nightmareEnergyMax)
                nightmareEnergy = nightmareEnergyMax;
        }

        /// <summary>
        /// 重置噩梦能量最大值
        /// </summary>
        private void ResetNightmareEnergy()
        {
            nightmareEnergyMax = 7;
        }

        /// <summary>
        /// 复活时清除噩梦效果
        /// </summary>
        private void ResetNightmare_Respawn()
        {
            nightmareCount = 0;
            nightmareEnergy = 0;
        }
    }
}
