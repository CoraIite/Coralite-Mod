using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Core
{
    public class CoraliteTileDrawing : ModSystem
    {
        public static MethodInfo DrawMultiTileVinesInWind_Info;
        public static Action<TileDrawing, Vector2, Vector2, int, int, int, int> DrawMultiTileVinesInWind;

        public FieldInfo specialPositions_Info;
        public FieldInfo specialsCount_Info;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            DrawMultiTileVinesInWind_Info = Main.instance.TilesRenderer.GetType().GetMethod("DrawMultiTileVinesInWind", BindingFlags.NonPublic | BindingFlags.Instance);
            DrawMultiTileVinesInWind = (Action<TileDrawing, Vector2, Vector2, int, int, int, int>)Delegate.CreateDelegate(
                typeof(Action<TileDrawing, Vector2, Vector2, int, int, int, int>), DrawMultiTileVinesInWind_Info);

            specialPositions_Info = Main.instance.TilesRenderer.GetType().GetField("_specialPositions", BindingFlags.NonPublic | BindingFlags.Instance);
            specialsCount_Info = Main.instance.TilesRenderer.GetType().GetField("_specialsCount", BindingFlags.NonPublic | BindingFlags.Instance);

            On_TileDrawing.DrawMultiTileVines += On_TileDrawing_DrawMultiTileVines;
        }

        private void On_TileDrawing_DrawMultiTileVines(On_TileDrawing.orig_DrawMultiTileVines orig, TileDrawing self)
        {
            //orig.Invoke(self);

            Point[][] _specialPositions = (Point[][])specialPositions_Info?.GetValue(Main.instance.TilesRenderer);
            int[] _specialsCount = (int[])specialsCount_Info?.GetValue(Main.instance.TilesRenderer);

            if (_specialPositions == null || _specialsCount == null)
            {
                orig.Invoke(self);
                return;
            }

            Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
            Vector2 zero = Vector2.Zero;
            int num = 5;
            int num2 = _specialsCount[num];
            for (int i = 0; i < num2; i++)
            {
                Point point = _specialPositions[num][i];
                int x = point.X;
                int y = point.Y;
                int sizeX = 1;
                int sizeY = 1;
                Tile tile = Main.tile[x, y];
                if (tile != null && tile.HasTile)
                {
                    if (CoraliteSets.Tiles.SpecialDraw[tile.TileType])
                    {
                        TileLoader.SpecialDraw(tile.TileType, x, y, Main.spriteBatch);
                        continue;
                    }

                    switch (Main.tile[x, y].TileType)
                    {
                        case TileID.Chandeliers:
                            sizeX = 3;
                            sizeY = 3;
                            break;
                        case TileID.Pigronata:
                            sizeX = 4;
                            sizeY = 3;
                            break;
                        case TileID.HangingLanterns:
                        case TileID.FireflyinaBottle:
                        case TileID.LightningBuginaBottle:
                        case TileID.SoulBottles:
                        case TileID.LavaflyinaBottle:
                        case TileID.ShimmerflyinaBottle:
                            sizeX = 1;
                            sizeY = 2;
                            break;
                        case TileID.Banners:
                            sizeX = 1;
                            sizeY = 3;
                            break;
                        case TileID.ChineseLanterns:
                        case TileID.DiscoBall:
                        case TileID.BeeHive:
                            sizeX = 2;
                            sizeY = 2;
                            break;
                        case TileID.WarTableBanner:
                        case TileID.PotsSuspended:
                        case TileID.BrazierSuspended:
                            sizeX = 2;
                            sizeY = 3;
                            break;
                        default:
                            if (TileID.Sets.MultiTileSway[tile.TileType])
                            {
                                TileObjectData tileObjectData = TileObjectData.GetTileData(tile);
                                sizeX = tileObjectData.Width;
                                sizeY = tileObjectData.Height;
                            }

                            break;
                    }

                    DrawMultiTileVinesInWind(self, unscaledPosition, zero, x, y, sizeX, sizeY);
                }
            }
        }

        //public static bool IsVisible(Tile tile)
        //{
        //    bool flag = tile.IsTileInvisible;

        //    if (flag)
        //        return shouldShowInvisibleBlocks;

        //    return true;
        //}

        //public enum TileCounterType
        //{
        //    Tree,
        //    DisplayDoll,
        //    HatRack,
        //    WindyGrass,
        //    MultiTileGrass,
        //    MultiTileVine,
        //    Vine,
        //    BiomeGrass,
        //    VoidLens,
        //    ReverseVine,
        //    TeleportationPylon,
        //    MasterTrophy,
        //    AnyDirectionalGrass,
        //    Count
        //}
    }
}
