using System;
using System.Collections.Generic;
using System.Linq;

namespace Coralite.Core.Systems.CoraliteActorComponent
{
    public interface IEntity
    {
        /// <summary>
        /// 使用组件ID查找组件的地方
        /// </summary>
        Dictionary<int, List<Component>> Components { get; }
        /// <summary>
        /// 遍历更新组件的地方
        /// </summary>
        List<Component> ComponentsCache { get; }

        public bool HasComponent(int componentId)
            => Components.ContainsKey(componentId);

        /// <summary>
        /// 向实体内加入组件
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        void AddComponent(Component component)
        {
            if (!Components.ContainsKey(component.ID))
                Components.Add(component.ID, new List<Component>());

            Components[component.ID].Add(component);

            ComponentsCache.Add(component);

            component.Entity = this;
            component.OnAdd(this);
        }

        /// <summary>
        /// 向实体内加入组件，不会触发<see cref="Component.OnAdd(IEntity)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        void AddComponentWithoutOnAdd(Component component)
        {
            if (!Components.ContainsKey(component.ID))
                Components.Add(component.ID, new List<Component>());

            Components[component.ID].Add(component);
            ComponentsCache.Add(component);

            component.Entity = this;
        }

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="listIndex"></param>
        public void RemoveComponent(int componentId, int listIndex)
        {
            if (Components[componentId] == null)
                return;

            Component component = Components[componentId][listIndex];
            Components[componentId].Remove(component);

            ComponentsCache.Remove(component);
            component.OnRemove(this);
        }

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="currentComponent"></param>
        public void RemoveComponent(int componentId, Component currentComponent)
        {
            Components[componentId].Remove(currentComponent);

            ComponentsCache.Remove(currentComponent);
            currentComponent.OnRemove(this);
        }

        /// <summary>
        /// 移除一种组件
        /// </summary>
        /// <param name="componentId"></param>
        public void RemoveAllComponent(int componentId)
        {
            if (Components[componentId] == null)
                return;

            for (int i = 0; i < Components[componentId].Count; i++)
            {
                Component component = Components[componentId][i];
                Components[componentId].Remove(component);

                ComponentsCache.Remove(component);
                component.OnRemove(this);
            }
        }

        /// <summary>
        /// 移除所有组件
        /// </summary>
        public void RemoveAllComponent()
        {
            for (int i = 0; i < ComponentsCache.Count; i++)
                ComponentsCache[i].OnRemove(this);

            ComponentsCache.Clear();
            Components.Clear();
        }

        public bool TryGetComponent<T>(int index, out T result) where T : Component
        {
            result = null;
            if (!HasComponent(index))
                return false;

            result = (T)Components[index].FirstOrDefault(c => c is T, null);
            return result != null;
        }

        public Component GetSingleComponent(int index)
        {
            if (!HasComponent(index))
                throw new Exception("所查找的组件不存在！");

            return Components[index].First();
        }

        public T GetSingleComponent<T>(int index) where T : Component
        {
            if (!HasComponent(index))
                throw new Exception("所查找的组件不存在！");

            return (T)Components[index].First();
        }
    }
}
