using Coralite.Content.Tiles.Icicle;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class BabyIceDragonTrophy : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BabyIceDragonTrophyTile>());

            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 1);
        }
    }
}