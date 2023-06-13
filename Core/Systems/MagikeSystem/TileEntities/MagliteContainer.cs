using Terraria;
using Terraria.ModLoader;

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

        /// <summary> 物块类型 </summary>
        public abstract ushort TileType { get; }

        public MagikeContainer(int magikeMax)
        {
            this.magikeMax = magikeMax;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == TileType;
        }
    }
}