using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using InnoVault.Storages;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.Storages
{
    /// <summary>
    /// 魔能物品容器的存储提供者实现
    /// 将 <see cref="ItemContainer"/> 组件适配为 <see cref="IStorageProvider"/> 接口
    /// </summary>
    public class MagikeStorageProvider : IStorageProvider
    {
        private readonly MagikeTP _entity;
        private readonly ItemContainer _container;

        public MagikeStorageProvider(MagikeTP entity, ItemContainer container)
        {
            _entity = entity;
            _container = container;
        }

        /// <summary>
        /// 存储提供者的唯一标识符
        /// </summary>
        public string Identifier => "Coralite.MagikeStorage";

        /// <summary>
        /// 存储对象在世界中的位置(物块坐标)
        /// </summary>
        public Point16 Position => _entity.Position;

        /// <summary>
        /// 存储对象在世界中的中心位置(像素坐标)
        /// </summary>
        public Vector2 WorldCenter => Helper.GetMagikeTileCenter(_entity.Position);

        /// <summary>
        /// 存储对象的碰撞区域(像素坐标)
        /// </summary>
        public Rectangle HitBox
        {
            get
            {
                Point16 pos = _entity.Position;
                MagikeHelper.GetMagikeAlternateData(pos.X, pos.Y, out TileObjectData data, out _);

                int width = data?.Width ?? 1;
                int height = data?.Height ?? 1;

                return new Rectangle(pos.X * 16, pos.Y * 16, width * 16, height * 16);
            }
        }

        /// <summary>
        /// 检查存储对象是否仍然有效
        /// </summary>
        public bool IsValid => _entity != null && _container != null && _entity.Active;

        /// <summary>
        /// 检查存储是否还有剩余空间
        /// </summary>
        public bool HasSpace
        {
            get
            {
                if (!IsValid)
                    return false;

                //检查是否有空槽位
                foreach (var item in _container.Items)
                {
                    if (item == null || item.IsAir || item.stack < item.maxStack)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 检查是否可以添加指定物品到存储中
        /// </summary>
        public bool CanAcceptItem(Item item)
        {
            if (!IsValid || item == null || item.IsAir)
                return false;

            //对于只读容器，不允许存入
            if (_container is GetOnlyItemContainer)
                return false;

            return _container.CanAddItem(item.type, item.stack);
        }

        /// <summary>
        /// 将物品存入存储
        /// </summary>
        public bool DepositItem(Item item)
        {
            if (!CanAcceptItem(item))
                return false;

            _container.AddItem(item);
            return item.IsAir; //如果物品被完全存入则返回true
        }

        /// <summary>
        /// 从存储中取出指定类型的物品
        /// </summary>
        public Item WithdrawItem(int itemType, int count)
        {
            if (!IsValid || count <= 0)
                return new Item();

            return _container.GetItem(count, itemType) ?? new Item();
        }

        /// <summary>
        /// 获取存储中所有物品的枚举
        /// </summary>
        public IEnumerable<Item> GetStoredItems()
        {
            if (!IsValid)
                yield break;

            foreach (var item in _container.Items)
            {
                if (item != null && !item.IsAir)
                    yield return item;
            }
        }

        /// <summary>
        /// 获取存储中指定类型物品的总数量
        /// </summary>
        public long GetItemCount(int itemType)
        {
            if (!IsValid)
                return 0;

            long count = 0;
            foreach (var item in _container.Items)
            {
                if (item != null && !item.IsAir && item.type == itemType)
                    count += item.stack;
            }

            return count;
        }

        /// <summary>
        /// 执行存入动画效果
        /// </summary>
        public void PlayDepositAnimation()
        {
            if (!IsValid)
                return;

            //生成魔能特效
            MagikeHelper.SpawnLozengeParticle_WithTopLeft(_entity.Position);
        }
    }

    /// <summary>
    /// 只读魔能物品容器的存储提供者实现
    /// 用于 <see cref="GetOnlyItemContainer"/>，只允许取出物品
    /// </summary>
    public class MagikeGetOnlyStorageProvider : MagikeStorageProvider
    {
        public MagikeGetOnlyStorageProvider(MagikeTP entity, GetOnlyItemContainer container)
            : base(entity, container)
        {
        }

        /// <summary>
        /// 只读容器不接受物品存入
        /// </summary>
        public new bool CanAcceptItem(Item item) => false;

        /// <summary>
        /// 只读容器不允许存入
        /// </summary>
        public new bool DepositItem(Item item) => false;
    }
}
