using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class AncientColumn : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            DefaultValues();
            MinPick = 110;
        }

        protected void DefaultValues()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;

            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Width = 9;
            TileObjectData.newTile.Height = 9;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 40];

            TileObjectData.newTile.DrawYOffset = -22;
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 8);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            HitSound = CoraliteSoundID.DigStone_Tink;
            DustType = DustID.Pearlsand;
            AddMapEntry(new Color(141, 171, 178));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
            => [
                new Item(ModContent.ItemType<Skarn>(),12),
                new Item(ModContent.ItemType<CrystallineMagike>(),6),
                ];
    }

    public class AncientColumnFake : AncientColumn
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + nameof(AncientColumn);

        public override void SetStaticDefaults()
        {
            DefaultValues();
            FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Skarn>(), Type, 0);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j) => [new Item(ModContent.ItemType<Skarn>())];
    }
}
