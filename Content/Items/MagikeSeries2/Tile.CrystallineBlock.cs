using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Helpers;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineBlock : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystallineBlockTile>());
            Item.GetMagikeItem().magikeAmount = 100;
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<CrystallineMagike>()
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}