using Coralite.Content.Items.Materials;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
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
                .AddIngredient<FragmentsOfLight>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<FeatherPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<SoulPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<HallowPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagikePolarizedFilter>()
                .AddIngredient<FragmentsOfLight>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<FragmentsOfLight>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>(2)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<FragmentsOfLight>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class HolyLightPolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel Level => MagikeApparatusLevel.HolyLight;

        public override int ItemType => ModContent.ItemType<HolyLightPolarizedFilter>();
    }
}
