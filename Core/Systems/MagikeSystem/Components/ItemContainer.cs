using Coralite.Core.Systems.MagikeSystem.TileEntities;
using System;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class ItemContainer : MagikeComponent
    {
        public override int ID => MagikeComponentID.ItemContainer;

        private int _capacityBase;

        /// <summary> 基础容量，不能小于1 </summary>
        public int CapacityBase
        {
            get => _capacityBase; 
            set
            {
                if (value < 1)
                    _capacityBase = 1;
                else
                    _capacityBase = value;
            }
        }

        /// <summary> 额外容量 </summary>
        public int CapacityExtra { get; set; }

        /// <summary> 容量 </summary>
        public int Capacity => CapacityBase + CapacityExtra;

        private Item[] _items;
        public Item[] Items => _items;

        /// <summary>
        /// 修改容量后必须调用这个方法！
        /// </summary>
        public void ResetCapacity()
        {
            Vector2 worldPos = (Entity as MagikeTileEntity).Position.ToWorldCoordinates();
            var source=new EntitySource_TileEntity(Entity as MagikeTileEntity);

            //超出容量的部分生成掉落物
            for (int i = Capacity; i < _items.Length; i++)
            {
                Item item = _items[i];
                if (item != null && !item.IsAir)
                    Item.NewItem(source, worldPos, item);
            }

            Array.Resize(ref _items, Capacity);
        }

        /// <summary>
        /// 填充物品数组让它不是null
        /// </summary>
        public void FillItemArray()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                    _items[i] = new Item();
            }
        }
    }
}
