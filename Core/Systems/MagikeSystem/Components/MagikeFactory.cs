using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Helpers;
using System;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeFactory : MagikeComponent, ITimerTriggerComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeFactory;

        /// <summary> 是否正在工作中 </summary>
        public bool IsWorking { get; set; }

        /// <summary> 基础工作时间 </summary>
        public int WorkTimeBase { get => DelayBase; set => DelayBase = value; }
        /// <summary> 工作时间增幅 </summary>
        public float WorkTimeBonus { get => DelayBonus; set => DelayBonus = value; }

        /// <summary> 工作时间 </summary>
        public int WorkTime { get => Math.Clamp((int)(WorkTimeBase * WorkTimeBonus), 1, int.MaxValue); }

        public int DelayBase { get; set; }
        public float DelayBonus { get; set; } = 1f;
        public int Timer { get; set; }

        public bool TimeResetable => false;

        public virtual bool UpdateTime()
        {
            Timer--;

            if (Timer <= 0)
            {
                Timer = 0;
                return true;
            }

            return false;
        }

        public override void Update()
        {
            if (!IsWorking || DuringWork())
                return;

            IsWorking = false;
            Work();
        }

        public virtual bool DuringWork()
        {
            OnWorking();

            return !UpdateTime();
        }

        /// <summary>
        /// 在工作时触发
        /// </summary>
        public virtual void OnWorking() { }

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
                text = MagikeSystem.GetApparatusDescriptionText(MagikeSystem.ApparatusDescriptionID.IsWorking);

                return false;
            }

            if (!CanActivated_SpecialCheck(out text))
                return false;

            StarkWork();
            return true;
        }

        public abstract bool CanActivated_SpecialCheck(out string text);

        /// <summary>
        /// 开始工作，基方法内包含对<see cref="Timer"/>的赋值
        /// </summary>
        public virtual void StarkWork()
        {
            IsWorking = true;
            Timer = WorkTime;
        }

        public virtual string SendDelayText(MagikeFactory s)
        {
            float timer = MathF.Round(s.Timer / 60f, 1);
            float delay = MathF.Round(s.WorkTime / 60f, 1);
            float delayBase = MathF.Round(s.WorkTimeBase / 60f, 1);
            float DelayBonus = s.WorkTimeBonus;

            return $"  ▶ {timer} / {MagikeHelper.BonusColoredText(delay.ToString(), DelayBonus, true)} ({delayBase} * {MagikeHelper.BonusColoredText(DelayBonus.ToString(), DelayBonus, true)})";
        }

        public override void SaveData(string preName, TagCompound tag)
        {
            //无需存储工作时间以及是否在工作中
            tag.Add(preName + nameof(WorkTimeBase), WorkTimeBase);
            tag.Add(preName + nameof(WorkTimeBonus), WorkTimeBonus);

            if (IsWorking)
            {
                tag.Add(preName + nameof(IsWorking), true);
                tag.Add(preName + nameof(Timer), Timer);
            }
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            WorkTimeBase = tag.GetInt(preName + nameof(WorkTimeBase));
            WorkTimeBonus = tag.GetFloat(preName + nameof(WorkTimeBonus));

            IsWorking = tag.ContainsKey(preName + nameof(IsWorking));
            if (IsWorking)
                Timer = tag.GetInt(preName + nameof(Timer));
        }
    }
}
