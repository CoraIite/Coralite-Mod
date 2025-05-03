using Coralite.Content.Dusts;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SkarnBrickTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            DustType = ModContent.DustType<SkarnDust>();
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Color(214, 245, 212));

            MinPick = 110;
            MineResist = 6;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 4;
        }

        public override bool CanExplode(int i, int j) => false;
    }
}
