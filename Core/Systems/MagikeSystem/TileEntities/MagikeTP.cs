using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using InnoVault.TileProcessors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeTP : TileProcessor, IEntity
    {
        public const string SaveName = "Component";

        public HybridDictionary Components { get; private set; }
        public List<Component> ComponentsCache { get; private set; }

        /// <summary>
        /// 扩展滤镜容量
        /// </summary>
        public virtual int ExtendFilterCapacity { get => 2; }

        /// <summary>
        /// 检测滤镜容量，如果已经满了那么就无法插入
        /// </summary>
        /// <returns></returns>
        public bool CanInsertFilter()
        {
            if (!Components.Contains(MagikeComponentID.MagikeFilter))
                return true;

            return ((List<Component>)Components[MagikeComponentID.MagikeFilter]).Count < ExtendFilterCapacity;
        }

        public override void Update()
        {
            for (int i = 0; i < ComponentsCache.Count; i++)
                ComponentsCache[i].Update(this);
        }

        public MagikeTP()
        {
            InitializeComponentCache();
            InitializeBeginningComponent();
        }

        /// <summary>
        /// 初始化用于存储组件的字典和数组
        /// </summary>
        public void InitializeComponentCache()
        {
            Components = [];
            ComponentsCache = [];
        }

        /// <summary>
        /// 初始化起始时的组件
        /// </summary>
        public abstract void InitializeBeginningComponent();

        public override void OnKill()
        {
            RemoveAllComponent();
        }


        #region 数据存储

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < ComponentsCache.Count; i++)
            {
                Component component = ComponentsCache[i];

                string fullName = component.GetType().FullName;
                string preName = SaveName + i.ToString();

                //存储全名以在加载时找到
                tag.Add(preName, fullName);

                component.SaveData(preName, tag);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            InitializeComponentCache();

            int i = 0;
            while (tag.TryGet(SaveName + i.ToString(), out string fullName))
            {
                var t = System.Type.GetType(fullName);
                if (t is null)
                {
                    i++;
                    continue;
                }

                var component = (Component)Activator.CreateInstance(t);
                component.LoadData(SaveName + i.ToString(), tag);
                i++;

                (this as IEntity).AddComponentWithoutOnAdd(component);
            }
        }

        #endregion

        #region 来自基类

        public bool HasComponent(int componentId)
            => Components.Contains(componentId);

        /// <summary>
        /// 向实体内加入组件
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public void AddComponent(Component component)
        {
            AddComponentWithoutOnAdd(component);
            component.OnAdd(this);
        }

        /// <summary>
        /// 向实体内加入组件，不会触发<see cref="Component.OnAdd(IEntity)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public void AddComponentWithoutOnAdd(Component component)
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

                ((List<Component>)Components[component.ID]).Add(component);
            }

            ComponentsCache.Add(component);

            component.Entity = this;
        }

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <param name="currentComponent"></param>
        public void RemoveComponent(Component currentComponent)
        {
            RemoveComponentWithoutOnRemove(currentComponent);
            currentComponent.OnRemove(this);
        }

        /// <summary>
        /// 移除一个组件，不触发<see cref="Component.OnRemove(IEntity)"/>
        /// </summary>
        /// <param name="currentComponent"></param>
        public void RemoveComponentWithoutOnRemove(Component currentComponent)
        {
            int id = currentComponent.ID;
            if (MagikeComponentID.IsSingleton(id))//该组件为单例形态
                Components.Remove(id);
            else//该组件需要多重存在
                ((List<Component>)Components[id]).Remove(currentComponent);

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
            ComponentsCache.RemoveAll(c => c.ID == componentId);
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

        public bool TryGetComponent<T>(int id, out T result) where T : Component
        {
            result = null;
            if (!HasComponent(id))
                return false;

            if (MagikeComponentID.IsSingleton(id))
                result = (T)Components[id];
            else
                result = (T)((List<Component>)Components[id]).FirstOrDefault(c => c is T, null);

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

        #endregion
    }

    public class MagikeGloblaTP:GlobalTileProcessor
    {
        public override Point16? PlaceInWorldGetTopLeftPoint(int x, int y)
        {
            Tile t=Framing.GetTileSafely(x, y);
            if (t.TileType < TileID.Count)
                return null;

            ModTile mt = TileLoader.GetTile(t.TileType);

            if (mt is BaseMagikeTile)
                return MagikeHelper.ToTopLeft(x, y);

            return null;
        }
    }
}
