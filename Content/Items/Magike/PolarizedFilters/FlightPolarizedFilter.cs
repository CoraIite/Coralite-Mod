using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class FlightPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.LightSkyBlue;

        public FlightPolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.LightRed)
        {
        }

        public override MagikeFilter GetFilterComponent() => new FlightPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EiderdownPolarizedFilter>()
                .AddIngredient(ItemID.SoulofFlight, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class FlightPolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel Level => MagikeApparatusLevel.Flight;

        public override int ItemType => ModContent.ItemType<FlightPolarizedFilter>();
    }
}
