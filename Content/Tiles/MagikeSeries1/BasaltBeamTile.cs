using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    internal class BasaltBeamTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            //Main.tileSolid[Type] = true;
            //Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.IsBeam[Type] = true;
            DustType = DustID.CorruptionThorns;
            MineResist = 2f;

            AddMapEntry(new Color(105, 97, 90));
            HitSound = CoraliteSoundID.DigStone_Tink;
        }

        public override bool CanExplode(int i, int j) => false;
        public override bool Slope(int i, int j) => false;

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            height = 20;
            offsetY = -2;

            int k = tileFrameY / 18;
            tileFrameY = (short)(k * 22);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

    }
}
