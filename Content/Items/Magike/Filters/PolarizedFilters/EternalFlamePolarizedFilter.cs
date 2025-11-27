using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class EternalFlamePolarizedFilter : PolarizedFilterItem
    {
        public EternalFlamePolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new EternalFlamePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellstonePolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .AddIngredient(ItemID.LivingFireBlock)
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class EternalFlamePolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => EternalFlameLevel.ID;

        public override int ItemType => ModContent.ItemType<EternalFlamePolarizedFilter>();
    }
}
