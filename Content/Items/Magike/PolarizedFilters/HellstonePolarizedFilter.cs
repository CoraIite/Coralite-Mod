using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class HellstonePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.Firebrick;

        public HellstonePolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new HellstonePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.HellstoneBar, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HellstonePolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Hellstone;

        public override int ItemType => ModContent.ItemType<HellstonePolarizedFilter>();
    }
}
