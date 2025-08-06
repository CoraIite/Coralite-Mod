using Coralite.Content.Items.FairyCatcher;
using ReLogic.Utilities;
using Terraria;
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
                , new Shapes.Circle(30)
                , Actions.Chain(
                    new Actions.Clear()
                    , new Actions.PlaceTile(TileID.Stone)
                    , new Actions.SetFrames(true).Output(OutCircleData)));

            //获得内部史莱姆形状，顺便清空一下
            ShapeData slimeData = new ShapeData();
            WorldUtils.Gen(origin
                , new Shapes.Slime(20, 1, 1)
                , Actions.Chain(
                    new Actions.ClearTile()
                    , new Actions.SetFrames(true).Output(slimeData)
                    ,new Modifiers.Expand(5)
                    ,new Modifiers.Blotches(5)
                    ,new Actions.PlaceWall(WallID.CaveUnsafe)));

            //通道的矩形
            ShapeData tunnelRectData = new ShapeData();

            int tunnelWidth = 80;
            int tunnelHeight = 8;

            WorldUtils.Gen(origin
                , new Shapes.Rectangle(new Rectangle(-tunnelWidth / 2, -tunnelHeight / 2, tunnelWidth, tunnelHeight))
                , Actions.Chain(
                    new Modifiers.Blotches()
                    , new Actions.ClearTile()
                    , new Actions.SetFrames(true).Output(tunnelRectData)));

            //内部的边缘
            slimeData.Add(tunnelRectData, origin, origin);

            WorldUtils.Gen(origin
                , new ModShapes.OuterOutline(slimeData)
                , Actions.Chain(
                    new Modifiers.OnlyTiles(TileID.Stone)
                    , new Actions.SetTile(TileID.VioletMoss)
                     , new ActionVioletMoss()
                     , new Actions.SetFrames(true)));

            WorldGen.PlaceObject(origin.X, origin.Y + 9, ModContent.TileType<ElfPortalTile>(), true);


        }

        public static Point FairyPortalSpawnPos()
        {
            Vector2D pos = GenVars.shimmerPosition;

            if (pos.X == 0 || pos.Y == 0)//唉，到底是谁在动这些东西，能别瞎即把搞吗
                return new Point(Main.maxTilesX / 2, Main.maxTilesY - 600);//没有微光湖强制生成到世界中心

            pos.X += GenVars.dungeonSide * 170;
            return new Point((int)pos.X, (int)pos.Y-5);
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
