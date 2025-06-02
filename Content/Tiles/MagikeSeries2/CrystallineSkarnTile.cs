using Coralite.Content.Dusts;
using Coralite.Content.Items.LandOfTheLustrousSeries;
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

            Main.tileLighted[Type] = true;

            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1000;

            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            //Main.tileMerge[Type][ModContent.TileType<BasaltTile>()] = true;

            //Main.tileMerge[Type][TileID.Stone] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            DustType = ModContent.DustType<CrystallineDust>();
            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            AddMapEntry(Coralite.CrystallinePurple);

            MinPick = 110;
            MineResist = 3;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            if (Main.rand.NextBool(15, 100))
                return
                [
                    new Item(ModContent.ItemType<CrystallineMagike>()),
                    new Item(ModContent.ItemType<SeniorRoughGemstone>())
                ];

            return
            [
                new Item(ModContent.ItemType<CrystallineMagike>())
            ];
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.08f;
            g = 0.03f;
            b = 0.1f;
        }

        public override bool CanExplode(int i, int j) => false;

        //public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        //{
        //    TileFraming.CustomMergeFrame(i, j, Type, ModContent.TileType<SkarnTile>(), true, true, false);
        //    return false;
        //}
    }
}
