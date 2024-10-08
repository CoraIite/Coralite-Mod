﻿using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Helpers;
using Terraria;
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
        public float UnitDeliveryBonus { get; set; } = 1f;

        /// <summary> 单次发送量 </summary>
        public int UnitDelivery { get => (int)(UnitDeliveryBase * UnitDeliveryBonus); }

        /// <summary> 基础发送时间 </summary>
        public int SendDelayBase { get => DelayBase; protected set => DelayBase = value; }
        /// <summary> 发送时间减少量（效率增幅量） </summary>
        public float SendDelayBonus { get => DelayBonus; set => DelayBonus = value; }

        /// <summary> 发送时间 </summary>
        public int SendDelay => (this as ITimerTriggerComponent).Delay;

        public int DelayBase { get; set; }
        public float DelayBonus { get; set; } = 1f;
        public int Timer { get; set; }

        public bool TimeResetable => true;

        public bool CanSend()
        {
            return (this as ITimerTriggerComponent).UpdateTime();
        }

        public virtual void OnSend(Point16 selfPoint, Point16 ReceiverPoint)
        {
            bool selfOnScreen = Helper.OnScreen(selfPoint.ToWorldCoordinates() - Main.screenPosition);
            bool rectiverOnScreen = Helper.OnScreen(ReceiverPoint.ToWorldCoordinates() - Main.screenPosition);

            if (selfOnScreen)
            {
                MagikeHelper.SpawnLozengeParticle_WithTopLeft(selfPoint);
                if (rectiverOnScreen)
                    MagikeHelper.SpawnDustOnSend(selfPoint, ReceiverPoint);
            }

            if (rectiverOnScreen)
                MagikeHelper.SpawnLozengeParticle_WithTopLeft(ReceiverPoint);
        }

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(Timer), Timer);

            tag.Add(preName + nameof(UnitDeliveryBase), UnitDeliveryBase);
            tag.Add(preName + nameof(UnitDeliveryBonus), UnitDeliveryBonus);

            tag.Add(preName + nameof(SendDelayBase), SendDelayBase);
            tag.Add(preName + nameof(SendDelayBonus), SendDelayBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            Timer = tag.GetInt(preName + nameof(Timer));

            UnitDeliveryBase = tag.GetInt(preName + nameof(UnitDeliveryBase));
            UnitDeliveryBonus = tag.GetFloat(preName + nameof(UnitDeliveryBonus));

            if (UnitDeliveryBonus == 0)
                UnitDeliveryBonus = 1;

            SendDelayBase = tag.GetInt(preName + nameof(SendDelayBase));
            SendDelayBonus = tag.GetFloat(preName + nameof(SendDelayBonus));
        }
    }
}
