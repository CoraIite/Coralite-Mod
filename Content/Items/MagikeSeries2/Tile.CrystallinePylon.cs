using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Terraria;
using Terraria.Enums;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallinePylon : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;
        public override void SetDefaults()
        {
            // Basically, this a just a shorthand method that will set all default values necessary to place
            // the passed in tile type; in this case, the Example Pylon tile.
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystallinePylonTile>());

            // Another shorthand method that will set the rarity and how much the item is worth.
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 10));
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }
    }
}
