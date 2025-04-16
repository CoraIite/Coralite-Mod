using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
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

            //ExampleStructure(p);
            var a = Main.chest[0];


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

        public void ExampleStructure(Point origin)
        {
            int width = 13;
            int height = 26;

            ShapeData slimeShapeData = new ShapeData();
            ShapeData moundShapeData = new ShapeData();
            Point point = new Point(origin.X, origin.Y);
            Point point2 = new Point(origin.X, origin.Y + height / 3);

            WorldUtils.Gen(
                point,
                new Shapes.Circle(20),//new Shapes.Slime(20, 0.8f, 1f),
                Actions.Chain(
                    //new Modifiers.Blotches(2, 0.4),
                    new Actions.ClearTile(frameNeighbors: true).Output(slimeShapeData)));

            WorldUtils.Gen(
                point2,
                new Shapes.Rectangle(new Rectangle(-width / 2, -height / 2, width, height)),//new Shapes.Mound(14, 14),
                Actions.Chain(
                    //new Modifiers.Blotches(2, 1, 0.8),
                    new Actions.SetTile(TileID.Ash),
                    new Actions.SetFrames(frameNeighbors: true).Output(moundShapeData)));

            WorldUtils.Gen(
                point,
                new ModShapes.OuterOutline(slimeShapeData),
                Actions.Chain(
                    new Modifiers.RectangleMask(-40, 40, 0, 40),
                    new Modifiers.Expand(1),
                    new Modifiers.IsEmpty(),
                    new Actions.PlaceTile(TileID.HellstoneBrick)));

            slimeShapeData.Subtract(moundShapeData, point, point2);

            WorldUtils.Gen(
                point2,
                new ModShapes.InnerOutline(moundShapeData),
                Actions.Chain(
                    new Modifiers.IsSolid(),
                    new Actions.SetTile(TileID.AshGrass),
                    new Actions.SetFrames(frameNeighbors: true)));


            WorldUtils.Gen(
                point,
                new ModShapes.All(slimeShapeData),
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
            WorldGen.PlaceObject(point2.X, point2.Y - height / 2 - 1, TileID.MasterTrophyBase, mute: true, 6);
            //Dust d=  Dust.NewDustPerfect(new Point(point2.X, point2.Y - height / 2).ToWorldCoordinates(), DustID.Torch, Vector2.Zero, Scale: 5);
            //  d.noGravity = true;
            // 将植物放置在土丘形状的草砖之上。
            WorldUtils.Gen(
                point2,
                new ModShapes.All(moundShapeData),
                Actions.Chain(
                    new Modifiers.Offset(0, -1),
                    new Modifiers.OnlyTiles(TileID.AshGrass),
                    //new Modifiers.Offset(0, -1),
                    new ActionAshGrass()));

            WorldUtils.Gen(
                point,
                new ModShapes.All(slimeShapeData),
                Actions.Chain(
                    new Actions.PlaceWall(WallID.HellstoneBrick),
                    new Modifiers.OnlyTiles(TileID.Ash),
                    new Modifiers.Offset(0, 1),
                    new ActionVines(3, 5)));
        }
    }

    public class ActionAshGrass : GenAction
    {
        public override bool Apply(Point origin, int x, int y, params object[] args)
        {
            if (!_tiles[x, y].HasTile || _tiles[x, y - 1].HasTile)
                return false;
            //Dust d = Dust.NewDustPerfect(new Point(x, y).ToWorldCoordinates(), DustID.Torch, Vector2.Zero, Scale: 5);
            //d.noGravity = true;

            //WorldGen.KillWall(x, y-1);
            WorldGen.PlaceTile(x, y - 1, TileID.AshPlants, mute: true);

            return UnitApply(origin, x, y, args);
        }
    }

}
