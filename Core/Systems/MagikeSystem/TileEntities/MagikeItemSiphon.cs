using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeItemSiphon : MagikeFactory, ISingleItemContainer, IItemSender
    {
        private int currentCheckItem;
        public readonly int connectLenghMax;
        public readonly int howManyToCheckPerFrame;
        public readonly int sendItemDelay;

        public Item containsItem = new Item();

        public Point16 receiverPoint;
        public int connectChestIndex = -1;
        /// <summary>
        /// 扩展膜，用于执行特定工作
        /// </summary>
        public Item[] extensions;

        public int sendItemCost;
        public int workCost;
        public int maxPerSend;
        public int sendTimer;
        public abstract Color MainColor { get; }
        public Item ContainsItem { get => containsItem; set => containsItem = value; }

        protected MagikeItemSiphon(int magikeMax, int howManyToCheckPerFrame, int extensionCount, int connectLenghMax, int sendItemCost, int workCost, int maxPerSend,int sendItemDelay) : base(magikeMax, (Main.maxItems / howManyToCheckPerFrame) + 3)
        {
            this.howManyToCheckPerFrame = howManyToCheckPerFrame;
            extensions = new Item[extensionCount];

            for (int i = 0; i < extensionCount; i++)
                extensions[i] = new Item();

            this.connectLenghMax = connectLenghMax;
            this.sendItemCost = sendItemCost;
            this.workCost = workCost;
            this.maxPerSend = maxPerSend;
            this.sendItemDelay = sendItemDelay;
        }

        public override void Update()
        {
            SendItem();
            base.Update();
        }

        #region 物品吸取工作相关

        public override void DuringWork()
        {
            if (currentCheckItem < Main.maxItems)
            {
                for (int i = 0; i < howManyToCheckPerFrame; i++)
                {
                    //遍历物品，检测是否能够吸取，如果能那么就把物品放入自身中

                    currentCheckItem++;
                }
            }
        }

        public override void WorkFinish()
        {
            Charge(-workCost);
            currentCheckItem = 0;
        }

        #endregion

        #region 物品交互相关

        public bool CanGetItem() => true;
        public bool CanInsertItem(Item item) => true;
        public Item GetItem() => containsItem;

        public bool InsertItem(Item item)
        {
            containsItem = item;
            return true;
        }

        bool ISingleItemContainer.TryOutputItem(Func<bool> rule, out Item item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 物品发送相关

        public bool CanSendItem()
        {
            sendTimer++;
            if (sendTimer > sendItemDelay)
            {
                sendTimer = 0;
                return true;
            }

            return false;
        }

        public void DisconnectAll_ItemSender()
        {
            receiverPoint = Point16.NegativeOne;
        }

        public void SendItem()
        {
            if (!CanSendItem())
                return;

            OnSendItem();
        }

        public void SendVisualEffect(Point16 position)
        {
            MagikeHelper.SpawnDustOnItemSend(1, 2, position, MainColor);
        }

        public void ShowConnection_ItemSender()
        {
            MagikeHelper.SpawnDustOnItemSend(1, 2, receiverPoint, MainColor);
        }

        public void OnSendItem()
        {
            if (containsItem == null || containsItem.IsAir || receiverPoint == Point16.NegativeOne)
                return;

            if (connectChestIndex != -1)
            {
                Chest chest = Main.chest[connectChestIndex];
                for (int i = 0; i < Chest.maxItems; i++)//如果物品不为空且物品类型不同或stack超出最大stack那么就向下查找
                {
                    Item item = chest.item[i];
                    if (item == null || item.type != containsItem.type)
                        continue;
                    if (item.IsAir)
                    {
                        chest.item[i] = containsItem.Clone();
                        chest.item[i].stack = maxPerSend;
                        containsItem.stack -= maxPerSend;
                        if (containsItem.stack < 1)
                            containsItem.TurnToAir();
                    }
                    else if (item.type == containsItem.type)
                    {
                        int sub = item.maxStack - item.stack;
                        if (sub < maxPerSend)
                        {
                            chest.item[i].stack = item.maxStack;
                            containsItem.stack -= sub;
                            if (containsItem.stack < 1)
                                containsItem.TurnToAir();
                        }
                        else
                        {
                            chest.item[i].stack += maxPerSend;
                            containsItem.stack -= sub;
                            if (containsItem.stack < 1)
                                containsItem.TurnToAir();
                        }
                    }

                    Charge(-sendItemCost);
                    SendVisualEffect(receiverPoint);
                    Chest.VisualizeChestTransfer(GetPosition.ToWorldCoordinates(8, 16), new Vector2(chest.x, chest.y) * 16, item, maxPerSend);
                    return;
                }

                return;
            }

            if (ByPosition.ContainsKey(receiverPoint) && ByPosition[receiverPoint] is ISingleItemContainer container)
                if (container.CanGetItem())
                {
                    Item item = container.GetItem();
                    if (item == null || item.type != containsItem.type)
                        return;
                    if (item.IsAir)
                    {
                        container.ContainsItem = containsItem.Clone();
                        container.ContainsItem.stack = maxPerSend;
                        containsItem.stack -= maxPerSend;
                        if (containsItem.stack < 1)
                            containsItem.TurnToAir();
                    }
                    else if (item.type == containsItem.type)
                    {
                        int sub = item.maxStack - item.stack;
                        if (sub < maxPerSend)
                        {
                            container.ContainsItem.stack = item.maxStack;
                            containsItem.stack -= sub;
                            if (containsItem.stack < 1)
                                containsItem.TurnToAir();
                        }
                        else
                        {
                            container.ContainsItem.stack += maxPerSend;
                            containsItem.stack -= sub;
                            if (containsItem.stack < 1)
                                containsItem.TurnToAir();
                        }
                    }

                    Charge(-sendItemCost);
                    SendVisualEffect(receiverPoint);

                    Tile tile = Framing.GetTileSafely(Position);
                    TileObjectData data = TileObjectData.GetTileData(tile);
                    int xOffset = data == null ? 16 : data.Width * 8;
                    int yOffset = data == null ? 24 : data.Height * 8;

                    Vector2 targetPos = receiverPoint.ToWorldCoordinates(xOffset, yOffset);
                    Chest.VisualizeChestTransfer(GetPosition.ToWorldCoordinates(8, 16), targetPos, item, maxPerSend);
                }
        }

        public bool ConnectToReceiver(Point16 position)
        {
            if (!CanConnect(position))
                return false;

            connectChestIndex = Chest.FindChest(position.X, position.Y);
            receiverPoint = position;

            return true;
        }

        public virtual bool CanConnect(Point16 position)
        {
            return Vector2.Distance(position.ToVector2() * 16, Position.ToVector2() * 16) < connectLenghMax;
        }

        #endregion

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (!containsItem.IsAir)
                tag.Add("containsItem", containsItem);

            for (int i = 0; i < extensions.Length; i++)
            {
                if (!extensions[i].IsAir)
                    tag.Add("extensions"+i, extensions[i]);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.TryGet("containsItem", out Item item))
                containsItem = item;

            for (int i = 0; i < extensions.Length; i++)
            {
                if (tag.TryGet("extensions" + i, out Item extension))
                    extensions[i] = extension;
            }
        }
    }
}
