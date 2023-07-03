using Coralite.Core;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Coralite.Content.Items.Magike;

namespace Coralite.Content.Tiles.Magike
{
    public class CrystalBasaltTile:ModTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;

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
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Microsoft.Xna.Framework.Color(31, 31, 50));
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[1]
            {
                new Item(ModContent.ItemType<MagicCrystal>())
            };
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type,ModContent.TileType<BasaltTile>(), true, true, false);
            return false;
        }

    }
}
