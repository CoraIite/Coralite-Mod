using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.ShadowCastle
{
    public class ShadowCandelabra : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadowCandelabraTile>());
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}
