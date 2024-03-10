using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinDragonTrophy : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Thunder.ThunderveinDragonTrophyTile>());

            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(0, 1);
        }
    }
}