using Coralite.Content.Items.BotanicalItems.Seeds;
using Coralite.Content.Tiles.Plants;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.GlobalTiles
{
    public class BotanicalGlobalTile : GlobalTile
    {
        public override void RandomUpdate(int i, int j, int type)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!tile.HasTile)
                return;
            Tile upTile = Framing.GetTileSafely(i, j - 1);
            if (upTile.HasTile)
                return;
            
            #region 天空层
            if (j < Main.worldSurface * 0.35f)
            {

                return;
            }
            #endregion

            #region 世界表面层
            if (j < Main.worldSurface)
            {
                #region 沙子
                if (type == TileID.Sand)
                {
                    //饮水棘
                    if (Main.rand.NextBool(250))
                    {
                        WorldGen.Place1x1(i, j - 1, TileType<WaterDrinker>());
                        upTile.TileFrameX = 54;
                        return;
                    }
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
                    if (Main.rand.NextBool(250))
                    {
                        WorldGen.Place1x1(i, j - 1, TileType<CoraliteJungleSpores>());
                        upTile.TileFrameX = 36;
                        return;
                    }

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

                return true; 
            }
            #endregion

            #region 泥土层
            if (j < Main.rockLayer)
            {

                return true;
            }
            #endregion

            #region 地下岩石层
            if (j < Main.maxTilesX - 200)
            {
                #region 丛林植物
                if (type == TileID.JunglePlants || type == TileID.JunglePlants2)
                {
                    if (Main.rand.NextBool(25))
                    {
                        Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
                        Item.NewItem(null, worldPosition, ItemType<JungleBuds>());
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
    }
}
