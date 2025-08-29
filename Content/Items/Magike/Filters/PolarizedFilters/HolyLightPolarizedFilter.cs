using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class HolyLightPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Main.DiscoColor;

        public HolyLightPolarizedFilter() : base(Item.sellPrice(0, 1), ItemRarityID.Cyan)
        {
        }

        public override MagikeFilter GetFilterComponent() => new HolyLightPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<FeatherPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<SoulPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<HallowPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagikePolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class HolyLightPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.HolyLight;

        public override int ItemType => ModContent.ItemType<HolyLightPolarizedFilter>();
    }
}
