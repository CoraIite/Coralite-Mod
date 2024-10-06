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

namespace Coralite.Content.Items.Magike.Filters.PulseFilters
{
    public class MagicCrystalPulseFilter : PackedFilterItem, IMagikeCraftable
    {
        public override Color FilterColor => Coralite.MagicCrystalPink;

        public MagicCrystalPulseFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalPulseFilterComponent();

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<MagicCrystalPolarizedFilter, MagicCrystalPulseFilter>(MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 6, 60 * 4))
                .AddIngredient<MagicCrystal>(6)
                .AddIngredient(ItemID.SilverBar, 8)
                .Register();

            MagikeCraftRecipe.CreateRecipe<MagicCrystalPolarizedFilter, MagicCrystalPulseFilter>(MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 6, 60 * 4))
                .AddIngredient<MagicCrystal>(6)
                .AddIngredient(ItemID.TungstenBar, 8)
                .Register();
        }
    }

    public class MagicCrystalPulseFilterComponent : PulseFilter
    {
        public override MALevel Level => MALevel.MagicCrystal;

        public override int ItemType => ModContent.ItemType<MagicCrystalPulseFilter>();

        public override int TimerBonus => 10;
    }
}
