using Coralite.Core;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Coralite.Content.Tiles.Magike
{
    public class HardBasaltTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.CorruptionThorns;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Microsoft.Xna.Framework.Color(31, 31, 50));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<BasaltTile>(), true, true, false);
            return false;
        }
    }
}
