using Coralite.Content.Items.FairyCatcher;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public void GenElfPortal(GenerationProgress progress, GameConfiguration configuration)
        {
            Point origin = FairyPortalSpawnPos();

            GenGenFairyPortal_AcutallyGen(origin);
        }

        public static void GenGenFairyPortal_AcutallyGen(Point origin)
        {
            //最外边的圆形
            ShapeData OutCircleData = new ShapeData();

            WorldUtils.Gen(origin
                , new Shapes.Circle(35,25)
                , Actions.Chain(
                    new Actions.ClearTile()
                    , new Actions.PlaceTile(TileID.Stone)
                    , new Actions.SetFrames(true).Output(OutCircleData)));

            //获得内部史莱姆形状，顺便清空一下
            ShapeData slimeData = new ShapeData();
            WorldUtils.Gen(origin
                , new Shapes.Slime(20, 1.5f, 0.8f)
                , Actions.Chain(
                    new Actions.ClearTile()
                    , new Actions.SetFrames(true).Output(slimeData)
                    ,new Modifiers.Expand(2)
                    ,new Modifiers.Blotches(5)));

            //通道的矩形
            //ShapeData tunnelRectData = new ShapeData();

            //int tunnelWidth = 80;
            //int tunnelHeight = 8;

            //WorldUtils.Gen(origin
            //    , new Shapes.Rectangle(new Rectangle(-tunnelWidth / 2, -tunnelHeight / 2, tunnelWidth, tunnelHeight))
            //    , Actions.Chain(
            //        new Modifiers.Blotches()
            //        , new Actions.ClearTile()
            //        , new Actions.SetFrames(true).Output(tunnelRectData)));

            //内部的边缘
            //slimeData.Add(tunnelRectData, origin, origin);

            //向外扩张，生成泥土
            WorldUtils.Gen(origin
                , new ModShapes.OuterOutline(slimeData)
                , Actions.Chain(
                    new Modifiers.Blotches(),
                    new Modifiers.Expand(2),
                    new Modifiers.OnlyTiles(TileID.Stone),
                    new Actions.SetTile(TileID.Dirt)));

            //在泥土边缘生成草方块
            WorldUtils.Gen(origin
                , new ModShapes.OuterOutline(slimeData)
                , Actions.Chain(
                    new Modifiers.OnlyTiles(TileID.Dirt),
                    new Actions.SetTile(TileID.Grass, true),
                    new Actions.Smooth(true),
                    new Actions.SetFrames(true)));

            //在草方块上生成杂草
            WorldUtils.Gen(origin
                , new ModShapes.All(slimeData)
                , Actions.Chain(
                    new Modifiers.Expand(1),
                    new Modifiers.IsTouchingAir(),
                    new ActionGrass(),
                    new Actions.SetFrames(true)));

            //生成几条从上到下的以太块追追
            for (int i = 0; i <= 5; i++)
            {
                //从左到右均匀分布追追
                int x = -24 + i * (24 * 2 / 5) + Main.rand.Next(-4, 4);
                int y = (int)(MathF.Sin((x + 24f) / (24f * 2) * MathHelper.Pi) * 15);
                Point pos = origin + new Point(x, -y);

                int width = Main.rand.Next(2, 4);

                //在微光湖底部和点位生成一条以太块方块
                WorldUtils.Gen(pos
                    , new Shapes.Rectangle(new Rectangle(-width / 2, -35, width, 35))
                    , Actions.Chain(
                        new Modifiers.Blotches(2),
                        new Modifiers.IsSolid(),
                        new Modifiers.Dither(0.2f),
                        new Actions.SetTile(TileID.ShimmerBlock),
                        new Actions.SetFrames(true)));

                //在方块下面生成尖端
                WorldUtils.Gen(pos + new Point(0, -12)
                    , new Shapes.Tail(width * 1.5f, new Vector2D(0, Main.rand.NextFloat(10, 14)))
                    , Actions.Chain(
                        new Modifiers.Blotches(1),
                        new Actions.SetTile(TileID.ShimmerBlock),
                        new Actions.SetFrames(true)));
            }

            WorldGen.PlaceObject(origin.X, origin.Y + 7, ModContent.TileType<ElfPortalTile>(), true);
        }

        public static Point FairyPortalSpawnPos()
        {
            Vector2D pos = GenVars.shimmerPosition;

            if (pos.X == 0 || pos.Y == 0)//唉，到底是谁在动这些东西，能别瞎即把搞吗
                return new Point(Main.maxTilesX / 2, Main.maxTilesY - 600);//没有微光湖强制生成到世界中心

            //pos.X += GenVars.dungeonSide * 170;
            return new Point((int)pos.X, (int)pos.Y + 45);
        }
    }

    public class ActionVioletMoss : GenAction
    {
        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            //这个_tiles引用的就是Main.tile
            //如果自身这一格没有物块或者顶上的一格有物块就跳过
            if (!_tiles[x, y].HasTile || WorldGen.genRand.NextBool(3, 5))
                return false;

            //放置苔藓
            if (!_tiles[x, y - 1].HasTile)
                WorldGen.PlaceTile(x, y - 1, TileID.LongMoss, mute: true, style: 9);
            if (!_tiles[x, y + 1].HasTile)
                WorldGen.PlaceTile(x, y + 1, TileID.LongMoss, mute: true, style: 9);
            if (!_tiles[x - 1, y].HasTile)
                WorldGen.PlaceTile(x - 1, y, TileID.LongMoss, mute: true, style: 9);
            if (!_tiles[x + 1, y].HasTile)
                WorldGen.PlaceTile(x + 1, y, TileID.LongMoss, mute: true, style: 9);

            return UnitApply(origin, x, y, args);
        }
    }

}
