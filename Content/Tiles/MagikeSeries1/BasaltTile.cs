using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class BasaltTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            //Main.tileMerge[Type][ModContent.TileType<MagikeCrystalBlockTile>()] = true;

            Main.tileMerge[TileID.Dirt][Type] = true;

            //Main.tileMerge[Type][ModContent.TileType<CrystalBasaltTile>()] = true;
            //Main.tileMerge[ModContent.TileType<CrystalBasaltTile>()][Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.CorruptionThorns;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Color(31, 31, 50));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.Dirt, true, false, false);
            return false;
        }
    }
}
