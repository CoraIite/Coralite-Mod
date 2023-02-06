using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RediancieTrophy : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RediancieTrophyTile>());

            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 1);
        }
    }
}
