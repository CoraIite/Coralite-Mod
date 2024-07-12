using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineSkarnTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

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

            DustType = DustID.BorealWood_Small;
            HitSound = CoraliteSoundID.DigStone_Tink;
            AddMapEntry(new Color(141, 171, 178));

            MinPick = 150;
            MineResist = 3;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new Item(ModContent.ItemType<CrystallineMagike>())
            ];
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<SkarnTile>(), true, true, false);
            return false;
        }

    }
}
