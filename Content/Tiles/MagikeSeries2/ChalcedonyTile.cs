using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class ChalcedonyTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            Main.tileMerge[Type][ModContent.TileType<LeafChalcedonyTile>()] = true;
            Main.tileMerge[ModContent.TileType<LeafChalcedonyTile>()][Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.GolfPaticle;
            HitSound = CoraliteSoundID.DigStone_Tink;

            AddMapEntry(new Color(217,216,185));
        }

        public override bool CanExplode(int i, int j) => false;
    }

    public class ChalcedonyWall : ModWall
    {
        public override string Texture => AssetDirectory.Walls + Name;
        public override void SetStaticDefaults()
        {
            DustType = DustID.GolfPaticle;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
