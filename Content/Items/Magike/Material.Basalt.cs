using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class Basalt : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BasaltTile>());
        }
    }
}
