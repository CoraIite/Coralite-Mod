using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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

        /// <summary> ��ǰ��װ���Ƿ���ʹ��״̬ </summary>
        public bool active;

        /// <summary> ������� </summary>
        public abstract ushort TileType { get; }

        public MagikeContainer(int magikeMax)
        {
            this.magikeMax = magikeMax;
        }

        /// <summary> ����ħ�������������������� </summary>
        public void Limit()
        {
            magike = Math.Clamp(magike, 0, magikeMax);
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return Framing.GetTileSafely(x, y).TileType == TileType;
        }

        /// <summary>
        /// ����ħ���������ܵķ�������Ҫ�Ȼ�ȡ��ʵ�����У�
        /// </summary>
        /// <param name="howManyMagite">�����</param>
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