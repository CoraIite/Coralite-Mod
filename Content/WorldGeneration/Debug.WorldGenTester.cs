using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public class WorldGenTester : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            //Item.shoot = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //if (Main.myPlayer == player.whoAmI)
            //{
            //    float rot = (Main.MouseWorld - player.Center).ToRotation();
            //    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<CrystalLaser>(), 10, 0, player.whoAmI, rot);

            //}
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            Vector2 myVector = Main.MouseWorld;
            Point p = myVector.ToTileCoordinates();

            CoraliteWorld.GenGenFairyPortal_AcutallyGen(p);
            //NPC.downedBoss1 = true;
            //NPC.downedBoss2 = true;
            //NPC.downedGolemBoss = true;
            //NPC.downedPlantBoss = true;
            //NPC.downedMartians = true;
            //NPC.downedMoonlord = true;
            //NPC.downedTowerNebula = true;
            //NPC.downedBoss3 = true;
            //NPC.downedQueenSlime = true;
            //NPC.downedMechBoss1 = true;
            //NPC.downedMechBoss2 = true;
            //NPC.downedMechBoss3 = true;
            //NPC.downedChristmasIceQueen = true;
            //NPC.downedQueenBee = true;
            //NPC.downedHalloweenKing = true;
            //Main.hardMode = true;
            //DownedBossSystem.downedNightmarePlantera = true;

            //WorldGen.paintTile();
            //WorldGen.paintWall();
            //WorldGen.SlopeTile(p.X, p.Y,(int)SlopeType.SlopeDownLeft);
            //WorldGen.PoundTile();
            //ExampleStructure(p);
            //Main.NewText(TileLoader.GetTile(860).Name);
            //WorldGen.destroyObject = false;
            //Main.tile[p].Clear(TileDataType.Wall);
            //WorldGen.KillTile(p.X, p.Y, false, false, true);
            //Main.tile[p.X, p.Y].Clear(TileDataType.Tile);
            //WorldGen.PlaceTile(p.X, p.Y, TileID.Dirt, true, true, -1);    //放置有帧图的物块
            //Main.AnglerQuestSwap();
            //Point p = Main.MouseWorld.ToTileCoordinates();
            //Tile t = Main.tile[p];
            //Main.NewText(t.TileFrameX);
            //WorldGen.PlaceObject(p.X, p.Y - 1, ModContent.TileType<ChalcedonySapling>(), true);
            //WorldGenHelper.ObjectPlace(point.X, point.Y - 1, ModContent.TileType<ChalcedonyGrass2x2>(), WorldGen.genRand.Next(2));
            //WorldGen.PlaceObject(pos.X, pos.Y, ModContent.TileType<MercuryPlatformTile>());

            //ModItem modItem = ItemLoader.GetItem(5614);
            //Main.NewText(modItem.Name);
            //Main.dayTime = true;
            //Main.time = 4000;
            //Main.windSpeedTarget = 0.8f;
            //TileEntity.Clear();

            //Main.tile.ClearEverything();

            //int x = (int)(Main.rand.NextFloat() * 100);
            //int y = (int)(Main.rand.NextFloat() * 100);

            //for (int i = 0; i < 100; i++)
            //    for (int j = 0; j < 100; j++)
            //    {
            //        float mainNoise = ModContent.GetInstance<CoraliteWorld>().MainNoise(new Vector2(x + i, y+j), new Vector2(100, 100)*8);
            //        if (mainNoise > 0.8f)
            //        {
            //            Dust d = Dust.NewDustPerfect(Main.MouseWorld + new Vector2(i, j) * 8, DustID.GemDiamond, Vector2.Zero, Scale: 2);
            //            d.noGravity = true;
            //        }
            //    }

            //int brickCount = 20;
            //ushort skarnBrick = (ushort)ModContent.TileType<SkarnBrickTile>();
            //Rectangle outerRect = Utils.CenteredRectangle(player.Center/16, new Vector2(100, 100));
            //for (int i = 0; i < brickCount; i++)
            //{
            //    //随机找点
            //    Point p = new Point(0, 0);

            //    for (int j = 0; j < 1000; j++)//找到一个自身没物块，但是底部有物块的地方
            //    {
            //        Point p2 = WorldGen.genRand.NextVector2FromRectangle(outerRect).ToPoint();

            //        Dictionary<ushort, int> scan = [];
            //        WorldUtils.Gen(p2 - new Point(4, 4), new Shapes.Rectangle(4, 4)
            //            , new Actions.TileScanner(skarnBrick).Output(scan));
            //        if (scan[skarnBrick] > 0)
            //            continue;

            //        p = p2;
            //    }

            //    if (p == default)
            //        continue;

            //    //额外向下的长度
            //    int exY = 1;

            //    int checkY = WorldGenHelper.CheckUpAreaEmpty(p);

            //    //检测上方的空间，如果有空间那么就向上突起
            //    if (checkY > 5)
            //        exY = WorldGen.genRand.Next(-1, 1);
            //    if (checkY > 7)
            //        exY = WorldGen.genRand.Next(-3, 1);

            //    int brickWidth = WorldGen.genRand.Next(2, 4);
            //    int maxY = 18;

            //    //向下生成砖块
            //    for (int k = 0; k < maxY; k++)
            //        for (int n = 0; n < brickWidth; n++)
            //        {
            //            Point brickP = p + new Point(brickWidth / 2 + n, exY + k);

            //            if (n == 0 && !Main.tile[brickP.X, brickP.Y].HasTile)
            //            {
            //                int yBottomCheck = WorldGenHelper.CheckBottomAreaEmpty(brickP);
            //                if (yBottomCheck < 5)
            //                    goto spawnEnd;
            //            }

            //            //Main.tile[brickP.X, brickP.Y].ClearTile();
            //            WorldGen.KillTile(brickP.X, brickP.Y, noItem: true);
            //            Main.tile[brickP.X, brickP.Y].ResetToType(skarnBrick);
            //        }

            //    spawnEnd:;
            //}


            //ModContent.GetInstance<CoraliteWorld>().GenMainSkyIsland(player.Center.ToTileCoordinates());
            //ClearWorldTile()

            // Main.NewText(NPC.downedBoss3);

            //int dir = WorldGen.genRand.NextFromList(-1, 1);
            //int cloudBallcount = 4;

            //for (int v = 0; v < cloudBallcount; v++)
            //{
            //    float scale = 1 + v * 0.1f;
            //    float yScale = 1 - v * 0.2f;

            //    int verticalRadius = (int)(8 / 2 * yScale);
            //    if (verticalRadius < 1)
            //        verticalRadius = 1;

            //    int horizontalRadius = (int)(10 / 2 * scale);
            //    if (horizontalRadius < 1)
            //        horizontalRadius = 1;

            //    WorldUtils.Gen(
            //        Main.MouseWorld.ToTileCoordinates() + new Point(horizontalRadius * dir * v, WorldGen.genRand.Next(-verticalRadius, verticalRadius)),
            //        new Shapes.Circle(horizontalRadius, verticalRadius),
            //        Actions.Chain(
            //            new Actions.PlaceTile(TileID.Cloud)
            //            , new Actions.SetFrames()));
            //}

            //int crystalSize = 14;
            //crystalSize /= 2;
            //crystalSize *= 2;
            //crystalSize ++;
            //Point orecenter = Main.MouseWorld.ToTileCoordinates();
            //Point oreP = orecenter + new Vector2(0, -crystalSize / 4f).ToPoint();

            //ShapeData oreData = new ShapeData();
            //int y1 = (int)(crystalSize / 2f * 1.732f);

            //WorldUtils.Gen(
            //oreP,
            //    new Shapes.Tail(crystalSize, new ReLogic.Utilities.Vector2D(0, y1)),
            //    new Actions.Blank().Output(oreData));

            ////生成外面一圈三角形
            //WorldUtils.Gen(
            //oreP,
            //    new ModShapes.InnerOutline(oreData),
            //    Actions.Chain(
            //        new Actions.ClearTile()
            //        , new Actions.PlaceTile(TileID.Mud)
            //, new Actions.SetFrames())
            //);

            //crystalSize /= 2;
            //crystalSize++;
            //oreP = orecenter + new Vector2(1, crystalSize / 4f).ToPoint();

            ////生成里面的小三角形
            //int y = (int)(-crystalSize / 2f * 1.732f);
            //WorldUtils.Gen(
            //oreP,
            //new Shapes.Tail(crystalSize, new ReLogic.Utilities.Vector2D(0, y)),
            //    Actions.Chain(
            //        new Actions.ClearTile()
            //        , new Actions.PlaceTile(TileID.Mud)
            //        , new Actions.SetFrames())
            //    );

            //Point p = Main.MouseWorld.ToTileCoordinates();

            //ShapeData shape = new ShapeData();

            //int radius = 20;

            ////获取形状
            //WorldUtils.Gen(
            //    p,
            //    new Shapes.Circle(radius),
            //    Actions.Chain(
            //        new Modifiers.Dither(0.1f).Output(shape)));

            //Point topLeft = p - new Point(radius , radius );

            //int x = (int)(WorldGen.genRand.NextFloat() * radius * 2);
            //int y = (int)(WorldGen.genRand.NextFloat() * radius);

            //for (int m = 0; m < radius * 2; m++)
            //    for (int n = 0; n < radius * 2; n++)
            //    {
            //        if (!shape.Contains(-radius + m, -radius + n))
            //            continue;

            //        Point currP = topLeft + new Point(m, n);

            //        float mainNoise = ModContent.GetInstance<CoraliteWorld>().MainNoise(new Vector2(x + m, y + n), new Vector2(radius * 2) * 6);
            //        if (mainNoise > 0.8f)
            //        {
            //            //if (Main.tile[currP].WallType > 0)
            //            {
            //                WorldGen.KillWall(currP.X, currP.Y);
            //                WorldGen.PlaceWall(currP.X, currP.Y, WallID.Cave1Echo);
            //            }
            //        }
            //        //else if (WorldGen.genRand.NextBool(5))
            //        //{
            //        //    WorldGen.KillWall(currP.X, currP.Y);
            //        //}
            //    }

            return base.CanUseItem(player);
        }

        /// <summary>
        /// 清空整个世界的物块
        /// </summary>
        public void ClearWorldTile()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                    Main.tile[i, j].Clear(TileDataType.All);
        }

        public static void ExampleStructure(GenerationProgress progress, GameConfiguration configuration)
        {
            //随机选择X位置，最小80是为了防止出地图或是触碰世界边界导致报错
            int x = Main.rand.Next(80, 300);
            int y = 200;

            //地狱就200格高
            for (int i = 30; i < 200; i++)
            {
                //获取物块
                Tile t = Main.tile[x, Main.maxTilesY - i];

                //如果当前位置有实心物块或者有液体（判断一下岩浆）就继续想上找
                if ((t.HasTile && Main.tileSolid[t.TileType]) || t.LiquidAmount > 0)
                    continue;

                bool empty = true;
                for (int j = 1; j < 8; j++)
                {
                    Tile t2 = Main.tile[x, Main.maxTilesY - i - j];
                    if ((t2.HasTile && Main.tileSolid[t2.TileType]) || t2.LiquidAmount > 0)
                    {
                        empty = false;
                        break;
                    }
                }

                if (empty)
                {
                    y = i;
                    break;
                }
            }

            Point origin = new Point(x, Main.maxTilesY - y);

            ShapeData circleData = new ShapeData();
            ShapeData circleExpandData = new ShapeData();
            ShapeData ashRectData = new ShapeData();

            WorldUtils.Gen(
                origin,
                new Shapes.Circle(20),
                new Actions.ClearTile(frameNeighbors: true).Output(circleData));

            WorldUtils.Gen(
                origin,
                new ModShapes.OuterOutline(circleData),
                Actions.Chain(
                    new Modifiers.RectangleMask(-40, 40, 0, 40),
                    new Modifiers.Expand(1),
                    new Modifiers.IsEmpty(),
                    new Actions.PlaceTile(TileID.HellstoneBrick).Output(circleExpandData)));

            int width = 13;
            int height = 26;
            Point origin2 = new Point(origin.X, origin.Y + height / 3);

            WorldUtils.Gen(
                origin2,
                new Shapes.Rectangle(new Rectangle(-width / 2, -height / 2, width, height)),
                Actions.Chain(
                    new Actions.SetTile(TileID.Ash),
                    new Actions.SetFrames(frameNeighbors: true).Output(ashRectData)));

            WorldUtils.Gen(
                origin2,
                new ModShapes.InnerOutline(ashRectData),
                Actions.Chain(
                    new Modifiers.IsSolid(),
                    new Actions.SetTile(TileID.AshGrass),
                    new Actions.SetFrames(frameNeighbors: true)));

            circleData.Subtract(ashRectData, origin, origin2);
            circleData.Subtract(circleExpandData, origin, origin);

            WorldUtils.Gen(
                origin,
                new ModShapes.All(circleData),
                Actions.Chain(
                    new Modifiers.RectangleMask(-40, 40, 0, 40),
                    new Modifiers.IsEmpty(),
                    new Actions.SetLiquid(LiquidID.Lava)));

            //ShapeData shaftShapeData = new ShapeData();
            //WorldUtils.Gen(
            //    new Point(origin.X, surfacePoint.Y + 10),
            //    new Shapes.Rectangle(1, origin.Y - surfacePoint.Y - 9),
            //    Actions.Chain(
            //        new Modifiers.Blotches(2, 0.2),
            //        new Actions.ClearTile().Output(shaftShapeData),
            //        new Modifiers.Expand(1),
            //        new Modifiers.OnlyTiles(TileID.Sand),
            //        new Actions.SetTile(TileID.HardenedSand).Output(shaftShapeData)));

            //WorldUtils.Gen(
            //    new Point(origin.X, surfacePoint.Y + 10),
            //    new ModShapes.All(shaftShapeData),
            //    new Actions.SetFrames(frameNeighbors: true));

            //放置一个肉山圣物（肉山圣物的style是5）
            WorldGen.PlaceObject(origin2.X, origin2.Y - height / 2 - 1, TileID.MasterTrophyBase, true, 6);

            //Dust d=  Dust.NewDustPerfect(new Point(point2.X, point2.Y - height / 2).ToWorldCoordinates(), DustID.Torch, Vector2.Zero, Scale: 5);
            //  d.noGravity = true;
            // 将植物放置在土丘形状的草砖之上。
            WorldUtils.Gen(
                origin2,
                new ModShapes.All(ashRectData),
                Actions.Chain(
                    new Modifiers.Offset(0, -1),
                    new Modifiers.OnlyTiles(TileID.AshGrass),
                    //new Modifiers.Offset(0, -1),
                    new ActionAshGrass()));



            WorldUtils.Gen(
                origin,
                new ModShapes.All(circleData),
                new Actions.PlaceWall(WallID.HellstoneBrick));

            return;
        }
    }

    public class ActionAshGrass : GenAction
    {
        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            //这个_tiles引用的就是Main.tile
            //如果自身这一格没有物块或者顶上的一格有物块就跳过
            if (!_tiles[x, y].HasTile || _tiles[x, y - 1].HasTile)
                return false;

            //下面这个是我用来测试的代码
            //Dust d = Dust.NewDustPerfect(new Point(x, y).ToWorldCoordinates(), DustID.Torch, Vector2.Zero, Scale: 5);
            //d.noGravity = true;

            //放置灰烬草，PlaceTile的参数和PlaceObject类似
            //但是PlaceTile更适合用来放单格物块，虽然放多物块也行就是不太好用
            //具体参数是干什么的看一下它的注释吧
            WorldGen.PlaceTile(x, y - 1, TileID.AshPlants, mute: true);

            //要用这个哦，不然输出图形会出问题
            return UnitApply(origin, x, y, args);
        }
    }
}
