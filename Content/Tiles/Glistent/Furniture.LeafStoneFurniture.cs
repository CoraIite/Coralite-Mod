using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Glistent
{
    public class LeafStoneTile : ModTile
    {
        public override string Texture => AssetDirectory.GlistentTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            Main.tileMerge[Type][TileID.LivingWood] = true;
            Main.tileMerge[Type][TileID.LeafBlock] = true;
            Main.tileMerge[TileID.LivingWood][Type] = true;
            Main.tileMerge[TileID.LeafBlock][Type]= true;

            DustType = DustID.JungleGrass;
            HitSound = CoraliteSoundID.DigStone_Tink;

            AddMapEntry(new Color(106, 124, 7));
        }
    }

    public class LeafStoneBrickTile : ModTile
    {
        public override string Texture => AssetDirectory.GlistentTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = DustID.JungleGrass;
            HitSound = CoraliteSoundID.DigStone_Tink;

            AddMapEntry(new Color(106, 124, 7));
        }
    }

    public class LeafStoneWall : ModWall
    {
        public override string Texture => AssetDirectory.GlistentTiles + Name;

        public override void SetStaticDefaults()
        {
            DustType = DustID.JungleGrass;
            AddMapEntry(new Color(106, 124, 7));
            Main.wallHouse[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class LeafStoneBathtubTile : BaseBathtubTile
    {
        public LeafStoneBathtubTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneBedTile : BaseBedTile<LeafStoneBed>
    {
        public LeafStoneBedTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneBigDroplightTile : BaseBigDroplightTile<LeafStoneBigDroplight>
    {
        public LeafStoneBigDroplightTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.6f; g = 0.8f; b = 0.1f;
        }
    }

    public class LeafStoneBookcaseTile : BaseBookcaseTile
    {
        public LeafStoneBookcaseTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneCandelabraTile : BaseCandelabraTile<LeafStoneCandelabra>
    {
        public LeafStoneCandelabraTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.6f; g = 0.8f; b = 0.1f;
        }
    }

    public class LeafStoneCandleTile : BaseCandleTile<LeafStoneCandle>
    {
        public LeafStoneCandleTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.5f; g = 0.6f; b = 0.1f;
        }
    }

    public class LeafStoneChairTile : BaseChairTile<LeafStoneChair>
    {
        public LeafStoneChairTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneClockTile : BaseClockTile
    {
        public LeafStoneClockTile() : base(DustID.JungleGrass, new Color(106, 124, 7), 5, new int[] { 16, 16, 16, 16, 16 }, AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneDoorClosedTile : BaseDoorClosedTile<LeafStoneDoor, LeafStoneDoorOpenTile>
    {
        public LeafStoneDoorClosedTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneDoorOpenTile : BaseDoorOpenTile<LeafStoneDoor, LeafStoneDoorClosedTile>
    {
        public LeafStoneDoorOpenTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneDresserTile : BaseDresserTile<LeafStoneDresser>
    {
        public LeafStoneDresserTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneDroplightTile : BaseDroplightTile<LeafStoneDroplight>
    {
        public LeafStoneDroplightTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.6f; g = 0.8f; b = 0.1f;
        }
    }

    public class LeafStoneFloorLampTile : BaseFloorLampTile<LeafStoneFloorLamp>
    {
        public LeafStoneFloorLampTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }

        public override void GetLight(ref float r, ref float g, ref float b)
        {
            r = 0.6f; g = 0.8f; b = 0.1f;
        }
    }

    public class LeafStonePianoTile : BasePianoTile
    {
        public LeafStonePianoTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneSinkTile : BaseSinkTile
    {
        public LeafStoneSinkTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneSofaTile : BaseSofaTile<LeafStoneSofa>
    {
        public LeafStoneSofaTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneTableTile : BaseTableTile
    {
        public LeafStoneTableTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneToiletTile : BaseToiletTile<LeafStoneToilet>
    {
        public LeafStoneToiletTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneWorkBenchTile : BaseWorkBenchTile
    {
        public LeafStoneWorkBenchTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStonePlatformTile : BasePlatformTile
    {
        public LeafStonePlatformTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }

    public class LeafStoneChestTile : BaseChestTile<LeafStoneChest>
    {
        public LeafStoneChestTile() : base(DustID.JungleGrass, new Color(106, 124, 7), AssetDirectory.GlistentTiles)
        {
        }
    }
}
