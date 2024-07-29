using Coralite.Content.Items.Glistent;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class GlistentPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.Instance.GlistentGreen;

        public GlistentPolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.Green)
        {
        }

        public override MagikeFilter GetFilterComponent() => new GlistentPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<GlistentBar>(6)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<GlistentBar>(4)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();
        }
    }

    public class GlistentPolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel UpgradeLevel => MagikeApparatusLevel.Glistent;

        public override int ItemType => ModContent.ItemType<GlistentPolarizedFilter>();
    }
}
