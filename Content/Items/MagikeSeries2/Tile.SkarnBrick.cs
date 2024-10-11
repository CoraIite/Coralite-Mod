using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnBrick:ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SkarnBrickTile>());
        }
    }
}
