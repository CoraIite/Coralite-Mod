using Coralite.Content.Items.Shadow;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class ShadowPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.ShadowPurple;

        public ShadowPolarizedFilter() : base(Item.sellPrice(0, 0, 40), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new ShadowPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<ShadowEnergy>(5)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<ShadowEnergy>(2)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<ShadowEnergy>(2)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<ShadowEnergy>(2)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<ShadowEnergy>(3)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<ShadowEnergy>(4)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class ShadowPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Shadow;

        public override int ItemType => ModContent.ItemType<ShadowPolarizedFilter>();
    }
}
