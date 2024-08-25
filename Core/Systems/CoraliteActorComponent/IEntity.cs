using Coralite.Core.Systems.MagikeSystem;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Coralite.Core.Systems.CoraliteActorComponent
{
    public interface IEntity
    {
        /// <summary> 使用组件ID查找组件的地方 </summary>
        HybridDictionary Components { get; }
        /// <summary> 遍历更新组件的地方 </summary>
        List<Component> ComponentsCache { get; }

        public bool HasComponent(int componentId)
            => Components.Contains(componentId);

        /// <summary>
        /// 向实体内加入组件
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        void AddComponent(Component component)
        {
            AddComponentWithoutOnAdd(component);
            component.OnAdd(this);
        }

        /// <summary>
        /// 向实体内加入组件，不会触发<see cref="Component.OnAdd(IEntity)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        void AddComponentWithoutOnAdd(Component component)
        {
            if (MagikeComponentID.IsSingleton(component.ID))//该组件为单例形态
            {
                if (!Components.Contains(component.ID))
                    Components.Add(component.ID, component);
                else
                    Components[component.ID] = component;
            }
            else//该组件需要多重存在
            {
                if (!Components.Contains(component.ID))
                    Components.Add(component.ID, new List<Component>());

                ((List<Component>)(Components[component.ID])).Add(component);
            }

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

            Component component;

            if (MagikeComponentID.IsSingleton(componentId))//该组件为单例形态
            {
                 component = (Component)(Components[componentId]);
                Components.Remove(componentId);
            }
            else//该组件需要多重存在
            {
                component = ((List<Component>)(Components[componentId]))[listIndex];
                ((List<Component>)(Components[componentId])).Remove(component);
            }

            ComponentsCache.Remove(component);
            component.OnRemove(this);
        }

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <param name="currentComponent"></param>
        public void RemoveComponent( Component currentComponent)
        {
            RemoveComponentWithoutOnRemove( currentComponent);
            currentComponent.OnRemove(this);
        }

        /// <summary>
        /// 移除一个组件，不触发<see cref="Component.OnRemove(IEntity)"/>
        /// </summary>
        /// <param name="currentComponent"></param>
        public void RemoveComponentWithoutOnRemove( Component currentComponent)
        {
            int id = currentComponent.ID;
            if (MagikeComponentID.IsSingleton(id))//该组件为单例形态
                Components.Remove(id);
            else//该组件需要多重存在
                ((List<Component>)(Components[id])).Remove(currentComponent);

            ComponentsCache.Remove(currentComponent);
        }

        /// <summary>
        /// 移除一种组件
        /// </summary>
        /// <param name="componentId"></param>
        public void RemoveAllComponent(int componentId)
        {
            if (Components[componentId] == null)
                return;

            Components.Remove(componentId);
            ComponentsCache.RemoveAll(c=>c.ID == componentId);
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

            if (MagikeComponentID.IsSingleton(index))
                result = (T)Components[index];
            else
                result = (T)((List<Component>)Components[index]).FirstOrDefault(c => c is T, null);

            return result != null;
        }

        public Component GetSingleComponent(int index)
        {
            if (!HasComponent(index))
                throw new Exception("所查找的组件不存在！");

            if (MagikeComponentID.IsSingleton(index))
                return (Component)Components[index];
            else
                return ((List<Component>)Components[index]).First();
        }

        public T GetSingleComponent<T>(int index) where T : Component
        {
            if (!HasComponent(index))
                throw new Exception("所查找的组件不存在！");

            if (MagikeComponentID.IsSingleton(index))
                return (T)Components[index];
            else
                return (T)((List<Component>)Components[index]).First();
        }
    }
}
