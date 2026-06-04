using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class MagicCrystalBlockTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 600;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[ModContent.TileType<BasaltTile>()][Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.CrystalSerpent_Pink;
            HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact;
            AddMapEntry(Coralite.MagicCrystalPink);
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<BasaltTile>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.05f;
            b = 0.1f;
        }
    }
}
