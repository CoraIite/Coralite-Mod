using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeLinerSender : MagikeSender
    {
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

        /// <summary> 当前连接者 </summary>
        public int CurrentConnector => _receivers.Count;

        private List<Point16> _receivers = new List<Point16>();

        public MagikeLinerSender()
        {
            _receivers = new List<Point16>(MaxConnect);
        }

        #region 发送工作相关

        public override void Update(IEntity entity)
        {
            //发送时间限制
            if (!CanSend())
                return;

            //获取魔能容器并检测能否发送魔能
            MagikeContainer container = Entity.GetMagikeContainer();
            if (!GetSendAmount(container, out int amount))
                return;

            //直接发送
            for (int i = 0; i < _receivers.Count; i++)
            {
                if (!container.HasMagike)//自身没魔能了就跳出
                    break;
                Send(container, _receivers[i], amount);
            }
        }

        /// <summary>
        /// 获取具体发送多少，最少为剩余量除以所有连接数量
        /// </summary>
        /// <param name="container"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool GetSendAmount(MagikeContainer container, out int amount)
        {
            amount = 0;
            //没有魔能直接返回
            if (!container.HasMagike)
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
        public void Send(MagikeContainer selfMagikeContainer, Point16 position, int amount)
        {
            //如果无法获取物块实体就移除
            if (MagikeHelper.TryGetEntity(position, out BaseMagikeTileEntity receiverEntity))
                goto remove;

            //如果不是魔能容器那么就丢掉喽
            if (!receiverEntity.IsMagikeContainer())
                goto remove;

            /*
             * 对于传入的魔能数量，分为以下几种情况
             *  - 接收者能接受，就全额发送
             *  - 接收者无法接受全部，就发送接收者能接受的数量
             *  
             *  由于已经事先判断过自身魔能容量所以这里必定有足够的魔能来发送
             */

            MagikeContainer receiver = receiverEntity.GetMagikeContainer();
            if (receiver.FullMagike)
                return;

            //限制不溢出
            receiver.LimitReceiveOverflow(ref amount);

            receiver.AddMagike(amount);
            selfMagikeContainer.ReduceMagike(amount);

            return;
        remove:
            RemoveReceiver(position);
        }

        #endregion

        #region 连接相关

        public void Connect(Point16 receiverPoint)
        {
            _receivers.Add(receiverPoint);
        }

        /// <summary>
        /// 移除接收者
        /// </summary>
        /// <param name="receiverPoint"></param>
        public void RemoveReceiver(Point16 receiverPoint) => _receivers.Remove(receiverPoint);

        #endregion

        /// <summary>
        /// 获取魔能量，一定得有唯一的魔能容器才行，没有的话我给你一拳
        /// </summary>
        /// <returns></returns>
        public int GetMagikeAmount()
            => Entity.GetMagikeContainer().Magike;

        /// <summary>
        /// 是否已经装满
        /// </summary>
        /// <returns></returns>
        public bool FillUp() => MaxConnect == _receivers.Count;

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);
            tag.Add(preName + nameof(MaxConnectBase), MaxConnectBase);
            tag.Add(preName + nameof(MaxConnectExtra), MaxConnectExtra);

            tag.Add(preName + nameof(ConnectLengthBase), ConnectLengthBase);
            tag.Add(preName + nameof(ConnectLengthExtra), ConnectLengthExtra);

            for (int i = 0; i < _receivers.Count; i++)
                tag.Add(preName + nameof(_receivers) + i, _receivers[i]);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);

            MaxConnectBase = tag.GetInt(preName + nameof(MaxConnectBase));
            MaxConnectExtra = tag.GetInt(preName + nameof(MaxConnectExtra));

            ConnectLengthBase = tag.GetInt(preName + nameof(ConnectLengthBase));
            ConnectLengthExtra = tag.GetInt(preName + nameof(ConnectLengthExtra));

            int i = 0;

            while (tag.TryGet(preName + nameof(_receivers) + i, out Point16 position))
            {
                _receivers.Add(position);
                i++;
            }
        }
    }
}
