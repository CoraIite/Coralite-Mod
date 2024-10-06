using Coralite.Content.Items.Gels;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class EmperorPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.DeepSkyBlue;

        public EmperorPolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Green)
        {
        }

        public override MagikeFilter GetFilterComponent() => new EmperorPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<EmperorGel>(8)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }

    public class EmperorPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Emperor;

        public override int ItemType => ModContent.ItemType<EmperorPolarizedFilter>();
    }
}
