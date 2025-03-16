using Coralite.Content.Dusts;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineBlockTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            MineResist = 2f;
            DustType = ModContent.DustType<CrystallineDust>();
            HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact;
            MinPick = 110;

            AddMapEntry(Coralite.CrystallinePurple);
        }
    }
}
