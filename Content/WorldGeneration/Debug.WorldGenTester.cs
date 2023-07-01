using Coralite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using Coralite.Content.Tiles.Magike;
using Terraria.GameContent.ItemDropRules;
using Conditions = Terraria.WorldBuilding.Conditions;

namespace Coralite.Content.WorldGeneration
{
    public class WorldGenTester:ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool CanUseItem(Player player)
        {
            ushort basalt = (ushort)ModContent.TileType<BasaltTile>();
            ushort crystalBasalt = (ushort)ModContent.TileType<CrystalBasaltTile>();
            ushort crystalBlock = (ushort)ModContent.TileType<MagikeCrystalBlockTile>();

            int minX = Main.maxTilesX / 4;
            int maxX = 3 * Main.maxTilesX / 4;

            int minY = Main.maxTilesY / 2;
            int maxY = Main.maxTilesY - 300;

            Point origin = new Point(WorldGen.genRand.Next(minX, maxX), WorldGen.genRand.Next(minY, maxY));

            int outWidthMin = 30;
            int outWidthMax = 45;
            int wallWidth = 3;
            if (Main.maxTilesX > 8000)
            {
                outWidthMin += 15;
                outWidthMax += 15;
                wallWidth += 2;
            }

            if (Main.maxTilesX > 6000)
            {
                outWidthMin += 15;
                outWidthMax += 15;
                wallWidth += 1;
            }

            // 通过使用TileScanner，检查以原点为中心的50x50区域是否大部分是泥土或石头。
            Dictionary<ushort, int> tileDictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(
                new Point(origin.X - 25, origin.Y - 25),
                new Shapes.Rectangle(50, 50),
                new Actions.TileScanner(TileID.Dirt, TileID.Stone).Output(tileDictionary));
            if (tileDictionary[TileID.Dirt] + tileDictionary[TileID.Stone] < 750)
                return false; //如果不是，则返回false，这将导致调用方法尝试一个不同的起源。


            ShapeData innerData = new ShapeData();
            ShapeData outerData = new ShapeData();
            Point point = origin;
            Point point2 = new Point(origin.X, origin.Y + 30);
            float xScale = WorldGen.genRand.NextFloat(0.8f, 1.1f);
            float yScale = WorldGen.genRand.NextFloat(0.8f, 1.1f);

            int width = (int)(WorldGen.genRand.Next(outWidthMin, outWidthMax) * xScale);
            int height = (int)(WorldGen.genRand.Next(outWidthMin, outWidthMax) * yScale);
            // 检查结构图中我们希望放置神龛的预定区域是否有任何现有冲突。
            //if (!structures.CanPlace(new Rectangle(point.X - (int)(20f * xScale), point.Y - 20, (int)(40f * xScale), 40)))
            //    return false;

            //挖出一个球，内部全空
            WorldUtils.Gen(
                point,
                new Shapes.Circle(width, height),
                Actions.Chain(
                    new Modifiers.Blotches(4, 0.4),
                    new Actions.Clear(),
                    new Actions.SetFrames(true).Output(innerData))) ;


            //生成外围圈
            WorldUtils.Gen(
                point,
                new ModShapes.OuterOutline(innerData),
                Actions.Chain(
                    new Modifiers.Expand(wallWidth,wallWidth),
                    new Actions.SetTile(basalt),
                    new Actions.SetFrames(true).Output(outerData)));


            //生成中心水晶小浮岛
            int howMany= WorldGen.genRand.Next(8, 16);

            for (int i = 0; i < howMany; i++)
            {
                Point selfPoint = point + Main.rand.NextVector2Circular(width * 0.75f, height * 0.75f).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Slime((int)WorldGen.genRand.NextFloat(width * 0.05f, width * 0.08f), 1f, WorldGen.genRand.NextFloat(0.25f, 0.5f)),
                    Actions.Chain(
                        new Modifiers.Flip(false, true),
                        new Modifiers.Blotches(2, 0.4),
                        new Actions.SetTile(crystalBasalt),
                        new Actions.SetFrames(true)));
            }

