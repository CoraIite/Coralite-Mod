using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Coralite.lib
{
    public class VanillaSwordBiome
    {
        public bool Place(Point origin, StructureMap structures)
        {
            // 通过使用TileScanner，检查以原点为中心的50x50区域是否大部分是泥土或石头。
            Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(
                new Point(origin.X - 25, origin.Y - 25),
                new Shapes.Rectangle(50, 50),
                new Actions.TileScanner(TileID.Dirt, TileID.Stone).Output(tileDictionary));
            if (tileDictionary[TileID.Dirt] + tileDictionary[TileID.Stone] < 1250)
                return false; //如果不是，则返回false，这将导致调用方法尝试一个不同的起源。

            Point surfacePoint;
            // 向上搜索多达1000块瓷砖，寻找一个50块瓷砖高、1块瓷砖宽、没有一块实心砖的区域。基本上找到表面。
            bool flag = WorldUtils.Find(
                origin,
                Searches.Chain(new Searches.Up(1000),
                new Conditions.IsSolid().AreaOr(1, 50).Not()), out surfacePoint);

            // 从原点到表面进行搜索，确保原点和表面之间没有沙子。
            if (WorldUtils.Find(
                origin,
                Searches.Chain(new Searches.Up(origin.Y - surfacePoint.Y),
                new Conditions.IsTile(TileID.Sand)), out Point _))
                return false;

            if (!flag)
                return false;

            surfacePoint.Y += 50; // 将结果调整为指向表面，而不是高于50块瓷砖的位置
            ShapeData slimeShapeData = new ShapeData();
            ShapeData moundShapeData = new ShapeData();
            Point point = new Point(origin.X, origin.Y + 20);
            Point point2 = new Point(origin.X, origin.Y + 30);
            float xScale = 0.8f + WorldGen.genRand.NextFloat() * 0.5f;
            // 随机安排神龛区的宽度
            // 检查结构图中我们希望放置神龛的预定区域是否有任何现有冲突。
            if (!structures.CanPlace(new Rectangle(point.X - (int)(20f * xScale), point.Y - 20, (int)(40f * xScale), 40)))
                return false;
            // 检查结构图中通往地面的竖井是否有任何现有冲突
            if (!structures.CanPlace(new Rectangle(origin.X, surfacePoint.Y + 10, 1, origin.Y - surfacePoint.Y - 9), 2))
                return false;


            // 使用史莱姆形状，清理出瓷砖。斑点使边缘看起来更加有机。 https://i.imgur.com/WtZaBbn.png
            WorldUtils.Gen(
                point,
                new Shapes.Slime(20, xScale, 1f),
                Actions.Chain(
                    new Modifiers.Blotches(2, 0.4),
                    new Actions.ClearTile(frameNeighbors: true).Output(slimeShapeData)));


            // 在切出的粘液形状内放置一个土堆。
            WorldUtils.Gen(
                point2,
                new Shapes.Mound(14, 14),
                Actions.Chain(
                    new Modifiers.Blotches(2, 1, 0.8),
                    new Actions.SetTile(TileID.Dirt),
                    new Actions.SetFrames(frameNeighbors: true).Output(moundShapeData)));


            // 从史莱姆坐标数据中删除土墩坐标
            slimeShapeData.Subtract(moundShapeData, point, point2);


            // 沿着史莱姆坐标数据的内部轮廓放置草方块
            WorldUtils.Gen(
                point,
                new ModShapes.InnerOutline(slimeShapeData),
                Actions.Chain(
                    new Actions.SetTile(TileID.Grass),
                    new Actions.SetFrames(frameNeighbors: true)));


            // 在史莱姆形状的下半部分的空坐标中放置水
            WorldUtils.Gen(
                point,
                new ModShapes.All(slimeShapeData),
                Actions.Chain(
                    new Modifiers.RectangleMask(-40, 40, 0, 40),
                    new Modifiers.IsEmpty(),
                    new Actions.SetLiquid()));


            // 在所有史莱姆形状的坐标上放置花墙。在粘液形状的所有草牌下放置一格藤蔓。
            WorldUtils.Gen(
                point,
                new ModShapes.All(slimeShapeData),
                Actions.Chain(
                    new Actions.PlaceWall(WallID.Flower),
                    new Modifiers.OnlyTiles(TileID.Grass),
                    new Modifiers.Offset(0, 1),
                    new ActionVines(3, 5)));


            // 拆除瓦片，创建轴心到地面。将沿轴线的沙子转换为硬化的沙。
            ShapeData shaftShapeData = new ShapeData();
            WorldUtils.Gen(
                new Point(origin.X, surfacePoint.Y + 10),
                new Shapes.Rectangle(1, origin.Y - surfacePoint.Y - 9),
                Actions.Chain(
                    new Modifiers.Blotches(2, 0.2),
                    new Actions.ClearTile().Output(shaftShapeData),
                    new Modifiers.Expand(1),
                    new Modifiers.OnlyTiles(TileID.Sand),
                    new Actions.SetTile(TileID.HardenedSand).Output(shaftShapeData)));


            //设置帧
            WorldUtils.Gen(
                new Point(origin.X, surfacePoint.Y + 10),
                new ModShapes.All(shaftShapeData),
                new Actions.SetFrames(frameNeighbors: true));


            // 有33%的几率放置一个被施了魔法的剑的神龛牌
            if (WorldGen.genRand.NextBool(3))
                WorldGen.PlaceTile(point2.X, point2.Y - 15, TileID.LargePiles2, mute: true, forced: false, -1, 17);
            else
                WorldGen.PlaceTile(point2.X, point2.Y - 15, TileID.LargePiles, mute: true, forced: false, -1, 15);
            // 将植物放置在土丘形状的草砖之上。
            WorldUtils.Gen(
                point2,
                new ModShapes.All(moundShapeData),
                Actions.Chain(
                    new Modifiers.Offset(0, -1),
                    new Modifiers.OnlyTiles(TileID.Grass),
                    new Modifiers.Offset(0, -1), new ActionGrass()));
            // 添加到结构图中，以防止其他世界源与该区域相交。
            structures.AddStructure(new Rectangle(point.X - (int)(20f * xScale), point.Y - 20, (int)(40f * xScale), 40), 4);
            return true;

        }
    }
}
