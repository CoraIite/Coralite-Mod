using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class BasaltBathtubTile : BaseBathtubTile
    {
        public BasaltBathtubTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltBedTile : BaseBedTile<BasaltBed>
    {
        public BasaltBedTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltBigDroplightTile : BaseBigDroplightTile<BasaltBigDroplight>
    {
        public BasaltBigDroplightTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.7f; b = 0.8f;
        }
    }

    public class BasaltBookcaseTile : BaseBookcaseTile
    {
        public BasaltBookcaseTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltCandelabraTile : BaseCandelabraTile<BasaltCandelabra>
    {
        public BasaltCandelabraTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.7f; b = 0.8f;
        }
    }

    public class BasaltCandleTile : BaseCandleTile<BasaltCandle>
    {
        public BasaltCandleTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.3f; b = 0.5f;
        }
    }

    public class BasaltChairTile : BaseChairTile<BasaltChair>
    {
        public BasaltChairTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltClockTile : BaseClockTile
    {
        public BasaltClockTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.3f; b = 0.5f;
        }
    }

    public class BasaltDoorClosedTile : BaseDoorClosedTile<BasaltDoor, BasaltDoorOpenTile>
    {
        public BasaltDoorClosedTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.3f; b = 0.5f;
        }
    }

    public class BasaltDoorOpenTile : BaseDoorOpenTile<BasaltDoor, BasaltDoorClosedTile>
    {
        public BasaltDoorOpenTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.3f; b = 0.5f;
        }
    }

    public class BasaltDresserTile : BaseDresserTile<BasaltDresser>
    {
        public BasaltDresserTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltDroplightTile : BaseDroplightTile<BasaltDroplight>
    {
        public BasaltDroplightTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.7f; b = 0.8f;
        }
    }

    public class BasaltFloorLampTile : BaseFloorLampTile<BasaltFloorLamp>
    {
        public BasaltFloorLampTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1.2f; g = 0.7f; b = 0.8f;
        }
    }

    public class BasaltPianoTile : BasePianoTile
    {
        public BasaltPianoTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltSinkTile : BaseSinkTile
    {
        public BasaltSinkTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.tileLighted[Type] = true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.3f; b = 0.5f;
        }
    }

    public class BasaltSofaTile : BaseSofaTile<BasaltSofa>
    {
        public BasaltSofaTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltTableTile : BaseTableTile
    {
        public BasaltTableTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltToiletTile : BaseToiletTile<BasaltToilet>
    {
        public BasaltToiletTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltWorkBenchTile : BaseWorkBenchTile
    {
        public BasaltWorkBenchTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }

    public class BasaltPlatformTile : BasePlatformTile
    {
        public BasaltPlatformTile() : base(DustID.CorruptionThorns, Coralite.MagicCrystalPink, AssetDirectory.MagikeSeries1Tile)
        {
        }
    }
}
