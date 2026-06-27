using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using InnoVault.TileProcessors;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeTP : TileProcessor, IEntity<MagikeComponent>
    {
        public const string SaveName = "Component";

        //internal static int playerInWorldTime;

        internal const string GUID = "UO98-MK87";

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
        /// 在UI中显示的主要组件，会在开启UI的时候设置
        /// </summary>
        public abstract int MainComponentID { get; }

        /// <summary>
        /// 获取主要组件在列表中的索引
        /// </summary>
        /// <returns></returns>
        public int GetMainComponentIndex()
            => ComponentsCache.FindIndex(c => c.ID == MainComponentID);

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

        public static bool Call_IsMagikeContainer(Point16 point, out MagikeContainer reset)
        {
            reset = null;

            if (!VaultUtils.SafeGetTopLeft(point.X, point.Y, out var newPoint))
            {
                return false;
            }

            if (!TileProcessorLoader.ByPositionGetTP(newPoint, out TileProcessor tp))
            {
                return false;
            }

            if (tp is not MagikeTP magikeTP)
            {
                return false;
            }

            if (magikeTP.IsMagikeContainer())
            {
                reset = magikeTP.GetMagikeContainer();
                return true;
            }

            return false;
        }

        public static bool Call_AddMagike(Point16 point, int amount)
        {
            if (!Call_IsMagikeContainer(point, out MagikeContainer reset))
            {
                return false;
            }
            reset.AddMagike(amount);
            return true;
        }

        public static bool Call_ReduceMagike(Point16 point, int amount)
        {
            if (!Call_IsMagikeContainer(point, out MagikeContainer reset))
            {
                return false;
            }
            reset.ReduceMagike(amount);
            return true;
        }

        public static MagikeContainerData Call_GetMagikeContainerData(Point16 point)
        {
            if (!Call_IsMagikeContainer(point, out MagikeContainer reset))
            {
                return default;
            }
            MagikeContainerData magikeContainerData = new()
            {
                Magike = reset.Magike,
                MagikeMax = reset.MagikeMax,
                MagikeMaxBase = reset.MagikeMaxBase,
                MagikeMaxBonus = reset.MagikeMaxBonus
            };
            return magikeContainerData;
        }

        public override void Update()
        {
            //机器逻辑服务端权威化：默认情况下仅服务端/单人运行组件更新（产出/消耗/合成/发送魔能等状态变更）。
            //纯客户端只运行显式声明 UpdateOnClient 的组件（视觉/插值类，如鸟控制器、脉冲发送器的特效）。
            bool isClient = VaultUtils.isClient;
            for (int i = 0; i < ComponentsCache.Count; i++)
            {
                MagikeComponent component = ComponentsCache[i];
                if (isClient && !component.UpdateOnClient)
                    continue;

                component.Update();
            }
        }

        public override void SetProperty()
        {
            InitializeComponentCache();
            ApparatusInformation c = AddInformation();
            if (c != null)
                AddComponent(c);

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
        /// 加入显示信息的组件
        /// </summary>
        /// <returns></returns>
        public virtual ApparatusInformation AddInformation()
        {
            return new ApparatusInformation();
        }

        /// <summary>
        /// 初始化起始时的组件
        /// </summary>
        public abstract void InitializeBeginningComponent();

        public override void OnKill()
        {
            RemoveAllComponent();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var component in ComponentsCache)
                component.Draw(spriteBatch);
        }

        #region 网络同步与数据存储

        /// <summary>
        /// 统一的网络写入格式（进图全量同步与运行时全量回包共用）：<br></br>
        /// <c>[组件数量][每个组件: GUID, 类型全名, 负载长度(int), 负载字节]</c><br></br>
        /// 写入组件数量与类型让接收端不再依赖"两端组件缓存数量手工一致"——
        /// 这是运行时增删组件（如移除滤镜触发全量 <see cref="SendData()"/>）后整座机器数据错位的根因。<br></br>
        /// 每个组件再加长度前缀，使类型缺失或单个组件反序列化出错时可安全跳过，不会污染同一 TP 内的其它组件。
        /// </summary>
        public override void SendData(ModPacket data)
        {
            data.Write(ComponentsCache.Count);
            for (int i = 0; i < ComponentsCache.Count; i++)
            {
                MagikeComponent component = ComponentsCache[i];
                data.Write(GUID);
                data.Write(component.GetType().FullName);
                WriteComponentPayload(data, component);
            }
        }

        /// <summary>
        /// 以长度前缀封装单个组件的负载，便于接收端隔离与安全跳过
        /// </summary>
        private static void WriteComponentPayload(ModPacket data, MagikeComponent component)
        {
            if (data.BaseStream is MemoryStream ms)
            {
                long lenPos = ms.Position;
                data.Write(0);                 //长度占位
                long start = ms.Position;
                component.SendData(data);
                long end = ms.Position;
                int len = (int)(end - start);
                ms.Position = lenPos;
                data.Write(len);               //回填真实长度
                ms.Position = end;
            }
            else
            {
                //极端兜底：无法定位流位置时不写长度框（-1 表示直接在原读取器上反序列化）
                data.Write(-1);
                component.SendData(data);
            }
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            int count = reader.ReadInt32();
            if (count < 0)//防御损坏数据
                return;

            if (TileProcessorNetWork.InitializeWorld)
            {
                //进图全量同步：彻底重建缓存（沿用经过验证的行为，并带长度框安全跳过未知类型）
                InitializeComponentCache();
                for (int i = 0; i < count; i++)
                {
                    MagikeComponent component = ReadOneComponent(reader, whoAmI, null);
                    if (component != null)
                        AddComponentWithoutOnAdd(component);
                }
            }
            else
            {
                //运行时全量回包：以"接收顺序 = 服务端权威顺序"重建缓存。
                //复用同类型既有实例以保留对象引用（UI 仍持有 ItemContainer/MagikeContainer 等引用）与各端视觉状态；
                //被服务端移除的组件留在 pool 中、最终被丢弃（不触发 OnRemove 副作用，因为服务端已是权威）。
                List<MagikeComponent> pool = new(ComponentsCache);
                List<MagikeComponent> rebuilt = new(count);
                for (int i = 0; i < count; i++)
                {
                    MagikeComponent component = ReadOneComponent(reader, whoAmI, pool, i);
                    if (component != null)
                        rebuilt.Add(component);
                }

                InitializeComponentCache();
                foreach (var component in rebuilt)
                    AddComponentWithoutOnAdd(component);
            }
        }

        /// <summary>
        /// 读取单个组件条目 <c>[GUID, 类型全名, 长度, 负载]</c>。<br></br>
        /// 当 <paramref name="reusePool"/> 非空时，按 <paramref name="reuseIndex"/> 与 ComponentsCache 序号对齐复用实例
        /// （同位置同类型才复用，避免同类型多实例如多个 BasicFilter 交叉套错负载）；类型未知或 GUID 不匹配时按长度安全跳过。
        /// </summary>
        private MagikeComponent ReadOneComponent(BinaryReader reader, int whoAmI, List<MagikeComponent> reusePool, int reuseIndex = -1)
        {
            string guiD = reader.ReadString();
            string fullName = reader.ReadString();
            int len = reader.ReadInt32();

            Type t = null;
            if (guiD != GUID || !GetMagikeComponentType(fullName, out t))
            {
                SkipPayload(reader, len);
                return null;
            }

            MagikeComponent component = null;
            if (reusePool != null && reuseIndex >= 0 && reuseIndex < reusePool.Count)
            {
                MagikeComponent candidate = reusePool[reuseIndex];
                if (candidate.GetType() == t)
                    component = candidate;
            }

            component ??= (MagikeComponent)Activator.CreateInstance(t);
            component.Entity = this;

            ReadPayload(reader, len, whoAmI, component);
            return component;
        }

        private static void ReadPayload(BinaryReader reader, int len, int whoAmI, MagikeComponent component)
        {
            if (len < 0)//兜底：无长度框，直接读
            {
                component.ReceiveData(reader, whoAmI);
                return;
            }

            //在独立子读取器上反序列化，彻底隔离单个组件可能的读取错位。
            //主读取器已按长度框越过本组件负载，因此即使子流解析失败也不影响同一 TP 内的其它组件。
            byte[] bytes = reader.ReadBytes(len);
            try
            {
                using MemoryStream ms = new(bytes);
                using BinaryReader sub = new(ms);
                component.ReceiveData(sub, whoAmI);
            }
            catch (Exception e)
            {
                $"魔能组件 {component.GetType().Name} 反序列化失败，已跳过：{e.Message}".DumpInConsole();
            }
        }

        private static void SkipPayload(BinaryReader reader, int len)
        {
            if (len > 0)
                reader.ReadBytes(len);
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
                if (!GetMagikeComponentType(fullName, out Type t))
                {
                    i++;
                    continue;
                }

                var component = (MagikeComponent)Activator.CreateInstance(t);
                component.Entity = this;
                component.LoadData(SaveName + i.ToString(), tag);
                i++;

                AddComponentWithoutOnAdd(component);
            }
        }

        public static bool GetMagikeComponentType(string fullName, out Type t)
        {
            string modName = fullName.Split('.')[0];

            if (modName == nameof(Coralite))  //本模组直接就获取类型
                t = Type.GetType(fullName);
            else                              //其他模组从别的模组中获取
            {
                ModLoader.TryGetMod(modName, out Mod mod);
                t = mod.Code.GetType(fullName);
            }

            if (t is null)
            {
                $"未找到指定类型{fullName}！".Dump();
                t = null;
                return false;
            }

            return true;
        }

        #endregion

        #region 来自基类

        public bool HasComponent(int componentId)
            => Components.Contains(componentId);

        public bool HasComponent<T>() where T : MagikeComponent
            => ComponentsCache.FirstOrDefault(c => c is T, null) != null;

        /// <summary>
        /// 向实体内加入组件
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public void AddComponent(MagikeComponent component)
        {
            AddComponentWithoutOnAdd(component);
            component.OnAdd(this);

            if (!VaultUtils.isClient)
                SendData();
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

            //服务端权威：只有服务端/单人才广播全量数据。客户端的本地乐观移除不应把状态推回服务端，
            //而是等待服务端处理移除请求后下发的权威全量同步来对账。
            if (!VaultUtils.isClient)
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

        public bool TryGetFilters(out List<MagikeComponent> result)
        {
            result = null;
            if (!HasComponent(MagikeComponentID.MagikeFilter))
                return false;

            result = (List<MagikeComponent>)Components[MagikeComponentID.MagikeFilter];

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

        /// <summary>
        /// 查找组件的索引
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public int IndexOf(MagikeComponent component)
        {
            return ComponentsCache.IndexOf(component);
        }

        #endregion
    }

    public class MagikeGloblaTP : GlobalTileProcessor
    {
        public override Point16? GetTopLeftOrNull(Tile tile, int i, int j)
        {
            if (TileLoader.GetTile(tile.TileType) is BaseMagikeTile magikeTile)
            {
                return magikeTile.ToTopLeft(i, j) ?? MagikeHelper.ToTopLeft(i, j);
            }
            return base.GetTopLeftOrNull(tile, i, j);
        }
    }
}
