using Coralite.Core.Systems.MagikeSystem.Attributes;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeSender : MagikeComponent, ITimerTriggerComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeSender;

        /// <summary> 基础单次发送量 </summary>
        [UpgradeableProp]
        public int UnitDeliveryBase { get; protected set; }
        /// <summary> 额外单次发送量 </summary>
        public float UnitDeliveryBonus { get; set; } = 1f;

        /// <summary> 单次发送量 </summary>
        public int UnitDelivery { get => (int)(UnitDeliveryBase * UnitDeliveryBonus); }

        /// <summary> 基础发送时间 </summary>
        [UpgradeableProp]
        public int SendDelayBase { get => DelayBase; protected set => DelayBase = value; }
        /// <summary> 发送时间减少量（效率增幅量） </summary>
        public float SendDelayBonus { get => DelayBonus; set => DelayBonus = value; }

        /// <summary> 发送时间 </summary>
        public int SendDelay => Math.Clamp((int)(DelayBase * DelayBonus), -1, int.MaxValue);

        public int DelayBase { get; set; }
        public float DelayBonus { get; set; } = 1f;
        public int Timer { get; set; }

        public bool TimeResetable => true;

        protected MagikeContainer container;
        /// <summary>
        /// 魔能容器
        /// </summary>
        protected MagikeContainer Container
        {
            get
            {
                container ??= Entity.GetMagikeContainer();
                return container;
            }
        }

        public virtual bool UpdateTime()
        {
            Timer--;
            if (Timer <= 0)
            {
                Timer = SendDelay;
                return true;
            }

            return false;
        }

        public virtual bool CanSend()
        {
            if (!Container.HasMagike)
                return false;

            return UpdateTime();
        }

        /// <summary>
        /// 从一个容器向另一个容器发送魔能
        /// </summary>
        /// <param name="self"></param>
        /// <param name="receiver"></param>
        /// <param name="amount"></param>
        public virtual void SendMagike(MagikeContainer self, MagikeContainer receiver, int amount)
        {
            //增加对面的
            receiver.LimitReceiveOverflow(ref amount);
            receiver.AddMagike(amount);

            //减少自己的
            self.ReduceMagike(amount);
            OnSend(Entity.Position, receiver.Entity.Position);
        }

        /// <summary>
        /// 发送魔能时调用，默认用于生成粒子
        /// </summary>
        /// <param name="selfPoint"></param>
        /// <param name="ReceiverPoint"></param>
        public virtual void OnSend(Point16 selfPoint, Point16 ReceiverPoint)
        {
            bool selfOnScreen = VaultUtils.IsPointOnScreen(selfPoint.ToWorldCoordinates() - Main.screenPosition);
            bool rectiverOnScreen = VaultUtils.IsPointOnScreen(ReceiverPoint.ToWorldCoordinates() - Main.screenPosition);

            if (selfOnScreen)
            {
                SpawnOnSendParticle(selfPoint);
                if (rectiverOnScreen)
                    MagikeHelper.SpawnDustOnSend(selfPoint, ReceiverPoint);
            }

            if (rectiverOnScreen)
                SpawnOnSendParticle(ReceiverPoint);
        }

        protected virtual void SpawnOnSendParticle(Point16 TopLeft)
        {
            MagikeHelper.GetMagikeAlternateData(TopLeft.X, TopLeft.Y, out TileObjectData data, out _);
            Point16 size = data == null ? new Point16(1) : new Point16(data.Width, data.Height);

            if (MagikeHelper.TryGetMagikeApparatusLevel(TopLeft, out ushort level))
                MagikeLozengeParticle2.Spawn(Helper.GetMagikeTileCenter(TopLeft), size, CoraliteContent.GetMagikeLevel(level).LevelColor);
        }

        
        /// <summary>
        /// 获取具体发送多少，最少为剩余量除以所有连接数量
        /// </summary>
        /// <param name="container"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual bool GetSendAmount(MagikeContainer container, out int amount)
        {
            amount = 0;
            //没有魔能直接返回
            if (!container.HasMagike)
                return false;

            int currentMagike = container.Magike;

            //设置初始发送量
            amount = UnitDelivery;

            if (currentMagike < amount)
                amount = currentMagike;

            //防止小于1
            if (amount < 1)
                amount = 1;

            return true;
        }

        public virtual string UnitDeliveryText(MagikeSender s)
            => $"\n  ▶ {MagikeHelper.BonusColoredText(s.UnitDelivery.ToString(), UnitDeliveryBonus)} ({s.UnitDeliveryBase} * {MagikeHelper.BonusColoredText(s.UnitDeliveryBonus.ToString(), UnitDeliveryBonus)})";

        public override void SendData(ModPacket data)
        {
            data.Write(Timer);

            data.Write(UnitDeliveryBase);
            data.Write(UnitDeliveryBonus);

            data.Write(SendDelayBase);
            data.Write(SendDelayBonus);
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            Timer = reader.ReadInt32();

            UnitDeliveryBase = reader.ReadInt32();
            UnitDeliveryBonus = reader.ReadSingle();

            SendDelayBase = reader.ReadInt32();
            SendDelayBonus = reader.ReadSingle();
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
