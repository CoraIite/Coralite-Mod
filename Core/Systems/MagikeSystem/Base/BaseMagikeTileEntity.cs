using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Base
{
    public abstract class BaseMagikeTileEntity : ModTileEntity, IEntity
    {
        public const string SaveName = "Component";

        /// <summary> 物块类型 </summary>
        public abstract ushort TileType { get; }

        public Dictionary<int, List<Component>> Components { get; private set; }
        public List<Component> ComponentsCache { get; private set; }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == TileType;
        }

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

        /// <summary>
        /// 初始化用于存储组件的字典和数组
        /// </summary>
        public void InitializeComponentCache()
        {
            Components = new Dictionary<int, List<Component>>();
            ComponentsCache = new List<Component>();
        }

        #region 数据存储

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < ComponentsCache.Count; i++)
            {
                Component component = ComponentsCache[i];

                string fullName = component.GetType().FullName;
                string preName = SaveName+ i.ToString();

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
                i++;
                var t = System.Type.GetType(fullName);
                if (t is null)
                    continue;

                var component = (Component)Activator.CreateInstance(t);
                component.LoadData(SaveName + i.ToString(),tag);

                (this as IEntity).AddComponent(component);
            }
        }

        #endregion
    }
}
