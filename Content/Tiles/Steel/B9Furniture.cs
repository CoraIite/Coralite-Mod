using Coralite.Content.Items.Steel;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.Steel
{
    public class B9AlloyBathtubTile : BaseBathtubTile
    {
        public B9AlloyBathtubTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyBedTile : BaseBedTile<B9AlloyBed>
    {
        public B9AlloyBedTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyBigDroplightTile : BaseBigDroplightTile<B9AlloyBigDroplight>
    {
        public B9AlloyBigDroplightTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 1f;
        }
    }

    public class B9AlloyBookcaseTile : BaseBookcaseTile
    {
        public B9AlloyBookcaseTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyCandelabraTile : BaseCandelabraTile<B9AlloyCandelabra>
    {
        public B9AlloyCandelabraTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 1f;
        }
    }

    public class B9AlloyCandleTile : BaseCandleTile<B9AlloyCandle>
    {
        public B9AlloyCandleTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.7f; g = 0.7f; b = 0.7f;
        }
    }

    public class B9AlloyChairTile : BaseChairTile<B9AlloyChair>
    {
        public B9AlloyChairTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyClockTile : BaseClockTile
    {
        public B9AlloyClockTile() : base(DustID.Titanium, new Color(180, 180, 180), 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyDoorClosedTile : BaseDoorClosedTile<B9AlloyDoor, B9AlloyDoorOpenTile>
    {
        public B9AlloyDoorClosedTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyDoorOpenTile : BaseDoorOpenTile<B9AlloyDoor, B9AlloyDoorClosedTile>
    {
        public B9AlloyDoorOpenTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyDresserTile : BaseDresserTile<B9AlloyDresser>
    {
        public B9AlloyDresserTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyDroplightTile : BaseDroplightTile<B9AlloyDroplight>
    {
        public B9AlloyDroplightTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 1f;
        }
    }

    public class B9AlloyFloorLampTile : BaseFloorLampTile<B9AlloyFloorLamp>
    {
        public B9AlloyFloorLampTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 1f;
        }
    }

    public class B9AlloyPianoTile : BasePianoTile
    {
        public B9AlloyPianoTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloySinkTile : BaseSinkTile
    {
        public B9AlloySinkTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloySofaTile : BaseSofaTile<B9AlloySofa>
    {
        public B9AlloySofaTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyTableTile : BaseTableTile
    {
        public B9AlloyTableTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyToiletTile : BaseToiletTile<B9AlloyToilet>
    {
        public B9AlloyToiletTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyWorkBenchTile : BaseWorkBenchTile
    {
        public B9AlloyWorkBenchTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyPlatformTile : BasePlatformTile
    {
        public B9AlloyPlatformTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }

    public class B9AlloyChestTile : BaseChestTile<B9AlloyChest>
    {
        public B9AlloyChestTile() : base(DustID.Titanium, new Color(180, 180, 180), AssetDirectory.SteelTiles)
        {
        }
    }
}
