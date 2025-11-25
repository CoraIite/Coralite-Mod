using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Helpers;
using InnoVault.PRT;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MTBStructure
{
    public abstract class Multiblock : ModType
    {
        public int Type { get; internal set; }

        protected sealed override void Register()
        {
            ModTypeLookup<Multiblock>.Register(this);

            MultiblockLoader.structures ??= [];
            MultiblockLoader.structures.Add(this);

            Type = MultiblockLoader.ReserveMTBStructureID();
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        /// <summary>
        /// 多方块结构的信息
        /// </summary>
        public List<MultiBlockInfo> Infos;

        /// <summary>
        /// 多方块信息
        /// </summary>
        public class MultiBlockInfo
        {
            public Point16 point;

            public bool checkTile;
            public short tileType;
            public short wallType;
            public short liquidType;
            public byte liquidAmount;

            public MultiBlockInfo(Point16 point,bool checkTile,short tileType, short wallType,short liquidType, byte liquidAmount)
            {
                this.point = point;
                this.checkTile = checkTile;
                this.tileType = tileType;
                this.wallType = wallType;
                this.liquidType = liquidType;
                this.liquidAmount = liquidAmount;
            }

            public MultiBlockInfo(Point16 point, short wallType, bool noTileCheck)
            {
                this.point = point;
                checkTile = noTileCheck;
                this.wallType = wallType;
            }

            public MultiBlockInfo(Point16 point, short tileType)
            {
                this.point = point;
                checkTile = true;
                this.tileType = tileType;
                wallType = 0;
                liquidType = 0;
                liquidAmount = 0;
            }
        }

        /// <summary>
        /// 添加多个物块
        /// </summary>
        /// <param name="posTileTypes"></param>
        public void AddTiles(params (int, int, int)[] posTileTypes)
        {
            foreach (var data in posTileTypes)
                AddTile(new Point16(data.Item1, data.Item2), data.Item3);
        }

        /// <summary>
        /// 添加物块
        /// </summary>
        /// <param name="tileType"></param>
        public void AddTile(Point16 pos,int tileType)
        {
            Infos ??= [];

            MultiBlockInfo oldInfo = Infos.Find(info => info.point == pos);
            if (oldInfo != null)
            {
                if (oldInfo.checkTile && oldInfo.tileType != tileType)
                    "你在同一位置重复向多方块结构添加物块，请确认你的代码是否正确！".Dump();

                oldInfo.tileType = (short)tileType;
                oldInfo.checkTile = true;
                return;
            }

            Infos.Add(new MultiBlockInfo(pos, (short)tileType));
        }

        /// <summary>
        /// 添加墙壁
        /// </summary>
        /// <param name="tileType"></param>
        public void AddWallCheck(Point16 pos,int wallType)
        {
            Infos ??= [];

            MultiBlockInfo oldInfo = Infos.Find(info => info.point == pos);
            if (oldInfo != null)
            {
                if (oldInfo.wallType != wallType)
                    "你在同一位置重复向多方块结构添加墙壁，请确认你的代码是否正确！".Dump();

                oldInfo.wallType = (short)wallType;
                return;
            }

            Infos.Add(new MultiBlockInfo(pos, (short)wallType,true));
        }

        /// <summary>
        /// 检测建筑，成功就触发<see cref="OnSuccess"/>
        /// </summary>
        /// <param name="origin"></param>
        public virtual void CheckStructure(Point16 origin)
        {
            if (!CheckStructureInner(origin, out Point16 failPoint))
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Color.Red,
                    Text = MultiblockSystem.FailText.Value,
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, origin.ToWorldCoordinates() - (Vector2.UnitY * 32));

                Fail(failPoint);
                return;
            }

            OnSuccess(origin);
        }

        /// <summary>
        /// 检测多方块结构
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        protected bool CheckStructureInner(Point16 origin, out Point16 failPoint)
        {
            failPoint = origin;
            foreach (var info in Infos)
            {
                Point16 p = origin + info.point;

                Tile t = Framing.GetTileSafely(p);

                //检测物块
                if (info.checkTile)
                    if (t.TileType != info.tileType)//检测物块类型
                    {
                        failPoint = p;
                        return false;
                    }

                //检测墙壁
                if (info.wallType>0)
                    if (t.WallType != info.wallType)//检测墙壁类型
                    {
                        failPoint = p;
                        return false;
                    }
                //检测液体
                if (info.liquidAmount > 0)
                    if (t.LiquidType != info.liquidType || t.LiquidAmount < info.liquidAmount)//检测墙壁类型
                    {
                        failPoint = p;
                        return false;
                    }
            }

            return true;
        }

        public virtual void Fail(Point16 failPoint)
        {
            PRTLoader.NewParticle<TileHightlight>(failPoint.ToWorldCoordinates(), Vector2.Zero, Color.Red);
        }

        /// <summary>
        /// 成功检测多方块结构后调用
        /// </summary>
        /// <param name="origin"></param>
        public virtual void OnSuccess(Point16 origin)
        {

        }

        /// <summary>
        /// 将多方块结构中的所有物块清除掉
        /// </summary>
        /// <param name="origin"></param>
        protected void KillAll(Point16 origin)
        {
            foreach (var info in Infos)
            {
                Point16 p = origin + info.point;

                Tile t = Framing.GetTileSafely(p);
                if (info.checkTile)
                {
                    var data = TileObjectData.GetTileData(t);
                    if (data == null)
                        WorldGen.KillTile(p.X, p.Y, false, false, true);
                    else
                        t.ClearTile();
                }

                //检测墙壁
                if (info.wallType > 0)
                    t.Clear(TileDataType.Wall);
                //检测液体
                if (info.liquidAmount > 0)
                    t.LiquidAmount-= info.liquidAmount;
            }
        }
    }
}
