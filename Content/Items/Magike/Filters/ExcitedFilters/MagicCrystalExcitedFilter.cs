using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.ExcitedFilters
{
    public class MagicCrystalExcitedFilter : PackedFilterItem, IMagikeCraftable
    {
        public override Color FilterColor => Coralite.MagicCrystalPink;

        public MagicCrystalExcitedFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalExcitedFilterComponent();

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateRecipe<MagicCrystalPolarizedFilter, MagicCrystalExcitedFilter>(MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 6, 60 * 4))
                .AddIngredient<MagicCrystal>(6)
                .AddIngredient(ItemID.Diamond, 2)
                .Register();
        }
    }

    public class MagicCrystalExcitedFilterComponent : ExcitedFilter
    {
        public override MALevel Level => MALevel.MagicCrystal;

        public override int ItemType => ModContent.ItemType<MagicCrystalExcitedFilter>();

        public override int ProduceBonus => 10;
    }
}
