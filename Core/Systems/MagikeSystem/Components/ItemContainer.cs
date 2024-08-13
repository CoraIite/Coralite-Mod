using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class ItemContainer : Component,IUIShowable
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
        public Item[] Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new Item[Capacity];
                    FillItemArray();
                }

                return _items;
            }
        }

        public override void Update(IEntity entity)
        {
        }

        /// <summary>
        /// 修改容量后必须调用这个方法！
        /// </summary>
        public void ResetCapacity()
        {
            Vector2 worldPos = (Entity as MagikeTileEntity).Position.ToWorldCoordinates();
            var source = new EntitySource_TileEntity(Entity as MagikeTileEntity);

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

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.ItemContainerName, parent);

            UIGrid grid =new();

            grid.Width.Set(0, 1);
        }

        #endregion

        #region 存储与加载部分

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(CapacityBase), CapacityBase);
            tag.Add(preName + nameof(CapacityExtra), CapacityExtra);

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].IsAir)
                    continue;

                tag.Add(preName + nameof(_items) + i, Items[i]);
            }
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            CapacityBase = tag.GetInt(preName + nameof(CapacityBase));
            CapacityExtra = tag.GetInt(preName + nameof(CapacityExtra));

            _items = new Item[Capacity];
            for (int i = 0; i < Items.Length; i++)
            {
                if (tag.TryGet(preName + nameof(_items) + i, out Item item))
                    _items[i] = item;
                else
                    _items[i] = new Item();
            }
        }

        #endregion
    }
}
