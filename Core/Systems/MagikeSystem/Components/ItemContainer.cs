﻿using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class ItemContainer : MagikeComponent, IUIShowable
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
                data.Write(Items[i].type);
                data.Write(Items[i].stack);
                data.Write(Items[i].prefix);
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

            List<Item> itemList = [];
            if (length > 999)
            {
                length = 999;
            }
            for (int i = 0; i < length; i++)
            {
                int type = reader.ReadInt32();
                int stack = reader.ReadInt32();
                int prefix = reader.ReadInt32();
                //$"ReceiveData-Items.type:{type}".LoggerDomp();
                //$"ReceiveData-Items.stack:{stack}".LoggerDomp();
                //$"ReceiveData-Items.prefix:{prefix}".LoggerDomp();
                if (type < 0 || type >= ItemLoader.ItemCount)
                {
                    type = ItemID.None;
                }
                Item item = new Item(type);
                if (type > 0)
                {
                    item.stack = stack;
                    item.prefix = prefix;
                }
                itemList.Add(item);
            }
            _items = [.. itemList];
        }

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
            {
                return;
            }
            if (_container.Entity is MagikeTP magikeTP)
            {
                magikeTP.SendData();
            }
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