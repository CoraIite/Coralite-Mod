using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class SeashorePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.SeaShell;

        public SeashorePolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Blue)
        {
        }

        public override MagikeFilter GetFilterComponent() => new SeashorePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.Coral)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.Seashell)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.Starfish)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe(2)
                .AddIngredient<BasicFilter>(2)
                .AddIngredient(ItemID.TulipShell)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe(2)
                .AddIngredient<BasicFilter>(2)
                .AddIngredient(ItemID.LightningWhelkShell)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe(2)
                .AddIngredient<BasicFilter>(2)
                .AddIngredient(ItemID.JunoniaShell)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class SeashorePolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Seashore;

        public override int ItemType => ModContent.ItemType<SeashorePolarizedFilter>();
    }
}
