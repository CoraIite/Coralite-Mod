using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class CrystallineMagikePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.Instance.CrystallineMagikePurple;

        public CrystallineMagikePolarizedFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<CrystallineMagikeRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new CrystallineMagikePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient<Skarn>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<CrystallineMagike>(4)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(4)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(4)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient<Skarn>(2)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<CrystallineMagike>(5)
                .AddIngredient<Skarn>(3)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class CrystallineMagikePolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel Level => MagikeApparatusLevel.CrystallineMagike;

        public override int ItemType => ModContent.ItemType<CrystallineMagikePolarizedFilter>();
    }
}
