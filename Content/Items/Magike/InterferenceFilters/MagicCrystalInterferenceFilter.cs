using Coralite.Content.Items.Magike.PolarizedFilters;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.InterferenceFilters
{
    public class MagicCrystalInterferenceFilter : PackedFilterItem
    {
        public override Color FilterColor => Coralite.MagicCrystalPink;

        public MagicCrystalInterferenceFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalInterferenceFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.GlassPlatform, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MagicCrystalInterferenceFilterComponent : InterferenceFilter
    {
        public override MALevel Level => MALevel.MagicCrystal;

        public override int ItemType => ModContent.ItemType<MagicCrystalInterferenceFilter>();

        public override int UnitDeliveryBonus => 10;
    }
}
