using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class EternalFlamePolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.OrangeRed;

        public EternalFlamePolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new EternalFlamePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellstonePolarizedFilter>()
                .AddIngredient(ItemID.HallowedBar, 6)
                .AddIngredient(ItemID.LivingFireBlock, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class EternalFlamePolarizedFilterComponent : PolarizedFilter
    {
        public override MagikeApparatusLevel Level => MagikeApparatusLevel.EternalFlame;

        public override int ItemType => ModContent.ItemType<EternalFlamePolarizedFilter>();
    }
}
