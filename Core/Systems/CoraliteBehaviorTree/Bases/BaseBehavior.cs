using Coralite.Systems.CoraliteBehaviorTree.Enums;
using Coralite.Systems.CoraliteBehaviorTree.Interfaces;

namespace Coralite.Systems.CoraliteBehaviorTree.Bases
{
    public abstract class BaseBehavior : IBehaviour
    {
        protected EStatus status;

        //创建对象请调用Create() 释放对象请调用Release()
        protected BaseBehavior()
        {
            SetStatus(EStatus.Invalid);
        }

        //update方法被首次调用前执行OnInitlize方法，每次行为树更新时调用一次update方法
        //当刚刚更新的行为不再运行时调用OnTerminate方法
        public EStatus Tick()
        {
            if (status != EStatus.Running)
                OnInitialize();

            status = Update();

            if (status != EStatus.Running)
                OnTerminate(status);

            return status;
        }

        public void SetStatus(EStatus status)
        {
            this.status = status;
        }

        public EStatus GetStatus()
        {
            return status;
        }

        public virtual void Release() { }

        public virtual void OnInitialize() { }

        public virtual void OnTerminate(EStatus Status) { }

        public virtual void Abort()
        {
            OnTerminate(EStatus.Aborted);
            SetStatus(EStatus.Aborted);
        }

        public virtual void Reset()
        {
            SetStatus(EStatus.Invalid);
        }

        public abstract void AddChild(IBehaviour child);

        public abstract EStatus Update();

    }
}
