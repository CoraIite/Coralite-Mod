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
    public class BeeswaxPolarizedFilter : PolarizedFilterItem
    {
        //public override Color FilterColor => Color.Honeydew;

        public BeeswaxPolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new BeeswaxPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.BeeWax, 2)
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class BeeswaxPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => BeeswaxLevel.ID;

        public override int ItemType => ModContent.ItemType<BeeswaxPolarizedFilter>();
    }
}
