using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PulseFilters
{
    public class MagicCrystalPulseFilter : PackedFilterItem, IMagikeCraftable
    {
        public MagicCrystalPulseFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalPulseFilterComponent();

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<MagicCrystalPolarizedFilter, MagicCrystalPulseFilter>(MagikeHelper.CalculateMagikeCost<CrystalLevel>(3, 60))
                .AddIngredient<MagicCrystal>(6)
                .AddIngredient(ItemID.SilverBar, 5)
                .Register();

            MagikeRecipe.CreateCraftRecipe<MagicCrystalPolarizedFilter, MagicCrystalPulseFilter>(MagikeHelper.CalculateMagikeCost<CrystalLevel>(3, 60))
                .AddIngredient<MagicCrystal>(6)
                .AddIngredient(ItemID.TungstenBar, 5)
                .Register();
        }
    }

    public class MagicCrystalPulseFilterComponent : PulseFilter
    {
        public override ushort Level => CrystalLevel.ID;

        public override int ItemType => ModContent.ItemType<MagicCrystalPulseFilter>();

        public override int TimerBonus => 10;
    }
}
