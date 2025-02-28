using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class BrokenLens : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public const int Random = 3;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights =
            [
                16,18
            ];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            if (UseRandom)
                TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
            RegisterItemDrop(ModContent.ItemType<MagicCrystal>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.7f;
            g = 0.45f;
            b = 0.65f;
        }
    }

    public class BrokenLensFake : BrokenLens
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(BrokenLens);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<MagicCrystal>(), Type, 0, 1, 2);
        }
    }
}
