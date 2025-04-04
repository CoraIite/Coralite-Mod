﻿using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class MagicalPowder : BaseMaterial, IMagikeCraftable
    {
        public MagicalPowder() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Green, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ModContent.ItemType<MagicalPowder>(), ItemID.EnchantedSword, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 3, 120)
                , MainItenStack: 40)
                .RegisterNewCraft(ItemID.FallenStar, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 1, 20)).SetMainStack(5)
                .RegisterNewCraft(ItemID.ManaCrystal, MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 3, 20)).SetMainStack(25)
                .Register();
        }
    }
}
