using Coralite.Content.Items.Steel;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Steel
{
    public class SteelBrickTile : ModTile
    {
        public override string Texture => AssetDirectory.SteelTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            MineResist = 2f;
            DustType = DustID.Titanium;
            HitSound = CoraliteSoundID.Metal_NPCHit4;
            MinPick = 110;

            AddMapEntry(new Color(214, 217, 231));
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class SteelBrickWall : ModWall
    {
        public override string Texture => AssetDirectory.SteelTiles + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.Titanium;
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(98, 102, 111));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class SteelBathtubTile : BaseBathtubTile
    {
        public SteelBathtubTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelBedTile : BaseBedTile<SteelBed>
    {
        public SteelBedTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelBigDroplightTile : BaseBigDroplightTile<SteelBigDroplight>
    {
        public SteelBigDroplightTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b =0.5f;
        }
    }

    public class SteelBookcaseTile : BaseBookcaseTile
    {
        public SteelBookcaseTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelCandelabraTile : BaseCandelabraTile<SteelCandelabra>
    {
        public SteelCandelabraTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b =0.75f;
        }
    }

    public class SteelCandleTile : BaseCandleTile<SteelCandle>
    {
        public SteelCandleTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.7f; g = 0.7f; b = 0.4f;
        }
    }

    public class SteelChairTile : BaseChairTile<SteelChair>
    {
        public SteelChairTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelClockTile : BaseClockTile
    {
        public SteelClockTile() : base(DustID.Titanium, new Color(214, 217, 231), 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelDoorClosedTile : BaseDoorClosedTile<SteelDoor, SteelDoorOpenTile>
    {
        public SteelDoorClosedTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelDoorOpenTile : BaseDoorOpenTile<SteelDoor, SteelDoorClosedTile>
    {
        public SteelDoorOpenTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelDresserTile : BaseDresserTile<SteelDresser>
    {
        public SteelDresserTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelDroplightTile : BaseDroplightTile<SteelDroplight>
    {
        public SteelDroplightTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b =0.75f;
        }
    }

    public class SteelFloorLampTile : BaseFloorLampTile<SteelFloorLamp>
    {
        public SteelFloorLampTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 1f; g = 1f; b =0.75f;
        }
    }

    public class SteelPianoTile : BasePianoTile
    {
        public SteelPianoTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelSinkTile : BaseSinkTile
    {
        public SteelSinkTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelSofaTile : BaseSofaTile<SteelSofa>
    {
        public SteelSofaTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelBenchTile : BaseSofaTile<SteelBench>
    {
        public SteelBenchTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelTableTile : BaseTableTile
    {
        public SteelTableTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelToiletTile : BaseToiletTile<SteelToilet>
    {
        public SteelToiletTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelWorkBenchTile : BaseWorkBenchTile
    {
        public SteelWorkBenchTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelPlatformTile : BasePlatformTile
    {
        public SteelPlatformTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }

    public class SteelChestTile : BaseChestTile<SteelChest>
    {
        public SteelChestTile() : base(DustID.Titanium, new Color(214, 217, 231), AssetDirectory.SteelTiles)
        {
        }
    }
}
