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
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1000;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[ModContent.TileType<BasaltTile>()][Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = DustID.CorruptionThorns;
            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            AddMapEntry(new Color(31, 31, 50));
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<MagicCrystal>())
            ];
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<BasaltTile>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}
