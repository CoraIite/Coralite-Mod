using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class HallowPolarizedFilter : PolarizedFilterItem
    {
        public HallowPolarizedFilter() : base(Item.sellPrice(0, 0, 60), ItemRarityID.Pink)
        {
        }

        public override MagikeFilter GetFilterComponent() => new HallowPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagikePolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 2)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class HallowPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => HallowLevel.ID;

        public override int ItemType => ModContent.ItemType<HallowPolarizedFilter>();
    }
}
