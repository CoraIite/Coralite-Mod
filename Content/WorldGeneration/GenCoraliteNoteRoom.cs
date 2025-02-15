using Coralite.Content.CoraliteNotes;
using Coralite.Content.Items.CoreKeeper;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText CoraliteNoteRoom { get; set; }

        public void GenCoraliteNoteRoom(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = CoraliteNoteRoom.Value;

            Point spawnPoint = new Point(Main.spawnTileX, Main.spawnTileY);
            bool spawn = false;

            for (int i = 0; i < 200; i++)
            {
                try
                {
                    int y = WorldGen.genRand.Next(32, 38);

                    Point position = spawnPoint + new Point(WorldGen.genRand.Next(-120, 120), y);

                    Dictionary<ushort, int> tileDictionary = new();
                    if (!WorldGen.InWorld(position.X - 14, position.Y - 11) || !WorldGen.InWorld(position.X + 14, position.Y + 11))
                        continue;
                    WorldUtils.Gen(
                        new Point(position.X - 14, position.Y - 11),
                        new Shapes.Rectangle(28, 23),
                        new Actions.TileScanner(TileID.Dirt, TileID.Grass, TileID.Mud, TileID.JungleGrass, TileID.Ash, TileID.AshGrass, TileID.Stone, TileID.ClayBlock).Output(tileDictionary));

                    int tileCount = 0;

                    foreach (var item in tileDictionary)
                        tileCount += item.Value;

                    if (tileCount < 20 * 20)
                        continue; //如果物块太少那就换个地方

                    PlaceCoraliteNoteRoom(position);
                    spawn = true;
                    break;
                }
                catch (System.Exception)
                {
                    continue;
                }
            }

            if (!spawn)
            {
                int y = WorldGen.genRand.Next(32, 38);
                Point position = spawnPoint + new Point(WorldGen.genRand.Next(-120, 120), y);

                PlaceCoraliteNoteRoom(position);
            }
        }

        public void PlaceCoraliteNoteRoom(Point center)
        {
            Point position = new Point(center.X - 14, center.Y - 11);

            Texture2D roomTex = ModContent.Request<Texture2D>(AssetDirectory.CoraliteNoteRoom + "CoraliteNoteRoom", AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.CoraliteNoteRoom + "CoraliteNoteRoomClear", AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.CoraliteNoteRoom + "CoraliteNoteRoomWall", AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallClearTex = ModContent.Request<Texture2D>(AssetDirectory.CoraliteNoteRoom + "CoraliteNoteRoomWallClear", AssetRequestMode.ImmediateLoad).Value;

            Dictionary<Color, int> mainDic = new()
            {
                [new Color(215, 123, 186)] = TileID.ReefBlock,//d77bba
                [new Color(217, 160, 102)] = TileID.Coralstone,//d9a066
                [new Color(132, 126, 135)] = TileID.GrayBrick,//847e87
                [new Color(255, 211, 238)] = TileID.PlatinumBrick,//ffd3ee
                [new Color(90, 100, 80)] = TileID.Chain,//5a6450
                [Color.Black] = -1
            };
            Dictionary<Color, int> wallDic = new()
            {
                [new Color(138, 0, 255)] = WallID.ArcaneRunes,//8a00ff
                [new Color(255, 0, 175)] = WallID.ReefWall,//ff00af
                [new Color(85, 85, 85)] = WallID.GrayBrick,//555555
                [Color.Black] = -1
            };

            GenByTexture(clearTex, roomTex, wallClearTex, wallTex, clearDic, mainDic, clearDic, wallDic, position.X, position.Y);

            //放火把
            WorldGen.PlaceObject(position.X + 11, position.Y + 7, TileID.Torches, true, TorchID.Shimmer);
            WorldGen.PlaceObject(position.X + 17, position.Y + 7, TileID.Torches, true, TorchID.Shimmer);
            //生成珊瑚笔记物块
            WorldGen.PlaceObject(position.X + 14, position.Y + 10, ModContent.TileType<CoraliteNoteTile>(), true);
            //生成箱子
            WorldGen.AddBuriedChest(position.X + 23, position.Y + 9, ModContent.ItemType<ChannelingGemstone>(),
                             notNearOtherChests: false, 14, trySlope: false, TileID.Containers2);
            //生成帐篷
            WorldGen.PlaceObject(position.X + 6, position.Y + 13, TileID.LargePiles2, true, 26);

            //刷漆
            for (int i = 0; i < 28; i++)
                for (int j = 0; j < 23; j++)
                {
                    int x = position.X + i;
                    int y = position.Y + j;
                    Tile t = Main.tile[x, y];
                    if (t.HasTile)
                    {
                        if (t.TileType == TileID.Coralstone)
                            WorldGen.paintTile(x, y, PaintID.WhitePaint);
                        else if (t.TileType == TileID.ReefBlock || t.TileType == TileID.Containers2)
                            WorldGen.paintTile(x, y, PaintID.GrayPaint);
                    }

                    if (t.WallType > 0)
                    {
                        if (t.WallType == WallID.ReefWall)
                            WorldGen.paintWall(x, y, PaintID.WhitePaint);
                        else if (t.WallType == WallID.ArcaneRunes)
                            WorldGen.paintWall(x, y, PaintID.DeepPinkPaint);
                    }
                }

            if (Main.remixWorld)
                return;

            ShapeData shaftShapeData = new ShapeData();
            Point tunnelP = new Point(position.X + WorldGen.genRand.Next(5, 7), position.Y + 23);
            WorldUtils.Gen(
                tunnelP,
                new Shapes.Rectangle(1, WorldGen.genRand.Next(80, 100)),
                Actions.Chain(
                    new Modifiers.Blotches(2, 0.2),
                    new Actions.ClearTile().Output(shaftShapeData),
                    new Modifiers.Expand(1),
                    new Modifiers.OnlyTiles(TileID.Sand),
                    new Actions.SetTile(TileID.HardenedSand).Output(shaftShapeData)));

            //设置帧
            WorldUtils.Gen(
                tunnelP,
                new ModShapes.All(shaftShapeData),
                new Actions.SetFrames(frameNeighbors: true));

        }
    }
}
