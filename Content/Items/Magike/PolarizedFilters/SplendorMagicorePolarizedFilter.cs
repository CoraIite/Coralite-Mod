using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class SplendorMagicorePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.SplendorMagicoreLightBlue;

        public SplendorMagicorePolarizedFilter() : base(Item.sellPrice(0, 2), ItemRarityID.Red)
        {
        }

        public override MagikeFilter GetFilterComponent() => new SplendorMagicorePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<HolyLightPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<FeatherPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<SoulPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<HallowPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagikePolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<SplendorMagicore>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SplendorMagicorePolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel Level => MagikeApparatusLevel.SplendorMagicore;

        public override int ItemType => ModContent.ItemType<SplendorMagicorePolarizedFilter>();
    }
}
