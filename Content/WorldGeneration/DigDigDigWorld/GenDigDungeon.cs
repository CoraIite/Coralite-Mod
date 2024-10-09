using System;
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
        public static LocalizedText DigTheDigDungeon { get; set; }

        private static int DungeonLeft;
        private static int DungeonRight;

        public static void GenGenDigDungeon(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheDigDungeon.Value;

            GenDungeonBar(progress);
            GenDungeonEntrance(progress);
        }

        private static void GenDungeonBar(GenerationProgress progress)
        {
            int dungeonSide = GenVars.dungeonSide;

            int num = WorldGen.genRand.Next(3);

            ushort brick;
            (ushort, ushort, ushort) wall;
            switch (num)
            {
                case 0:
                    brick = TileID.BlueDungeonBrick;
                    wall = (WallID.BlueDungeonUnsafe, WallID.BlueDungeonSlabUnsafe, WallID.BlueDungeonTileUnsafe);
                    GenVars.crackedType = TileID.CrackedBlueDungeonBrick;
                    break;
                case 1:
                    brick = TileID.GreenDungeonBrick;
                    wall = (WallID.GreenDungeonUnsafe, WallID.GreenDungeonSlabUnsafe, WallID.GreenDungeonTileUnsafe);
                    GenVars.crackedType = TileID.CrackedGreenDungeonBrick;
                    break;
                default:
                    brick = TileID.PinkDungeonBrick;
                    wall = (WallID.PinkDungeonUnsafe, WallID.PinkDungeonSlabUnsafe, WallID.PinkDungeonTileUnsafe);
                    GenVars.crackedType = TileID.CrackedPinkDungeonBrick;
                    break;
            }

            int center = Main.maxTilesX / 2;
            int width = Main.maxTilesX / 3 + WorldGen.genRand.Next(-15, 30);
            int offset = Main.maxTilesX / 80;

            int dungeonLimit = 80;
            int dir = 0;
            int yDir = 1;
            int genCount = WorldGen.genRand.Next(10, 40);

            int x = center + dungeonSide * (width + offset);
            int min = x - dungeonLimit;
            int max = x + dungeonLimit;

            DungeonLeft = min;
            DungeonRight = max;

            Point origin = new Point(x, 30);

            while (origin.Y < Main.maxTilesY - 30)//生成一条地牢
            {
                ushort wallType = wall.Item1;
                if (origin.Y > Main.maxTilesY / 3)
                    wallType = wall.Item2;
                if (origin.Y > Main.maxTilesY * 2 / 3)
                    wallType = wall.Item3;

                GenDungeonBox(origin, brick, GenVars.crackedType, wallType);

                genCount--;
                if (genCount < 1)//生成结束，换条路
                {
                    genCount = WorldGen.genRand.Next(10, 40);
                    dir = WorldGen.genRand.Next(-1, 2);

                    if (yDir == 1 && WorldGen.genRand.NextBool(8))
                    {
                        yDir = 0;
                        genCount = WorldGen.genRand.Next(15, 30);
                        dir = WorldGen.genRand.NextFromList(-1, 1);
                    }
                    else
                        yDir = 1;

                    if (origin.X < min + 8)
                        dir = 1;
                    else if (origin.X > max - 8)
                        dir = -1;
                }

                origin += new Point(dir, yDir);
                origin.X = Math.Clamp(origin.X, min, max);
            }

            progress.Value = 0.75f;
        }

        private static void GenDungeonBox(Point origin, ushort tileType,ushort crackedTileType, ushort wallType)
        {
            for (int i = -7; i < 8; i++)
                for (int j = -7; j < 8; j++)
                {
                    Tile t = Main.tile[origin.X + i, origin.Y + j];

                    if (i > -6 && i < 7 && j > -6 && j < 7)//限制区域放置墙壁
                    {
                        t.Clear(Terraria.DataStructures.TileDataType.Wall);
                        WorldGen.PlaceWall(origin.X + i, origin.Y + j, wallType, true);
                    }

                    if (t.TileType != crackedTileType)
                        t.ResetToType(tileType);

                    if (i > -3 && i < 3 && j > -3 && j < 3)//生成碎砖
                        t.ResetToType(crackedTileType);
                }
        }
   
        private static void GenDungeonEntrance(GenerationProgress progress)
        {
            int y = Main.maxTilesY / 2;

            int x = DungeonLeft;

            int Width = 0;

            for (int i = DungeonLeft; i < DungeonRight; i++)//找一下地牢位置和宽度
            {
                Tile t = Main.tile[i, y];
                if (TileID.Sets.DungeonBiome[t.TileType] > 0)
                {
                    if (Width == 0)
                        x = i;
                    Width++;
                }
                else if (Width > 0)
                    break;
            }

            x -= 8;
            Width += 16;
            y -= 10;

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < 20; j++)
                {
                    Tile t = Main.tile[x + i, y + j];
                    if (j > 4 && j < 20 - 5)//中间清空，间隔放置石柱
                    {
                        t.ClearEverything();
                        WorldGen.PlaceWall(x + i, y + j, WallID.GrayBrick, true);
                        if (i % 3 == 0 && i > 1 && i < Width - 1)
                            t.ResetToType(TileID.MarbleColumn);

                        continue;
                    }

                    bool useMoss = false;

                    if (i == 0 || i == Width - 1)
                        useMoss = true;

                    if (j == 0 || j == 4 || j == 20 - 5 || j == 20 - 1)
                        useMoss = true;

                    ushort tileType = useMoss ? TileID.RainbowMossBrick : TileID.GrayBrick;

                    if (TileID.Sets.CrackedBricks[t.TileType])//跳过碎砖
                    {
                        t.ResetToType(TileID.HeavenforgeBrick);//铺上一层石砖
                        continue;
                    }

                    t.Clear(TileDataType.Wall);
                    t.ResetToType(tileType);//铺上一层石砖
                }

            Main.dungeonX = x + Width / 2;
            Main.dungeonY = y + 8;

            NPC.NewNPC(new EntitySource_WorldGen(), Main.dungeonX * 16, Main.dungeonY * 16, NPCID.OldMan);
        }
    }
}
