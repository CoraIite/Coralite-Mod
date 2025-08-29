using Coralite.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        /// <summary>
        /// 珊瑚猫的世界！
        /// </summary>
        public static bool CoralCatWorld { get; set; }

        public void CoralCatWorldGen(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "CoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCat";

            SoundEngine.PlaySound(CoraliteSoundID.Mio_Mewo_Miao_Item57);

            int OceanHeight = WorldGen.genRand.Next((int)(Main.worldSurface * 0.1f), (int)(Main.worldSurface * 0.35f));
            if (OceanHeight < 80)
                OceanHeight = 80;
            int maxDepth = (int)Main.rockLayer;

            for (int i = 0; i < 41; i++)
            {
                for (int j = OceanHeight; j < Main.maxTilesY; j++)
                {
                    Main.tile[i, j].ResetToType(TileID.Sand);
                }
            }

            for (int i = Main.maxTilesX - 42; i < Main.maxTilesX; i++)
            {
                for (int j = OceanHeight; j < Main.maxTilesY; j++)
                {
                    Main.tile[i, j].ResetToType(TileID.Sand);
                }
            }

            for (int i = 41; i < Main.maxTilesX - 42; i++)
                for (int j = OceanHeight; j < maxDepth; j++)
                {
                    Tile tile = Main.tile[i, j];

                    Tile top = Main.tile[i, j - 1];
                    Tile bottom = Main.tile[i, j + 1];
                    Tile left = Main.tile[i - 1, j];
                    Tile right = Main.tile[i + 1, j];

                    if ((top.LiquidType != 0 && top.LiquidAmount != 0) ||
                        (bottom.LiquidType != 0 && bottom.LiquidAmount != 0) ||
                        (left.LiquidType != 0 && left.LiquidAmount != 0) ||
                        (right.LiquidType != 0 && right.LiquidAmount != 0))
                    {
                        Main.tile[i, j].ResetToType(TileID.Sandstone);
                    }

                    if (tile.HasTile && Main.tileSolid[tile.TileType] && tile.LiquidType != 0 && tile.LiquidAmount != 0)
                        continue;

                    WorldGen.PlaceLiquid(i, j, (byte)LiquidID.Water, 255);//没水就放水
                }

            int shimmerLeft;
            int shimmerRight;

            if (GenVars.dungeonSide > 0)
            {
                shimmerLeft = (Main.maxTilesX / 2) + (Main.maxTilesX / 6);
                shimmerRight = Main.maxTilesX - 42;
            }
            else
            {
                shimmerLeft = 41;
                shimmerRight = (Main.maxTilesX / 2) - (Main.maxTilesX / 6);
            }

            //修改物块类型
            for (int i = 0; i < Main.maxTilesX; i++)
                for (int j = OceanHeight; j < maxDepth + 4; j++)
                {
                    Tile selfTile = Main.tile[i, j];
                    Tile bottomTile = Main.tile[i, j + 1];

                    if (selfTile.HasTile)
                    {
                        if (j > maxDepth && WorldGen.genRand.NextBool())
                            continue;
                        switch (selfTile.TileType)
                        {
                            default:
                                break;
                            case TileID.Trees:
                                if (bottomTile.HasTile)//树替换为沙子
                                    Main.tile[i, j].ResetToType(TileID.ShellPile);
                                else
                                    Main.tile[i, j].ResetToType(TileID.ReefBlock);
                                break;
                            case TileID.ClayBlock:
                            case TileID.Cloud:
                            case TileID.Mud:
                                Main.tile[i, j].ResetToType(TileID.ReefBlock);
                                break;
                            case TileID.Dirt:
                                Main.tile[i, j].ResetToType(TileID.Coralstone);
                                break;
                            case TileID.Grass:
                                Main.tile[i, j].ResetToType(TileID.BambooBlock);
                                break;
                            case TileID.SnowBlock:
                                Main.tile[i, j].ResetToType(TileID.SnowBrick);
                                break;
                            case TileID.IceBlock:
                                Main.tile[i, j].ResetToType(TileID.Waterfall);
                                break;
                            case TileID.Copper:
                                Main.tile[i, j].ResetToType(TileID.CopperBrick);
                                break;
                            case TileID.Tin:
                                Main.tile[i, j].ResetToType(TileID.TinBrick);
                                break;
                            case TileID.Iron:
                                Main.tile[i, j].ResetToType(TileID.IronBrick);
                                break;
                            case TileID.Lead:
                                Main.tile[i, j].ResetToType(TileID.LeadBrick);
                                break;
                            case TileID.Silver:
                                Main.tile[i, j].ResetToType(TileID.SilverBrick);
                                break;
                            case TileID.Tungsten:
                                Main.tile[i, j].ResetToType(TileID.TungstenBrick);
                                break;
                            case TileID.Gold:
                                Main.tile[i, j].ResetToType(TileID.GoldBrick);
                                break;
                            case TileID.Platinum:
                                Main.tile[i, j].ResetToType(TileID.PlatinumBrick);
                                break;

                            case TileID.Stone:
                                if (WorldGen.genRand.NextBool(40))
                                {
                                    ushort tileType = WorldGen.genRand.NextFromList(
                                        TileID.DiamondGemspark,
                                        TileID.AmethystGemspark,
                                        TileID.AmberGemspark,
                                        TileID.EmeraldGemspark,
                                        TileID.RubyGemspark,
                                        TileID.SapphireGemspark,
                                        TileID.TopazGemspark
                                        );
                                    Main.tile[i, j].ResetToType(tileType);
                                }
                                else
                                    Main.tile[i, j].ResetToType(TileID.HardenedSand);
                                break;
                        }
                    }
                }
            for (int i = 0; i < Main.maxTilesX; i++)
                for (int j = maxDepth; j < Main.maxTilesY; j++)
                {
                    Tile selfTile = Main.tile[i, j];
                    Tile bottomTile = Main.tile[i, j + 1];

                    if (selfTile.HasTile)
                    {
                        switch (selfTile.TileType)
                        {
                            default:
                                break;
                            case TileID.TreeAsh:
                                Main.tile[i, j].ResetToType(TileID.Ash);
                                break;
                            case TileID.Ash:
                                if (i < shimmerRight + WorldGen.genRand.Next(-4, 4)
                                    && i > shimmerLeft + WorldGen.genRand.Next(-4, 4))
                                {
                                    if (j > Main.maxTilesY - 80 + WorldGen.genRand.Next(-4, 4))
                                        Main.tile[i, j].ResetToType(TileID.ShimmerBlock);
                                    else
                                        Main.tile[i, j].ResetToType(TileID.ShimmerBrick);
                                }
                                else
                                    Main.tile[i, j].ResetToType(TileID.Dirt);
                                break;
                            case TileID.AshGrass:
                                {
                                    if (i < shimmerRight + WorldGen.genRand.Next(-4, 4)
                                         && i > shimmerLeft + WorldGen.genRand.Next(-4, 4))
                                    {
                                        Main.tile[i, j].ResetToType(TileID.RainbowMoss);
                                    }
                                    else
                                        Main.tile[i, j].ResetToType(TileID.Grass);
                                }
                                break;
                            case TileID.Sandstone:
                                {
                                    if (WorldGen.genRand.NextBool(8))
                                    {
                                        ushort tileType = WorldGen.genRand.NextFromList(
                                            TileID.Hive,
                                            TileID.HoneyBlock,
                                            TileID.CrispyHoneyBlock,
                                            TileID.PumpkinBlock,
                                            TileID.HayBlock
                                            );
                                        Main.tile[i, j].ResetToType(tileType);
                                    }
                                }
                                break;
                            case TileID.ObsidianBrick:
                                Main.tile[i, j].ResetToType(TileID.PineTree);
                                break;
                            case TileID.HellstoneBrick:
                                Main.tile[i, j].ResetToType(TileID.LivingMahoganyLeaves);
                                break;
                            case TileID.Hellstone:
                                {
                                    if (i < shimmerRight + WorldGen.genRand.Next(-4, 4)
                                        && i > shimmerLeft + WorldGen.genRand.Next(-4, 4))
                                    {
                                        ushort tileType = WorldGen.genRand.NextFromList(
                                        TileID.LivingFire,
                                        TileID.LivingCursedFire,
                                        TileID.LivingDemonFire,
                                        TileID.LivingFrostFire,
                                        TileID.LivingUltrabrightFire
                                        );
                                        Main.tile[i, j].ResetToType(tileType);
                                    }
                                    else
                                        Main.tile[i, j].ResetToType(TileID.Stone);
                                }
                                break;
                            case TileID.Copper:
                                Main.tile[i, j].ResetToType(TileID.Glass);
                                break;
                            case TileID.Tin:
                                Main.tile[i, j].ResetToType(TileID.Glass);
                                break;
                            case TileID.Iron:
                                Main.tile[i, j].ResetToType(TileID.Cloud);
                                break;
                            case TileID.Lead:
                                Main.tile[i, j].ResetToType(TileID.Cloud);
                                break;
                            case TileID.Silver:
                                Main.tile[i, j].ResetToType(TileID.RainCloud);
                                break;
                            case TileID.Tungsten:
                                Main.tile[i, j].ResetToType(TileID.RainCloud);
                                break;
                            case TileID.Gold:
                                Main.tile[i, j].ResetToType(TileID.Spikes);
                                break;
                            case TileID.Platinum:
                                Main.tile[i, j].ResetToType(TileID.Spikes);
                                break;
                            case TileID.Stone:
                                {
                                    if (i < (Main.maxTilesX / 4) + WorldGen.genRand.Next(-4, 4)
                                        || i > (Main.maxTilesX * 3 / 4) + WorldGen.genRand.Next(-4, 4))
                                    {
                                        Main.tile[i, j].ResetToType(TileID.Coralstone);
                                    }
                                    else
                                    {
                                        ushort tileType = WorldGen.genRand.NextFromList(
                                            TileID.WoodBlock,
                                            TileID.AshWood,
                                            TileID.BorealWood,
                                            TileID.DynastyWood,
                                            TileID.LivingWood,
                                            TileID.PalmWood,
                                            TileID.Ebonwood,
                                            TileID.Shadewood,
                                            TileID.Pearlwood
                                            );
                                        Main.tile[i, j].ResetToType(tileType);
                                    }
                                }
                                break;
                            case TileID.IceBlock:
                                {
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        Main.tile[i, j].ResetToType(TileID.Coralstone);
                                    }
                                }
                                break;
                            case TileID.SnowBlock:
                                {
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        Main.tile[i, j].ResetToType(TileID.SnowCloud);
                                    }
                                }
                                break;
                            case TileID.JungleGrass:
                                {
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        ushort tileType = WorldGen.genRand.NextFromList(
                                            TileID.SlimeBlock,
                                            TileID.FrozenSlimeBlock,
                                            TileID.SlimeBlock
                                            );
                                        Main.tile[i, j].ResetToType(tileType);
                                    }
                                }
                                break;
                            case TileID.Mud:
                                {
                                    if (WorldGen.genRand.NextBool(5))
                                    {
                                        ushort tileType = WorldGen.genRand.NextFromList(
                                            TileID.SlimeBlock,
                                            TileID.FrozenSlimeBlock,
                                            TileID.SlimeBlock
                                            );
                                        Main.tile[i, j].ResetToType(tileType);
                                    }
                                }
                                break;
                        }
                    }

                    if (selfTile.LiquidType == LiquidID.Lava && selfTile.LiquidAmount != 0)
                    {
                        Main.tile[i, j].Clear(Terraria.DataStructures.TileDataType.Liquid);
                        if (i < shimmerRight + WorldGen.genRand.Next(-4, 4)
                             && i > shimmerLeft + WorldGen.genRand.Next(-4, 4)
                                && j > Main.maxTilesY - 200 + WorldGen.genRand.Next(-4, 4))
                        {
                            Main.tile[i, j].ResetToType(TileID.Lavafall);
                        }
                        else if (j > Main.maxTilesY - 200 + WorldGen.genRand.Next(-4, 4))
                            Main.tile[i, j].ResetToType(TileID.Waterfall);
                        else
                            Main.tile[i, j].ResetToType(TileID.Lavafall);
                    }
                }

            for (int i = 41; i < Main.maxTilesX - 42; i++)
                for (int j = Main.maxTilesY - 200; j < Main.maxTilesY; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (!tile.HasTile || tile.TileType != TileID.Dirt)
                        continue;
                    Tile up = Main.tile[i, j - 1];
                    Tile bottom = Main.tile[i, j + 1];
                    Tile left = Main.tile[i - 1, j];
                    Tile right = Main.tile[i + 1, j];
                    Tile upLeft = Main.tile[i - 1, j - 1];
                    Tile upRight = Main.tile[i + 1, j - 1];
                    Tile bottomLeft = Main.tile[i - 1, j + 1];
                    Tile bottomRight = Main.tile[i + 1, j + 1];

                    if (!up.HasTile || !bottom.HasTile || !left.HasTile || !right.HasTile ||
                        !upLeft.HasTile || !upRight.HasTile || !bottomLeft.HasTile || !bottomRight.HasTile ||
                      !Main.tileSolid[up.TileType] || !Main.tileSolid[bottom.TileType]
                      || !Main.tileSolid[left.TileType] || !Main.tileSolid[right.TileType] ||
                      !Main.tileSolid[upLeft.TileType] || !Main.tileSolid[upRight.TileType]
                      || !Main.tileSolid[bottomLeft.TileType] || !Main.tileSolid[bottomRight.TileType])
                        Main.tile[i, j].ResetToType(TileID.Grass);
                }

            //地表沙滩化
            int count = WorldGen.genRand.Next(7, 9);
            for (int k = 1; k < count; k++)
            {
                for (int i = 0; i < Main.maxTilesX; i++)
                    for (int j = OceanHeight; j < maxDepth; j++)
                    {
                        Tile tile = Main.tile[i, j - k];
                        Tile selfTile = Main.tile[i, j];

                        if (WorldGen.genRand.NextBool(1, k))
                            continue;
                        if (!selfTile.HasTile || !Main.tileSolid[selfTile.TileType] || TileID.Sets.IsAContainer[selfTile.TileType])
                            continue;
                        if (tile.HasTile && (Main.tileSolid[tile.TileType] || TileID.Sets.IsAContainer[tile.TileType]))
                            continue;

                        if (tile.LiquidType != LiquidID.Water && tile.LiquidAmount != 0)
                            continue;

                        if (Main.tile[i, j + 1].HasTile)
                            Main.tile[i, j].ResetToType(TileID.Sand);//有水就放沙子
                        else
                            Main.tile[i, j].ResetToType(TileID.Sandstone);//有水就放沙子
                    }
            }
            //放置彩虹
            for (int i = 40; i < Main.maxTilesX - 40; i++)
            {
                int y = Main.maxTilesY - 240 + (int)(MathF.Sin(i * 0.05f) * 5);

                for (int j = 0; j < 14; j++)
                {
                    int y2 = y + j;
                    ushort tileType = (j / 2) switch
                    {
                        0 => TileID.RubyGemspark,
                        1 => TileID.AmberGemspark,
                        2 => TileID.TopazGemspark,
                        3 => TileID.EmeraldGemspark,
                        4 => TileID.XenonMossBlock,
                        5 => TileID.SapphireGemspark,
                        _ => TileID.AmethystGemspark,
                    };
                    Main.tile[i, y2].ClearEverything();
                    Main.tile[i, y2].ResetToType(tileType);
                    WorldGen.paintCoatTile(i, y2, PaintCoatingID.Glow);
                }
            }
            for (int i = 40; i < Main.maxTilesX - 40; i++)
            {
                int y = maxDepth + (int)(MathF.Sin(i * 0.05f) * 3);

                Tile tile = Main.tile[i, y];
                if (tile.HasTile && Main.tileDungeon[tile.TileType])
                    continue;
                if (tile.LiquidType == LiquidID.Shimmer && tile.LiquidAmount > 0)
                    continue;

                for (int j = 0; j < 7; j++)
                {
                    int y2 = y + j;
                    Main.tile[i, y2].ClearEverything();
                    Main.tile[i, y2].ResetToType(TileID.Bubble);
                    WorldGen.paintCoatTile(i, y2, PaintCoatingID.Glow);
                }
            }

            //生成泡泡
            for (int i = 2; i < 120; i++)
            {
                int x = i * (Main.maxTilesX - 200) / 120;

                int y = WorldGen.genRand.Next(OceanHeight + 20, (int)Main.worldSurface);

                if (Main.tile[x, y].HasTile || Main.tile[x, y].LiquidAmount == 0)
                    continue;

                Point origin = new(x, y);
                int width = WorldGen.genRand.Next(2, 12);

                ShapeData circleData = new();

                WorldUtils.Gen(
                    origin,  //中心点
                    new Shapes.Circle(width, width),   //形状：圆
                    Actions.Chain(  //如果要添加多个效果得使用这个chain
                        new Actions.Clear().Output(circleData)    //清除形状内所有物块
                       ));   //通过output记录当前的形状

                //生成外围圈
                WorldUtils.Gen(
                    origin,
                    new ModShapes.OuterOutline(circleData),  //使用刚刚生成出来的形状，该形状的取外边缘
                    Actions.Chain(
                                new Actions.SetTile(TileID.Bubble)    //放置物块
                       ));
            }
            //生成水下小遗迹
            for (int i = 2; i < 100; i++)
            {
                int x = i * (Main.maxTilesX - 200) / 100;

                int y = (int)Main.worldSurface;

                while (y > 0)
                {
                    if (!Main.tile[x, y].HasTile)
                        break;
                    y--;
                }

                if (y < 100)
                    continue;

                int roomWidth = WorldGen.genRand.Next(10, 15);
                int roomHeight = WorldGen.genRand.Next(8, 12);
                for (int m = 0; m < roomWidth; m++)
                    for (int n = 0; n < roomHeight; n++)
                    {
                        if (m == 0 || m == roomWidth - 1 || n == 0 || n == roomHeight - 1)//放置盒子
                        {
                            Main.tile[x + m, y - n].ResetToType(TileID.GrayBrick);
                            continue;
                        }

                        WorldGen.KillTile(x + m, y - n, noItem: true);
                        if (Math.Abs((roomWidth / 2f) - m) > (roomWidth / 4f) || Math.Abs((roomHeight / 2f) - n) > roomHeight / 4f)
                        {
                            WorldGen.PlaceWall(x + m, y - n, WallID.GrayBrick);
                        }
                    }

                WorldGen.AddBuriedChest(new Point(x + WorldGen.genRand.Next(0, roomHeight), y - 1),
                    WorldGen.genRand.NextFromList(
                        //ItemID.RainbowBrick,
                        ItemID.JojaCola,
                        ItemID.LicenseBunny,
                        ItemID.LicenseCat,
                        ItemID.LicenseDog,
                        ItemID.GoldBirdCage,
                        ItemID.GoldBunnyCage,
                        ItemID.GoldButterflyCage,
                        ItemID.GoldDragonflyJar,
                        ItemID.GoldFrogCage,
                        ItemID.GoldGrasshopperCage,
                        ItemID.GoldGoldfishBowl,
                        ItemID.GoldLadybugCage,
                        ItemID.GoldMouseCage,
                        ItemID.GoldSeahorseCage,
                        ItemID.GoldWaterStriderCage,
                        ItemID.GoldWormCage,
                        ItemID.CatEars,
                        ItemID.Catfish,
                        ItemID.CatMask,
                        ItemID.CatPants,
                        ItemID.CatShirt,
                        ItemID.CatSword
                        ), Style: 5);
            }
        }

        public void CoralCatWorldSpawn(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "CoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCatCoralCat";

            int spawnY = 0;
            for (; spawnY < Main.worldSurface; spawnY++)
            {
                Tile tile = Main.tile[Main.spawnTileX, spawnY];
                if (tile.HasTile || tile.LiquidAmount != 0)
                    break;
            }

            if (spawnY < 3)
                spawnY = 3;
            Main.spawnTileY = spawnY - 1;

            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 2 + (j * 2); i++)
                    Main.tile[Main.spawnTileX - (1 + j) + i, spawnY + 5 - j].ResetToType(TileID.Sandstone);
            }

            for (int i = 0; i < 8; i++)
                Main.tile[Main.spawnTileX - 4 + i, spawnY].ResetToType(TileID.Sand);

            WorldGen.AddBuriedChest(new Point(Main.spawnTileX, spawnY - 1), ItemID.Acorn, Style: 5);
        }
    }
}
