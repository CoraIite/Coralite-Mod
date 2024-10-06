using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class QuicksandPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.SandyBrown;

        public QuicksandPolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new QuicksandPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.SandBlock, 20)
                .AddIngredient(ItemID.Bone, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class QuicksandPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Quicksand;

        public override int ItemType => ModContent.ItemType<QuicksandPolarizedFilter>();
    }
}
