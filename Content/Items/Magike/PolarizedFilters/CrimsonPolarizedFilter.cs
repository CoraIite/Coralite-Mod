using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class CrimsonPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.CrimsonRed;

        public CrimsonPolarizedFilter() : base(Item.sellPrice(0, 0, 30), ItemRarityID.Blue)
        {
        }

        public override MagikeFilter GetFilterComponent() => new CrimsonPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.TissueSample, 5)
                .AddIngredient(ItemID.CrimtaneBar, 8)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient(ItemID.TissueSample, 3)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.TissueSample, 4)
                .AddIngredient(ItemID.CrimtaneBar, 6)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CrimsonPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Crimson;

        public override int ItemType => ModContent.ItemType<CrimsonPolarizedFilter>();
    }
}
