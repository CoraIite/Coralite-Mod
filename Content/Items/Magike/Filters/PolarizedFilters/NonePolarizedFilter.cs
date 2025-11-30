using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class NonePolarizedFilter : PolarizedFilterItem
    {
        public NonePolarizedFilter() : base(Item.sellPrice(0, 0, 1), ItemRarityID.White)
        {
        }

        public override MagikeFilter GetFilterComponent() => new NonePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.StoneBlock, 2)
                .Register();
        }
    }

    public class NonePolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => NoneLevel.ID;

        public override int ItemType => ModContent.ItemType<NonePolarizedFilter>();
    }
}
