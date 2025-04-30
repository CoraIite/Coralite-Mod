namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        /// <summary>
        /// 血印礼帽血池的血量
        /// </summary>
        public int bloodPoolCount;
        /// <summary>
        /// 血印礼帽血池的血量上限
        /// </summary>
        public const int BloodPoolCountMax = 50;

        /// <summary> 血印礼帽影礼帽增益倒计时 </summary>
        public short shadowBonusTime;
        /// <summary> 血印礼帽影礼帽攻击增益 </summary>
        public byte shadowAttackBonus;
        /// <summary> 血印礼帽影礼帽生命上限增益 </summary>
        public byte shadowLifeMaxBonus;
        /// <summary> 血印礼帽影礼帽防御增益 </summary>
        public byte shadowDefenceBonus;

        public void CheckBloodPool()
        {
            if (bloodPoolCount > BloodPoolCountMax)
                bloodPoolCount = BloodPoolCountMax;
        }

        /// <summary>
        /// 血印礼帽添加血池
        /// </summary>
        /// <param name="howMany"></param>
        public void GetBloodPool(int howMany)
        {
            bloodPoolCount += howMany;
            if (bloodPoolCount > BloodPoolCountMax)
                bloodPoolCount = BloodPoolCountMax;
        }

        /// <summary>
        /// 更新血印礼帽影礼帽增益
        /// </summary>
        public void UpdateShadowTopper()
        {
            shadowBonusTime++;
            if (shadowBonusTime > 60 * 12)
            {
                shadowBonusTime = 0;
                if (shadowAttackBonus > 0)
                    shadowAttackBonus--;
                if (shadowLifeMaxBonus > 0)
                    shadowLifeMaxBonus--;
                if (shadowDefenceBonus > 0)
                    shadowDefenceBonus--;
            }
        }

        /// <summary>
        /// 重置血印礼帽影礼帽增益倒计时
        /// </summary>
        public void ResetShadowTopperTime()
            => shadowBonusTime = 0;

        public void GetShadowAttackBonus(byte howMany = 1)
        {
            shadowAttackBonus += howMany;
            if (shadowAttackBonus < 5)
                shadowAttackBonus = 5;
        }

        public void GetShadowLifeMaxBonus(byte howMany = 1)
        {
            shadowLifeMaxBonus += howMany;
            if (shadowLifeMaxBonus < 5)
                shadowLifeMaxBonus = 5;
        }

        public void GetshadowDefenceBonus(byte howMany = 1)
        {
            shadowDefenceBonus += howMany;
            if (shadowDefenceBonus < 5)
                shadowDefenceBonus = 5;
        }
    }
}
