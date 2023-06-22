using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
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

        public event Action OnCharged;
        public event Action OnDisCharged;

        public MagikeContainer(int magikeMax)
        {
            this.magikeMax = magikeMax;
        }

        public virtual void OnReceiveVisualEffect() { }

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


        public override void SaveData(TagCompound tag)
        {
            tag.Add("Magike", magike);
        }

        public override void LoadData(TagCompound tag)
        {
            magike = tag.GetInt("Magike");
        }

        //===================�����ǰ�������========================

        /// <summary> ����ħ�������������������� </summary>
        public void Limit()
        {
            magike = Math.Clamp(magike, 0, magikeMax);
        }

        /// <summary>
        /// ����ħ���������ܵķ�������Ҫ�Ȼ�ȡ��ʵ������
        /// </summary>
        /// <param name="howManyMagite">�����</param>
        public bool Charge(int howManyMagite)
        {
            bool ChargeOrDischarge = howManyMagite >= 0;
            if (magike >= magikeMax && ChargeOrDischarge)
                return false;

            if (ChargeOrDischarge)
            {
                OnCharged?.Invoke();
                OnReceiveVisualEffect();
            }
            else
                OnDisCharged?.Invoke();

            magike += howManyMagite;
            Limit();
            CheckActive();

            return true;
        }

        public virtual void CheckActive()
        {
            active = magike > 0;
        }

        public virtual Vector2 GetWorldPosition()
        {
            return Position.ToWorldCoordinates(16);
        }

    }
}