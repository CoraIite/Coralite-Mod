using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeSender : Component
    {
        public sealed override int ID => MagikeComponentID.MagikeSender;

        /// <summary> 基础单次发送量 </summary>
        public int UnitDeliveryBase { get; private set; }
        /// <summary> 额外单次发送量 </summary>
        public int UnitDeliveryExtra { get; set; }

        /// <summary> 单次发送量 </summary>
        public int UnitDelivery { get => UnitDeliveryBase + UnitDeliveryExtra; }

        /// <summary> 基础发送时间 </summary>
        public int SendDelayBase { get; private set; }
        /// <summary> 发送时间减少量（效率增幅量） </summary>
        public float SendDelayBonus { get; set; } = 1f;

        /// <summary> 发送时间 </summary>
        public int SendDelay { get => Math.Clamp((int)(SendDelayBase * SendDelayBonus), 1, int.MaxValue); }

        /// <summary> 发送魔能的计时器 </summary>
        private int _sendTimer;

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

        public void OnSend(Point16 selfPoint,Point ReceiverPoint)
        {

        }

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(_sendTimer), _sendTimer);

            tag.Add(preName + nameof(UnitDeliveryBase), UnitDeliveryBase);
            tag.Add(preName + nameof(UnitDeliveryExtra), UnitDeliveryExtra);

            tag.Add(preName + nameof(SendDelayBase), SendDelayBase);
            tag.Add(preName + nameof(SendDelayBonus), SendDelayBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            _sendTimer = tag.GetInt(preName + nameof(_sendTimer));

            UnitDeliveryBase = tag.GetInt(preName + nameof(UnitDeliveryBase));
            UnitDeliveryExtra = tag.GetInt(preName + nameof(UnitDeliveryExtra));

            SendDelayBase = tag.GetInt(preName + nameof(SendDelayBase));
            SendDelayBonus = tag.GetFloat(preName + nameof(SendDelayBonus));
        }
    }
}
