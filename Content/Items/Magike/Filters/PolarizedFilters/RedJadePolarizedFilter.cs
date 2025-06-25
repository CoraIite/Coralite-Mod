using Coralite.Content.Items.RedJades;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class RedJadePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.RedJadeRed;

        public RedJadePolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.Blue)
        {
        }

        public override MagikeFilter GetFilterComponent() => new RedJadePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<RedJade>(2)
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class RedJadePolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.RedJade;

        public override int ItemType => ModContent.ItemType<RedJadePolarizedFilter>();
    }
}
