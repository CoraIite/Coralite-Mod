﻿using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike
{
    public class BasicFilter() : BaseMaterial(Item.CommonMaxStack, Item.sellPrice(0, 0, 1), ItemRarityID.White, AssetDirectory.MagikeFilters)
    {
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ItemID.Glass, 4)
                .AddIngredient(ItemID.Lens)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .Register();
        }
    }
}
