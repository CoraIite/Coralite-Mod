using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeFactory_PolymerizeAltar : MagikeFactory, IMagikeSender, ISingleItemContainer
    {
        public MagikeFactory_PolymerizeAltar(int magikeMax,int perWorkTime, int connectLenghMax, int howManyCanConnect = 1) : base(magikeMax, 100)
        {
            this.connectLenghMax = connectLenghMax;
            receiverPoints = new Point16[howManyCanConnect];
            for (int i = 0; i < howManyCanConnect; i++)
                receiverPoints[i] = Point16.NegativeOne;

            this.perWorkTime = perWorkTime;
        }

        /// <summary>
        /// 固定为1，台座的魔能量也固定为1
        /// </summary>
        public int HowManyPerSend => 1;
        public abstract Color MainColor { get; }

        /// <summary> 接收者的位置 </summary>
        public Point16[] receiverPoints;
        /// <summary> 距离多少才能连接 </summary>
        public readonly int connectLenghMax;
        /// <summary> 每消耗一个物品所需的时间 </summary>
        public readonly int perWorkTime;
        public int sendTimer;

        public event Action<IMagikeContainer> OnConnected;

        public Item containsItem = new Item();
        public PolymerizeRecipe chooseRecipe;

        public void InitReceivers()
        {
            for (int k = 0; k < receiverPoints.Length; k++)
                receiverPoints[k] = Point16.NegativeOne;
        }

        #region 发送接收魔能相关

        public bool CanSend()
        {
            sendTimer++;
            if (sendTimer > 1200)
            {
                sendTimer = 0;
                return true;
            }

            return false;
        }

        public void OnSend(int howMany, IMagikeContainer receiver)
        {
            Charge(-howMany);
            SendVisualEffect(receiver);
        }

        public void Send()
        {
            if (!CanSend())
                return;

            int howMany = HowManyPerSend;
            CheckActive();
            if (magike == 0)   //当前量为0时直接返回
                return;
            else
            {
                if (magike < howMany)   //不够发送时直接把自身剩下的全发送了
                    howMany = magike;
            }

            for (int i = 0; i < receiverPoints.Length; i++)
            {
                Point16 position = receiverPoints[i];
                if (position != Point16.NegativeOne && ByPosition.ContainsKey(position) && ByPosition[position] is IMagikeContainer container)
                {
                    bool overflowOrNot = (container.Magike + howMany) <= container.MagikeMax;//是否溢出
                    bool isZero = container.Magike == 0;//接收者是否为0
                    if ((overflowOrNot || isZero) && container.Charge(howMany))//魔能不满时才能发送给接收者，发送后会防止越界情况出现
                    {
                        OnSend(howMany, container);
                        if (magike < howMany)   //发送完之后如果剩余量不能够继续发送那么直接返回
                            return;
                    }
                }
                else
                    receiverPoints[i] = Point16.NegativeOne;
            }
        }

        public void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 2, Position, container, MainColor);
        }

        public void ShowConnection()
        {
            for (int i = 0; i < receiverPoints.Length; i++)
            {
                if (receiverPoints[i] != Point16.NegativeOne)
                {
                    if (MagikeHelper.TryGetEntityWithTopLeft(receiverPoints[i].X, receiverPoints[i].Y, out IMagikeContainer container))
                        SendVisualEffect(container);
                }
            }
        }

        public bool ConnectToRecevier(IMagikeContainer container)
        {
            if (CanConnect(container))
            {
                bool hasSlot = false;
                for (int i = 0; i < receiverPoints.Length; i++)
                {
                    if (receiverPoints[i] == Point16.NegativeOne)
                    {
                        receiverPoints[i] = container.GetPosition;
                        hasSlot = true;
                        break;
                    }
                }

                if (!hasSlot)
                    receiverPoints[0] = container.GetPosition;

                OnConnected?.Invoke(container);
                return true;
            }

            return false;
        }

        public virtual bool CanConnect(IMagikeContainer container)
        {
            return Vector2.Distance(container.GetPosition.ToVector2() * 16, Position.ToVector2() * 16) < connectLenghMax;
        }

        public void DisconnectAll()
        {
            for (int i = 0; i < receiverPoints.Length; i++)
                receiverPoints[i] = Point16.NegativeOne;
        }

        #endregion

        #region 聚合工作相关

        public override bool StartWork()
        {
            if (containsItem is not null && !containsItem.IsAir && containsItem.type == chooseRecipe.MainItem.type && containsItem.stack >= chooseRecipe.MainItem.stack && chooseRecipe is not null)
            {
                foreach (var item in chooseRecipe.RequiredItems)
                {
                    int howManyNeed = item.stack;
                    for (int i = 0; i < receiverPoints.Length; i++)
                    {
                        Point16 position = receiverPoints[i];
                        if (position != Point16.NegativeOne && ByPosition.ContainsKey(position) && ByPosition[position] is PolymerizePedestal pedestal)
                        {
                            Item pItem = pedestal.GetItem();
                            if (pItem.type == item.type)  //如果台座上的是需要的物品，那就减一下，减完了之后如果小于零说明物品数量足够
                            {
                                howManyNeed -= pItem.stack;
                                if (howManyNeed <= 0)
                                    break;
                            }
                        }
                    }

                    if (howManyNeed > 0)//如果遍历完一遍之后发现这个还大于零那么说明当前台座上需要的数量不够，不能聚合
                        return false;
                }

                workTimeMax = perWorkTime * chooseRecipe.RequiredItems.Count;
                return base.StartWork();
            }

            return false;
        }

        public override void DuringWork()
        {
            if (workTimer % perWorkTime == 0)
            {
                int index = workTimer / perWorkTime;
                if (index < chooseRecipe.RequiredItems.Count - 1)
                {
                    Item item = chooseRecipe.RequiredItems[index];
                    int howManyNeed = item.stack;

                    for (int i = 0; i < receiverPoints.Length; i++)
                    {
                        Point16 position = receiverPoints[i];
                        if (position != Point16.NegativeOne && ByPosition.ContainsKey(position) && ByPosition[position] is PolymerizePedestal pedestal)
                        {
                            Item pItem = pedestal.GetItem();
                            if (pItem.type == item.type)  //如果台座上的是需要的物品，那就减一下，减完了之后如果小于零说明物品数量足够
                            {
                                pedestal.OnReceiveVisualEffect();
                                SendVisualEffect(pedestal);
                                if (pItem.stack >= howManyNeed)//数量够，减少一下stack
                                {
                                    pItem.stack -= howManyNeed;
                                    if (pItem.stack <= 0)
                                        pItem.TurnToAir();
                                    howManyNeed = 0;
                                    break;
                                }
                                else//数量不够，全部消耗之后继续循环
                                {
                                    howManyNeed -= pItem.stack;
                                    pItem.TurnToAir();
                                }
                            }
                        }
                    }

                    if (howManyNeed>0)//如果没能完全消耗那么就说明物品不够，停止聚合
                        workTimer = -1;
                }
            }


        }

        public override void WorkFinish()
        {
            if (containsItem is not null && !containsItem.IsAir && containsItem.type == chooseRecipe.MainItem.type && 
                containsItem.stack >= chooseRecipe.MainItem.stack && chooseRecipe is not null&&magike>=chooseRecipe.magikeCost)
            {
                Charge(-chooseRecipe.magikeCost);
                Vector2 position = Position.ToWorldCoordinates(16, -8);

                Item item = chooseRecipe.ResultItem.Clone();
                if (item.TryGetGlobalItem(out MagikeItem magikeItem))   //把不必要的东西删掉
                {
                    magikeItem.magike_CraftRequired = -1;
                    magikeItem.stack_CraftRequired = 0;
                    magikeItem.condition = null;
                }

                int index = Item.NewItem(new EntitySource_TileEntity(this), position, item);    //生成掉落物
                chooseRecipe.onPolymerize?.Invoke(containsItem, Main.item[index]); //触发On合成

                containsItem.stack -= chooseRecipe.selfRequiredNumber;  //消耗原物品
                if (containsItem.stack < 1)
                    containsItem.TurnToAir();

                SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, position);
                MagikeHelper.SpawnDustOnGenerate(2, 2, Position + new Point16(0, -2), MainColor);
            }
        }

        public override void OnKill()
        {
            MagikePolymerizeUI.visible = false;
            UILoader.GetUIState<MagikePolymerizeUI>().Recalculate();

            if (!containsItem.IsAir)
                Item.NewItem(new EntitySource_TileEntity(this), Position.ToWorldCoordinates(16, -8), containsItem);
        }

        #endregion

        #region 物品存取相关

        public virtual bool InsertItem(Item item)
        {
            containsItem = item;
            return true;
        }

        public virtual bool CanInsertItem(Item item)
        {
            return true;
        }

        public virtual Item GetItem()
        {
            return containsItem;
        }

        public bool CanGetItem()
        {
            return workTimer == -1;
        }

        bool ISingleItemContainer.TryOutputItem(Func<bool> rule, out Item item)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (!containsItem.IsAir)
            {
                tag.Add("containsItem", containsItem);
            }

            for (int i = 0; i < receiverPoints.Length; i++)
                if (receiverPoints[i] != Point16.NegativeOne)
                {
                    tag.Add("Receiver_x" + i, receiverPoints[i].X);
                    tag.Add("Receiver_y" + i, receiverPoints[i].Y);
                }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.TryGet("containsItem", out Item item))
            {
                containsItem = item;
            }

            for (int i = 0; i < receiverPoints.Length; i++)
            {
                if (tag.TryGet("Receiver_x" + i, out short x) && tag.TryGet("Receiver_y" + i, out short y))
                    receiverPoints[i] = new Point16(x, y);
                else
                    receiverPoints[i] = Point16.NegativeOne;
            }
        }
    }
}
