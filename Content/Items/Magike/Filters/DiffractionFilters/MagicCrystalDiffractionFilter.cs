﻿using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike.Filters.DiffractionFilters
{
    public class MagicCrystalDiffractionFilter : PackedFilterItem, IMagikeCraftable
    {
        public override Color FilterColor => Coralite.MagicCrystalPink;

        public MagicCrystalDiffractionFilter() : base(Item.sellPrice(0, 0, 50), ModContent.RarityType<MagicCrystalRarity>())
        {
        }

        public override MagikeFilter GetFilterComponent() => new MagicCrystalDiffractionFilterComponent();

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<MagicCrystalPolarizedFilter, MagicCrystalDiffractionFilter>(MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 5, 60 * 2))
                .AddIngredient<Basalt>(10)
                .AddIngredient<MagicCrystal>(8)
                .AddIngredient(ItemID.Glass, 4)
                .Register();
        }
    }

    public class MagicCrystalDiffractionFilterComponent : DiffractionFilter
    {
        public override MALevel Level => MALevel.MagicCrystal;

        public override int ItemType => ModContent.ItemType<MagicCrystalDiffractionFilter>();

        public override byte ConnectMaxBonus => 1;
    }
}
