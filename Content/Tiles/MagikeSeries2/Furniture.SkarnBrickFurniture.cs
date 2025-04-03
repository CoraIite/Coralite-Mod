using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SkarnBrickBathtubTile : BaseBathtubTile
    {
        public SkarnBrickBathtubTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f; g = 0.05f; b = 0.2f;
        }
    }

    public class SkarnBrickBedTile : BaseBedTile<SkarnBrickBed>
    {
        public SkarnBrickBedTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickBigDroplightTile : BaseBigDroplightTile<SkarnBrickBigDroplight>
    {
        public SkarnBrickBigDroplightTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 0.5f; b = 0.95f;
        }
    }

    public class SkarnBrickBookcaseTile : BaseBookcaseTile
    {
        public SkarnBrickBookcaseTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickCandelabraTile : BaseCandelabraTile<SkarnBrickCandelabra>
    {
        public SkarnBrickCandelabraTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 0.9f; b = 0.6f;
        }
    }

    public class SkarnBrickCandleTile : BaseCandleTile<SkarnBrickCandle>
    {
        public SkarnBrickCandleTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.7f; g = 0.65f; b = 0.3f;
        }
    }

    public class SkarnBrickChairTile : BaseChairTile<SkarnBrickChair>
    {
        public SkarnBrickChairTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickClockTile : BaseClockTile
    {
        public SkarnBrickClockTile() : base(DustID.Stone, new Color(173, 193, 183), 5, new int[] { 16, 16, 16, 16, 18 }, AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileTable[Type] = true;
            TileID.Sets.Clock[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            DustType = DustID.Stone;
            AdjTiles = new int[] { TileID.GrandfatherClocks };

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.Origin = new Point16(0, 5 - 1);
            TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 18 };
            TileObjectData.addTile(Type);

            // Etc
            AddMapEntry(new Color(173, 193, 183), Language.GetText("ItemName.GrandfatherClock"));
        }
    }

    public class SkarnBrickDoorClosedTile : BaseDoorClosedTile<SkarnBrickDoor, SkarnBrickDoorOpenTile>
    {
        public SkarnBrickDoorClosedTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickDoorOpenTile : BaseDoorOpenTile<SkarnBrickDoor, SkarnBrickDoorClosedTile>
    {
        public SkarnBrickDoorOpenTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickDresserTile : BaseDresserTile<SkarnBrickDresser>
    {
        public SkarnBrickDresserTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickDroplightTile : BaseDroplightTile<SkarnBrickDroplight>
    {
        public SkarnBrickDroplightTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 0.5f; b = 0.95f;
        }
    }

    public class SkarnBrickFloorLampTile : BaseFloorLampTile<SkarnBrickFloorLamp>
    {
        public SkarnBrickFloorLampTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 0.5f; b = 0.95f;
        }
    }

    public class SkarnBrickPianoTile : BasePianoTile
    {
        public SkarnBrickPianoTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickSinkTile : BaseSinkTile
    {
        public SkarnBrickSinkTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f; g = 0.05f; b = 0.2f;
        }
    }

    public class SkarnBrickSofaTile : BaseSofaTile<SkarnBrickSofa>
    {
        public SkarnBrickSofaTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickBenchTile : BaseSofaTile<SkarnBrickBench>
    {
        public SkarnBrickBenchTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickTableTile : BaseTableTile
    {
        public SkarnBrickTableTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickToiletTile : BaseToiletTile<SkarnBrickToilet>
    {
        public SkarnBrickToiletTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickWorkBenchTile : BaseWorkBenchTile
    {
        public SkarnBrickWorkBenchTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class SkarnBrickChestTile : BaseChestTile<SkarnBrickChest>
    {
        public SkarnBrickChestTile() : base(DustID.Stone, new Color(173, 193, 183), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }
}
