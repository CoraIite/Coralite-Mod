using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class HellstonePolarizedFilter : PolarizedFilterItem
    {
        public HellstonePolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new HellstonePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.HellstoneBar, 2)
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class HellstonePolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => HellstoneLevel.ID;

        public override int ItemType => ModContent.ItemType<HellstonePolarizedFilter>();
    }
}
