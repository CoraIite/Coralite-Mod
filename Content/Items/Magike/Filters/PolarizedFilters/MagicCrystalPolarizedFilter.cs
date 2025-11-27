using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
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
    public class MagicCrystalPolarizedFilter : PolarizedFilterItem
    {
        public MagicCrystalPolarizedFilter() : base(Item.sellPrice(0, 0, 10), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient<Basalt>()
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class MagicCrystalPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => CrystalLevel.ID;

        public override int ItemType => ModContent.ItemType<MagicCrystalPolarizedFilter>();
    }
}
