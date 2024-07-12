using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class ShadowBrickTile : ModTile
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        private const int sheetWidth = 234;
        private const int sheetHeight = 90;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileMerge[Type][ModContent.TileType<ShadowQuadrelTile>()] = true;
            Main.tileMerge[ModContent.TileType<ShadowQuadrelTile>()][Type] = true;
            //Main.tileMerge[Type][ModContent.TileType<ShadowImaginaryBrickTile>()] = true;
            //Main.tileMerge[ModContent.TileType<ShadowImaginaryBrickTile>()][Type] = true;
            Main.tileMerge[ModContent.TileType<ShadowQuadrelTile>()][ModContent.TileType<ShadowImaginaryBrickTile>()] = true;
            //Main.tileMerge[ModContent.TileType<ShadowImaginaryBrickTile>()][ModContent.TileType<ShadowQuadrelTile>()] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            MinPick = 100;
            DustType = DustID.Shadowflame;
            MineResist = 2f;

            AddMapEntry(new Microsoft.Xna.Framework.Color(104, 54, 192));
            HitSound = CoraliteSoundID.DigStone_Tink;
        }

        public override bool CanExplode(int i, int j) => false;
        public override bool Slope(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            int xPos = i % 2;
            int yPos = j % 2;
            frameXOffset = xPos * sheetWidth;
            frameYOffset = yPos * sheetHeight;
        }

    }
}
