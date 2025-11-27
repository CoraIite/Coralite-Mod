using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class GlistentPolarizedFilter : PolarizedFilterItem
    {
        public GlistentPolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.Green)
        {
        }

        public override MagikeFilter GetFilterComponent() => new GlistentPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<GlistentBar>(2)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<GlistentBar>(2)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class GlistentPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => GlistentLevel.ID;

        public override int ItemType => ModContent.ItemType<GlistentPolarizedFilter>();
    }
}
