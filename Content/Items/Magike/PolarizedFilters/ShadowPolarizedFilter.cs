using Coralite.Content.Items.Shadow;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class ShadowPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.Instance.ShadowPurple;

        public ShadowPolarizedFilter() : base(Item.sellPrice(0, 0, 40), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new ShadowPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient<ShadowEnergy>(10)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient<ShadowEnergy>(4)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient<ShadowEnergy>(4)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient<ShadowEnergy>(4)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient<ShadowEnergy>(6)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient<ShadowEnergy>(8)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ShadowPolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel UpgradeLevel => MagikeApparatusLevel.Shadow;

        public override int ItemType => ModContent.ItemType<ShadowPolarizedFilter>();
    }
}
