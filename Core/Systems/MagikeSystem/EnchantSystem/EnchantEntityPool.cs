using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    // 貌似一大堆特判，写的并不是太好，但是能实现就行了 暂时也不想管那么多
    public class EnchantEntityPool
    {
        //一个附魔对象池，内部包含一堆附魔
        private readonly List<BasicBonusEnchant> _basicData;
        private readonly List<OtherBonusEnchant> _othersData;
        private readonly List<SpecialEnchant> _specialData;

        public EnchantEntityPool()
        {
            _basicData = new List<BasicBonusEnchant>();
            _othersData = new List<OtherBonusEnchant>();
            _specialData = new List<SpecialEnchant>();
        }

        public EnchantEntityPool AddBasicBonus(BasicBonusEnchant newEnchant)
        {
            _basicData.Add(newEnchant);
            return this;
        }

        public float GetAllChance(int whichSlot)
        {
            float all = 0;
            switch (whichSlot)
            {
                default:
                case 0: //基础
                    foreach (var item in _basicData)
                        all += item.EnchantChance;
                    break;
                case 1: //其他
                    foreach (var item in _othersData)
                        all += item.EnchantChance;
                    break;
                case 2: //特殊
                    foreach (var item in _specialData)
                        all += item.EnchantChance;
                    break;
            }
            return all;
        }
    }
}
