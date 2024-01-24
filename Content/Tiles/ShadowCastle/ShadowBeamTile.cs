using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class ShadowBeamTile : ModTile
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override void SetStaticDefaults()
        {
            //Main.tileSolid[Type] = true;
            //Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = false;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.IsBeam[Type] = true;
            MinPick = 100;
            DustType = DustID.Shadowflame;
            MineResist = 0.3f;

            AddMapEntry(new Microsoft.Xna.Framework.Color(104, 34, 192));
            HitSound = CoraliteSoundID.DigStone_Tink;
        }

        public override bool CanExplode(int i, int j) => false;
        public override bool Slope(int i, int j) => false;

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            height = 18;

            int k = tileFrameY / 18;
            tileFrameY = (short)(k * 22);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

    }
}
