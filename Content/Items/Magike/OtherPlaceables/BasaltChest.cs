using Coralite.Content.Raritys;
using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class BasaltChest : BaseChestItem
    {
        public BasaltChest() : base(Item.sellPrice(0, 0, 0, 10), ModContent.RarityType<MagicCrystalRarity>(), ModContent.TileType<BasaltChestTile>(), AssetDirectory.MagikeItems)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient<Basalt>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
