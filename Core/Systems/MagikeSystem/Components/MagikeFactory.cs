using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeFactory : Component, ITimerTriggerComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeFactory;

        /// <summary> 是否正在工作中 </summary>
        public bool IsWorking { get; set; }

        /// <summary> 基础工作时间 </summary>
        public int WorkTimeBase  { get => DelayBase; set => DelayBase = value; }
        /// <summary> 工作时间增幅 </summary>
        public float WorkTimeBonus { get => DelayBonus; set => DelayBonus = value; }

        /// <summary> 工作时间 </summary>
        public int WorkTime { get => Math.Clamp((int)(WorkTimeBase * WorkTimeBonus), 1, int.MaxValue); }

        public int DelayBase { get; set; }
        public float DelayBonus { get; set; }
        public int Timer { get; set ; }

        public bool UpdateTime()
        {
            Timer--;

            if (Timer <= 0)
            {
                Timer = 0;
                return true;
            }

            return false;
        }

        public override void Update(IEntity entity)
        {
            if (!IsWorking || !UpdateTime())
                return;

            IsWorking = false;
            Work();
        }

        /// <summary>
        /// 特定的工作
        /// </summary>
        public virtual void Work() { }

        /// <summary>
        /// 是否能被激活
        /// </summary>
        /// <returns></returns>
        public bool Activation(out string text)
        {
            text = "";
            if (IsWorking)
            {
                return false;
            }

            if (!CanActivated_SpecialCheck(out text))
                return false;

            StarkWork();
            return true;
        }

        public abstract bool CanActivated_SpecialCheck(out string text);

        public virtual void StarkWork()
        {
            IsWorking = true;
            Timer = WorkTime;
        }

        public override void SaveData(string preName, TagCompound tag)
        {
            //无需存储工作时间以及是否在工作中
            tag.Add(preName + nameof(WorkTimeBase), WorkTimeBase);
            tag.Add(preName + nameof(WorkTimeBonus), WorkTimeBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            WorkTimeBase = tag.GetInt(preName + nameof(WorkTimeBase));
            WorkTimeBonus = tag.GetFloat(preName + nameof(WorkTimeBonus));
        }
    }
}
