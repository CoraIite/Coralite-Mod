﻿using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.PolarizedFilters
{
    public class FrostPolarizedFilter : PolarizedFilterItem
    {
        public override Color FilterColor => Color.LightSkyBlue;

        public FrostPolarizedFilter() : base(Item.sellPrice(0, 0, 50), ItemRarityID.LightRed)
        {
        }

        public override MagikeFilter GetFilterComponent() => new FrostPolarizedFilterComponent();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IciclePolarizedFilter>()
                .AddIngredient(ItemID.FrostCore)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class FrostPolarizedFilterComponent : PolarizedFilter
    {
        public override MALevel Level => MALevel.Frost;

        public override int ItemType => ModContent.ItemType<FrostPolarizedFilter>();
    }
}
