using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Core.Prefabs.Tiles
{
    public static class FurniturePrefabs
    {
        public static void DoorOpenPrefab(this ModTile tile, int closeDoorID, int dustType, string mapName, Color mapColor, bool LavaDeath = true)
        {
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileSolid[tile.Type] = false;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            Main.tileNoSunLight[tile.Type] = true;
            TileID.Sets.HousingWalls[tile.Type] = true; // 非实心物块作为墙
            TileID.Sets.HasOutlines[tile.Type] = true;
            TileID.Sets.DisableSmartCursor[tile.Type] = true;

            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            tile.DustType = dustType;
            tile.AdjTiles = new int[] { TileID.OpenDoor };
            tile.CloseDoorID = closeDoorID;

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = LavaDeath;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 0);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 1);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(1, 2);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(tile.Type);
        }

        public static void DoorClosedPrefab(this ModTile tile, int openDoorID, int dustType, string mapName, Color mapColor, bool LavaDeath = true)
        {
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileBlockLight[tile.Type] = true;
            Main.tileSolid[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            TileID.Sets.NotReallySolid[tile.Type] = true;
            TileID.Sets.DrawsWalls[tile.Type] = true;
            TileID.Sets.HasOutlines[tile.Type] = true;
            TileID.Sets.DisableSmartCursor[tile.Type] = true;

            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            tile.DustType = dustType;
            tile.AdjTiles = new int[] { TileID.ClosedDoor };
            tile.OpenDoorID = openDoorID;

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.LavaDeath = LavaDeath;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 1);
            TileObjectData.addAlternate(0);
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Origin = new Point16(0, 2);
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(tile.Type);
        }

        public static void BedPrefab(this ModTile tile, int dustType, string mapName, Color mapColor, bool LavaDeath = true)
        {
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            TileID.Sets.HasOutlines[tile.Type] = true;
            TileID.Sets.CanBeSleptIn[tile.Type] = true; // Facilitates calling ModifySleepingTargetInfo
            TileID.Sets.InteractibleByNPCs[tile.Type] = true; // Town NPCs will palm their hand at this tile
            TileID.Sets.IsValidSpawnPoint[tile.Type] = true;
            TileID.Sets.DisableSmartCursor[tile.Type] = true;

            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation

            tile.DustType = dustType;
            tile.AdjTiles = new int[] { TileID.Beds };

            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); // this style already takes care of direction for us
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
            TileObjectData.addTile(tile.Type);

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

        public static void WorkBenchPrefab(this ModTile tile, int dustType, string mapName, Color mapColor, bool LavaDeath = true)
        {
            Main.tileTable[tile.Type] = true;
            Main.tileSolidTop[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            Main.tileFrameImportant[tile.Type] = true;
            TileID.Sets.DisableSmartCursor[tile.Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[tile.Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            tile.DustType = dustType;
            tile.AdjTiles = new int[] { TileID.WorkBenches };

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.CoordinateHeights = new[] { 18 };
            TileObjectData.addTile(tile.Type);

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);

        }

        public static void TablePrefab(this ModTile tile, int dustType, string mapName, Color mapColor, bool LavaDeath = true)
        {
            Main.tileTable[tile.Type] = true;
            Main.tileSolidTop[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            Main.tileFrameImportant[tile.Type] = true;
            TileID.Sets.DisableSmartCursor[tile.Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[tile.Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

            tile.DustType = dustType;
            tile.AdjTiles = new int[] { TileID.Tables };

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = LavaDeath;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(tile.Type);

            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

        public static void ChairPrefab(this ModTile tile, int dustType, Color mapColor, bool LavaDeath = true)
        {
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            TileID.Sets.HasOutlines[tile.Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[tile.Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
            TileID.Sets.CanBeSatOnForPlayers[tile.Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
            TileID.Sets.DisableSmartCursor[tile.Type] = true;

            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            tile.DustType = dustType;
            tile.AdjTiles = new int[] { TileID.Chairs };

            tile.AddMapEntry(mapColor, Language.GetText("MapObject.Chair"));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.LavaDeath = LavaDeath;
            // The following 3 lines are needed if you decide to add more styles and stack them vertically
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
            TileObjectData.addTile(tile.Type);
        }

        public static void DropLightPrefab(this ModTile tile, int height, int[] coordinateHeights, string mapName, int dustType, Color mapColor, bool LavaDeath = true)
        {
            Main.tileLighted[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileSolid[tile.Type] = false;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            Main.tileFrameImportant[tile.Type] = true;
            TileID.Sets.DisableSmartCursor[tile.Type] = true;

            tile.DustType = dustType;

            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom /*| AnchorType.PlanterBox*/, 1, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = LavaDeath;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.CoordinateHeights = coordinateHeights;
            TileObjectData.addTile(tile.Type);

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

        public static void CandlePrefab(this ModTile tile, int itemDrop, string mapName, int dustType, Color mapColor, bool LavaDeath = true)
        {
            Main.tileLighted[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileSolid[tile.Type] = false;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            Main.tileFrameImportant[tile.Type] = true;
            TileID.Sets.DisableSmartCursor[tile.Type] = true;

            tile.DustType = dustType;
            tile.ItemDrop = itemDrop;

            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = LavaDeath;
            TileObjectData.newTile.CoordinateHeights = new int[1] { 20 };
            TileObjectData.newTile.DrawYOffset = -4;
            TileObjectData.addTile(tile.Type);

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

        public static void ToiletPrefab(this ModTile tile, int dustType, Color mapColor, bool LavaDeath = true)
        {
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileLavaDeath[tile.Type] = LavaDeath;
            TileID.Sets.HasOutlines[tile.Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[tile.Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
            TileID.Sets.CanBeSatOnForPlayers[tile.Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
            TileID.Sets.DisableSmartCursor[tile.Type] = true;

            tile.AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            tile.DustType = dustType;
            tile.AdjTiles = new int[] { TileID.Toilets }; // Condider adding TileID.Chairs to AdjTiles to mirror "(regular) Toilet" and "Golden Toilet" behavior for crafting stations

            tile.AddMapEntry(mapColor, Language.GetText("MapObject.Toilet"));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.LavaDeath = LavaDeath;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(tile.Type);
        }

        public static void ChestPrefab(this ModTile tile, int dustType, int chestDrop, string containerName, Color mapColor, string mapName, Func<string, int, int, string> MapChestName, int tileShine = 1200)
        {
            Main.tileSpelunker[tile.Type] = true;
            Main.tileContainer[tile.Type] = true;
            Main.tileShine2[tile.Type] = true;
            Main.tileShine[tile.Type] = tileShine;
            Main.tileFrameImportant[tile.Type] = true;
            Main.tileNoAttach[tile.Type] = true;
            Main.tileOreFinderPriority[tile.Type] = 500;
            TileID.Sets.HasOutlines[tile.Type] = true;
            TileID.Sets.BasicChest[tile.Type] = true;
            TileID.Sets.DisableSmartCursor[tile.Type] = true;

            tile.DustType = dustType;
            tile.AdjTiles = new int[] { TileID.Containers };
            tile.ChestDrop = chestDrop;

            tile.ContainerName.SetDefault(containerName);

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name, MapChestName);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(tile.Type);
        }
    }
}
