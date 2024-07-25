using Coralite.Core.Systems.CoraliteActorComponent;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeSender : Component, ITimerTriggerComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeSender;

        /// <summary> 基础单次发送量 </summary>
        public int UnitDeliveryBase { get; protected set; }
        /// <summary> 额外单次发送量 </summary>
        public int UnitDeliveryExtra { get; set; }

        /// <summary> 单次发送量 </summary>
        public int UnitDelivery { get => UnitDeliveryBase + UnitDeliveryExtra; }

        /// <summary> 基础发送时间 </summary>
        public int SendDelayBase { get => DelayBase; protected set => DelayBase = value; }
        /// <summary> 发送时间减少量（效率增幅量） </summary>
        public float SendDelayBonus { get => DelayBonus; set => DelayBonus = value; }

        /// <summary> 发送时间 </summary>
        public int SendDelay => (this as ITimerTriggerComponent).Delay;

        public int DelayBase { get; set; }
        public float DelayBonus { get; set; } = 1f;
        public int Timer { get; set; }

        public bool CanSend()
        {
            return (this as ITimerTriggerComponent).UpdateTime();
        }

        public void OnSend(Point16 selfPoint, Point ReceiverPoint)
        {

        }

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(Timer), Timer);

            tag.Add(preName + nameof(UnitDeliveryBase), UnitDeliveryBase);
            tag.Add(preName + nameof(UnitDeliveryExtra), UnitDeliveryExtra);

            tag.Add(preName + nameof(SendDelayBase), SendDelayBase);
            tag.Add(preName + nameof(SendDelayBonus), SendDelayBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            Timer = tag.GetInt(preName + nameof(Timer));

            UnitDeliveryBase = tag.GetInt(preName + nameof(UnitDeliveryBase));
            UnitDeliveryExtra = tag.GetInt(preName + nameof(UnitDeliveryExtra));

            SendDelayBase = tag.GetInt(preName + nameof(SendDelayBase));
            SendDelayBonus = tag.GetFloat(preName + nameof(SendDelayBonus));
        }
    }
}
