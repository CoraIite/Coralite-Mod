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
    public class BloodJadePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.RedJadeRed;

        public BloodJadePolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Pink)
        {
        }

        public override MagikeFilter GetFilterComponent() => new BloodJadePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJadePolarizedFilter>()
                .AddIngredient<BloodJade>(3)
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class BloodJadePolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.BloodJade;

        public override int ItemType => ModContent.ItemType<BloodJadePolarizedFilter>();
    }
}
