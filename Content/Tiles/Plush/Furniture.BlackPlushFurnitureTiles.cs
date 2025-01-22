using Coralite.Content.Items.Plush;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Plush
{
    public class BlackPlushTile : ModTile
    {
        public override string Texture => AssetDirectory.PlushTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = DustID.Wraith;
            HitSound = CoraliteSoundID.Dig;

            AddMapEntry(new Color(10, 10, 10));
        }
    }

    public class BlackPlushWall : ModWall
    {
        public override string Texture => AssetDirectory.PlushTiles + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.Wraith;
            AddMapEntry(new Color(10, 10, 10));
            Main.wallHouse[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class BlackPlushBathtubTile : BaseBathtubTile
    {
        public BlackPlushBathtubTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushBedTile : BaseBedTile<BlackPlushBed>
    {
        public BlackPlushBedTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushBigDroplightTile : BaseBigDroplightTile<BlackPlushBigDroplight>
    {
        public BlackPlushBigDroplightTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b = 0.8f;
        }
    }

    public class BlackPlushBookcaseTile : BaseBookcaseTile
    {
        public BlackPlushBookcaseTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushCandelabraTile : BaseCandelabraTile<BlackPlushCandelabra>
    {
        public BlackPlushCandelabraTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.8f; b = 0.8f;
        }
    }

    public class BlackPlushCandleTile : BaseCandleTile<BlackPlushCandle>
    {
        public BlackPlushCandleTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.6f; g = 0.6f; b = 0.6f;
        }
    }

    public class BlackPlushChairTile : BaseChairTile<BlackPlushChair>
    {
        public BlackPlushChairTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushClockTile : BaseClockTile
    {
        public BlackPlushClockTile() : base(DustID.Wraith, new Color(10, 10, 10), 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushDoorClosedTile : BaseDoorClosedTile<BlackPlushDoor, BlackPlushDoorOpenTile>
    {
        public BlackPlushDoorClosedTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushDoorOpenTile : BaseDoorOpenTile<BlackPlushDoor, BlackPlushDoorClosedTile>
    {
        public BlackPlushDoorOpenTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushDresserTile : BaseDresserTile<BlackPlushDresser>
    {
        public BlackPlushDresserTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushDroplightTile : BaseDroplightTile<BlackPlushDroplight>
    {
        public BlackPlushDroplightTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.8f; b = 0.8f;
        }
    }

    public class BlackPlushFloorLampTile : BaseFloorLampTile<BlackPlushFloorLamp>
    {
        public BlackPlushFloorLampTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.8f; g = 0.8f; b = 0.8f;
        }
    }

    public class BlackPlushPianoTile : BasePianoTile
    {
        public BlackPlushPianoTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushSinkTile : BaseSinkTile
    {
        public BlackPlushSinkTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushSofaTile : BaseSofaTile<BlackPlushSofa>
    {
        public BlackPlushSofaTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushTableTile : BaseTableTile
    {
        public BlackPlushTableTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushToiletTile : BaseToiletTile<BlackPlushToilet>
    {
        public BlackPlushToiletTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushWorkBenchTile : BaseWorkBenchTile
    {
        public BlackPlushWorkBenchTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushPlatformTile : BasePlatformTile
    {
        public BlackPlushPlatformTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }

    public class BlackPlushChestTile : BaseChestTile<BlackPlushChest>
    {
        public BlackPlushChestTile() : base(DustID.Wraith, new Color(10, 10, 10), AssetDirectory.PlushTiles)
        {
        }
    }
}
