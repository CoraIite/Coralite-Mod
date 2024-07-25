using Coralite.Core.Systems.CoraliteActorComponent;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeFilter : Component
    {
        public sealed override int ID => MagikeComponentID.MagikeFilter;

        /// <summary>
        /// 对应的物品类型，在替换时弹出以及在物块破坏时弹出
        /// </summary>
        public abstract int ItemType { get; }

        public override void OnAdd(IEntity entity)
        {
            for (int i = 0; i < entity.ComponentsCache.Count; i++)
                ChangeComponentValues(entity.ComponentsCache[i]);
        }

        public override void OnRemove(IEntity entity)
        {
            for (int i = 0; i < entity.ComponentsCache.Count; i++)
                RestoreComponentValues(entity.ComponentsCache[i]);
        }

        public virtual void ChangeComponentValues(Component component) { }

        public virtual void RestoreComponentValues(Component component) { }
    }
}
