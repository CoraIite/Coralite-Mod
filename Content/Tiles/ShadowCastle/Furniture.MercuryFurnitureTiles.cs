using Coralite.Content.Items.ShadowCastle;
using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria.ID;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class MercuryBathtubTile : BaseBathtubTile
    {
        public MercuryBathtubTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryBedTile : BaseBedTile<MercuryBed>
    {
        public MercuryBedTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryBigDroplightTile : BaseBigDroplightTile<MercuryBigDroplight>
    {
        public MercuryBigDroplightTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.85f; b = 1.2f;
        }
    }

    public class MercuryBookcaseTile : BaseBookcaseTile
    {
        public MercuryBookcaseTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryCandelabraTile : BaseCandelabraTile<MercuryCandelabra>
    {
        public MercuryCandelabraTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.85f; b = 1.2f;
        }
    }

    public class MercuryCandleTile : BaseCandleTile<MercuryCandle>
    {
        public MercuryCandleTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.45f; b = 0.8f;
        }
    }

    public class MercuryChairTile : BaseChairTile<MercuryChair>
    {
        public MercuryChairTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryClockTile : BaseClockTile
    {
        public MercuryClockTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryDoorClosedTile : BaseDoorClosedTile<MercuryBigDroplight, MercuryDoorOpenTile>
    {
        public MercuryDoorClosedTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryDoorOpenTile : BaseDoorOpenTile<MercuryBigDroplight, MercuryDoorClosedTile>
    {
        public MercuryDoorOpenTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryDresserTile : BaseDresserTile<MercuryDresser>
    {
        public MercuryDresserTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryDroplight2Tile : BaseDroplightTile<MercuryDroplight2>
    {
        public MercuryDroplight2Tile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.85f; b = 1.2f;
        }
    }

    public class MercuryFloorLampTile : BaseFloorLampTile<MercuryFloorLamp>
    {
        public MercuryFloorLampTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.85f; b = 1.2f;
        }
    }

    public class MercuryPianoTile : BasePianoTile
    {
        public MercuryPianoTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercurySinkTile : BaseSinkTile
    {
        public MercurySinkTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercurySofaTile : BaseSofaTile<MercurySofa>
    {
        public MercurySofaTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryTableTile : BaseTableTile
    {
        public MercuryTableTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryToiletTile : BaseToiletTile<MercuryToilet>
    {
        public MercuryToiletTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }

    public class MercuryWorkBenchTile : BaseWorkBenchTile
    {
        public MercuryWorkBenchTile() : base(DustID.Shadowflame, Coralite.Instance.ThunderveinYellow, AssetDirectory.ShadowCastleTiles)
        {
        }
    }
}
