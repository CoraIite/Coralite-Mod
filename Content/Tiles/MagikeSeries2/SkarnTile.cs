using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SkarnTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            //Main.tileMerge[Type][ModContent.TileType<MagikeCrystalBlockTile>()] = true;

            Main.tileMerge[TileID.Dirt][Type] = true;

            //Main.tileMerge[Type][ModContent.TileType<CrystalBasaltTile>()] = true;
            //Main.tileMerge[ModContent.TileType<CrystalBasaltTile>()][Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.BorealWood_Small;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Color(141, 171, 178));

            MinPick = 150;
            MineResist = 3;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 4;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.Dirt, true, false, false);
            return false;
        }


    }
}
