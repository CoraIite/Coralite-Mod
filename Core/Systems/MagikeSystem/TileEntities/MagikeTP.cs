﻿using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using InnoVault.TileProcessors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeTP : TileProcessor, IEntity<MagikeComponent>
    {
        public const string SaveName = "Component";

        internal static int playerInWorldTime;

        internal const string GUID = "CoraliteMod_GUIDValue-XZ90-UO98-MK87";

        /// <summary>
        /// 存储各类组件的地方<br></br>
        /// 不同类型的组件分为单一实例以及多实例，具体可以调用<see cref="MagikeComponentID.IsSingleton(int)"/>来查看<br></br>
        /// 多实例的组件使用<see cref="List{T}"/>来存储
        /// </summary>
        public HybridDictionary Components { get; private set; }
        /// <summary>
        /// 为了提高性能，在遍历组件进行更新等操作时使用这个<br></br>
        /// 如果想要精确获取某个组件请使用<see cref="Components"/>
        /// </summary>
        public List<MagikeComponent> ComponentsCache { get; private set; }

        /// <summary>
        /// 扩展滤镜容量<br></br>
        /// 该值无法随意更改
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

            return ((List<MagikeComponent>)Components[MagikeComponentID.MagikeFilter]).Count < ExtendFilterCapacity;
        }

        public override void Update()
        {
            for (int i = 0; i < ComponentsCache.Count; i++)
                ComponentsCache[i].Update();
        }

        public override void SetProperty()
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

        public override void SendData(ModPacket data)
        {
            //$"SendData-ComponentsCache.Count:{ComponentsCache.Count}".LoggerDomp();
            if (TileProcessorNetWork.InitializeWorld)
            {
                data.Write(ComponentsCache.Count);
                for (int i = 0; i < ComponentsCache.Count; i++)
                {
                    MagikeComponent component = ComponentsCache[i];
                    string fullName = component.GetType().FullName;
                    data.Write(GUID);
                    data.Write(fullName);
                    component.SendData(data);
                }
            }
            else
            {
                for (int i = 0; i < ComponentsCache.Count; i++)
                {
                    ComponentsCache[i].SendData(data);
                }
            }
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            //$"ReceiveData-ComponentsCache.Count:{ComponentsCache.Count}".LoggerDomp();
            if (TileProcessorNetWork.InitializeWorld)
            {
                InitializeComponentCache();
                int leng = reader.ReadInt32();
                for (int i = 0; i < leng; i++)
                {
                    string guiD = reader.ReadString();
                    if (guiD != GUID)
                    {
                        continue;
                    }
                    string fullName = reader.ReadString();
                    Type t = Type.GetType(fullName);
                    MagikeComponent component = (MagikeComponent)Activator.CreateInstance(t);
                    component.ReceiveData(reader, whoAmI);
                    AddComponentWithoutOnAdd(component);
                }
            }
            else
            {
                for (int i = 0; i < ComponentsCache.Count; i++)
                {
                    ComponentsCache[i].ReceiveData(reader, whoAmI);
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < ComponentsCache.Count; i++)
            {
                MagikeComponent component = ComponentsCache[i];

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

                var component = (MagikeComponent)Activator.CreateInstance(t);
                component.LoadData(SaveName + i.ToString(), tag);
                i++;

                AddComponentWithoutOnAdd(component);
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
        public void AddComponent(MagikeComponent component)
        {
            AddComponentWithoutOnAdd(component);
            component.OnAdd(this);
        }

        /// <summary>
        /// 向实体内加入组件，不会触发<see cref="MagikeComponent.OnAdd(IEntity)"/>
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public void AddComponentWithoutOnAdd(MagikeComponent component)
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
                    Components.Add(component.ID, new List<MagikeComponent>());

                ((List<MagikeComponent>)Components[component.ID]).Add(component);
            }

            ComponentsCache.Add(component);

            component.Entity = this;
        }

        /// <summary>
        /// 移除一个组件
        /// </summary>
        /// <param name="currentComponent"></param>
        public void RemoveComponent(MagikeComponent currentComponent)
        {
            RemoveComponentWithoutOnRemove(currentComponent);
            currentComponent.OnRemove(this);
            SendData();
        }

        /// <summary>
        /// 移除一个组件，不触发<see cref="MagikeComponent.OnRemove(IEntity)"/>
        /// </summary>
        /// <param name="currentComponent"></param>
        public void RemoveComponentWithoutOnRemove(MagikeComponent currentComponent)
        {
            int id = currentComponent.ID;
            if (MagikeComponentID.IsSingleton(id))//该组件为单例形态
                Components.Remove(id);
            else//该组件需要多重存在
                ((List<MagikeComponent>)Components[id]).Remove(currentComponent);

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

        public bool TryGetComponent<T>(int id, out T result) where T : MagikeComponent
        {
            result = null;
            if (!HasComponent(id))
                return false;

            if (MagikeComponentID.IsSingleton(id))
                result = (T)Components[id];
            else
                result = (T)((List<MagikeComponent>)Components[id]).FirstOrDefault(c => c is T, null);

            return result != null;
        }

        public MagikeComponent GetSingleComponent(int index)
        {
            if (!HasComponent(index))
                $"所查找的组件不存在！index:{index}".LoggerDomp();

            if (MagikeComponentID.IsSingleton(index))
                return (MagikeComponent)Components[index];
            else
                return ((List<MagikeComponent>)Components[index]).First();
        }

        public T GetSingleComponent<T>(int index) where T : MagikeComponent
        {
            if (!HasComponent(index))
                $"所查找的组件不存在！name:{nameof(T)},index:{index}".LoggerDomp();

            if (MagikeComponentID.IsSingleton(index))
                return (T)Components[index];
            else
                return (T)((List<MagikeComponent>)Components[index]).First();
        }

        #endregion
    }

    public class MagikeGloblaTP : GlobalTileProcessor
    {
        public override Point16? GetTopLeftPoint(int x, int y)
        {
            Tile t = Framing.GetTileSafely(x, y);
            if (t.TileType < TileID.Count)
                return null;

            ModTile mt = TileLoader.GetTile(t.TileType);

            if (mt is BaseMagikeTile)
                return MagikeHelper.ToTopLeft(x, y);

            return null;
        }

        public override bool? TryIsTopLeftPoint(int x, int y, out Point16 position)
        {
            position = default;
            Tile t = Framing.GetTileSafely(x, y);
            if (t.TileType < TileID.Count)
                return null;

            ModTile mt = TileLoader.GetTile(t.TileType);

            if (mt is BaseMagikeTile)
            {
                Point16? p = MagikeHelper.ToTopLeft(x, y);
                if (p.HasValue)
                {
                    position = p.Value;
                    return x == position.X && y == position.Y;
                }
            }

            return null;
        }
    }
}
