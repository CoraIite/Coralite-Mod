
using System;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能工厂类，可以实现想要的功能
    /// </summary>
    public abstract class MagikeFactory : MagikeContainer
    {
        public int workTimer = -1;
        public readonly int workTimeMax;

        public event Action OnWorkFinshed;

        public MagikeFactory(int magikeMax, int workTimeMax) : base(magikeMax)
        {
            this.workTimeMax = workTimeMax;
        }

        public override void Update()
        {
            if (CanWork())
                Work();
        }

        /// <summary>
        /// 帮助方法，意为开始工作
        /// </summary>
        public virtual bool StartWork()
        {
            if (workTimer == -1)
            {
                workTimer = 0;
                return true;
            }

            return false;
        }

        public virtual bool CanWork()
        {
            return workTimer >= 0;
        }

        public virtual void Work()
        {
            workTimer++;
            if (workTimer >= workTimeMax)
            {
                workTimer = -1;
                OnWorkFinshed?.Invoke();
                WorkFinish();
            }
            else
                DuringWork();
        }

        public virtual void DuringWork (){ }

        /// <summary>
        /// 工作完成，在此执行对应的物品消耗，魔能消耗等工作
        /// </summary>
        public virtual void WorkFinish() { }
    }
}
