using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries1
{
    public class BasaltStalactiteBottom : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.StyleWrapLimit = 3;
            TileObjectData.newTile.StyleMultiplier = 1;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = DustID.CorruptionThorns;
            AddMapEntry(new Color(31, 31, 50));
        }
    }

    public class BasaltStalactiteBottomFake : BasaltStalactiteBottom
    {
        public override string Texture => AssetDirectory.MagikeSeries1Tile + nameof(BasaltStalactiteBottom);

        public override void SetStaticDefaults()
        {
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Basalt>(), Type, 0, 1, 2);
            RegisterItemDrop(ModContent.ItemType<Basalt>());
        }
    }
}
