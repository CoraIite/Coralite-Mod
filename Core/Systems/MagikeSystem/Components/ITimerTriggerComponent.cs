using System;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public interface ITimerTriggerComponent
    {
        /// <summary> 基础时间 </summary>
        int DelayBase { get; set; }
        /// <summary> 时间减少量（效率增幅量） </summary>
        float DelayBonus { get; set; }

        /// <summary> 发送时间 </summary>
        int Delay
        {
            get => Math.Clamp((int)(DelayBase * DelayBonus), 1, int.MaxValue);
        }

        int Timer { get; set; }

        public bool UpdateTime()
        {
            Timer--;
            if (Timer <= 0)
            {
                Timer = Delay;
                return true;
            }

            return false;
        }

    }
}
