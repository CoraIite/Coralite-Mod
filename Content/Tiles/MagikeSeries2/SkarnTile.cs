﻿using Coralite.Core;
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

            //Main.tileMerge[TileID.Dirt][Type] = true;

            Main.tileMerge[Type][ModContent.TileType<CrystallineSkarnTile>()] = true;
            Main.tileMerge[ModContent.TileType<CrystallineSkarnTile>()][Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;

            DustType = DustID.BorealWood_Small;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Color(141, 171, 178));

            MinPick = 150;
            MineResist = 6;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 4;
        }
    }
}
