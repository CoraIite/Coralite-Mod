using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration.MagikeShrineDatas
{
#if DEBUG
    public class BrokenLensPlacer : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.placeStyle = 0;
            Item.createTile = ModContent.TileType<BrokenLensUp>();
        }
    }
#endif
}
