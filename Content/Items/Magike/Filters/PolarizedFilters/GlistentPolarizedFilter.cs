using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class GlistentPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.GlistentGreen;

        public GlistentPolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.Green)
        {
        }

        public override MagikeFilter GetFilterComponent() => new GlistentPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<GlistentBar>(2)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<GlistentBar>(2)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class GlistentPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Glistent;

        public override int ItemType => ModContent.ItemType<GlistentPolarizedFilter>();
    }
}
