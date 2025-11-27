using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class PelagicPolarizedFilter : PolarizedFilterItem
    {
        public PelagicPolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.LightRed)
        {
        }

        public override MagikeFilter GetFilterComponent() => new PelagicPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SeashorePolarizedFilter>()
                .AddIngredient(ItemID.SoulofLight)
                .AddIngredient(ItemID.SharkFin)
                .AddTile(TileID.WorkBenches)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class PelagicPolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => PelagicLevel.ID;

        public override int ItemType => ModContent.ItemType<PelagicPolarizedFilter>();
    }

}
