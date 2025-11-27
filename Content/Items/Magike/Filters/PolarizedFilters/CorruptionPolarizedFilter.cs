using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class CorruptionPolarizedFilter : PolarizedFilterItem
    {
        public CorruptionPolarizedFilter() : base(Item.sellPrice(0, 0, 30), ItemRarityID.Blue)
        {
        }

        public override MagikeFilter GetFilterComponent() => new CorruptionPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.ShadowScale, 2)
                .AddIngredient(ItemID.DemoniteBar)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient(ItemID.ShadowScale, 2)
                .AddIngredient(ItemID.DemoniteBar)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.ShadowScale, 2)
                .AddIngredient(ItemID.DemoniteBar)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class CorruptionPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => CorruptionLevel.ID;

        public override int ItemType => ModContent.ItemType<CorruptionPolarizedFilter>();
    }
}
