using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria.ModLoader;
using Terraria;
using Coralite.Content.Raritys;
using Terraria.ID;
using Coralite.Helpers;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class MagicCrystalBrick:ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MagicCrystalBrickTile>());
            Item.rare = ModContent.RarityType<MagikeCrystalRarity>();
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
