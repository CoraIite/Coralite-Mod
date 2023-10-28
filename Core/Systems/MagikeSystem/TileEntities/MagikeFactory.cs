using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能工厂类，可以实现想要的功能
    /// </summary>
    public abstract class MagikeFactory : ModTileEntity, IMagikeFactory
    {
        /// <summary> 当前魔能量 </summary>
        public int magike;
        /// <summary> 魔能最大值 </summary>
        public readonly int magikeMax;
        /// <summary> 当前的装置是否在使用状态 </summary>
        public bool active;

        public int workTimer = -1;
        /// <summary> 最大工作时间，一般不需要手动修改它 </summary>
        public int workTimeMax;

        public abstract ushort TileType { get; }

        public int Magike => magike;
        public int MagikeMax => magikeMax;
        public bool Active => active;
        public Point16 GetPosition => Position;

        public event Action OnWorkFinshed;
        public event Action OnCharged;
        public event Action OnDisCharged;

        public MagikeFactory(int magikeMax, int workTimeMax)
        {
            this.magikeMax = magikeMax;
            this.workTimeMax = workTimeMax;
        }

        public override void Update()
        {
            if (CanWork())
                Work();
        }

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

        /// <summary> 限制魔能量，让它不超过上限 </summary>
        public virtual void Limit()
        {
            magike = Math.Clamp(magike, 0, magikeMax);
        }

        /// <summary>
        /// 给改魔能容器充能的方法，需要先获取到实例才行
        /// </summary>
        /// <param name="howManyMagite">充多少</param>
        public virtual bool Charge(int howManyMagite)
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

        public virtual void OnReceiveVisualEffect() { }

        public virtual void CheckActive()
        {
            active = magike > 0;
        }

        public virtual Vector2 GetWorldPosition()
        {
            return Position.ToWorldCoordinates(16);
        }


        /// <summary>
        /// 帮助方法，意为开始工作
        /// </summary>
        public virtual bool StartWork()
        {
            if (workTimer == -1)
            {
                workTimer = 0;
                return true;
            }

            return false;
        }

        public virtual bool CanWork()
        {
            return workTimer >= 0;
        }

        public virtual void Work()
        {
            if (workTimer >= workTimeMax)
            {
                workTimer = -1;
                OnWorkFinshed?.Invoke();
                WorkFinish();
                return;
            }
            else
                DuringWork();

            workTimer++;
        }

        public virtual void DuringWork() { }

        /// <summary>
        /// 工作完成，在此执行对应的物品消耗，魔能消耗等工作
        /// </summary>
        public virtual void WorkFinish() { }
    }
}
