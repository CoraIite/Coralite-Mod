using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
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
        public static void SoildBottomPlantPrefab<T>(this ModTile tile, int[] AnchorValidTiles, int[] AnchorAlternateTiles, int[] CoordinatePadding, int DrawYOffset, SoundStyle? HitSound, int DustType, Color mapColor, int CoordinateWidth = 16, string mapName = "", float MineResist = 1f, int MinPick = 0, bool tileLighted = false) where T : ModTileEntity
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
            TileID.Sets.ReplaceTileBreakUp[tile.Type] = true;//成熟后可以用种子快速补种
            TileID.Sets.IgnoredInHouseScore[tile.Type] = true;//不计入NPC住房得分系统中
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
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

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
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
            TileID.Sets.ReplaceTileBreakUp[tile.Type] = true;//成熟后可以用种子快速补种
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

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

    }
}
