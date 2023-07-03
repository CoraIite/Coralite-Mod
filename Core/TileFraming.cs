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

    //TODO: 增加在改变自身进入到CustomMergeConditionalTree方法中之前检测融合的物块的各种半砖或斜坡

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

            tileMergeTypes[ModContent.TileType<BasaltTile>()][ModContent.TileType<CrystalBasaltTile>()] = true;
            tileMergeTypes[ModContent.TileType<BasaltTile>()][ModContent.TileType<MagicCrystalBlockTile>()] = true;
            tileMergeTypes[ModContent.TileType<BasaltTile>()][ModContent.TileType<HardBasaltTile>()] = true;
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
                return Similarity.None;
            if (check.TileType == myType || Main.tileMerge[myType][check.TileType])
                return Similarity.Same;
            if (check.TileType == mergeType)
                return Similarity.MergeLink;

            return Similarity.None;
        }

        public static void CustomMergeFrame(int x, int y, int myType, int mergeType, bool myTypeBrimFrame = false, bool mergeTypeBrimFrame = false, bool overrideBrimStates = false)
        {
             if (x < 0 || x >= Main.maxTilesX)
                return;
            if (y < 0 || y >= Main.maxTilesY)
                return;

            bool forceSameUp = false;
            bool forceSameDown = false;
            bool forceSameLeft = false;
            bool forceSameRight = false;
            Tile north = Main.tile[x, y - 1];
            Tile south = Main.tile[x, y + 1];
            Tile west = Main.tile[x - 1, y];
            Tile east = Main.tile[x + 1, y];

            if (north != null && north.HasTile && tileMergeTypes[myType][north.TileType])
                CustomMergeFrameExplicit(x, y - 1, north.TileType, myType, out _, out _, out _, out forceSameUp, false, false, false, false, false, mergeTypeBrimFrame, true);
            if (west != null && west.HasTile && tileMergeTypes[ myType][west.TileType])
                CustomMergeFrameExplicit(x - 1, y, west.TileType, myType, out _, out _, out forceSameLeft, out _, false, false, false, false, false, mergeTypeBrimFrame, true);
            if (east != null && east.HasTile && tileMergeTypes[myType][east.TileType])
                CustomMergeFrameExplicit(x + 1, y, east.TileType, myType, out _, out forceSameRight, out _, out _, false, false, false, false, false, mergeTypeBrimFrame, true);
            if (south != null && south.HasTile && tileMergeTypes[myType ][south.TileType])
                CustomMergeFrameExplicit(x, y + 1, south.TileType, myType, out forceSameDown, out _, out _, out _, false, false, false, false, false, mergeTypeBrimFrame, false);
            
            CustomMergeFrameExplicit(x, y, myType, mergeType, out _, out _, out _, out _, forceSameDown, forceSameUp, forceSameLeft, forceSameRight, true, myTypeBrimFrame, overrideBrimStates);
        }

        private static void CustomMergeConditionalTree(int x, int y, int randomFrame, Similarity leftSim, Similarity rightSim, Similarity upSim, Similarity downSim, Similarity topLeftSim, Similarity topRightSim, Similarity bottomLeftSim, Similarity bottomRightSim, ref bool mergedLeft, ref bool mergedRight, ref bool mergedUp, ref bool mergedDown)
        {
            if (leftSim == Similarity.None)
            {
                if (upSim == Similarity.Same)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 0, 18 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            mergedRight = true;
                            SetFrameAt(x, y, 234 + 18 * randomFrame, 36);
                            return;
                        }
                        SetFrameAt(x, y, 90, 18 * randomFrame);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedDown = true;
                            SetFrameAt(x, y, 72, 90 + 18 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedDown = true;
                        SetFrameAt(x, y, 126, 90 + 18 * randomFrame);
                        return;
                    }
                    else
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 36 * randomFrame, 72);
                            return;
                        }
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                        return;
                    }
                }
                else if (upSim == Similarity.MergeLink)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = true;
                            SetFrameAt(x, y, 72, 144 + 18 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                            return;
                        }
                        mergedUp = true;
                        SetFrameAt(x, y, 126, 144 + 18 * randomFrame);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 162, 18 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedUp = true;
                        mergedDown = true;
                        SetFrameAt(x, y, 108, 216 + 18 * randomFrame);
                        return;
                    }
                    else
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 162, 18 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedUp = true;
                        SetFrameAt(x, y, 108, 144 + 18 * randomFrame);
                        return;
                    }
                }
                else if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 36 * randomFrame, 54);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    mergedDown = true;
                    SetFrameAt(x, y, 108, 90 + 18 * randomFrame);
                    return;
                }
                else
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    if (rightSim == Similarity.MergeLink)
                    {
                        mergedRight = true;
                        SetFrameAt(x, y, 54 + 18 * randomFrame, 234);
                        return;
                    }
                    SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                    return;
                }
            }
            else if (leftSim == Similarity.MergeLink)
            {
                if (upSim == Similarity.Same)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedLeft = true;
                            SetFrameAt(x, y, 162, 126 + 18 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = true;
                            mergedRight = true;
                            SetFrameAt(x, y, 180, 126 + 18 * randomFrame);
                            return;
                        }
                        mergedLeft = true;
                        SetFrameAt(x, y, 234 + 18 * randomFrame, 54);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedLeft = (mergedDown = true);
                            SetFrameAt(x, y, 36, 108 + 36 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = (mergedRight = (mergedDown = true));
                            SetFrameAt(x, y, 198, 144 + 18 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                        return;
                    }
                    else
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedLeft = true;
                            SetFrameAt(x, y, 18 * randomFrame, 216);
                            return;
                        }
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                        return;
                    }
                }
                else if (upSim == Similarity.MergeLink)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = (mergedLeft = true);
                            SetFrameAt(x, y, 36, 90 + 36 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = (mergedRight = (mergedUp = true));
                            SetFrameAt(x, y, 198, 90 + 18 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = (mergedLeft = (mergedDown = true));
                            SetFrameAt(x, y, 216, 90 + 18 * randomFrame);
                            return;
                        }
                        if (rightSim == Similarity.MergeLink)
                        {
                            mergedDown = (mergedLeft = (mergedRight = (mergedUp = true)));
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 198);
                            return;
                        }
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    else
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 162, 18 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                }
                else if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedLeft = true;
                        SetFrameAt(x, y, 18 * randomFrame, 198);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                    return;
                }
                else
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedLeft = true;
                        SetFrameAt(x, y, 18 * randomFrame, 252);
                        return;
                    }
                    if (rightSim == Similarity.MergeLink)
                    {
                        mergedRight = (mergedLeft = true);
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 198);
                        return;
                    }
                    mergedLeft = true;
                    SetFrameAt(x, y, 18 * randomFrame, 234);
                    return;
                }
            }
            else if (upSim == Similarity.Same)
            {
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        if (topLeftSim != Similarity.MergeLink && topRightSim != Similarity.MergeLink && bottomLeftSim != Similarity.MergeLink && bottomRightSim != Similarity.MergeLink)
                        {
                            if (topLeftSim == Similarity.Same)
                            {
                                if (topRightSim == Similarity.Same)
                                {
                                    if (bottomLeftSim == Similarity.Same)
                                    {
                                        SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                        return;
                                    }
                                    if (bottomRightSim == Similarity.Same)
                                    {
                                        SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                        return;
                                    }
                                    SetFrameAt(x, y, 108 + 18 * randomFrame, 36);
                                    return;
                                }
                                else if (bottomLeftSim == Similarity.Same)
                                {
                                    if (bottomRightSim != Similarity.Same)
                                    {
                                        SetFrameAt(x, y, 198, 18 * randomFrame);
                                        return;
                                    }
                                    if (topRightSim == Similarity.MergeLink)
                                    {
                                        SetFrameAt(x, y, 0, 108 + 36 * randomFrame);
                                        return;
                                    }
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                            }
                            else if (topLeftSim == Similarity.None)
                            {
                                if (topRightSim != Similarity.Same)
                                {
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                if (bottomRightSim == Similarity.Same)
                                {
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                return;
                            }
                            SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                            return;
                        }
                        if (bottomRightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 0, 90 + 36 * randomFrame);
                            return;
                        }
                        if (bottomLeftSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 18, 90 + 36 * randomFrame);
                            return;
                        }
                        if (topRightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 0, 108 + 36 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 18, 108 + 36 * randomFrame);
                        return;
                    }
                    else
                    {
                        if (rightSim == Similarity.MergeLink)
                        {
                            mergedRight = true;
                            SetFrameAt(x, y, 144, 126 + 18 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 72, 18 * randomFrame);
                        return;
                    }
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedDown = true;
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 90);
                        return;
                    }
                    if (rightSim == Similarity.MergeLink)
                    {
                        mergedDown = (mergedRight = true);
                        SetFrameAt(x, y, 54, 108 + 36 * randomFrame);
                        return;
                    }
                    mergedDown = true;
                    SetFrameAt(x, y, 90, 90 + 18 * randomFrame);
                    return;
                }
                else
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 18 + 18 * randomFrame, 36);
                        return;
                    }
                    if (rightSim == Similarity.MergeLink)
                    {
                        mergedRight = true;
                        SetFrameAt(x, y, 54 + 18 * randomFrame, 216);
                        return;
                    }
                    SetFrameAt(x, y, 18 + 36 * randomFrame, 72);
                    return;
                }
            }
            else if (upSim == Similarity.MergeLink)
            {
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedUp = true;
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 108);
                        return;
                    }
                    if (rightSim == Similarity.MergeLink)
                    {
                        mergedRight = (mergedUp = true);
                        SetFrameAt(x, y, 54, 90 + 36 * randomFrame);
                        return;
                    }
                    mergedUp = true;
                    SetFrameAt(x, y, 90, 144 + 18 * randomFrame);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedUp = (mergedDown = true);
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 180);
                        return;
                    }
                    if (rightSim == Similarity.MergeLink)
                    {
                        mergedUp = (mergedRight = (mergedDown = true));
                        SetFrameAt(x, y, 216, 144 + 18 * randomFrame);
                        return;
                    }
                    SetFrameAt(x, y, 216, 18 * randomFrame);
                    return;
                }
                else
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedUp = true;
                        SetFrameAt(x, y, 234 + 18 * randomFrame, 18);
                        return;
                    }
                    SetFrameAt(x, y, 216, 18 * randomFrame);
                    return;
                }
            }
            else if (downSim == Similarity.Same)
            {
                if (rightSim == Similarity.Same)
                {
                    SetFrameAt(x, y, 18 + 18 * randomFrame, 0);
                    return;
                }
                if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = true;
                    SetFrameAt(x, y, 54 + 18 * randomFrame, 198);
                    return;
                }
                SetFrameAt(x, y, 18 + 36 * randomFrame, 54);
                return;
            }
            else if (downSim == Similarity.MergeLink)
            {
                if (rightSim == Similarity.Same)
                {
                    mergedDown = true;
                    SetFrameAt(x, y, 234 + 18 * randomFrame, 0);
                    return;
                }
                SetFrameAt(x, y, 216, 18 * randomFrame);
                return;
            }
            else
            {
                if (rightSim == Similarity.Same)
                {
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 72);
                    return;
                }
                if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = true;
                    SetFrameAt(x, y, 54 + 18 * randomFrame, 252);
                    return;
                }
                SetFrameAt(x, y, 216, 18 * randomFrame);
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
            Tile self = Main.tile[x, y];
            Tile tileLeft = Main.tile[x - 1, y];
            Tile tileRight = Main.tile[x + 1, y];
            Tile tileUp = Main.tile[x, y - 1];
            Tile tileDown = Main.tile[x, y + 1];
            Tile tileTopLeft = Main.tile[x - 1, y - 1];
            Tile tileTopRight = Main.tile[x + 1, y - 1];
            Tile tileBottomLeft = Main.tile[x - 1, y + 1];
            Tile tileBottomRight = Main.tile[x + 1, y + 1];
            Similarity leftSim = forceSameLeft ? Similarity.Same : GetSimilarity(tileLeft, myType, mergeType);
            if (tileLeft.Slope is SlopeType.SlopeDownLeft or SlopeType.SlopeUpLeft)
                leftSim = Similarity.None;

            Similarity rightSim = forceSameRight ? Similarity.Same : GetSimilarity(tileRight, myType, mergeType);
            if (tileRight.Slope is SlopeType.SlopeDownRight or SlopeType.SlopeUpRight)
                rightSim = Similarity.None;

            Similarity upSim = forceSameUp ? Similarity.Same : GetSimilarity(tileUp, myType, mergeType);
            if (tileUp.Slope is SlopeType.SlopeUpLeft or SlopeType.SlopeUpRight || self.IsHalfBlock)
                upSim = Similarity.None;

            Similarity downSim = forceSameDown ? Similarity.Same : GetSimilarity(tileDown, myType, mergeType);
            if (tileDown.Slope is SlopeType.SlopeDownLeft or SlopeType.SlopeDownRight || tileDown.IsHalfBlock)
                downSim = Similarity.None;

            switch (self.Slope)
            {
                default:
                case SlopeType.Solid:
                    break;
                case SlopeType.SlopeDownLeft:
                    upSim = Similarity.None;
                    rightSim = Similarity.None;
                    break;
                case SlopeType.SlopeDownRight:
                    upSim = Similarity.None;
                    leftSim = Similarity.None;
                    break;
                case SlopeType.SlopeUpLeft:
                    downSim = Similarity.None;
                    rightSim = Similarity.None;
                    break;
                case SlopeType.SlopeUpRight:
                    downSim = Similarity.None;
                    leftSim = Similarity.None;
                    break;
            }

            Similarity topLeftSim = GetSimilarity(tileTopLeft, myType, mergeType);
            Similarity topRightSim = GetSimilarity(tileTopRight, myType, mergeType);
            Similarity bottomLeftSim = GetSimilarity(tileBottomLeft, myType, mergeType);
            Similarity bottomRightSim = GetSimilarity(tileBottomRight, myType, mergeType);
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
            if (myTypeBrimFrame && !overrideBrimStates && !BrimstoneFraming(x, y, resetFrame, forceSameDown, forceSameUp, forceSameRight, forceSameLeft, false, false, false, false))
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
                Main.tile[x, y].TileFrameX = (short)(randomFrame * 36);
                Main.tile[x, y].TileFrameY = 54;
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
                Main.tile[x, y].TileFrameY = (short)(randomFrame * 18);
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
            if (GetMerge(tile, north) && !tile.IsHalfBlock&&
                (tile.Slope is not SlopeType.SlopeDownLeft or SlopeType.SlopeDownRight) &&
                (north.Slope is SlopeType.Solid or  SlopeType.SlopeDownLeft or SlopeType.SlopeDownRight))
            {
                up = true;
            }
            if (GetMerge(tile, south) && !south.IsHalfBlock &&
                (tile.Slope is not SlopeType.SlopeUpLeft or SlopeType.SlopeUpRight) &&
                (south.Slope is SlopeType.Solid or SlopeType.SlopeUpLeft or SlopeType.SlopeUpRight))
            {
                down = true;
            }
            if (GetMerge(tile, west) && (west.Slope == SlopeType.Solid || west.Slope == SlopeType.SlopeDownRight || west.Slope == SlopeType.SlopeUpRight))
            {
                left = true;
            }
            if (GetMerge(tile, east) && (east.Slope == SlopeType.Solid || east.Slope == SlopeType.SlopeDownLeft || east.Slope == SlopeType.SlopeUpLeft))
            {
                right = true;
            }
            if (GetMerge(tile, north) && GetMerge(tile, west) && GetMerge(tile, northwest) &&
                (northwest.Slope == SlopeType.Solid || northwest.Slope == SlopeType.SlopeDownRight) &&
                (north.Slope == SlopeType.Solid || north.Slope == SlopeType.SlopeDownLeft || north.Slope == SlopeType.SlopeUpLeft) && 
                (west.Slope == SlopeType.Solid || west.Slope == SlopeType.SlopeUpLeft || west.Slope == SlopeType.SlopeUpRight))
            {
                upLeft = true;
            }
            if (GetMerge(tile, north) && GetMerge(tile, east) && GetMerge(tile, northeast) &&
                (northeast.Slope == SlopeType.Solid || northeast.Slope == SlopeType.SlopeDownLeft) &&
                (north.Slope == SlopeType.Solid || north.Slope == SlopeType.SlopeDownRight || north.Slope == SlopeType.SlopeUpRight) &&
                (east.Slope == SlopeType.Solid || east.Slope == SlopeType.SlopeUpLeft || east.Slope == SlopeType.SlopeUpRight))
            {
                upRight = true;
            }
            if (GetMerge(tile, south) && GetMerge(tile, west) && GetMerge(tile, southwest) &&
                !southwest.IsHalfBlock && (southwest.Slope == SlopeType.Solid || southwest.Slope == SlopeType.SlopeUpRight) &&
                (south.Slope == SlopeType.Solid || south.Slope == SlopeType.SlopeDownLeft || south.Slope == SlopeType.SlopeUpLeft) &&
                (west.Slope == SlopeType.Solid || west.Slope == SlopeType.SlopeDownLeft || west.Slope == SlopeType.SlopeDownRight))
            {
                downLeft = true;
            }
            if (GetMerge(tile, south) && GetMerge(tile, east) && GetMerge(tile, southeast) && 
                !southeast.IsHalfBlock && (southeast.Slope == SlopeType.Solid || southeast.Slope == SlopeType.SlopeUpLeft) && 
                (south.Slope == SlopeType.Solid || south.Slope == SlopeType.SlopeDownRight || south.Slope == SlopeType.SlopeUpRight) && 
                (east.Slope == SlopeType.Solid || east.Slope == SlopeType.SlopeDownLeft || east.Slope == SlopeType.SlopeDownRight))
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
