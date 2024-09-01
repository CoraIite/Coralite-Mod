using Coralite.Content.Items.ShadowCastle;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class ShadowCastleBannerTile : ModTile
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[3] {
                16,
                16,
                16
             };

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleWrapLimit = 111;
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.DrawYOffset = -10;
            TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Brown);
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            int whoamI = 1 + (tileFrameY / 18);
            Tile tile = Main.tile[i, j - whoamI];
            if (TileID.Sets.Platforms[tile.TileType])
                offsetY = -10;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Item[] items = new Item[1];

            Tile t = Framing.GetTileSafely(i, j);

            items[0] = (t.TileFrameX / 18) switch
            {
                1 => new Item(ModContent.ItemType<GalacticVortexBanner>()),
                2 => new Item(ModContent.ItemType<LunarEclipseBanner>()),
                3 => new Item(ModContent.ItemType<StarRuneBanner>()),
                _ => new Item(ModContent.ItemType<StarlightBanner>()),
            };
            return items;
        }
    }
}
