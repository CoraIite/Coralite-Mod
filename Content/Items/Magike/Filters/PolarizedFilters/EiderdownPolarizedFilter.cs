using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class EiderdownPolarizedFilter : PolarizedFilterItem
    {
        public EiderdownPolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Blue)
        {
        }

        public override MagikeFilter GetFilterComponent() => new EiderdownPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.Feather, 2)
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class EiderdownPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => EiderdownLevel.ID;

        public override int ItemType => ModContent.ItemType<EiderdownPolarizedFilter>();
    }
}
