using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class BonePolarizedFilter : PolarizedFilterItem
    {
        //public override Color FilterColor => Color.DimGray;

        public BonePolarizedFilter() : base(Item.sellPrice(0, 0, 20), ItemRarityID.Orange)
        {
        }

        public override MagikeFilter GetFilterComponent() => new BonePolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicFilter>()
                .AddIngredient(ItemID.Bone, 2)
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();
        }
    }

    public class BonePolarizedFilterComponent : PolarizedFilter
    {
        public override ushort Level => BoneLevel.ID;

        public override int ItemType => ModContent.ItemType<BonePolarizedFilter>();
    }
}
