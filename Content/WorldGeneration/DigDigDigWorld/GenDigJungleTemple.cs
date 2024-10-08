using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheJungleTemple { get; set; }

        public static void GenDigJungleTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheJungleTemple.Value;

            Point origin = new Point(GenVars.jungleOriginX, Main.maxTilesY / 2);

            //生成一个尖角朝上的等腰三角形，和一个矩形
            var triangle = new Shapes.Tail(50, new ReLogic.Utilities.Vector2D(0, -30));
            var rectangleCut = new Shapes.Rectangle(new Rectangle(-24, -220, 45, 200));

            var rectangle = new Shapes.Rectangle(new Rectangle(-25, -5, 50, 10));
            var rectangle2 = new Shapes.Rectangle(new Rectangle(-25, -1, 50, 3));

            ShapeData triangleData = new ShapeData();
            ShapeData rectangleCutData = new ShapeData();

            ShapeData rectangleData = new ShapeData();

            //生成形状
            WorldUtils.Gen(origin, triangle, new Actions.Blank().Output(triangleData));
            WorldUtils.Gen(origin, rectangleCut, new Actions.Blank().Output(rectangleCutData));

            ShapeData trapezium = new ShapeData();
            ShapeData trapezium2 = new ShapeData();

            Actions.SetTile setTile = new Actions.SetTile(TileID.LihzahrdBrick);

            //生成顶部等腰梯形
            WorldUtils.Gen(
            origin + new Point(0, -4),
            triangle,//三角形
                Actions.Chain(
                    new Modifiers.NotInShape(rectangleCutData),//用矩形裁剪一下三角形，变成一个等腰梯形
                    setTile.Output(trapezium)));

            //生成底部等腰梯形，直接使用上面生成的形状
            WorldUtils.Gen(
            origin + new Point(0, 4),
                new ModShapes.All(trapezium),//三角形
                Actions.Chain(
                    new Modifiers.Flip(false, true),
                    setTile.Output(trapezium2)));

            //生成主体矩形
            WorldUtils.Gen(origin, rectangle, setTile.Output(rectangleData));

            //将上中下两个等腰梯形和一个矩形叠加变成主体形状
            trapezium.Add(trapezium2, origin + new Point(0, -4), origin + new Point(0, 4));
            trapezium.Add(rectangleData, origin + new Point(0, -4), origin);

            //挖空一条走廊
            WorldUtils.Gen(origin, rectangle2, new Actions.Clear());

            //挖出小空腔
            WorldUtils.Gen(origin, new Shapes.Circle(12, 8), new Actions.Clear());

            //放置小突起
            WorldUtils.Gen(origin + new Point(0, 4), new Shapes.Mound(8, 6),
                 Actions.Chain(
                     new Modifiers.Flip(false, true),
                     setTile));

            //填充墙壁
            ShapeData outline = new ShapeData();
            WorldUtils.Gen(origin, new ModShapes.OuterOutline(trapezium), new Modifiers.Expand(2).Output(outline));

            WorldUtils.Gen(origin + new Point(0, -4), new ModShapes.All(trapezium),
                 Actions.Chain(
                     new Modifiers.NotInShape(outline),//把边缘一圈去掉
                     new Actions.ClearWall(),
                     new Actions.PlaceWall(WallID.LihzahrdBrickUnsafe),
                     new Actions.SetFrames(true)));

            //放门
            for (int i = -1; i < 2; i += 2)
            {
                Point doorP = origin + new Point(i * 20 + (i > 0 ? 0 : -1), 0);
                WorldGen.PlaceDoor(doorP.X, doorP.Y, TileID.ClosedDoor, 11);
            }

            //放蜥蜴神庙
            WorldGen.PlaceObject(origin.X, origin.Y + 3, TileID.LihzahrdAltar, true);
        }
    }
}
