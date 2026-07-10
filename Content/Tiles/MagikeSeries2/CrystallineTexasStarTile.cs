using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineTexasStarTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public const int Random = 3;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(1, 2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.AnchorBottom = new Terraria.DataStructures.AnchorData(Terraria.Enums.AnchorType.SolidTile | Terraria.Enums.AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);

            TileObjectData.newTile.AnchorInvalidTiles = [TileID.Cloud, ModContent.TileType<SkarnBrickTile>(), ModContent.TileType<CrystallineBrickTile>(), ModContent.TileType<CrystallineBlockTile>(), ModContent.TileType<CrystallineBarrier>()];

            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.Grass;
            DustType = ModContent.DustType<CrystallineTexasStarDust>();
            AddMapEntry(new Color(255, 244, 183));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.6f;
            g = 0.55f;
            b = 0.35f;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer)
                return;

            Tile t = Framing.GetTileSafely(i, j);
            if (t.TileFrameY < 19 && Main.rand.NextBool(50))
            {
                Dust.NewDustPerfect(new Vector2(i * 16 + 8, j * 16 + 8), ModContent.DustType<CrystallineTexasStarDust>(), new Vector2(Main.WindForVisuals + Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-0.5f, 1)), 0, Color.White, Main.rand.NextFloat(1, 1.2f));
            }
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [new Item(ModContent.ItemType<CrystallineTexasStar>())];
        }
    }
}
