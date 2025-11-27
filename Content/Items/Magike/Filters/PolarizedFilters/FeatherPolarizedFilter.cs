using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class FeatherPolarizedFilter : PolarizedFilterItem
    {
        public FeatherPolarizedFilter() : base(Item.sellPrice(0, 0, 70), ItemRarityID.Yellow)
        {
        }

        public override MagikeFilter GetFilterComponent() => new FeatherPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<HallowPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagikePolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<FlyingSnakeFeather>(2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class FeatherPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => FeatherLevel.ID;

        public override int ItemType => ModContent.ItemType<FeatherPolarizedFilter>();
    }
}
