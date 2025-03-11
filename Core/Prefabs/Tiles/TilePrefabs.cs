using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Prefabs.Tiles
{
    public static class TilePrefabs
    {
        /// <summary>
        /// 设置种在地上的植物基本信息
        /// <para>AnchorValidTiles默认应为TileID.Grass</para>
        /// AnchorAlternateTiles的默认值应为TileID.ClayPot,TileID.PlanterBox
        /// <para>SoundStyle默认值应为SoundID.Grass</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tile"></param>
        /// <param name="AnchorValidTiles"></param>
        /// <param name="AnchorAlternateTiles"></param>
        /// <param name="HitSound"></param>
        /// <param name="DustType"></param>
        /// <param name="mapColor"></param>
        /// <param name="mapName"></param>
        /// <param name="MineResist"></param>
        /// <param name="MinPick"></param>
        public static void SoildBottomPlantPrefab<T>(this ModTile tile, int[] AnchorValidTiles, int[] AnchorAlternateTiles, int[] CoordinateHeights, int DrawYOffset, SoundStyle? HitSound, int DustType, Color mapColor, int CoordinateWidth = 16, string mapName = "", float MineResist = 1f, int MinPick = 0, bool tileLighted = false) where T : ModTileEntity
        {
            Main.tileFrameImportant[tile.Type] = true;//帧重要
            Main.tileObsidianKill[tile.Type] = true;
            Main.tileNoFail[tile.Type] = true;//不会出现挖掘失败的情况
            Main.tileLighted[tile.Type] = tileLighted;//光照
            Main.tileSolid[tile.Type] = false;//实心
            Main.tileSolidTop[tile.Type] = false;//顶部实心，类似平台
            Main.tileNoAttach[tile.Type] = false;//
            Main.tileTable[tile.Type] = false;//是桌子
            Main.tileLavaDeath[tile.Type] = true;//会被岩浆烫
            Main.tileCut[tile.Type] = true;//被弹幕切掉
            Main.tileBlockLight[tile.Type] = false;//阻挡光转递

            TileID.Sets.SwaysInWindBasic[tile.Type] = true;//随风摇摆a
            TileID.Sets.ReplaceTileBreakUp[tile.Type] = true;//
            TileID.Sets.IgnoredInHouseScore[tile.Type] = true;//不计入NPC住房得分系统中
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = CoordinateHeights;
            TileObjectData.newTile.CoordinateWidth = CoordinateWidth;
            TileObjectData.newTile.DrawYOffset = DrawYOffset;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = AnchorValidTiles;
            //种在这上面时就可以使用快速再种植功能
            TileObjectData.newTile.AnchorAlternateTiles = AnchorAlternateTiles;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<T>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(tile.Type);
            tile.HitSound = HitSound;
            tile.DustType = DustType;
            tile.MineResist = MineResist;
            tile.MinPick = MinPick;

            LocalizedText name = tile.CreateMapEntryName();
            // name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

        /// <summary>
        /// 设置种在天花板上的植物基本信息
        /// <para>AnchorValidTiles默认应为TileID.Grass</para>
        /// AnchorAlternateTiles的默认值应为TileID.ClayPot,TileID.PlanterBox
        /// <para>SoundStyle默认值应为SoundID.Grass</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tile"></param>
        /// <param name="AnchorValidTiles"></param>
        /// <param name="AnchorAlternateTiles"></param>
        /// <param name="HitSound"></param>
        /// <param name="DustType"></param>
        /// <param name="mapColor"></param>
        /// <param name="mapName"></param>
        /// <param name="MineResist"></param>
        /// <param name="MinPick"></param>
        public static void SoildTopPlantPrefab<T>(this ModTile tile, int[] AnchorValidTiles, int[] AnchorAlternateTiles, int[] CoordinatePadding, int DrawYOffset, SoundStyle? HitSound, int DustType, Color mapColor, int CoordinateWidth = 16, string mapName = "", float MineResist = 1f, int MinPick = 0, bool tileLighted = false) where T : ModTileEntity
        {
            Main.tileFrameImportant[tile.Type] = true;//帧重要
            Main.tileObsidianKill[tile.Type] = true;
            Main.tileNoFail[tile.Type] = true;//不会出现挖掘失败的情况
            Main.tileLighted[tile.Type] = tileLighted;//光照
            Main.tileSolid[tile.Type] = false;//实心
            Main.tileSolidTop[tile.Type] = false;//顶部实心，类似平台
            Main.tileNoAttach[tile.Type] = false;//
            Main.tileTable[tile.Type] = false;//是桌子
            Main.tileLavaDeath[tile.Type] = true;//会被岩浆烫
            Main.tileCut[tile.Type] = true;//被弹幕切掉
            Main.tileBlockLight[tile.Type] = false;//阻挡光转递

            TileID.Sets.VineThreads[tile.Type] = true;//像藤蔓那样在风中摇摆
            TileID.Sets.ReplaceTileBreakUp[tile.Type] = true;//
            TileID.Sets.IgnoredInHouseScore[tile.Type] = true;//不计入NPC住房得分系统中
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.CoordinateHeights = CoordinatePadding;
            TileObjectData.newTile.CoordinateWidth = CoordinateWidth;
            TileObjectData.newTile.DrawYOffset = DrawYOffset;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = AnchorValidTiles;
            //种在这上面时就可以使用快速再种植功能
            TileObjectData.newTile.AnchorAlternateTiles = AnchorAlternateTiles;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<T>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(tile.Type);
            tile.HitSound = HitSound;
            tile.DustType = DustType;
            tile.MineResist = MineResist;
            tile.MinPick = MinPick;

            LocalizedText name = tile.CreateMapEntryName();
            // name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

        public static void SaplingPrefab(this ModTile tile, int[] AnchorValidTiles, int dustType, Color mapColor, int width = 16, int Bottomheight = 18)
        {
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = [16, Bottomheight];
            TileObjectData.newTile.CoordinateWidth = width;
            TileObjectData.newTile.DrawYOffset = 16 - Bottomheight;
            TileObjectData.newTile.AnchorValidTiles = AnchorValidTiles;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.StyleMultiplier = 3;
            TileObjectData.addTile(tile.Type);

            tile.AddMapEntry(mapColor);

            TileID.Sets.TreeSapling[tile.Type] = true;
            TileID.Sets.CommonSapling[tile.Type] = true;
            TileID.Sets.SwaysInWindBasic[tile.Type] = true;
            TileMaterials.SetForTileId(tile.Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

            tile.DustType = dustType;
            tile.AdjTiles = [TileID.Saplings];
        }
    }
}
