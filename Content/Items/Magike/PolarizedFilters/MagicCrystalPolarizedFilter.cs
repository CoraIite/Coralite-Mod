using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class MagicCrystalPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.MagicCrystalPink;

        public MagicCrystalPolarizedFilter() : base(Item.sellPrice(0, 0, 10), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<MagicCrystal>(6)
                .AddIngredient<Basalt>(4)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();
        }
    }

    public class MagicCrystalPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.MagicCrystal;

        public override int ItemType => ModContent.ItemType<MagicCrystalPolarizedFilter>();
    }
}
