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

        public EnchantEntityPool AddBonus(EnchantData newEnchant)
        {
            _datas.Add(newEnchant);
            return this;
        }

        public IEnumerable<EnchantData> GetSonPool(int whichSlot, Enchant.Level level)
        {
            return from d in _datas
                   where d.whichSlot == whichSlot && d.level == level
                   select d;
        }

        public IEnumerable<T> Find<T>(int whichSlot, Enchant.Level level) where T : EnchantData
        {
            return from d in _datas
                   where d.whichSlot == whichSlot && d.level == level && d is T
                   select d as T;
        }

        /// <summary>
        /// 由于想不到什么好方法索性就全部移除了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whichSlot"></param>
        /// <param name="level"></param>
        public void FindAndRemoveAll<T>(int whichSlot, Enchant.Level level) where T : EnchantData
        {
            IEnumerable<T> values = from d in _datas
                                    where d.whichSlot == whichSlot && d.level == level && d is T
                                    select d as T;

            T[] arr = values.ToArray();

            for (int i = 0; i < arr.Length; i++)
            {
                _datas.Remove(arr[i]);
            }
        }

        public void FindAndRemoveAll(int whichSlot, Enchant.Level level)
        {
            IEnumerable<EnchantData> values = from d in _datas
                                              where d.whichSlot == whichSlot && d.level == level
                                              select d;

            EnchantData[] arr = values.ToArray();

            for (int i = 0; i < arr.Length; i++)
            {
                _datas.Remove(arr[i]);
            }
        }

        /// <summary>
        /// 由于想不到什么好方法索性就全部移除了
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whichSlot"></param>
        public void FindAndRemoveAll<T>(int whichSlot) where T : EnchantData
        {
            IEnumerable<T> values = from d in _datas
                                    where d.whichSlot == whichSlot && d is T
                                    select d as T;

            T[] arr = values.ToArray();

            for (int i = 0; i < arr.Length; i++)
            {
                _datas.Remove(arr[i]);
            }
        }



        public EnchantEntityPool Clone()
        {
            EnchantEntityPool pool = new EnchantEntityPool();
            pool._datas.AddRange(_datas);

            return pool;
        }
    }
}
