using System.Collections.Generic;
using System.Linq;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    // 貌似一大堆特判，写的并不是太好，但是能实现就行了 暂时也不想管那么多
    public class EnchantEntityPool
    {
        //一个附魔对象池，内部包含一堆附魔
        private readonly List<EnchantData> _datas;

        public EnchantEntityPool()
        {
            _datas = new List<EnchantData>();
        }

        public EnchantEntityPool AddBasicBonus(EnchantData newEnchant)
        {
            _datas.Add(newEnchant);
            return this;
        }

        public IEnumerable<EnchantData> GetPool(int whichSlot,int level)
        {
            return from d in _datas
                   where d.whichSlot == whichSlot && d.level == level
                   select d;
        }
    }
}
