using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Network;
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
        [SyncVar]
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

        /// <summary> 容量 </summary>
        public int Capacity => CapacityBase;

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

        /// <summary>
        /// 供 <see cref="SyncVarManager"/> 同步物品数组用的访问器。<br></br>
        /// 取值时走 <see cref="Items"/> 保证非空且长度为 <see cref="Capacity"/>，
        /// 赋值时直接落到底层字段（自定义 <c>Item[]</c> 同步类型已在 <see cref="MagikeSyncRegistration"/> 注册）。
        /// </summary>
        [SyncVar]
        private Item[] SyncItems
        {
            get => Items;
            set => _items = value;
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

        /// <summary> 客户端已发出取出请求、等待服务端槽位回传 </summary>
        private bool _dropPending;

        #region 网络同步部分

        //改用 InnoVault SyncVar 同步（[SyncVar] 标注的 CapacityBase 与 SyncItems）。
        //由 SyncVarManager 统一按字段顺序读写，配合 MagikeTP 的长度框，
        //彻底杜绝旧实现因运行时组件错位导致 length 读成天文数字而需要 "999 挡刀" 的问题。
        public override void SendData(ModPacket data) => SyncVarManager.Send(this, data);

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            SyncVarManager.Receive(this, reader);
            _dropPending = false;
        }

        /// <summary>
        /// 发送指定索引的物品（即时单包，不走 3 秒 batch）
        /// </summary>
        public void SendIndexedItem(int index)
        {
            if (!Items.IndexInRange(index))
            {
                $"物品容器索引越界！在{Entity.Position.X} {Entity.Position.Y}".DumpInConsole();
                $"发生致命错误！".Dump();
                return;
            }

            MagikeSystem.SendImmediateMagikePack(
                new MagikeNetPack(Entity.Position, MagikeNetPackType.ItemContainer_IndexedItem),
                packet =>
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
            _dropPending = false;
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
                    modPacket.Write(ownerIndex);//原样回传发起者索引，而非 Main.myPlayer（服务端=255）
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
                    modPacket.Write(ownerIndex);//原样回传发起者索引，而非 Main.myPlayer（服务端=255）
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

            //超出容量的部分生成掉落物：仅服务端/单人产出，走 InnoVault VaultUtils.SpwanItem 自带网络同步
            for (int i = Capacity; i < Items.Length; i++)
            {
                Item item = Items[i];
                if (item != null && !item.IsAir && !VaultUtils.isClient)
                    VaultUtils.SpwanItem(source, worldPos, item);
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
            //物块破坏时掉落容器内物品：仅服务端/单人产出，避免客户端各自再生成一份（重复刷物品）
            if (VaultUtils.isClient)
                return;

            Point16 coord = entity.Position;
            Vector2 pos = Helper.GetMagikeTileCenter(coord);
            for (int i = 0; i < Items.Length; i++)
                if (!Items[i].IsAir)
                    VaultUtils.SpwanItem(new EntitySource_TileBreak(coord.X, coord.Y), pos, Items[i]);
        }

        /// <summary>
        /// 向容器内加入物品，注意在结束后会使传入的item消失
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddItem(Item item)
        {
            int type = item.type;

            foreach (var i in Items.Where(i => !i.IsAir && i.type == type && i.stack < i.maxStack))
            {
                int maxCanInsert = Math.Min(i.maxStack - i.stack, item.stack);
                i.stack += maxCanInsert;
                item.stack -= maxCanInsert;
                if (item.stack < 1)
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

            //容器装满后多余的部分掉到地上：仅服务端/单人产出（客户端等待权威同步对账）
            if (!VaultUtils.isClient)
                VaultUtils.SpwanItem(item.GetSource_DropAsItem(), Helper.GetMagikeTileCenter(Entity.Position), item.Clone());
            item.TurnToAir();
        }

        /// <summary>
        /// 向容器内加入物品，并指定索引<br></br>
        /// 成功加入后会将传入的物品重置为空物品
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public virtual void AddItemByIndex(Item item, int index)
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

            //装满后多余部分掉落：仅服务端/单人产出
            if (!VaultUtils.isClient)
                VaultUtils.SpwanItem(new EntitySource_DropAsItem(Main.LocalPlayer), Helper.GetMagikeTileCenter(Entity.Position), new Item(itemType, stack));
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
                if (item.IsAir || item.type == itemType && item.stack <= item.maxStack - stack)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 取出一个物品。<br></br>
        /// 多人下取出改为服务端权威：客户端只发取出请求，由服务端 <see cref="ServerDropItem"/> 生成掉落物并回传被清空的槽位，
        /// 杜绝旧实现里点击端本地 <see cref="Item.NewItem"/> 造成的 desync（点击端凭空多一份物品、其它端槽位不清空）。
        /// </summary>
        /// <param name="slotIndex">指定槽位；<see langword="null"/> 时由服务端取第一个非空槽</param>
        public virtual bool DropItem(int? slotIndex = null)
        {
            //客户端：仅在本地确有物品且未处于 pending 时发请求；实际取出与槽位清空等待服务端即时回传
            if (VaultUtils.isClient)
            {
                if (_dropPending || !HasAnyItem())
                    return false;

                if (slotIndex.HasValue)
                {
                    if (!Items.IndexInRange(slotIndex.Value) || Items[slotIndex.Value].IsAir)
                        return false;
                }

                _dropPending = true;
                RequestDropItem(slotIndex);
                return true;
            }

            //单人/服务端：权威执行
            return slotIndex.HasValue ? ServerDropItem(slotIndex.Value) : ServerDropItem();
        }

        /// <summary>
        /// 服务端/单人权威取出一个物品：走 <see cref="VaultUtils.SpwanItem"/> 生成带网络同步的掉落物并清空对应槽位，
        /// 再通过 <see cref="SendIndexedItem"/> 把该槽位即时下发给所有客户端。
        /// </summary>
        /// <param name="slotIndex">指定槽位；省略时取第一个非空槽</param>
        public bool ServerDropItem(int slotIndex = -1)
        {
            if (VaultUtils.isClient)
                return false;

            if (slotIndex < 0)
            {
                for (int i = 0; i < Items.Length; i++)
                    if (!Items[i].IsAir)
                    {
                        slotIndex = i;
                        break;
                    }
            }

            if (!Items.IndexInRange(slotIndex) || Items[slotIndex].IsAir)
                return false;

            VaultUtils.SpwanItem(new EntitySource_DropAsItem(Main.LocalPlayer), Helper.GetMagikeTileCenter(Entity.Position), Items[slotIndex].Clone());
            Items[slotIndex].TurnToAir();
            SendIndexedItem(slotIndex);
            return true;
        }

        /// <summary>
        /// 是否存在任意非空物品
        /// </summary>
        public bool HasAnyItem()
        {
            for (int i = 0; i < Items.Length; i++)
                if (!Items[i].IsAir)
                    return true;

            return false;
        }

        /// <summary>
        /// 客户端 → 服务端：发送取出请求。复用 <see cref="CoraliteNetWorkEnum.MagikeSystem"/> 子协议的单包 batch 格式，
        /// 不新增网络 worker 枚举（<see cref="CoraliteNetWorkEnum"/> 是网络层禁区）。
        /// </summary>
        private void RequestDropItem(int? slotIndex = null)
        {
            if (!VaultUtils.isClient)
                return;

            ModPacket p = Coralite.Instance.GetPacket();
            p.Write((byte)CoraliteNetWorkEnum.MagikeSystem);
            p.Write(1);//包数量：单包
            p.Write(MagikeSystem.GUID);//与 SendMagikePack/ReceiveMagikePack 一致的分割标记
            p.WriteMagikePack(Entity.Position, MagikeNetPackType.ItemContainer_DropRequest);
            p.Write(ID);//容器组件 ID（普通 ItemContainer 与只读 GetOnlyItemContainer 共用本路径，需区分）
            p.Write(slotIndex ?? -1);
            p.Send();
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
            + $"\n  ▶ {c.Capacity}", parent);

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