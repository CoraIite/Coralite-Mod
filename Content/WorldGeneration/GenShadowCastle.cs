using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Content.UI;
using Coralite.Content.WorldGeneration.Generators;
using Coralite.Content.WorldGeneration.ShadowCastleRooms;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
//using static Terraria.WorldGen;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        internal static Rectangle shadowCastleRestraint;

        public static bool ShadowCastle
        {
            get
            {
                return CoraliteWorldSettings.DungeonType switch
                {
                    CoraliteWorldSettings.WorldDungeonID.Random => Main.rand.NextBool(),
                    CoraliteWorldSettings.WorldDungeonID.ShadowCastle => true,
                    _ => false,
                };
            }
        }

        public void GenShadowCastle(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "正在修建影之城";

            GenVars.dEnteranceX = 0;
            GenVars.numDRooms = 0;
            GenVars.numDDoors = 0;
            GenVars.numDungeonPlatforms = 0;

            int limit = 700;
            int roomCount = 40;
            if (Main.maxTilesX > 6000)
            {
                limit += 100;
                roomCount = 50;
            }
            if (Main.maxTilesY > 8000)
            {
                limit += 100;
                roomCount = 60;
            }

            for (int i = 0; i < 20000; i++)
            {
                int dungeonLocation = GenVars.dungeonLocation;
                int num756 = (int)((Main.worldSurface + Main.rockLayer) / 2.0) + 200;
                int dungeonHeight = (int)((Main.worldSurface + Main.rockLayer) / 2.0) + WorldGen.genRand.Next(-200, 200);
                bool flag47 = false;
                for (int j = 0; j < 10; j++)
                    if (WorldGen.SolidTile(dungeonLocation, dungeonHeight + j))
                    {
                        flag47 = true;
                        break;
                    }

                if (!flag47)
                    for (; dungeonHeight < num756 && !WorldGen.SolidTile(dungeonLocation, dungeonHeight + 10); dungeonHeight++)
                    { }

                if (WorldGen.drunkWorldGen)
                    dungeonHeight = (int)Main.worldSurface + 70;

                shadowCastleRestraint = new Rectangle(dungeonLocation - limit, dungeonHeight, limit * 2, Main.UnderworldLayer - dungeonHeight);

                NormalRoom root = new NormalRoom(new Point(dungeonLocation, dungeonHeight));
                root.InitializeType();

                if (root.shadowCastleRooms.Count < roomCount)
                    continue;

                List<ShadowCastleRoom> rooms = root.shadowCastleRooms;

                for (int m = 0; m < rooms.Count; m++)
                {
                    ShadowCastleRoom room = rooms[m];

                    #region 最优先：尖塔替换
                    if ((room.childrenRooms == null || room.childrenRooms.Count == 0)
                        && room.parentDirection != ShadowCastleRoom.Direction.Down
                        && WorldGen.genRand.NextBool())
                    {
                        Spire spire = new Spire(room.roomRect.Center);//底端换成 我超，塔！
                        ShadowCastleRoom.Exchange(room, spire);//交换一下信息
                        rooms[m] = spire;//替换列表里的
                        continue;
                    }
                    #endregion

                }

                root.Generate();
                root.CreateCorridor();
                break;
            }
        }

        #region 原版地牢
        public static void MakeDungeon(int x, int y)
        {
            GenVars.dEnteranceX = 0;
            GenVars.numDRooms = 0;
            GenVars.numDDoors = 0;
            GenVars.numDungeonPlatforms = 0;
            int brickTypeRandom = WorldGen.genRand.Next(3);
            WorldGen.genRand.Next(3);
            if (WorldGen.remixWorldGen)
                brickTypeRandom = (WorldGen.crimson ? 2 : 0);

            ushort brickTileType;
            int BrickwallType;
            switch (brickTypeRandom)
            {
                case 0:
                    brickTileType = 41;
                    BrickwallType = 7;
                    GenVars.crackedType = 481;
                    break;
                case 1:
                    brickTileType = 43;
                    BrickwallType = 8;
                    GenVars.crackedType = 482;
                    break;
                default:
                    brickTileType = 44;
                    BrickwallType = 9;
                    GenVars.crackedType = 483;
                    break;
            }

            Main.tileSolid[GenVars.crackedType] = false;
            GenVars.dungeonLake = true;
            GenVars.numDDoors = 0;
            GenVars.numDungeonPlatforms = 0;
            GenVars.numDRooms = 0;
            GenVars.dungeonX = x;
            GenVars.dungeonY = y;
            GenVars.dMinX = x;
            GenVars.dMaxX = x;
            GenVars.dMinY = y;
            GenVars.dMaxY = y;
            GenVars.dxStrength1 = WorldGen.genRand.Next(25, 30);
            GenVars.dyStrength1 = WorldGen.genRand.Next(20, 25);
            GenVars.dxStrength2 = WorldGen.genRand.Next(35, 50);
            GenVars.dyStrength2 = WorldGen.genRand.Next(10, 15);
            double num4 = Main.maxTilesX / 60;
            num4 += WorldGen.genRand.Next(0, (int)(num4 / 3.0));
            double num5 = num4;
            int num6 = 5;
            DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
            while (num4 > 0.0)
            {
                if (GenVars.dungeonX < GenVars.dMinX)
                    GenVars.dMinX = GenVars.dungeonX;

                if (GenVars.dungeonX > GenVars.dMaxX)
                    GenVars.dMaxX = GenVars.dungeonX;

                if (GenVars.dungeonY > GenVars.dMaxY)
                    GenVars.dMaxY = GenVars.dungeonY;

                num4 -= 1.0;
                Main.statusText = Lang.gen[58].Value + " " + (int)((num5 - num4) / num5 * 60.0) + "%";
                if (num6 > 0)
                    num6--;

                if ((num6 == 0) & (WorldGen.genRand.NextBool(3)))
                {
                    num6 = 5;
                    if (WorldGen.genRand.NextBool(2))
                    {
                        int dungeonX = GenVars.dungeonX;
                        int dungeonY = GenVars.dungeonY;
                        DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
                        if (WorldGen.genRand.NextBool(2))
                            DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);

                        DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
                        GenVars.dungeonX = dungeonX;
                        GenVars.dungeonY = dungeonY;
                    }
                    else
                    {
                        DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
                    }
                }
                else
                {
                    DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
                }
            }

            DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
            int num7 = GenVars.dRoomX[0];
            int num8 = GenVars.dRoomY[0];
            for (int i = 0; i < GenVars.numDRooms; i++)
            {
                if (GenVars.dRoomY[i] < num8)
                {
                    num7 = GenVars.dRoomX[i];
                    num8 = GenVars.dRoomY[i];
                }
            }

            GenVars.dungeonX = num7;
            GenVars.dungeonY = num8;
            GenVars.dEnteranceX = num7;
            GenVars.dSurface = false;
            num6 = 5;
            if (WorldGen.drunkWorldGen)
                GenVars.dSurface = true;

            while (!GenVars.dSurface)
            {
                if (num6 > 0)
                    num6--;

                if (num6 == 0 && WorldGen.genRand.NextBool(5) && GenVars.dungeonY > Main.worldSurface + 100.0)
                {
                    num6 = 10;
                    int dungeonX2 = GenVars.dungeonX;
                    int dungeonY2 = GenVars.dungeonY;
                    DungeonHalls(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType, forceX: true);
                    DungeonRoom(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
                    GenVars.dungeonX = dungeonX2;
                    GenVars.dungeonY = dungeonY2;
                }

                DungeonStairs(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
            }

            DungeonEnt(GenVars.dungeonX, GenVars.dungeonY, brickTileType, BrickwallType);
            Main.statusText = Lang.gen[58].Value + " 65%";
            int num9 = Main.maxTilesX * 2;
            int num10;
            for (num10 = 0; num10 < num9; num10++)
            {
                int i2 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num11 = GenVars.dMinY;
                if ((double)num11 < Main.worldSurface)
                    num11 = (int)Main.worldSurface;

                int j = WorldGen.genRand.Next(num11, GenVars.dMaxY);
                num10 = ((!DungeonPitTrap(i2, j, brickTileType, BrickwallType)) ? (num10 + 1) : (num10 + 1500));
            }

            for (int k = 0; k < GenVars.numDRooms; k++)
            {
                for (int l = GenVars.dRoomL[k]; l <= GenVars.dRoomR[k]; l++)
                {
                    if (!Main.tile[l, GenVars.dRoomT[k] - 1].HasTile)
                    {
                        GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = l;
                        GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = GenVars.dRoomT[k] - 1;
                        GenVars.numDungeonPlatforms++;
                        break;
                    }
                }

                for (int m = GenVars.dRoomL[k]; m <= GenVars.dRoomR[k]; m++)
                {
                    if (!Main.tile[m, GenVars.dRoomB[k] + 1].HasTile)
                    {
                        GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = m;
                        GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = GenVars.dRoomB[k] + 1;
                        GenVars.numDungeonPlatforms++;
                        break;
                    }
                }

                for (int n = GenVars.dRoomT[k]; n <= GenVars.dRoomB[k]; n++)
                {
                    if (!Main.tile[GenVars.dRoomL[k] - 1, n].HasTile)
                    {
                        GenVars.DDoorX[GenVars.numDDoors] = GenVars.dRoomL[k] - 1;
                        GenVars.DDoorY[GenVars.numDDoors] = n;
                        GenVars.DDoorPos[GenVars.numDDoors] = -1;
                        GenVars.numDDoors++;
                        break;
                    }
                }

                for (int num12 = GenVars.dRoomT[k]; num12 <= GenVars.dRoomB[k]; num12++)
                {
                    if (!Main.tile[GenVars.dRoomR[k] + 1, num12].HasTile)
                    {
                        GenVars.DDoorX[GenVars.numDDoors] = GenVars.dRoomR[k] + 1;
                        GenVars.DDoorY[GenVars.numDDoors] = num12;
                        GenVars.DDoorPos[GenVars.numDDoors] = 1;
                        GenVars.numDDoors++;
                        break;
                    }
                }
            }

            Main.statusText = Lang.gen[58].Value + " 70%";
            int num13 = 0;
            int num14 = 1000;
            int num15 = 0;
            int num16 = Main.maxTilesX / 100;
            if (WorldGen.getGoodWorldGen)
                num16 *= 3;

            while (num15 < num16)
            {
                num13++;
                int num17 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num18 = WorldGen.genRand.Next((int)Main.worldSurface + 25, GenVars.dMaxY);
                if (WorldGen.drunkWorldGen)
                    num18 = WorldGen.genRand.Next(GenVars.dungeonY + 25, GenVars.dMaxY);

                int num19 = num17;
                if (Main.tile[num17, num18].WallType == BrickwallType && !Main.tile[num17, num18].HasTile)
                {
                    int num20 = 1;
                    if (WorldGen.genRand.NextBool(2))
                        num20 = -1;

                    for (; !Main.tile[num17, num18].HasTile; num18 += num20)
                    {
                    }

                    if (Main.tile[num17 - 1, num18].HasTile && Main.tile[num17 + 1, num18].HasTile && Main.tile[num17 - 1, num18].TileType != GenVars.crackedType && !Main.tile[num17 - 1, num18 - num20].HasTile && !Main.tile[num17 + 1, num18 - num20].HasTile)
                    {
                        num15++;
                        int num21 = WorldGen.genRand.Next(5, 13);
                        while (Main.tile[num17 - 1, num18].HasTile && Main.tile[num17 - 1, num18].TileType != GenVars.crackedType && Main.tile[num17, num18 + num20].HasTile && Main.tile[num17, num18].HasTile && !Main.tile[num17, num18 - num20].HasTile && num21 > 0)
                        {
                            Main.tile[num17, num18].TileType = 48;
                            if (!Main.tile[num17 - 1, num18 - num20].HasTile && !Main.tile[num17 + 1, num18 - num20].HasTile)
                            {
                                Main.tile[num17, num18 - num20].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20].ResetToType(48);
                                Main.tile[num17, num18 - num20 * 2].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20 * 2].ResetToType(48);
                            }

                            num17--;
                            num21--;
                        }

                        num21 = WorldGen.genRand.Next(5, 13);
                        num17 = num19 + 1;
                        while (Main.tile[num17 + 1, num18].HasTile && Main.tile[num17 + 1, num18].TileType != GenVars.crackedType && Main.tile[num17, num18 + num20].HasTile && Main.tile[num17, num18].HasTile && !Main.tile[num17, num18 - num20].HasTile && num21 > 0)
                        {
                            Main.tile[num17, num18].TileType = 48;
                            if (!Main.tile[num17 - 1, num18 - num20].HasTile && !Main.tile[num17 + 1, num18 - num20].HasTile)
                            {
                                Main.tile[num17, num18 - num20].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20].ResetToType(48);
                                Main.tile[num17, num18 - num20 * 2].Clear(TileDataType.Slope);
                                Main.tile[num17, num18 - num20 * 2].ResetToType(48);
                            }

                            num17++;
                            num21--;
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            num13 = 0;
            num14 = 1000;
            num15 = 0;
            Main.statusText = Lang.gen[58].Value + " 75%";
            while (num15 < num16)
            {
                num13++;
                int num22 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num23 = WorldGen.genRand.Next((int)Main.worldSurface + 25, GenVars.dMaxY);
                int num24 = num23;
                if (Main.tile[num22, num23].WallType == BrickwallType && !Main.tile[num22, num23].HasTile)
                {
                    int num25 = 1;
                    if (WorldGen.genRand.NextBool(2))
                        num25 = -1;

                    for (; num22 > 5 && num22 < Main.maxTilesX - 5 && !Main.tile[num22, num23].HasTile; num22 += num25)
                    {
                    }

                    if (Main.tile[num22, num23 - 1].HasTile && Main.tile[num22, num23 + 1].HasTile && Main.tile[num22, num23 - 1].TileType != GenVars.crackedType && !Main.tile[num22 - num25, num23 - 1].HasTile && !Main.tile[num22 - num25, num23 + 1].HasTile)
                    {
                        num15++;
                        int num26 = WorldGen.genRand.Next(5, 13);
                        while (Main.tile[num22, num23 - 1].HasTile && Main.tile[num22, num23 - 1].TileType != GenVars.crackedType && Main.tile[num22 + num25, num23].HasTile && Main.tile[num22, num23].HasTile && !Main.tile[num22 - num25, num23].HasTile && num26 > 0)
                        {
                            Main.tile[num22, num23].TileType = 48;
                            if (!Main.tile[num22 - num25, num23 - 1].HasTile && !Main.tile[num22 - num25, num23 + 1].HasTile)
                            {
                                Main.tile[num22 - num25, num23].ResetToType(48);
                                Main.tile[num22 - num25, num23].Clear(TileDataType.Slope);
                                Main.tile[num22 - num25 * 2, num23].ResetToType(48);
                                Main.tile[num22 - num25 * 2, num23].Clear(TileDataType.Slope);
                            }

                            num23--;
                            num26--;
                        }

                        num26 = WorldGen.genRand.Next(5, 13);
                        num23 = num24 + 1;
                        while (Main.tile[num22, num23 + 1].HasTile && Main.tile[num22, num23 + 1].TileType != GenVars.crackedType && Main.tile[num22 + num25, num23].HasTile && Main.tile[num22, num23].HasTile && !Main.tile[num22 - num25, num23].HasTile && num26 > 0)
                        {
                            Main.tile[num22, num23].TileType = 48;
                            if (!Main.tile[num22 - num25, num23 - 1].HasTile && !Main.tile[num22 - num25, num23 + 1].HasTile)
                            {
                                Main.tile[num22 - num25, num23].ResetToType(48);
                                Main.tile[num22 - num25, num23].Clear(TileDataType.Slope);
                                Main.tile[num22 - num25 * 2, num23].ResetToType(48);
                                Main.tile[num22 - num25 * 2, num23].Clear(TileDataType.Slope);
                            }

                            num23++;
                            num26--;
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            Main.statusText = Lang.gen[58].Value + " 80%";
            for (int num27 = 0; num27 < GenVars.numDDoors; num27++)
            {
                int num28 = GenVars.DDoorX[num27] - 10;
                int num29 = GenVars.DDoorX[num27] + 10;
                int num30 = 100;
                int num31 = 0;
                int num32 = 0;
                int num33 = 0;
                for (int num34 = num28; num34 < num29; num34++)
                {
                    bool flag = true;
                    int num35 = GenVars.DDoorY[num27];
                    while (num35 > 10 && !Main.tile[num34, num35].HasTile)
                    {
                        num35--;
                    }

                    if (!Main.tileDungeon[Main.tile[num34, num35].TileType])
                        flag = false;

                    num32 = num35;
                    for (num35 = GenVars.DDoorY[num27]; !Main.tile[num34, num35].HasTile; num35++)
                    {
                    }

                    if (!Main.tileDungeon[Main.tile[num34, num35].TileType])
                        flag = false;

                    num33 = num35;
                    if (num33 - num32 < 3)
                        continue;

                    int num36 = num34 - 20;
                    int num37 = num34 + 20;
                    int num38 = num33 - 10;
                    int num39 = num33 + 10;
                    for (int num40 = num36; num40 < num37; num40++)
                    {
                        for (int num41 = num38; num41 < num39; num41++)
                        {
                            if (Main.tile[num40, num41].HasTile && Main.tile[num40, num41].TileType == 10)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }

                    if (flag)
                    {
                        for (int num42 = num33 - 3; num42 < num33; num42++)
                        {
                            for (int num43 = num34 - 3; num43 <= num34 + 3; num43++)
                            {
                                if (Main.tile[num43, num42].HasTile)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (flag && num33 - num32 < 20)
                    {
                        bool flag2 = false;
                        if (GenVars.DDoorPos[num27] == 0 && num33 - num32 < num30)
                            flag2 = true;

                        if (GenVars.DDoorPos[num27] == -1 && num34 > num31)
                            flag2 = true;

                        if (GenVars.DDoorPos[num27] == 1 && (num34 < num31 || num31 == 0))
                            flag2 = true;

                        if (flag2)
                        {
                            num31 = num34;
                            num30 = num33 - num32;
                        }
                    }
                }

                if (num30 >= 20)
                    continue;

                int num44 = num31;
                int num45 = GenVars.DDoorY[num27];
                int num46 = num45;
                for (; !Main.tile[num44, num45].HasTile; num45++) { }

                while (!Main.tile[num44, num46].HasTile)
                {
                    num46--;
                }

                num45--;
                num46++;
                for (int num47 = num46; num47 < num45 - 2; num47++)
                {
                    Main.tile[num44, num47].Clear(TileDataType.Slope);
                    Main.tile[num44, num47].ResetToType(brickTileType);
                    if (Main.tile[num44 - 1, num47].TileType == brickTileType)
                    {
                        Main.tile[num44 - 1, num47].ClearTile();
                        Main.tile[num44 - 1, num47].ClearEverything();
                        Main.tile[num44 - 1, num47].WallType = (ushort)BrickwallType;
                    }

                    if (Main.tile[num44 - 2, num47].TileType == brickTileType)
                    {
                        Main.tile[num44 - 2, num47].ClearTile();
                        Main.tile[num44 - 2, num47].ClearEverything();
                        Main.tile[num44 - 2, num47].WallType = (ushort)BrickwallType;
                    }

                    if (Main.tile[num44 + 1, num47].TileType == brickTileType)
                    {
                        Main.tile[num44 + 1, num47].ClearTile();
                        Main.tile[num44 + 1, num47].ClearEverything();
                        Main.tile[num44 + 1, num47].WallType = (ushort)BrickwallType;
                    }

                    if (Main.tile[num44 + 2, num47].TileType == brickTileType)
                    {
                        Main.tile[num44 + 2, num47].ClearTile();
                        Main.tile[num44 + 2, num47].ClearEverything();
                        Main.tile[num44 + 2, num47].WallType = (ushort)BrickwallType;
                    }
                }

                int style = 13;
                if (WorldGen.genRand.NextBool(3))
                {
                    switch (BrickwallType)
                    {
                        case 7:
                            style = 16;
                            break;
                        case 8:
                            style = 17;
                            break;
                        case 9:
                            style = 18;
                            break;
                    }
                }

                WorldGen.PlaceTile(num44, num45, 10, mute: true, forced: false, -1, style);
                num44--;
                int num48 = num45 - 3;
                while (!Main.tile[num44, num48].HasTile)
                {
                    num48--;
                }

                if (num45 - num48 < num45 - num46 + 5 && Main.tileDungeon[Main.tile[num44, num48].TileType])
                {
                    for (int num49 = num45 - 4 - WorldGen.genRand.Next(3); num49 > num48; num49--)
                    {
                        Main.tile[num44, num49].Clear(TileDataType.Slope);
                        Main.tile[num44, num49].ResetToType(brickTileType);
                        if (Main.tile[num44 - 1, num49].TileType == brickTileType)
                        {
                            Main.tile[num44 - 1, num49].ClearTile();
                            Main.tile[num44 - 1, num49].ClearEverything();
                            Main.tile[num44 - 1, num49].WallType = (ushort)BrickwallType;
                        }

                        if (Main.tile[num44 - 2, num49].TileType == brickTileType)
                        {
                            Main.tile[num44 - 2, num49].ClearTile();
                            Main.tile[num44 - 2, num49].ClearEverything();
                            Main.tile[num44 - 2, num49].WallType = (ushort)BrickwallType;
                        }
                    }
                }

                num44 += 2;
                num48 = num45 - 3;
                while (!Main.tile[num44, num48].HasTile)
                {
                    num48--;
                }

                if (num45 - num48 < num45 - num46 + 5 && Main.tileDungeon[Main.tile[num44, num48].TileType])
                {
                    for (int num50 = num45 - 4 - WorldGen.genRand.Next(3); num50 > num48; num50--)
                    {
                        Main.tile[num44, num50].Clear(TileDataType.Slope);
                        Main.tile[num44, num50].ResetToType(brickTileType);
                        if (Main.tile[num44 + 1, num50].TileType == brickTileType)
                        {
                            Main.tile[num44 + 1, num50].ClearTile();
                            Main.tile[num44 + 1, num50].ClearEverything();
                            Main.tile[num44 + 1, num50].WallType = (ushort)BrickwallType;
                        }

                        if (Main.tile[num44 + 2, num50].TileType == brickTileType)
                        {
                            Main.tile[num44 + 2, num50].ClearTile();
                            Main.tile[num44 + 2, num50].ClearEverything();
                            Main.tile[num44 + 2, num50].WallType = (ushort)BrickwallType;
                        }
                    }
                }

                num45++;
                num44--;
                for (int num51 = num45 - 8; num51 < num45; num51++)
                {
                    if (Main.tile[num44 + 2, num51].TileType == brickTileType)
                    {
                        Main.tile[num44 + 2, num51].ClearTile();
                        Main.tile[num44 + 2, num51].ClearEverything();
                        Main.tile[num44 + 2, num51].WallType = (ushort)BrickwallType;
                    }

                    if (Main.tile[num44 + 3, num51].TileType == brickTileType)
                    {
                        Main.tile[num44 + 3, num51].ClearTile();
                        Main.tile[num44 + 3, num51].ClearEverything();
                        Main.tile[num44 + 3, num51].WallType = (ushort)BrickwallType;
                    }

                    if (Main.tile[num44 - 2, num51].TileType == brickTileType)
                    {
                        Main.tile[num44 - 2, num51].ClearTile();
                        Main.tile[num44 - 2, num51].ClearEverything();
                        Main.tile[num44 - 2, num51].WallType = (ushort)BrickwallType;
                    }

                    if (Main.tile[num44 - 3, num51].TileType == brickTileType)
                    {
                        Main.tile[num44 - 3, num51].ClearTile();
                        Main.tile[num44 - 3, num51].ClearEverything();
                        Main.tile[num44 - 3, num51].WallType = (ushort)BrickwallType;
                    }
                }

                Main.tile[num44 - 1, num45].ResetToType(brickTileType);
                Main.tile[num44 - 1, num45].Clear(TileDataType.Slope);
                Main.tile[num44 + 1, num45].ResetToType(brickTileType);
                Main.tile[num44 + 1, num45].Clear(TileDataType.Slope);
            }

            int[] array = new int[3];
            switch (BrickwallType)
            {
                case 7:
                    array[0] = 7;
                    array[1] = 94;
                    array[2] = 95;
                    break;
                case 9:
                    array[0] = 9;
                    array[1] = 96;
                    array[2] = 97;
                    break;
                default:
                    array[0] = 8;
                    array[1] = 98;
                    array[2] = 99;
                    break;
            }

            for (int num52 = 0; num52 < 5; num52++)
            {
                for (int num53 = 0; num53 < 3; num53++)
                {
                    int num54 = WorldGen.genRand.Next(40, 240);
                    int num55 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    int num56 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                    for (int num57 = num55 - num54; num57 < num55 + num54; num57++)
                    {
                        for (int num58 = num56 - num54; num58 < num56 + num54; num58++)
                        {
                            if ((double)num58 > Main.worldSurface)
                            {
                                double num59 = Math.Abs(num55 - num57);
                                double num60 = Math.Abs(num56 - num58);
                                if (Math.Sqrt(num59 * num59 + num60 * num60) < (double)num54 * 0.4 && Main.wallDungeon[Main.tile[num57, num58].WallType])
                                    WorldGen.Spread.WallDungeon(num57, num58, array[num53]);
                            }
                        }
                    }
                }
            }

            Main.statusText = Lang.gen[58].Value + " 85%";
            for (int num61 = 0; num61 < GenVars.numDungeonPlatforms; num61++)
            {
                int num62 = GenVars.dungeonPlatformX[num61];
                int num63 = GenVars.dungeonPlatformY[num61];
                int num64 = Main.maxTilesX;
                int num65 = 10;
                if ((double)num63 < Main.worldSurface + 50.0)
                    num65 = 20;

                for (int num66 = num63 - 5; num66 <= num63 + 5; num66++)
                {
                    int num67 = num62;
                    int num68 = num62;
                    bool flag3 = false;
                    if (Main.tile[num67, num66].HasTile)
                    {
                        flag3 = true;
                    }
                    else
                    {
                        while (!Main.tile[num67, num66].HasTile)
                        {
                            num67--;
                            if (!Main.tileDungeon[Main.tile[num67, num66].TileType] || num67 == 0)
                            {
                                flag3 = true;
                                break;
                            }
                        }

                        while (!Main.tile[num68, num66].HasTile)
                        {
                            num68++;
                            if (!Main.tileDungeon[Main.tile[num68, num66].TileType] || num68 == Main.maxTilesX - 1)
                            {
                                flag3 = true;
                                break;
                            }
                        }
                    }

                    if (flag3 || num68 - num67 > num65)
                        continue;

                    bool flag4 = true;
                    int num69 = num62 - num65 / 2 - 2;
                    int num70 = num62 + num65 / 2 + 2;
                    int num71 = num66 - 5;
                    int num72 = num66 + 5;
                    for (int num73 = num69; num73 <= num70; num73++)
                    {
                        for (int num74 = num71; num74 <= num72; num74++)
                        {
                            if (Main.tile[num73, num74].HasTile && Main.tile[num73, num74].TileType == 19)
                            {
                                flag4 = false;
                                break;
                            }
                        }
                    }

                    for (int num75 = num66 + 3; num75 >= num66 - 5; num75--)
                    {
                        if (Main.tile[num62, num75].HasTile)
                        {
                            flag4 = false;
                            break;
                        }
                    }

                    if (flag4)
                    {
                        num64 = num66;
                        break;
                    }
                }

                if (num64 <= num63 - 10 || num64 >= num63 + 10)
                    continue;

                int num76 = num62;
                int num77 = num64;
                int num78 = num62 + 1;
                while (!Main.tile[num76, num77].HasTile)
                {
                    Main.tile[num76, num77].ResetToType(19);
                    Main.tile[num76, num77].Clear(TileDataType.Slope);
                    switch (BrickwallType)
                    {
                        case 7:
                            Main.tile[num76, num77].TileFrameY = 108;
                            break;
                        case 8:
                            Main.tile[num76, num77].TileFrameY = 144;
                            break;
                        default:
                            Main.tile[num76, num77].TileFrameY = 126;
                            break;
                    }

                    WorldGen.TileFrame(num76, num77);
                    num76--;
                }

                for (; !Main.tile[num78, num77].HasTile; num78++)
                {
                    Main.tile[num78, num77].ResetToType(19);
                    Main.tile[num78, num77].Clear(TileDataType.Slope);
                    switch (BrickwallType)
                    {
                        case 7:
                            Main.tile[num78, num77].TileFrameY = 108;
                            break;
                        case 8:
                            Main.tile[num78, num77].TileFrameY = 144;
                            break;
                        default:
                            Main.tile[num78, num77].TileFrameY = 126;
                            break;
                    }

                    WorldGen.TileFrame(num78, num77);
                }
            }

            int num79 = 5;
            if (WorldGen.drunkWorldGen)
                num79 = 6;

            for (int i1 = 0; i1 < num79; i1++) //地牢宝箱生成
            {
                bool flag5 = false;
                while (!flag5)
                {
                    int num81 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    int num82 = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.dMaxY);
                    if (!Main.wallDungeon[Main.tile[num81, num82].WallType] || Main.tile[num81, num82].HasTile)
                        continue;

                    ushort chestTileType = 21;
                    int contain = 0;
                    int style2 = 0;
                    switch (i1)
                    {
                        case 0:
                            style2 = 23;
                            contain = 1156;
                            break;
                        case 1:
                            if (!WorldGen.crimson)
                            {
                                style2 = 24;
                                contain = 1571;
                            }
                            else
                            {
                                style2 = 25;
                                contain = 1569;
                            }
                            break;
                        case 5:
                            if (WorldGen.crimson)
                            {
                                style2 = 24;
                                contain = 1571;
                            }
                            else
                            {
                                style2 = 25;
                                contain = 1569;
                            }
                            break;
                        case 2:
                            style2 = 26;
                            contain = 1260;
                            break;
                        case 3:
                            style2 = 27;
                            contain = 1572;
                            break;
                        case 4:
                            chestTileType = 467;
                            style2 = 13;
                            contain = 4607;
                            break;
                    }

                    flag5 = WorldGen.AddBuriedChest(num81, num82, contain, notNearOtherChests: false, style2, trySlope: false, chestTileType);
                }
            }

            int[] array2 = new int[3] {
            WorldGen.genRand.Next(9, 13),
            WorldGen.genRand.Next(9, 13),
            0
        };

            while (array2[1] == array2[0])
            {
                array2[1] = WorldGen.genRand.Next(9, 13);
            }

            array2[2] = WorldGen.genRand.Next(9, 13);
            while (array2[2] == array2[0] || array2[2] == array2[1])
            {
                array2[2] = WorldGen.genRand.Next(9, 13);
            }

            Main.statusText = Lang.gen[58].Value + " 90%";
            num13 = 0;
            num14 = 1000;
            num15 = 0;
            while (num15 < Main.maxTilesX / 20)
            {
                num13++;
                int num83 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num84 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                bool flag6 = true;
                if (Main.wallDungeon[Main.tile[num83, num84].WallType] && !Main.tile[num83, num84].HasTile)
                {
                    int num85 = 1;
                    if (WorldGen.genRand.NextBool(2))
                        num85 = -1;

                    while (flag6 && !Main.tile[num83, num84].HasTile)
                    {
                        num83 -= num85;
                        if (num83 < 5 || num83 > Main.maxTilesX - 5)
                            flag6 = false;
                        else if (Main.tile[num83, num84].HasTile && !Main.tileDungeon[Main.tile[num83, num84].TileType])
                            flag6 = false;
                    }

                    if (flag6 && Main.tile[num83, num84].HasTile && Main.tileDungeon[Main.tile[num83, num84].TileType] && Main.tile[num83, num84 - 1].HasTile && Main.tileDungeon[Main.tile[num83, num84 - 1].TileType] && Main.tile[num83, num84 + 1].HasTile && Main.tileDungeon[Main.tile[num83, num84 + 1].TileType])
                    {
                        num83 += num85;
                        for (int num86 = num83 - 3; num86 <= num83 + 3; num86++)
                        {
                            for (int num87 = num84 - 3; num87 <= num84 + 3; num87++)
                            {
                                if (Main.tile[num86, num87].HasTile && Main.tile[num86, num87].TileType == 19)
                                {
                                    flag6 = false;
                                    break;
                                }
                            }
                        }

                        if (flag6 && (!Main.tile[num83, num84 - 1].HasTile & !Main.tile[num83, num84 - 2].HasTile & !Main.tile[num83, num84 - 3].HasTile))
                        {
                            int num88 = num83;
                            int num89 = num83;
                            for (; num88 > GenVars.dMinX && num88 < GenVars.dMaxX && !Main.tile[num88, num84].HasTile && !Main.tile[num88, num84 - 1].HasTile && !Main.tile[num88, num84 + 1].HasTile; num88 += num85)
                            {
                            }

                            num88 = Math.Abs(num83 - num88);
                            bool flag7 = false;
                            if (WorldGen.genRand.NextBool(2))
                                flag7 = true;

                            if (num88 > 5)
                            {
                                for (int num90 = WorldGen.genRand.Next(1, 4); num90 > 0; num90--)
                                {
                                    Main.tile[num83, num84].Clear(TileDataType.Slope);
                                    Main.tile[num83, num84].ResetToType(19);
                                    if (Main.tile[num83, num84].WallType == array[0])
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[0]);
                                    else if (Main.tile[num83, num84].WallType == array[1])
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[1]);
                                    else
                                        Main.tile[num83, num84].TileFrameY = (short)(18 * array2[2]);

                                    WorldGen.TileFrame(num83, num84);
                                    if (flag7)
                                    {
                                        WorldGen.PlaceTile(num83, num84 - 1, 50, mute: true);
                                        if (WorldGen.genRand.NextBool(50) && (double)num84 > (Main.worldSurface + Main.rockLayer) / 2.0 && Main.tile[num83, num84 - 1].TileType == 50)
                                            Main.tile[num83, num84 - 1].TileFrameX = 90;
                                    }

                                    num83 += num85;
                                }

                                num13 = 0;
                                num15++;
                                if (!flag7 && WorldGen.genRand.NextBool(2))
                                {
                                    num83 = num89;
                                    num84--;
                                    int num91 = 0;
                                    if (WorldGen.genRand.NextBool(4))
                                        num91 = 1;

                                    switch (num91)
                                    {
                                        case 0:
                                            num91 = 13;
                                            break;
                                        case 1:
                                            num91 = 49;
                                            break;
                                    }

                                    WorldGen.PlaceTile(num83, num84, num91, mute: true);
                                    if (Main.tile[num83, num84].TileType == 13)
                                    {
                                        if (WorldGen.genRand.NextBool(2))
                                            Main.tile[num83, num84].TileFrameX = 18;
                                        else
                                            Main.tile[num83, num84].TileFrameX = 36;
                                    }
                                }
                            }
                        }
                    }
                }

                if (num13 > num14)
                {
                    num13 = 0;
                    num15++;
                }
            }

            Main.statusText = Lang.gen[58].Value + " 95%";
            int num92 = 1;
            for (int num93 = 0; num93 < GenVars.numDRooms; num93++)
            {
                int num94 = 0;
                while (num94 < 1000)
                {
                    int num95 = (int)((double)GenVars.dRoomSize[num93] * 0.4);
                    int i3 = GenVars.dRoomX[num93] + WorldGen.genRand.Next(-num95, num95 + 1);
                    int num96 = GenVars.dRoomY[num93] + WorldGen.genRand.Next(-num95, num95 + 1);
                    int num97 = 0;
                    int style3 = 2;
                    if (num92 == 1)
                        num92++;

                    switch (num92)
                    {
                        case 2:
                            num97 = 155;
                            break;
                        case 3:
                            num97 = 156;
                            break;
                        case 4:
                            num97 = ((!WorldGen.remixWorldGen) ? 157 : 2623);
                            break;
                        case 5:
                            num97 = 163;
                            break;
                        case 6:
                            num97 = 113;
                            break;
                        case 7:
                            num97 = 3317;
                            break;
                        case 8:
                            num97 = 327;
                            style3 = 0;
                            break;
                        default:
                            num97 = 164;
                            num92 = 0;
                            break;
                    }

                    if ((double)num96 < Main.worldSurface + 50.0)
                    {
                        num97 = 327;
                        style3 = 0;
                    }

                    if (num97 == 0 && WorldGen.genRand.NextBool(2))
                    {
                        num94 = 1000;
                        continue;
                    }

                    if (WorldGen.AddBuriedChest(i3, num96, num97, notNearOtherChests: false, style3, trySlope: false, 0))
                    {
                        num94 += 1000;
                        num92++;
                    }

                    num94++;
                }
            }

            GenVars.dMinX -= 25;
            GenVars.dMaxX += 25;
            GenVars.dMinY -= 25;
            GenVars.dMaxY += 25;
            if (GenVars.dMinX < 0)
                GenVars.dMinX = 0;

            if (GenVars.dMaxX > Main.maxTilesX)
                GenVars.dMaxX = Main.maxTilesX;

            if (GenVars.dMinY < 0)
                GenVars.dMinY = 0;

            if (GenVars.dMaxY > Main.maxTilesY)
                GenVars.dMaxY = Main.maxTilesY;

            num13 = 0;
            num14 = 1000;
            num15 = 0;
            MakeDungeon_Lights(brickTileType, ref num13, num14, ref num15, array);
            num13 = 0;
            num14 = 1000;
            num15 = 0;
            MakeDungeon_Traps(ref num13, num14, ref num15);
            double count = MakeDungeon_GroundFurniture(BrickwallType);
            count = MakeDungeon_Pictures(array, count);
            count = MakeDungeon_Banners(array, count);
        }

        private static void MakeDungeon_Traps(ref int failCount, int failMax, ref int numAdd)
        {
            while (numAdd < Main.maxTilesX / 500)
            {
                failCount++;
                int num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                while (num2 < Main.worldSurface)
                {
                    num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                }

                if (Main.wallDungeon[Main.tile[num, num2].WallType] && WorldGen.placeTrap(num, num2, 0))
                    failCount = failMax;

                if (failCount > failMax)
                {
                    numAdd++;
                    failCount = 0;
                }
            }
        }

        private static void MakeDungeon_Lights(ushort tileType, ref int failCount, int failMax, ref int numAdd, int[] roomWall)
        {
            int[] array = new int[3] {
            WorldGen.genRand.Next(7),
            WorldGen.genRand.Next(7),
            0
        };

            while (array[1] == array[0])
            {
                array[1] = WorldGen.genRand.Next(7);
            }

            array[2] = WorldGen.genRand.Next(7);
            while (array[2] == array[0] || array[2] == array[1])
            {
                array[2] = WorldGen.genRand.Next(7);
            }

            while (numAdd < Main.maxTilesX / 150)
            {
                failCount++;
                int num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                if (Main.wallDungeon[Main.tile[num, num2].WallType])
                {
                    for (int num3 = num2; num3 > GenVars.dMinY; num3--)
                    {
                        if (Main.tile[num, num3 - 1].HasTile && Main.tile[num, num3 - 1].TileType == tileType)
                        {
                            bool flag = false;
                            for (int i = num - 15; i < num + 15; i++)
                            {
                                for (int j = num3 - 15; j < num3 + 15; j++)
                                {
                                    if (i > 0 && i < Main.maxTilesX && j > 0 && j < Main.maxTilesY && (Main.tile[i, j].TileType == 42 || Main.tile[i, j].TileType == 34))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                            }

                            if (Main.tile[num - 1, num3].HasTile || Main.tile[num + 1, num3].HasTile || Main.tile[num - 1, num3 + 1].HasTile || Main.tile[num + 1, num3 + 1].HasTile || Main.tile[num, num3 + 2].HasTile)
                                flag = true;

                            if (flag)
                                break;

                            bool flag2 = false;
                            if (!flag2 && WorldGen.genRand.NextBool(7))
                            {
                                int style = 27;
                                switch (roomWall[0])
                                {
                                    case 7:
                                        style = 27;
                                        break;
                                    case 8:
                                        style = 28;
                                        break;
                                    case 9:
                                        style = 29;
                                        break;
                                }

                                bool flag3 = false;
                                for (int k = 0; k < 15; k++)
                                {
                                    if (WorldGen.SolidTile(num, num3 + k))
                                    {
                                        flag3 = true;
                                        break;
                                    }
                                }

                                if (!flag3)
                                    WorldGen.PlaceChand(num, num3, 34, style);

                                if (Main.tile[num, num3].TileType == 34)
                                {
                                    flag2 = true;
                                    failCount = 0;
                                    numAdd++;
                                    for (int l = 0; l < 1000; l++)
                                    {
                                        int num4 = num + WorldGen.genRand.Next(-12, 13);
                                        int num5 = num3 + WorldGen.genRand.Next(3, 21);
                                        if (Main.tile[num4, num5].HasTile || Main.tile[num4, num5 + 1].HasTile || !Main.tileDungeon[Main.tile[num4 - 1, num5].TileType] || !Main.tileDungeon[Main.tile[num4 + 1, num5].TileType] || !Collision.CanHit(new Point(num4 * 16, num5 * 16), 16, 16, new Point(num * 16, num3 * 16 + 1), 16, 16))
                                            continue;

                                        if (((WorldGen.SolidTile(num4 - 1, num5) && Main.tile[num4 - 1, num5].TileType != 10) || (WorldGen.SolidTile(num4 + 1, num5) && Main.tile[num4 + 1, num5].TileType != 10) || WorldGen.SolidTile(num4, num5 + 1)) && Main.wallDungeon[Main.tile[num4, num5].WallType] && (Main.tileDungeon[Main.tile[num4 - 1, num5].TileType] || Main.tileDungeon[Main.tile[num4 + 1, num5].TileType]))
                                            WorldGen.PlaceTile(num4, num5, 136, mute: true);

                                        if (!Main.tile[num4, num5].HasTile)
                                            continue;

                                        while (num4 != num || num5 != num3)
                                        {
                                            WorldGen.PlaceWire(num4, num5);
                                            if (num4 > num)
                                                num4--;

                                            if (num4 < num)
                                                num4++;

                                            WorldGen.PlaceWire(num4, num5);
                                            if (num5 > num3)
                                                num5--;

                                            if (num5 < num3)
                                                num5++;

                                            WorldGen.PlaceWire(num4, num5);
                                        }

                                        if (WorldGen.genRand.Next(3) > 0)
                                        {
                                            Main.tile[num, num3].TileFrameX = 18;
                                            Main.tile[num, num3 + 1].TileFrameX = 18;
                                        }

                                        break;
                                    }
                                }
                            }

                            if (flag2)
                                break;

                            int style2 = array[0];
                            if (Main.tile[num, num3].WallType == roomWall[1])
                                style2 = array[1];

                            if (Main.tile[num, num3].WallType == roomWall[2])
                                style2 = array[2];

                            WorldGen.Place1x2Top(num, num3, 42, style2);
                            if (Main.tile[num, num3].TileType != 42)
                                break;

                            flag2 = true;
                            failCount = 0;
                            numAdd++;
                            for (int m = 0; m < 1000; m++)
                            {
                                int num6 = num + WorldGen.genRand.Next(-12, 13);
                                int num7 = num3 + WorldGen.genRand.Next(3, 21);
                                if (Main.tile[num6, num7].HasTile || Main.tile[num6, num7 + 1].HasTile || Main.tile[num6 - 1, num7].TileType == 48 || Main.tile[num6 + 1, num7].TileType == 48 || !Collision.CanHit(new Point(num6 * 16, num7 * 16), 16, 16, new Point(num * 16, num3 * 16 + 1), 16, 16))
                                    continue;

                                if ((WorldGen.SolidTile(num6 - 1, num7) && Main.tile[num6 - 1, num7].TileType != 10) || (WorldGen.SolidTile(num6 + 1, num7) && Main.tile[num6 + 1, num7].TileType != 10) || WorldGen.SolidTile(num6, num7 + 1))
                                    WorldGen.PlaceTile(num6, num7, 136, mute: true);

                                if (!Main.tile[num6, num7].HasTile)
                                    continue;

                                while (num6 != num || num7 != num3)
                                {
                                    WorldGen.PlaceWire(num6, num7);
                                    if (num6 > num)
                                        num6--;

                                    if (num6 < num)
                                        num6++;

                                    WorldGen.PlaceWire(num6, num7);
                                    if (num7 > num3)
                                        num7--;

                                    if (num7 < num3)
                                        num7++;

                                    WorldGen.PlaceWire(num6, num7);
                                }

                                if (WorldGen.genRand.Next(3) > 0)
                                {
                                    Main.tile[num, num3].TileFrameX = 18;
                                    Main.tile[num, num3 + 1].TileFrameX = 18;
                                }

                                break;
                            }

                            break;
                        }
                    }
                }

                if (failCount > failMax)
                {
                    numAdd++;
                    failCount = 0;
                }
            }
        }

        private static double MakeDungeon_Banners(int[] roomWall, double count)
        {
            count = 840000.0 / (double)Main.maxTilesX;
            for (int i = 0; (double)i < count; i++)
            {
                int num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                while (!Main.wallDungeon[Main.tile[num, num2].WallType] || Main.tile[num, num2].HasTile)
                {
                    num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    num2 = WorldGen.genRand.Next(GenVars.dMinY, GenVars.dMaxY);
                }

                while (!WorldGen.SolidTile(num, num2) && num2 > 10)
                {
                    num2--;
                }

                num2++;
                if (!Main.wallDungeon[Main.tile[num, num2].WallType] || Main.tile[num, num2 - 1].TileType == 48 || Main.tile[num, num2].HasTile || Main.tile[num, num2 + 1].HasTile || Main.tile[num, num2 + 2].HasTile || Main.tile[num, num2 + 3].HasTile)
                    continue;

                bool flag = true;
                for (int j = num - 1; j <= num + 1; j++)
                {
                    for (int k = num2; k <= num2 + 3; k++)
                    {
                        if (Main.tile[j, k].HasTile && (Main.tile[j, k].TileType == 10 || Main.tile[j, k].TileType == 11 || Main.tile[j, k].TileType == 91))
                            flag = false;
                    }
                }

                if (flag)
                {
                    int num3 = 10;
                    if (Main.tile[num, num2].WallType == roomWall[1])
                        num3 = 12;

                    if (Main.tile[num, num2].WallType == roomWall[2])
                        num3 = 14;

                    num3 += WorldGen.genRand.Next(2);
                    WorldGen.PlaceTile(num, num2, 91, mute: true, forced: false, -1, num3);
                }
            }

            return count;
        }

        private static double MakeDungeon_Pictures(int[] roomWall, double count)
        {
            count = 420000.0 / (double)Main.maxTilesX;
            for (int i = 0; (double)i < count; i++)
            {
                int num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int num2 = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.dMaxY);
                while (!Main.wallDungeon[Main.tile[num, num2].WallType] || Main.tile[num, num2].HasTile)
                {
                    num = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    num2 = WorldGen.genRand.Next((int)Main.worldSurface, GenVars.dMaxY);
                }

                int num3 = num;
                int num4 = num;
                int num5 = num2;
                int num6 = num2;
                int num7 = 0;
                int num8 = 0;
                for (int j = 0; j < 2; j++)
                {
                    num3 = num;
                    num4 = num;
                    while (!Main.tile[num3, num2].HasTile && Main.wallDungeon[Main.tile[num3, num2].WallType])
                    {
                        num3--;
                    }

                    num3++;
                    for (; !Main.tile[num4, num2].HasTile && Main.wallDungeon[Main.tile[num4, num2].WallType]; num4++)
                    {
                    }

                    num4--;
                    num = (num3 + num4) / 2;
                    num5 = num2;
                    num6 = num2;
                    while (!Main.tile[num, num5].HasTile && Main.wallDungeon[Main.tile[num, num5].WallType])
                    {
                        num5--;
                    }

                    num5++;
                    for (; !Main.tile[num, num6].HasTile && Main.wallDungeon[Main.tile[num, num6].WallType]; num6++)
                    {
                    }

                    num6--;
                    num2 = (num5 + num6) / 2;
                }

                num3 = num;
                num4 = num;
                while (!Main.tile[num3, num2].HasTile && !Main.tile[num3, num2 - 1].HasTile && !Main.tile[num3, num2 + 1].HasTile)
                {
                    num3--;
                }

                num3++;
                for (; !Main.tile[num4, num2].HasTile && !Main.tile[num4, num2 - 1].HasTile && !Main.tile[num4, num2 + 1].HasTile; num4++)
                {
                }

                num4--;
                num5 = num2;
                num6 = num2;
                while (!Main.tile[num, num5].HasTile && !Main.tile[num - 1, num5].HasTile && !Main.tile[num + 1, num5].HasTile)
                {
                    num5--;
                }

                num5++;
                for (; !Main.tile[num, num6].HasTile && !Main.tile[num - 1, num6].HasTile && !Main.tile[num + 1, num6].HasTile; num6++)
                {
                }

                num6--;
                num = (num3 + num4) / 2;
                num2 = (num5 + num6) / 2;
                num7 = num4 - num3;
                num8 = num6 - num5;
                if (num7 <= 7 || num8 <= 5)
                    continue;

                bool[] array = new bool[3] {
                true,
                false,
                false
            };

                if (num7 > num8 * 3 && num7 > 21)
                    array[1] = true;

                if (num8 > num7 * 3 && num8 > 21)
                    array[2] = true;

                int num9 = WorldGen.genRand.Next(3);
                if (Main.tile[num, num2].WallType == roomWall[0])
                    num9 = 0;

                while (!array[num9])
                {
                    num9 = WorldGen.genRand.Next(3);
                }

                if (WorldGen.nearPicture2(num, num2))
                    num9 = -1;

                switch (num9)
                {
                    case 0:
                        {
                            PaintingEntry paintingEntry2 = WorldGen.RandPictureTile();
                            if (Main.tile[num, num2].WallType != roomWall[0])
                                paintingEntry2 = RandBonePicture();

                            if (!WorldGen.nearPicture(num, num2))
                                WorldGen.PlaceTile(num, num2, paintingEntry2.tileType, mute: true, forced: false, -1, paintingEntry2.style);

                            break;
                        }
                    case 1:
                        {
                            PaintingEntry paintingEntry3 = WorldGen.RandPictureTile();
                            if (Main.tile[num, num2].WallType != roomWall[0])
                                paintingEntry3 = RandBonePicture();

                            if (!Main.tile[num, num2].HasTile)
                                WorldGen.PlaceTile(num, num2, paintingEntry3.tileType, mute: true, forced: false, -1, paintingEntry3.style);

                            int num13 = num;
                            int num14 = num2;
                            int num15 = num2;
                            for (int m = 0; m < 2; m++)
                            {
                                num += 7;
                                num5 = num15;
                                num6 = num15;
                                while (!Main.tile[num, num5].HasTile && !Main.tile[num - 1, num5].HasTile && !Main.tile[num + 1, num5].HasTile)
                                {
                                    num5--;
                                }

                                num5++;
                                for (; !Main.tile[num, num6].HasTile && !Main.tile[num - 1, num6].HasTile && !Main.tile[num + 1, num6].HasTile; num6++)
                                {
                                }

                                num6--;
                                num15 = (num5 + num6) / 2;
                                paintingEntry3 = WorldGen.RandPictureTile();
                                if (Main.tile[num, num15].WallType != roomWall[0])
                                    paintingEntry3 = RandBonePicture();

                                if (Math.Abs(num14 - num15) >= 4 || WorldGen.nearPicture(num, num15))
                                    break;

                                WorldGen.PlaceTile(num, num15, paintingEntry3.tileType, mute: true, forced: false, -1, paintingEntry3.style);
                            }

                            num15 = num2;
                            num = num13;
                            for (int n = 0; n < 2; n++)
                            {
                                num -= 7;
                                num5 = num15;
                                num6 = num15;
                                while (!Main.tile[num, num5].HasTile && !Main.tile[num - 1, num5].HasTile && !Main.tile[num + 1, num5].HasTile)
                                {
                                    num5--;
                                }

                                num5++;
                                for (; !Main.tile[num, num6].HasTile && !Main.tile[num - 1, num6].HasTile && !Main.tile[num + 1, num6].HasTile; num6++)
                                {
                                }

                                num6--;
                                num15 = (num5 + num6) / 2;
                                paintingEntry3 = WorldGen.RandPictureTile();
                                if (Main.tile[num, num15].WallType != roomWall[0])
                                    paintingEntry3 = RandBonePicture();

                                if (Math.Abs(num14 - num15) >= 4 || WorldGen.nearPicture(num, num15))
                                    break;

                                WorldGen.PlaceTile(num, num15, paintingEntry3.tileType, mute: true, forced: false, -1, paintingEntry3.style);
                            }

                            break;
                        }
                    case 2:
                        {
                            PaintingEntry paintingEntry = WorldGen.RandPictureTile();
                            if (Main.tile[num, num2].WallType != roomWall[0])
                                paintingEntry = RandBonePicture();

                            if (!Main.tile[num, num2].HasTile)
                                WorldGen.PlaceTile(num, num2, paintingEntry.tileType, mute: true, forced: false, -1, paintingEntry.style);

                            int num10 = num2;
                            int num11 = num;
                            int num12 = num;
                            for (int k = 0; k < 3; k++)
                            {
                                num2 += 7;
                                num3 = num12;
                                num4 = num12;
                                while (!Main.tile[num3, num2].HasTile && !Main.tile[num3, num2 - 1].HasTile && !Main.tile[num3, num2 + 1].HasTile)
                                {
                                    num3--;
                                }

                                num3++;
                                for (; !Main.tile[num4, num2].HasTile && !Main.tile[num4, num2 - 1].HasTile && !Main.tile[num4, num2 + 1].HasTile; num4++)
                                {
                                }

                                num4--;
                                num12 = (num3 + num4) / 2;
                                paintingEntry = WorldGen.RandPictureTile();
                                if (Main.tile[num12, num2].WallType != roomWall[0])
                                    paintingEntry = RandBonePicture();

                                if (Math.Abs(num11 - num12) >= 4 || WorldGen.nearPicture(num12, num2))
                                    break;

                                WorldGen.PlaceTile(num12, num2, paintingEntry.tileType, mute: true, forced: false, -1, paintingEntry.style);
                            }

                            num12 = num;
                            num2 = num10;
                            for (int l = 0; l < 3; l++)
                            {
                                num2 -= 7;
                                num3 = num12;
                                num4 = num12;
                                while (!Main.tile[num3, num2].HasTile && !Main.tile[num3, num2 - 1].HasTile && !Main.tile[num3, num2 + 1].HasTile)
                                {
                                    num3--;
                                }

                                num3++;
                                for (; !Main.tile[num4, num2].HasTile && !Main.tile[num4, num2 - 1].HasTile && !Main.tile[num4, num2 + 1].HasTile; num4++)
                                {
                                }

                                num4--;
                                num12 = (num3 + num4) / 2;
                                paintingEntry = WorldGen.RandPictureTile();
                                if (Main.tile[num12, num2].WallType != roomWall[0])
                                    paintingEntry = RandBonePicture();

                                if (Math.Abs(num11 - num12) >= 4 || WorldGen.nearPicture(num12, num2))
                                    break;

                                WorldGen.PlaceTile(num12, num2, paintingEntry.tileType, mute: true, forced: false, -1, paintingEntry.style);
                            }

                            break;
                        }
                }
            }

            return count;
        }

        private static double MakeDungeon_GroundFurniture(int wallType)
        {
            double num = (double)(2000 * Main.maxTilesX) / 4200.0;
            int num2 = 1 + (int)((double)Main.maxTilesX / 4200.0);
            int num3 = 1 + (int)((double)Main.maxTilesX / 4200.0);
            for (int i = 0; (double)i < num; i++)
            {
                if (num2 > 0 || num3 > 0)
                    i--;

                int num4 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                int j = WorldGen.genRand.Next((int)Main.worldSurface + 10, GenVars.dMaxY);
                while (!Main.wallDungeon[Main.tile[num4, j].WallType] || Main.tile[num4, j].HasTile)
                {
                    num4 = WorldGen.genRand.Next(GenVars.dMinX, GenVars.dMaxX);
                    j = WorldGen.genRand.Next((int)Main.worldSurface + 10, GenVars.dMaxY);
                }

                if (!Main.wallDungeon[Main.tile[num4, j].WallType] || Main.tile[num4, j].HasTile)
                    continue;

                for (; !WorldGen.SolidTile(num4, j) && j < Main.UnderworldLayer; j++)
                {
                }

                j--;
                int num5 = num4;
                int k = num4;
                while (!Main.tile[num5, j].HasTile && WorldGen.SolidTile(num5, j + 1))
                {
                    num5--;
                }

                num5++;
                for (; !Main.tile[k, j].HasTile && WorldGen.SolidTile(k, j + 1); k++)
                {
                }

                k--;
                int num6 = k - num5;
                int num7 = (k + num5) / 2;
                if (Main.tile[num7, j].HasTile || !Main.wallDungeon[Main.tile[num7, j].WallType] || !WorldGen.SolidTile(num7, j + 1) || Main.tile[num7, j + 1].TileType == 48)
                    continue;

                int style = 13;
                int style2 = 10;
                int style3 = 11;
                int num8 = 1;
                int num9 = 46;
                int style4 = 1;
                int num10 = 5;
                int num11 = 11;
                int num12 = 5;
                int num13 = 6;
                int num14 = 21;
                int num15 = 22;
                int num16 = 24;
                int num17 = 30;
                switch (wallType)
                {
                    case 8:
                        style = 14;
                        style2 = 11;
                        style3 = 12;
                        num8 = 2;
                        num9 = 47;
                        style4 = 2;
                        num10 = 6;
                        num11 = 12;
                        num12 = 6;
                        num13 = 7;
                        num14 = 22;
                        num15 = 23;
                        num16 = 25;
                        num17 = 31;
                        break;
                    case 9:
                        style = 15;
                        style2 = 12;
                        style3 = 13;
                        num8 = 3;
                        num9 = 48;
                        style4 = 3;
                        num10 = 7;
                        num11 = 13;
                        num12 = 7;
                        num13 = 8;
                        num14 = 23;
                        num15 = 24;
                        num16 = 26;
                        num17 = 32;
                        break;
                }

                if (Main.tile[num7, j].WallType >= 94 && Main.tile[num7, j].WallType <= 105)
                {
                    style = 17;
                    style2 = 14;
                    style3 = 15;
                    num8 = -1;
                    num9 = -1;
                    style4 = 5;
                    num10 = -1;
                    num11 = -1;
                    num12 = -1;
                    num13 = -1;
                    num14 = -1;
                    num15 = -1;
                    num16 = -1;
                    num17 = -1;
                }

                int num18 = WorldGen.genRand.Next(13);
                if ((num18 == 10 || num18 == 11 || num18 == 12) && !WorldGen.genRand.NextBool(4))
                    num18 = WorldGen.genRand.Next(13);

                while ((num18 == 2 && num9 == -1) || (num18 == 5 && num10 == -1) || (num18 == 6 && num11 == -1) || (num18 == 7 && num12 == -1) || (num18 == 8 && num13 == -1) || (num18 == 9 && num14 == -1) || (num18 == 10 && num15 == -1) || (num18 == 11 && num16 == -1) || (num18 == 12 && num17 == -1))
                {
                    num18 = WorldGen.genRand.Next(13);
                }

                int num19 = 0;
                int num20 = 0;
                if (num18 == 0)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 1)
                {
                    num19 = 4;
                    num20 = 3;
                }

                if (num18 == 2)
                {
                    num19 = 3;
                    num20 = 5;
                }

                if (num18 == 3)
                {
                    num19 = 4;
                    num20 = 6;
                }

                if (num18 == 4)
                {
                    num19 = 3;
                    num20 = 3;
                }

                if (num18 == 5)
                {
                    num19 = 5;
                    num20 = 3;
                }

                if (num18 == 6)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 7)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 8)
                {
                    num19 = 5;
                    num20 = 4;
                }

                if (num18 == 9)
                {
                    num19 = 5;
                    num20 = 3;
                }

                if (num18 == 10)
                {
                    num19 = 2;
                    num20 = 4;
                }

                if (num18 == 11)
                {
                    num19 = 3;
                    num20 = 3;
                }

                if (num18 == 12)
                {
                    num19 = 2;
                    num20 = 5;
                }

                for (int l = num7 - num19; l <= num7 + num19; l++)
                {
                    for (int m = j - num20; m <= j; m++)
                    {
                        if (Main.tile[l, m].HasTile)
                        {
                            num18 = -1;
                            break;
                        }
                    }
                }

                if ((double)num6 < (double)num19 * 1.75)
                    num18 = -1;

                if (num2 > 0 || num3 > 0)
                {
                    if (num2 > 0)
                    {
                        WorldGen.PlaceTile(num7, j, 355, mute: true);
                        if (Main.tile[num7, j].TileType == 355)
                            num2--;
                    }
                    else if (num3 > 0)
                    {
                        WorldGen.PlaceTile(num7, j, 354, mute: true);
                        if (Main.tile[num7, j].TileType == 354)
                            num3--;
                    }

                    continue;
                }

                switch (num18)
                {
                    case 0:
                        {
                            WorldGen.PlaceTile(num7, j, 14, mute: true, forced: false, -1, style2);
                            if (Main.tile[num7, j].HasTile)
                            {
                                if (!Main.tile[num7 - 2, j].HasTile)
                                {
                                    WorldGen.PlaceTile(num7 - 2, j, 15, mute: true, forced: false, -1, style);
                                    if (Main.tile[num7 - 2, j].HasTile)
                                    {
                                        Main.tile[num7 - 2, j].TileFrameX += 18;
                                        Main.tile[num7 - 2, j - 1].TileFrameX += 18;
                                    }
                                }

                                if (!Main.tile[num7 + 2, j].HasTile)
                                    WorldGen.PlaceTile(num7 + 2, j, 15, mute: true, forced: false, -1, style);
                            }

                            for (int num22 = num7 - 1; num22 <= num7 + 1; num22++)
                            {
                                if (WorldGen.genRand.NextBool(2) && !Main.tile[num22, j - 2].HasTile)
                                {
                                    int num23 = WorldGen.genRand.Next(5);
                                    if (num8 != -1 && num23 <= 1 && !Main.tileLighted[Main.tile[num22 - 1, j - 2].TileType])
                                        WorldGen.PlaceTile(num22, j - 2, 33, mute: true, forced: false, -1, num8);

                                    if (num23 == 2 && !Main.tileLighted[Main.tile[num22 - 1, j - 2].TileType])
                                        WorldGen.PlaceTile(num22, j - 2, 49, mute: true);

                                    if (num23 == 3)
                                        WorldGen.PlaceTile(num22, j - 2, 50, mute: true);

                                    if (num23 == 4)
                                        WorldGen.PlaceTile(num22, j - 2, 103, mute: true);
                                }
                            }

                            break;
                        }
                    case 1:
                        {
                            WorldGen.PlaceTile(num7, j, 18, mute: true, forced: false, -1, style3);
                            if (!Main.tile[num7, j].HasTile)
                                break;

                            if (WorldGen.genRand.NextBool(2))
                            {
                                if (!Main.tile[num7 - 1, j].HasTile)
                                {
                                    WorldGen.PlaceTile(num7 - 1, j, 15, mute: true, forced: false, -1, style);
                                    if (Main.tile[num7 - 1, j].HasTile)
                                    {
                                        Main.tile[num7 - 1, j].TileFrameX += 18;
                                        Main.tile[num7 - 1, j - 1].TileFrameX += 18;
                                    }
                                }
                            }
                            else if (!Main.tile[num7 + 2, j].HasTile)
                            {
                                WorldGen.PlaceTile(num7 + 2, j, 15, mute: true, forced: false, -1, style);
                            }

                            for (int n = num7; n <= num7 + 1; n++)
                            {
                                if (WorldGen.genRand.NextBool(2) && !Main.tile[n, j - 1].HasTile)
                                {
                                    int num21 = WorldGen.genRand.Next(5);
                                    if (num8 != -1 && num21 <= 1 && !Main.tileLighted[Main.tile[n - 1, j - 1].TileType])
                                        WorldGen.PlaceTile(n, j - 1, 33, mute: true, forced: false, -1, num8);

                                    if (num21 == 2 && !Main.tileLighted[Main.tile[n - 1, j - 1].TileType])
                                        WorldGen.PlaceTile(n, j - 1, 49, mute: true);

                                    if (num21 == 3)
                                        WorldGen.PlaceTile(n, j - 1, 50, mute: true);

                                    if (num21 == 4)
                                        WorldGen.PlaceTile(n, j - 1, 103, mute: true);
                                }
                            }

                            break;
                        }
                    case 2:
                        WorldGen.PlaceTile(num7, j, 105, mute: true, forced: false, -1, num9);
                        break;
                    case 3:
                        WorldGen.PlaceTile(num7, j, 101, mute: true, forced: false, -1, style4);
                        break;
                    case 4:
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(num7, j, 15, mute: true, forced: false, -1, style);
                            Main.tile[num7, j].TileFrameX += 18;
                            Main.tile[num7, j - 1].TileFrameX += 18;
                        }
                        else
                        {
                            WorldGen.PlaceTile(num7, j, 15, mute: true, forced: false, -1, style);
                        }
                        break;
                    case 5:
                        if (WorldGen.genRand.NextBool(2))
                            WorldGen.Place4x2(num7, j, 79, 1, num10);
                        else
                            WorldGen.Place4x2(num7, j, 79, -1, num10);
                        break;
                    case 6:
                        WorldGen.PlaceTile(num7, j, 87, mute: true, forced: false, -1, num11);
                        break;
                    case 7:
                        WorldGen.PlaceTile(num7, j, 88, mute: true, forced: false, -1, num12);
                        break;
                    case 8:
                        WorldGen.PlaceTile(num7, j, 89, mute: true, forced: false, -1, num13);
                        break;
                    case 9:
                        if (WorldGen.genRand.NextBool(2))
                            WorldGen.Place4x2(num7, j, 90, 1, num14);
                        else
                            WorldGen.Place4x2(num7, j, 90, -1, num14);
                        break;
                    case 10:
                        WorldGen.PlaceTile(num7, j, 93, mute: true, forced: false, -1, num16);
                        break;
                    case 11:
                        WorldGen.PlaceTile(num7, j, 100, mute: true, forced: false, -1, num15);
                        break;
                    case 12:
                        WorldGen.PlaceTile(num7, j, 104, mute: true, forced: false, -1, num17);
                        break;
                }
            }

            return num;
        }

        public static PaintingEntry RandBonePicture()
        {
            int num = WorldGen.genRand.Next(2);
            int num2 = 0;
            switch (num)
            {
                case 0:
                    num = 240;
                    num2 = WorldGen.genRand.Next(2);
                    switch (num2)
                    {
                        case 0:
                            num2 = 16;
                            break;
                        case 1:
                            num2 = 17;
                            break;
                    }
                    break;
                case 1:
                    num = 241;
                    num2 = WorldGen.genRand.Next(9);
                    break;
            }

            PaintingEntry result = default(PaintingEntry);
            result.tileType = num;
            result.style = num2;
            return result;
        }

        public static void DungeonStairs(int i, int j, ushort tileType, int wallType)
        {
            Vector2D zero = Vector2D.Zero;
            double num = WorldGen.genRand.Next(5, 9);
            int num2 = 1;
            Vector2D vector2D = default(Vector2D);
            vector2D.X = i;
            vector2D.Y = j;
            int num3 = WorldGen.genRand.Next(10, 30);
            num2 = ((i <= GenVars.dEnteranceX) ? 1 : (-1));
            if (i > Main.maxTilesX - 400)
                num2 = -1;
            else if (i < 400)
                num2 = 1;

            zero.Y = -1.0;
            zero.X = num2;
            if (!WorldGen.genRand.NextBool(3))
                zero.X *= 1.0 + (double)WorldGen.genRand.Next(0, 200) * 0.01;
            else if (WorldGen.genRand.NextBool(3))
                zero.X *= (double)WorldGen.genRand.Next(50, 76) * 0.01;
            else if (WorldGen.genRand.NextBool(6))
                zero.Y *= 2.0;

            if (GenVars.dungeonX < Main.maxTilesX / 2 && zero.X < 0.0 && zero.X < 0.5)
                zero.X = -0.5;

            if (GenVars.dungeonX > Main.maxTilesX / 2 && zero.X > 0.0 && zero.X > 0.5)
                zero.X = -0.5;

            if (WorldGen.drunkWorldGen)
            {
                num2 *= -1;
                zero.X *= -1.0;
            }

            while (num3 > 0)
            {
                num3--;
                int num4 = (int)(vector2D.X - num - 4.0 - (double)WorldGen.genRand.Next(6));
                int num5 = (int)(vector2D.X + num + 4.0 + (double)WorldGen.genRand.Next(6));
                int num6 = (int)(vector2D.Y - num - 4.0);
                int num7 = (int)(vector2D.Y + num + 4.0 + (double)WorldGen.genRand.Next(6));
                if (num4 < 0)
                    num4 = 0;

                if (num5 > Main.maxTilesX)
                    num5 = Main.maxTilesX;

                if (num6 < 0)
                    num6 = 0;

                if (num7 > Main.maxTilesY)
                    num7 = Main.maxTilesY;

                int num8 = 1;
                if (vector2D.X > (double)(Main.maxTilesX / 2))
                    num8 = -1;

                int num9 = (int)(vector2D.X + GenVars.dxStrength1 * 0.6 * (double)num8 + GenVars.dxStrength2 * (double)num8);
                int num10 = (int)(GenVars.dyStrength2 * 0.5);
                if (vector2D.Y < Main.worldSurface - 5.0 && Main.tile[num9, (int)(vector2D.Y - num - 6.0 + (double)num10)].WallType == 0 && Main.tile[num9, (int)(vector2D.Y - num - 7.0 + (double)num10)].WallType == 0 && Main.tile[num9, (int)(vector2D.Y - num - 8.0 + (double)num10)].WallType == 0)
                {
                    GenVars.dSurface = true;
                    WorldGen.TileRunner(num9, (int)(vector2D.Y - num - 6.0 + (double)num10), WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(10, 20), -1, addTile: false, 0.0, -1.0);
                }

                for (int k = num4; k < num5; k++)
                {
                    for (int l = num6; l < num7; l++)
                    {
                        Main.tile[k, l].LiquidAmount = 0;
                        if (!Main.wallDungeon[Main.tile[k, l].WallType])
                        {
                            Main.tile[k, l].WallType = 0;
                            Main.tile[k, l].ResetToType(tileType);
                        }
                    }
                }

                for (int m = num4 + 1; m < num5 - 1; m++)
                {
                    for (int n = num6 + 1; n < num7 - 1; n++)
                    {
                        Main.tile[m, n].WallType = (ushort)wallType;
                    }
                }

                int num11 = 0;
                if (WorldGen.genRand.NextBool((int)num))
                    num11 = WorldGen.genRand.Next(1, 3);

                num4 = (int)(vector2D.X - num * 0.5 - (double)num11);
                num5 = (int)(vector2D.X + num * 0.5 + (double)num11);
                num6 = (int)(vector2D.Y - num * 0.5 - (double)num11);
                num7 = (int)(vector2D.Y + num * 0.5 + (double)num11);
                if (num4 < 0)
                    num4 = 0;

                if (num5 > Main.maxTilesX)
                    num5 = Main.maxTilesX;

                if (num6 < 0)
                    num6 = 0;

                if (num7 > Main.maxTilesY)
                    num7 = Main.maxTilesY;

                for (int num12 = num4; num12 < num5; num12++)
                {
                    for (int num13 = num6; num13 < num7; num13++)
                    {
                        Main.tile[num12, num13].ClearTile();
                        WorldGen.PlaceWall(num12, num13, wallType, mute: true);
                    }
                }

                if (GenVars.dSurface)
                    num3 = 0;

                vector2D += zero;
                if (vector2D.Y < Main.worldSurface)
                    zero.Y *= 0.98;
            }

            GenVars.dungeonX = (int)vector2D.X;
            GenVars.dungeonY = (int)vector2D.Y;
        }

        public static bool DungeonPitTrap(int i, int j, ushort tileType, int wallType)
        {
            int num = 30;
            int num2 = j;
            int num3 = num2;
            int num4 = WorldGen.genRand.Next(8, 19);
            int num5 = WorldGen.genRand.Next(19, 46);
            int num6 = num4 + WorldGen.genRand.Next(6, 10);
            int num7 = num5 + WorldGen.genRand.Next(6, 10);
            if (!Main.wallDungeon[Main.tile[i, num2].WallType])
                return false;

            if (Main.tile[i, num2].HasTile)
                return false;

            for (int k = num2; k < Main.maxTilesY; k++)
            {
                if (k > Main.maxTilesY - 300)
                    return false;

                if (Main.tile[i, k].HasTile && WorldGen.SolidTile(i, k))
                {
                    if (Main.tile[i, k].TileType == 48)
                        return false;

                    num2 = k;
                    break;
                }
            }

            if (!Main.wallDungeon[Main.tile[i - num4, num2].WallType] || !Main.wallDungeon[Main.tile[i + num4, num2].WallType])
                return false;

            bool flag = true;
            for (int l = num2; l < num2 + num; l++)
            {
                flag = true;
                for (int m = i - num4; m <= i + num4; m++)
                {
                    Tile tile = Main.tile[m, l];
                    if (tile.HasTile && Main.tileDungeon[tile.TileType])
                        flag = false;
                }

                if (flag)
                {
                    num2 = l;
                    break;
                }
            }

            for (int n = i - num4; n <= i + num4; n++)
            {
                for (int num8 = num2; num8 <= num2 + num5; num8++)
                {
                    Tile tile2 = Main.tile[n, num8];
                    if (tile2.HasTile && (Main.tileDungeon[tile2.TileType] || tile2.TileType == GenVars.crackedType))
                        return false;
                }
            }

            bool flag2 = false;
            if (GenVars.dungeonLake)
            {
                flag2 = true;
                GenVars.dungeonLake = false;
            }
            else if (WorldGen.genRand.NextBool(8))
            {
                flag2 = true;
            }

            for (int num9 = i - num4; num9 <= i + num4; num9++)
            {
                for (int num10 = num3; num10 <= num2 + num5; num10++)
                {
                    if (Main.tileDungeon[Main.tile[num9, num10].TileType])
                    {
                        Main.tile[num9, num10].TileType = GenVars.crackedType;
                        Main.tile[num9, num10].WallType = (ushort)wallType;
                    }
                }
            }

            for (int num11 = i - num6; num11 <= i + num6; num11++)
            {
                for (int num12 = num3; num12 <= num2 + num7; num12++)
                {
                    Main.tile[num11, num12].Clear(TileDataType.Liquid);
                    Main.tile[num11, num12].LiquidAmount = 0;
                    if (!Main.wallDungeon[Main.tile[num11, num12].WallType] && Main.tile[num11, num12].TileType != GenVars.crackedType)
                    {
                        Main.tile[num11, num12].Clear(TileDataType.Slope);
                        Main.tile[num11, num12].ResetToType(tileType);
                        if (num11 > i - num6 && num11 < i + num6 && num12 < num2 + num7)
                            Main.tile[num11, num12].WallType = (ushort)wallType;
                    }
                }
            }

            for (int num13 = i - num4; num13 <= i + num4; num13++)
            {
                for (int num14 = num3; num14 <= num2 + num5; num14++)
                {
                    if (Main.tile[num13, num14].TileType != GenVars.crackedType)
                    {
                        if (flag2)
                            Main.tile[num13, num14].LiquidAmount = byte.MaxValue;

                        if (num13 == i - num4 || num13 == i + num4 || num14 == num2 + num5)
                            Main.tile[num13, num14].TileType = 48;
                        else if ((num13 == i - num4 + 1 && num14 % 2 == 0) || (num13 == i + num4 - 1 && num14 % 2 == 0) || (num14 == num2 + num5 - 1 && num13 % 2 == 0))
                            Main.tile[num13, num14].TileType = 48;
                        else
                            Main.tile[num13, num14].ClearTile();
                    }
                }
            }

            return true;
        }

        public static void DungeonHalls(int i, int j, ushort tileType, int wallType, bool forceX = false)
        {
            Vector2D zero = Vector2D.Zero;
            double num = WorldGen.genRand.Next(4, 6);
            double num2 = num;
            Vector2D zero2 = Vector2D.Zero;
            Vector2D zero3 = Vector2D.Zero;
            int num3 = 1;
            Vector2D vector2D = default(Vector2D);
            vector2D.X = i;
            vector2D.Y = j;
            int num4 = WorldGen.genRand.Next(35, 80);
            bool flag = false;
            if (WorldGen.genRand.NextBool(6))
                flag = true;

            if (forceX)
            {
                num4 += 20;
                GenVars.lastDungeonHall = Vector2D.Zero;
            }
            else if (WorldGen.genRand.NextBool(5))
            {
                num *= 2.0;
                num4 /= 2;
            }

            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = true;
            bool flag5 = false;
            while (!flag2)
            {
                flag5 = false;
                if (flag4 && !forceX)
                {
                    bool flag6 = true;
                    bool flag7 = true;
                    bool flag8 = true;
                    bool flag9 = true;
                    int num5 = num4;
                    bool flag10 = false;
                    for (int num6 = j; num6 > j - num5; num6--)
                    {
                        if (Main.tile[i, num6].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag6 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int k = j; k < j + num5; k++)
                    {
                        if (Main.tile[i, k].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag7 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int num7 = i; num7 > i - num5; num7--)
                    {
                        if (Main.tile[num7, j].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag8 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    flag10 = false;
                    for (int l = i; l < i + num5; l++)
                    {
                        if (Main.tile[l, j].WallType == wallType)
                        {
                            if (flag10)
                            {
                                flag9 = false;
                                break;
                            }
                        }
                        else
                        {
                            flag10 = true;
                        }
                    }

                    if (!flag8 && !flag9 && !flag6 && !flag7)
                    {
                        num3 = ((!WorldGen.genRand.NextBool(2)) ? 1 : (-1));
                        if (WorldGen.genRand.NextBool(2))
                            flag5 = true;
                    }
                    else
                    {
                        int num8 = WorldGen.genRand.Next(4);
                        do
                        {
                            num8 = WorldGen.genRand.Next(4);
                        } while (!(num8 == 0 && flag6) && !(num8 == 1 && flag7) && !(num8 == 2 && flag8) && !(num8 == 3 && flag9));

                        switch (num8)
                        {
                            case 0:
                                num3 = -1;
                                break;
                            case 1:
                                num3 = 1;
                                break;
                            default:
                                flag5 = true;
                                num3 = ((num8 != 2) ? 1 : (-1));
                                break;
                        }
                    }
                }
                else
                {
                    num3 = ((!WorldGen.genRand.NextBool(2)) ? 1 : (-1));
                    if (WorldGen.genRand.NextBool(2))
                        flag5 = true;
                }

                flag4 = false;
                if (forceX)
                    flag5 = true;

                if (flag5)
                {
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero3.Y = 0.0;
                    zero3.X = -num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.NextBool(3))
                    {
                        if (WorldGen.genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else
                {
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    zero3.X = 0.0;
                    zero3.Y = -num3;
                    if (!WorldGen.genRand.NextBool(3))
                    {
                        flag3 = true;
                        if (WorldGen.genRand.NextBool(2))
                            zero.X = (double)WorldGen.genRand.Next(10, 20) * 0.1;
                        else
                            zero.X = (double)(-WorldGen.genRand.Next(10, 20)) * 0.1;
                    }
                    else if (WorldGen.genRand.NextBool(2))
                    {
                        if (WorldGen.genRand.NextBool(2))
                            zero.X = (double)WorldGen.genRand.Next(20, 40) * 0.01;
                        else
                            zero.X = (double)(-WorldGen.genRand.Next(20, 40)) * 0.01;
                    }
                    else
                    {
                        num4 /= 2;
                    }
                }

                if (GenVars.lastDungeonHall != zero3)
                    flag2 = true;
            }

            int num9 = 0;
            bool flag11 = vector2D.Y < Main.rockLayer + 100.0;
            if (WorldGen.remixWorldGen)
                flag11 = vector2D.Y < Main.worldSurface + 100.0;

            if (!forceX)
            {
                if (vector2D.X > (double)(Main.maxTilesX - 200))
                {
                    num3 = -1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.NextBool(3))
                    {
                        if (WorldGen.genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.X < 200.0)
                {
                    num3 = 1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.NextBool(3))
                    {
                        if (WorldGen.genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.Y > (double)(Main.maxTilesY - 300))
                {
                    num3 = -1;
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    if (WorldGen.genRand.NextBool(2))
                    {
                        if (WorldGen.genRand.NextBool(2))
                            zero.X = (double)WorldGen.genRand.Next(20, 50) * 0.01;
                        else
                            zero.X = (double)(-WorldGen.genRand.Next(20, 50)) * 0.01;
                    }
                }
                else if (flag11)
                {
                    num3 = 1;
                    num += 1.0;
                    zero.Y = num3;
                    zero.X = 0.0;
                    zero2.X = 0.0;
                    zero2.Y = num3;
                    if (!WorldGen.genRand.NextBool(3))
                    {
                        flag3 = true;
                        if (WorldGen.genRand.NextBool(2))
                            zero.X = (double)WorldGen.genRand.Next(10, 20) * 0.1;
                        else
                            zero.X = (double)(-WorldGen.genRand.Next(10, 20)) * 0.1;
                    }
                    else if (WorldGen.genRand.NextBool(2))
                    {
                        if (WorldGen.genRand.NextBool(2))
                            zero.X = (double)WorldGen.genRand.Next(20, 50) * 0.01;
                        else
                            zero.X = (double)WorldGen.genRand.Next(20, 50) * 0.01;
                    }
                }
                else if (vector2D.X < (double)(Main.maxTilesX / 2) && vector2D.X > (double)Main.maxTilesX * 0.25)
                {
                    num3 = -1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.NextBool(3))
                    {
                        if (WorldGen.genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
                else if (vector2D.X > (double)(Main.maxTilesX / 2) && vector2D.X < (double)Main.maxTilesX * 0.75)
                {
                    num3 = 1;
                    zero2.Y = 0.0;
                    zero2.X = num3;
                    zero.Y = 0.0;
                    zero.X = num3;
                    if (WorldGen.genRand.NextBool(3))
                    {
                        if (WorldGen.genRand.NextBool(2))
                            zero.Y = -0.2;
                        else
                            zero.Y = 0.2;
                    }
                }
            }

            if (zero2.Y == 0.0)
            {
                GenVars.DDoorX[GenVars.numDDoors] = (int)vector2D.X;
                GenVars.DDoorY[GenVars.numDDoors] = (int)vector2D.Y;
                GenVars.DDoorPos[GenVars.numDDoors] = 0;
                GenVars.numDDoors++;
            }
            else
            {
                GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = (int)vector2D.X;
                GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = (int)vector2D.Y;
                GenVars.numDungeonPlatforms++;
            }

            GenVars.lastDungeonHall = zero2;
            if (Math.Abs(zero.X) > Math.Abs(zero.Y) && !WorldGen.genRand.NextBool(3))
                num = (int)(num2 * ((double)WorldGen.genRand.Next(110, 150) * 0.01));

            while (num4 > 0)
            {
                num9++;
                if (zero2.X > 0.0 && vector2D.X > (double)(Main.maxTilesX - 100))
                    num4 = 0;
                else if (zero2.X < 0.0 && vector2D.X < 100.0)
                    num4 = 0;
                else if (zero2.Y > 0.0 && vector2D.Y > (double)(Main.maxTilesY - 100))
                    num4 = 0;
                else if (WorldGen.remixWorldGen && zero2.Y < 0.0 && vector2D.Y < (Main.rockLayer + Main.worldSurface) / 2.0)
                    num4 = 0;
                else if (!WorldGen.remixWorldGen && zero2.Y < 0.0 && vector2D.Y < Main.rockLayer + 50.0)
                    num4 = 0;

                num4--;
                int num10 = (int)(vector2D.X - num - 4.0 - (double)WorldGen.genRand.Next(6));
                int num11 = (int)(vector2D.X + num + 4.0 + (double)WorldGen.genRand.Next(6));
                int num12 = (int)(vector2D.Y - num - 4.0 - (double)WorldGen.genRand.Next(6));
                int num13 = (int)(vector2D.Y + num + 4.0 + (double)WorldGen.genRand.Next(6));
                if (num10 < 0)
                    num10 = 0;

                if (num11 > Main.maxTilesX)
                    num11 = Main.maxTilesX;

                if (num12 < 0)
                    num12 = 0;

                if (num13 > Main.maxTilesY)
                    num13 = Main.maxTilesY;

                for (int m = num10; m < num11; m++)
                {
                    for (int n = num12; n < num13; n++)
                    {
                        if (m < GenVars.dMinX)
                            GenVars.dMinX = m;

                        if (m > GenVars.dMaxX)
                            GenVars.dMaxX = m;

                        if (n > GenVars.dMaxY)
                            GenVars.dMaxY = n;

                        Main.tile[m, n].LiquidAmount = 0;
                        if (!Main.wallDungeon[Main.tile[m, n].WallType])
                        {
                            Main.tile[m, n].ResetToType(tileType);
                            Main.tile[m, n].Clear(TileDataType.Slope);
                        }
                    }
                }

                for (int num14 = num10 + 1; num14 < num11 - 1; num14++)
                {
                    for (int num15 = num12 + 1; num15 < num13 - 1; num15++)
                    {
                        Main.tile[num14, num15].WallType = (ushort)wallType;
                    }
                }

                int num16 = 0;
                if (zero.Y == 0.0 && WorldGen.genRand.NextBool((int)num + 1))
                    num16 = WorldGen.genRand.Next(1, 3);
                else if (zero.X == 0.0 && WorldGen.genRand.NextBool((int)num - 1))
                    num16 = WorldGen.genRand.Next(1, 3);
                else if (WorldGen.genRand.NextBool((int)num * 3))
                    num16 = WorldGen.genRand.Next(1, 3);

                num10 = (int)(vector2D.X - num * 0.5 - (double)num16);
                num11 = (int)(vector2D.X + num * 0.5 + (double)num16);
                num12 = (int)(vector2D.Y - num * 0.5 - (double)num16);
                num13 = (int)(vector2D.Y + num * 0.5 + (double)num16);
                if (num10 < 0)
                    num10 = 0;

                if (num11 > Main.maxTilesX)
                    num11 = Main.maxTilesX;

                if (num12 < 0)
                    num12 = 0;

                if (num13 > Main.maxTilesY)
                    num13 = Main.maxTilesY;

                for (int num17 = num10; num17 < num11; num17++)
                {
                    for (int num18 = num12; num18 < num13; num18++)
                    {
                        Main.tile[num17, num18].Clear(TileDataType.Slope);
                        if (flag)
                        {
                            if (Main.tile[num17, num18].HasTile || Main.tile[num17, num18].WallType != wallType)
                            {
                                Main.tile[num17, num18].ResetToType(GenVars.crackedType);
                            }
                        }
                        else
                        {
                            Main.tile[num17, num18].ClearTile();
                        }

                        Main.tile[num17, num18].Clear(TileDataType.Slope);
                        Main.tile[num17, num18].WallType = (ushort)wallType;
                    }
                }

                vector2D += zero;
                if (flag3 && num9 > WorldGen.genRand.Next(10, 20))
                {
                    num9 = 0;
                    zero.X *= -1.0;
                }
            }

            GenVars.dungeonX = (int)vector2D.X;
            GenVars.dungeonY = (int)vector2D.Y;
            if (zero2.Y == 0.0)
            {
                GenVars.DDoorX[GenVars.numDDoors] = (int)vector2D.X;
                GenVars.DDoorY[GenVars.numDDoors] = (int)vector2D.Y;
                GenVars.DDoorPos[GenVars.numDDoors] = 0;
                GenVars.numDDoors++;
            }
            else
            {
                GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = (int)vector2D.X;
                GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = (int)vector2D.Y;
                GenVars.numDungeonPlatforms++;
            }
        }

        public static void DungeonRoom(int i, int j, ushort tileType, int wallType)
        {
            double num = WorldGen.genRand.Next(15, 30);
            Vector2D vector2D = default;
            vector2D.X = WorldGen.genRand.Next(-10, 11) * 0.1;
            vector2D.Y = WorldGen.genRand.Next(-10, 11) * 0.1;
            Vector2D vector2D2 = default;
            vector2D2.X = i;
            vector2D2.Y = j - num / 2.0;
            int count = WorldGen.genRand.Next(10, 20);
            double num3 = vector2D2.X;
            double num4 = vector2D2.X;
            double num5 = vector2D2.Y;
            double num6 = vector2D2.Y;
            while (count > 0)
            {
                count--;
                int num7 = (int)(vector2D2.X - num * 0.8 - 5.0);
                int num8 = (int)(vector2D2.X + num * 0.8 + 5.0);
                int num9 = (int)(vector2D2.Y - num * 0.8 - 5.0);
                int num10 = (int)(vector2D2.Y + num * 0.8 + 5.0);
                if (num7 < 0)
                    num7 = 0;

                if (num8 > Main.maxTilesX)
                    num8 = Main.maxTilesX;

                if (num9 < 0)
                    num9 = 0;

                if (num10 > Main.maxTilesY)
                    num10 = Main.maxTilesY;

                for (int k = num7; k < num8; k++)
                {
                    for (int l = num9; l < num10; l++)
                    {
                        if (k < GenVars.dMinX)
                            GenVars.dMinX = k;

                        if (k > GenVars.dMaxX)
                            GenVars.dMaxX = k;

                        if (l > GenVars.dMaxY)
                            GenVars.dMaxY = l;

                        Main.tile[k, l].LiquidAmount = 0;
                        if (!Main.wallDungeon[Main.tile[k, l].WallType])
                        {
                            Main.tile[k, l].Clear(TileDataType.Slope);
                            Main.tile[k, l].ResetToType(tileType);
                        }
                    }
                }

                for (int m = num7 + 1; m < num8 - 1; m++)
                {
                    for (int n = num9 + 1; n < num10 - 1; n++)
                    {
                        Main.tile[m, n].WallType = (ushort)wallType;
                    }
                }

                num7 = (int)(vector2D2.X - num * 0.5);
                num8 = (int)(vector2D2.X + num * 0.5);
                num9 = (int)(vector2D2.Y - num * 0.5);
                num10 = (int)(vector2D2.Y + num * 0.5);
                if (num7 < 0)
                    num7 = 0;

                if (num8 > Main.maxTilesX)
                    num8 = Main.maxTilesX;

                if (num9 < 0)
                    num9 = 0;

                if (num10 > Main.maxTilesY)
                    num10 = Main.maxTilesY;

                if (num7 < num3)
                    num3 = num7;

                if (num8 > num4)
                    num4 = num8;

                if (num9 < num5)
                    num5 = num9;

                if (num10 > num6)
                    num6 = num10;

                for (int m = num7; m < num8; m++)
                {
                    for (int n = num9; n < num10; n++)
                    {
                        Main.tile[m, n].ClearTile();
                        Main.tile[m, n].WallType = (ushort)wallType;
                    }
                }

                vector2D2 += vector2D;
                vector2D.X += WorldGen.genRand.Next(-10, 11) * 0.05;
                vector2D.Y += WorldGen.genRand.Next(-10, 11) * 0.05;
                if (vector2D.X > 1.0)
                    vector2D.X = 1.0;

                if (vector2D.X < -1.0)
                    vector2D.X = -1.0;

                if (vector2D.Y > 1.0)
                    vector2D.Y = 1.0;

                if (vector2D.Y < -1.0)
                    vector2D.Y = -1.0;
            }

            GenVars.dRoomX[GenVars.numDRooms] = (int)vector2D2.X;
            GenVars.dRoomY[GenVars.numDRooms] = (int)vector2D2.Y;
            GenVars.dRoomSize[GenVars.numDRooms] = (int)num;
            GenVars.dRoomL[GenVars.numDRooms] = (int)num3;
            GenVars.dRoomR[GenVars.numDRooms] = (int)num4;
            GenVars.dRoomT[GenVars.numDRooms] = (int)num5;
            GenVars.dRoomB[GenVars.numDRooms] = (int)num6;
            GenVars.dRoomTreasure[GenVars.numDRooms] = false;
            GenVars.numDRooms++;
        }

        public static void DungeonEnt(int i, int j, ushort tileType, int wallType)
        {
            int num = 60;
            for (int k = i - num; k < i + num; k++)
            {
                for (int l = j - num; l < j + num; l++)
                {
                    if (WorldGen.InWorld(k, l))
                    {
                        Main.tile[k, l].Clear(TileDataType.Liquid);
                        Main.tile[k, l].Clear(TileDataType.Slope);
                    }
                }
            }

            double dxStrength = GenVars.dxStrength1;
            double dyStrength = GenVars.dyStrength1;
            Vector2D vector2D = default(Vector2D);
            vector2D.X = i;
            vector2D.Y = (double)j - dyStrength / 2.0;
            GenVars.dMinY = (int)vector2D.Y;
            int num2 = 1;
            if (i > Main.maxTilesX / 2)
                num2 = -1;

            if (WorldGen.drunkWorldGen || WorldGen.getGoodWorldGen)
                num2 *= -1;

            int num3 = (int)(vector2D.X - dxStrength * 0.6 - (double)WorldGen.genRand.Next(2, 5));
            int num4 = (int)(vector2D.X + dxStrength * 0.6 + (double)WorldGen.genRand.Next(2, 5));
            int num5 = (int)(vector2D.Y - dyStrength * 0.6 - (double)WorldGen.genRand.Next(2, 5));
            int num6 = (int)(vector2D.Y + dyStrength * 0.6 + (double)WorldGen.genRand.Next(8, 16));
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int m = num3; m < num4; m++)
            {
                for (int n = num5; n < num6; n++)
                {
                    Main.tile[m, n].LiquidAmount = 0;
                    if (Main.tile[m, n].WallType != wallType)
                    {
                        Main.tile[m, n].WallType = 0;
                        if (m > num3 + 1 && m < num4 - 2 && n > num5 + 1 && n < num6 - 2)
                            Main.tile[m, n].WallType = (ushort)wallType;

                        Main.tile[m, n].ResetToType(tileType);
                        Main.tile[m, n].Clear(TileDataType.Slope);
                    }
                }
            }

            int num7 = num3;
            int num8 = num3 + 5 + WorldGen.genRand.Next(4);
            int num9 = num5 - 3 - WorldGen.genRand.Next(3);
            int num10 = num5;
            for (int num11 = num7; num11 < num8; num11++)
            {
                for (int num12 = num9; num12 < num10; num12++)
                {
                    Main.tile[num11, num12].LiquidAmount = 0;
                    if (Main.tile[num11, num12].WallType != wallType)
                    {
                        Main.tile[num11, num12].ResetToType(tileType);
                        Main.tile[num11, num12].Clear(TileDataType.Slope);
                    }
                }
            }

            num7 = num4 - 5 - WorldGen.genRand.Next(4);
            num8 = num4;
            num9 = num5 - 3 - WorldGen.genRand.Next(3);
            num10 = num5;
            for (int num13 = num7; num13 < num8; num13++)
            {
                for (int num14 = num9; num14 < num10; num14++)
                {
                    Main.tile[num13, num14].LiquidAmount = 0;
                    if (Main.tile[num13, num14].WallType != wallType)
                    {
                        Main.tile[num13, num14].ResetToType(tileType);
                        Main.tile[num13, num14].Clear(TileDataType.Slope);
                    }
                }
            }

            int num15 = 1 + WorldGen.genRand.Next(2);
            int num16 = 2 + WorldGen.genRand.Next(4);
            int num17 = 0;
            for (int num18 = num3; num18 < num4; num18++)
            {
                for (int num19 = num5 - num15; num19 < num5; num19++)
                {
                    Main.tile[num18, num19].LiquidAmount = 0;
                    if (Main.tile[num18, num19].WallType != wallType)
                    {
                        Main.tile[num18, num19].ResetToType(tileType);
                        Main.tile[num18, num19].Clear(TileDataType.Slope);
                    }
                }

                num17++;
                if (num17 >= num16)
                {
                    num18 += num16;
                    num17 = 0;
                }
            }

            for (int num20 = num3; num20 < num4; num20++)
            {
                for (int num21 = num6; (double)num21 < Main.worldSurface; num21++)
                {
                    Main.tile[num20, num21].LiquidAmount = 0;
                    if (!Main.wallDungeon[Main.tile[num20, num21].WallType])
                    {
                        Main.tile[num20, num21].ResetToType(tileType);
                    }

                    if (num20 > num3 && num20 < num4 - 1)
                        Main.tile[num20, num21].WallType = (ushort)wallType;

                    Main.tile[num20, num21].Clear(TileDataType.Slope);
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.6);
            num4 = (int)(vector2D.X + dxStrength * 0.6);
            num5 = (int)(vector2D.Y - dyStrength * 0.6);
            num6 = (int)(vector2D.Y + dyStrength * 0.6);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num22 = num3; num22 < num4; num22++)
            {
                for (int num23 = num5; num23 < num6; num23++)
                {
                    Main.tile[num22, num23].LiquidAmount = 0;
                    Main.tile[num22, num23].WallType = (ushort)wallType;
                    Main.tile[num22, num23].Clear(TileDataType.Slope);
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.6 - 1.0);
            num4 = (int)(vector2D.X + dxStrength * 0.6 + 1.0);
            num5 = (int)(vector2D.Y - dyStrength * 0.6 - 1.0);
            num6 = (int)(vector2D.Y + dyStrength * 0.6 + 1.0);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            if (WorldGen.drunkWorldGen)
                num3 -= 4;

            for (int num24 = num3; num24 < num4; num24++)
            {
                for (int num25 = num5; num25 < num6; num25++)
                {
                    Main.tile[num24, num25].LiquidAmount = 0;
                    Main.tile[num24, num25].WallType = (ushort)wallType;
                    Main.tile[num24, num25].Clear(TileDataType.Slope);
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            num5 = (int)(vector2D.Y - dyStrength * 0.5);
            num6 = (int)(vector2D.Y + dyStrength * 0.5);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num26 = num3; num26 < num4; num26++)
            {
                for (int num27 = num5; num27 < num6; num27++)
                {
                    Main.tile[num26, num27].LiquidAmount = 0;
                    Main.tile[num26, num27].ClearTile();
                    Main.tile[num26, num27].WallType = (ushort)wallType;
                }
            }

            int num28 = (int)vector2D.X;
            int num29 = num6;
            for (int num30 = 0; num30 < 20; num30++)
            {
                num28 = (int)vector2D.X - num30;
                if (!Main.tile[num28, num29].HasTile && Main.wallDungeon[Main.tile[num28, num29].WallType])
                {
                    GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = num28;
                    GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = num29;
                    GenVars.numDungeonPlatforms++;
                    break;
                }

                num28 = (int)vector2D.X + num30;
                if (!Main.tile[num28, num29].HasTile && Main.wallDungeon[Main.tile[num28, num29].WallType])
                {
                    GenVars.dungeonPlatformX[GenVars.numDungeonPlatforms] = num28;
                    GenVars.dungeonPlatformY[GenVars.numDungeonPlatforms] = num29;
                    GenVars.numDungeonPlatforms++;
                    break;
                }
            }

            vector2D.X += dxStrength * 0.6 * (double)num2;
            vector2D.Y += dyStrength * 0.5;
            dxStrength = GenVars.dxStrength2;
            dyStrength = GenVars.dyStrength2;
            vector2D.X += dxStrength * 0.55 * (double)num2;
            vector2D.Y -= dyStrength * 0.5;
            num3 = (int)(vector2D.X - dxStrength * 0.6 - (double)WorldGen.genRand.Next(1, 3));
            num4 = (int)(vector2D.X + dxStrength * 0.6 + (double)WorldGen.genRand.Next(1, 3));
            num5 = (int)(vector2D.Y - dyStrength * 0.6 - (double)WorldGen.genRand.Next(1, 3));
            num6 = (int)(vector2D.Y + dyStrength * 0.6 + (double)WorldGen.genRand.Next(6, 16));
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num31 = num3; num31 < num4; num31++)
            {
                for (int num32 = num5; num32 < num6; num32++)
                {
                    Main.tile[num31, num32].LiquidAmount = 0;
                    if (Main.tile[num31, num32].WallType == wallType)
                        continue;

                    bool flag = true;
                    if (num2 < 0)
                    {
                        if ((double)num31 < vector2D.X - dxStrength * 0.5)
                            flag = false;
                    }
                    else if ((double)num31 > vector2D.X + dxStrength * 0.5 - 1.0)
                    {
                        flag = false;
                    }

                    if (flag)
                    {
                        Main.tile[num31, num32].WallType = 0;
                        Main.tile[num31, num32].ResetToType(tileType);
                        Main.tile[num31, num32].Clear(TileDataType.Slope);
                    }
                }
            }

            for (int num33 = num3; num33 < num4; num33++)
            {
                for (int num34 = num6; (double)num34 < Main.worldSurface; num34++)
                {
                    Main.tile[num33, num34].LiquidAmount = 0;
                    if (!Main.wallDungeon[Main.tile[num33, num34].WallType])
                    {
                        Main.tile[num33, num34].ResetToType(tileType);
                    }

                    Main.tile[num33, num34].WallType = (ushort)wallType;
                    Main.tile[num33, num34].Clear(TileDataType.Slope);
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            num7 = num3;
            if (num2 < 0)
                num7++;

            num8 = num7 + 5 + WorldGen.genRand.Next(4);
            num9 = num5 - 3 - WorldGen.genRand.Next(3);
            num10 = num5;
            for (int num35 = num7; num35 < num8; num35++)
            {
                for (int num36 = num9; num36 < num10; num36++)
                {
                    Main.tile[num35, num36].LiquidAmount = 0;
                    if (Main.tile[num35, num36].WallType != wallType)
                    {
                        Main.tile[num35, num36].ResetToType(tileType);
                        Main.tile[num35, num36].Clear(TileDataType.Slope);
                    }
                }
            }

            num7 = num4 - 5 - WorldGen.genRand.Next(4);
            num8 = num4;
            num9 = num5 - 3 - WorldGen.genRand.Next(3);
            num10 = num5;
            for (int num37 = num7; num37 < num8; num37++)
            {
                for (int num38 = num9; num38 < num10; num38++)
                {
                    Main.tile[num37, num38].LiquidAmount = 0;
                    if (Main.tile[num37, num38].WallType != wallType)
                    {
                        Main.tile[num37, num38].ResetToType(tileType);
                        Main.tile[num37, num38].Clear(TileDataType.Slope);
                    }
                }
            }

            num15 = 1 + WorldGen.genRand.Next(2);
            num16 = 2 + WorldGen.genRand.Next(4);
            num17 = 0;
            if (num2 < 0)
                num4++;

            for (int num39 = num3 + 1; num39 < num4 - 1; num39++)
            {
                for (int num40 = num5 - num15; num40 < num5; num40++)
                {
                    Main.tile[num39, num40].LiquidAmount = 0;
                    if (Main.tile[num39, num40].WallType != wallType)
                    {
                        Main.tile[num39, num40].ResetToType(tileType);
                        Main.tile[num39, num40].Clear(TileDataType.Slope);
                    }
                }

                num17++;
                if (num17 >= num16)
                {
                    num39 += num16;
                    num17 = 0;
                }
            }

            if (!WorldGen.drunkWorldGen)
            {
                num3 = (int)(vector2D.X - dxStrength * 0.6);
                num4 = (int)(vector2D.X + dxStrength * 0.6);
                num5 = (int)(vector2D.Y - dyStrength * 0.6);
                num6 = (int)(vector2D.Y + dyStrength * 0.6);
                if (num3 < 0)
                    num3 = 0;

                if (num4 > Main.maxTilesX)
                    num4 = Main.maxTilesX;

                if (num5 < 0)
                    num5 = 0;

                if (num6 > Main.maxTilesY)
                    num6 = Main.maxTilesY;

                for (int num41 = num3; num41 < num4; num41++)
                {
                    for (int num42 = num5; num42 < num6; num42++)
                    {
                        Main.tile[num41, num42].LiquidAmount = 0;
                        Main.tile[num41, num42].WallType = 0;
                    }
                }
            }

            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            num5 = (int)(vector2D.Y - dyStrength * 0.5);
            num6 = (int)(vector2D.Y + dyStrength * 0.5);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num43 = num3; num43 < num4; num43++)
            {
                for (int num44 = num5; num44 < num6; num44++)
                {
                    Main.tile[num43, num44].LiquidAmount = 0;
                    Main.tile[num43, num44].ClearTile();
                    Main.tile[num43, num44].WallType = 0;
                }
            }

            Main.dungeonX = (int)vector2D.X;
            Main.dungeonY = num6;
            int num45 = NPC.NewNPC(new EntitySource_WorldGen(), Main.dungeonX * 16 + 8, Main.dungeonY * 16, 37);
            Main.npc[num45].homeless = false;
            Main.npc[num45].homeTileX = Main.dungeonX;
            Main.npc[num45].homeTileY = Main.dungeonY;
            if (WorldGen.drunkWorldGen)
            {
                int num46 = (int)Main.worldSurface;
                while (Main.tile[GenVars.dungeonX, num46].HasTile || Main.tile[GenVars.dungeonX, num46].WallType > 0 || Main.tile[GenVars.dungeonX, num46 - 1].HasTile || Main.tile[GenVars.dungeonX, num46 - 1].WallType > 0 || Main.tile[GenVars.dungeonX, num46 - 2].HasTile || Main.tile[GenVars.dungeonX, num46 - 2].WallType > 0 || Main.tile[GenVars.dungeonX, num46 - 3].HasTile || Main.tile[GenVars.dungeonX, num46 - 3].WallType > 0 || Main.tile[GenVars.dungeonX, num46 - 4].HasTile || Main.tile[GenVars.dungeonX, num46 - 4].WallType > 0)
                {
                    num46--;
                    if (num46 < 50)
                        break;
                }

                if (num46 > 50)
                    GrowDungeonTree(GenVars.dungeonX, num46);
            }

            if (!WorldGen.drunkWorldGen)
            {
                int num47 = 100;
                if (num2 == 1)
                {
                    int num48 = 0;
                    for (int num49 = num4; num49 < num4 + num47; num49++)
                    {
                        num48++;
                        for (int num50 = num6 + num48; num50 < num6 + num47; num50++)
                        {
                            Main.tile[num49, num50].LiquidAmount = 0;
                            Main.tile[num49, num50 - 1].LiquidAmount = 0;
                            Main.tile[num49, num50 - 2].LiquidAmount = 0;
                            Main.tile[num49, num50 - 3].LiquidAmount = 0;
                            if (!Main.wallDungeon[Main.tile[num49, num50].WallType] && Main.tile[num49, num50].WallType != 3 && Main.tile[num49, num50].WallType != 83)
                            {
                                Main.tile[num49, num50].ResetToType(tileType);
                                Main.tile[num49, num50].Clear(TileDataType.Slope);
                            }
                        }
                    }
                }
                else
                {
                    int num51 = 0;
                    for (int num52 = num3; num52 > num3 - num47; num52--)
                    {
                        num51++;
                        for (int num53 = num6 + num51; num53 < num6 + num47; num53++)
                        {
                            Main.tile[num52, num53].LiquidAmount = 0;
                            Main.tile[num52, num53 - 1].LiquidAmount = 0;
                            Main.tile[num52, num53 - 2].LiquidAmount = 0;
                            Main.tile[num52, num53 - 3].LiquidAmount = 0;
                            if (!Main.wallDungeon[Main.tile[num52, num53].WallType] && Main.tile[num52, num53].WallType != 3 && Main.tile[num52, num53].WallType != 83)
                            {
                                Main.tile[num52, num53].ResetToType(tileType);
                                Main.tile[num52, num53].Clear(TileDataType.Slope);
                            }
                        }
                    }
                }
            }

            num15 = 1 + WorldGen.genRand.Next(2);
            num16 = 2 + WorldGen.genRand.Next(4);
            num17 = 0;
            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            if (WorldGen.drunkWorldGen)
            {
                if (num2 == 1)
                {
                    num4--;
                    num3--;
                }
                else
                {
                    num3++;
                    num4++;
                }
            }
            else
            {
                num3 += 2;
                num4 -= 2;
            }

            for (int num54 = num3; num54 < num4; num54++)
            {
                for (int num55 = num5; num55 < num6 + 1; num55++)
                {
                    WorldGen.PlaceWall(num54, num55, wallType, mute: true);
                }

                if (!WorldGen.drunkWorldGen)
                {
                    num17++;
                    if (num17 >= num16)
                    {
                        num54 += num16 * 2;
                        num17 = 0;
                    }
                }
            }

            if (WorldGen.drunkWorldGen)
            {
                num3 = (int)(vector2D.X - dxStrength * 0.5);
                num4 = (int)(vector2D.X + dxStrength * 0.5);
                if (num2 == 1)
                    num3 = num4 - 3;
                else
                    num4 = num3 + 3;

                for (int num56 = num3; num56 < num4; num56++)
                {
                    for (int num57 = num5; num57 < num6 + 1; num57++)
                    {
                        Main.tile[num56, num57].ResetToType(tileType);
                        Main.tile[num56, num57].Clear(TileDataType.Slope);
                    }
                }
            }

            vector2D.X -= dxStrength * 0.6 * (double)num2;
            vector2D.Y += dyStrength * 0.5;
            dxStrength = 15.0;
            dyStrength = 3.0;
            vector2D.Y -= dyStrength * 0.5;
            num3 = (int)(vector2D.X - dxStrength * 0.5);
            num4 = (int)(vector2D.X + dxStrength * 0.5);
            num5 = (int)(vector2D.Y - dyStrength * 0.5);
            num6 = (int)(vector2D.Y + dyStrength * 0.5);
            if (num3 < 0)
                num3 = 0;

            if (num4 > Main.maxTilesX)
                num4 = Main.maxTilesX;

            if (num5 < 0)
                num5 = 0;

            if (num6 > Main.maxTilesY)
                num6 = Main.maxTilesY;

            for (int num58 = num3; num58 < num4; num58++)
            {
                for (int num59 = num5; num59 < num6; num59++)
                {
                    Main.tile[num58, num59].ClearTile();
                }
            }

            if (num2 < 0)
                vector2D.X -= 1.0;

            WorldGen.PlaceTile((int)vector2D.X, (int)vector2D.Y + 1, 10, mute: true, forced: false, -1, 13);
        }

        public static bool GrowDungeonTree(int i, int j, bool patch = false)
        {
            int num = 0;
            int[] array = new int[1000];
            int[] array2 = new int[1000];
            int[] array3 = new int[1000];
            int[] array4 = new int[1000];
            int num2 = 0;
            int[] array5 = new int[2000];
            int[] array6 = new int[2000];
            bool[] array7 = new bool[2000];
            int num3 = i - WorldGen.genRand.Next(2, 3);
            int num4 = i + WorldGen.genRand.Next(2, 3);
            if (WorldGen.genRand.NextBool(5))
            {
                if (WorldGen.genRand.NextBool(2))
                    num3--;
                else
                    num4++;
            }

            int num5 = num4 - num3;
            int num6 = num3;
            int num7 = num4;
            int minl = num3;
            int minr = num4;
            bool flag = true;
            int num8 = WorldGen.genRand.Next(-8, -4);
            int num9 = WorldGen.genRand.Next(2);
            int num10 = j;
            int num11 = WorldGen.genRand.Next(5, 15);
            Main.tileSolid[48] = false;
            while (flag)
            {
                num8++;
                if (num8 > num11)
                {
                    num11 = WorldGen.genRand.Next(5, 15);
                    num8 = 0;
                    array2[num] = num10 + WorldGen.genRand.Next(5);
                    if (WorldGen.genRand.NextBool(5))
                        num9 = ((num9 == 0) ? 1 : 0);

                    if (num9 == 0)
                    {
                        array3[num] = -1;
                        array[num] = num3;
                        array4[num] = num4 - num3;
                        if (WorldGen.genRand.NextBool(2))
                            num3++;

                        num6++;
                        num9 = 1;
                    }
                    else
                    {
                        array3[num] = 1;
                        array[num] = num4;
                        array4[num] = num4 - num3;
                        if (WorldGen.genRand.NextBool(2))
                            num4--;

                        num7--;
                        num9 = 0;
                    }

                    if (num6 == num7)
                        flag = false;

                    num++;
                }

                for (int k = num3; k <= num4; k++)
                {
                    Main.tile[k, num10].ResetToType(191);
                    Main.tile[k, num10].Clear(TileDataType.Slope);
                    WorldGen.paintTile(k, num10, 28);
                }

                num10--;
            }

            for (int l = 0; l < num - 1; l++)
            {
                int num12 = array[l] + array3[l];
                int num13 = array2[l];
                int num14 = (int)((double)array4[l] * (1.0 + (double)WorldGen.genRand.Next(20, 30) * 0.1));
                Main.tile[num12, num13 + 1].ResetToType(191);
                Main.tile[num12, num13 + 1].Clear(TileDataType.Slope);
                WorldGen.paintTile(num12, num13 + 1, 28);
                int num15 = WorldGen.genRand.Next(3, 5);
                while (num14 > 0)
                {
                    num14--;
                    Main.tile[num12, num13].ResetToType(191);
                    Main.tile[num12, num13].Clear(TileDataType.Slope);
                    WorldGen.paintTile(num12, num13, 28);
                    if (WorldGen.genRand.NextBool(10))
                        num13 = ((!WorldGen.genRand.NextBool(2)) ? (num13 + 1) : (num13 - 1));
                    else
                        num12 += array3[l];

                    if (num15 > 0)
                    {
                        num15--;
                    }
                    else if (WorldGen.genRand.NextBool(2))
                    {
                        num15 = WorldGen.genRand.Next(2, 5);
                        if (WorldGen.genRand.NextBool(2))
                        {
                            Main.tile[num12, num13].ResetToType(191);
                            Main.tile[num12, num13].Clear(TileDataType.Slope);
                            WorldGen.paintTile(num12, num13, 28);
                            Main.tile[num12, num13 - 1].ResetToType(191);
                            Main.tile[num12, num13 - 1].Clear(TileDataType.Slope);
                            WorldGen.paintTile(num12, num13 - 1, 28);
                            array5[num2] = num12;
                            array6[num2] = num13;
                            num2++;
                        }
                        else
                        {
                            Main.tile[num12, num13].ResetToType(191);
                            Main.tile[num12, num13].Clear(TileDataType.Slope);
                            WorldGen.paintTile(num12, num13, 28);
                            Main.tile[num12, num13 + 1].ResetToType(191);
                            Main.tile[num12, num13 + 1].Clear(TileDataType.Slope);
                            WorldGen.paintTile(num12, num13 + 1, 28);
                            array5[num2] = num12;
                            array6[num2] = num13;
                            num2++;
                        }
                    }

                    if (num14 == 0)
                    {
                        array5[num2] = num12;
                        array6[num2] = num13;
                        num2++;
                    }
                }
            }

            int num16 = (num3 + num4) / 2;
            int num17 = num10;
            int num18 = WorldGen.genRand.Next(num5 * 3, num5 * 5);
            int num19 = 0;
            int num20 = 0;
            while (num18 > 0)
            {
                Main.tile[num16, num17].ResetToType(191);
                Main.tile[num16, num17].Clear(TileDataType.Slope);
                WorldGen.paintTile(num16, num17, 28);
                if (num19 > 0)
                    num19--;

                if (num20 > 0)
                    num20--;

                for (int m = -1; m < 2; m++)
                {
                    if (m == 0 || ((m >= 0 || num19 != 0) && (m <= 0 || num20 != 0)) || !WorldGen.genRand.NextBool(2))
                        continue;

                    int num21 = num16;
                    int num22 = num17;
                    int num23 = WorldGen.genRand.Next(num5, num5 * 3);
                    if (m < 0)
                        num19 = WorldGen.genRand.Next(3, 5);

                    if (m > 0)
                        num20 = WorldGen.genRand.Next(3, 5);

                    int num24 = 0;
                    while (num23 > 0)
                    {
                        num23--;
                        num21 += m;
                        Main.tile[num21, num22].ResetToType(191);
                        Main.tile[num21, num22].Clear(TileDataType.Slope);
                        WorldGen.paintTile(num21, num22, 28);
                        if (num23 == 0)
                        {
                            array5[num2] = num21;
                            array6[num2] = num22;
                            array7[num2] = true;
                            num2++;
                        }

                        if (WorldGen.genRand.NextBool(5))
                        {
                            num22 = ((!WorldGen.genRand.NextBool(2)) ? (num22 + 1) : (num22 - 1));
                            Main.tile[num21, num22].ResetToType(191);
                            Main.tile[num21, num22].Clear(TileDataType.Slope);
                            WorldGen.paintTile(num21, num22, 28);
                        }

                        if (num24 > 0)
                        {
                            num24--;
                        }
                        else if (WorldGen.genRand.NextBool(3))
                        {
                            num24 = WorldGen.genRand.Next(2, 4);
                            int num25 = num21;
                            int num26 = num22;
                            num26 = ((!WorldGen.genRand.NextBool(2)) ? (num26 + 1) : (num26 - 1));
                            Main.tile[num25, num26].ResetToType(191);
                            Main.tile[num25, num26].Clear(TileDataType.Slope);
                            WorldGen.paintTile(num25, num26, 28);
                            array5[num2] = num25;
                            array6[num2] = num26;
                            array7[num2] = true;
                            num2++;
                            array5[num2] = num25 + WorldGen.genRand.Next(-5, 6);
                            array6[num2] = num26 + WorldGen.genRand.Next(-5, 6);
                            array7[num2] = true;
                            num2++;
                        }
                    }
                }

                array5[num2] = num16;
                array6[num2] = num17;
                num2++;
                if (WorldGen.genRand.NextBool(4))
                {
                    num16 = ((!WorldGen.genRand.NextBool(2)) ? (num16 + 1) : (num16 - 1));
                    Main.tile[num16, num17].ResetToType(191);
                    Main.tile[num16, num17].Clear(TileDataType.Slope);
                    WorldGen.paintTile(num16, num17, 28);
                }

                num17--;
                num18--;
            }

            for (int n = minl; n <= minr; n++)
            {
                int num27 = WorldGen.genRand.Next(1, 6);
                int num28 = j + 1;
                while (num27 > 0)
                {
                    if (WorldGen.SolidTile(n, num28))
                        num27--;

                    Main.tile[n, num28].ResetToType(191);
                    Main.tile[n, num28].Clear(TileDataType.Slope);
                    num28++;
                }

                int num29 = num28;
                int num30 = WorldGen.genRand.Next(2, num5 + 1);
                for (int num31 = 0; num31 < num30; num31++)
                {
                    num28 = num29;
                    int num32 = (minl + minr) / 2;
                    int num33 = 0;
                    int num34 = 1;
                    num33 = ((n >= num32) ? 1 : (-1));
                    if (n == num32 || (num5 > 6 && (n == num32 - 1 || n == num32 + 1)))
                        num33 = 0;

                    int num35 = num33;
                    int num36 = n;
                    num27 = WorldGen.genRand.Next((int)((double)num5 * 3.5), num5 * 6);
                    while (num27 > 0)
                    {
                        num27--;
                        num36 += num33;
                        if (Main.tile[num36, num28].WallType != 244)
                        {
                            Main.tile[num36, num28].ResetToType(191);
                            Main.tile[num36, num28].Clear(TileDataType.Slope);
                        }

                        num28 += num34;
                        if (Main.tile[num36, num28].WallType != 244)
                        {
                            Main.tile[num36, num28].ResetToType(191);
                            Main.tile[num36, num28].Clear(TileDataType.Slope);
                        }

                        if (!Main.tile[num36, num28 + 1].HasTile)
                        {
                            num33 = 0;
                            num34 = 1;
                        }

                        if (WorldGen.genRand.NextBool(3))
                            num33 = ((num35 < 0) ? ((num33 == 0) ? (-1) : 0) : ((num35 <= 0) ? WorldGen.genRand.Next(-1, 2) : ((num33 == 0) ? 1 : 0)));

                        if (WorldGen.genRand.NextBool(3))
                            num34 = ((num34 == 0) ? 1 : 0);
                    }
                }
            }

            if (!WorldGen.remixWorldGen)
            {
                for (int num37 = 0; num37 < num2; num37++)
                {
                    int num38 = WorldGen.genRand.Next(5, 8);
                    num38 = (int)((double)num38 * (1.0 + (double)num5 * 0.05));
                    if (array7[num37])
                        num38 = WorldGen.genRand.Next(6, 12) + num5;

                    int num39 = array5[num37] - num38 * 2;
                    int num40 = array5[num37] + num38 * 2;
                    int num41 = array6[num37] - num38 * 2;
                    int num42 = array6[num37] + num38 * 2;
                    double num43 = 2.0 - (double)WorldGen.genRand.Next(5) * 0.1;
                    for (int num44 = num39; num44 <= num40; num44++)
                    {
                        for (int num45 = num41; num45 <= num42; num45++)
                        {
                            if (Main.tile[num44, num45].TileType == 191)
                                continue;

                            if (array7[num37])
                            {
                                if ((new Vector2D(array5[num37], array6[num37]) - new Vector2D(num44, num45)).Length() < (double)num38 * 0.9)
                                {
                                    Main.tile[num44, num45].ResetToType(192);
                                    Main.tile[num44, num45].Clear(TileDataType.Slope);
                                    WorldGen.paintTile(num44, num45, 28);
                                }
                            }
                            else if ((double)Math.Abs(array5[num37] - num44) + (double)Math.Abs(array6[num37] - num45) * num43 < (double)num38)
                            {
                                Main.tile[num44, num45].ResetToType(192);
                                Main.tile[num44, num45].Clear(TileDataType.Slope);
                                WorldGen.paintTile(num44, num45, 28);
                            }
                        }
                    }
                }
            }

            GrowDungeonTree_MakePassage(j, num5, ref minl, ref minr, patch);
            Main.tileSolid[48] = true;
            return true;
        }

        public static void GrowDungeonTree_MakePassage(int j, int W, ref int minl, ref int minr, bool noSecretRoom = false)
        {
            int num = minl;
            int num2 = minr;
            _ = (minl + minr) / 2;
            int num3 = 5;
            int num4 = j - 6;
            int num5 = 0;
            bool flag = true;
            WorldGen.genRand.Next(5, 16);
            while (true)
            {
                num4++;
                if (num4 > GenVars.dungeonY - 5)
                    break;

                int num6 = (minl + minr) / 2;
                int num7 = 1;
                if (num4 > j && W <= 4)
                    num7++;

                for (int i = minl - num7; i <= minr + num7; i++)
                {
                    if (i > num6 - 2 && i <= num6 + 1)
                    {
                        if (num4 > j - 4)
                        {
                            if (Main.tile[i, num4].TileType != 19 && Main.tile[i, num4].TileType != 15 && Main.tile[i, num4].TileType != 304 && Main.tile[i, num4].TileType != 21 && Main.tile[i, num4].TileType != 10 && Main.tile[i, num4 - 1].TileType != 15 && Main.tile[i, num4 - 1].TileType != 304 && Main.tile[i, num4 - 1].TileType != 21 && Main.tile[i, num4 - 1].TileType != 10 && Main.tile[i, num4 + 1].TileType != 10)
                                Main.tile[i, num4].ClearTile();

                            if (!Main.wallDungeon[Main.tile[i, num4].WallType])
                                Main.tile[i, num4].WallType = 244;

                            if (!Main.wallDungeon[Main.tile[i - 1, num4].WallType] && (Main.tile[i - 1, num4].WallType > 0 || (double)num4 >= Main.worldSurface))
                                Main.tile[i - 1, num4].WallType = 244;

                            if (!Main.wallDungeon[Main.tile[i + 1, num4].WallType] && (Main.tile[i + 1, num4].WallType > 0 || (double)num4 >= Main.worldSurface))
                                Main.tile[i + 1, num4].WallType = 244;

                            if (num4 == j && i > num6 - 2 && i <= num6 + 1)
                            {
                                Main.tile[i, num4 + 1].ClearTile();
                                WorldGen.PlaceTile(i, num4 + 1, 19, mute: true, forced: false, -1, 23);
                            }
                        }
                    }
                    else
                    {
                        if (Main.tile[i, num4].TileType != 15 && Main.tile[i, num4].TileType != 304 && Main.tile[i, num4].TileType != 21 && Main.tile[i, num4].TileType != 10 && Main.tile[i - 1, num4].TileType != 10 && Main.tile[i + 1, num4].TileType != 10)
                        {
                            if (!Main.wallDungeon[Main.tile[i, num4].WallType])
                            {
                                Main.tile[i, num4].ResetToType(191);
                                Main.tile[i, num4].Clear(TileDataType.Slope);
                            }

                            if (Main.tile[i - 1, num4].TileType == 40)
                                Main.tile[i - 1, num4].ResetToType(0);

                            if (Main.tile[i + 1, num4].TileType == 40)
                                Main.tile[i - 1, num4].ResetToType(0);
                        }

                        if (num4 <= j && num4 > j - 4 && i > minl - num7 && i <= minr + num7 - 1)
                            Main.tile[i, num4].WallType = 244;
                    }

                    if (!WorldGen.gen)
                    {
                        WorldGen.SquareTileFrame(i, num4);
                        WorldGen.SquareWallFrame(i, num4);
                    }
                }

                num5++;
                if (num5 < 6)
                    continue;

                num5 = 0;
                int num8 = WorldGen.genRand.Next(3);
                if (num8 == 0)
                    num8 = -1;

                if (flag)
                    num8 = 2;

                if (num8 == -1 && Main.tile[minl - num3, num4].WallType == 244)
                    num8 = 1;
                else if (num8 == 1 && Main.tile[minr + num3, num4].WallType == 244)
                    num8 = -1;

                if (num8 == 2)
                {
                    flag = false;
                    int num9 = 23;
                    if (Main.wallDungeon[Main.tile[minl, num4 + 1].WallType] || Main.wallDungeon[Main.tile[minl + 1, num4 + 1].WallType] || Main.wallDungeon[Main.tile[minl + 2, num4 + 1].WallType])
                        num9 = 12;

                    if (!WorldGen.SolidTile(minl - 1, num4 + 1) && !WorldGen.SolidTile(minr + 1, num4 + 1) && num9 == 12)
                        continue;

                    for (int k = minl; k <= minr; k++)
                    {
                        if (k > num6 - 2 && k <= num6 + 1)
                        {
                            Main.tile[k, num4 + 1].ClearTile();
                            Main.tile[k, num4 + 1].Clear(TileDataType.Slope);
                            WorldGen.PlaceTile(k, num4 + 1, 19, mute: true, forced: false, -1, num9);
                        }
                    }
                }
                else
                {
                    minl += num8;
                    minr += num8;
                }
            }

            minl = num;
            minr = num2;
            _ = (minl + minr) / 2;
            for (int l = minl; l <= minr; l++)
            {
                for (int m = j - 3; m <= j; m++)
                {
                    Main.tile[l, m].ClearTile();
                    if (!Main.wallDungeon[Main.tile[l, m].WallType])
                        Main.tile[l, m].WallType = 244;
                }
            }
        }
        #endregion
    }

    public class ShadowCastleRoom
    {
        public enum RoomType
        {
            /// <summary> 普通房间，毛都没有，一般较小 </summary>
            Normal = 0,
            /// <summary> 普通影箱子 </summary>
            SingleChest,
            /// <summary> 所有存放地牢宝箱的地方 </summary>
            DenguonChest,
            /// <summary> 大堂，固定有3个方向 </summary>
            Lobby,
            /// <summary> 尖顶,作为末端而存在 </summary>
            Spire,
            /// <summary> 书房，包含水矢 </summary>
            Sanctum,
            /// <summary> 厨房 </summary>
            Kitchen,
            /// <summary> 温室，用于种植花朵 </summary>
            Greenhouse,
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        private static readonly string[] _roomTypeString = new string[]
        {
            "Normal",
            "SingleChest",
            "DenguonChest",
            "Lobby",
            "Spire",
            "Sanctum",
            "Kitchen",
            "Greenhouse",
        };
        public static string[] RoomTypeString => _roomTypeString;

        /// <summary>
        /// 这里存放了生成时的所有房间，子类请勿乱动它，仅在树根处能够使用
        /// </summary>
        public List<ShadowCastleRoom> shadowCastleRooms;

        public static Dictionary<Color, int> clearDic = new Dictionary<Color, int>()
        {
            [Color.White] = -2,
            [Color.Black] = -1
        };
        public static Dictionary<Color, int> GenDic = new Dictionary<Color, int>()
        {
            [Color.Black] = -1,
            [Color.White] = ModContent.TileType<ShadowBrickTile>(),
            [new Color(160, 95, 185)] = ModContent.TileType<ShadowBrickTile>(),//影之城砖 a05fb9
            [new Color(154, 153, 168)] = ModContent.TileType<ShadowQuadrelTile>(),//影方砖9a99a8
            [new Color(189, 109, 255)] = ModContent.TileType<ShadowImaginaryBrickTile>(),//影虚砖bd6dff
        };
        public static Dictionary<Color, int> WallDic = new Dictionary<Color, int>()
        {
            [Color.Black] = -1,
            [Color.White] = WallID.SandFall,
            [new Color(48, 18, 37)] =  ModContent.WallType<ShadowBrickWall>(),//影砖墙301225
        };
        public static Dictionary<Color, (int, int)> ObjectDic = new Dictionary<Color, (int, int)>
        {
            [new Color(186, 255, 196)] = (ModContent.TileType<MercuryPlatformTile>(), 0),//水银平台baffc4
        };

        public RoomType roomType;
        /// <summary>
        /// 房间的位置，大小
        /// </summary>
        public Rectangle roomRect;

        /// <summary>
        /// 房间链的深度
        /// </summary>
        public int depth;
        /// <summary>
        /// 随机类型<br></br>
        /// 使用<see cref="RandomTypeCount"/>来控制有多少种类型<br></br>
        /// 默认-1，在new的时候赋值
        /// </summary>
        public int randomType = -1;

        public virtual int RandomTypeCount { get => 1; }
        /// <summary>
        /// 顶部通道的位置
        /// </summary>
        public virtual Point[] UpCorridor { get; }
        /// <summary>
        /// 下方通道的位置
        /// </summary>
        public virtual Point[] DownCorridor { get; }
        /// <summary>
        /// 左边通道的位置
        /// </summary>
        public virtual Point[] LeftCorridor { get; }
        /// <summary>
        /// 右边通道的位置
        /// </summary>
        public virtual Point[] RightCorridor { get; }


        public virtual string RoomGenTex { get => GetType().Name; }
        public virtual string WallGenTex { get => RoomGenTex + "Wall"; }
        public virtual string RoomClearTex { get => RoomGenTex + "Clear"; }
        public virtual string WallClearTex { get => WallGenTex + "Clear"; }
        public virtual string ObjectTex { get => RoomGenTex + "Object"; }

        public int Width => roomRect.Width;
        public int Height => roomRect.Height;

        //父节点相对于自身的位置
        public Direction parentDirection;
        public ShadowCastleRoom parentRoom;
        public List<ShadowCastleRoom> childrenRooms;

        public ShadowCastleRoom(Point center, RoomType roomType)
        {
            if (RandomTypeCount > 1)
                randomType = WorldGen.genRand.Next(RandomTypeCount);

            string rand = "";
            if (randomType >= 0)
                rand = randomType.ToString();

            Texture2D roomTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + RoomGenTex + rand, AssetRequestMode.ImmediateLoad).Value;

            roomRect = new Rectangle(center.X - roomTex.Width / 2, center.Y - roomTex.Height / 2, roomTex.Width, roomTex.Height);
            this.roomType = roomType;
        }



        /// <summary>
        /// 生成type,并且创建
        /// </summary>
        public void InitializeType()
        {
            shadowCastleRooms = new List<ShadowCastleRoom>
            {
                this
            };
            OnInitialize(shadowCastleRooms);
        }

        public void OnInitialize(List<ShadowCastleRoom> rooms = null)
        {
            InitializeChildrens(rooms);

            if (childrenRooms != null)
                foreach (var room in childrenRooms)
                {
                    room.OnInitialize(rooms);
                }
        }

        /// <summary>
        /// 在这里new出子类，并添加进lizt中<br></br>
        /// 请使用<see cref="Append"/>以将实例化出的子房间加入自身列表中
        /// </summary>
        /// <param name="rooms"></param>
        public virtual void InitializeChildrens(List<ShadowCastleRoom> rooms = null)
        {

        }

        /// <summary>
        /// 将子房间加入到自身列表中
        /// </summary>
        /// <param name="room">子房间</param>
        /// <param name="direction">子房间相对于自身的方向</param>
        public bool Append(ShadowCastleRoom room, Direction direction, List<ShadowCastleRoom> rooms = null)
        {
            Vector2 topLeft = room.roomRect.TopLeft();
            Vector2 bottomRight = room.roomRect.BottomRight();
            if (!WorldGen.InWorld((int)topLeft.X, (int)topLeft.Y) || !WorldGen.InWorld((int)bottomRight.X, (int)bottomRight.Y))
            {
                return false;
            }

            //检测碰撞，如果产生了碰撞那么就直接重来！
            if (rooms != null)
            {
                foreach (var room2 in rooms)
                {
                    if (room2.roomRect.Intersects(room.roomRect))
                        return false;
                }
            }

            childrenRooms ??= new List<ShadowCastleRoom>();

            room.depth = depth + 1;
            room.parentRoom = this;
            room.parentDirection = ReverseDirection(direction);

            childrenRooms.Add(room);
            rooms?.Add(room);
            return true;
        }

        public virtual void Generate()
        {
            //生成自己
            GenerateSelf();
            PostGenerateSelf();

            //生成子房间
            GenChild();
        }

        public virtual void GenerateSelf()
        {
            string rand = "";
            if (randomType >= 0)
                rand = randomType.ToString();

            Texture2D roomTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + RoomGenTex + rand, AssetRequestMode.ImmediateLoad).Value;
            Texture2D clearTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + RoomClearTex + rand, AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallClearTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + WallClearTex + rand, AssetRequestMode.ImmediateLoad).Value;
            Texture2D wallTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + WallGenTex + rand, AssetRequestMode.ImmediateLoad).Value;

            //Task.Run(async () =>
            //{
            //    await GenRoom(clearTex, roomTex, wallClearTex, wallTex, clearDic, GenDic, clearDic, WallDic, roomRect.X, roomRect.Y);
            //}); 

            GenRoom2(clearTex, roomTex, wallClearTex, wallTex, clearDic, GenDic, clearDic, WallDic
                , roomRect.X, roomRect.Y);
        }

        public virtual void GenChild()
        {
            //调用子类的Generate方法
            if (childrenRooms != null)
                foreach (var room in childrenRooms)
                    room.Generate();
        }

        public virtual void CreateCorridor()
        {
            CreateSelfCorridor();

            CreateChildCorridor();
        }

        public virtual void CreateSelfCorridor()
        {
            if (childrenRooms != null)
            {
                foreach (var child in childrenRooms)
                {
                    Direction dir = child.parentDirection;
                    Point endPoint = new Point(child.roomRect.X, child.roomRect.Y) + child.GetCorridorPoint(dir);
                    dir = ReverseDirection(dir);
                    Point startPoint = new Point(roomRect.X, roomRect.Y) + GetCorridorPoint(dir);

                    GenerateCorridor(startPoint, endPoint, dir);
                }
            }
        }

        public virtual void CreateChildCorridor()
        {
            //调用子类的方法
            if (childrenRooms != null)
                foreach (var room in childrenRooms)
                    room.CreateCorridor();
        }


        /// <summary>
        /// 在生成完自身主体房间和墙壁后执行，用于生成特殊建筑物
        /// </summary>
        public virtual void PostGenerateSelf() { }

        public virtual bool NextDirection(out Direction direction)
        {
            List<Direction> directions = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            directions.Remove(parentDirection);

            if (childrenRooms != null)
                foreach (var child in childrenRooms)
                    directions.Remove(ReverseDirection(child.parentDirection));

            if (!directions.Any())
            {
                direction = Direction.Up;
                return false;
            }

            direction = WorldGen.genRand.NextFromList(directions.ToArray());
            return true;
        }

        #region 一些帮助方法
        public static Task GenRoom(Texture2D clearTex, Texture2D roomTex, Texture2D wallClearTex, Texture2D wallTex,
            Dictionary<Color, int> clearDic, Dictionary<Color, int> roomDic, Dictionary<Color, int> wallClearDic, Dictionary<Color, int> wallDic,
            int genOrigin_x, int genOrigin_y)
        {
            bool genned = false;
            bool placed = false;
            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    //清理范围
                    Texture2TileGenerator clearGenerator = TextureGeneratorDatas.GetTex2TileGenerator(clearTex, clearDic);
                    clearGenerator.Generate(genOrigin_x, genOrigin_y, true);

                    //生成主体地形
                    Texture2TileGenerator roomGenerator = TextureGeneratorDatas.GetTex2TileGenerator(roomTex, roomDic);
                    roomGenerator.Generate(genOrigin_x, genOrigin_y, true);

                    //清理范围
                    if (wallClearTex != null)
                    {
                        Texture2WallGenerator wallClearGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallClearTex, wallClearDic);
                        wallClearGenerator.Generate(genOrigin_x, genOrigin_y, true);
                    }

                    //生成墙壁
                    if (wallTex != null)
                    {
                        Texture2WallGenerator wallGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallTex, wallDic);
                        wallGenerator.Generate(genOrigin_x, genOrigin_y, true);
                    }

                    genned = true;
                });
                placed = true;
            }

            return Task.CompletedTask;
        }

        public static void GenRoom2(Texture2D clearTex, Texture2D roomTex, Texture2D wallClearTex, Texture2D wallTex,
            Dictionary<Color, int> clearDic, Dictionary<Color, int> roomDic, Dictionary<Color, int> wallClearDic, Dictionary<Color, int> wallDic,
            int genOrigin_x, int genOrigin_y)
        {
            bool genned = false;
            bool placed = false;

            WorldGenHelper.ClearLiuid(genOrigin_x, genOrigin_y, clearTex.Width, clearTex.Height);

            Texture2TileGenerator clearGenerator = null;
            Texture2TileGenerator roomGenerator = null;
            Texture2WallGenerator wallClearGenerator = null;
            Texture2WallGenerator wallGenerator = null;

            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    //清理范围
                    clearGenerator = TextureGeneratorDatas.GetTex2TileGenerator(clearTex, clearDic);

                    //生成主体地形
                    roomGenerator = TextureGeneratorDatas.GetTex2TileGenerator(roomTex, roomDic);

                    //清理范围
                    if (wallClearTex != null)
                        wallClearGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallClearTex, wallClearDic);

                    //生成墙壁
                    if (wallTex != null)
                        wallGenerator = TextureGeneratorDatas.GetTex2WallGenerator(wallTex, wallDic);

                    genned = true;
                });
                placed = true;
            }

            clearGenerator?.Generate(genOrigin_x, genOrigin_y, true);
            roomGenerator?.Generate(genOrigin_x, genOrigin_y, true);
            wallClearGenerator?.Generate(genOrigin_x, genOrigin_y, true);
            wallGenerator?.Generate(genOrigin_x, genOrigin_y, true);
        }

        public static void GenObject(Texture2D objectTex, Dictionary<Color, (int,int)> objectDic, int genOrigin_x, int genOrigin_y)
        {
            bool genned = false;
            bool placed = false;
            Texture2Object objectGenerator = null;

            while (!genned)
            {
                if (placed)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    //清理范围
                    objectGenerator = TextureGeneratorDatas.GetTex2ObjectGenerator(objectTex, objectDic);
                    genned = true;
                });

                placed = true;
            }

            objectGenerator?.Generate(genOrigin_x, genOrigin_y, true);
        }

        /// <summary>
        /// 生成通道
        /// </summary>
        /// <param name="startPoint">起始中心点</param>
        /// <param name="endPoint">结束中心点</param>
        /// <param name="direction">方向</param>
        /// <param name="CorridorHeight">通道的宽度，默认5格</param>
        /// <param name="WallWidth">通道墙壁的厚度，默认5格</param>
        public static void GenerateCorridor(Point startPoint, Point endPoint, Direction direction, int CorridorHeight = 5, int WallWidth = 5)
        {
            int shadowBrick = WorldGen.genRand.Next(3) switch
            {
                0 => ModContent.TileType<ShadowBrickTile>(),
                _ => ModContent.TileType<ShadowImaginaryBrickTile>()
            };
            //墙壁

            switch (direction)
            {
                default:
                case Direction.Up:
                case Direction.Down:
                    {
                        //方向
                        int dir = endPoint.Y > startPoint.Y ? 1 : -1;
                        //生成多远
                        int count = Math.Abs(endPoint.Y - startPoint.Y);
                        if (direction == Direction.Up)
                            startPoint += new Point(0, -1);

                        //把中心点挪到最左边
                        startPoint -= new Point(CorridorHeight / 2 + WallWidth, 0);
                        endPoint -= new Point(CorridorHeight / 2 + WallWidth, 0);

                        for (int y = 0; y < count; y++)
                        {
                            //当前的y位置
                            int currentY = startPoint.Y + y * dir;
                            int baseX = (int)Math.Round(Helper.Lerp(startPoint.X, endPoint.X, y / (float)(count - 1)));
                            for (int x = 0; x < CorridorHeight + WallWidth * 2; x++)
                            {
                                int currentX = baseX + x;

                                Tile tile = Main.tile[currentX, currentY];

                                //放置两边墙壁块
                                if (x < WallWidth || x > CorridorHeight + WallWidth)
                                {
                                    Main.tile[currentX, currentY].ClearEverything();
                                    WorldGen.PlaceTile(currentX, currentY, shadowBrick);
                                    //放墙
                                }
                                else//清空中间范围
                                {
                                    Main.tile[currentX, currentY].ClearEverything();
                                    //放墙
                                }
                            }
                        }
                    }
                    break;
                case Direction.Left:
                case Direction.Right:
                    {
                        //方向
                        int dir = endPoint.X > startPoint.X ? 1 : -1;
                        //生成多远
                        int count = Math.Abs(endPoint.X - startPoint.X);
                        //把中心点挪到最上边
                        if (direction == Direction.Left)//左边要额外减一下，不然会出问题
                            startPoint += new Point(-1, 0);

                        startPoint -= new Point(0, CorridorHeight / 2 + WallWidth);
                        endPoint -= new Point(0, CorridorHeight / 2 + WallWidth);


                        for (int x = 0; x < count; x++)
                        {
                            //当前的x位置
                            int currentX = startPoint.X + x * dir;
                            int baseY = (int)Math.Round(Helper.Lerp(startPoint.Y, endPoint.Y, x / (float)(count - 1)));

                            for (int y = 0; y < CorridorHeight + WallWidth * 2; y++)
                            {
                                int currentY = baseY + y;

                                Tile tile = Main.tile[currentX, currentY];

                                //放置上下两边墙壁物块
                                if (y < WallWidth || y > CorridorHeight + WallWidth)
                                {
                                    Main.tile[currentX, currentY].ClearEverything();
                                    WorldGen.PlaceTile(currentX, currentY, shadowBrick);
                                    //放墙壁
                                }
                                else//清空中间范围
                                {
                                    Main.tile[currentX, currentY].ClearEverything();
                                    //放墙壁
                                }
                            }
                        }

                    }
                    break;
            }
        }

        /// <summary>
        /// 将第一个房间替换为第二个房间
        /// </summary>
        /// <param name="room"></param>
        /// <param name="newRoom"></param>
        public static void Exchange(ShadowCastleRoom room, ShadowCastleRoom newRoom)
        {
            ShadowCastleRoom parentRoom = room.parentRoom;
            List<ShadowCastleRoom> childrens = room.childrenRooms;

            parentRoom.childrenRooms?.Remove(room);

            newRoom.parentDirection = room.parentDirection;
            newRoom.depth = room.depth;
            newRoom.parentRoom = parentRoom;
            newRoom.childrenRooms = childrens;

            parentRoom.childrenRooms?.Add(newRoom);
        }

        public void ResetCenter(Point center)
        {
            int width = roomRect.Width;
            int height = roomRect.Height;

            roomRect = new Rectangle(center.X - width / 2, center.Y - height / 2, width, height);
        }

        public static Direction ReverseDirection(Direction baseDirection)
        {
            return baseDirection switch
            {
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => Direction.Down,
            };

        }

        public static Point GetDir(Direction d)
        {
            return d switch
            {
                Direction.Up => new Point(0, -1),
                Direction.Down => new Point(0, 1),
                Direction.Left => new Point(-1, 0),
                _ => new Point(1, 0),
            };
        }

        public Point GetCorridorPoint(Direction direction)
        {
            int random = randomType == -1 ? 0 : randomType;
            Point p = direction switch
            {
                Direction.Up => UpCorridor[random],
                Direction.Down => DownCorridor[random],
                Direction.Left => LeftCorridor[random],
                _ => RightCorridor[random],
            };

            return p;
        }

        public void GenerateObject()
        {
            string rand = "";
            if (randomType >= 0)
                rand = randomType.ToString();

            Texture2D objectTex = ModContent.Request<Texture2D>(AssetDirectory.ShadowCastleRooms + ObjectTex + rand, AssetRequestMode.ImmediateLoad).Value;

            GenObject(objectTex, ObjectDic, roomRect.X, roomRect.Y);
        }


        #endregion
    }
}
