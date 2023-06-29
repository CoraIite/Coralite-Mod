using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria.ModLoader;
using Terraria;

namespace Coralite.Content.Items.Magike
{
    public class MagikeCrystalBlock : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MagikeCrystalBlockTile>());
        }

    }
}
