using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class BasaltBrickTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            DustType = DustID.CorruptionThorns;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Color(31, 31, 50));
        }
    }
}
