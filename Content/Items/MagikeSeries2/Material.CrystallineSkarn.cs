using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineSkarn : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystallineSkarnTile>());
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineMagike>()
                .AddIngredient<Skarn>()
                .AddTile(TileID.HeavyWorkBench)
                .AddCondition(Condition.InGraveyard)
                .Register();
        }

    }
}