            //生成中心大浮岛
            for (int i = 0; i < 3; i++)
            {
                Point selfPoint = point + Main.rand.NextVector2Circular(width * 0.9f, height * 0.9f).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Slime((int)(width * 0.4f), WorldGen.genRand.NextFloat(1.2f, 1.5f), WorldGen.genRand.NextFloat(0.1f, 0.3f)),
                    Actions.Chain(
                        new Modifiers.Blotches(2, 0.4),
                        new Actions.SetTile(basalt),
                        new Actions.SetFrames(true)));
            }
            
            howMany = WorldGen.genRand.Next(8, 12);

            for (int i = 0; i < howMany; i++)
            {
                Point selfPoint = point + Main.rand.NextVector2Circular(width, height).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Slime((int)WorldGen.genRand.NextFloat(width * 0.05f, width * 0.15f), WorldGen.genRand.NextFloat(1f, 1.5f), WorldGen.genRand.NextFloat(0.3f, 0.6f)),
                    Actions.Chain(
                        new Modifiers.Blotches(2, 0.4),
                        new Actions.SetTile(basalt),
                        new Actions.SetFrames(true)));
            }


            //随机替换为水晶的
            //WorldUtils.Gen(
            //    point,
            //    new ModShapes.OuterOutline(innerData),
            //    Actions.Chain(
            //        new Actions.SetTile(crystalBasalt),
            //        new Actions.SetFrames(true).Output(outerData)));

            howMany = WorldGen.genRand.Next(12, 24);

            //生成小起伏
            for (int i = 0; i < howMany; i++)
            {
                Point selfPoint = point + Main.rand.NextVector2CircularEdge(width, height).ToPoint() + new Point(WorldGen.genRand.Next(-8, 8), WorldGen.genRand.Next(-3, 3));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Slime((int)WorldGen.genRand.NextFloat(width * 0.1f, width * 0.2f)),
                    Actions.Chain(
                        new Actions.SetTile(basalt),
                        new Actions.SetFrames(true)));
            }

            howMany = WorldGen.genRand.Next(12, 24);

            //生成玄武岩水晶
            for (int i = 0; i < howMany; i++)
            {
                Point selfPoint = point + Main.rand.NextVector2CircularEdge(width, height).ToPoint() + new Point(WorldGen.genRand.Next(-3, 3), WorldGen.genRand.Next(-3, 3));
                Vector2 dir = (point.ToVector2() - selfPoint.ToVector2()).SafeNormalize(Vector2.Zero).RotatedBy(WorldGen.genRand.NextFloat(-0.4f, 0.4f));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Tail(WorldGen.genRand.NextFloat(width * 0.08f, width * 0.1f), dir.ToVector2D() * WorldGen.genRand.NextFloat(width * 0.05f, width * 0.3f)),
                    Actions.Chain(
                        new Actions.SetTile(crystalBasalt),
                        new Actions.SetFrames(true)));
            }

            howMany = WorldGen.genRand.Next(12, 24);

            //生成水晶锥
            for (int i = 0; i < howMany; i++)
            {
                Point selfPoint = point + Main.rand.NextVector2CircularEdge(width, height).ToPoint()+new Point(WorldGen.genRand.Next(-3,3), WorldGen.genRand.Next(-3, 3));
                Vector2 dir = (point.ToVector2() - selfPoint.ToVector2()).SafeNormalize(Vector2.Zero).RotatedBy(WorldGen.genRand.NextFloat(-0.4f,0.4f));
                WorldUtils.Gen(
                    selfPoint,
                    new Shapes.Tail(WorldGen.genRand.NextFloat(width * 0.1f, width * 0.2f), dir.ToVector2D() * WorldGen.genRand.NextFloat(width * 0.05f, width * 0.55f)),
                    Actions.Chain(
                        new Actions.SetTile(crystalBlock),
                        new Actions.SetFrames(true)));
            }

            Point topLeft = origin - new Point(width, height);

            //添加斜坡
            WorldGenHelper.SmoothSlope(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, basalt, 5);
            WorldGenHelper.SmoothSlope(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, crystalBasalt, 5);
            WorldGenHelper.SmoothSlope(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, crystalBlock, 2);

            //生成小装饰物块
            WorldGenHelper.PlaceOnTopDecorations(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, (ushort)ModContent.TileType<BasaltStalactiteTop2>(), 4, 0, basalt);
            WorldGenHelper.PlaceOnGroundDecorations(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, (ushort)ModContent.TileType<BasaltStalactiteBottom2>(), 4, 0, basalt);

            WorldGenHelper.PlaceOnTopDecorations(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, (ushort)ModContent.TileType<BasaltStalactiteTop>(), 5, 0, basalt);
            WorldGenHelper.PlaceOnGroundDecorations(topLeft.X, topLeft.Y, 0, 0, width * 2, height * 2, (ushort)ModContent.TileType<BasaltStalactiteBottom>(), 5, 0, basalt);


            //int howMany = WorldGen.genRand.Next(1, 4);

            ////生成中心
            //int outWidth = WorldGen.genRand.Next(outWidthMin, outWidthMax);
            //WorldUtils.Gen(origin, new Shapes.Circle(outWidth, outWidth), new Actions.SetTile((ushort)ModContent.TileType<BasaltTile>(), true));
            //int inWidth = (int)(outWidth * WorldGen.genRand.NextFloat(0.7f,0.9f));
            //WorldUtils.Gen(origin, new Shapes.Circle(inWidth, inWidth), new Actions.Clear());

            //for (int i = 0; i < howMany; i++)
            //{
            //    outWidth = WorldGen.genRand.Next(outWidthMin, outWidthMax);
            //    inWidth = (int)(outWidth * 0.8f);

            //    Point offset = WorldGen.genRand.NextVector2CircularEdge(inWidth, inWidth).ToPoint();
            //    WorldUtils.Gen(origin + offset, new Shapes.Circle((int)(outWidth * WorldGen.genRand.NextFloat(0.9f, 1f)), (int)(outWidth * WorldGen.genRand.NextFloat(0.9f, 1f))), new Actions.SetTile((ushort)ModContent.TileType<BasaltTile>(), true));
            //    WorldUtils.Gen(origin + offset, new Shapes.Circle((int)(inWidth * WorldGen.genRand.NextFloat(0.9f, 1f)), (int)(inWidth * WorldGen.genRand.NextFloat(0.9f, 1f))), new Actions.Clear());


            //}



            return base.CanUseItem(player);
        }
    }
}
