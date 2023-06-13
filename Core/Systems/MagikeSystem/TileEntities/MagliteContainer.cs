using Terraria;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// ħ���������̳��������ʵ�����������Ϊ�����Ĺ���
    /// </summary>
    public abstract class MagikeContainer : ModTileEntity
    {
        /// <summary> ��ǰħ���� </summary>
        public int magike;
        /// <summary> ħ�����ֵ </summary>
        public readonly int magikeMax;

        /// <summary> ������� </summary>
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