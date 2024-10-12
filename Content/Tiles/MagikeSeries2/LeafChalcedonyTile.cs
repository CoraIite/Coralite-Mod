using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class LeafChalcedonyTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.GolfPaticle;

            AddMapEntry(new Color(217, 216, 185));
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
