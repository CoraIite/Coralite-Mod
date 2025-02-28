using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class CrystalStalactiteTop : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public const int Random = 4;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default;
            TileObjectData.newTile.CoordinateHeights = [24];
            TileObjectData.newTile.StyleWrapLimit = 4;
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
    }

    public class CrystalStalactiteTopFake : CrystalStalactiteTop
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(CrystalStalactiteTop);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<MagicCrystal>(), Type, 0, 1, 2, 3);
        }
    }

    public class CrystalStalactiteBottom : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public const int Random = 4;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = [24];
            TileObjectData.newTile.DrawYOffset = -6;
            TileObjectData.newTile.StyleWrapLimit = 4;
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
    }

    public class CrystalStalactiteBottomFake : CrystalStalactiteBottom
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(CrystalStalactiteBottom);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<MagicCrystal>(), Type, 0, 1, 2, 3);
        }
    }

    public class CrystalStalactiteLeft : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public const int Random = 4;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default;
            TileObjectData.newTile.CoordinateWidth = 24;
            TileObjectData.newTile.StyleWrapLimit = 4;
            TileObjectData.newTile.StyleMultiplier = 1;
            //TileObjectData.newTile.StyleHorizontal = true;
            if (UseRandom)
                TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
            RegisterItemDrop(ModContent.ItemType<MagicCrystal>());
        }
    }

    public class CrystalStalactiteLeftFake : CrystalStalactiteLeft
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(CrystalStalactiteLeft);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<MagicCrystal>(), Type, 0, 1, 2, 3);
        }
    }

    public class CrystalStalactiteRight : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public const int Random = 4;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default;
            TileObjectData.newTile.StyleWrapLimit = 1;
            TileObjectData.newTile.StyleMultiplier = 1;
            //TileObjectData.newTile.StyleHorizontal = true;
            if (UseRandom)
                TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
            RegisterItemDrop(ModContent.ItemType<MagicCrystal>());
        }
    }

    public class CrystalStalactiteRightFake : CrystalStalactiteRight
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(CrystalStalactiteRight);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<MagicCrystal>(), Type, 0, 1, 2, 3);
        }
    }

    public class BigCrystalStalactiteTop : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public const int Random = 6;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default;
            TileObjectData.newTile.StyleWrapLimit = 6;
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
    }

    public class BigCrystalStalactiteTopFake : BigCrystalStalactiteTop
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(BigCrystalStalactiteTop);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<MagicCrystal>(), Type, 0, 1, 2, 3,4,5);
        }
    }

    public class BigCrystalStalactiteBottom : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public const int Random = 6;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.newTile.StyleWrapLimit = 6;
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
    }

    public class BigCrystalStalactiteBottomFake : BigCrystalStalactiteBottom
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(BigCrystalStalactiteBottom);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<MagicCrystal>(), Type, 0, 1, 2, 3, 4, 5);
        }
    }
}
