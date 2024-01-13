using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class MercuryDroplight : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MercuryDroplightTile>());
        }
    }
}
