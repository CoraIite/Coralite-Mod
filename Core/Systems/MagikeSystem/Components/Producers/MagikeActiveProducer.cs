﻿using Coralite.Helpers;
using System;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components.Producers
{
    public abstract class MagikeActiveProducer : MagikeProducer, ITimerTriggerComponent
    {
        /// <summary> 基础生产时间 </summary>
        public int ProductionDelayBase { get => DelayBase; protected set => DelayBase = value; }
        /// <summary> 发送时间减少量（效率增幅量） </summary>
        public float ProductionDelayBonus { get => DelayBonus; set => DelayBonus = value; }

        /// <summary> 生产时间 </summary>
        public int ProductionDelay => (this as ITimerTriggerComponent).Delay;

        public int DelayBase { get; set; }
        public float DelayBonus { get; set; } = 1f;
        public int Timer { get; set; }

        public bool TimeResetable => true;

        public bool CheckTime()
        {
            return (this as ITimerTriggerComponent).UpdateTime();
        }

        public bool CanProduce_CheckMagike()
        {
            return !Entity.GetMagikeContainer().FullMagike;
        }

        /// <summary>
        /// 重写这个检测是否能生产
        /// </summary>
        public override bool CanProduce()
        {
            //魔能容量限制
            //其他特定的特殊条件
            return CanProduce_CheckMagike() && CanProduce_SpecialCheck();
        }

        public virtual bool CanProduce_SpecialCheck() => true;

        public override void Update()
        {
            //生产时间限制
            if (!CheckTime())
                return;

            if (!CanProduce())
                return;

            Produce();
        }

        public virtual string ProductionDelayText(MagikeActiveProducer p)
        {
            float timer = MathF.Round(p.Timer / 60f, 1);
            float delay = MathF.Round(p.ProductionDelay / 60f, 1);
            float delayBase = MathF.Round(p.ProductionDelayBase / 60f, 1);
            float DelayBonus = p.ProductionDelayBonus;

            return $"  ▶ {timer} / {MagikeHelper.BonusColoredText(delay.ToString(), DelayBonus, true)} ({delayBase} * {MagikeHelper.BonusColoredText(DelayBonus.ToString(), DelayBonus, true)})";
        }

        public virtual string ThroughputText(MagikeActiveProducer p)
            => $"  ▶ {MagikeHelper.BonusColoredText(p.Throughput.ToString(), p.ThroughputBonus)} ({p.ThroughputBase} * {MagikeHelper.BonusColoredText(p.ThroughputBonus.ToString(), p.ThroughputBonus)})";

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);

            tag.Add(preName + nameof(Timer), Timer);

            tag.Add(preName + nameof(ProductionDelayBase), ProductionDelayBase);
            tag.Add(preName + nameof(ProductionDelayBonus), ProductionDelayBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);

            Timer = tag.GetInt(preName + nameof(Timer));

            ProductionDelayBase = tag.GetInt(preName + nameof(ProductionDelayBase));
            ProductionDelayBonus = tag.GetFloat(preName + nameof(ProductionDelayBonus));
        }
    }
}
