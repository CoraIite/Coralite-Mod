using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class ShroomitePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.DarkBlue;

        public ShroomitePolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Yellow)
        {
        }

        public override MagikeFilter GetFilterComponent() => new ShroomitePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.ShroomiteBar, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ShroomitePolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Shroomite;

        public override int ItemType => ModContent.ItemType<ShroomitePolarizedFilter>();
    }
}
