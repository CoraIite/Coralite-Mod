using System;

namespace Coralite.Core.Systems.CoraliteActorComponent
{
    //并非完全的ECS系统
    //继承后加载
    public abstract class Component
    {
        //正在考虑加入“可拆卸”的属性

        public abstract int ID { get; }

        public virtual void OnAdd(IEntity entity) { }
        public virtual void OnRemove(IEntity entity) { }

        public abstract void Update(IEntity entity);
    }
}
