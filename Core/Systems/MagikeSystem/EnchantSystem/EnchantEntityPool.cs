using System.Collections.Generic;
using System.Linq;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
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

        public IEnumerable<EnchantData> GetPool(int whichSlot, Enchant.Level level)
        {
            return from d in _datas
                   where d.whichSlot == whichSlot && d.level == level
                   select d;
        }
    }
}
