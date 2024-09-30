using Coralite.Content.Items.BossSummons;
using Coralite.Content.Items.CoreKeeper;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.Pets;
using Coralite.Content.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Tiles.GlobalTiles
{
    public class CoraliteGlobalTile : GlobalTile
    {
        //public override void RandomUpdate(int i, int j, int type)
        //{
        //    #region 天空层
        //    if (j < Main.worldSurface * 0.35f)
        //    {

        //        return;
        //    }
        //    #endregion

        //    #region 世界表面层
        //    if (j < Main.worldSurface)
        //    {
        //        #region 草
        //        if (type == TileID.Grass)
        //        {

        //            return;
        //        }
        //        #endregion
        //        #region 沙子
        //        if (type == TileID.Sand)
        //        {
        //            //饮水棘
        //            if (Main.rand.NextBool(3000))
        //                if (CanPlace(i, j, out Tile upTile))
        //                {
        //                    WorldGen.Place1x1(i, j - 1, TileType<WaterDrinker>());
        //                    upTile.TileFrameX = 54;
        //                    return;
        //                }
        //            return;
        //        }
        //        #endregion
        //        #region 雪块
        //        if (type == TileID.SnowBlock)
        //        {
        //            //冷水花
        //            if (Main.rand.NextBool(3000))
        //                if (CanPlace(i, j, out _))
        //                {
        //                    WorldGen.Place1x1(i, j - 1, TileType<PileaNotata>());
        //                    //upTile.TileFrameX = 54;
        //                    return;
        //                }
        //            //雪融花
        //            if (Main.rand.NextBool(2800))
        //                if (CanPlace(i, j, out Tile upTile))
        //                {
        //                    WorldGen.Place1x1(i, j - 1, TileType<SnoowFlower>());
        //                    upTile.TileFrameX = 150;
        //                    return;
        //                }
        //            return;
        //        }
        //        #endregion
        //        #region 冰块
        //        if (type == TileID.IceBlock)
        //        {
        //            //冷水花
        //            if (Main.rand.NextBool(3000))
        //                if (CanPlace(i, j, out _))
        //                {
        //                    WorldGen.Place1x1(i, j - 1, TileType<PileaNotata>());
        //                    //upTile.TileFrameX = 54;
        //                    return;
        //                }
        //        }
        //        #endregion
        //        return;
        //    }
        //    #endregion

        //    #region 泥土层
        //    if (j < Main.rockLayer)
        //    {

        //        return;
        //    }
        //    #endregion

        //    #region 地下岩石层
        //    if (j < Main.maxTilesX - 200)
        //    {
        //        #region 丛林草
        //        if (type == TileID.JungleGrass)
        //        {
        //            //丛林芽孢
        //            if (Main.rand.NextBool(2500))
        //                if (CanPlace(i, j, out Tile upTile))
        //                {
        //                    WorldGen.Place1x1(i, j - 1, TileType<CoraliteJungleSpores>());
        //                    upTile.TileFrameX = 36;
        //                    return;
        //                }
        //            return;
        //        }

        //        #endregion
        //        #region 雪块&冰块
        //        if (type == TileID.SnowBlock || type == TileID.IceBlock)
        //        {
        //            //寒霜冰草
        //            if (Main.rand.NextBool(3000))
        //                if (CanPlace(i, j, out Tile upTile))
        //                {
        //                    WorldGen.Place1x1(i, j - 1, TileType<AgropyronFrozen>());
        //                    upTile.TileFrameX = 52;
        //                    return;
        //                }
        //            return;
        //        }
        //        #endregion
        //        #region 沙石&硬化沙
        //        if (type == TileID.Sandstone || type == TileID.HardenedSand)
        //        {
        //            //刺蕨
        //            if (Main.rand.NextBool(3000))
        //                if (CanPlace(i, j, out Tile upTile))
        //                {
        //                    WorldGen.Place1x1(i, j - 1, TileType<EgenolfiaSandy>());
        //                    upTile.TileFrameX = 56;
        //                    return;
        //                }
        //            return;
        //        }
        //        #endregion
        //        #region 蘑菇草
        //        if (type == TileID.MushroomGrass)
        //        {
        //            //吸光蘑菇
        //            if (Main.rand.NextBool(2500))
        //                if (CanPlace(i, j, out Tile upTile) && Main.hardMode)
        //                {
        //                    WorldGen.Place1x1(i, j - 1, TileType<GloomMushroom>());
        //                    upTile.TileFrameX = 18;
        //                    return;
        //                }
        //            return;
        //        }
        //        #endregion
        //        return;
        //    }
        //    #endregion

        //    #region 地狱层
        //    #endregion
        //}

        public override bool CanDrop(int i, int j, int type)/* tModPorter Suggestion: Use CanDrop to decide if items can drop, use this method to drop additional items. See documentation. */
        {
            #region 天空层
            if (j < Main.worldSurface * 0.35f)
            {

                return true;
            }
            #endregion

            #region 世界表面层
            if (j < Main.worldSurface)
            {
                //#region 木头
                //if (type == TileID.Trees)
                //{
                //    //树生树树果
                //    if (Main.rand.NextBool(30))
                //    {
                //        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                //        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<TreeJokeSeed>());
                //        return false;
                //    }
                //}
                //#endregion
                #region 各种宝石树
                if (type == TileID.TreeTopaz || type == TileID.TreeAmethyst || type == TileID.TreeSapphire || type == TileID.TreeEmerald || type == TileID.TreeRuby || type == TileID.TreeDiamond || type == TileID.TreeAmber)
                {
                    //赤色果实
                    if (Main.rand.NextBool(20))
                    {
                        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<RedBerry>());
                        return false;
                    }
                }
                #endregion
                //#region 丛林植物
                //if (type == TileID.JunglePlants || type == TileID.JunglePlants2)
                //{
                //    //西瓜
                //    if (Main.rand.NextBool(30))
                //    {
                //        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                //        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<WatermelonSeed>());
                //        return false;
                //    }
                //}
                //#endregion

                return true;
            }
            #endregion

            #region 泥土层
            if (j < Main.rockLayer)
            {
                #region 各种宝石树
                if (type == TileID.TreeTopaz || type == TileID.TreeAmethyst || type == TileID.TreeSapphire || type == TileID.TreeEmerald || type == TileID.TreeRuby || type == TileID.TreeDiamond || type == TileID.TreeAmber)
                {
                    //赤色果实
                    if (Main.rand.NextBool(20))
                    {
                        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<RedBerry>());
                        return false;
                    }
                }
                #endregion
                return true;
            }
            #endregion

            #region 地下岩石层
            if (j < Main.maxTilesX - 200)
            {
                //#region 丛林植物
                //if (type == TileID.JunglePlants || type == TileID.JunglePlants2)
                //{
                //    //丛林芽孢
                //    if (Main.rand.NextBool(30))
                //    {
                //        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                //        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<JungleBuds>());
                //        return false;
                //    }
                //}
                //#endregion
                #region 各种宝石树
                if (type == TileID.TreeTopaz || type == TileID.TreeAmethyst || type == TileID.TreeSapphire || type == TileID.TreeEmerald || type == TileID.TreeRuby || type == TileID.TreeDiamond || type == TileID.TreeAmber)
                {
                    //赤色果实
                    if (Main.rand.NextBool(20))
                    {
                        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<RedBerry>());
                        return false;
                    }
                }
                #endregion
                return true;
            }
            #endregion

            #region 地狱层
            #endregion

            return true;
        }

        public override void Drop(int i, int j, int type)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            switch (tile.TileType)
            {
                default:
                    break;
                case TileID.Trees:
                    WorldGen.GetTreeBottom(i, j, out int x, out int y);
                    Tile bottomTile = Framing.GetTileSafely(x, y);
                    TreeTypes tree = WorldGen.GetTreeType(bottomTile.TileType);

                    switch (tree)
                    {
                        case TreeTypes.None:
                            break;
                        case TreeTypes.Forest:
                            break;
                        case TreeTypes.Corrupt:
                            break;
                        case TreeTypes.Mushroom:
                            break;
                        case TreeTypes.Crimson:
                            break;
                        case TreeTypes.Jungle:
                            break;
                        case TreeTypes.Snow:
                            break;
                        case TreeTypes.Hallowed:
                            break;
                        case TreeTypes.Palm:
                            break;
                        case TreeTypes.PalmCrimson:
                            break;
                        case TreeTypes.PalmCorrupt:
                            break;
                        case TreeTypes.PalmHallowed:
                            break;
                        case TreeTypes.Ash:
                            break;
                        default:
                            break;
                    }
                    break;
                case TileID.PalmTree:
                    {
                        if (CoraliteWorld.CoralCatWorld)
                        {
                            if (Main.rand.NextBool(8))
                                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, ItemID.GillsPotion);
                            if (Main.rand.NextBool(4))
                                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, ItemID.CoralTorch, Main.rand.Next(1, 5));
                        }
                    }
                    break;
                case TileID.VanityTreeSakura:
                    if (Main.hardMode && Main.rand.NextBool(1, 400))
                        Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, ItemType<CrystalBlossomShards>());
                    break;
                case TileID.Heart:
                    if (Main.rand.NextBool(15))
                        Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, ItemType<HeartBerryNecklace>());
                    break;

                case TileID.Sapphire:
                case TileID.Ruby:
                case TileID.Emerald:
                case TileID.Topaz:
                case TileID.Amethyst:
                case TileID.Diamond:
                case TileID.DesertFossil:
                    if (Main.rand.NextBool(12, 100))
                        Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, ItemType<PrimaryRoughGemstone>());
                    break;
                case TileID.Crystals:
                    if (Main.rand.NextBool(15, 100))
                        Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, ItemType<SeniorRoughGemstone>());
                    break;
            }
        }

        public bool CanPlace(int i, int j, out Tile upTile, bool shouldCheckLiquid = true)
        {
            upTile = Framing.GetTileSafely(i, j - 1);
            if (upTile.HasTile)
                return false;
            if (shouldCheckLiquid)
                if (upTile.CheckingLiquid)
                    return false;

            return true;
        }
    }
}
