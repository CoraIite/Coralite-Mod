using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheMushroomCircle { get; set; }

        public static void GenMushroomCircle(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheMushroomCircle.Value;

            Point origin = new Point(Main.maxTilesX / 2, Main.maxTilesY / 2);

            int side = GenVars.dungeonSide;

            int minR = MushroomRadius - 60;
            int maxR = MushroomRadius + 60;

            Shapes.Circle circle = new Shapes.Circle(maxR);
            Shapes.Circle circle2 = new Shapes.Circle(minR);

            ShapeData circleData = new ShapeData();
            ShapeData circleData2 = new ShapeData();

            //让两个圆相减得到一个圆环
            WorldUtils.Gen(origin, circle2, new Actions.Blank().Output(circleData2));
            WorldUtils.Gen(origin, circle, Actions.Chain(new Modifiers.NotInShape(circleData2), new Actions.Blank().Output(circleData)));

            ModShapes.All circleShape = new ModShapes.All(circleData);
            Modifiers.RadialDither radialDither = new Modifiers.RadialDither(maxR - 10, maxR);
            Modifiers.RadialDither radialDither2 = new Modifiers.RadialDither(minR + 10, minR);

            WorldUtils.Gen(
                origin,  //中心点
                circleShape,   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    radialDither,
                    radialDither2,
                    new Actions.Clear(),    //清除形状内所有物块
                    new Actions.SetTile(TileID.Mud),//放泥
                    new Actions.PlaceWall(WallID.MushroomUnsafe)));   //放墙壁

            WorldUtils.Gen(
                origin,  //中心点
                circleShape,   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    radialDither,
                    radialDither2,
                   new Modifiers.Dither(0.9f),
                    new Actions.ClearTile(),    //清除形状内所有物块
                    new Actions.SetTile(TileID.MushroomGrass)));//放蘑菇草块
        }
    }
}
