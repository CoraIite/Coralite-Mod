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

namespace Coralite.Content.Items.Magike.Filters.DiffractionFilters
{
    public class MagicCrystalDiffractionFilter : PackedFilterItem, IMagikeCraftable
    {
        //public override Color FilterColor => Coralite.MagicCrystalPink;

        public MagicCrystalDiffractionFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalDiffractionFilterComponent();

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<MagicCrystalPolarizedFilter, MagicCrystalDiffractionFilter>(MagikeHelper.CalculateMagikeCost<CrystalLevel>( 3, 60))
                .AddIngredient<Basalt>(8)
                .AddIngredient<MagicCrystal>(6)
                .AddIngredient(ItemID.Glass, 4)
                .Register();
        }
    }

    public class MagicCrystalDiffractionFilterComponent : DiffractionFilter
    {
        public override ushort Level => CrystalLevel.ID;

        public override int ItemType => ModContent.ItemType<MagicCrystalDiffractionFilter>();

        public override byte ConnectMaxBonus => 1;
    }
}
