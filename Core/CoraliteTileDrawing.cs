using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;

namespace Coralite.Core
{
    public class CoraliteTileDrawing : ModSystem
    {
        public static double sunflowerWindCounter;
        public static bool shouldShowInvisibleBlocks;

        private MethodInfo DrawMultiTileVinesInWind_Info;
        private Action<TileDrawing, Vector2, Vector2, int, int, int, int> DrawMultiTileVinesInWind;
        public static MethodInfo GetHighestWindGridPushComplex_Info;
        public static Func<TileDrawing, int, int, int, int, int, float, int, bool, float> GetHighestWindGridPushComplex;

        public FieldInfo specialPositions_Info;
        public FieldInfo specialsCount_Info;

        public void AddSpecialPoint(int x, int y, TileCounterType type)
        {
            Point[][] _specialPositions = (Point[][])specialPositions_Info?.GetValue(Main.instance.TilesRenderer);
            int[] _specialsCount = (int[])specialsCount_Info?.GetValue(Main.instance.TilesRenderer);

            if (_specialPositions != null && _specialsCount != null)
                _specialPositions[(int)type][_specialsCount[(int)type]++] = new Point(x, y);
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GetHighestWindGridPushComplex_Info = Main.instance.TilesRenderer.GetType().GetMethod("GetHighestWindGridPushComplex", BindingFlags.NonPublic | BindingFlags.Instance);
            GetHighestWindGridPushComplex = (Func<TileDrawing, int, int, int, int, int, float, int, bool, float>)Delegate.CreateDelegate(
                typeof(Func<TileDrawing, int, int, int, int, int, float, int, bool, float>), GetHighestWindGridPushComplex_Info);

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
                return;

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
                    if (tile.TileType >= TileID.Count)
                        TileLoader.SpecialDraw(tile.TileType, x, y, Main.spriteBatch);
                    else
                    {
                        switch (Main.tile[x, y].TileType)
                        {
                            case 34:
                                sizeX = 3;
                                sizeY = 3;
                                break;
                            case 454:
                                sizeX = 4;
                                sizeY = 3;
                                break;
                            case 42:
                            case 270:
                            case 271:
                            case 572:
                            case 581:
                            case 660:
                                sizeX = 1;
                                sizeY = 2;
                                break;
                            case 91:
                                sizeX = 1;
                                sizeY = 3;
                                break;
                            case 95:
                            case 126:
                            case 444:
                                sizeX = 2;
                                sizeY = 2;
                                break;
                            case 465:
                            case 591:
                            case 592:
                                sizeX = 2;
                                sizeY = 3;
                                break;
                        }

                        DrawMultiTileVinesInWind(self, unscaledPosition, zero, x, y, sizeX, sizeY);
                    }
                }
            }

        }

        public override void PostUpdateEverything()
        {
            if (!Main.dedServ)
            {
                double num = Math.Abs(Main.WindForVisuals);
                num = Utils.GetLerpValue(0.08f, 1.2f, (float)num, clamped: true);
                sunflowerWindCounter += 1.0 / 420.0 + 1.0 / 420.0 * num * 5.0;
                shouldShowInvisibleBlocks = Main.ShouldShowInvisibleWalls();

            }
        }

        public static bool IsVisible(Tile tile)
        {
            bool flag = tile.IsTileInvisible;

            if (flag)
                return shouldShowInvisibleBlocks;

            return true;
        }

        public enum TileCounterType
        {
            Tree,
            DisplayDoll,
            HatRack,
            WindyGrass,
            MultiTileGrass,
            MultiTileVine,
            Vine,
            BiomeGrass,
            VoidLens,
            ReverseVine,
            TeleportationPylon,
            MasterTrophy,
            AnyDirectionalGrass,
            Count
        }

    }
}
