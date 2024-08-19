using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeTileEntity : ModTileEntity, IEntity
    {
        public const string SaveName = "Component";

        /// <summary> 物块类型 </summary>
        public abstract ushort TileType { get; }

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

        public override bool IsTileValidForEntity(int x, int y) => Framing.GetTileSafely(x, y).TileType == TileType;

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, TileChangeType.HoneyLava);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }

            return Place(i, j);
        }

        public override void Update()
        {
            for (int i = 0; i < ComponentsCache.Count; i++)
                ComponentsCache[i].Update(this);
        }

        public MagikeTileEntity()
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

        public void AddComponentDirectly(Component component)
            => (this as IEntity).AddComponent(component);

        /// <summary>
        /// 初始化起始时的组件
        /// </summary>
        public abstract void InitializeBeginningComponent();

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
    }
}
