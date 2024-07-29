using Coralite.Content.Items.Materials;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class FeatherPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.Instance.FeatherLime;

        public FeatherPolarizedFilter() : base(Item.sellPrice(0, 0, 70), ItemRarityID.Yellow)
        {
        }

        public override MagikeFilter GetFilterComponent() => new FeatherPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<FlyingSnakeFeather>(8)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<HallowPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(3)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagikePolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(4)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(6)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(6)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(6)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(7)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(8)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class FeatherPolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel UpgradeLevel => MagikeApparatusLevel.Feather;

        public override int ItemType => ModContent.ItemType<FeatherPolarizedFilter>();
    }
}
