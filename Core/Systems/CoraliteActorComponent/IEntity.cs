using System.Collections.Generic;
using System.Collections.Specialized;

namespace Coralite.Core.Systems.CoraliteActorComponent
{
    public interface IEntity<TComponent> 
    {
        /// <summary> 使用组件ID查找组件的地方 </summary>
        HybridDictionary Components { get; }
        /// <summary> 遍历更新组件的地方 </summary>
        List<TComponent> ComponentsCache { get; }

        public bool HasComponent(int componentId)
            => Components.Contains(componentId);

        /// <summary>
        /// 向实体内加入组件
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        void AddComponent(TComponent component);

        /// <summary>
        /// 向实体内加入组件，不会触发<see cref="MagikeComponent.OnAdd(IEntity)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        void AddComponentWithoutOnAdd(TComponent component);

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <param name="currentComponent"></param>
        public void RemoveComponent(TComponent currentComponent);

        /// <summary>
        /// 移除一个组件，不触发<see cref="MagikeComponent.OnRemove(IEntity)"/>
        /// </summary>
        /// <param name="currentComponent"></param>
        public void RemoveComponentWithoutOnRemove(TComponent currentComponent);

        /// <summary>
        /// 移除一种组件
        /// </summary>
        /// <param name="componentId"></param>
        public void RemoveAllComponent(int componentId);

        /// <summary>
        /// 移除所有组件
        /// </summary>
        public void RemoveAllComponent();

        public bool TryGetComponent<T>(int id, out T result) where T :TComponent;

        public TComponent GetSingleComponent(int index);

        public T GetSingleComponent<T>(int index) where T : TComponent;
    }
}
