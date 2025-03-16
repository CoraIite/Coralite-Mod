using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class CrystallineMagikePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.CrystallinePurple;

        public CrystallineMagikePolarizedFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<CrystallineMagikeRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new CrystallineMagikePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<CrystallineMagike>(3)
                .AddIngredient<Skarn>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(1)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<CrystallineMagike>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(2)
                .AddIngredient<Skarn>(1)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(3)
                .AddIngredient<Skarn>(1)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class CrystallineMagikePolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.CrystallineMagike;

        public override int ItemType => ModContent.ItemType<CrystallineMagikePolarizedFilter>();
    }
}
