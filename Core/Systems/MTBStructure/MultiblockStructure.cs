using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.Particles;
using InnoVault.PRT;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.MTBStructure
{
    public abstract class MultiblockStructure : ModType
    {
        public int Type { get; internal set; }

        protected sealed override void Register()
        {
            ModTypeLookup<MultiblockStructure>.Register(this);

            MTBStructureLoader.structures ??= new List<MultiblockStructure>();
            MTBStructureLoader.structures.Add(this);

            Type = MTBStructureLoader.ReserveMTBStructureID();
        }

        /// <summary>
        /// 结构的物块
        /// </summary>
        public abstract int[,] StructureTile { get; }
        /// <summary>
        /// 结构的墙壁
        /// </summary>
        public virtual int[,] StructureWall { get; } = null;
        /// <summary>
        /// 结构的液体
        /// </summary>
        public virtual int[,] StructureLiquid { get; } = null;

        /// <summary>
        /// 检测建筑，成功就触发<see cref="OnSuccess"/>
        /// </summary>
        /// <param name="origin"></param>
        public void CheckStructure(Point origin)
        {
            if (!CheckStructureInner(origin, out Point failPoint))
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Color.Red,
                    Text = MTBStructureSystem.FailText.Value,
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
        private bool CheckStructureInner(Point origin, out Point failPoint)
        {
            int[,] structureTile = StructureTile;
            int width = structureTile.GetLength(1);
            int height = structureTile.GetLength(0);

            Point topleft = origin - new Point(width / 2, height / 2);

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Point p = new Point(topleft.X + j, topleft.Y + i);
                    Tile tile = Framing.GetTileSafely(p);
                    int type = structureTile[i, j];

                    if (type >= 0)
                        if (tile.TileType != type)//检测物块类型
                        {
                            failPoint = p;
                            return false;
                        }

                    if (StructureWall != null)
                    {
                        int wallType = StructureWall[i, j];

                        if (wallType >= 0)
                            if (tile.WallType != wallType)//检测墙壁类型
                            {
                                failPoint = p;
                                return false;
                            }
                    }

                    if (StructureLiquid != null)
                    {
                        int liquidType = StructureLiquid[i, j];

                        if (liquidType >= 0)
                            if (tile.WallType != liquidType)//检测墙壁类型
                            {
                                failPoint = p;
                                return false;
                            }
                    }
                }

            failPoint = Point.Zero;
            return true;
        }

        public virtual void Fail(Point failPoint)
        {
            PRTLoader.NewParticle<TileHightlight>(failPoint.ToWorldCoordinates(), Vector2.Zero, Color.Red);
        }

        /// <summary>
        /// 成功检测多方块结构后调用
        /// </summary>
        /// <param name="origin"></param>
        public virtual void OnSuccess(Point origin)
        {

        }

        /// <summary>
        /// 将多方块结构中的所有物块清除掉
        /// </summary>
        /// <param name="origin"></param>
        protected void KillAll(Point origin)
        {
            int[,] structureTile = StructureTile;
            int width = structureTile.GetLength(1);
            int height = structureTile.GetLength(0);

            Point topleft = origin - new Point(width / 2, height / 2);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Point p = new Point(topleft.X + j, topleft.Y + i);
                    int type = structureTile[i, j];

                    if (type >= 0)
                    {
                        WorldGen.KillTile(p.X, p.Y, noItem: true);
                    }
                }
        }
    }
}
