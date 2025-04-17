using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineStalactite2x2 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public const int Random = 2;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.AnchorBottom = Terraria.DataStructures.AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = new Terraria.DataStructures.AnchorData(Terraria.Enums.AnchorType.SolidBottom | Terraria.Enums.AnchorType.SolidTile, 2, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            if (UseRandom)
                TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorInvalidTiles = [
                TileID.Cloud,
                TileID.RainCloud,
                ModContent.TileType<SkarnBrickTile>(),
                ModContent.TileType<ChalcedonyTile>(),
                ModContent.TileType<LeafChalcedonyTile>(),
                ];
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = ModContent.DustType<CrystallineDust>();

            MinPick = 110;

            AddMapEntry(Coralite.CrystallinePurple);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool CanExplode(int i, int j) => CoraliteWorld.HasPermission;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<CrystallineMagike>(),2)
            ];
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.016f;
            g = 0.06f;
            b = 0.2f;
        }
    }

    public class CrystallineStalactite2x2Fake : CrystallineStalactite2x2
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(CrystallineStalactite2x2);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementMedium.AddVariations(ModContent.ItemType<CrystallineMagike>(), Type, 0, 1);
        }
    }

    public class CrystallineStalactite : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public const int Random = 4;

        public override void SetStaticDefaults()
        {
            DefaultValues(true);
        }

        protected void DefaultValues(bool UseRandom)
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorBottom = Terraria.DataStructures.AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = new Terraria.DataStructures.AnchorData(Terraria.Enums.AnchorType.SolidBottom | Terraria.Enums.AnchorType.SolidTile, 2, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.CoordinateHeights = [18];
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            if (UseRandom)
                TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorInvalidTiles = [
                TileID.Cloud,
                TileID.RainCloud,
                ModContent.TileType<SkarnBrickTile>(),
                ModContent.TileType<ChalcedonyTile>(),
                ModContent.TileType<LeafChalcedonyTile>(),
                ];
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            DustType = ModContent.DustType<CrystallineDust>();
            AddMapEntry(Coralite.CrystallinePurple);

            MinPick = 110;

            RegisterItemDrop(ModContent.ItemType<CrystallineMagike>());
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool CanExplode(int i, int j) => CoraliteWorld.HasPermission;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.016f;
            g = 0.06f;
            b = 0.2f;
        }
    }

    public class CrystallineStalactiteFake : CrystallineStalactite
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(CrystallineStalactite);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<CrystallineMagike>(), Type, 0, 1, 2, 3);
        }
    }
}
