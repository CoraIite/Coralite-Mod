using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class EmperorPolarizedFilter : PolarizedFilterItem
    {
        //public override Color FilterColor => Color.DeepSkyBlue;

        public EmperorPolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Green)
        {
        }

        public override MagikeFilter GetFilterComponent() => new EmperorPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<EmperorGel>(2)
                .AddTile(TileID.Solidifier)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class EmperorPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => EmperorLevel.ID;

        public override int ItemType => ModContent.ItemType<EmperorPolarizedFilter>();
    }
}
