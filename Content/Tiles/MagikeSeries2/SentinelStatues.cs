using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SentinelStatuesMain : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = false;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = [16,16,16,16, 18];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 5);
            TileObjectData.addTile(Type);

            MinPick = 200;

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = ModContent.DustType<SkarnDust>();
            AddMapEntry(new Color(141, 171, 178));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {

            }
        }
    }

    public class SentinelStatues4x2 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 1);
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

    public class SentinelStatues4x2Fake : SentinelStatues4x2
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(SentinelStatues4x2);

        public override void SetStaticDefaults()
        {
            DefaultValues();

            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<SkarnBrick>(), Type, 0);
            RegisterItemDrop(ModContent.ItemType<SkarnBrick>());
        }
    }

    public class SentinelStatues3x2 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 1);
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

    public class SentinelStatues3x2Fake : SentinelStatues3x2
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(SentinelStatues3x2);

        public override void SetStaticDefaults()
        {
            DefaultValues();

            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<SkarnBrick>(), Type, 0);
            RegisterItemDrop(ModContent.ItemType<SkarnBrick>());
        }
    }

    public class SentinelStatues3x4 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 3);
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

    public class SentinelStatues3x4Fake : SentinelStatues3x4
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(SentinelStatues3x4);

        public override void SetStaticDefaults()
        {
            DefaultValues();

            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<SkarnBrick>(), Type, 0);
            RegisterItemDrop(ModContent.ItemType<SkarnBrick>());
        }
    }

    public class SentinelStatues4x5 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 18];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 4);
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

    public class SentinelStatues4x5Fake : SentinelStatues4x5
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(SentinelStatues4x5);

        public override void SetStaticDefaults()
        {
            DefaultValues();

            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<SkarnBrick>(), Type, 0, 1);
            RegisterItemDrop(ModContent.ItemType<SkarnBrick>());
        }
    }

    public class SentinelStatues4x6 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
        }

        protected void DefaultValues()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 18];
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 5);
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

    public class SentinelStatues4x6Fake : SentinelStatues4x6
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(SentinelStatues4x6);

        public override void SetStaticDefaults()
        {
            DefaultValues();

            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<SkarnBrick>(), Type, 0);
            RegisterItemDrop(ModContent.ItemType<SkarnBrick>());
        }
    }
}
