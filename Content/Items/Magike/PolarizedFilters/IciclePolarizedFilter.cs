using Coralite.Content.Items.Icicle;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class IciclePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.IcicleCyan;

        public IciclePolarizedFilter() : base(Item.sellPrice(0, 0, 30), ItemRarityID.Blue)
        {
        }

        public override MagikeFilter GetFilterComponent() => new IciclePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<IcicleCrystal>()
                .AddIngredient<IcicleBreath>(2)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<IcicleCrystal>()
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<IcicleCrystal>()
                .AddIngredient<IcicleBreath>()
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class IciclePolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel Level => MagikeApparatusLevel.Icicle;

        public override int ItemType => ModContent.ItemType<IciclePolarizedFilter>();
    }
}
