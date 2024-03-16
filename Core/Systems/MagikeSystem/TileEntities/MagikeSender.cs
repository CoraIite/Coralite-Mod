using Coralite.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeSender : ModTileEntity, IMagikeSender
    {
        /// <summary> 当前魔能量 </summary>
        public int magike;
        /// <summary> 魔能最大值 </summary>
        public readonly int magikeMax;
        /// <summary> 当前的装置是否在使用状态 </summary>
        public bool active;

        /// <summary> 每次发送多少，可以自定义 </summary>
        public abstract int HowManyPerSend { get; }
        public abstract ushort TileType { get; }

        public int Magike => magike;
        public int MagikeMax => magikeMax;
        public bool Active => active;
        public Point16 GetPosition => Position;

        public event Action<IMagikeContainer> OnSended;
        public event Action OnCharged;
        public event Action OnDisCharged;

        public MagikeSender(int magikeMax)
        {
            this.magikeMax = magikeMax;
        }

        public override void Update()
        {
            Send();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == TileType;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, TileChangeType.HoneyLava);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }

            return Place(i, j);
        }


        public override void SaveData(TagCompound tag)
        {
            tag.Add("Magike", magike);
        }

        public override void LoadData(TagCompound tag)
        {
            magike = tag.GetInt("Magike");
        }


        public virtual void Send() { }

        /// <summary>
        /// 是否能发送
        /// </summary>
        /// <returns></returns>
        public abstract bool CanSend();

        /// <summary> 限制魔能量，让它不超过上限 </summary>
        public virtual void Limit()
        {
            magike = Math.Clamp(magike, 0, magikeMax);
        }

        /// <summary>
        /// 给改魔能容器充能的方法，需要先获取到实例才行
        /// </summary>
        /// <param name="howManyMagike">充多少</param>
        public virtual bool Charge(int howManyMagike)
        {
            bool ChargeOrDischarge = howManyMagike >= 0;
            if (magike >= magikeMax && ChargeOrDischarge)
                return false;

            if (ChargeOrDischarge)
            {
                OnCharged?.Invoke();
                OnReceiveVisualEffect();
            }
            else
                OnDisCharged?.Invoke();

            magike += howManyMagike;
            Limit();
            CheckActive();

            return true;
        }

        public void OnSend(int howMany, IMagikeContainer receiver)
        {
            Charge(-howMany);
            SendVisualEffect(receiver);
            OnSended?.Invoke(receiver);
        }

        /// <summary>
        /// 发送时产生的视觉效果
        /// </summary>
        /// <param name="container"></param>
        public virtual void SendVisualEffect(IMagikeContainer container) { }

        public virtual void OnReceiveVisualEffect() { }

        public virtual void CheckActive()
        {
            active = magike > 0;
        }

        public virtual Vector2 GetWorldPosition()
        {
            return Position.ToWorldCoordinates(16);
        }

        public virtual void ShowConnection()
        {

        }

        public abstract bool ConnectToRecevier(IMagikeContainer container);

        public virtual void DisconnectAll() { }
    }

    /// <summary>
    /// 线性魔能发送器，能够存储魔能，同时可以发送魔能，使用howManyCanConnect来决定能连接多少魔能容器
    /// </summary>
    public abstract class MagikeSender_Line : MagikeSender, IMagikeSender_Line
    {
        /// <summary> 接收者的位置 </summary>
        public Point16[] receiverPoints;
        /// <summary> 距离多少才能连接 </summary>
        public readonly int connectLenghMax;

        Point16[] IMagikeSender_Line.receiverPoints => receiverPoints;
        int IMagikeSender_Line.connectLenghMax => connectLenghMax;

        public event Action<IMagikeContainer> OnConnected;

        public MagikeSender_Line(int magikeMax, int connectLenghMax, int howManyCanConnect = 1) : base(magikeMax)
        {
            this.connectLenghMax = connectLenghMax;
            receiverPoints = new Point16[howManyCanConnect];
            for (int i = 0; i < howManyCanConnect; i++)
                receiverPoints[i] = Point16.NegativeOne;
        }

        public void InitReceivers()
        {
            for (int k = 0; k < receiverPoints.Length; k++)
                receiverPoints[k] = Point16.NegativeOne;
        }

        public override void Send()
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

        /// <summary>
        /// 一个简单的帮助方法，展示目前和谁连接了
        /// </summary>
        public override void ShowConnection()
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


        /// <summary>
        /// 帮助方法，判断两个魔能容器的距离并根据自身的connectLenghMax决定是否能发送给对方
        /// </summary>
        /// <returns></returns>
        public virtual bool CanConnect(IMagikeContainer container)
        {
            return Vector2.Distance(container.GetPosition.ToVector2() * 16, Position.ToVector2() * 16) < connectLenghMax
                && container.GetPosition != Position;
        }

        public virtual bool CanConnect(Point16 targetPos)
        {
            return Vector2.Distance(targetPos.ToVector2() * 16, Position.ToVector2() * 16) < connectLenghMax
                && targetPos != Position;
        }

        /// <summary>
        /// 一个简单的帮助方法，用于判断是否能连接到对面的接收器
        /// </summary>
        /// <remarks>
        ///  默认的连接方法是判断与对方的距离是否小于<see cref="connectLenghMax"/>，如果小于的话就能够在二者之间建立连接<br></br>
        ///  否则将返回<see langword="false"/>。如果成功连接将触发<see cref="OnConnected"/>
        /// </remarks>
        /// <param name="container"></param>
        /// <returns></returns>
        public override bool ConnectToRecevier(IMagikeContainer container)
        {
            if (CanConnect(container))
            {
                bool hasSlot = false;
                for (int i = 0; i < receiverPoints.Length; i++) //感觉...不太优雅，以后还是想一想怎么改用LINQ吧
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

        /// <summary>
        /// 取消所有连接
        /// </summary>
        public override void DisconnectAll()
        {
            for (int i = 0; i < receiverPoints.Length; i++)
                receiverPoints[i] = Point16.NegativeOne;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
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
