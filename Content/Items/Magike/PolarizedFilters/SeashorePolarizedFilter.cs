using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
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
                .AddIngredient(ItemID.Coral, 5)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.Seashell, 4)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.Starfish, 3)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.TulipShell, 2)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.LightningWhelkShell, 2)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.JunoniaShell)
                .DisableDecraft()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class SeashorePolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Seashore;

        public override int ItemType => ModContent.ItemType<SeashorePolarizedFilter>();
    }
}
