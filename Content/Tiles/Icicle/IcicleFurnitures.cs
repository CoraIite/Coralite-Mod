using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Icicle
{
    public class IcicleStoneTile : ModTile
    {
        public override string Texture => AssetDirectory.IcicleTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.Ices[Type] = true;
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[TileID.IceBrick][Type] = true;

            DustType = DustID.ApprenticeStorm;
            MinPick = 64;
            AddMapEntry(Coralite.Instance.IcicleCyan);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 2;
        }
    }

    public class IcicleBathtubTile : BaseBathtubTile
    {
        public IcicleBathtubTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleBedTile : BaseBedTile<IcicleBed>
    {
        public IcicleBedTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleBigDroplightTile : BaseBigDroplightTile<IcicleBigDroplight>
    {
        public IcicleBigDroplightTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.65f; g = 0.75f; b = 1.2f;
        }
    }

    public class IcicleBookcaseTile : BaseBookcaseTile
    {
        public IcicleBookcaseTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleCandelabraTile : BaseCandelabraTile<IcicleCandelabra>
    {
        public IcicleCandelabraTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.65f; g = 0.75f; b = 1.2f;
        }
    }

    public class IcicleCandleTile : BaseCandleTile<IcicleCandle>
    {
        public IcicleCandleTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.3f; g = 0.45f; b = 0.8f;
        }
    }

    public class IcicleChairTile : BaseChairTile<IcicleChair>
    {
        public IcicleChairTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleChestTile : BaseChestTile<IcicleChest>
    {
        public IcicleChestTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleClockTile : BaseClockTile
    {
        public IcicleClockTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleDoorClosedTile : BaseDoorClosedTile<IcicleDoor, IcicleDoorOpenTile>
    {
        public IcicleDoorClosedTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleDoorOpenTile : BaseDoorOpenTile<IcicleDoor, IcicleDoorClosedTile>
    {
        public IcicleDoorOpenTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleDresserTile : BaseDresserTile<IcicleDresser>
    {
        public IcicleDresserTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleDroplightTile : BaseDroplightTile<IcicleDroplight>
    {
        public IcicleDroplightTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.65f; g = 0.75f; b = 1.2f;
        }
    }

    public class IcicleFloorLampTile : BaseFloorLampTile<IcicleFloorLamp>
    {
        public IcicleFloorLampTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.65f; g = 0.75f; b = 1.2f;
        }
    }

    public class IciclePianoTile : BasePianoTile
    {
        public IciclePianoTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleSinkTile : BaseSinkTile
    {
        public IcicleSinkTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleSofaTile : BaseSofaTile<IcicleSofa>
    {
        public IcicleSofaTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleTableTile : BaseTableTile
    {
        public IcicleTableTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleToiletTile : BaseToiletTile<IcicleToilet>
    {
        public IcicleToiletTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IcicleWorkBenchTile : BaseWorkBenchTile
    {
        public IcicleWorkBenchTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }

    public class IciclePlatformTile : BasePlatformTile
    {
        public IciclePlatformTile() : base(DustID.ApprenticeStorm, Coralite.Instance.IcicleCyan, AssetDirectory.IcicleTiles)
        {
        }
    }
}
