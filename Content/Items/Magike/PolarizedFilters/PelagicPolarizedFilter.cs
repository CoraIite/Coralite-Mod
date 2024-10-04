using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class PelagicPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.DeepSkyBlue;

        public PelagicPolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.LightRed)
        {
        }

        public override MagikeFilter GetFilterComponent() => new PelagicPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SeashorePolarizedFilter>()
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SharkFin)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class PelagicPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Pelagic;

        public override int ItemType => ModContent.ItemType<PelagicPolarizedFilter>();
    }

}
