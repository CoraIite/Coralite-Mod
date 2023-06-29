using Coralite.Core;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Magike
{
    public class BasaltTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            //Main.tileMerge[Type][ModContent.TileType<MagikeCrystalBlockTile>()] = true;
            
            //Main.tileMerge[Type][TileID.Stone] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.CorruptionThorns;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Microsoft.Xna.Framework.Color(31, 31, 50));
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, TileID.Dirt, true, false, false);
            return false;
        }


    }
}
