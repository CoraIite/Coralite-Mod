using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class CrystalBasaltTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1000;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            //Main.tileMerge[Type][ModContent.TileType<BasaltTile>()] = true;

            //Main.tileMerge[Type][TileID.Stone] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.CorruptionThorns;
            HitSound = SoundID.DD2_CrystalCartImpact;
            AddMapEntry(new Color(31, 31, 50));
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[1]
            {
                new(ModContent.ItemType<MagicCrystal>())
            };
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<BasaltTile>(), true, true, false);
            return false;
        }

    }
}
