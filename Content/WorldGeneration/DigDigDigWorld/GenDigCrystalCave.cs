using Coralite.Content.Items.Magike.Factorys;
using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Content.WorldGeneration.ShadowCastleRooms;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheCrystalCave { get; set; }

        public void GenDigCrystalCave(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheCrystalCave.Value;

            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();

            int wallWidth = 4;

            ShapeData innerData = new();
            ShapeData outerData = new();

            Point origin = new Point(Main.maxTilesX / 2, Main.maxTilesY / 2);

            #region 主体圆环
            //挖出一个球，内部全空
            WorldUtils.Gen(
                origin,  //中心点
                new Shapes.Circle(CrystalCaveRadius, CrystalCaveRadius),   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    new Modifiers.Blotches(4, 0.4),     //添加边缘的抖动，让边缘处不那么平滑
                    new Actions.Clear(),    //清除形状内所有物块
                    new Actions.SetFrames(true).Output(innerData)));   //通过output记录当前的形状

            //生成外围圈
            WorldUtils.Gen(
                origin,
                new ModShapes.OuterOutline(innerData),  //使用刚刚生成出来的形状，该形状的取外边缘
                Actions.Chain(
                    new Modifiers.Expand(wallWidth, wallWidth),  //扩展这个边缘线，得到一个圆环
                    new Actions.SetTile(basalt),    //放置物块
                    new Actions.SetFrames(true).Output(outerData)));    //存储圆环形状
            #endregion

            progress.Value = 0.2f;

            BasaltFloatIsland(origin, CrystalCaveRadius, CrystalCaveRadius);
            int howMany = WorldGen.genRand.Next(12, 24);

            progress.Value = 0.4f;

            //生成小起伏
            BasaltBall(origin, CrystalCaveRadius, CrystalCaveRadius, howMany);
            GenCrystalSpike(origin, CrystalCaveRadius, CrystalCaveRadius);

            progress.Value = 0.6f;

            Point topLeft = origin - new Point(CrystalCaveRadius, CrystalCaveRadius);

            ShapeData bornData = new();

            //出生点小石球
            Point origin2 = origin + new Point(0, -2);

            WorldUtils.Gen(
                origin2,  //中心点
                new Shapes.Circle(6, 6),   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    new Actions.ClearTile(),
                    new Actions.PlaceTile(TileID.Stone),    //清除形状内所有物块
                    new Actions.SetFrames(true).Output(bornData)));   //通过output记录当前的形状

            WorldUtils.Gen(
                origin2,  //中心点
                new ModShapes.OuterOutline(bornData),  //使用刚刚生成出来的形状，该形状的取外边缘
                Actions.Chain(
                    new Modifiers.Expand(1),  //扩展这个边缘线，得到一个圆环
                    new Actions.ClearTile(),
                    new Actions.SetTile((ushort)ModContent.TileType<HardBasaltTile>()),    //放置物块
                    new Actions.SetFrames(true)));    //存储圆环形状

            WorldUtils.Gen(
                origin2,  //中心点
                new ModShapes.OuterOutline(bornData),  //使用刚刚生成出来的形状，该形状的取外边缘
                Actions.Chain(
                    new Actions.ClearTile(),
                    new Actions.SetTile((ushort)ModContent.TileType<MagicCrystalBrickTile>()),    //放置物块
                    new Actions.SetFrames(true)));    //存储圆环形状

            for (int i = -1; i < 2; i++)
                for (int j = -3; j < 0; j++)
                    Main.tile[origin.X + i, origin.Y + j].Clear(TileDataType.Tile);

            GenCrystalClusters(origin, CrystalCaveRadius, CrystalCaveRadius);
            Point chest = MagicCrystalCaveChest(origin + new Point(0, 25));

            int index = Chest.FindChest(chest.X - 1, chest.Y);//往箱子里多塞一点
            if (index != -1)
            {
                ChestRoom.RandChestItem(Main.chest[index], ItemID.AncientChisel);
                ChestRoom.RandChestItem(Main.chest[index], ModContent.ItemType<StoneMaker>(), 2);
                ChestRoom.RandChestItem(Main.chest[index], ModContent.ItemType<MagicCrystalPolarizedFilter>(), 1);
                ChestRoom.RandChestItem(Main.chest[index], ModContent.ItemType<MagicCrystalPolarizedFilter>(), 1);
                ChestRoom.RandChestItem(Main.chest[index], ModContent.ItemType<MagicCrystalPolarizedFilter>(), 2);
                ChestRoom.RandChestItem(Main.chest[index], ModContent.ItemType<MagicCrystalPolarizedFilter>(), 4);
                ChestRoom.RandChestItem(Main.chest[index], ModContent.ItemType<MagicCrystalPolarizedFilter>(), 4);
            }

            progress.Value = 0.8f;

            AddCrystalCaveDecoration(CrystalCaveRadius, CrystalCaveRadius, topLeft);

            MagicCrystalCaveCenters.Add(new Point16(origin.X, origin.Y));
        }
    }
}