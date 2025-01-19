using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.TotalReflectionFilters
{
    public class MagicCrystalTRFilter : PackedFilterItem
    {
        public override Color FilterColor => Coralite.MagicCrystalPink;

        public MagicCrystalTRFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalTRFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.MarbleBlock, 3)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.GraniteBlock, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class MagicCrystalTRFilterComponent : TotalReflectionFilter
    {
        public override MALevel Level => MALevel.MagicCrystal;

        public override int ItemType => ModContent.ItemType<MagicCrystalTRFilter>();

        public override int MagikeBonus => 10;
    }
}
