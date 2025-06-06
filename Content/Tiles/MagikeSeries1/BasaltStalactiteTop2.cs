﻿using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class BasaltStalactiteTop2 : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

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

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default;
            TileObjectData.newTile.StyleWrapLimit = 3;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            if (UseRandom)
                TileObjectData.newTile.RandomStyleRange = Random;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = DustID.CorruptionThorns;
            AddMapEntry(new Color(31, 31, 50));
        }
    }

    public class BasaltStalactiteTop2Fake : BasaltStalactiteTop2
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(BasaltStalactiteTop2);

        public override void SetStaticDefaults()
        {
            DefaultValues(false);

            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Basalt>(), Type, 0, 1, 2);
            RegisterItemDrop(ModContent.ItemType<Basalt>());
        }
    }
}
