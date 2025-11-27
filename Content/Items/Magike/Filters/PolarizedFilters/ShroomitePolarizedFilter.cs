using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class ShroomitePolarizedFilter : PolarizedFilterItem
    {
        public ShroomitePolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Yellow)
        {
        }

        public override MagikeFilter GetFilterComponent() => new ShroomitePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.ShroomiteBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class ShroomitePolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => ShroomiteLevel.ID;

        public override int ItemType => ModContent.ItemType<ShroomitePolarizedFilter>();
    }
}
