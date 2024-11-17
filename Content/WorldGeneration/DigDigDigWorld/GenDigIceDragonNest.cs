using Coralite.Content.Tiles.Icicle;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheIceDragonNest { get; set; }

        public static void GenDigIceDragonNest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheIceDragonNest.Value;

            int x = (GenVars.snowOriginLeft + GenVars.snowOriginRight) / 2;
            Point origin = new Point(x, Main.maxTilesY / 2);

            Actions.SetTile setTile = new Actions.SetTile((ushort)ModContent.TileType<IcicleStoneTile>());
            var rectangle = new Shapes.Rectangle(new Rectangle(-200, 0, 400, 600));

            ShapeData bottomData = new ShapeData();
            ShapeData TopData = new ShapeData();
            ShapeData rectangleData = new ShapeData();

            WorldUtils.Gen(origin, rectangle, new Actions.Blank().Output(rectangleData));

            //生成下半平台
            WorldUtils.Gen(origin, new Shapes.Circle(13, 12),
                 Actions.Chain(
                     new Modifiers.Flip(false, true),
                     new Modifiers.InShape(rectangleData),
                     setTile.Output(bottomData)));

            //生成上半圆
            WorldUtils.Gen(origin, new Shapes.Circle(13, 24),
                 Actions.Chain(
                     new Modifiers.Flip(false, true),
                     new Modifiers.NotInShape(rectangleData),
                     setTile.Output(TopData)));

            TopData.Add(bottomData, origin, origin);

            ShapeData eddEdge = new ShapeData();
            //蛋边缘一圈
            WorldUtils.Gen(origin, new ModShapes.OuterOutline(TopData), new Modifiers.Expand(6).Output(eddEdge));

            //挖空小区域
            WorldUtils.Gen(
            origin,
           new ModShapes.All(TopData),//三角形
                Actions.Chain(
                    new Modifiers.NotInShape(eddEdge),//用矩形裁剪一下三角形，变成一个等腰梯形
                    new Actions.ClearTile(),
                    new Actions.PlaceWall(WallID.IceUnsafe)));

            WorldUtils.Gen(
            origin,
           new Shapes.Mound(3, 3),
                Actions.Chain(
                    new Modifiers.Flip(false, true),
                    setTile));

            NestCenter = origin + new Point(0, -2);
        }
    }
}
