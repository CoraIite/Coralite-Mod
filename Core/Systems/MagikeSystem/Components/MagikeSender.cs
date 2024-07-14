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

        /// <summary> 发送魔能的计时器 </summary>
        private int _sendTimer;

        /// <summary>
        /// 是否已经装满
        /// </summary>
        /// <returns></returns>
        public bool FillUp() => MaxConnect == Receivers.Count;

        public List<Point> Receivers = new List<Point>();

        public MagikeSender()
        {
            Receivers = new List<Point>(MaxConnect);
        }

        public override void Update(IEntity entity)
        {

        }
    }
}
