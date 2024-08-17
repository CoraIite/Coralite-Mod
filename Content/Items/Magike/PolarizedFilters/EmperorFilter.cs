using Coralite.Content.Items.Gels;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class EmperorFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.DeepSkyBlue;

        public EmperorFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Green)
        {
        }

        public override MagikeFilter GetFilterComponent() => new EmperorFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<EmperorGel>(8)
                .DisableDecraft()
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }

    public class EmperorFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel Level => MagikeApparatusLevel.Emperor;

        public override int ItemType => ModContent.ItemType<EmperorFilter>();
    }

}
