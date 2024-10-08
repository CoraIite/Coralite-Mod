using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheGMHalfCircle { get; set; }

        public static void GenGMHalfCircle(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheGMHalfCircle.Value;

            GenHalfCircle(progress);
        }

        private static void GenHalfCircle(GenerationProgress progress)
        {
            Point origin = new Point(Main.maxTilesX / 2, Main.maxTilesY / 2);

            int side = GenVars.dungeonSide;

            int minR = GraniteMarbleRadius - 45;
            int maxR = GraniteMarbleRadius + 45;

            Shapes.Circle circle = new Shapes.Circle(maxR);
            Shapes.Circle circle2 = new Shapes.Circle(minR);

            Rectangle rect = new Rectangle((side > 0 ? 0 : -2000), -1000, 2000, 2000);
            Shapes.Rectangle rectangle = new Shapes.Rectangle(rect);
            Rectangle rect2 = new Rectangle((side < 0 ? 0 : -2000), -1000, 2000, 2000);
            Shapes.Rectangle rectangle2 = new Shapes.Rectangle(rect2);

            ShapeData rectangleData = new ShapeData();//准备一个圆形和两个矩形
            ShapeData rectangleData2 = new ShapeData();
            ShapeData circleData = new ShapeData();
            ShapeData circleData2 = new ShapeData();

            //让两个圆相减得到一个圆环
            WorldUtils.Gen(origin, rectangle, new Actions.Blank().Output(rectangleData));
            WorldUtils.Gen(origin, rectangle2, new Actions.Blank().Output(rectangleData2));
            WorldUtils.Gen(origin, circle2, new Actions.Blank().Output(circleData2));
            WorldUtils.Gen(origin, circle, Actions.Chain(new Modifiers.NotInShape(circleData2), new Actions.Blank().Output(circleData)));

            ModShapes.All circleShape = new ModShapes.All(circleData);
            Modifiers.RadialDither radialDither = new Modifiers.RadialDither(maxR - 10, maxR);

            WorldUtils.Gen(
                origin,  //中心点
                circleShape,   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    new Modifiers.InShape(rectangleData),//与矩形相交，裁成半圆环
                    radialDither,
                    new Actions.Clear(),    //清除形状内所有物块
                    new Actions.SetTile(TileID.Marble),//放大理石块
                    new Actions.PlaceWall(WallID.MarbleUnsafe)));   //放墙壁

            progress.Value = 0.5f;

            WorldUtils.Gen(
                origin,  //中心点
                circleShape,   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    new Modifiers.InShape(rectangleData2),//与矩形相交，裁成半圆环
                    radialDither,
                    new Actions.Clear(),    //清除形状内所有物块
                    new Actions.SetTile(TileID.Granite),//放花岗岩
                    new Actions.PlaceWall(WallID.GraniteUnsafe)));   //放墙壁
        }
    }
}