using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.PolarizedFilters
{
    public class ForbiddenPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.Gold;

        public ForbiddenPolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.LightRed)
        {
        }

        public override MagikeFilter GetFilterComponent() => new ForbiddenPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<QuicksandPolarizedFilter>()
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ForbiddenPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Forbidden;

        public override int ItemType => ModContent.ItemType<ForbiddenPolarizedFilter>();
    }
}
