using Coralite.Content.Items.BossSummons;
using Coralite.Content.Items.BotanicalItems.Seeds;
using Coralite.Content.Tiles.Plants;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.GlobalTiles
{
    public class BotanicalGlobalTile : GlobalTile
    {
        public override void SetStaticDefaults()
        {
            //用于修复原版的一个BUG，那就是透明方块会阻挡楼梯绘制，而不透明方块却不会阻挡
            for (int i = 0; i < TileID.Count; i++)
            {
                if (i == 386 || i == 387 || i == 54 || i == 541)
                {
                    TileID.Sets.BlocksStairs[i] = false;
                    TileID.Sets.BlocksStairsAbove[i] = false;
                    continue;
                }

                TileID.Sets.BlocksStairs[i] = true;
                TileID.Sets.BlocksStairsAbove[i] = true;
            }
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            #region 天空层
            if (j < Main.worldSurface * 0.35f)
            {

                return;
            }
            #endregion

            #region 世界表面层
            if (j < Main.worldSurface)
            {
                #region 草
                if (type == TileID.Grass)
                {

                    return;
                }
                #endregion
                #region 沙子
                if (type == TileID.Sand)
                {
                    //饮水棘
                    if (Main.rand.NextBool(3000))
                        if (CanPlace(i, j, out Tile upTile))
                        {
                            WorldGen.Place1x1(i, j - 1, TileType<WaterDrinker>());
                            upTile.TileFrameX = 54;
                            return;
                        }
                    return;
                }
                #endregion
                #region 雪块&冰块
                if (type == TileID.SnowBlock || type == TileID.IceBlock)
                {
                    //冷水花
                    if (Main.rand.NextBool(3000))
                        if (CanPlace(i, j, out Tile upTile))
                        {
                            WorldGen.Place1x1(i, j - 1, TileType<PileaNotata>());
                            //upTile.TileFrameX = 54;
                            return;
                        }
                    return;
                }
                #endregion
                return;
            }
            #endregion

            #region 泥土层
            if (j < Main.rockLayer)
            {

                return;
            }
            #endregion

            #region 地下岩石层
            if (j < Main.maxTilesX - 200)
            {
                #region 丛林草
                if (type == TileID.JungleGrass)
                {
                    //丛林芽孢
                    if (Main.rand.NextBool(2500))
                        if (CanPlace(i, j, out Tile upTile))
                        {
                            WorldGen.Place1x1(i, j - 1, TileType<CoraliteJungleSpores>());
                            upTile.TileFrameX = 36;
                            return;
                        }
                    return;
                }

                #endregion
                #region 雪块&冰块
                if (type == TileID.SnowBlock || type == TileID.IceBlock)
                {
                    //寒霜冰草
                    if (Main.rand.NextBool(3000))
                        if (CanPlace(i, j, out Tile upTile))
                        {
                            WorldGen.Place1x1(i, j - 1, TileType<AgropyronFrozen>());
                            upTile.TileFrameX = 52;
                            return;
                        }
                    return;
                }
                #endregion
                #region 沙石&硬化沙
                if (type == TileID.Sandstone || type == TileID.HardenedSand)
                {
                    //刺蕨
                    if (Main.rand.NextBool(3000))
                        if (CanPlace(i, j, out Tile upTile))
                        {
                            WorldGen.Place1x1(i, j - 1, TileType<EgenolfiaSandy>());
                            upTile.TileFrameX = 56;
                            return;
                        }
                    return;
                }
                #endregion
                #region 蘑菇草
                if (type == TileID.MushroomGrass)
                {
                    //吸光蘑菇
                    if (Main.rand.NextBool(2500))
                        if (CanPlace(i, j, out Tile upTile) && Main.hardMode)
                        {
                            WorldGen.Place1x1(i, j - 1, TileType<GloomMushroom>());
                            upTile.TileFrameX = 18;
                            return;
                        }
                    return;
                }
                #endregion
                return;
            }
            #endregion

            #region 地狱层
            #endregion
        }

        public override bool Drop(int i, int j, int type)
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
                #region 木头
                if (type == TileID.Trees)
                {
                    //树生树树果
                    if (Main.rand.NextBool(30))
                    {
                        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<TreeJokeSeed>());
                        return false;
                    }
                }
                #endregion
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
                #region 丛林植物
                if (type == TileID.JunglePlants || type == TileID.JunglePlants2)
                {
                    //西瓜
                    if (Main.rand.NextBool(30))
                    {
                        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<WatermelonSeed>());
                        return false;
                    }
                }
                #endregion

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
                #region 丛林植物
                if (type == TileID.JunglePlants || type == TileID.JunglePlants2)
                {
                    //丛林芽孢
                    if (Main.rand.NextBool(30))
                    {
                        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                        Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<JungleBuds>());
                        return false;
                    }
                }
                #endregion
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
