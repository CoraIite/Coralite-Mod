using Coralite.Content.Raritys;
using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class MagicCrystalBrick : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MagicCrystalBrickTile>());
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magiteAmount = 10;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystalBlock>(2)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
