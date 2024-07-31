using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class SoulPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Coralite.Instance.SoulCyan;

        public SoulPolarizedFilter() : base(Item.sellPrice(0, 0, 70), ItemRarityID.Yellow)
        {
        }

        public override MagikeFilter GetFilterComponent() => new SoulPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.Ectoplasm, 8)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<HallowPolarizedFilter>()
                .AddIngredient(ItemID.Ectoplasm, 3)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagikePolarizedFilter>()
                .AddIngredient(ItemID.Ectoplasm, 4)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<ShadowPolarizedFilter>()
                .AddIngredient(ItemID.Ectoplasm, 5)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient(ItemID.Ectoplasm, 6)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CorruptionPolarizedFilter>()
                .AddIngredient(ItemID.Ectoplasm, 6)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<CrimsonPolarizedFilter>()
                .AddIngredient(ItemID.Ectoplasm, 6)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<GlistentPolarizedFilter>()
                .AddIngredient(ItemID.Ectoplasm, 7)
                .AddTile(TileID.MythrilAnvil)
                .DisableDecraft()
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystalPolarizedFilter>()
                .AddIngredient(ItemID.Ectoplasm, 8)
                .DisableDecraft()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SoulPolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel Level => MagikeApparatusLevel.Soul;

        public override int ItemType => ModContent.ItemType<SoulPolarizedFilter>();
    }
}
