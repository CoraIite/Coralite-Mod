using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能容器，继承这个类以实现最基础的作为容器的功能
    /// </summary>
    public abstract class OldMagikeContainer : ModTileEntity, IMagikeContainer
    {
        /// <summary> 当前魔能量 </summary>
        public int magike;
        /// <summary> 魔能最大值 </summary>
        public readonly int magikeMax;

        /// <summary> 当前的装置是否在使用状态 </summary>
        public bool active;

        /// <summary> 物块类型 </summary>
        public abstract ushort TileType { get; }

        public int Magike => magike;
        public int MagikeMax => magikeMax;
        public bool Active => active;
        public Point16 GetPosition => Position;

        public event Action OnCharged;
        public event Action OnDisCharged;

        public OldMagikeContainer(int magikeMax)
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

        //===================以下是帮助方法========================

        /// <summary> 限制魔能量，让它不超过上限 </summary>
        public virtual void Limit()
        {
            magike = Math.Clamp(magike, 0, magikeMax);
        }

        /// <summary>
        /// 给改魔能容器充能的方法，需要先获取到实例才行
        /// </summary>
        /// <param name="howManyMagike">充多少</param>
        public virtual bool Charge(int howManyMagike)
        {
            bool ChargeOrDischarge = howManyMagike >= 0;
            if (magike >= magikeMax && ChargeOrDischarge)
                return false;

            if (ChargeOrDischarge)
            {
                OnCharged?.Invoke();
                OnReceiveVisualEffect();
            }
            else
                OnDisCharged?.Invoke();

            magike += howManyMagike;
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