using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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
            //Point pos = Main.MouseWorld.ToTileCoordinates();
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

            int crystalSize = 14;
            crystalSize /= 2;
            crystalSize *= 2;
            crystalSize ++;
            Point orecenter = Main.MouseWorld.ToTileCoordinates();
            Point oreP = orecenter + new Vector2(0, -crystalSize / 4f).ToPoint();

            ShapeData oreData = new ShapeData();
            int y1 = (int)(crystalSize / 2f * 1.732f);

            WorldUtils.Gen(
            oreP,
                new Shapes.Tail(crystalSize, new ReLogic.Utilities.Vector2D(0, y1)),
                new Actions.Blank().Output(oreData));

            //生成外面一圈三角形
            WorldUtils.Gen(
            oreP,
                new ModShapes.InnerOutline(oreData),
                Actions.Chain(
                    new Actions.ClearTile()
                    , new Actions.PlaceTile(TileID.Mud)
            , new Actions.SetFrames())
            );

            crystalSize /= 2;
            crystalSize++;
            oreP = orecenter + new Vector2(1, crystalSize / 4f).ToPoint();

            //生成里面的小三角形
            int y = (int)(-crystalSize / 2f * 1.732f);
            WorldUtils.Gen(
            oreP,
            new Shapes.Tail(crystalSize, new ReLogic.Utilities.Vector2D(0, y)),
                Actions.Chain(
                    new Actions.ClearTile()
                    , new Actions.PlaceTile(TileID.Mud)
                    , new Actions.SetFrames())
                );

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
    }
}
