using Coralite.Core;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Coralite.Helpers;
using System.Collections.Generic;

namespace Coralite.Content.Tiles.Magike
{
    public class MagikeCrystalBlockTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            //Main.tileMerge[Type][ModContent.TileType<BasaltTile>()] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.CrystalSerpent_Pink;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(Coralite.Instance.MagicCrystalPink);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<BasaltTile>(), true, true, false);
            return false;
        }

    }
}
