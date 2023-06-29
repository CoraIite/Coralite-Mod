using Coralite.Content.Tiles.Magike;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core
{
    /*
     * 这里的码是来自厄厄的
     * 部分码其实需要改一改，比方说没什么意义的额外补充贴图
     * 但是.......改起来难度过于地高了，所以还算放弃了
     */
    public class TileFraming:ModSystem
    {
        public static bool[][] tileMergeTypes;

        public override void OnModLoad()
        {
            tileMergeTypes = new bool[TileLoader.TileCount][];
            for (int i = 0; i < tileMergeTypes.Length; i++)
            {
                tileMergeTypes[i] = new bool[TileLoader.TileCount];
            }

            tileMergeTypes[TileID.Dirt][ModContent.TileType<BasaltTile>()] = true;
            tileMergeTypes[ModContent.TileType<BasaltTile>()][ModContent.TileType<MagikeCrystalBlockTile>()] = true;
        }

        public override void Unload()
        {
            tileMergeTypes = null;
        }

        private enum Similarity
        {
            Same,
            MergeLink,
            None
        }

        public static void SetFrameAt(int x, int y, int frameX, int frameY)
        {
            Tile tile = Main.tile[x, y];
            if (tile != null)
            {
                Main.tile[x, y].TileFrameX = (short)frameX;
                Main.tile[x, y].TileFrameY = (short)frameY;
            }
        }

        private static Similarity GetSimilarity(Tile check, int myType, int mergeType)
        {
            if (!check.HasTile)
            {
                return TileFraming.Similarity.None;
            }
            if ((int)(check.TileType) == myType || Main.tileMerge[myType][(int)(check.TileType)])
            {
                return TileFraming.Similarity.Same;
            }
            if ((int)(check.TileType) == mergeType)
            {
                return TileFraming.Similarity.MergeLink;
            }
            return TileFraming.Similarity.None;
        }

        public static void CustomMergeFrame(int x, int y, int myType, int mergeType, bool myTypeBrimFrame = false, bool mergeTypeBrimFrame = false, bool overrideBrimStates = false)
        {
            if (x < 0 || x >= Main.maxTilesX)
            {
                return;
            }
            if (y < 0 || y >= Main.maxTilesY)
            {
                return;
            }
            bool forceSameUp = false;
            bool forceSameDown = false;
            bool forceSameLeft = false;
            bool forceSameRight = false;
            Tile north = Main.tile[x, y - 1];
            Tile south = Main.tile[x, y + 1];
            Tile west = Main.tile[x - 1, y];
            Tile east = Main.tile[x + 1, y];
            bool flag;
            bool flag2;
            bool flag3;
            if (north != null && north.HasTile && tileMergeTypes[north.TileType ][myType])
                CustomMergeFrameExplicit(x, y - 1, north.TileType, myType, out flag, out flag2, out flag3, out forceSameUp, false, false, false, false, false, mergeTypeBrimFrame, true);
            if (west != null && west.HasTile && tileMergeTypes[west.TileType ][myType])
                CustomMergeFrameExplicit(x - 1, y, west.TileType, myType, out flag3, out flag2, out forceSameLeft, out flag, false, false, false, false, false, mergeTypeBrimFrame, true);
            if (east != null && east.HasTile && tileMergeTypes[east.TileType][myType])
                CustomMergeFrameExplicit(x + 1, y, (int)(east.TileType), myType, out flag, out forceSameRight, out flag2, out flag3, false, false, false, false, false, mergeTypeBrimFrame, true);
            if (south != null && south.HasTile && tileMergeTypes[south.TileType ][myType])
                CustomMergeFrameExplicit(x, y + 1, (int)(south.TileType), myType, out forceSameDown, out flag3, out flag2, out flag, false, false, false, false, false, mergeTypeBrimFrame, false);
            bool flag4;
            CustomMergeFrameExplicit(x, y, myType, mergeType, out flag, out flag2, out flag3, out flag4, forceSameDown, forceSameUp, forceSameLeft, forceSameRight, true, myTypeBrimFrame, overrideBrimStates);
        }

        private static void CustomMergeConditionalTree(int x, int y, int randomFrame, Similarity leftSim, TileFraming.Similarity rightSim, TileFraming.Similarity upSim, TileFraming.Similarity downSim, TileFraming.Similarity topLeftSim, TileFraming.Similarity topRightSim, TileFraming.Similarity bottomLeftSim, TileFraming.Similarity bottomRightSim, ref bool mergedLeft, ref bool mergedRight, ref bool mergedUp, ref bool mergedDown)
        {
            if (leftSim == TileFraming.Similarity.None)
            {
                if (upSim == TileFraming.Similarity.Same)
                {
                    if (downSim == TileFraming.Similarity.Same)
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            TileFraming.SetFrameAt(x, y, 0, 18 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            mergedRight = true;
                            TileFraming.SetFrameAt(x, y, 234 + 18 * randomFrame, 36);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 90, 18 * randomFrame);
                        return;
                    }
                    else if (downSim == TileFraming.Similarity.MergeLink)
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            mergedDown = true;
                            TileFraming.SetFrameAt(x, y, 72, 90 + 18 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedDown = true;
                        TileFraming.SetFrameAt(x, y, 126, 90 + 18 * randomFrame);
                        return;
                    }
                    else
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            TileFraming.SetFrameAt(x, y, 36 * randomFrame, 72);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                        return;
                    }
                }
                else if (upSim == TileFraming.Similarity.MergeLink)
                {
                    if (downSim == TileFraming.Similarity.Same)
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            mergedUp = true;
                            TileFraming.SetFrameAt(x, y, 72, 144 + 18 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                            return;
                        }
                        mergedUp = true;
                        TileFraming.SetFrameAt(x, y, 126, 144 + 18 * randomFrame);
                        return;
                    }
                    else if (downSim == TileFraming.Similarity.MergeLink)
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            TileFraming.SetFrameAt(x, y, 162, 18 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            TileFraming.SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedUp = true;
                        mergedDown = true;
                        TileFraming.SetFrameAt(x, y, 108, 216 + 18 * randomFrame);
                        return;
                    }
                    else
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            TileFraming.SetFrameAt(x, y, 162, 18 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            TileFraming.SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedUp = true;
                        TileFraming.SetFrameAt(x, y, 108, 144 + 18 * randomFrame);
                        return;
                    }
                }
                else if (downSim == TileFraming.Similarity.Same)
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        TileFraming.SetFrameAt(x, y, 36 * randomFrame, 54);
                        return;
                    }
                    TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                    return;
                }
                else if (downSim == TileFraming.Similarity.MergeLink)
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        TileFraming.SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    if (rightSim == TileFraming.Similarity.MergeLink)
                    {
                        TileFraming.SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    mergedDown = true;
                    TileFraming.SetFrameAt(x, y, 108, 90 + 18 * randomFrame);
                    return;
                }
                else
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        TileFraming.SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    if (rightSim == TileFraming.Similarity.MergeLink)
                    {
                        mergedRight = true;
                        TileFraming.SetFrameAt(x, y, 54 + 18 * randomFrame, 234);
                        return;
                    }
                    TileFraming.SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                    return;
                }
            }
            else if (leftSim == TileFraming.Similarity.MergeLink)
            {
                if (upSim == TileFraming.Similarity.Same)
                {
                    if (downSim == TileFraming.Similarity.Same)
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            mergedLeft = true;
                            TileFraming.SetFrameAt(x, y, 162, 126 + 18 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            mergedLeft = true;
                            mergedRight = true;
                            TileFraming.SetFrameAt(x, y, 180, 126 + 18 * randomFrame);
                            return;
                        }
                        mergedLeft = true;
                        TileFraming.SetFrameAt(x, y, 234 + 18 * randomFrame, 54);
                        return;
                    }
                    else if (downSim == TileFraming.Similarity.MergeLink)
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            mergedLeft = (mergedDown = true);
                            TileFraming.SetFrameAt(x, y, 36, 108 + 36 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            mergedLeft = (mergedRight = (mergedDown = true));
                            TileFraming.SetFrameAt(x, y, 198, 144 + 18 * randomFrame);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                        return;
                    }
                    else
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            mergedLeft = true;
                            TileFraming.SetFrameAt(x, y, 18 * randomFrame, 216);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                        return;
                    }
                }
                else if (upSim == TileFraming.Similarity.MergeLink)
                {
                    if (downSim == TileFraming.Similarity.Same)
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            mergedUp = (mergedLeft = true);
                            TileFraming.SetFrameAt(x, y, 36, 90 + 36 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            mergedLeft = (mergedRight = (mergedUp = true));
                            TileFraming.SetFrameAt(x, y, 198, 90 + 18 * randomFrame);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                        return;
                    }
                    else if (downSim == TileFraming.Similarity.MergeLink)
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            mergedUp = (mergedLeft = (mergedDown = true));
                            TileFraming.SetFrameAt(x, y, 216, 90 + 18 * randomFrame);
                            return;
                        }
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            mergedDown = (mergedLeft = (mergedRight = (mergedUp = true)));
                            TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 198);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    else
                    {
                        if (rightSim == TileFraming.Similarity.Same)
                        {
                            TileFraming.SetFrameAt(x, y, 162, 18 * randomFrame);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                }
                else if (downSim == TileFraming.Similarity.Same)
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        mergedLeft = true;
                        TileFraming.SetFrameAt(x, y, 18 * randomFrame, 198);
                        return;
                    }
                    TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                    return;
                }
                else if (downSim == TileFraming.Similarity.MergeLink)
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        TileFraming.SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    TileFraming.SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                    return;
                }
                else
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        mergedLeft = true;
                        TileFraming.SetFrameAt(x, y, 18 * randomFrame, 252);
                        return;
                    }
                    if (rightSim == TileFraming.Similarity.MergeLink)
                    {
                        mergedRight = (mergedLeft = true);
                        TileFraming.SetFrameAt(x, y, 162 + 18 * randomFrame, 198);
                        return;
                    }
                    mergedLeft = true;
                    TileFraming.SetFrameAt(x, y, 18 * randomFrame, 234);
                    return;
                }
            }
            else if (upSim == TileFraming.Similarity.Same)
            {
                if (downSim == TileFraming.Similarity.Same)
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        if (topLeftSim != TileFraming.Similarity.MergeLink && topRightSim != TileFraming.Similarity.MergeLink && bottomLeftSim != TileFraming.Similarity.MergeLink && bottomRightSim != TileFraming.Similarity.MergeLink)
                        {
                            if (topLeftSim == TileFraming.Similarity.Same)
                            {
                                if (topRightSim == TileFraming.Similarity.Same)
                                {
                                    if (bottomLeftSim == TileFraming.Similarity.Same)
                                    {
                                        TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                        return;
                                    }
                                    if (bottomRightSim == TileFraming.Similarity.Same)
                                    {
                                        TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                        return;
                                    }
                                    TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 36);
                                    return;
                                }
                                else if (bottomLeftSim == TileFraming.Similarity.Same)
                                {
                                    if (bottomRightSim != TileFraming.Similarity.Same)
                                    {
                                        TileFraming.SetFrameAt(x, y, 198, 18 * randomFrame);
                                        return;
                                    }
                                    if (topRightSim == TileFraming.Similarity.MergeLink)
                                    {
                                        TileFraming.SetFrameAt(x, y, 0, 108 + 36 * randomFrame);
                                        return;
                                    }
                                    TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                            }
                            else if (topLeftSim == TileFraming.Similarity.None)
                            {
                                if (topRightSim != TileFraming.Similarity.Same)
                                {
                                    TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                if (bottomRightSim == TileFraming.Similarity.Same)
                                {
                                    TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                return;
                            }
                            TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                            return;
                        }
                        if (bottomRightSim == TileFraming.Similarity.MergeLink)
                        {
                            TileFraming.SetFrameAt(x, y, 0, 90 + 36 * randomFrame);
                            return;
                        }
                        if (bottomLeftSim == TileFraming.Similarity.MergeLink)
                        {
                            TileFraming.SetFrameAt(x, y, 18, 90 + 36 * randomFrame);
                            return;
                        }
                        if (topRightSim == TileFraming.Similarity.MergeLink)
                        {
                            TileFraming.SetFrameAt(x, y, 0, 108 + 36 * randomFrame);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 18, 108 + 36 * randomFrame);
                        return;
                    }
                    else
                    {
                        if (rightSim == TileFraming.Similarity.MergeLink)
                        {
                            mergedRight = true;
                            TileFraming.SetFrameAt(x, y, 144, 126 + 18 * randomFrame);
                            return;
                        }
                        TileFraming.SetFrameAt(x, y, 72, 18 * randomFrame);
                        return;
                    }
                }
                else if (downSim == TileFraming.Similarity.MergeLink)
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        mergedDown = true;
                        TileFraming.SetFrameAt(x, y, 144 + 18 * randomFrame, 90);
                        return;
                    }
                    if (rightSim == TileFraming.Similarity.MergeLink)
                    {
                        mergedDown = (mergedRight = true);
                        TileFraming.SetFrameAt(x, y, 54, 108 + 36 * randomFrame);
                        return;
                    }
                    mergedDown = true;
                    TileFraming.SetFrameAt(x, y, 90, 90 + 18 * randomFrame);
                    return;
                }
                else
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 36);
                        return;
                    }
                    if (rightSim == TileFraming.Similarity.MergeLink)
                    {
                        mergedRight = true;
                        TileFraming.SetFrameAt(x, y, 54 + 18 * randomFrame, 216);
                        return;
                    }
                    TileFraming.SetFrameAt(x, y, 18 + 36 * randomFrame, 72);
                    return;
                }
            }
            else if (upSim == TileFraming.Similarity.MergeLink)
            {
                if (downSim == TileFraming.Similarity.Same)
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        mergedUp = true;
                        TileFraming.SetFrameAt(x, y, 144 + 18 * randomFrame, 108);
                        return;
                    }
                    if (rightSim == TileFraming.Similarity.MergeLink)
                    {
                        mergedRight = (mergedUp = true);
                        TileFraming.SetFrameAt(x, y, 54, 90 + 36 * randomFrame);
                        return;
                    }
                    mergedUp = true;
                    TileFraming.SetFrameAt(x, y, 90, 144 + 18 * randomFrame);
                    return;
                }
                else if (downSim == TileFraming.Similarity.MergeLink)
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        mergedUp = (mergedDown = true);
                        TileFraming.SetFrameAt(x, y, 144 + 18 * randomFrame, 180);
                        return;
                    }
                    if (rightSim == TileFraming.Similarity.MergeLink)
                    {
                        mergedUp = (mergedRight = (mergedDown = true));
                        TileFraming.SetFrameAt(x, y, 216, 144 + 18 * randomFrame);
                        return;
                    }
                    TileFraming.SetFrameAt(x, y, 216, 18 * randomFrame);
                    return;
                }
                else
                {
                    if (rightSim == TileFraming.Similarity.Same)
                    {
                        mergedUp = true;
                        TileFraming.SetFrameAt(x, y, 234 + 18 * randomFrame, 18);
                        return;
                    }
                    TileFraming.SetFrameAt(x, y, 216, 18 * randomFrame);
                    return;
                }
            }
            else if (downSim == TileFraming.Similarity.Same)
            {
                if (rightSim == TileFraming.Similarity.Same)
                {
                    TileFraming.SetFrameAt(x, y, 18 + 18 * randomFrame, 0);
                    return;
                }
                if (rightSim == TileFraming.Similarity.MergeLink)
                {
                    mergedRight = true;
                    TileFraming.SetFrameAt(x, y, 54 + 18 * randomFrame, 198);
                    return;
                }
                TileFraming.SetFrameAt(x, y, 18 + 36 * randomFrame, 54);
                return;
            }
            else if (downSim == TileFraming.Similarity.MergeLink)
            {
                if (rightSim == TileFraming.Similarity.Same)
                {
                    mergedDown = true;
                    TileFraming.SetFrameAt(x, y, 234 + 18 * randomFrame, 0);
                    return;
                }
                TileFraming.SetFrameAt(x, y, 216, 18 * randomFrame);
                return;
            }
            else
            {
                if (rightSim == TileFraming.Similarity.Same)
                {
                    TileFraming.SetFrameAt(x, y, 108 + 18 * randomFrame, 72);
                    return;
                }
                if (rightSim == TileFraming.Similarity.MergeLink)
                {
                    mergedRight = true;
                    TileFraming.SetFrameAt(x, y, 54 + 18 * randomFrame, 252);
                    return;
                }
                TileFraming.SetFrameAt(x, y, 216, 18 * randomFrame);
                return;
            }
        }

        internal static void CustomMergeFrameExplicit(int x, int y, int myType, int mergeType, out bool mergedUp, out bool mergedLeft, out bool mergedRight, out bool mergedDown, bool forceSameDown = false, bool forceSameUp = false, bool forceSameLeft = false, bool forceSameRight = false, bool resetFrame = true, bool myTypeBrimFrame = false, bool overrideBrimStates = false)
        {
            if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
            {
                mergedUp = (mergedLeft = (mergedRight = (mergedDown = false)));
                return;
            }
            Main.tileMerge[myType][mergeType] = false;
            Tile tileLeft = Main.tile[x - 1, y];
            Tile tileRight = Main.tile[x + 1, y];
            Tile tileUp = Main.tile[x, y - 1];
            Tile tileDown = Main.tile[x, y + 1];
            Tile tileTopLeft = Main.tile[x - 1, y - 1];
            Tile tileTopRight = Main.tile[x + 1, y - 1];
            Tile tileBottomLeft = Main.tile[x - 1, y + 1];
            Tile tileBottomRight = Main.tile[x + 1, y + 1];
            TileFraming.Similarity leftSim = forceSameLeft ? TileFraming.Similarity.Same : TileFraming.GetSimilarity(tileLeft, myType, mergeType);
            TileFraming.Similarity rightSim = forceSameRight ? TileFraming.Similarity.Same : TileFraming.GetSimilarity(tileRight, myType, mergeType);
            TileFraming.Similarity upSim = forceSameUp ? TileFraming.Similarity.Same : TileFraming.GetSimilarity(tileUp, myType, mergeType);
            TileFraming.Similarity downSim = forceSameDown ? TileFraming.Similarity.Same : TileFraming.GetSimilarity(tileDown, myType, mergeType);
            TileFraming.Similarity topLeftSim = TileFraming.GetSimilarity(tileTopLeft, myType, mergeType);
            TileFraming.Similarity topRightSim = TileFraming.GetSimilarity(tileTopRight, myType, mergeType);
            TileFraming.Similarity bottomLeftSim = TileFraming.GetSimilarity(tileBottomLeft, myType, mergeType);
            TileFraming.Similarity bottomRightSim = TileFraming.GetSimilarity(tileBottomRight, myType, mergeType);
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = (int)((byte)randomFrame);
            }
            else
            {
                randomFrame = Main.tile[x, y].TileFrameNumber;
            }
            mergedDown = (mergedLeft = (mergedRight = (mergedUp = false)));
            if (myTypeBrimFrame && !overrideBrimStates && !TileFraming.BrimstoneFraming(x, y, resetFrame, forceSameDown, forceSameUp, forceSameRight, forceSameLeft, false, false, false, false))
            {
                return;
            }
            CustomMergeConditionalTree(x, y, randomFrame, leftSim, rightSim, upSim, downSim, topLeftSim, topRightSim, bottomLeftSim, bottomRightSim, ref mergedLeft, ref mergedRight, ref mergedUp, ref mergedDown);
            if (myTypeBrimFrame && overrideBrimStates && Main.tile[x, y].TileFrameX < 234 && Main.tile[x, y].TileFrameY < 90)
            {
                BrimstoneFraming(x, y, resetFrame, forceSameDown, forceSameUp, forceSameRight, forceSameLeft, false, false, false, false);
            }
        }



        internal static bool BrimstoneFraming(int x, int y, bool resetFrame, bool forceSameDown, bool forceSameUp, bool forceSameRight, bool forceSameLeft, bool forceSameDR, bool forceSameDL, bool forceSameUR, bool forceSameUL)
        {
            if (x < 0 || x >= Main.maxTilesX)
            {
                return false;
            }
            if (y < 0 || y >= Main.maxTilesY)
            {
                return false;
            }
            Tile tile = Main.tile[x, y];
            if (tile.Slope > 0 && TileID.Sets.HasSlopeFrames[tile.TileType])
                return true;

            GetAdjacentTiles(x, y, out bool up, out bool down, out bool left, out bool right, out bool upLeft, out bool upRight, out bool downLeft, out bool downRight);
            up = (forceSameUp || up);
            down = (forceSameDown || down);
            left = (forceSameLeft || left);
            right = (forceSameRight || right);
            upLeft = (forceSameUL || upLeft);
            upRight = (forceSameUR || upRight);
            downLeft = (forceSameDL || downLeft);
            downRight = (forceSameDR || downRight);
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = randomFrame;
            }
            else
            {
                randomFrame = Main.tile[x, y].TileFrameNumber;
            }
            if (!up && down && !left && right && !downRight)
            {
                Main.tile[x, y].TileFrameX = (short)( randomFrame * 36);
                Main.tile[x, y].TileFrameY = 0;
                return false;
            }
            if (!up && down && left && !right && !downLeft)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 36);
                Main.tile[x, y].TileFrameY = 54;
                return false;
            }
            if (up && !down && !left && right && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(0 + randomFrame * 36);
                Main.tile[x, y].TileFrameY = 72;
                return false;
            }
            if (up && !down && left && !right && !upLeft)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 36);
                Main.tile[x, y].TileFrameY = 72;
                return false;
            }
            if (!up && down && left && right && !downLeft && !downRight)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 0;
                return false;
            }
            if (up && !down && left && right && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 36;
                return false;
            }
            if (up && down && !left && right && !downRight && !upRight)
            {
                Main.tile[x, y].TileFrameX = 0;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && !right && !downLeft && !upLeft)
            {
                Main.tile[x, y].TileFrameX = 72;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 36;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 36;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 18;
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 36;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX = 180;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = 198;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = 180;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX =180;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 36;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 18;
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 18 ;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(108 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 36;
                return false;
            }
            if (!up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 0;
                return false;
            }
            if (!up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 0;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 36;
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = (short)(18 + randomFrame * 18);
                Main.tile[x, y].TileFrameY = 36;
                return false;
            }
            if (up && down && !left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                Main.tile[x, y].TileFrameX = 0;
                Main.tile[x, y].TileFrameY = (short)( randomFrame * 18);
                return false;
            }
            if (up && down && !left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = 0;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 36);
                return false;
            }
            if (up && down && left && !right && !downLeft && !downRight && upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = 72;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && !right && downLeft && !downRight && !upLeft && !upRight)
            {
                Main.tile[x, y].TileFrameX = 72;
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            return true;
        }

        private static void GetAdjacentTiles(int x, int y, out bool up, out bool down, out bool left, out bool right, out bool upLeft, out bool upRight, out bool downLeft, out bool downRight)
        {
            Tile tile = Main.tile[x, y];
            Tile north = Main.tile[x, y - 1];
            Tile south = Main.tile[x, y + 1];
            Tile west = Main.tile[x - 1, y];
            Tile east = Main.tile[x + 1, y];
            Tile southwest = Main.tile[x - 1, y + 1];
            Tile southeast = Main.tile[x + 1, y + 1];
            Tile northwest = Main.tile[x - 1, y - 1];
            Tile northeast = Main.tile[x + 1, y - 1];
            left = false;
            right = false;
            up = false;
            down = false;
            upLeft = false;
            upRight = false;
            downLeft = false;
            downRight = false;
            if (TileFraming.GetMerge(tile, north) && ( north.Slope == SlopeType.SlopeDownLeft || north.Slope == SlopeType.SlopeDownRight))
            {
                up = true;
            }
            if (TileFraming.GetMerge(tile, south) && (south.Slope == SlopeType.SlopeUpLeft || south.Slope == SlopeType.SlopeUpRight))
            {
                down = true;
            }
            if (TileFraming.GetMerge(tile, west) && (west.Slope ==  SlopeType.SlopeDownRight || west.Slope == SlopeType.SlopeUpRight))
            {
                left = true;
            }
            if (TileFraming.GetMerge(tile, east) && ( east.Slope == SlopeType.SlopeDownLeft || east.Slope == SlopeType.SlopeUpLeft))
            {
                right = true;
            }
            if (GetMerge(tile, north) && GetMerge(tile, west) && GetMerge(tile, northwest) &&  
                northwest.Slope == SlopeType.SlopeDownRight && ( north.Slope == SlopeType.SlopeDownLeft || north.Slope == SlopeType.SlopeUpLeft) &&
                ( west.Slope == SlopeType.SlopeUpLeft || west.Slope == SlopeType.SlopeUpRight))
            {
                upLeft = true;
            }
            if (TileFraming.GetMerge(tile, north) && TileFraming.GetMerge(tile, east) && TileFraming.GetMerge(tile, northeast) &&  
                northeast.Slope == SlopeType.SlopeDownLeft && ( north.Slope == SlopeType.SlopeDownRight || north.Slope == SlopeType.SlopeUpRight) && 
                (east.Slope == SlopeType.SlopeUpLeft || east.Slope == SlopeType.SlopeUpRight))
            {
                upRight = true;
            }
            if (GetMerge(tile, south) && GetMerge(tile, west) && GetMerge(tile, southwest) &&
                !southwest.IsHalfBlock &&  southwest.Slope == SlopeType.SlopeUpRight &&
                (south.Slope == SlopeType.SlopeDownLeft || south.Slope == SlopeType.SlopeUpLeft) &&
                ( west.Slope == SlopeType.SlopeDownLeft || west.Slope == SlopeType.SlopeDownRight))
            {
                downLeft = true;
            }
            if (GetMerge(tile, south) && GetMerge(tile, east) && GetMerge(tile, southeast) &&
                !southeast.IsHalfBlock && (southeast.Slope == SlopeType.SlopeUpLeft) &&
                (south.Slope == SlopeType.SlopeDownRight || south.Slope == SlopeType.SlopeUpRight) &&
                (east.Slope == SlopeType.SlopeDownLeft || east.Slope == SlopeType.SlopeDownRight))
            {
                downRight = true;
            }

        }

        private static bool GetMerge(Tile myTile, Tile mergeTile)
        {
            return mergeTile.HasTile && (mergeTile.TileType == myTile.TileType || Main.tileMerge[myTile.TileType][mergeTile.TileType]);
        }

    }
}
