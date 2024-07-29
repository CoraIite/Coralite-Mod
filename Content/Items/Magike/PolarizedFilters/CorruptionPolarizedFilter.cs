using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class CorruptionPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.Instance.CorruptionPurple;

        public CorruptionPolarizedFilter() : base(Item.sellPrice(0, 0, 30), ItemRarityID.Blue)
        {
        }

        public override MagikeFilter GetFilterComponent() => new CorruptionPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddIngredient(ItemID.DemoniteBar, 8)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient(ItemID.ShadowScale, 3)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.ShadowScale, 4)
                .AddIngredient(ItemID.DemoniteBar, 6)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CorruptionPolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel UpgradeLevel => MagikeApparatusLevel.Corruption;

        public override int ItemType => ModContent.ItemType<CorruptionPolarizedFilter>();
    }
}
