using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class Basalt : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BasaltTile>());
        }
    }
}
