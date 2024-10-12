using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class ChalcedonyBathtubTile : BaseBathtubTile
    {
        public ChalcedonyBathtubTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyBedTile : BaseBedTile<ChalcedonyBed>
    {
        public ChalcedonyBedTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyBigDroplightTile : BaseBigDroplightTile<ChalcedonyBigDroplight>
    {
        public ChalcedonyBigDroplightTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 0.8f;
        }
    }

    public class ChalcedonyBookcaseTile : BaseBookcaseTile
    {
        public ChalcedonyBookcaseTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyCandelabraTile : BaseCandelabraTile<ChalcedonyCandelabra>
    {
        public ChalcedonyCandelabraTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 0.8f;
        }
    }

    public class ChalcedonyCandleTile : BaseCandleTile<ChalcedonyCandle>
    {
        public ChalcedonyCandleTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.7f; g = 0.7f; b = 0.6f;
        }
    }

    public class ChalcedonyChairTile : BaseChairTile<ChalcedonyChair>
    {
        public ChalcedonyChairTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyClockTile : BaseClockTile
    {
        public ChalcedonyClockTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyDoorClosedTile : BaseDoorClosedTile<ChalcedonyDoor, ChalcedonyDoorOpenTile>
    {
        public ChalcedonyDoorClosedTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyDoorOpenTile : BaseDoorOpenTile<ChalcedonyDoor, ChalcedonyDoorClosedTile>
    {
        public ChalcedonyDoorOpenTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyDresserTile : BaseDresserTile<ChalcedonyDresser>
    {
        public ChalcedonyDresserTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyDroplightTile : BaseDroplightTile<ChalcedonyDroplight>
    {
        public ChalcedonyDroplightTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 0.8f;
        }
    }

    public class ChalcedonyFloorLampTile : BaseFloorLampTile<ChalcedonyFloorLamp>
    {
        public ChalcedonyFloorLampTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 0.8f;
        }
    }

    public class ChalcedonyPianoTile : BasePianoTile
    {
        public ChalcedonyPianoTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonySinkTile : BaseSinkTile
    {
        public ChalcedonySinkTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonySofaTile : BaseSofaTile<ChalcedonySofa>
    {
        public ChalcedonySofaTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyTableTile : BaseTableTile
    {
        public ChalcedonyTableTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyToiletTile : BaseToiletTile<ChalcedonyToilet>
    {
        public ChalcedonyToiletTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyWorkBenchTile : BaseWorkBenchTile
    {
        public ChalcedonyWorkBenchTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyPlatformTile : BasePlatformTile
    {
        public ChalcedonyPlatformTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }

    public class ChalcedonyChestTile : BaseChestTile<ChalcedonyChest>
    {
        public ChalcedonyChestTile() : base(DustID.GolfPaticle, new Color(217, 216, 185), AssetDirectory.MagikeSeries2Tile)
        {
        }
    }
}
