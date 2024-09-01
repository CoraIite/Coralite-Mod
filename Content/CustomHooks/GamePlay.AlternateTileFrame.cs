using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.CustomHooks
{
    public class AlternateTileFrame : HookGroup
    {
        //private static PropertyInfo _alternateInfo;
        private static MethodInfo _killTile;

        public override void Load()
        {
            // _alternateInfo = typeof(TileObjectData).GetProperty("Alternates", BindingFlags.Instance | BindingFlags.NonPublic);
            _killTile = typeof(WorldGen).GetMethod("KillTile_DropItems", BindingFlags.Static | BindingFlags.NonPublic);

            var tileLoader = typeof(TileLoader);
            var info = tileLoader.GetMethod("CheckModTile", BindingFlags.Public | BindingFlags.Static);

            if (info is not null)
                //MonoModHooks.Modify(info, ModifyAlternateCheck);
                MonoModHooks.Add(info, ModifyAlternateHook);
        }

        public delegate void ModifyAlternateCheckDelegate(int i, int j, int type);

        private void ModifyAlternateHook(ModifyAlternateCheckDelegate orig, int i, int j, int type)
        {
            if (type <= TileID.Count)
                return;
            if (WorldGen.destroyObject)
                return;
            TileObjectData tileData = TileObjectData.GetTileData(type, 0, 0);
            if (tileData == null)
                return;

            ModTile m = TileLoader.GetTile(type);
            if (!Main.tileSolidTop[type] && m is BaseMagikeTile)
            {
                Tile t = Main.tile[i, j];
                int width = tileData.Width;
                int height = tileData.Height;
                int y1 = t.TileFrameY / 18;

                TileObjectData alternateData;
                int style = 0;

                int partFrameY = t.TileFrameY % tileData.CoordinateFullHeight;

                if (y1 < height)
                    alternateData = tileData;
                else if (y1 < height * 2)
                {
                    style = 1;
                    alternateData = TileObjectData.GetTileData(type, 0, style + 1);// ((List<TileObjectData>)_alternateInfo.GetValue(tileData))[style];
                }
                else if (y1 < (height * 2) + width)
                {
                    style = 2;
                    alternateData = TileObjectData.GetTileData(type, 0, style + 1);// ((List<TileObjectData>)_alternateInfo.GetValue(tileData))[style];
                    partFrameY = (t.TileFrameY - (alternateData.CoordinateFullWidth * 2)) % alternateData.CoordinateFullHeight;
                }
                else
                {
                    style = 3;
                    alternateData = TileObjectData.GetTileData(type, 0, style + 1);// ((List<TileObjectData>)_alternateInfo.GetValue(tileData))[style];
                    partFrameY = (t.TileFrameY - (alternateData.CoordinateFullWidth * 2)) % alternateData.CoordinateFullHeight;
                }

                //因为放下来的时候就是0所以不管他
                int partFrameX = t.TileFrameX % alternateData.CoordinateFullWidth;

                int partX = partFrameX / (alternateData.CoordinateWidth + alternateData.CoordinatePadding);
                int partY = 0;
                for (int remainingFrameY = partFrameY; partY + 1 < alternateData.Height && remainingFrameY - alternateData.CoordinateHeights[partY] - alternateData.CoordinatePadding >= 0; partY++)
                {
                    remainingFrameY -= alternateData.CoordinateHeights[partY] + alternateData.CoordinatePadding;
                }
                // We need to use the tile that trigger this, since it still has the tile type instead of air
                int originalI = i;
                int originalJ = j;
                i -= partX;
                j -= partY;
                int originX = i + alternateData.Origin.X;
                int originY = j + alternateData.Origin.Y;
                bool partiallyDestroyed = false;
                for (int x = i; x < i + alternateData.Width; x++)
                {
                    for (int y = j; y < j + alternateData.Height; y++)
                    {
                        if (!Main.tile[x, y].HasTile || Main.tile[x, y].TileType != type)
                        {
                            partiallyDestroyed = true;
                            break;
                        }
                    }
                    if (partiallyDestroyed)
                        break;
                }

                if (partiallyDestroyed || !CanPlace(originX, originY, type, style, alternateData, 0, checkStay: true))
                {
                    TileObject.objectPreview.Active = false;
                    WorldGen.destroyObject = true;
                    // First the Items to drop are tallied and spawned, then Kill each tile, then KillMultiTile can clean up TileEntities or Chests
                    // KillTile will handle calling DropItems for 1x1 tiles.
                    if (alternateData.Width != 1 || alternateData.Height != 1)
                        _killTile?.Invoke(null, [originalI, originalJ, Main.tile[originalI, originalJ], true, true]); // include all drops.
                    for (int x = i; x < i + alternateData.Width; x++)
                    {
                        for (int y = j; y < j + alternateData.Height; y++)
                        {
                            if (Main.tile[x, y].TileType == type && Main.tile[x, y].HasTile)
                            {
                                WorldGen.KillTile(x, y, false, false, false);
                            }
                        }
                    }
                    TileLoader.KillMultiTile(i, j, t.TileFrameX - partFrameX, t.TileFrameY - partFrameY, type);
                    WorldGen.destroyObject = false;
                    for (int x = i - 1; x < i + tileData.Width + 2; x++)
                    {
                        for (int y = j - 1; y < j + tileData.Height + 2; y++)
                        {
                            WorldGen.TileFrame(x, y, false, false);
                        }
                    }
                }

                return;
            }

            orig.Invoke(i, j, type);
        }

        public static bool CanPlace(int x, int y, int type, int alternate, TileObjectData tileData, int dir, bool checkStay = false)
        {
            if (tileData == null)
                return false;

            int num = x - tileData.Origin.X;
            int num2 = y - tileData.Origin.Y;
            if (num < 0 || num + tileData.Width >= Main.maxTilesX || num2 < 0 || num2 + tileData.Height >= Main.maxTilesY)
                return false;

            bool flag = tileData.RandomStyleRange > 0;
            TileObjectPreviewData.placementCache ??= new TileObjectPreviewData();
            TileObjectPreviewData.placementCache.Reset();

            int num3 = 0;
            if (tileData.AlternatesCount != 0)
                num3 = tileData.AlternatesCount;

            float num4 = -1f;
            float num5 = -1f;
            int num7 = alternate;
            do
            {
                int num8 = x - tileData.Origin.X;
                int num9 = y - tileData.Origin.Y;
                if (num8 < 5 || num8 + tileData.Width > Main.maxTilesX - 5 || num9 < 5 || num9 + tileData.Height > Main.maxTilesY - 5)
                    return false;

                Rectangle rectangle = new(0, 0, tileData.Width, tileData.Height);
                int num10 = 0;
                int num11 = 0;
                if (tileData.AnchorTop.tileCount != 0)
                {
                    if (rectangle.Y == 0)
                    {
                        rectangle.Y = -1;
                        rectangle.Height++;
                        num11++;
                    }

                    int checkStart = tileData.AnchorTop.checkStart;
                    if (checkStart < rectangle.X)
                    {
                        rectangle.Width += rectangle.X - checkStart;
                        num10 += rectangle.X - checkStart;
                        rectangle.X = checkStart;
                    }

                    int num12 = checkStart + tileData.AnchorTop.tileCount - 1;
                    int num13 = rectangle.X + rectangle.Width - 1;
                    if (num12 > num13)
                        rectangle.Width += num12 - num13;
                }

                if (tileData.AnchorBottom.tileCount != 0)
                {
                    if (rectangle.Y + rectangle.Height == tileData.Height)
                        rectangle.Height++;

                    int checkStart2 = tileData.AnchorBottom.checkStart;
                    if (checkStart2 < rectangle.X)
                    {
                        rectangle.Width += rectangle.X - checkStart2;
                        num10 += rectangle.X - checkStart2;
                        rectangle.X = checkStart2;
                    }

                    int num14 = checkStart2 + tileData.AnchorBottom.tileCount - 1;
                    int num15 = rectangle.X + rectangle.Width - 1;
                    if (num14 > num15)
                        rectangle.Width += num14 - num15;
                }

                if (tileData.AnchorLeft.tileCount != 0)
                {
                    if (rectangle.X == 0)
                    {
                        rectangle.X = -1;
                        rectangle.Width++;
                        num10++;
                    }

                    int num16 = tileData.AnchorLeft.checkStart;
                    if ((tileData.AnchorLeft.type & AnchorType.Tree) == AnchorType.Tree)
                        num16--;

                    if (num16 < rectangle.Y)
                    {
                        rectangle.Width += rectangle.Y - num16;
                        num11 += rectangle.Y - num16;
                        rectangle.Y = num16;
                    }

                    int num17 = num16 + tileData.AnchorLeft.tileCount - 1;
                    if ((tileData.AnchorLeft.type & AnchorType.Tree) == AnchorType.Tree)
                        num17 += 2;

                    int num18 = rectangle.Y + rectangle.Height - 1;
                    if (num17 > num18)
                        rectangle.Height += num17 - num18;
                }

                if (tileData.AnchorRight.tileCount != 0)
                {
                    if (rectangle.X + rectangle.Width == tileData.Width)
                        rectangle.Width++;

                    int num19 = tileData.AnchorLeft.checkStart;
                    if ((tileData.AnchorRight.type & AnchorType.Tree) == AnchorType.Tree)
                        num19--;

                    if (num19 < rectangle.Y)
                    {
                        rectangle.Width += rectangle.Y - num19;
                        num11 += rectangle.Y - num19;
                        rectangle.Y = num19;
                    }

                    int num20 = num19 + tileData.AnchorRight.tileCount - 1;
                    if ((tileData.AnchorRight.type & AnchorType.Tree) == AnchorType.Tree)
                        num20 += 2;

                    int num21 = rectangle.Y + rectangle.Height - 1;
                    if (num20 > num21)
                        rectangle.Height += num20 - num21;
                }

                TileObject.objectPreview.Reset();
                TileObject.objectPreview.Active = true;
                TileObject.objectPreview.Type = (ushort)type;
                TileObject.objectPreview.Style = 0;
                TileObject.objectPreview.Alternate = num7;
                TileObject.objectPreview.Size = new Point16(rectangle.Width, rectangle.Height);
                TileObject.objectPreview.ObjectStart = new Point16(num10, num11);
                TileObject.objectPreview.Coordinates = new Point16(num8 - num10, num9 - num11);

                float num22 = 0f;
                float num23 = tileData.Width * tileData.Height;
                float num24 = 0f;
                float num25 = 0f;
                for (int i = 0; i < tileData.Width; i++)
                {
                    for (int j = 0; j < tileData.Height; j++)
                    {
                        Tile tileSafely = Framing.GetTileSafely(num8 + i, num9 + j);
                        bool flag2 = !tileData.LiquidPlace(tileSafely, checkStay);
                        bool flag3 = false;
                        if (tileData.AnchorWall)
                        {
                            num25 += 1f;
                            if (!tileData.isValidWallAnchor(tileSafely.WallType))
                                flag3 = true;
                            else
                                num24 += 1f;
                        }

                        bool flag4 = false;
                        if (tileSafely.HasTile && (!Main.tileCut[tileSafely.TileType]) && !TileID.Sets.BreakableWhenPlacing[tileSafely.TileType] && !checkStay)
                            flag4 = true;

                        if (flag4 || flag2 || flag3)
                        {
                            TileObject.objectPreview[i + num10, j + num11] = 2;

                            continue;
                        }

                        TileObject.objectPreview[i + num10, j + num11] = 1;

                        num22 += 1f;
                    }
                }

                AnchorData anchorBottom = tileData.AnchorBottom;
                if (anchorBottom.tileCount != 0)
                {
                    num25 += anchorBottom.tileCount;
                    int height = tileData.Height;
                    for (int k = 0; k < anchorBottom.tileCount; k++)
                    {
                        int num26 = anchorBottom.checkStart + k;
                        Tile tileSafely = Framing.GetTileSafely(num8 + num26, num9 + height);
                        bool flag5 = false;
                        if (tileSafely.HasUnactuatedTile)
                        {
                            if ((anchorBottom.type & AnchorType.SolidTile) == AnchorType.SolidTile && Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType] && !Main.tileNoAttach[tileSafely.TileType] && (tileData.FlattenAnchors || tileSafely.BlockType == 0))
                                flag5 = tileData.isValidTileAnchor(tileSafely.TileType);

                            if (!flag5 && ((anchorBottom.type & AnchorType.SolidWithTop) == AnchorType.SolidWithTop || (anchorBottom.type & AnchorType.Table) == AnchorType.Table))
                            {
                                if (TileID.Sets.Platforms[tileSafely.TileType])
                                {
                                    _ = tileSafely.TileFrameX / TileObjectData.PlatformFrameWidth();
                                    if (!tileSafely.IsHalfBlock && WorldGen.PlatformProperTopFrame(tileSafely.TileFrameX))
                                        flag5 = true;
                                }
                                else if (Main.tileSolid[tileSafely.TileType] && Main.tileSolidTop[tileSafely.TileType])
                                {
                                    flag5 = true;
                                }
                            }

                            if (!flag5 && (anchorBottom.type & AnchorType.Table) == AnchorType.Table && !TileID.Sets.Platforms[tileSafely.TileType] && Main.tileTable[tileSafely.TileType] && tileSafely.BlockType == 0)
                                flag5 = true;

                            if (!flag5 && (anchorBottom.type & AnchorType.SolidSide) == AnchorType.SolidSide && Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType])
                            {
                                int num27 = (int)tileSafely.BlockType;
                                if ((uint)(num27 - 4) <= 1u)
                                    flag5 = tileData.isValidTileAnchor(tileSafely.TileType);
                            }

                            if (!flag5 && (anchorBottom.type & AnchorType.AlternateTile) == AnchorType.AlternateTile && tileData.isValidAlternateAnchor(tileSafely.TileType))
                                flag5 = true;
                        }
                        else if (!flag5 && (anchorBottom.type & AnchorType.EmptyTile) == AnchorType.EmptyTile)
                        {
                            flag5 = true;
                        }

                        if (!flag5)
                        {
                            TileObject.objectPreview[num26 + num10, height + num11] = 2;

                            continue;
                        }

                        TileObject.objectPreview[num26 + num10, height + num11] = 1;

                        num24 += 1f;
                    }
                }

                anchorBottom = tileData.AnchorTop;
                if (anchorBottom.tileCount != 0)
                {
                    num25 += anchorBottom.tileCount;
                    int num28 = -1;
                    for (int l = 0; l < anchorBottom.tileCount; l++)
                    {
                        int num29 = anchorBottom.checkStart + l;
                        Tile tileSafely = Framing.GetTileSafely(num8 + num29, num9 + num28);
                        bool flag6 = false;
                        if (tileSafely.HasUnactuatedTile)
                        {
                            if (Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType] && !Main.tileNoAttach[tileSafely.TileType] && (tileData.FlattenAnchors || tileSafely.BlockType == 0))
                                flag6 = tileData.isValidTileAnchor(tileSafely.TileType);

                            if (!flag6 && (anchorBottom.type & AnchorType.SolidBottom) == AnchorType.SolidBottom && ((Main.tileSolid[tileSafely.TileType] && (!Main.tileSolidTop[tileSafely.TileType] || (TileID.Sets.Platforms[tileSafely.TileType] && (tileSafely.IsHalfBlock || tileSafely.topSlope())))) || tileSafely.IsHalfBlock || tileSafely.topSlope()) && !TileID.Sets.NotReallySolid[tileSafely.TileType] && !tileSafely.BottomSlope)
                                flag6 = tileData.isValidTileAnchor(tileSafely.TileType);

                            if (!flag6 && (anchorBottom.type & AnchorType.Platform) == AnchorType.Platform && TileID.Sets.Platforms[tileSafely.TileType])
                                flag6 = tileData.isValidTileAnchor(tileSafely.TileType);

                            if (!flag6 && (anchorBottom.type & AnchorType.PlatformNonHammered) == AnchorType.PlatformNonHammered && TileID.Sets.Platforms[tileSafely.TileType] && tileSafely.Slope == 0 && !tileSafely.IsHalfBlock)
                                flag6 = tileData.isValidTileAnchor(tileSafely.TileType);

                            if (!flag6 && (anchorBottom.type & AnchorType.PlanterBox) == AnchorType.PlanterBox && tileSafely.TileType == 380)
                                flag6 = tileData.isValidTileAnchor(tileSafely.TileType);

                            if (!flag6 && (anchorBottom.type & AnchorType.SolidSide) == AnchorType.SolidSide && Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType])
                            {
                                int num27 = (int)tileSafely.BlockType;
                                if ((uint)(num27 - 2) <= 1u)
                                    flag6 = tileData.isValidTileAnchor(tileSafely.TileType);
                            }

                            if (!flag6 && (anchorBottom.type & AnchorType.AlternateTile) == AnchorType.AlternateTile && tileData.isValidAlternateAnchor(tileSafely.TileType))
                                flag6 = true;
                        }
                        else if (!flag6 && (anchorBottom.type & AnchorType.EmptyTile) == AnchorType.EmptyTile)
                        {
                            flag6 = true;
                        }

                        if (!flag6)
                        {
                            TileObject.objectPreview[num29 + num10, num28 + num11] = 2;

                            continue;
                        }

                        TileObject.objectPreview[num29 + num10, num28 + num11] = 1;

                        num24 += 1f;
                    }
                }

                anchorBottom = tileData.AnchorRight;
                if (anchorBottom.tileCount != 0)
                {
                    num25 += anchorBottom.tileCount;
                    int width = tileData.Width;
                    for (int m = 0; m < anchorBottom.tileCount; m++)
                    {
                        int num30 = anchorBottom.checkStart + m;
                        Tile tileSafely = Framing.GetTileSafely(num8 + width, num9 + num30);
                        bool flag7 = false;
                        if (tileSafely.HasUnactuatedTile)
                        {
                            if (Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType] && !Main.tileNoAttach[tileSafely.TileType] && (tileData.FlattenAnchors || tileSafely.BlockType == 0))
                                flag7 = tileData.isValidTileAnchor(tileSafely.TileType);

                            if (!flag7 && (anchorBottom.type & AnchorType.SolidSide) == AnchorType.SolidSide && Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType])
                            {
                                int num27 = (int)tileSafely.BlockType;
                                if (num27 == 2 || num27 == 4)
                                    flag7 = tileData.isValidTileAnchor(tileSafely.TileType);
                            }

                            if (!flag7 && (anchorBottom.type & AnchorType.Tree) == AnchorType.Tree && TileID.Sets.IsATreeTrunk[tileSafely.TileType])
                            {
                                flag7 = true;
                                if (m == 0)
                                {
                                    num25 += 1f;
                                    Tile tileSafely2 = Framing.GetTileSafely(num8 + width, num9 + num30 - 1);
                                    if (tileSafely2.HasUnactuatedTile && TileID.Sets.IsATreeTrunk[tileSafely2.TileType])
                                    {
                                        num24 += 1f;
                                        TileObject.objectPreview[width + num10, num30 + num11 - 1] = 1;
                                    }
                                    else
                                    {
                                        TileObject.objectPreview[width + num10, num30 + num11 - 1] = 2;
                                    }
                                }

                                if (m == anchorBottom.tileCount - 1)
                                {
                                    num25 += 1f;
                                    Tile tileSafely3 = Framing.GetTileSafely(num8 + width, num9 + num30 + 1);
                                    if (tileSafely3.HasUnactuatedTile && TileID.Sets.IsATreeTrunk[tileSafely3.TileType])
                                    {
                                        num24 += 1f;
                                        TileObject.objectPreview[width + num10, num30 + num11 + 1] = 1;
                                    }
                                    else
                                    {
                                        TileObject.objectPreview[width + num10, num30 + num11 + 1] = 2;
                                    }
                                }
                            }

                            if (!flag7 && (anchorBottom.type & AnchorType.AlternateTile) == AnchorType.AlternateTile && tileData.isValidAlternateAnchor(tileSafely.TileType))
                                flag7 = true;
                        }
                        else if (!flag7 && (anchorBottom.type & AnchorType.EmptyTile) == AnchorType.EmptyTile)
                        {
                            flag7 = true;
                        }

                        if (!flag7)
                        {
                            TileObject.objectPreview[width + num10, num30 + num11] = 2;

                            continue;
                        }

                        TileObject.objectPreview[width + num10, num30 + num11] = 1;

                        num24 += 1f;
                    }
                }

                anchorBottom = tileData.AnchorLeft;
                if (anchorBottom.tileCount != 0)
                {
                    num25 += anchorBottom.tileCount;
                    int num31 = -1;
                    for (int n = 0; n < anchorBottom.tileCount; n++)
                    {
                        int num32 = anchorBottom.checkStart + n;
                        Tile tileSafely = Framing.GetTileSafely(num8 + num31, num9 + num32);
                        bool flag8 = false;
                        if (tileSafely.HasUnactuatedTile)
                        {
                            if (Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType] && !Main.tileNoAttach[tileSafely.TileType] && (tileData.FlattenAnchors || tileSafely.BlockType == 0))
                                flag8 = tileData.isValidTileAnchor(tileSafely.TileType);

                            if (!flag8 && (anchorBottom.type & AnchorType.SolidSide) == AnchorType.SolidSide && Main.tileSolid[tileSafely.TileType] && !Main.tileSolidTop[tileSafely.TileType])
                            {
                                int num27 = (int)tileSafely.BlockType;
                                if (num27 == 3 || num27 == 5)
                                    flag8 = tileData.isValidTileAnchor(tileSafely.TileType);
                            }

                            if (!flag8 && (anchorBottom.type & AnchorType.Tree) == AnchorType.Tree && TileID.Sets.IsATreeTrunk[tileSafely.TileType])
                            {
                                flag8 = true;
                                if (n == 0)
                                {
                                    num25 += 1f;
                                    Tile tileSafely4 = Framing.GetTileSafely(num8 + num31, num9 + num32 - 1);
                                    if (tileSafely4.HasUnactuatedTile && TileID.Sets.IsATreeTrunk[tileSafely4.TileType])
                                    {
                                        num24 += 1f;
                                        TileObject.objectPreview[num31 + num10, num32 + num11 - 1] = 1;
                                    }
                                    else
                                    {
                                        TileObject.objectPreview[num31 + num10, num32 + num11 - 1] = 2;
                                    }
                                }

                                if (n == anchorBottom.tileCount - 1)
                                {
                                    num25 += 1f;
                                    Tile tileSafely5 = Framing.GetTileSafely(num8 + num31, num9 + num32 + 1);
                                    if (tileSafely5.HasUnactuatedTile && TileID.Sets.IsATreeTrunk[tileSafely5.TileType])
                                    {
                                        num24 += 1f;
                                        TileObject.objectPreview[num31 + num10, num32 + num11 + 1] = 1;
                                    }
                                    else
                                    {
                                        TileObject.objectPreview[num31 + num10, num32 + num11 + 1] = 2;
                                    }
                                }
                            }

                            if (!flag8 && (anchorBottom.type & AnchorType.AlternateTile) == AnchorType.AlternateTile && tileData.isValidAlternateAnchor(tileSafely.TileType))
                                flag8 = true;
                        }
                        else if (!flag8 && (anchorBottom.type & AnchorType.EmptyTile) == AnchorType.EmptyTile)
                        {
                            flag8 = true;
                        }

                        if (!flag8)
                        {
                            TileObject.objectPreview[num31 + num10, num32 + num11] = 2;

                            continue;
                        }

                        TileObject.objectPreview[num31 + num10, num32 + num11] = 1;

                        num24 += 1f;
                    }
                }

                if (tileData.HookCheckIfCanPlace.hook != null)
                {
                    if (tileData.HookCheckIfCanPlace.hook(x, y, type, 0, dir, num7) == tileData.HookCheckIfCanPlace.badReturn && tileData.HookCheckIfCanPlace.badResponse == 0)
                    {
                        num24 = 0f;
                        num22 = 0f;
                        TileObject.objectPreview.AllInvalid();
                    }
                }

                float num33 = num24 / num25;
                // Backport a fix for tiles with no anchors: if (totalAnchorCount == 0) anchorPercent = 1;
                if (num25 == 0)
                    num33 = 1;
                float num34 = num22 / num23;
                if (num34 == 1f && num25 == 0f)
                {
                    num23 = 1f;
                    num25 = 1f;
                    num33 = 1f;
                    num34 = 1f;
                }

                if (num33 == 1f && num34 == 1f)
                {
                    num4 = 1f;
                    num5 = 1f;
                    break;
                }

                if (num33 > num4 || (num33 == num4 && num34 > num5))
                {
                    TileObjectPreviewData.placementCache.CopyFrom(TileObject.objectPreview);
                    num4 = num33;
                    num5 = num34;
                }
            }
            while (false);

            int num35 = -1;
            if (flag)
            {
                TileObjectPreviewData.randomCache ??= new TileObjectPreviewData();

                bool flag9 = false;
                if (TileObjectPreviewData.randomCache.Type == type)
                {
                    Point16 coordinates = TileObjectPreviewData.randomCache.Coordinates;
                    Point16 objectStart = TileObjectPreviewData.randomCache.ObjectStart;
                    int num36 = coordinates.X + objectStart.X;
                    int num37 = coordinates.Y + objectStart.Y;
                    int num38 = x - tileData.Origin.X;
                    int num39 = y - tileData.Origin.Y;
                    if (num36 != num38 || num37 != num39)
                        flag9 = true;
                }
                else
                {
                    flag9 = true;
                }

                int randomStyleRange = tileData.RandomStyleRange;
                int num40 = Main.rand.Next(tileData.RandomStyleRange);

                num35 = (!flag9) ? TileObjectPreviewData.randomCache.Random : num40;
            }

            if (tileData.SpecificRandomStyles != null)
            {
                TileObjectPreviewData.randomCache ??= new TileObjectPreviewData();

                bool flag10 = false;
                if (TileObjectPreviewData.randomCache.Type == type)
                {
                    Point16 coordinates2 = TileObjectPreviewData.randomCache.Coordinates;
                    Point16 objectStart2 = TileObjectPreviewData.randomCache.ObjectStart;
                    int num41 = coordinates2.X + objectStart2.X;
                    int num42 = coordinates2.Y + objectStart2.Y;
                    int num43 = x - tileData.Origin.X;
                    int num44 = y - tileData.Origin.Y;
                    if (num41 != num43 || num42 != num44)
                        flag10 = true;
                }
                else
                {
                    flag10 = true;
                }

                int num45 = tileData.SpecificRandomStyles.Length;
                int num46 = Main.rand.Next(num45);

                num35 = !flag10 ? TileObjectPreviewData.randomCache.Random : tileData.SpecificRandomStyles[num46];
            }

            if (num4 != 1f || num5 != 1f)
            {
                TileObject.objectPreview.CopyFrom(TileObjectPreviewData.placementCache);
            }

            TileObject.objectPreview.Random = num35;
            if (tileData.RandomStyleRange > 0 || tileData.SpecificRandomStyles != null)
                TileObjectPreviewData.randomCache.CopyFrom(TileObject.objectPreview);

            if (num4 == 1f)
                return num5 == 1f;

            return false;
        }


        //private void ModifyAlternateCheck(ILContext il)
        //{
        //    ILCursor cursor = new ILCursor(il);
        //    cursor.TryGotoNext(
        //         i => i.MatchAdd(),
        //         i => i.MatchCall<TileObjectData>("GetTileData"),
        //         i => i.MatchStloc(0));

        //    cursor.Index += 2;

        //    cursor.EmitLdloc(0);
        //    cursor.EmitLdarg(0);
        //    cursor.EmitLdarg(1);
        //    cursor.EmitDelegate(GetAlternateData);
        //    cursor.EmitStloc(0);
        //}

        //private TileObjectData GetAlternateData(TileObjectData originData, int i, int j)
        //{
        //    Tile t = Main.tile[i, j];

        //    if (t == null || t.TileType < TileID.Count)
        //        return originData;

        //    ModTile m = TileLoader.GetTile(t.TileType);
        //    if (!Main.tileSolidTop[t.TileType] && m is BaseMagikeTile)
        //    {
        //        TileObjectData data = TileObjectData.GetTileData(t.TileType, 0);
        //        int width = data.Width;
        //        int height = data.Height;
        //        int y = t.TileFrameY / 18;
        //        if (y < height)
        //            return originData;
        //        if (y < height * 2)
        //            return ((List<TileObjectData>)_alternateInfo.GetValue(originData))[1];
        //        if (y < height * 2 + width)
        //            return ((List<TileObjectData>)_alternateInfo.GetValue(originData))[2];

        //        return ((List<TileObjectData>)_alternateInfo.GetValue(originData))[3];
        //    }

        //    return originData;
        //}

        public override void Unload()
        {
        }
    }
}
