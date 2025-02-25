using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class CrystalStalactiteTop : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default;
            TileObjectData.newTile.CoordinateHeights = new int[1] { 24 };
            TileObjectData.newTile.StyleWrapLimit = 4;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[1]
            {
                new(ModContent.ItemType<MagicCrystal>())
            };
        }

    }

    public class CrystalStalactiteBottom : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = new int[1] { 24 };
            TileObjectData.newTile.DrawYOffset = -6;
            TileObjectData.newTile.StyleWrapLimit = 4;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[1]
            {
                new(ModContent.ItemType<MagicCrystal>())
            };
        }
    }

    public class CrystalStalactiteLeft : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
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
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[1]
            {
                new(ModContent.ItemType<MagicCrystal>())
            };
        }

    }

    public class CrystalStalactiteRight : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
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
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[1]
            {
                new(ModContent.ItemType<MagicCrystal>())
            };
        }

    }

    public class BigCrystalStalactiteTop : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
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
            TileObjectData.newTile.RandomStyleRange = 6;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[1]
            {
                new(ModContent.ItemType<MagicCrystal>())
            };
        }

    }

    public class BigCrystalStalactiteBottom : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.newTile.StyleWrapLimit = 6;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 6;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = DustID.CrystalSerpent_Pink;
            AddMapEntry(Coralite.MagicCrystalPink);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[1]
            {
                new(ModContent.ItemType<MagicCrystal>())
            };
        }
    }
}
