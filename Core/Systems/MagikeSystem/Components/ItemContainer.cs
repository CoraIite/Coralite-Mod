using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class ItemContainer : MagikeComponent, IUIShowable, IEnumerable<Item>
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

        public Item this[int index]
        {
            get
            {
                if (Items.IndexInRange(index))
                    return Items[index];

                return null;
            }

            set
            {
                if (Items.IndexInRange(index))
                    Items[index] = value;
            }
        }

        #region 网络同步部分

        public override void SendData(ModPacket data)
        {
            //$"SendData-CapacityBase:{CapacityBase}".LoggerDomp();
            data.Write(CapacityBase);

            //$"SendData-CapacityExtra:{CapacityExtra}".LoggerDomp();
            data.Write(CapacityExtra);

            //$"SendData-Items[].Length:{Items.Length}".LoggerDomp();
            data.Write(Items.Length);

            for (int i = 0; i < Items.Length; i++)
            {
                //$"SendData-Items.type:{Items[i].type}".LoggerDomp();
                //data.Write(Items[i].type);
                //data.Write(Items[i].stack);
                //data.Write(Items[i].prefix);
                if (Items[i].IsAir)
                    data.Write(false);
                else
                {
                    data.Write(true);
                    ItemIO.Send(Items[i], data, true);
                }
            }
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            CapacityBase = reader.ReadInt32();
            //$"ReceiveData-CapacityBase:{CapacityBase}".LoggerDomp();

            CapacityExtra = reader.ReadInt32();
            //$"ReceiveData-CapacityExtra:{CapacityExtra}".LoggerDomp();

            int length = reader.ReadInt32();
            //$"ReceiveData-Items[].Length:{length}".LoggerDomp();

            //List<Item> itemList = [];

            if (length > 999)//限制最大长度，放置一些可能的BUG导致游戏爆炸
                length = 999;

            _items = new Item[length];

            for (int i = 0; i < length; i++)
            {
                bool isAir = reader.ReadBoolean();
                if (!isAir)
                    Items[i] = new Item();
                else
                    Items[i] = ItemIO.Receive(reader, true);

                //int type = reader.ReadInt32();
                //int stack = reader.ReadInt32();
                //int prefix = reader.ReadInt32();
                //$"ReceiveData-Items.type:{type}".LoggerDomp();
                //$"ReceiveData-Items.stack:{stack}".LoggerDomp();
                //$"ReceiveData-Items.prefix:{prefix}".LoggerDomp();
                //if (type < 0 || type >= ItemLoader.ItemCount)
                //    type = ItemID.None;

                //Item item=new Item();

                //if (type > 0)
                //{
                //    item.stack = stack;
                //    item.prefix = prefix;
                //}
                //itemList.Add(item);
            }

            //_items = [.. itemList];
        }

        /// <summary>
        /// 发送指定索引的物品
        /// </summary>
        public void SendIndexedItem(int index)
        {
            if (!Items.IndexInRange(index))
            {
                $"物品容器索引越界！在{Entity.Position.X} {Entity.Position.Y}".DumpInConsole();
                $"发生致命错误！".Dump();
                return;
            }

            this.AddToPackList(MagikeNetPackType.ItemContainer_IndexedItem, packet =>
            {
                packet.Write(index);
                ItemIO.Send(Items[index], packet, true);
            });
        }

        /// <summary>
        /// 接受指定索引的物品
        /// </summary>
        /// <param name="reader"></param>
        public void ReceiveIndexedItem(BinaryReader reader)
        {
            int index = reader.ReadInt32();

            Item item = ItemIO.Receive(reader, true);

            if (!Items.IndexInRange(index))
            {
                $"物品容器索引越界！在{Entity.Position.X} {Entity.Position.Y}".DumpInConsole();
                $"发生致命错误！".Dump();
                return;
            }

            Items[index] = item;
        }

        /// <summary>
        /// 客户端向其他端发送信息，只同步一个物品
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="whoAmI"></param>
        public static void ReceiveSpecificItem(BinaryReader reader, int whoAmI)
        {
            int ownerIndex = reader.ReadInt32();
            Point16 position = reader.ReadPoint16();
            int index = reader.ReadInt32();

            if (!Main.player.IndexInRange(ownerIndex))
                return;

            if (MagikeHelper.TryGetEntityWithTopLeft(position, out var tp)
                && tp.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
            {
                container[index] = ItemIO.Receive(reader, true);//接收

                if (Main.dedServ)//服务器端再次向其他客户端同步一下
                {
                    ModPacket modPacket = Coralite.Instance.GetPacket();
                    modPacket.Write((byte)CoraliteNetWorkEnum.ItemContainer_SpecificIndex);
                    modPacket.Write(Main.myPlayer);
                    modPacket.WritePoint16(container.Entity.Position);
                    modPacket.Write(index);
                    ItemIO.Send(container[index], modPacket, true);
                    modPacket.Send(-1, whoAmI);
                }
                else
                {
                    //刷新魔能UI
                    if (MagikeSystem.UIActive())
                        MagikeSystem.RecalculateComponentPanel();
                }
            }
        }

        public static void ReceiveItem(BinaryReader reader, int whoAmI)
        {
            int ownerIndex = reader.ReadInt32();
            Point16 position = reader.ReadPoint16();

            if (!Main.player.IndexInRange(ownerIndex))
                return;

            if (MagikeHelper.TryGetEntityWithTopLeft(position, out var tp)
                && tp.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
            {
                Item item = ItemIO.Receive(reader, true);
                container.AddItem(item);//接收

                if (Main.dedServ)//服务器端再次向其他客户端同步一下
                {
                    ModPacket modPacket = Coralite.Instance.GetPacket();
                    modPacket.Write((byte)CoraliteNetWorkEnum.ItemContainer);
                    modPacket.Write(Main.myPlayer);
                    modPacket.WritePoint16(container.Entity.Position);
                    ItemIO.Send(item, modPacket, true);
                    modPacket.Send(-1, whoAmI);
                }
                else
                {
                    //刷新魔能UI
                    if (MagikeSystem.UIActive())
                        MagikeSystem.RecalculateComponentPanel();
                }
            }
        }

        #endregion

        public override void Update() { }

        /// <summary>
        /// 修改容量后必须调用这个方法！
        /// </summary>
        public void ResetCapacity()
        {
            var e = Entity;
            Vector2 worldPos = e.Position.ToWorldCoordinates();
            var source = new EntitySource_WorldGen($"MagikeTP:{e.ID}");

            //超出容量的部分生成掉落物
            for (int i = Capacity; i < Items.Length; i++)
            {
                Item item = Items[i];
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
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null)
                    Items[i] = new Item();
            }
        }

        public override void OnRemove(MagikeTP entity)
        {
            Point16 coord = entity.Position;
            Vector2 pos = Helper.GetMagikeTileCenter(coord);
            for (int i = 0; i < Items.Length; i++)
                if (!Items[i].IsAir)
                    Item.NewItem(new EntitySource_TileBreak(coord.X, coord.Y), pos, Items[i]);
        }

        /// <summary>
        /// 向容器内加入物品，注意在结束后会使传入的item消失
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddItem(Item item)
        {
            int type = item.type;
            int stack = item.stack;

            foreach (var i in Items.Where(i => !i.IsAir && i.type == type && i.stack < i.maxStack))
            {
                int maxCanInsert = Math.Min(i.maxStack - i.stack, stack);
                i.stack += maxCanInsert;
                stack -= maxCanInsert;
                if (stack < 1)
                {
                    item.TurnToAir();
                    return;
                }
            }

            for (int i = 0; i < Items.Length; i++)
                if (Items[i].IsAir)
                {
                    Items[i] = item.Clone();
                    item.TurnToAir();
                    return;
                }

            Item.NewItem(item.GetSource_DropAsItem(), Helper.GetMagikeTileCenter(Entity.Position), item.Clone());
            item.TurnToAir();
        }

        /// <summary>
        /// 向容器内加入物品，并指定索引<br></br>
        /// 成功加入后会将传入的物品重置为空物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public virtual void AddItemByIndex(Item item,int index)
        {
            if (!Items.IndexInRange(index))
                return;

            if (!Items[index].IsAir)
                return;

            Items[index] = item.Clone();
            item.TurnToAir();
        }

        /// <summary>
        /// 向容器内加入物品，注意在结束后会使传入的item消失
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddItem(int itemType, int stack)
        {
            foreach (var i in Items.Where(i => !i.IsAir && i.type == itemType && i.stack < i.maxStack))
            {
                int maxCanInsert = Math.Min(i.maxStack - i.stack, stack);
                i.stack += maxCanInsert;
                stack -= maxCanInsert;
                if (stack < 1)
                    return;
            }

            for (int i = 0; i < Items.Length; i++)
                if (Items[i].IsAir)
                {
                    Items[i] = new Item(itemType, stack);
                    return;
                }

            Item.NewItem(new EntitySource_DropAsItem(Main.LocalPlayer), Helper.GetMagikeTileCenter(Entity.Position), itemType, stack);
        }

        /// <summary>
        /// 能否放入一个物品
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="stack"></param>
        public virtual bool CanAddItem(int itemType, int stack)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                Item item = Items[i];//有空物品或者容量足够就放入
                if (item.IsAir || item.type == itemType && item.stack < item.maxStack - stack)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 取出一个物品
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual bool DropItem()
        {
            for (int i = 0; i < Items.Length; i++)
                if (!Items[i].IsAir)
                {
                    Item.NewItem(new EntitySource_DropAsItem(Main.LocalPlayer), Helper.GetMagikeTileCenter(Entity.Position), Items[i].Clone());
                    Items[i].TurnToAir();
                    return true;
                }

            return false;
        }

        /// <summary>
        /// 获得一个物品，需要指定数量，还可以指定类型<br></br>
        /// 如果数量不足则会只拿出该拿的
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual Item GetItem(int stack, int? type = null)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                Item item = Items[i];
                if (item.IsAir)
                    continue;

                if (type != null)//有指定的类型
                {
                    if (item.type == type.Value)//正好对上了
                        return NewItem(stack, item);

                    continue;
                }

                return NewItem(stack, item);
            }

            return null;

            static Item NewItem(int stack, Item item)
            {
                if (item.stack > stack)//数量多，直接减少
                {
                    item.stack -= stack;

                    Item item1 = item.Clone();
                    item1.stack = stack;
                    return item1;
                }
                else//数量不够，全部返回，自身重置
                {
                    Item item1 = item.Clone();
                    item.TurnToAir();
                    return item1;
                }
            }
        }

        #region UI部分

        public virtual void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.ItemContainerName, parent);

            UIElement text = this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ItemMax)
            + $"\n  ▶ {c.Capacity} ({c.CapacityBase} {(c.CapacityExtra < 0 ? "-" : "+")} {c.CapacityExtra})", parent);

            text.SetTopLeft(title.Height.Pixels + 8, 0);
            parent.Append(text);

            UIGrid grid = new()
            {
                OverflowHidden = false
            };

            for (int i = 0; i < Items.Length; i++)
            {
                ItemContainerSlot slot = new(this, i);
                grid.Add(slot);
            }

            grid.SetSize(0, -text.Top.Pixels - text.Height.Pixels, 1, 1);
            grid.SetTopLeft(text.Top.Pixels + text.Height.Pixels + 6, 0);

            parent.Append(grid);
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

        #region 迭代相关

        public IEnumerator<Item> GetEnumerator()
        {
            return ((IEnumerable<Item>)Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        #endregion
    }

    public class ItemContainerSlot : UIElement
    {
        private readonly ItemContainer _container;
        private readonly int _index;
        private float _scale = 1f;

        public ItemContainerSlot(ItemContainer container, int index)
        {
            _container = container;
            _index = index;
            this.SetSize(54, 54);
        }

        public bool TryGetItem(out Item item)
        {
            item = _container[_index];
            if (item == null)
            {
                UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();
                return false;
            }

            return true;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public void SendData()
        {
            if (!VaultUtils.isClient)
                return;

            //_container.Entity.SendData();

            ModPacket modPacket = Coralite.Instance.GetPacket();
            modPacket.Write((byte)CoraliteNetWorkEnum.ItemContainer_SpecificIndex);
            modPacket.Write(Main.myPlayer);
            modPacket.WritePoint16(_container.Entity.Position);
            modPacket.Write(_index);
            ItemIO.Send(_container[_index], modPacket, true);

            modPacket.Send();
        }

        //public void GrabSound()
        //{
        //    Helper.PlayPitched("Fairy/FairyBottleClick", 0.4f, 0);
        //}

        private void HandleItemSlotLogic()
        {
            if (IsMouseHovering)
            {
                Item inv = _container[_index];
                Main.LocalPlayer.mouseInterface = true;
                ItemSlot.OverrideHover(ref inv, ItemSlot.Context.VoidItem);
                ItemSlot.LeftClick(ref inv, ItemSlot.Context.VoidItem);
                ItemSlot.RightClick(ref inv, ItemSlot.Context.VoidItem);
                ItemSlot.MouseHover(ref inv, ItemSlot.Context.VoidItem);
                _container[_index] = inv;
                _scale = Helper.Lerp(_scale, 1.1f, 0.2f);

                if ((Main.mouseRightRelease && Main.mouseRight) || (Main.mouseLeftRelease && Main.mouseLeft))
                {
                    SendData();
                }
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!TryGetItem(out Item inv))
                return;

            HandleItemSlotLogic();

            float scale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref inv, ItemSlot.Context.VoidItem, position, Color.White);

            Main.inventoryScale = scale;
        }
    }
}