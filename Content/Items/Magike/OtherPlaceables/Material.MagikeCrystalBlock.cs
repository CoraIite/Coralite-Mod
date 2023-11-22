using Coralite.Content.Raritys;
using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class MagicCrystalBlock : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MagicCrystalBlockTile>());
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magikeAmount = 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<MagicCrystal>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
