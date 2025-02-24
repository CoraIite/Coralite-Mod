using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Helpers;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineBrick : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystallineBrickTile>());
            Item.GetMagikeItem().magikeAmount = 200;
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<CrystallineBlock>(2)
            //    .AddTile(TileID.HeavyWorkBench)
            //    .Register();
        }
    }
}
