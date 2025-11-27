using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class SplendorMagicorePolarizedFilter : PolarizedFilterItem
    {
        public SplendorMagicorePolarizedFilter() : base(Item.sellPrice(0, 2), ItemRarityID.Red)
        {
        }

        public override MagikeFilter GetFilterComponent() => new SplendorMagicorePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<HolyLightPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<FeatherPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<SoulPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<HallowPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagikePolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<SplendorMagicore>()
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class SplendorMagicorePolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => SplendorLevel.ID;

        public override int ItemType => ModContent.ItemType<SplendorMagicorePolarizedFilter>();
    }
}
