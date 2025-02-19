using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SmoothSkarnTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false; 

            Main.tileMerge[Type][ModContent.TileType<SkarnTile>()] = true;
            Main.tileMerge[ModContent.TileType<SkarnTile>()][Type] = true;

            Main.tileMerge[Type][ModContent.TileType<CrystallineSkarnTile>()] = true;
            Main.tileMerge[ModContent.TileType<CrystallineSkarnTile>()][Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.BorealWood_Small;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Color(91, 117, 141));

            MinPick = 110;
            MineResist = 4;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 4;
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
