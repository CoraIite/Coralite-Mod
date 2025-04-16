using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class BrokenColumn : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public const int Random = 3;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            if (UseRandom)
                TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorInvalidTiles = [TileID.Cloud, ModContent.TileType<SkarnBrickTile>(), ModContent.TileType<CrystallineBrickTile>(), ModContent.TileType<CrystallineBlockTile>()];
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = ModContent.DustType<SkarnDust>();
            AddMapEntry(new Color(141, 171, 178));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class BrokenColumnFake : BrokenColumn
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(BrokenColumn);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Skarn>(), Type, 0, 1, 2);
            RegisterItemDrop(ModContent.ItemType<Skarn>());
        }
    }

    public class BrokenLaser : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public const int Random = 3;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            if (UseRandom)
                TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.AnchorInvalidTiles = [TileID.Cloud, ModContent.TileType<SkarnBrickTile>(), ModContent.TileType<CrystallineBrickTile>(), ModContent.TileType<CrystallineBlockTile>()];
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = ModContent.DustType<SkarnDust>();
            AddMapEntry(new Color(141, 171, 178));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class BrokenLaserFake : BrokenLaser
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(BrokenLaser);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Skarn>(), Type, 0, 1, 2);
            RegisterItemDrop(ModContent.ItemType<Skarn>());
        }
    }
}
