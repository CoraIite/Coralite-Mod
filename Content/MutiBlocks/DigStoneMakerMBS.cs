using Coralite.Content.Items.Magike.Factorys;
using Coralite.Content.Tiles.DigDigDig;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core.Systems.MTBStructure;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.MutiBlocks
{
    public class DigStoneMakerMBS : Multiblock
    {
        /// <summary>
        /// 魔力水晶砖
        /// </summary>
        private int cb => ModContent.TileType<MagicCrystalBrickTile>();
        /// <summary>
        /// 硬化玄武岩
        /// </summary>
        private int h => ModContent.TileType<HardBasaltTile>();
        /// <summary>
        /// 玄武岩
        /// </summary>
        private int b => ModContent.TileType<BasaltTile>();
        /// <summary>
        /// 核心方块
        /// </summary>
        private int core => ModContent.TileType<StoneMakerCoreTile>();

        public override int[,] StructureTile =>
            new int[3, 3]
            {
                {cb,-1,cb },
                {b,core,b },
                {h, h, h }
            };

        public override void OnSuccess(Point origin)
        {
            KillAll(origin);

            Item.NewItem(new EntitySource_TileBreak(origin.X, origin.Y), origin.ToWorldCoordinates(), ModContent.ItemType<StoneMaker>());
        }
    }
}
