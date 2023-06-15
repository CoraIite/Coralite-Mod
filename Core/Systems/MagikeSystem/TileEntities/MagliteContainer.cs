using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能容器，继承这个类以实现最基础的作为容器的功能
    /// </summary>
    public abstract class MagikeContainer : ModTileEntity
    {
        /// <summary> 当前魔能量 </summary>
        public int magike;
        /// <summary> 魔能最大值 </summary>
        public readonly int magikeMax;

        /// <summary> 当前的装置是否在使用状态 </summary>
        public bool active;

        /// <summary> 物块类型 </summary>
        public abstract ushort TileType { get; }

        public MagikeContainer(int magikeMax)
        {
            this.magikeMax = magikeMax;
        }

        /// <summary> 限制魔能量，让它不超过上限 </summary>
        public void Limit()
        {
            magike = Math.Clamp(magike, 0, magikeMax);
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == TileType;
        }

        /// <summary>
        /// 给改魔能容器充能的方法，需要先获取到实例才行（
        /// </summary>
        /// <param name="howManyMagite">充多少</param>
        public virtual bool Charge(int howManyMagite)
        {
            if (magike >= magikeMax)
                return false;

            magike += howManyMagite;
            Limit();
            if (magike > 0)
                active = true;
            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("Magike", magike);
        }

        public override void LoadData(TagCompound tag)
        {
            magike = tag.GetInt("Magike");
        }
    }
}