using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using System.Collections.Generic;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeSender : Component
    {
        public override int ID => MagikeComponentID.MagikeSender;

        /// <summary> 基础连接数量 </summary>
        public int MaxConnectBase { get; private set; }
        /// <summary> 额外连接数量 </summary>
        public int MaxConnectExtra { get; set; }

        /// <summary> 可连接数量 </summary>
        public int MaxConnect { get => MaxConnectBase + MaxConnectExtra; }

        /// <summary> 基础连接距离 </summary>
        public int ConnectLengthBase { get; private set; }
        /// <summary> 额外连接距离 </summary>
        public int ConnectLengthExtra { get; set; }

        /// <summary> 连接距离 </summary>
        public int ConnectLength { get => ConnectLengthBase + ConnectLengthExtra; }

        /// <summary> 基础单次发送量 </summary>
        public int UnitDeliveryBase { get; private set; }
        /// <summary> 额外单次发送量 </summary>
        public int UnitDeliveryExtra { get; set; }

        /// <summary> 单次发送量 </summary>
        public int UnitDelivery { get => UnitDeliveryBase + UnitDeliveryExtra; }

        /// <summary> 基础发送时间 </summary>
        public int SendDelayBase { get; private set; }
        /// <summary> 额外发送时间 </summary>
        public float SendDelayBonus { get; set; }

        /// <summary> 发送时间 </summary>
        public int SendDelay { get => Math.Clamp((int)(SendDelayBase * SendDelayBonus), 1, int.MaxValue); }

        /// <summary> 当前连接者 </summary>
        public int CurrentConnector => Receivers.Count;

        /// <summary> 发送魔能的计时器 </summary>
        private int _sendTimer;

        public List<Point> Receivers = new List<Point>();

        public MagikeSender()
        {
            Receivers = new List<Point>(MaxConnect);
        }

        public override void Update(IEntity entity)
        {
            //发送时间限制
            if (!CanSend())
                return;

            //获取魔能容器并检测能否发送魔能
            var container = GetMagikeContainer();
            if (!GetSendAmount(container,out int amount))
                return;
            
            //直接发送
            Send(amount);
        }

        public bool CanSend()
        {
            _sendTimer--;
            if (_sendTimer == 0)
            {
                _sendTimer = SendDelay;
                return true;
            }

            return false;
        }

        public bool GetSendAmount(MagikeContainer container, out int amount)
        {
            amount = 0;
            //没有魔能直接返回
            if (!container.HasMagike())
                return false;

            int currentMagike = container.Magike;

            //设置初始发送量
            amount = UnitDelivery;

            //如果魔能量不够挨个发一份那么就把当前剩余的魔能挨个发一份
            if (currentMagike < amount * CurrentConnector)
                amount = currentMagike / CurrentConnector;

            //防止小于1
            if (amount < 1)
                amount = 1;

            return true;
        }

        /// <summary>
        /// 发送魔能
        /// </summary>
        public void Send(int amount)
        {


        }

        public MagikeContainer GetMagikeContainer()
            => Entity.GetSingleComponent(MagikeComponentID.MagikeContainer) as MagikeContainer;

        /// <summary>
        /// 获取魔能量，一定得有唯一的魔能容器才行，没有的话我给你一拳
        /// </summary>
        /// <returns></returns>
        public int GetMagikeAmount()
            => (Entity.GetSingleComponent(MagikeComponentID.MagikeContainer) as MagikeContainer).Magike;

        /// <summary>
        /// 是否已经装满
        /// </summary>
        /// <returns></returns>
        public bool FillUp() => MaxConnect == Receivers.Count;
    }
}
