using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class HardBasaltTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
            Main.tileMerge[ModContent.TileType<BasaltTile>()][Type] = true;

            MineResist = 3f;
            DustType = DustID.CorruptionThorns;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Color(31, 31, 50));
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<BasaltTile>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}
