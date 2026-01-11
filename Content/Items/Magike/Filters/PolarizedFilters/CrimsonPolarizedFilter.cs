using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class CrimsonPolarizedFilter : PolarizedFilterItem
    {
        //public override Color FilterColor => Coralite.CrimsonRed;

        public CrimsonPolarizedFilter() : base(Item.sellPrice(0, 0, 30), ItemRarityID.Blue)
        {
        }

        public override MagikeFilter GetFilterComponent() => new CrimsonPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.TissueSample, 2)
                .AddIngredient(ItemID.CrimtaneBar)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient(ItemID.TissueSample, 2)
                .AddIngredient(ItemID.CrimtaneBar)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.TissueSample, 2)
                .AddIngredient(ItemID.CrimtaneBar)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class CrimsonPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => CrimsonLevel.ID;

        public override int ItemType => ModContent.ItemType<CrimsonPolarizedFilter>();
    }
}
